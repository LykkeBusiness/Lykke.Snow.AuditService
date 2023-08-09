namespace Lykke.Snow.AuditService.Domain.Model
{
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