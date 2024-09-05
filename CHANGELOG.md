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
