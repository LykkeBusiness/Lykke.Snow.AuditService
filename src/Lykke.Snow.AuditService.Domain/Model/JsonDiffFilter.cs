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
        public string PropertyName { get; set; }
        public string? Value { get; set; }
        
        /// <summary>
        /// Use this constructor if only interested whether the property has changed or not.
        /// </summary>
        /// <param name="propertyName"></param>
        public JsonDiffFilter(string propertyName)
        {
            PropertyName = propertyName;
        }
        
        /// <summary>
        /// Use this constructor if interested in particular value of the property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public JsonDiffFilter(string propertyName, string value)
        {
            PropertyName = propertyName;
            Value = value;
        }
    }
}