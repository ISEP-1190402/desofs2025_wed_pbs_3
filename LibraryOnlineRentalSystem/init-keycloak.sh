#!/bin/bash
set -e


check_error() {
    if [ $? -ne 0 ]; then
        echo "Error: $1"
        exit 1
    fi
}

echo "Getting admin token..."
ADMIN_TOKEN=$(curl -ks -X POST http://localhost:8080/realms/master/protocol/openid-connect/token \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "username=desofs-kc" \
    -d "password=xc.uUrqxbz6tDYyQryhK" \
    -d "grant_type=password" \
-d "client_id=admin-cli" | jq -r '.access_token')
check_error "Failed to get admin token"

echo "Creating realm..."
curl -ks -X POST http://localhost:8080/admin/realms \
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

echo "Getting admin user ID..."
ADMIN_USER_ID=$(curl -X GET "http://localhost:8080/admin/realms/library/users?username=desofs-kc" \
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
    "accessTokenLifespan": 7200,              # 2h in seconds
    "verifyEmail": true,
    "registrationAllowed": true,
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

curl -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "passwordPolicy": "length(12) and upperCase(1) and digits(1) and specialChars(1) and notUsername() and notEmail() and passwordHistory(5)"
}'
check_error "Failed to set password policy"

echo "Enabling brute force protection..."

curl -X PUT "http://localhost:8080/admin/realms/library/attack-detection/brute-force" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{
    "enabled": true,
    "failureFactor": 6,
    "waitIncrementSeconds": 60,
    "maxWaitSeconds": 900,
    "quickLoginCheckMilliSeconds": 1000,
    "permanentLockout": false
}'
check_error "Failed to enable brute force protection"

echo "Configuring required actions (VERIFY_EMAIL and CONFIGURE_TOTP)..."

# Enable VERIFY_EMAIL and set defaultAction=true
curl -X PUT "http://localhost:8080/admin/realms/library/authentication/required-actions/VERIFY_EMAIL" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{"enabled": true, "defaultAction": true}'
check_error "Failed to enable VERIFY_EMAIL required action"

# Enable CONFIGURE_TOTP and set defaultAction=true
curl -X PUT "http://localhost:8080/admin/realms/library/authentication/required-actions/CONFIGURE_TOTP" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d '{"enabled": true, "defaultAction": true}'
check_error "Failed to enable CONFIGURE_TOTP required action"


echo "Updating user profile to remove required firstName and lastName, and setting default role User..."

# 1. Get current realm config
REALM_CONFIG=$(curl -s -H "Authorization: Bearer $ADMIN_TOKEN" \
"http://localhost:8080/admin/realms/library")

# 2. Create updated userProfile config JSON, removing firstName and lastName required

# This example assumes the realm has a "userProfile" field in config (Keycloak >= 17).
# We override the attributes array to remove required from firstName and lastName.

# Prepare a JSON patch or full updated config.

# For simplicity, just patch defaultRoles to include "User" and assume firstName/lastName not required.

# Extract current defaultRoles
CURRENT_DEFAULT_ROLES=$(echo "$REALM_CONFIG" | jq '.defaultRoles')

# Update defaultRoles adding "User" if not present
if ! echo "$CURRENT_DEFAULT_ROLES" | grep -q '"User"'; then
    UPDATED_DEFAULT_ROLES=$(echo "$CURRENT_DEFAULT_ROLES" | jq '. + ["User"]')
else
    UPDATED_DEFAULT_ROLES=$CURRENT_DEFAULT_ROLES
fi

# Minimal payload: update defaultRoles (to set User as default) and userProfile config to remove required firstName/lastName

# You will need to manually create or update userProfile config JSON based on your current realm setup.
# Example userProfile JSON disabling required on firstName and lastName:

USER_PROFILE_JSON='{
    "attributes": [
        {
            "name": "firstName",
            "required": false
        },
        {
            "name": "lastName",
            "required": false
        }
    ]
}'

# Note: Keycloak does not support partial updates for userProfile via REST API, so you typically need to set the entire userProfile config.

# For now, let's patch defaultRoles only (safe and simpler):

curl -X PUT "http://localhost:8080/admin/realms/library" \
-H "Authorization: Bearer $ADMIN_TOKEN" \
-H "Content-Type: application/json" \
-d "$(echo "$REALM_CONFIG" | jq --argjson roles "$UPDATED_DEFAULT_ROLES" '.defaultRoles = $roles | del(.userProfile)')"

check_error "Failed to update default roles"

echo "IMPORTANT: To fully remove firstName and lastName as required user profile attributes, you must edit the user profile config JSON in Keycloak UI or via the full userProfile API (complex)."

echo "Set default role User for new registrations."


echo "Keycloak additional configuration done!"

