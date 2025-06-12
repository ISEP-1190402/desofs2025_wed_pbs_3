# Infrastructure

For this project, infrastructure resources were created in Azure to support our project.

The main resources are as follows:

## libraryrentalsystem - Azure Web App

[Open on Azure](https://login.microsoftonline.com/myisepipp.onmicrosoft.com/oauth2/v2.0/authorize?redirect_uri=https%3A%2F%2Fportal.azure.com%2Fsignin%2Findex%2F&response_type=code%20id_token&scope=https%3A%2F%2Fmanagement.core.windows.net%2F%2Fuser_impersonation%20openid%20email%20profile&state=OpenIdConnect.AuthenticationProperties%3Db3x3JwaJnU6DtG2VEWFizwlKcOpqiS8uas3pkzYW080MBqJiohsV_654b-xU9BObQdTrDlG78sK_0NRBPTAkh3QyTvxfR2StVa6f8a_HLU9CWjXiTjEIaqCoO0LhoI4gYYC2baLkwNHgfhM3MW16k_B26SwzG37euMhafOy44683EXkK0wUBuApac7uuZP1IqrBfq3SZhzVdajx8InsedS1xTxbLzY8SMGx2BNUlJQRKMnZdnl2xhKP-vOzbN1_r1xlEJmZvzJpr-3HVLewGaqyqsDG1Lz1eURXQSD_DsChaZFSaMs3VLrwRzqM2PNqignHediBbjagKvXPQ0EHL1nNcbX7euQmPb1bquTnF96flmEGixo3fxZkotm0BoV054HL7oHNhxbIlSxhTlxJgGYXcTXePzsiXChzVsWf72IeXaIpJ6ojJb3yo8iFjR0G3algd7-Fg8QxHdARaYoBLBrddeUjFVMDJs5bA_7qeFjbAYRDgQgaKw8YyMX4pWzh_vIPnH-Rveu770gOE1GU-KMBf9Y3GgWefAhDTJXLcxNrsRLNpUq_95brEVzGFd6GfIAfOOfEdJcWwK9ky2ON29jGolJprbhDbYxcn3GHncuo&response_mode=form_post&nonce=638852215996869749.MDhkN2EyMGMtYzEyMC00NDJmLWFiYzgtMjQ3YTAyNDcxZGYxZmVkN2M0YjYtZTAxMS00NTUwLWE0NzEtNDk1ZWI4NjRjYTE3&client_id=c44b4083-3bb0-49c1-b47d-974e53cbdf3c&site_id=501430&client-request-id=42748f09-8ac3-49f2-869b-8888a4021670&x-client-SKU=ID_NET472&x-client-ver=8.3.0.0)

This resource is used to host our API and is powered by automatic deployment via github actions.

![API - pagina inicial.png](Pictures/API%20-%20pagina%20inicial.png)

As you can see, the deploys are operational:

![API - Deploys.png](Pictures/API%20-%20Deploys.png)

## libraryorsdb-desofs - Azure Database for MySQL flexible server

[Open on Azure](https://portal.azure.com/#@myisepipp.onmicrosoft.com/resource/subscriptions/ebeba1b7-1df1-4739-8d56-839f00cf37c1/resourceGroups/desofs-2025-m1b-pbs-3/providers/Microsoft.DBforMySQL/flexibleServers/libraryorsdb-desofs/overview)

A database server (libraryorsdb-desofs.mysql.database.azure.com) was created to support the bookshop application:

![DB SERVER overview.png](Pictures/DB%20SERVER%20overview.png)

The database in question is ‘librarydb’.

![api db.png](Pictures/api%20db.png)

![working db connection from datagrip.png](Pictures/working%20db%20connection%20from%20datagrip.png)

## keycloakvm - Azure Virtual Machine

[Open on Azure](https://portal.azure.com/#@myisepipp.onmicrosoft.com/resource/subscriptions/ebeba1b7-1df1-4739-8d56-839f00cf37c1/resourceGroups/desofs-2025-m1b-pbs-3/providers/Microsoft.Compute/virtualMachines/keycloakvm/overview)

![keycloak vm.png](Pictures/keycloak%20vm.png)

This virtual machine has been created to support Keycloak and its database.
Access to it is conditional on having the SSH key:

![ssh key - keycloakvm.png](Pictures/ssh%20key%20-%20keycloakvm.png)

To do this, docker was configured with 2 containers, one for keycloak itself and 1 for its database:

![keycloak containers on vm.png](Pictures/keycloak%20containers%20on%20vm.png)

## AI-libraryrentalsystem - Application Insights

**Application Insights** is a feature of Azure Monitor used to collect telemetry data such as request rates, response times, failure rates, and diagnostic traces. In the **AI-libraryrentalsystem**, it provides visibility into the health, performance, and usage patterns of the system, enabling proactive monitoring and rapid issue resolution.

[Open on Azure](https://portal.azure.com/#@myisepipp.onmicrosoft.com/resource/subscriptions/ebeba1b7-1df1-4739-8d56-839f00cf37c1/resourceGroups/desofs-2025-m1b-pbs-3/providers/microsoft.insights/components/AI-libraryrentalsystem/overview)
![App Insights.png](Pictures/App%20Insights.png)

### Integration Details

- Application Insights is enabled in the application via the Azure SDK for ASP.NET Core.
- The configuration is set using the **Application Insights Connection String**, stored in an environment variable:

---

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=7cf64314-a013-42a7-9871-7ff3c401d98b;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=d37599ab-0a53-4de7-9452-8fd271a3dc6d"
  }
}
```

---

## Security & Privacy

Sensitive information such as connection strings and user data is not logged. Only metadata and performance telemetry are collected. Telemetry data is encrypted in transit and at rest within Azure.

---

## Monitoring & Observability

Developers and maintainers can monitor metrics and traces using the Azure Portal > Application Insights dashboard, which provides:

- **Live Metrics Stream** – Real-time health and performance data
- **Failures** – Shows exceptions and failed requests
- **Performance** – Tracks response times and load
- **Usage** – Page views, session counts, and user behavior
- **Logs (Log Analytics)** – Deep queries with **Kusto Query Language (KQL)**

## Benefits

- Enables **DevOps** and **DevSecOps** practices with continuous monitoring
- Reduces **MTTR (Mean Time to Recovery)** during production incidents
- Supports **data-driven** decisions for performance optimizations
- Allows integration with **alerts, dashboards**, and **workbooks** in Azure Monitor
