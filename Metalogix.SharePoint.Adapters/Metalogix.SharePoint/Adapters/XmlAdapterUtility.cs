using System;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    [Obsolete("Please use System.Xml or System.Xml.Linq extensions methods in favour of methods in XMLUtility class")]
    public static class XmlAdapterUtility
    {
        public static string getAtt(XmlNode xNode, TaxonomyFields fieldType)
        {
            return XmlAdapterUtility.getAtt(xNode, fieldType.ToString());
        }

        public static string getAtt(XmlNode xNode, string tag)
        {
            if (!(xNode as XmlElement).HasAttribute(tag))
            {
                return string.Empty;
            }

            return xNode.Attributes[tag].Value;
        }

        public static bool getBoolAtt(XmlNode xNode, TaxonomyFields fieldType)
        {
            return XmlAdapterUtility.getBoolAtt(xNode, fieldType.ToString());
        }

        public static bool getBoolAtt(XmlNode xNode, string tag)
        {
            string att = XmlAdapterUtility.getAtt(xNode, tag);
            if (string.IsNullOrEmpty(att))
            {
                return false;
            }

            return bool.Parse(att);
        }

        public static Guid getGuidAtt(XmlNode xNode, TaxonomyFields fieldType)
        {
            string att = XmlAdapterUtility.getAtt(xNode, fieldType.ToString());
            if (string.IsNullOrEmpty(att))
            {
                return Guid.Empty;
            }

            return new Guid(att);
        }

        public static int getIntAtt(XmlNode xNode, TaxonomyFields fieldType)
        {
            string att = XmlAdapterUtility.getAtt(xNode, fieldType);
            if (string.IsNullOrEmpty(att))
            {
                return 0;
            }

            return int.Parse(att);
        }

        public static void setAtt(XmlWriter xWriter, object val, TaxonomyFields fieldType)
        {
            XmlAdapterUtility.setAtt(xWriter, val, fieldType.ToString());
        }

        public static void setAtt(XmlWriter xWriter, object val, string tag)
        {
            if (val == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(val as string) || val is bool && Convert.ToBoolean(val) ||
                val is Guid && !val.Equals(Guid.Empty) || val is int)
            {
                xWriter.WriteAttributeString(tag, val.ToString());
            }
        }
    }
}