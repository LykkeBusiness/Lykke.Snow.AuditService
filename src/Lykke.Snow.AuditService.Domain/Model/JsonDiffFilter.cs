// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.Snow.AuditService.Domain.Model
{
    /// <summary>
    /// This class intended for representing a json diff based filter.
    /// An object that's been instantiated from this class should be used if,
    /// It's needed to filter json diffs based on some change on specific properties.
    /// </summary>
    public class JsonDiffFilter
    {
        public string PropertyName { get; }
        
        /// <summary>
        /// Can be null if calling code does not mind what new value was.
        /// </summary>
        public object? Value { get; }
        
        public JsonDiffFilter(string propertyName, object? value = null)
        {
            PropertyName = propertyName;
            Value = value;
        }
    }
}