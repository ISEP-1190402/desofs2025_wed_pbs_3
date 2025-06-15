# Development Notes

## API

We've fixed an identified issue related to Migrations by removing the project dependency:

![csproj without migration.png](Pictures/csproj%20without%20migration.png)

The rental aggregate was created this sprint. However, when creating a rental, an issue was identified in the EF (Entity Framework) that could not be corrected.

![rental problem - ef.png](Pictures/rental%20problem%20-%20ef.png)
![rental error - ef.png](Pictures/rental%20error%20-%20ef.png)

In any case, for demonstration purposes, the validation that was affecting the functionality has been removed.

### Swagger

We implemented Swagger so that we could have better visibility over the application's endpoints:

![Endpoints p1.png](Pictures/Endpoints%20p1.png)

![Endpoints p2.png](Pictures/Endpoints%20p2.png)

![Endpoints p3.png](Pictures/Endpoints%20p3.png)

In addition, we can also see a list of the schemas used.

![Schemas.png](Pictures/Schemas.png)

---

## Keycloak

During the requirements gathering we thought it was a good idea to have roles in the database but once we started configuring keycloak we realized this was redundant since keycloak already provides role features out of the box. We've removed role table from the app and all logic related with roles other than those configured within the keycloak instance.

We've faced some issues while using a shared database for multiple keycloak instances while developing and testing.
Overwritten settings and database locks are some examples of the issues faced.

"
2025-06-15 01:09:03,043 WARN  [org.keycloak.connections.jpa.updater.liquibase.lock.CustomLockService] (main) Lock didn't yet acquired. Will possibly retry to acquire lock. Details: Lock wait timeout exceeded; try restarting transaction [Failed SQL: (1205) SELECT ID FROM keycloak.databasechangeloglock WHERE ID=1000 FOR UPDATE]: liquibase.exception.DatabaseException: Lock wait timeout exceeded; try restarting transaction [Failed SQL: (1205) SELECT ID FROM keycloak.databasechangeloglock WHERE ID=1000 FOR UPDATE]"






