using System;
using System.ComponentModel;
using System.Globalization;

namespace Metalogix.Metabase.DataTypes
{
    public class UrlTypeConverter : ExpandableObjectConverter
    {
        public UrlTypeConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Equals(typeof(string)))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str != null)
            {
                return new Url(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            Url url = value as Url;
            if (url != null && destinationType == typeof(string))
            {
                return url.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}