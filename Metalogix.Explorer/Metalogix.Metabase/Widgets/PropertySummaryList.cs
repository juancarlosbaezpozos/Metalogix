using Metalogix.Metabase;
using Metalogix.Metabase.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Metabase.Widgets
{
    public class PropertySummaryList : PropertyValueSummaryList
    {
        private bool m_bSummarizeRequired;

        private ListView m_dataSource;

        private Hashtable m_expandedNodes = new Hashtable();

        public ListView DataSource
        {
            get { return this.m_dataSource; }
        }

        public Hashtable ExpandedNodes
        {
            get { return this.m_expandedNodes; }
        }

        public bool SummarizeRequired
        {
            get { return this.m_bSummarizeRequired; }
            set { this.m_bSummarizeRequired = value; }
        }

        public PropertySummaryList(ITypedList dataSource, string strPropertiesXML)
        {
            IList lists = dataSource as IList;
            if (lists == null)
            {
                throw new Exception("Input list was not of type: IList");
            }

            this.m_dataSource = new ListView(lists);
            this.m_dataSource.ListChanged += new ListChangedEventHandler(this.On_PropertySummaryList_ListChanged);
            PropertyDescriptorCollection itemProperties = dataSource.GetItemProperties(null);
            if (strPropertiesXML != null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(strPropertiesXML);
                foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//PropertySummary"))
                {
                    XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(".//PropertyName");
                    if (xmlNodes1 == null)
                    {
                        continue;
                    }

                    PropertyDescriptor propertyDescriptor = itemProperties.Find(xmlNodes1.InnerText, true);
                    if (propertyDescriptor == null)
                    {
                        continue;
                    }

                    PropertySummary propertySummary = new PropertySummary(propertyDescriptor);
                    XmlNode xmlNodes2 = xmlNodes.SelectSingleNode(".//Format");
                    if (xmlNodes2 != null)
                    {
                        propertySummary.Format = xmlNodes2.InnerText;
                    }

                    XmlNode xmlNodes3 = xmlNodes.SelectSingleNode(".//Separators");
                    if (xmlNodes3 != null)
                    {
                        char[] charArray = xmlNodes3.InnerText.ToCharArray();
                        if ((int)charArray.Length > 0)
                        {
                            propertySummary.Separators = charArray;
                        }
                    }

                    this.Add(propertySummary);
                }
            }
        }

        public override void Add(PropertyValueSummary propertyValueSummary)
        {
            PropertySummary propertySummary = propertyValueSummary as PropertySummary;
            if (propertySummary == null)
            {
                throw new Exception("Cannot add value that is not of type 'PropertySummary'");
            }

            base.Add(propertyValueSummary);
            propertySummary.ParentList = this;
        }

        private void FireListChanged(PropertySummaryList.PropertySummaryListChangeType changeType,
            PropertySummary itemChanged)
        {
            if (this.ListChanged != null)
            {
                this.ListChanged(changeType, itemChanged);
            }
        }

        private void On_PropertySummaryList_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.m_bSummarizeRequired = true;
        }

        public void SummarizeData()
        {
            lock (this)
            {
                DateTime now = DateTime.Now;
                PropertyDescriptorCollection itemProperties = this.DataSource.GetItemProperties(null);
                ArrayList arrayLists = new ArrayList(4);
                foreach (PropertySummary propertySummary in this)
                {
                    string name = propertySummary.PropertyDescriptor.Name;
                    if (itemProperties[name] != null)
                    {
                        continue;
                    }

                    arrayLists.Add(name);
                }

                foreach (string arrayList in arrayLists)
                {
                    base.Remove((PropertySummary)base[arrayList]);
                }

                foreach (PropertySummary propertySummary1 in this)
                {
                    propertySummary1.Clear();
                }

                for (int i = 0; i < this.DataSource.Count; i++)
                {
                    object item = this.DataSource[i];
                    foreach (PropertySummary propertySummary2 in this)
                    {
                        propertySummary2.SummarizeValue(item);
                    }
                }

                this.SummarizeRequired = false;
                if (this.DataSource.BaseList is RecordList)
                {
                    RecordList baseList = (RecordList)this.DataSource.BaseList;
                    if (!baseList.FillFactorsUpToDate)
                    {
                        DateTime dateTime = DateTime.Now;
                        baseList.UpdateFillFactors();
                    }
                }
            }

            this.FireListChanged(PropertySummaryList.PropertySummaryListChangeType.Reset, null);
        }

        public void SummarizeData(PropertySummary propertySummary)
        {
            lock (this)
            {
                DateTime now = DateTime.Now;
                propertySummary.Clear();
                for (int i = 0; i < this.DataSource.Count; i++)
                {
                    propertySummary.SummarizeValue(this.DataSource[i]);
                }

                this.SummarizeRequired = false;
            }

            this.FireListChanged(PropertySummaryList.PropertySummaryListChangeType.PropertyChanged, propertySummary);
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXml(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("PropertySummaryList");
            foreach (PropertySummary propertySummary in this)
            {
                propertySummary.ToXml(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }

        public event PropertySummaryList.ListChangedHandler ListChanged;

        public delegate void ListChangedHandler(PropertySummaryList.PropertySummaryListChangeType changeType,
            PropertySummary itemChanged);

        public enum PropertySummaryListChangeType
        {
            Reset,
            PropertyChanged,
            SortOrderChanged
        }
    }
}