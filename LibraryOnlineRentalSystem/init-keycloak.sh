#!/bin/bash
set -e

# Load environment variables from .env file
if [ -f .env ]; then
  set -a
  source .env
  set +a
else
  echo "Missing .env file. Exiting."
  exit 1
fi

check_error() {
    if [ $? -ne 0 ]; then
        echo "Error: $1"
        exit 1
    fi
}

echo "Getting admin token..."
ADMIN_TOKEN=$(curl -ks -X POST https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/realms/master/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=${KEYCLOAK_ADMIN}" \
  -d "password=${KEYCLOAK_ADMIN_PASSWORD}" \
  -d "grant_type=password" \
  -d "client_id=admin-cli" | jq -r '.access_token')
check_error "Failed to get admin token"

echo "Creating realm..."
curl -ks -X POST https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms \
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
until curl -ks -H "Authorization: Bearer $ADMIN_TOKEN" \
  "https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/roles/Admin" | grep -q '"name":"Admin"'; do
    sleep 2
    echo "Still waiting for Admin role..."
done
echo "Admin role is now available."

echo "Creating client..."
curl -ks -X POST https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/clients \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "library-client",
    "secret": "'${KEYCLOAK_CLIENT_SECRET}'",
    "redirectUris": ["http://localhost:8081/*"],
    "webOrigins": ["http://localhost:8081"],
    "publicClient": false,
    "directAccessGrantsEnabled": true,
    "serviceAccountsEnabled": true,
    "authorizationServicesEnabled": true,
    "standardFlowEnabled": true,
    "implicitFlowEnabled": true,
    "protocol": "openid-connect",
    "clientAuthenticatorType": "client-secret"
  }'
check_error "Failed to create client"

echo "Creating admin user..."
curl -ks -X POST https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/users \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "admin@library.com",
    "enabled": true,
    "credentials": [{
      "type": "password",
      "value": "'${KEYCLOAK_ADMIN_PASSWORD}'",
      "temporary": false
    }]
  }'
check_error "Failed to create admin user"

echo "Fetching available required actions..."
AVAILABLE_ACTIONS=$(curl -ks -H "Authorization: Bearer $ADMIN_TOKEN" \
  https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/authentication/required-actions \
  | jq -r '.[] | select(.alias != null) | .alias')

for action in $AVAILABLE_ACTIONS; do
  echo "Disabling required action: $action"
  curl -ks -X PUT "https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/authentication/required-actions/$action" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" \
    -d '{"enabled": false}'
done

echo "Getting admin user ID..."
ADMIN_USER_ID=$(curl -ks -X GET "https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/users?username=admin" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')
check_error "Failed to get admin user ID"

echo "Fetching Admin role object..."
ADMIN_ROLE=$(curl -ks -H "Authorization: Bearer $ADMIN_TOKEN" \
  "https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/roles/Admin")
check_error "Failed to fetch Admin role"

echo "Assigning admin role to admin user..."
curl -ks -X POST "https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/realms/library/users/${ADMIN_USER_ID}/role-mappings/realm" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "[$ADMIN_ROLE]"
check_error "Failed to assign admin role"

echo "✅ Keycloak initialization completed successfully!"
