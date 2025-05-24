#!/bin/bash

# Wait for Keycloak to be ready
#echo "Waiting for Keycloak to be ready..."
#sleep 30

# Function to check if a command succeeded
check_error() {
    if [ $? -ne 0 ]; then
        echo "Error: $1"
        exit 1
    fi
}

# Get admin token
echo "Getting admin token..."
ADMIN_TOKEN=$(curl -X POST http://localhost:8082/realms/master/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=admin" \
  -d "password=admin" \
  -d "grant_type=password" \
  -d "client_id=admin-cli" | jq -r '.access_token')
check_error "Failed to get admin token"

echo "Creating realm..."
curl -X POST http://localhost:8082/admin/realms \
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

# Wait for the Admin role to be available
echo "Waiting for Admin role to be available..."
until curl -s -H "Authorization: Bearer $ADMIN_TOKEN" \
  "http://localhost:8082/admin/realms/library/roles/Admin" | grep -q '"name":"Admin"'; do
    sleep 2
    echo "Still waiting for Admin role..."
done
echo "Admin role is now available."

echo "Creating client..."
curl -X POST http://localhost:8082/admin/realms/library/clients \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "library-client",
    "secret": "your-client-secret",
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
curl -X POST http://localhost:8082/admin/realms/library/users \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "admin@library.com",
    "enabled": true,
    "credentials": [{
      "type": "password",
      "value": "admin123",
      "temporary": false
    }]
  }'
check_error "Failed to create admin user"

echo "Getting admin user ID..."
ADMIN_USER_ID=$(curl -X GET http://localhost:8082/admin/realms/library/users?username=admin \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')
check_error "Failed to get admin user ID"

echo "Assigning admin role to admin user..."
curl -X POST http://localhost:8082/admin/realms/library/users/$ADMIN_USER_ID/role-mappings/realm \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '[{
    "name": "Admin",
    "description": "Administrator role"
  }]'
check_error "Failed to assign admin role"

echo "Keycloak initialization completed successfully!" 