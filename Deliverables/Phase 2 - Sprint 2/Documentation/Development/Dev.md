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







