// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.Snow.AuditService.Settings
{
    public class CsvExportSettings
    {
        public bool ShouldOutputHeader { get; set; }
        public string? Delimiter { get; set; }
    }
}