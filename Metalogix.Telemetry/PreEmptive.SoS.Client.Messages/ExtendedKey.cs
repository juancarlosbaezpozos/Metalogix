using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PreEmptive.SoS.Client.Messages
{
    public class ExtendedKey
    {
        private string keyField;

        private string valueField;

        private string dataType;

        public static int KEY_LIMIT;

        public static int VALUE_LIMIT;

        private Regex regex = new Regex("^[-+]?[0-9]+[.]?[0-9]*([eE][-+]?[0-9]+)?$");

        public string DataType
        {
            get
            {
                if (!this.IsNumeric(this.valueField))
                {
                    this.dataType = "string";
                }
                else
                {
                    this.dataType = "decimal";
                }

                return this.dataType;
            }
        }

        public string Key
        {
            get { return this.keyField; }
            set
            {
                if (value != null && value.Length > ExtendedKey.KEY_LIMIT)
                {
                    value = value.Substring(0, ExtendedKey.KEY_LIMIT);
                }

                this.keyField = value;
            }
        }

        public string Value
        {
            get { return this.valueField; }
            set
            {
                if (value != null && value.Length > ExtendedKey.VALUE_LIMIT)
                {
                    value = value.Substring(0, ExtendedKey.VALUE_LIMIT);
                }

                this.valueField = value;
            }
        }

        static ExtendedKey()
        {
            ExtendedKey.KEY_LIMIT = 2000;
            ExtendedKey.VALUE_LIMIT = 4000;
        }

        public ExtendedKey(string string_0, string value)
        {
            this.Key = string_0;
            this.Value = value;
        }

        public ExtendedKey()
        {
        }

        public static string GetValueAsString(object object_0)
        {
            string str = null;
            if (object_0 == null)
            {
                str = "null";
            }
            else if (object_0 is DateTime)
            {
                DateTime universalTime = ((DateTime)object_0).ToUniversalTime();
                str = universalTime.ToString("R", CultureInfo.InvariantCulture);
            }
            else if (!(object_0 is IFormattable))
            {
                str = object_0.ToString();
            }
            else
            {
                try
                {
                    str = ((IFormattable)object_0).ToString("G", CultureInfo.InvariantCulture);
                }
                catch (FormatException formatException)
                {
                    str = object_0.ToString();
                }
            }

            return str;
        }

        private bool IsNumeric(string inputString)
        {
            return this.regex.IsMatch(inputString);
        }
    }
}