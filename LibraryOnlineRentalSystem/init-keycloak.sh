#!/bin/bash
set -e
check_error() {
    if [ $? -ne 0 ]; then
        echo "Error: $1"
        exit 1
    fi
}

echo "Getting admin token..."
ADMIN_TOKEN=$(curl -X POST http://localhost:8080/realms/master/protocol/openid-connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "username=${KEYCLOAK_ADMIN}" \
    -d "password=${KEYCLOAK_ADMIN_PASSWORD}" \
    -d "grant_type=password" \
-d "client_id=admin-cli" | jq -r '.access_token')
check_error "Failed to get admin token"

echo "Updating master realm admin user email and verification status..."


MASTER_ADMIN_ID=$(curl -s -X GET "http://localhost:8080/admin/realms/master/users?username=${KEYCLOAK_ADMIN}" \
-H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

check_error "Failed to get master admin user ID"


curl -X PUT "http://localhost:8080/admin/realms/master/users/${MASTER_ADMIN_ID}" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "email": "examplegogsi@gmail.com",
    "emailVerified": true
}'

check_error "Failed to update admin user in master realm"


echo "Creating realm..."
curl -X POST http://localhost:8080/admin/realms \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "realm": "library",
    "enabled": true,
    "roles": {
      "realm": [
        {
          "name": "Admin",
          "description": "Administrator role"
        },
        {
          "name": "LibraryManager",
          "description": "Library Manager role"
        },
        {
          "name": "User",
          "description": "Regular user role"
        }
      ]
    }
}'
check_error "Failed to create realm"

echo "Waiting for Admin role to be available..."
until curl -H "Authorization: Bearer $ADMIN_TOKEN" \
"http://localhost:8080/admin/realms/library/roles/Admin" | grep -q '"name":"Admin"'; do
    sleep 2
    echo "Still waiting for Admin role..."
done
echo "Admin role is now available."

echo "Creating public client..."
curl -X POST http://localhost:8080/admin/realms/library/clients \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "clientId": "library-client",
    "redirectUris": ["http://localhost:8081/*"],
    "webOrigins": ["http://localhost:8081"],
    "publicClient": true,
    "directAccessGrantsEnabled": true,
    "standardFlowEnabled": true,
    "implicitFlowEnabled": true,
    "protocol": "openid-connect"
}'
check_error "Failed to create client"



echo "Creating admin user..."
curl -X POST http://localhost:8080/admin/realms/library/users \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "username": "admin",
    "email": "examplegogsi@gmail.com",
    "enabled": true,
    "emailVerified": true,
    "credentials": [{
      "type": "password",
      "value": "'${KEYCLOAK_ADMIN_PASSWORD}'",
      "temporary": false
    }]
}'
check_error "Failed to create admin user"

echo "Getting admin user ID..."
ADMIN_USER_ID=$(curl -X GET "http://localhost:8080/admin/realms/library/users?username=admin" \
-H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')
check_error "Failed to get admin user ID"

echo "Fetching Admin role object..."
ADMIN_ROLE=$(curl -H "Authorization: Bearer $ADMIN_TOKEN" \
"http://localhost:8080/admin/realms/library/roles/Admin")
check_error "Failed to fetch Admin role"

echo "Assigning admin role to admin user..."
curl -X POST "http://localhost:8080/admin/realms/library/users/${ADMIN_USER_ID}/role-mappings/realm" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d "[$ADMIN_ROLE]"
check_error "Failed to assign admin role"

echo "Keycloak initialization completed successfully!"



echo "Updating realm settings..."

curl -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "accessTokenLifespan": 7200,
    "verifyEmail": true,
    "smtpServer": {
      "host": "smtp.gmail.com",
      "port": "587",
      "from": "examplegogsi@gmail.com",
      "fromDisplayName": "Library-desofs-3",
      "auth": true,
      "user": "examplegogsi@gmail.com",
      "password": "'${SMTP_PASSWORD}'",
      "ssl": false,
      "starttls": true
    }
}'
check_error "Failed to update realm settings"

echo "Setting password policy..."

REALM_JSON=$(curl -s -H "Authorization: Bearer $ADMIN_TOKEN" \
"http://localhost:8080/admin/realms/library")

echo "Updating password policy..."
UPDATED_JSON=$(echo "$REALM_JSON" | jq '
  .passwordPolicy = "length(12) and maxLength(128) and upperCase(1) and digits(1) and specialChars(1) and notUsername() and notEmail() and passwordHistory(5)"
')

curl -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d "$UPDATED_JSON"

check_error "Failed to set password policy"

echo "Updating brute force..."
curl -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "bruteForceProtected": true,
    "permanentLockout": false,
    "failureFactor": 5,
    "waitIncrementSeconds": 60,
    "maxFailureWaitSeconds": 600,
    "minimumQuickLoginWaitSeconds": 60,
    "maxDeltaTimeSeconds": 43200

}'

echo " Enable event logging"
curl -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "eventsEnabled": true,
    "adminEventsEnabled": true,
    "adminEventsDetailsEnabled": true,
    "eventsListeners": ["jboss-logging"],
    "enabledEventTypes": [
      "LOGIN",
      "LOGIN_ERROR",
      "REGISTER",
      "UPDATE_PROFILE",
      "UPDATE_EMAIL",
      "UPDATE_PASSWORD",
      "SEND_RESET_PASSWORD",
      "SEND_VERIFY_EMAIL",
      "REMOVE_TOTP",
      "VERIFY_EMAIL",
      "DELETE_ACCOUNT"
    ],
    "eventsExpiration": 0  # Never expire events
}'

echo "Updating themes in realm settings..."
UPDATED_JSON=$(echo "$REALM_JSON" | jq '
  .loginTheme = "keycloak" |
  .accountTheme = "keycloak.v3" |
  .adminTheme = "keycloak.v2" |
  .emailTheme = "keycloak"
')

curl -s -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d "$UPDATED_JSON"
check_error "Failed to set realm themes"

echo "Keycloak additional configuration done!"

