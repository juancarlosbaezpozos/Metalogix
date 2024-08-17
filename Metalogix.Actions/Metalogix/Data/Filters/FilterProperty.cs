using System;

namespace Metalogix.Data.Filters
{
    public class FilterProperty
    {
        public Type PropertyType;

        public string Name;

        public bool IsSystemAttribute;

        public FilterProperty(string name, Type propertyType, bool isSystemAttribute)
        {
            this.Name = name;
            this.PropertyType = propertyType;
            this.IsSystemAttribute = isSystemAttribute;
        }
    }
}