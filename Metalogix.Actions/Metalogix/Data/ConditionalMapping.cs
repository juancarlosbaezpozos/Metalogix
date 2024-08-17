using Metalogix;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;

namespace Metalogix.Data
{
    public class ConditionalMapping : IXmlable
    {
        private bool m_bSuspendEvents;

        private string m_sourceName = "";

        private string m_targetName = "";

        private IFilterExpression m_filterCondition;

        public IFilterExpression Condition
        {
            get { return this.m_filterCondition; }
            set
            {
                this.m_filterCondition = value;
                this.FirePropertyChanged("Condition");
            }
        }

        public string SourceName
        {
            get { return this.m_sourceName; }
            set
            {
                this.m_sourceName = value;
                this.FirePropertyChanged("SourceName");
            }
        }

        public string TargetName
        {
            get { return this.m_targetName; }
            set
            {
                this.m_targetName = value;
                this.FirePropertyChanged("TargetName");
            }
        }

        public ConditionalMapping(XmlNode node)
        {
            if (node != null)
            {
                this.FromXML(node);
            }
        }

        public ConditionalMapping(string sSourceName, string sTargetName, IFilterExpression condition)
        {
            this.m_sourceName = sSourceName;
            this.m_targetName = sTargetName;
            this.m_filterCondition = condition;
        }

        private void FirePropertyChanged(string sPropName)
        {
            if (this.PropertiesChanged != null && !this.m_bSuspendEvents)
            {
                this.PropertiesChanged(this, new PropertyChangedEventArgs(sPropName));
            }
        }

        public void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = xmlNode.SelectSingleNode("//ConditionalMapping");
            if (xmlNodes != null)
            {
                this.m_sourceName = xmlNodes.SelectSingleNode("./Source").InnerText;
                this.m_targetName = xmlNodes.SelectSingleNode("./Target").InnerText;
                this.m_filterCondition =
                    FilterExpression.ParseExpression(xmlNodes.SelectSingleNode("./Condition").FirstChild);
            }
        }

        private string GetFirstTypeName(IFilterExpression iFilter)
        {
            if (!(iFilter is FilterExpression))
            {
                string firstTypeName = null;
                using (IEnumerator<IFilterExpression> enumerator = ((FilterExpressionList)iFilter).GetEnumerator())
                {
                    do
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }

                        firstTypeName = this.GetFirstTypeName(enumerator.Current);
                    } while (firstTypeName == null);
                }

                return firstTypeName;
            }

            string str = "";
            int num = 0;
            foreach (string appliesToType in ((FilterExpression)iFilter).AppliesToTypes)
            {
                num++;
                Type type = Type.GetType(appliesToType);
                str = string.Concat(str, ActionUtils.GetTypePluralizedName(type));
                if (num >= ((FilterExpression)iFilter).AppliesToTypes.Count)
                {
                    continue;
                }

                str = string.Concat(str, ", ");
            }

            return str;
        }

        public void ResumeUpdates()
        {
            this.m_bSuspendEvents = false;
            this.FirePropertyChanged("Multiple");
        }

        public void SuspendUpdates()
        {
            this.m_bSuspendEvents = true;
        }

        public override string ToString()
        {
            string logicString;
            if (this.Condition != null)
            {
                logicString = this.Condition.GetLogicString();
            }
            else
            {
                logicString = null;
            }

            string str = logicString;
            str = (str != "()" ? string.Concat(" on ", str) : "");
            return string.Concat(
                (this.SourceName == null || this.SourceName.Length <= 0 ? "<Source>" : this.SourceName), " to ",
                (this.TargetName == null || this.TargetName.Length <= 0 ? "<Target>" : this.TargetName), str);
        }

        public string ToXML()
        {
            string str;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    this.ToXML(xmlTextWriter);
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ConditionalMapping");
            xmlWriter.WriteElementString("Source", this.SourceName);
            xmlWriter.WriteElementString("Target", this.TargetName);
            xmlWriter.WriteStartElement("Condition");
            if (this.Condition == null)
            {
                xmlWriter.WriteRaw("<And/>");
            }
            else
            {
                this.Condition.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        public event PropertyChangedEventHandler PropertiesChanged;
    }
}