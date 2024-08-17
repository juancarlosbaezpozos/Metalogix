using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Widgets
{
    public class PropertyValueSummary
    {
        private Dictionary<string, int> m_dictionaryVariations = new Dictionary<string, int>(4);

        private PropertyValueSummary m_parentNode;

        private PropertySummary m_rootNode;

        private string m_strPropertyValue;

        private object m_oSortValue;

        private int m_iOccurrences;

        private int m_iOccurrencesExact;

        private int m_iTotalOccurrences;

        private readonly int m_iDepth;

        private PropertyValueSummaryList m_childNodes;

        private bool m_bHasChildren;

        public PropertyValueSummaryList Children
        {
            get
            {
                if (this.m_childNodes == null)
                {
                    this.m_childNodes = new PropertyValueSummaryList();
                }

                return this.m_childNodes;
            }
        }

        public int Depth
        {
            get { return this.m_iDepth; }
        }

        public bool HasChildren
        {
            get { return this.m_bHasChildren; }
        }

        public bool IsExpanded
        {
            get
            {
                if (this.m_rootNode == null || this.m_rootNode.ParentList == null)
                {
                    return true;
                }

                return this.m_rootNode.ParentList.ExpandedNodes.ContainsKey(this.Path);
            }
        }

        public static int MaxItems
        {
            get { return 400; }
        }

        public static string NullText
        {
            get { return "<Empty>"; }
        }

        public int NumOccurrences
        {
            get { return this.m_iOccurrences; }
            set { this.m_iOccurrences = value; }
        }

        public int NumOccurrencesExact
        {
            get { return this.m_iOccurrencesExact; }
            set { this.m_iOccurrencesExact = value; }
        }

        public static string OtherText
        {
            get { return "<Other>"; }
        }

        public PropertyValueSummary ParentNode
        {
            get { return this.m_parentNode; }
        }

        public string Path
        {
            get
            {
                return string.Concat((this.ParentNode != null ? this.ParentNode.Path : ""), "/",
                    this.PropertyStringValue);
            }
        }

        public List<string> PathVariations
        {
            get
            {
                List<string> strs = new List<string>(4);
                if (this.ParentNode == null || this.ParentNode.ParentNode == null)
                {
                    strs = new List<string>(this.Variations);
                }
                else
                {
                    foreach (string pathVariation in this.ParentNode.PathVariations)
                    {
                        string[] variations = this.Variations;
                        for (int i = 0; i < (int)variations.Length; i++)
                        {
                            strs.Add(string.Concat(pathVariation, variations[i]));
                        }
                    }
                }

                return strs;
            }
        }

        public int Percentages
        {
            get
            {
                if (this.ParentNode == null || this.RootNode == null || this.ParentNode.NumOccurrences <= 0)
                {
                    return 100;
                }

                return this.NumOccurrences * 100 / this.ParentNode.NumOccurrences;
            }
        }

        public int PercentagesTotal
        {
            get
            {
                this.UpdateTotalOccurrences();
                if (this.TotalOccurrences <= 0)
                {
                    return 100;
                }

                return this.NumOccurrences * 100 / this.TotalOccurrences;
            }
        }

        public virtual string PropertyStringValue
        {
            get { return this.m_strPropertyValue; }
        }

        public PropertySummary RootNode
        {
            get { return this.m_rootNode; }
            set { this.m_rootNode = value; }
        }

        public object SortValue
        {
            get { return this.m_oSortValue; }
        }

        public virtual string Text
        {
            get
            {
                string str;
                string str1;
                string empty = string.Empty;
                if (this.RootNode.ShowTotal)
                {
                    empty = this.NumOccurrencesExact.ToString();
                }
                else if (this.RootNode.ShowTotalWithChildren)
                {
                    empty = this.NumOccurrences.ToString();
                }

                if (this.RootNode.ShowPercentageToParent)
                {
                    if (string.IsNullOrEmpty(empty))
                    {
                        str1 = string.Concat(this.Percentages, "%");
                    }
                    else
                    {
                        object[] percentages = new object[] { empty, "/", this.Percentages, "%" };
                        str1 = string.Concat(percentages);
                    }

                    empty = str1;
                }
                else if (this.RootNode.ShowPercentageToTotal)
                {
                    if (string.IsNullOrEmpty(empty))
                    {
                        str = string.Concat(this.PercentagesTotal, "%");
                    }
                    else
                    {
                        object[] percentagesTotal = new object[] { empty, "/", this.PercentagesTotal, "%" };
                        str = string.Concat(percentagesTotal);
                    }

                    empty = str;
                }

                if (string.IsNullOrEmpty(empty))
                {
                    return this.PropertyStringValue;
                }

                return string.Concat(this.PropertyStringValue, " (", empty, ")");
            }
        }

        public int TotalOccurrences
        {
            get { return this.m_iTotalOccurrences; }
            set { this.m_iTotalOccurrences = value; }
        }

        public string ValuePath
        {
            get
            {
                return string.Concat(
                    (this.ParentNode == null || this.ParentNode.ParentNode == null
                        ? string.Empty
                        : string.Concat(this.ParentNode.ValuePath, "/")), this.PropertyStringValue);
            }
        }

        public string[] Variations
        {
            get
            {
                string[] strArrays = new string[this.m_dictionaryVariations.Keys.Count];
                this.m_dictionaryVariations.Keys.CopyTo(strArrays, 0);
                return strArrays;
            }
        }

        public PropertyValueSummary(string strPropertyValue, object oSortValue, PropertyValueSummary parentNode,
            PropertySummary rootNode, int iDepth)
        {
            this.m_strPropertyValue = strPropertyValue;
            this.m_oSortValue = oSortValue;
            this.m_parentNode = parentNode;
            this.RootNode = rootNode;
            this.m_iDepth = iDepth;
        }

        public PropertyValueSummary(string strPropertyValue, object oSortValue, PropertyValueSummary parentNode)
        {
            this.m_strPropertyValue = strPropertyValue;
            this.m_oSortValue = oSortValue;
            this.m_parentNode = parentNode;
        }

        public void AddVariation(string sVariation)
        {
            if (!this.m_dictionaryVariations.ContainsKey(sVariation))
            {
                this.m_dictionaryVariations[sVariation] = 1;
                return;
            }

            Dictionary<string, int> mDictionaryVariations = this.m_dictionaryVariations;
            Dictionary<string, int> strs = mDictionaryVariations;
            string str = sVariation;
            mDictionaryVariations[str] = strs[str] + 1;
        }

        public void Clear()
        {
            this.m_iOccurrences = 0;
            this.m_bHasChildren = false;
            this.m_childNodes = null;
        }

        public PropertyValueSummary Find(object oValue)
        {
            if (oValue == null)
            {
                return null;
            }

            PropertyValueSummary.PropertyValueSummaryKey propertyKey = this.GetPropertyKey(oValue.ToString());
            PropertyValueSummary item = this.Children[propertyKey.Key];
            if (item == null)
            {
                return null;
            }

            if (propertyKey.Remainder == null)
            {
                return item;
            }

            return item.Find(propertyKey.Remainder);
        }

        private PropertyValueSummary.PropertyValueSummaryKey GetPropertyKey(string sValue)
        {
            string empty = string.Empty;
            string str = string.Empty;
            int num = -1;
            string str1 = null;
            string str2 = null;
            char[] separators = this.m_rootNode.Separators;
            if (!string.IsNullOrEmpty(sValue))
            {
                string str3 = sValue.Trim(separators);
                if (str3.Length == 0)
                {
                    return null;
                }

                int num1 = sValue.IndexOf(str3);
                num = sValue.IndexOfAny(separators, num1);
                int num2 = str3.LastIndexOfAny(separators);
                str1 = (num2 <= 0 || str3.Length <= num2 ? str3 : str3.Substring(num2 + 1));
                str1 = str1.Trim();
                if (num < 0)
                {
                    empty = str3;
                    str = sValue;
                }
                else
                {
                    empty = sValue.Substring(num1, num - num1);
                    str = sValue.Substring(0, num);
                    str2 = sValue.Substring(num);
                }

                empty = empty.Trim();
                if (this.RootNode != null && !this.RootNode.CaseSensitive && empty != PropertyValueSummary.NullText)
                {
                    empty = empty.ToLower();
                }
            }
            else
            {
                empty = PropertyValueSummary.NullText;
            }

            return new PropertyValueSummary.PropertyValueSummaryKey(empty, str, str2, str1);
        }

        public void SummarizeChildValue(string sValue, object oSortValue)
        {
            object key;
            PropertyValueSummary.PropertyValueSummaryKey propertyKey = this.GetPropertyKey(sValue);
            if (propertyKey == null)
            {
                return;
            }

            this.m_bHasChildren = true;
            string rawKey = propertyKey.RawKey;
            if (this.Children.Count >= PropertyValueSummary.MaxItems)
            {
                rawKey = PropertyValueSummary.OtherText;
            }

            PropertyValueSummary propertyValueSummary = null;
            if (!this.Children.ContainsKey(propertyKey.Key))
            {
                if (oSortValue is string)
                {
                    key = propertyKey.Key;
                }
                else
                {
                    key = oSortValue;
                }

                object obj = key;
                propertyValueSummary =
                    new PropertyValueSummary(propertyKey.Key, obj, this, this.RootNode, this.m_iDepth + 1);
                propertyValueSummary.AddVariation(rawKey);
                this.Children.Add(propertyValueSummary);
            }
            else
            {
                propertyValueSummary = this.Children[propertyKey.Key];
                propertyValueSummary.AddVariation(rawKey);
            }

            if (propertyKey.LastPart == propertyValueSummary.PropertyStringValue &&
                this.Depth + 1 == propertyValueSummary.Depth)
            {
                PropertyValueSummary numOccurrencesExact = propertyValueSummary;
                numOccurrencesExact.NumOccurrencesExact = numOccurrencesExact.NumOccurrencesExact + 1;
            }

            PropertyValueSummary numOccurrences = propertyValueSummary;
            numOccurrences.NumOccurrences = numOccurrences.NumOccurrences + 1;
            if (propertyKey.Remainder != null && propertyKey.Remainder.Length > 0 &&
                this.Children.Count < PropertyValueSummary.MaxItems)
            {
                propertyValueSummary.SummarizeChildValue(propertyKey.Remainder, propertyKey.Remainder);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToString(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToString(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("PropertyValueSummary");
            xmlWriter.WriteAttributeString("PropertyStringValue", this.PropertyStringValue);
            xmlWriter.WriteAttributeString("NumOccurrences", this.NumOccurrences.ToString());
            xmlWriter.WriteAttributeString("HasChildren", this.HasChildren.ToString());
            xmlWriter.WriteAttributeString("IsExpanded", this.IsExpanded.ToString());
            foreach (PropertyValueSummary child in this.Children)
            {
                child.ToString(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }

        private void UpdateTotalOccurrences()
        {
            if (this.ParentNode == null)
            {
                this.TotalOccurrences = this.NumOccurrences;
                return;
            }

            if (this.ParentNode.NumOccurrences > this.TotalOccurrences)
            {
                this.ParentNode.UpdateTotalOccurrences();
            }

            this.TotalOccurrences = this.ParentNode.TotalOccurrences;
        }

        private class PropertyValueSummaryKey
        {
            private readonly string m_sKey;

            private readonly string m_sRawKey;

            private readonly string m_sRemainder;

            private readonly string m_sLastPart;

            public string Key
            {
                get { return this.m_sKey; }
            }

            public string LastPart
            {
                get { return this.m_sLastPart; }
            }

            public string RawKey
            {
                get { return this.m_sRawKey; }
            }

            public string Remainder
            {
                get { return this.m_sRemainder; }
            }

            public PropertyValueSummaryKey(string sKey, string sRawKey, string sRemainder, string sLastPart)
            {
                this.m_sKey = sKey;
                this.m_sRawKey = sRawKey;
                this.m_sRemainder = sRemainder;
                this.m_sLastPart = sLastPart;
            }
        }
    }
}