using System;
using System.ComponentModel;

namespace Metalogix.Metabase.DataTypes
{
    public class TextMonikerTypeConverter : StringConverter
    {
        public TextMonikerTypeConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return false;
            }

            return base.CanConvertFrom(context, sourceType);
        }
    }
}