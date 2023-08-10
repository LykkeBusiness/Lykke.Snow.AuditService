namespace Lykke.Snow.AuditService.Settings
{
    public class CsvExportSettings
    {
        public bool ShouldOutputHeader { get; set; }
        public string? Delimiter { get; set; }
    }
}