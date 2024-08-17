using System;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Metabase.Data
{
    public class EnumDescriptions
    {
        public EnumDescriptions()
        {
        }

        public static EnumDescriptions.EnumDescPair[] GetComboBoxValues(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            EnumDescriptions.EnumDescPair[] enumDescPair = new EnumDescriptions.EnumDescPair[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                Enum value = (Enum)values.GetValue(i);
                string enumDescription = EnumDescriptions.GetEnumDescription(value);
                enumDescPair[i] = new EnumDescriptions.EnumDescPair(enumDescription, value);
            }

            return enumDescPair;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] customAttributes =
                (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((int)customAttributes.Length <= 0)
            {
                return value.ToString();
            }

            return customAttributes[0].Description;
        }

        public class EnumDescPair
        {
            private string m_strName;

            private object m_oValue;

            public string Name
            {
                get { return this.m_strName; }
            }

            public object Value
            {
                get { return this.m_oValue; }
            }

            public EnumDescPair(string strName, object objVal)
            {
                this.m_strName = strName;
                this.m_oValue = objVal;
            }
        }
    }
}