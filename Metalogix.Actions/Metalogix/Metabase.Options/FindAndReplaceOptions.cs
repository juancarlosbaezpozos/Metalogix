using Metalogix.Actions;
using Metalogix.Metabase.Data;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.Metabase.Options
{
    public class FindAndReplaceOptions : ActionOptions
    {
        private string m_sFindValue;

        private IEnumerable m_findHistory;

        private string m_sReplaceValue;

        private IEnumerable m_replaceHistory;

        private string m_sProperty;

        private bool m_bMatchCase;

        private bool m_bMultiLine;

        private PropertyFilterOperand m_filterOperand = PropertyFilterOperand.Contains;

        private bool m_bIsXpathQuery;

        public PropertyFilterOperand FilterOperand
        {
            get { return this.m_filterOperand; }
            set { this.m_filterOperand = value; }
        }

        public IEnumerable FindHistory
        {
            get { return this.m_findHistory; }
            set { this.m_findHistory = value; }
        }

        public string FindValue
        {
            get { return this.m_sFindValue; }
            set { this.m_sFindValue = value; }
        }

        public bool IsXPathQuery
        {
            get { return this.m_bIsXpathQuery; }
            set { this.m_bIsXpathQuery = value; }
        }

        public bool MatchCase
        {
            get { return this.m_bMatchCase; }
            set { this.m_bMatchCase = value; }
        }

        public bool MultiLine
        {
            get { return this.m_bMultiLine; }
            set { this.m_bMultiLine = value; }
        }

        public string Property
        {
            get { return this.m_sProperty; }
            set { this.m_sProperty = value; }
        }

        public IEnumerable ReplaceHistory
        {
            get { return this.m_replaceHistory; }
            set { this.m_replaceHistory = value; }
        }

        public string ReplaceValue
        {
            get { return this.m_sReplaceValue; }
            set { this.m_sReplaceValue = value; }
        }

        public FindAndReplaceOptions()
        {
        }

        public override void FromXML(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = xmlNode.SelectSingleNode("//SearchAndReplaceOptions");
            if (xmlNodes == null)
            {
                return;
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./FindValue");
            if (xmlNodes1 != null)
            {
                this.m_sFindValue = xmlNodes1.InnerText;
            }

            XmlNodeList xmlNodeLists = xmlNodes.SelectNodes("./FindHistory/HistoryItem");
            if (xmlNodeLists.Count > 0)
            {
                int num = (xmlNodeLists.Count > 10 ? 10 : xmlNodeLists.Count);
                ArrayList arrayLists = new ArrayList(num);
                for (int i = 0; i < num; i++)
                {
                    arrayLists.Add(xmlNodeLists[i].InnerText);
                }

                this.m_findHistory = arrayLists;
            }

            XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./ReplaceVal");
            if (xmlNodes2 != null)
            {
                this.m_sReplaceValue = xmlNodes2.InnerText;
            }

            XmlNodeList xmlNodeLists1 = xmlNodes.SelectNodes("./ReplaceHistory/HistoryItem");
            if (xmlNodeLists1.Count > 0)
            {
                ArrayList arrayLists1 = new ArrayList();
                int num1 = (xmlNodeLists1.Count > 10 ? 10 : xmlNodeLists1.Count);
                for (int j = 0; j < num1; j++)
                {
                    arrayLists1.Add(xmlNodeLists1[j].InnerText);
                }

                this.m_replaceHistory = arrayLists1;
            }

            XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./MatchCase");
            if (xmlNodes3 != null)
            {
                bool.TryParse(xmlNodes3.InnerText, out this.m_bMatchCase);
            }

            XmlNode xmlNodes4 = xmlNodes.SelectSingleNode("./MultiLine");
            if (xmlNodes4 != null)
            {
                bool.TryParse(xmlNodes4.InnerText, out this.m_bMultiLine);
            }

            XmlNode xmlNodes5 = xmlNodes.SelectSingleNode("./SelectedField");
            if (xmlNodes5 != null)
            {
                this.m_sProperty = xmlNodes5.InnerText;
            }

            XmlNode xmlNodes6 = xmlNodes.SelectSingleNode("./SelectedOperand");
            if (xmlNodes6 != null)
            {
                try
                {
                    this.m_filterOperand =
                        (PropertyFilterOperand)Enum.Parse(typeof(PropertyFilterOperand), xmlNodes6.InnerText);
                }
                catch
                {
                    this.m_filterOperand = PropertyFilterOperand.Contains;
                }
            }

            XmlNode xmlNodes7 = xmlNodes.SelectSingleNode("./IsXPathQuery");
            bool.TryParse(xmlNodes7.InnerText, out this.m_bIsXpathQuery);
        }

        public override void ToXML(XmlWriter writer)
        {
            writer.WriteStartElement("SearchAndReplaceOptions");
            writer.WriteElementString("FindValue", this.m_sFindValue);
            writer.WriteStartElement("FindHistory");
            if (this.m_findHistory != null)
            {
                foreach (object mFindHistory in this.m_findHistory)
                {
                    if (mFindHistory == null)
                    {
                        continue;
                    }

                    writer.WriteElementString("HistoryItem", mFindHistory.ToString());
                }
            }

            writer.WriteEndElement();
            writer.WriteElementString("ReplaceVal", this.m_sReplaceValue);
            writer.WriteStartElement("ReplaceHistory");
            if (this.m_replaceHistory != null)
            {
                foreach (object mReplaceHistory in this.m_replaceHistory)
                {
                    if (mReplaceHistory == null)
                    {
                        continue;
                    }

                    writer.WriteElementString("HistoryItem", mReplaceHistory.ToString());
                }
            }

            writer.WriteEndElement();
            writer.WriteElementString("MatchCase", this.m_bMatchCase.ToString());
            writer.WriteElementString("MultiLine", this.m_bMultiLine.ToString());
            writer.WriteElementString("SelectedField", this.m_sProperty);
            writer.WriteElementString("SelectedOperand", this.m_filterOperand.ToString());
            writer.WriteElementString("IsXPathQuery", this.m_bIsXpathQuery.ToString());
            writer.WriteEndElement();
        }

        private struct XmlNames
        {
            public const string OPTIONS = "SearchAndReplaceOptions";

            public const string FIND_VALUE = "FindValue";

            public const string FIND_HISTORY = "FindHistory";

            public const string REPLACE_VALUE = "ReplaceVal";

            public const string REPLACE_HISTORY = "ReplaceHistory";

            public const string MATCH_CASE = "MatchCase";

            public const string MULTI_LINE = "MultiLine";

            public const string PROPERTY = "SelectedField";

            public const string FILTER_OPERAND = "SelectedOperand";

            public const string IS_XPATH_QUERY = "IsXPathQuery";

            public const string HISTORY_ITEM = "HistoryItem";
        }
    }
}