// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.Snow.AuditService.Client.Model
{
    /// <summary>
    /// It is used to filter the audit events by their diffs. (Audit events that have changed some properties of the objects)
    /// </summary>
    public class JsonDiffFilterContract
    {
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public object? Value { get; set; }
    }
}
