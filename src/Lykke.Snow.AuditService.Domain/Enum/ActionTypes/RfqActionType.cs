namespace Lykke.Snow.AuditService.Domain.Enum.ActionTypes
{
    // TODO: Note that we don't have Deletion for Rfq,
    // That gives another reason why we should ActionType per AuditDataType
    public enum RfqActionType
    {
        Creation,
        Edition,
        StatusChanged
    }
}
