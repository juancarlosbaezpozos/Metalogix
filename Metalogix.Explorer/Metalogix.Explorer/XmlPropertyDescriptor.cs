using System;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.Explorer
{
    public class XmlPropertyDescriptor : PropertyDescriptor
    {
        private string m_sXPath;

        public override Type ComponentType
        {
            get { return typeof(Node); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }

        public XmlPropertyDescriptor(string sName, string XPath) : base(sName, null)
        {
            this.m_sXPath = XPath;
        }

        public XmlPropertyDescriptor(string sName, string XPath, Attribute[] attributes) : base(sName, attributes)
        {
            this.m_sXPath = XPath;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            object value;
            if (component is Node)
            {
                try
                {
                    XmlNode xmlNodes = ((Node)component).GetNodeXML().SelectSingleNode(this.m_sXPath);
                    if (xmlNodes == null)
                    {
                        return null;
                    }
                    else
                    {
                        value = xmlNodes.Value;
                    }
                }
                catch
                {
                    return null;
                }

                return value;
            }

            return null;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}