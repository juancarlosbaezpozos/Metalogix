using Metalogix.Data.Mapping;
using Metalogix.Metabase.DataTypes;
using System;
using System.ComponentModel;

namespace Metalogix.Metabase.Mappings
{
    public class PropertyDescriptorMapper : ListTransformer<PropertyDescriptor>
    {
        public PropertyDescriptorMapper()
        {
        }

        public override string Transform(PropertyDescriptor item)
        {
            return item.DisplayName;
        }

        public override string TransformColumn(PropertyDescriptor item, string propertyName)
        {
            return string.Empty;
        }

        public override string TransformType(PropertyDescriptor item)
        {
            if (item.PropertyType == typeof(Url))
            {
                return "URL";
            }

            if (item.PropertyType != typeof(decimal))
            {
                if (item.PropertyType != typeof(int))
                {
                    if (item.PropertyType == typeof(TextMoniker))
                    {
                        return "Note";
                    }

                    if (item.PropertyType == typeof(string))
                    {
                        return "Text";
                    }

                    if (item.PropertyType == typeof(bool))
                    {
                        return "Boolean";
                    }

                    if (item.PropertyType == typeof(DateTime))
                    {
                        return "DateTime";
                    }

                    return "Text";
                }
            }

            return "Number";
        }
    }
}