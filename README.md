# Lykke.Snow.AuditService
This service is responsible for storing &amp; serving audit events.

## How to use in prod environment?
// To be filled.

## How to run for debug?

1. Clone the repository anywhere in filesystem.
2. Create an `appsettings.dev.json` file in `src/Lykke.Snow.AuditService` directory. (Please refer to **Configuration** section.)
3. Add an environment variable `SettingsUrl` with value of `appsettings.dev.json`
4. Corresponding VPN (dev, test) must be activated prior to running the project.
5. Run the project

## Dependencies

- RabbitMQ Broker
- MSSQL Database

## Configuration

### 1. Hosting Configuration

Kestrel configuration may be passed through appsettings.json, secrets or environment.
All variables and value constraints are default. For instance, to set host URL the following env variable may be set:

```json
{
    "Kestrel__EndPoints__Http__Url": "http://*:5010"
}
```

## Environment Variables

* *RESTART_ATTEMPTS_NUMBER* - number of restart attempts. If not set int.MaxValue is used.
* *RESTART_ATTEMPTS_INTERVAL_MS* - interval between restarts in milliseconds. If not set 10000 is used.
* *SettingsUrl* - defines URL of remote settings or path for local settings.

## Settings

Settings schema is.

```json
{
    "AuditService": {
        "Db": {
            "ConnectionString": ""
        },
        "Subscribers": {
            "RfqEventSubscriber": {
              "ConnectionString": "",
              "QueueName": "",
              "RoutingKey": "",
              "ExchangeName": "",
              "IsDurable": false
            }
        },
        "AuditServiceClient": {
            "ServiceUrl": "http://audit-service.mt.svc.cluster.local",
            "ApiKey": ""
        },
        "CsvExportSettings": {
            "Delimiter": ",",
            "ShouldOutputHeader": true
        }
    }
}
```
