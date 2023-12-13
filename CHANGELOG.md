## 1.1.0 - Nova 2. Delivery 38 (December 13, 2023)
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
