using Metalogix.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.Metabase.Options
{
    public class ExtractContentOptions : ActionOptions
    {
        private string m_sSearchExpression;

        private List<string> m_sSearchHistory = new List<string>(10);

        private bool m_bSearchHtml;

        private string m_sSource = string.Empty;

        private string m_sTarget = string.Empty;

        private ExtractContentOptions.ReturnFormat m_rformat;

        private ExtractContentOptions.SearchLogic m_sLogic = ExtractContentOptions.SearchLogic.Text;

        private ExtractContentOptions.ReturnDetail m_rdetail;

        private string m_sSeparator = string.Empty;

        private bool m_bMatchCase;

        private bool m_bMultiline;

        private bool m_bDisplayMultiItemWarning = true;

        private bool m_pRecursive;

        public ExtractContentOptions.ReturnDetail Detail
        {
            get { return this.m_rdetail; }
            set { this.m_rdetail = value; }
        }

        public bool DisplayMultiItemWarning
        {
            get { return this.m_bDisplayMultiItemWarning; }
            set { this.m_bDisplayMultiItemWarning = value; }
        }

        public ExtractContentOptions.ReturnFormat Format
        {
            get { return this.m_rformat; }
            set { this.m_rformat = value; }
        }

        public ExtractContentOptions.SearchLogic Logic
        {
            get { return this.m_sLogic; }
            set { this.m_sLogic = value; }
        }

        public bool MatchCase
        {
            get { return this.m_bMatchCase; }
            set { this.m_bMatchCase = value; }
        }

        public bool Multiline
        {
            get { return this.m_bMultiline; }
            set { this.m_bMultiline = value; }
        }

        public bool Recursive
        {
            get { return this.m_pRecursive; }
            set { this.m_pRecursive = value; }
        }

        public string SearchExpression
        {
            get { return this.m_sSearchExpression; }
            set { this.m_sSearchExpression = value; }
        }

        public List<string> SearchHistory
        {
            get { return this.m_sSearchHistory; }
            set { this.m_sSearchHistory = value; }
        }

        public bool SearchHtml
        {
            get { return this.m_bSearchHtml; }
            set { this.m_bSearchHtml = value; }
        }

        public string Separator
        {
            get { return this.m_sSeparator; }
            set { this.m_sSeparator = value; }
        }

        public string SourceProperty
        {
            get { return this.m_sSource; }
            set { this.m_sSource = value; }
        }

        public string TargetProperty
        {
            get { return this.m_sTarget; }
            set { this.m_sTarget = value; }
        }

        public ExtractContentOptions()
        {
        }

        public override void FromXML(XmlNode xmlNode)
        {
            bool flag;
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = xmlNode;
            if (xmlNodes.LocalName != "ExtractContentSettings")
            {
                xmlNodes = xmlNode.SelectSingleNode("./ExtractContentSettings");
            }

            if (xmlNodes == null)
            {
                xmlNodes = xmlNode.SelectSingleNode("//ExtractContentSettings");
                if (xmlNodes == null)
                {
                    return;
                }
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./SearchHtml");
            if (xmlNodes1 != null && xmlNodes1.HasChildNodes && xmlNodes1.FirstChild is XmlCDataSection)
            {
                try
                {
                    this.SearchHtml = bool.Parse(((XmlCDataSection)xmlNodes1.FirstChild).Value);
                }
                catch
                {
                }
            }

            XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./SearchExpression");
            if (xmlNodes2 != null && xmlNodes2.HasChildNodes && xmlNodes2.FirstChild is XmlCDataSection)
            {
                this.SearchExpression = ((XmlCDataSection)xmlNodes2.FirstChild).Value;
            }

            XmlNodeList xmlNodeLists = xmlNodes.SelectNodes("./SearchHistory/HistoryItem");
            if (xmlNodeLists.Count > 0)
            {
                this.m_sSearchHistory.Clear();
                int num = (xmlNodeLists.Count > 10 ? 10 : xmlNodeLists.Count);
                for (int i = 0; i < num; i++)
                {
                    this.m_sSearchHistory.Add(xmlNodeLists[i].InnerText);
                }
            }

            XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./TargetProperty");
            if (xmlNodes3 != null)
            {
                this.TargetProperty = xmlNodes3.InnerText;
            }

            XmlNode xmlNodes4 = xmlNodes.SelectSingleNode("./SourceProperty");
            if (xmlNodes4 != null)
            {
                this.SourceProperty = xmlNodes4.InnerText;
            }

            XmlNode xmlNodes5 = xmlNodes.SelectSingleNode("./SearchLogic");
            if (xmlNodes5 != null)
            {
                XmlAttribute itemOf = xmlNodes5.Attributes["Value"];
                if (itemOf != null)
                {
                    string value = itemOf.Value;
                    string str = value;
                    if (value != null)
                    {
                        if (str == "Xpath")
                        {
                            this.Logic = ExtractContentOptions.SearchLogic.XPath;
                        }
                        else if (str == "Regex")
                        {
                            this.Logic = ExtractContentOptions.SearchLogic.RegEx;
                        }
                        else if (str == "Text")
                        {
                            this.Logic = ExtractContentOptions.SearchLogic.Text;
                        }
                    }
                }

                XmlAttribute xmlAttribute = xmlNodes5.Attributes["MatchCase"];
                if (xmlAttribute != null)
                {
                    try
                    {
                        this.MatchCase = bool.Parse(xmlAttribute.Value);
                    }
                    catch
                    {
                    }
                }

                XmlAttribute itemOf1 = xmlNodes5.Attributes["Multiline"];
                if (itemOf1 != null)
                {
                    try
                    {
                        this.Multiline = bool.Parse(itemOf1.Value);
                    }
                    catch
                    {
                    }
                }

                XmlAttribute xmlAttribute1 = xmlNodes5.Attributes["Recursive"];
                if (xmlAttribute1 != null)
                {
                    try
                    {
                        this.Recursive = bool.Parse(xmlAttribute1.Value);
                    }
                    catch
                    {
                    }
                }
            }

            XmlNode xmlNodes6 = xmlNodes.SelectSingleNode("./ReportDetail");
            if (xmlNodes6 != null)
            {
                XmlAttribute itemOf2 = xmlNodes6.Attributes["Value"];
                if (itemOf2 != null)
                {
                    string value1 = itemOf2.Value;
                    string str1 = value1;
                    if (value1 != null)
                    {
                        if (str1 == "FirstMatch")
                        {
                            this.Detail = ExtractContentOptions.ReturnDetail.FirstMatch;
                        }
                        else if (str1 == "AllMatches")
                        {
                            this.Detail = ExtractContentOptions.ReturnDetail.AllMatches;
                        }
                        else if (str1 == "Count")
                        {
                            this.Detail = ExtractContentOptions.ReturnDetail.CountsOnly;
                        }
                    }
                }

                XmlNode xmlNodes7 = xmlNodes6.SelectSingleNode("./Separator");
                if (xmlNodes7 is XmlElement && xmlNodes7.HasChildNodes)
                {
                    XmlCDataSection firstChild = xmlNodes7.FirstChild as XmlCDataSection;
                    if (firstChild != null)
                    {
                        try
                        {
                            this.Separator = firstChild.Value;
                            string separator = this.Separator;
                            string str2 = separator;
                            if (separator != null)
                            {
                                if (str2 == "\\t")
                                {
                                    this.Separator = "\t";
                                }
                                else if (str2 == "\\r\\n")
                                {
                                    this.Separator = "\r\n";
                                }
                                else if (str2 == "\\n")
                                {
                                    this.Separator = "\n";
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            XmlNode xmlNodes8 = xmlNodes.SelectSingleNode("./ReportFormat");
            if (xmlNodes8 != null)
            {
                XmlAttribute xmlAttribute2 = xmlNodes8.Attributes["Value"];
                if (xmlAttribute2 != null)
                {
                    string value2 = xmlAttribute2.Value;
                    string str3 = value2;
                    if (value2 != null)
                    {
                        if (str3 == "Text")
                        {
                            this.Format = ExtractContentOptions.ReturnFormat.Text;
                        }
                        else if (str3 == "InnerXml")
                        {
                            this.Format = ExtractContentOptions.ReturnFormat.InnerXML;
                        }
                        else if (str3 == "OuterXml")
                        {
                            this.Format = ExtractContentOptions.ReturnFormat.OuterXML;
                        }
                    }
                }
            }

            XmlNode xmlNodes9 = xmlNodes.SelectSingleNode("./MultiItemWarning");
            if (xmlNodes9 != null && bool.TryParse(xmlNodes9.InnerText, out flag))
            {
                this.m_bDisplayMultiItemWarning = flag;
            }
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("ExtractContentSettings");
            xmlTextWriter.WriteStartElement("SearchHtml");
            xmlTextWriter.WriteCData(this.SearchHtml.ToString());
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("SearchExpression");
            xmlTextWriter.WriteCData(this.SearchExpression);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("SearchHistory");
            if (this.SearchHistory != null)
            {
                foreach (object searchHistory in this.SearchHistory)
                {
                    if (searchHistory == null)
                    {
                        continue;
                    }

                    xmlTextWriter.WriteElementString("HistoryItem", searchHistory.ToString());
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteElementString("TargetProperty", this.TargetProperty);
            xmlTextWriter.WriteElementString("SourceProperty", this.SourceProperty);
            xmlTextWriter.WriteStartElement("SearchLogic");
            xmlTextWriter.WriteStartAttribute("Value");
            switch (this.Logic)
            {
                case ExtractContentOptions.SearchLogic.XPath:
                {
                    xmlTextWriter.WriteString("Xpath");
                    break;
                }
                case ExtractContentOptions.SearchLogic.RegEx:
                {
                    xmlTextWriter.WriteString("Regex");
                    break;
                }
                case ExtractContentOptions.SearchLogic.Text:
                {
                    xmlTextWriter.WriteString("Text");
                    break;
                }
            }

            xmlTextWriter.WriteEndAttribute();
            xmlTextWriter.WriteAttributeString("MatchCase", this.MatchCase.ToString());
            xmlTextWriter.WriteAttributeString("Multiline", this.Multiline.ToString());
            xmlTextWriter.WriteAttributeString("Recursive", this.Recursive.ToString());
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("ReportDetail");
            xmlTextWriter.WriteStartAttribute("Value");
            switch (this.Detail)
            {
                case ExtractContentOptions.ReturnDetail.FirstMatch:
                {
                    xmlTextWriter.WriteString("FirstMatch");
                    break;
                }
                case ExtractContentOptions.ReturnDetail.AllMatches:
                {
                    xmlTextWriter.WriteString("AllMatches");
                    break;
                }
                case ExtractContentOptions.ReturnDetail.CountsOnly:
                {
                    xmlTextWriter.WriteString("Count");
                    break;
                }
            }

            xmlTextWriter.WriteEndAttribute();
            xmlTextWriter.WriteStartElement("Separator");
            string separator = this.Separator;
            string str = this.Separator;
            string str1 = str;
            if (str != null)
            {
                if (str1 == "\t")
                {
                    separator = "\\t";
                }
                else if (str1 == "\r\n")
                {
                    separator = "\\r\\n";
                }
                else if (str1 == "\n")
                {
                    separator = "\\n";
                }
            }

            xmlTextWriter.WriteCData(separator);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("ReportFormat");
            xmlTextWriter.WriteStartAttribute("Value");
            switch (this.Format)
            {
                case ExtractContentOptions.ReturnFormat.Text:
                {
                    xmlTextWriter.WriteString("Text");
                    break;
                }
                case ExtractContentOptions.ReturnFormat.InnerXML:
                {
                    xmlTextWriter.WriteString("InnerXml");
                    break;
                }
                case ExtractContentOptions.ReturnFormat.OuterXML:
                {
                    xmlTextWriter.WriteString("OuterXml");
                    break;
                }
            }

            xmlTextWriter.WriteEndAttribute();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteElementString("MultiItemWarning", this.DisplayMultiItemWarning.ToString());
            xmlTextWriter.WriteEndElement();
        }

        public enum ReturnDetail
        {
            FirstMatch,
            AllMatches,
            CountsOnly
        }

        public enum ReturnFormat
        {
            [Description("Text")] Text,
            [Description("Inner XML")] InnerXML,
            [Description("Outer XML")] OuterXML
        }

        public enum SearchLogic
        {
            XPath,
            RegEx,
            Text
        }

        private struct XmlNames
        {
            public const string EXTRACT_SETTINGS = "ExtractContentSettings";

            public const string SEARCH_HTML = "SearchHtml";

            public const string SEARCH_EXPRESSION = "SearchExpression";

            public const string SEARCH_HISTORY = "SearchHistory";

            public const string HISTORY_ITEM = "HistoryItem";

            public const string SEARCH_LOGIC = "SearchLogic";

            public const string LOGIC_XPATH = "Xpath";

            public const string LOGIC_REGEX = "Regex";

            public const string LOGIC_TEXT = "Text";

            public const string MATCH_CASE = "MatchCase";

            public const string MULTILINE = "Multiline";

            public const string RECURSIVE = "Recursive";

            public const string REPORT_DETAIL = "ReportDetail";

            public const string DETAIL_COUNT = "Count";

            public const string DETAIL_FIRST_MATCH = "FirstMatch";

            public const string DETAIL_ALL_MATCHES = "AllMatches";

            public const string SEPARATOR = "Separator";

            public const string REPORT_FORMAT = "ReportFormat";

            public const string FORMAT_TEXT = "Text";

            public const string FORMAT_OUTER_XML = "OuterXml";

            public const string FORMAT_INNER_XML = "InnerXml";

            public const string TARGET_PROPERTY = "TargetProperty";

            public const string SOURCE_PROPERTY = "SourceProperty";

            public const string VALUE = "Value";

            public const string MULTI_ITEM_WARNING = "MultiItemWarning";
        }
    }
}