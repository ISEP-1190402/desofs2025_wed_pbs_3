# Infrastructure

## Azure Infra

![azure-rg.png](Pictures/azure-rg.png)

## libraryrentalsystem - Azure Web App

[Open on Azure](https://login.microsoftonline.com/myisepipp.onmicrosoft.com/oauth2/v2.0/authorize?redirect_uri=https%3A%2F%2Fportal.azure.com%2Fsignin%2Findex%2F&response_type=code%20id_token&scope=https%3A%2F%2Fmanagement.core.windows.net%2F%2Fuser_impersonation%20openid%20email%20profile&state=OpenIdConnect.AuthenticationProperties%3Db3x3JwaJnU6DtG2VEWFizwlKcOpqiS8uas3pkzYW080MBqJiohsV_654b-xU9BObQdTrDlG78sK_0NRBPTAkh3QyTvxfR2StVa6f8a_HLU9CWjXiTjEIaqCoO0LhoI4gYYC2baLkwNHgfhM3MW16k_B26SwzG37euMhafOy44683EXkK0wUBuApac7uuZP1IqrBfq3SZhzVdajx8InsedS1xTxbLzY8SMGx2BNUlJQRKMnZdnl2xhKP-vOzbN1_r1xlEJmZvzJpr-3HVLewGaqyqsDG1Lz1eURXQSD_DsChaZFSaMs3VLrwRzqM2PNqignHediBbjagKvXPQ0EHL1nNcbX7euQmPb1bquTnF96flmEGixo3fxZkotm0BoV054HL7oHNhxbIlSxhTlxJgGYXcTXePzsiXChzVsWf72IeXaIpJ6ojJb3yo8iFjR0G3algd7-Fg8QxHdARaYoBLBrddeUjFVMDJs5bA_7qeFjbAYRDgQgaKw8YyMX4pWzh_vIPnH-Rveu770gOE1GU-KMBf9Y3GgWefAhDTJXLcxNrsRLNpUq_95brEVzGFd6GfIAfOOfEdJcWwK9ky2ON29jGolJprbhDbYxcn3GHncuo&response_mode=form_post&nonce=638852215996869749.MDhkN2EyMGMtYzEyMC00NDJmLWFiYzgtMjQ3YTAyNDcxZGYxZmVkN2M0YjYtZTAxMS00NTUwLWE0NzEtNDk1ZWI4NjRjYTE3&client_id=c44b4083-3bb0-49c1-b47d-974e53cbdf3c&site_id=501430&client-request-id=42748f09-8ac3-49f2-869b-8888a4021670&x-client-SKU=ID_NET472&x-client-ver=8.3.0.0)

This resource is used to host our API and is powered by automatic deployment via github actions.

![API - pagina inicial.png](Pictures/API%20-%20pagina%20inicial.png)

![vm-overview.png](Pictures/vm-overview.png)

| **Property**          | **Value**                                             |
| --------------------- | ----------------------------------------------------- |
| **VM Name**           | vm-desofs2025-wed-pbs-3                               |
| **Location**          | West Europe (Zone 1)                                  |
| **Operating System**  | Windows Server 2022 Datacenter Azure Edition          |
| **Size**              | Standard B2ms (2 vCPUs, 8 GiB memory)                 |
| **Resource Group**    | desofs-2025-m1b-pbs-3                                 |
| **Public IP Address** | 51.105.240.143                                        |
| **Virtual Network**   | keycloakvm-vnet/default                               |
| **DNS Name**          | vm-desofs2025-wed-pbs-3.westeurope.cloudapp.azure.com |
| **Health State**      | Healthy                                               |
| **Time Created**      | 11/06/2025, 17:32 UTC                                 |
| **Availability Zone** | 1                                                     |

### Networking

- Public IP address: 51.105.240.143
- Virtual network/subnet:keycloakvm-vnet/default
- DNS name:vm-desofs2025-wed-pbs-3.westeurope.cloudapp.azure.com

**Rules**:

![azure-networking.png](Pictures/azure-networking.png)

### Monitoring

#### Rules and Alerts

Rules for VM:
![vm-alert-rules.png](Pictures/vm-alert-rules.png)

Rules for Backup:

![vault - alerts.png](Pictures/vault%20-%20alerts.png)

Alerts:

![azure-alarmística.png](Pictures/azure-alarm%C3%ADstica.png)

Alerts are received by the whole team via email.

#### Dashboard

![insights1.png](Pictures/insights1.png)

![insights2.png](Pictures/insights2.png)

## libraryorsdb-desofs - Azure Database for MySQL flexible server

![vm_backups.png](Pictures/vm_backups.png)

As you can see in the image above, backups have been activated for the vm.

The backup policy is as follows:

![backup-policy.png](Pictures/backup-policy.png)

**Backup Policy Summary**

**Policy Name**: EnhancedPolicy-mb853f

**Backup Frequency and Schedule**:

- Backups are taken every 4 hours, starting at 08:00 UTC.
- Each backup job runs for a duration of 12 hours.

## keycloakvm - Azure Virtual Machine

- Daily Backups: Retained for 30 days.
- Weekly Backups: Retained for 12 weeks, with backups taken every Sunday.
- Monthly Backups: Retained for 60 months, with backups taken on the first Sunday of each month.
- Yearly Backups: Retained for 10 years, with backups taken on the first Sunday of January each year.

**Tiering Option**

Tiering is enabled.

- Recovery points are automatically moved to the vault-archive tier based on recommended recovery points, ensuring cost effectiveness by leveraging lower-cost storage for long-term retention.

`This configuration leverages Azure Backup’s tiering capabilities to optimise storage costs while maintaining a robust retention strategy for daily, weekly, monthly, and yearly backups`

### Disaster Recovery

**Backup Source**:
Use the latest valid backup from the Azure Recovery Services Vault.

**Restore Process**:

- In the Azure portal, navigate to the Recovery Services Vault.
- Select the backup item for the affected VM.
- Choose the desired recovery point.
- Select the option to "Restore to a new VM."
- Configure the new VM (name, resource group, network, etc.) as needed.
- Initiate the restore operation; Azure will provision a new VM with the restored disks and configuration.

**Post-Restore Steps**:

- Reapply any custom configurations, environment variables, and secrets.
- Validate connectivity for API and DB endpoints.
- Restrict Keycloak access to the internal network as previously configured.
- Re-enable monitoring via Application Insights by restoring the connection string.
- Test all system components to confirm full functionality.

### Patching

Configurations:

![azure updates - configuration.png](Pictures/azure%20updates%20-%20configuration.png)

Collected Data:

![updates.png](Pictures/updates.png)

To do this, docker was configured with 2 containers, one for keycloak itself and 1 for its database:

Environment variables have been created for all components to allow applications and services to run without exposing passwords in the code:

## AI-libraryrentalsystem - Application Insights

**Application Insights** is a feature of Azure Monitor used to collect telemetry data such as request rates, response
times, failure rates, and diagnostic traces. In the **AI-libraryrentalsystem**, it provides visibility into the health,
performance, and usage patterns of the system, enabling proactive monitoring and rapid issue resolution.

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

Sensitive information such as connection strings and user data is not logged. Only metadata and performance telemetry
are collected. Telemetry data is encrypted in transit and at rest within Azure.

---

## Monitoring & Observability

Developers and maintainers can monitor metrics and traces using the Azure Portal > Application Insights dashboard, which
provides:

- **Live Metrics Stream** – Real-time health and performance data
- **Failures** – Shows exceptions and failed requests
- **Performance** – Tracks response times and load
- **Usage** – Page views, session counts, and user behavior
- **Logs (Log Analytics)** – Deep queries with **Kusto Query Language (KQL)**

### Benefits

- Enables **DevOps** and **DevSecOps** practices with continuous monitoring
- Reduces **MTTR (Mean Time to Recovery)** during production incidents
- Supports **data-driven** decisions for performance optimizations
- Allows integration with **alerts, dashboards**, and **workbooks** in Azure Monitor
