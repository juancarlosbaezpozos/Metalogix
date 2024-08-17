using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.DataStructures
{
    public class TransformationTask : IXmlable, Metalogix.DataStructures.IComparable
    {
        private IFilterExpression m_ApplyTo;

        private SerializableTable<string, string> m_stChangeOperations;

        public IFilterExpression ApplyTo
        {
            get { return this.m_ApplyTo; }
            set { this.m_ApplyTo = value; }
        }

        public SerializableTable<string, string> ChangeOperations
        {
            get { return this.m_stChangeOperations; }
        }

        public TransformationTask()
        {
            this.m_stChangeOperations = new CommonSerializableTable<string, string>();
        }

        public TransformationTask(XmlNode xmlNode)
        {
            if (xmlNode != null)
            {
                this.m_stChangeOperations = new CommonSerializableTable<string, string>();
                this.FromXML(xmlNode);
            }
        }

        public void FromXML(XmlNode node)
        {
            XmlNode xmlNodes =
                (node.Name == "TransformationTask" ? node : node.SelectSingleNode("//TransformationTask"));
            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(".//XmlableTable");
            XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./AppliesToFilter");
            if (xmlNodes2 == null)
            {
                xmlNodes2 = xmlNodes.SelectSingleNode(".//FilterExpression");
                if (xmlNodes2 != null)
                {
                    this.m_ApplyTo = new FilterExpression(xmlNodes.FirstChild);
                }
            }
            else if (xmlNodes2.ChildNodes.Count > 0)
            {
                if (xmlNodes2.ChildNodes[0].Name.Equals("And") || xmlNodes2.ChildNodes[0].Name.Equals("Or"))
                {
                    this.m_ApplyTo = new FilterExpressionList(xmlNodes2.ChildNodes[0]);
                }
                else
                {
                    this.m_ApplyTo = new FilterExpression(xmlNodes2.ChildNodes[0]);
                }
            }

            if (xmlNodes1 != null)
            {
                this.m_stChangeOperations.FromXML(xmlNodes1);
            }
        }

        public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput,
            ComparisonOptions options)
        {
            if (!(targetComparable is TransformationTask))
            {
                differencesOutput.Write("Target comparable is not a compatible type.");
                return false;
            }

            TransformationTask transformationTask = targetComparable as TransformationTask;
            if (!this.m_stChangeOperations.IsEqual(transformationTask.ChangeOperations, differencesOutput, null))
            {
                return false;
            }

            if (transformationTask.ApplyTo.ToString().Equals(this.m_ApplyTo.ToString()))
            {
                return true;
            }

            differencesOutput.Write("Filter Expression is different on Target than on Source", "Filter expression");
            return false;
        }

        public string PerformTransformation(string sourceXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sourceXml);
            XmlNode firstChild = xmlDocument.FirstChild;
            foreach (KeyValuePair<string, string> changeOperation in this.ChangeOperations)
            {
                string key = changeOperation.Key;
                XmlNode itemOf = firstChild;
                if (key.IndexOfAny(new char[] { '.', '/', ':', '[', ']', '@' }) < 0)
                {
                    itemOf = firstChild.Attributes[key];
                    if (itemOf == null)
                    {
                        XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(key);
                        firstChild.Attributes.Append(xmlAttribute);
                        itemOf = xmlAttribute;
                    }
                }
                else
                {
                    itemOf = XmlUtility.FindOrCreateNode(key, firstChild);
                }

                if (itemOf == null)
                {
                    continue;
                }

                if (!(itemOf is XmlElement))
                {
                    itemOf.Value = changeOperation.Value;
                }
                else
                {
                    itemOf.InnerXml = changeOperation.Value;
                }
            }

            return firstChild.OuterXml;
        }

        public string TaskToUIString()
        {
            if (this.m_stChangeOperations == null)
            {
                return null;
            }

            string str = null;
            foreach (KeyValuePair<string, string> changeOperation in this.ChangeOperations)
            {
                string str1 = str;
                string[] key = new string[]
                    { str1, "Change ", changeOperation.Key, " To '", changeOperation.Value, "' , " };
                str = string.Concat(key);
            }

            return str.Substring(0, str.Length - 2);
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXML(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TransformationTask");
            xmlWriter.WriteStartElement("AppliesToFilter");
            if (this.ApplyTo != null)
            {
                this.ApplyTo.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
            this.ChangeOperations.ToXML(xmlWriter);
            xmlWriter.WriteEndElement();
        }
    }
}