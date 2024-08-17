using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.Utilities
{
    public static class XmlExtensions
    {
        public static void AddOrUpdateAttribute(this XmlElement element, string attributeName, string attributeValue)
        {
            if (element.Attributes[attributeName] != null)
            {
                element.Attributes[attributeName].Value = attributeValue;
                return;
            }

            XmlAttribute xmlAttribute = element.OwnerDocument.CreateAttribute(attributeName);
            xmlAttribute.Value = attributeValue;
            element.Attributes.Append(xmlAttribute);
        }

        public static bool ContainsInnerText(XmlNode node)
        {
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
            {
                return true;
            }

            return false;
        }

        public static bool GetAttributeValueAsBoolean(this XmlAttributeCollection attributes, string attributeName)
        {
            bool flag = false;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                bool.TryParse(attributes[attributeName].Value, out flag);
            }

            return flag;
        }

        public static bool GetAttributeValueAsBoolean(this XmlNode node, string attributeName)
        {
            bool attributeValueAsBoolean = false;
            if (node != null && node.Attributes != null)
            {
                attributeValueAsBoolean = node.Attributes.GetAttributeValueAsBoolean(attributeName);
            }

            return attributeValueAsBoolean;
        }

        public static T GetAttributeValueAsEnumValue<T>(this XmlNode node, string attributeName)
        {
            T t = default(T);
            if (node != null && node.Attributes != null)
            {
                string attributeValueAsString = node.Attributes.GetAttributeValueAsString(attributeName);
                int num = 0;
                if (int.TryParse(attributeValueAsString, out num))
                {
                    if (Enum.IsDefined(typeof(T), num))
                    {
                        t = (T)Enum.Parse(typeof(T), attributeValueAsString);
                    }
                }
                else if (Enum.IsDefined(typeof(T), attributeValueAsString))
                {
                    t = (T)Enum.Parse(typeof(T), attributeValueAsString);
                }
            }

            return t;
        }

        public static T GetAttributeValueAsEnumValue<T>(this XmlNode node, string attributeName, T defaultValue)
        {
            T t = defaultValue;
            if (node != null && node.Attributes != null)
            {
                string attributeValueAsString = node.Attributes.GetAttributeValueAsString(attributeName);
                int num = 0;
                if (int.TryParse(attributeValueAsString, out num))
                {
                    if (Enum.IsDefined(typeof(T), num))
                    {
                        t = (T)Enum.Parse(typeof(T), attributeValueAsString);
                    }
                }
                else if (Enum.IsDefined(typeof(T), attributeValueAsString))
                {
                    t = (T)Enum.Parse(typeof(T), attributeValueAsString);
                }
            }

            return t;
        }

        public static Guid GetAttributeValueAsGuid(this XmlAttributeCollection attributes, string attributeName)
        {
            Guid empty = Guid.Empty;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                string value = attributes[attributeName].Value;
                if (!string.IsNullOrEmpty(value))
                {
                    empty = new Guid(value);
                }
            }

            return empty;
        }

        public static Guid GetAttributeValueAsGuid(this XmlNode node, string attributeName)
        {
            Guid empty = Guid.Empty;
            if (node != null && node.Attributes != null)
            {
                empty = node.Attributes.GetAttributeValueAsGuid(attributeName);
            }

            return empty;
        }

        public static int GetAttributeValueAsInt(this XmlAttributeCollection attributes, string attributeName)
        {
            int num = 0;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                int.TryParse(attributes[attributeName].Value, out num);
            }

            return num;
        }

        public static int GetAttributeValueAsInt(this XmlNode node, string attributeName)
        {
            int attributeValueAsInt = 0;
            if (node != null && node.Attributes != null)
            {
                attributeValueAsInt = node.Attributes.GetAttributeValueAsInt(attributeName);
            }

            return attributeValueAsInt;
        }

        public static long GetAttributeValueAsLong(this XmlNode node, string attributeName)
        {
            long attributeValueAsLong = (long)0;
            if (node != null && node.Attributes != null)
            {
                attributeValueAsLong = node.Attributes.GetAttributeValueAsLong(attributeName);
            }

            return attributeValueAsLong;
        }

        public static long GetAttributeValueAsLong(this XmlAttributeCollection attributes, string attributeName)
        {
            long num = (long)0;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                long.TryParse(attributes[attributeName].Value, out num);
            }

            return num;
        }

        public static bool? GetAttributeValueAsNullableBoolean(this XmlAttributeCollection attributes,
            string attributeName)
        {
            bool? nullable = null;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                bool flag = false;
                if (bool.TryParse(attributes[attributeName].Value, out flag))
                {
                    nullable = new bool?(flag);
                }
            }

            return nullable;
        }

        public static bool? GetAttributeValueAsNullableBoolean(this XmlNode node, string attributeName)
        {
            bool? attributeValueAsNullableBoolean = null;
            if (node != null && node.Attributes != null)
            {
                attributeValueAsNullableBoolean = node.Attributes.GetAttributeValueAsNullableBoolean(attributeName);
            }

            return attributeValueAsNullableBoolean;
        }

        public static short GetAttributeValueAsShortInt(this XmlAttributeCollection attributes, string attributeName)
        {
            short num = 0;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                short.TryParse(attributes[attributeName].Value, out num);
            }

            return num;
        }

        public static short GetAttributeValueAsShortInt(this XmlNode node, string attributeName)
        {
            short attributeValueAsShortInt = 0;
            if (node != null && node.Attributes != null)
            {
                attributeValueAsShortInt = node.Attributes.GetAttributeValueAsShortInt(attributeName);
            }

            return attributeValueAsShortInt;
        }

        public static string GetAttributeValueAsString(this XmlAttributeCollection attributes, string attributeName)
        {
            string empty = string.Empty;
            if (attributes != null && !string.IsNullOrEmpty(attributeName) && attributes[attributeName] != null)
            {
                empty = attributes[attributeName].Value;
            }

            return empty;
        }

        public static string GetAttributeValueAsString(this XmlNode node, string attributeName)
        {
            string empty = string.Empty;
            if (node != null && node.Attributes != null)
            {
                empty = node.Attributes.GetAttributeValueAsString(attributeName);
            }

            return empty;
        }

        public static bool IsAttributeValueInSet(this XmlNode node, string attributeName, params string[] stringSet)
        {
            bool flag = false;
            if (node != null && node.Attributes != null)
            {
                flag = stringSet.Contains<string>(node.Attributes.GetAttributeValueAsString(attributeName),
                    StringComparer.OrdinalIgnoreCase);
            }

            return flag;
        }

        public static bool IsValidXmlAttributeValue(string value)
        {
            bool flag;
            if (value == null)
            {
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
            {
                try
                {
                    xmlWriter.WriteAttributeString("a", value);
                }
                catch (ArgumentException argumentException)
                {
                    flag = false;
                    return flag;
                }

                return true;
            }

            return flag;
        }

        public static string ReplaceInvalidEscapeCharInXmlAttributeValue(string value)
        {
            if (XmlExtensions.IsValidXmlAttributeValue(value) || string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Regex.Replace(value, "[\\v|\\b|\\a|\\f]", string.Empty);
        }

        public static XmlNode SelectSingleNodeWithIgnoreCase(this XmlNode xmlNode, string xpath, string attributeName,
            string attributeValue)
        {
            return xmlNode.SelectSingleNode(string.Format(
                "{0}[translate(@{1}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{2}']", xpath,
                attributeName, attributeValue.ToLower()));
        }
    }
}