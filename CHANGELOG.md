## 1.6.0 - Nova 2. Delivery 47 (November 18, 2024)
### What's changed
* LT-5841: Update messagepack to 2.x version.
* LT-5757: Migrate to quorum queues.
* LT-5754: Add assembly load logger.

### Deployment
In this release, all previously specified queues have been converted to quorum queues to enhance system reliability. The affected queues are:
- `dev.AuditService.queue.RfqEvent`

#### Automatic Conversion to Quorum Queues
The conversion to quorum queues will occur automatically upon service startup **if**:
* There are **no messages** in the existing queues.
* There are **no active** subscribers to the queues.

**Warning**: If messages or subscribers are present, the automatic conversion will fail. In such cases, please perform the following steps:
1. Run the previous version of the component associated with the queue.
1. Make sure all the messages are processed and the queue is empty.
1. Shut down the component associated with the queue.
1. Manually delete the existing classic queue from the RabbitMQ server.
1. Restart the component to allow it to create the quorum queue automatically.

#### Poison Queues
All the above is also applicable to the poison queues associated with the affected queues. Please ensure that the poison queues are also converted to quorum queues.

#### Disabling Mirroring Policies
Since quorum queues inherently provide data replication and reliability, server-side mirroring policies are no longer necessary for these queues. Please disable any existing mirroring policies applied to them to prevent redundant configurations and potential conflicts.

#### Environment and Instance Identifiers
Please note that the queue names may include environment-specific identifiers (e.g., dev, test, prod). Ensure you replace these placeholders with the actual environment names relevant to your deployment. The same applies to instance names embedded within the queue names (e.g., DefaultEnv, etc.).

## 1.5.0 - Nova 2. Delivery 46 (September 27, 2024)
### What's changed
* LT-5634: Migrate to .net 8.


## 1.4.0 - Nova 2. Delivery 44 (August 16, 2024)
### What's changed
* LT-5502: Update rabbitmq broker library with new rabbitmq.client and templates.

### Deployment
Please ensure that the mirroring policy is configured on the RabbitMQ server side for the following queues:
- `dev.AuditService.queue.RfqEvent`

These queues require the mirroring policy to be enabled as part of our ongoing initiative to enhance system reliability. They are now classified as "no loss" queues, which necessitates proper configuration. The mirroring feature must be enabled on the RabbitMQ server side.

In some cases, you may encounter an error indicating that the server-side configuration of a queue differs from the client’s expected configuration. If this occurs, please delete the queue, allowing it to be automatically recreated by the client.

**Warning**: The "no loss" configuration is only valid if the mirroring policy is enabled on the server side.

Please be aware that the provided queue names may include environment-specific identifiers (e.g., dev, test, prod). Be sure to replace these with the actual environment name in use. The same applies to instance names embedded within the queue names (e.g., DefaultEnv, etc.).

## 1.3.0 - Nova 2. Delivery 41 (March 29, 2024)
### What's changed
* LT-5441: Update packages.


## 1.2.0 - Nova 2. Delivery 39 (January 30, 2024)
### What's changed
* LT-5149: Changelog.md for lykke.snow.auditservice.
* LT-5115: Update readme.md.


## 1.1.0 - Nova 2. Delivery 38 (2023-12-13)
### What's changed
* LT-5057: Filter rfq events by broker id.

### Deployment
* Add `BrokerId` key to configuration. Example:
  ```json
  {
    "BrokerId": "BBVA",
    ...
  }
  ```

## 1.0.0 - Nova 2. Delivery 36 (2023-08-31)
### What's changed
* LT-4944: Create audit trail api.
* LT-4939: Consume rfq events and save them to the db.

## Deployment
* Please make sure the rabbitmq queue `[ENV].AuditService.queue.RfqEvent` created
* Please make sure the following tables created:
  * `audit.Events`
  * `audit.AuditObjectStates` 

Both the tables and the rabbitmq queue should be automatically created by the service.
