using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Metalogix.Data.Filters
{
    public class StringFilterExpression : FilterExpression
    {
        public new FilterOperand Operand
        {
            get { return this.m_ifilterOperand; }
            set { this.m_ifilterOperand = value; }
        }

        public new string Pattern
        {
            get { return this.m_sPattern; }
            set { this.m_sPattern = value; }
        }

        public StringFilterExpression(FilterOperand operand, string sPattern, bool bCaseSensitive) : base(operand,
            typeof(string), "", sPattern, bCaseSensitive, false)
        {
        }

        public StringFilterExpression(XmlNode xmlNode) : base(xmlNode)
        {
            if (xmlNode != null)
            {
                this.FromXML(xmlNode);
            }
        }

        public override bool Evaluate(object component, Comparison<object> comparer)
        {
            if (component == null && this.Operand == FilterOperand.IsNull)
            {
                return true;
            }

            string str = component as string;
            if (str == null)
            {
                return false;
            }

            bool flag = false;
            if (!base.EvaluateAgainstString(str, null, out flag))
            {
                flag = false;
            }

            return flag;
        }

        public override void FromXML(XmlNode node)
        {
            string value;
            System.Globalization.CultureInfo cultureInfo;
            XmlNode xmlNodes = (node.Name == "StringFilterExpression"
                ? node
                : node.SelectSingleNode(".//StringFilterExpression"));
            if (xmlNodes != null)
            {
                string name = xmlNodes.FirstChild.Name;
                this.m_ifilterOperand = (FilterOperand)Enum.Parse(typeof(FilterOperand), name);
                this.m_sProperty = xmlNodes.FirstChild.Attributes["Property"].Value;
                if (xmlNodes.FirstChild.Attributes["Pattern"] != null)
                {
                    value = xmlNodes.FirstChild.Attributes["Pattern"].Value;
                }
                else
                {
                    value = null;
                }

                this.m_sPattern = value;
                this.m_bIsCaseSensitive = (xmlNodes.FirstChild.Attributes["CaseSensitive"] != null
                    ? xmlNodes.FirstChild.Attributes["CaseSensitive"].Value == "True"
                    : false);
                this.m_bIsBaseFilter = false;
                if (xmlNodes.FirstChild.SelectSingleNode(".//AppliesToTypes") != null)
                {
                    this.m_AppliesToTypes = new List<string>();
                    foreach (XmlNode xmlNodes1 in xmlNodes.FirstChild.SelectSingleNode(".//AppliesToTypes"))
                    {
                        if (xmlNodes1.Value == null)
                        {
                            continue;
                        }

                        this.m_AppliesToTypes.Add(xmlNodes1.Value);
                    }
                }
                else if (xmlNodes.FirstChild.Attributes["AppliesTo"] != null)
                {
                    this.m_AppliesToTypes.Add(xmlNodes.FirstChild.Attributes["AppliesTo"].Value);
                }

                if (xmlNodes.FirstChild.Attributes["CultureInfoName"] != null)
                {
                    cultureInfo =
                        new System.Globalization.CultureInfo(xmlNodes.FirstChild.Attributes["CultureInfoName"].Value);
                }
                else
                {
                    cultureInfo = null;
                }

                this.m_cultureInfo = cultureInfo;
            }
        }

        public override string GetLogicString()
        {
            return string.Concat(this.Operand.ToString(), " ", this.Pattern);
        }

        public override string ToString()
        {
            return string.Format("Filter System.String where {1} {2}", this.Operand.ToString(), this.Pattern);
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("StringFilterExpression");
            xmlWriter.WriteStartElement(this.Operand.ToString());
            xmlWriter.WriteAttributeString("Property", base.Property);
            xmlWriter.WriteAttributeString("CaseSensitive", base.IsCaseSensitive.ToString());
            xmlWriter.WriteAttributeString("BaseFilter", base.IsBaseFilter.ToString());
            if (this.Pattern != null)
            {
                xmlWriter.WriteAttributeString("Pattern", this.Pattern);
            }

            if (this.m_cultureInfo != null)
            {
                xmlWriter.WriteAttributeString("CultureInfoName", this.m_cultureInfo.Name);
            }

            if (base.AppliesToTypes != null && base.AppliesToTypes.Count > 0)
            {
                xmlWriter.WriteStartElement("AppliesToTypes");
                foreach (string appliesToType in base.AppliesToTypes)
                {
                    xmlWriter.WriteElementString("AppliesTo", appliesToType);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
}