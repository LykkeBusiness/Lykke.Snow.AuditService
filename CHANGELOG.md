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
