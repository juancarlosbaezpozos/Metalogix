using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace Metalogix.Utilities
{
    public static class StringExtensions
    {
        public static T Deserialize<T>(this string objectXml)
            where T : class
        {
            T t;
            if (string.IsNullOrEmpty(objectXml))
            {
                return default(T);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(objectXml))
            {
                t = (T)xmlSerializer.Deserialize(stringReader);
            }

            return t;
        }

        public static string GetArgumentsAsString(this string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < args.Count<string>(); i++)
            {
                if (!args[i].Contains<char>(' '))
                {
                    stringBuilder.Append(args[i]);
                }
                else
                {
                    stringBuilder.Append(string.Format("\"{0}\"", args[i]));
                }

                if (i < args.Count<string>() - 1)
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return string.IsNullOrEmpty(value.Trim());
        }

        public static T ToEnumValue<T>(this string stringValue, T defaultValue)
        {
            T t = defaultValue;
            if (stringValue != null && Enum.IsDefined(typeof(T), stringValue))
            {
                t = (T)Enum.Parse(typeof(T), stringValue);
            }

            return t;
        }

        public static bool TryParseGuid(this string guidString, out Guid guid)
        {
            bool flag;
            try
            {
                guid = new Guid(guidString);
                flag = true;
            }
            catch
            {
                guid = Guid.Empty;
                flag = false;
            }

            return flag;
        }
    }
}