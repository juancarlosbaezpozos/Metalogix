using Metalogix.Actions;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Metalogix.Metabase
{
    [Image("Metalogix.UI.Metabase.Icons.Grid.ico")]
    public class Workspace : MetabaseObject, IHasRecords
    {
        private static string s_lock;

        private readonly string[] m_saIllegalChars = new string[] { "\n", "\t", "\r", "\0" };

        private bool m_bReadOnly;

        private RecordPropertyDescriptorList m_properties;

        private RecordList m_recordList;

        private PropertyDescriptor m_propBrowserViewFrame1Property;

        private PropertyDescriptor m_propBrowserViewFrame2Property;

        private List<Workspace.WorkspaceNodeView> m_views = new List<Workspace.WorkspaceNodeView>();

        private static DataColumn[] s_columns;

        public Type BaseType
        {
            get
            {
                object fullName;
                object item = base.Data["BaseType"];
                if (item is DBNull)
                {
                    return null;
                }

                if (item as string == "Metalogix.Metabase.Record")
                {
                    fullName = typeof(Record).FullName;
                }
                else
                {
                    fullName = item;
                }

                item = fullName;
                return Type.GetType((string)item);
            }
        }

        public PropertyDescriptor BrowserViewFrame1Property
        {
            get
            {
                if (this.m_propBrowserViewFrame1Property != null)
                {
                    return this.m_propBrowserViewFrame1Property;
                }

                return this.GetProperties().Find("SourceURL", true);
            }
            set
            {
                this.m_propBrowserViewFrame1Property = value;
                this.SaveSettings();
            }
        }

        public PropertyDescriptor BrowserViewFrame2Property
        {
            get
            {
                if (this.m_propBrowserViewFrame2Property != null)
                {
                    return this.m_propBrowserViewFrame2Property;
                }

                return this.GetProperties().Find("ResolvedURL", true);
            }
            set
            {
                this.m_propBrowserViewFrame2Property = value;
                this.SaveSettings();
            }
        }

        public DateTime DateCreated
        {
            get
            {
                if (base.Data["DateCreated"] is DBNull)
                {
                    return DateTime.MinValue;
                }

                return (DateTime)base.Data["DateCreated"];
            }
        }

        public DateTime DateModified
        {
            get
            {
                if (base.Data["DateModified"] is DBNull)
                {
                    return DateTime.MinValue;
                }

                return (DateTime)base.Data["DateModified"];
            }
        }

        public string ID
        {
            get { return (string)base.Data["WorkspaceID"]; }
        }

        public string Name
        {
            get
            {
                if (base.Data.RowState != DataRowState.Deleted)
                {
                    if (base.Data["Name"] is DBNull)
                    {
                        return null;
                    }

                    return (string)base.Data["Name"];
                }

                object item = base.Data["Name", DataRowVersion.Original];
                if (item == DBNull.Value)
                {
                    return null;
                }

                return (string)item;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                int maxLength = base.Data.Table.Columns["Name"].MaxLength;
                string str = (value.Length > maxLength ? value.Substring(0, maxLength) : value);
                string[] mSaIllegalChars = this.m_saIllegalChars;
                for (int i = 0; i < (int)mSaIllegalChars.Length; i++)
                {
                    str = str.Replace(mSaIllegalChars[i], string.Empty);
                }

                base.Data["Name"] = str;
                base.Data["DateModified"] = DateTime.Now;
                if (this.NameChanged != null)
                {
                    this.NameChanged(str);
                }
            }
        }

        public string PropertySummaries
        {
            get
            {
                if (base.Data["PropertySummaries"] is DBNull)
                {
                    return null;
                }

                return (string)base.Data["PropertySummaries"];
            }
            set { base.Data["PropertySummaries"] = value; }
        }

        public bool ReadOnly
        {
            get { return this.m_bReadOnly; }
            set
            {
                this.m_bReadOnly = value;
                if (this.ReadOnlyChanged != null)
                {
                    this.ReadOnlyChanged();
                }
            }
        }

        public RecordList Records
        {
            get
            {
                if (this.m_recordList == null)
                {
                    DataSet dataSet = base.Connection.Adapter.FetchItems(this.ID);
                    if (dataSet == null)
                    {
                        return null;
                    }

                    RecordPropertyDescriptorList properties = (RecordPropertyDescriptorList)this.GetProperties();
                    this.m_recordList = new RecordList(this, properties, dataSet);
                }

                return this.m_recordList;
            }
        }

        internal static DataColumn[] Schema
        {
            get
            {
                if (Workspace.s_columns == null)
                {
                    Workspace.BuildColumns();
                }

                return Workspace.s_columns;
            }
        }

        public List<Workspace.WorkspaceNodeView> Views
        {
            get { return this.m_views; }
        }

        static Workspace()
        {
            Workspace.s_lock = string.Empty;
        }

        public Workspace(MetabaseConnection connection, DataRow dataRow) : base(connection, null, dataRow)
        {
            this.LoadSettings();
            this.m_views.Add(new Workspace.WorkspaceNodeView("SourceURL"));
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type)
        {
            return this.AddProperty(sPropName, type, "Misc");
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type, string sCategory)
        {
            return this.AddProperty(sPropName, type, sCategory, null);
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type, string sCategory, string sDisplayName)
        {
            return this.AddProperty(sPropName, type, sCategory, sDisplayName, null);
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type, string sCategory, string sDisplayName,
            string sDescription)
        {
            return this.AddProperty(sPropName, type, sCategory, sDisplayName, sDescription, true);
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type, string sCategory, string sDisplayName,
            string sDescription, bool bCommitChanges)
        {
            return this.AddProperty(sPropName, type, sCategory, sDisplayName, sDescription, bCommitChanges, false,
                true);
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type, string sCategory, string sDisplayName,
            string sDescription, bool bCommitChanges, bool bGenerateUniqueSuffix)
        {
            return this.AddProperty(sPropName, type, sCategory, sDisplayName, sDescription, bCommitChanges,
                bGenerateUniqueSuffix, true);
        }

        public PropertyDescriptor AddProperty(string sPropName, Type type, string sCategory, string sDisplayName,
            string sDescription, bool bCommitChanges, bool bGenerateUniqueSuffix, bool bAddToSelectedView)
        {
            if (sPropName == null)
            {
                throw new ArgumentNullException("sPropName");
            }

            if (sCategory == null)
            {
                throw new ArgumentNullException("sCategory");
            }

            if (sPropName == string.Empty)
            {
                throw new ArgumentException("sPropName is empty.");
            }

            if (sCategory == string.Empty)
            {
                throw new ArgumentException("sCategory is empty.");
            }

            string[] mSaIllegalChars = this.m_saIllegalChars;
            for (int i = 0; i < (int)mSaIllegalChars.Length; i++)
            {
                string str = mSaIllegalChars[i];
                if (sPropName.Contains(str))
                {
                    throw new ArgumentException(string.Concat("sPropName contains illegal character: <", str, ">."));
                }

                if (sCategory.Contains(str))
                {
                    throw new ArgumentException(string.Concat("sCategory contains illegal character: <", str, ">."));
                }
            }

            RecordPropertyDescriptorList properties = (RecordPropertyDescriptorList)this.GetProperties();
            string str1 = sPropName;
            if (bGenerateUniqueSuffix)
            {
                int num = 0;
                foreach (PropertyDescriptor property in properties)
                {
                    if (!property.Name.StartsWith(str1))
                    {
                        continue;
                    }

                    int num1 = 0;
                    try
                    {
                        num1 = Convert.ToInt32(property.Name.Substring(str1.Length));
                    }
                    catch
                    {
                    }

                    if (num1 < num)
                    {
                        continue;
                    }

                    num = num1 + 1;
                }

                str1 = string.Concat(str1, num.ToString());
            }

            RecordPropertyDescriptor recordPropertyDescriptor = new RecordPropertyDescriptor(str1, type);
            recordPropertyDescriptor.SetCategory(sCategory);
            if (!string.IsNullOrEmpty(sDisplayName))
            {
                recordPropertyDescriptor.SetDisplayName(sDisplayName);
            }

            if (!string.IsNullOrEmpty(sDescription))
            {
                recordPropertyDescriptor.SetDescription(sDescription);
            }

            properties.Add(recordPropertyDescriptor, sCategory);
            if (bAddToSelectedView)
            {
                ViewList views = this.GetViews();
                View selectedView = views.SelectedView;
                selectedView.ViewProperties.Find(recordPropertyDescriptor.Name).IsDisplayed = true;
                this.SetViews(views);
            }

            this.SetProperties(properties);
            if (bCommitChanges)
            {
                this.CommitChanges();
            }

            return recordPropertyDescriptor;
        }

        private static void BuildColumns()
        {
            Workspace.s_columns = new DataColumn[8];
            DataColumn dataColumn = new DataColumn("WorkspaceID", typeof(string))
            {
                MaxLength = 36
            };
            Workspace.s_columns[0] = dataColumn;
            DataColumn dataColumn1 = new DataColumn("Name", typeof(string))
            {
                MaxLength = 255
            };
            Workspace.s_columns[1] = dataColumn1;
            DataColumn dataColumn2 = new DataColumn("BaseType", typeof(string))
            {
                MaxLength = -1
            };
            Workspace.s_columns[2] = dataColumn2;
            DataColumn dataColumn3 = new DataColumn("CustomPropertyDefinitions", typeof(string))
            {
                MaxLength = -1
            };
            Workspace.s_columns[3] = dataColumn3;
            DataColumn dataColumn4 = new DataColumn("ViewDefinitions", typeof(string))
            {
                MaxLength = -1
            };
            Workspace.s_columns[4] = dataColumn4;
            DataColumn dataColumn5 = new DataColumn("PropertySummaries", typeof(string))
            {
                MaxLength = -1
            };
            Workspace.s_columns[5] = dataColumn5;
            DataColumn dataColumn6 = new DataColumn("DateCreated", typeof(DateTime))
            {
                MaxLength = -1
            };
            Workspace.s_columns[6] = dataColumn6;
            DataColumn dataColumn7 = new DataColumn("DateModified", typeof(DateTime))
            {
                MaxLength = -1
            };
            Workspace.s_columns[7] = dataColumn7;
        }

        public void CommitChanges()
        {
            base.Connection.Adapter.CommitWorkspace(base.Data);
        }

        public void Delete()
        {
            base.Connection.Workspaces.Remove(this);
            base.Dispose();
        }

        public override void Dispose(bool bForceGarbageCollection)
        {
            this.m_recordList = null;
            this.m_views = null;
            this.m_properties = null;
            if (bForceGarbageCollection)
            {
                GC.Collect();
            }
        }

        public PropertyDescriptor EnsureProperty(string sPropName, Type type, string sCategory = null)
        {
            PropertyDescriptor propertyDescriptor;
            RecordPropertyDescriptorList properties = (RecordPropertyDescriptorList)this.GetProperties();
            propertyDescriptor = (!string.IsNullOrEmpty(sCategory)
                ? properties.Find(sPropName, sCategory) ?? this.AddProperty(sPropName, type, sCategory)
                : properties.Find(sPropName) ?? this.AddProperty(sPropName, type));
            return propertyDescriptor;
        }

        public void ExpandPropertyStorage(RecordPropertyDescriptor ipd)
        {
            if (ipd == null)
            {
                return;
            }

            if (ipd.PropertyType == typeof(string))
            {
                base.Connection.Adapter.ExpandTextStorage(ipd.Name, this.ID);
            }

            RecordPropertyDescriptorList properties = (RecordPropertyDescriptorList)this.GetProperties();
            RecordPropertyDescriptor item = properties[ipd.Name] as RecordPropertyDescriptor;
            if (item != null)
            {
                item.SetPropertyType(typeof(TextMoniker));
                ipd.SetPropertyType(typeof(TextMoniker));
                this.SetProperties(properties);
                this.CommitChanges();
            }
        }

        public Record FetchSingleRecord(string sourceUrl)
        {
            DataSet dataSet = base.Connection.Adapter.FetchSingleItem(this.ID, sourceUrl);
            RecordList recordLists = new RecordList(this, (RecordPropertyDescriptorList)this.GetProperties(), dataSet);
            if (recordLists.Count != 1)
            {
                return recordLists.AddNew(Guid.NewGuid(), sourceUrl);
            }

            IEnumerator enumerator = recordLists.GetEnumerator();
            enumerator.MoveNext();
            return (Record)enumerator.Current;
        }

        public Record FindRecord(string sPropertyName, object oValueToFind, PropertyFilterOperand operand,
            bool bCaseSensitive)
        {
            Record record;
            if (sPropertyName == null)
            {
                throw new ArgumentNullException("sPropertyName");
            }

            if (string.IsNullOrEmpty(sPropertyName))
            {
                throw new ArgumentException("sPropertyName is empty.");
            }

            if (oValueToFind == null)
            {
                return null;
            }

            try
            {
                foreach (Record record1 in this.Records)
                {
                    string str = record1.Properties[sPropertyName].Value.ToString();
                    string lower = oValueToFind.ToString();
                    if (!bCaseSensitive)
                    {
                        str = str.ToLower();
                        lower = lower.ToLower();
                    }

                    switch (operand)
                    {
                        case PropertyFilterOperand.Equals:
                        {
                            if (str != lower)
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        case PropertyFilterOperand.StartsWith:
                        {
                            if (!str.StartsWith(lower))
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        case PropertyFilterOperand.NotContains:
                        {
                            if (str.IndexOf(lower) >= 0)
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        case PropertyFilterOperand.Contains:
                        {
                            if (str.IndexOf(lower) < 0)
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        case PropertyFilterOperand.ContainedBy:
                        {
                            if (lower.IndexOf(str) < 0)
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        case PropertyFilterOperand.RegularExpression:
                        {
                            if (!Regex.Match(str, lower).Success)
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        case PropertyFilterOperand.EndsWith:
                        {
                            if (!str.EndsWith(lower))
                            {
                                continue;
                            }

                            record = record1;
                            return record;
                        }
                        default:
                        {
                            continue;
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }

            return record;
        }

        public Record GetFirstRecord()
        {
            if (this.Records.Count == 0)
            {
                return null;
            }

            return (Record)this.Records[0];
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            string item;
            lock (Workspace.s_lock)
            {
                if (this.m_properties == null)
                {
                    if (base.Data["CustomPropertyDefinitions"] is DBNull)
                    {
                        item = null;
                    }
                    else
                    {
                        item = (string)base.Data["CustomPropertyDefinitions"];
                    }

                    this.m_properties = new RecordPropertyDescriptorList(this, item);
                }
            }

            return this.m_properties;
        }

        public int GetRecordCount()
        {
            return this.Records.Count;
        }

        public RecordCollection GetRecords()
        {
            RecordCollection recordCollection = new RecordCollection();
            foreach (Record record in this.Records)
            {
                recordCollection.Add(record);
            }

            return recordCollection;
        }

        public ViewList GetViews()
        {
            string str = null;
            object item = base.Data["ViewDefinitions"];
            if (!(item is DBNull))
            {
                str = (string)item;
            }

            return new ViewList(this, this.GetProperties(), str);
        }

        public void LoadSettings()
        {
            try
            {
                string str = base.Data["Settings"].ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(str);
                    XmlNode xmlNodes = xmlDocument.SelectSingleNode("//BrowserViewFrame1Property");
                    if (xmlNodes != null)
                    {
                        this.m_propBrowserViewFrame1Property = this.GetProperties().Find(xmlNodes.InnerText, true);
                    }

                    XmlNode xmlNodes1 = xmlDocument.SelectSingleNode("//BrowserViewFrame2Property");
                    if (xmlNodes1 != null)
                    {
                        this.m_propBrowserViewFrame2Property = this.GetProperties().Find(xmlNodes1.InnerText, true);
                    }
                }
            }
            catch (Exception exception)
            {
            }
        }

        private void SaveSettings()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            xmlTextWriter.WriteStartElement("Settings");
            xmlTextWriter.WriteElementString("BrowserViewFrame1Property", this.BrowserViewFrame1Property.Name);
            if (this.BrowserViewFrame2Property != null)
            {
                xmlTextWriter.WriteElementString("BrowserViewFrame2Property", this.BrowserViewFrame2Property.Name);
            }

            xmlTextWriter.WriteEndElement();
            if (base.Data.Table.Columns["Settings"] != null)
            {
                base.Data["Settings"] = stringBuilder.ToString();
                base.Data["DateModified"] = DateTime.Now;
            }

            if (this.SettingsChanged != null)
            {
                this.SettingsChanged();
            }
        }

        public void SetProperties(RecordPropertyDescriptorList properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.m_properties = properties;
            base.Data["CustomPropertyDefinitions"] = properties.ToXml();
            List<RecordPropertyDescriptor> recordPropertyDescriptors = new List<RecordPropertyDescriptor>();
            foreach (PropertyDescriptor property in properties)
            {
                if (!(property is RecordPropertyDescriptor) ||
                    ((RecordPropertyDescriptor)property).DefaultValue == null)
                {
                    continue;
                }

                recordPropertyDescriptors.Add((RecordPropertyDescriptor)property);
            }

            if (recordPropertyDescriptors.Count > 0)
            {
                foreach (Record record in this.Records)
                {
                    foreach (RecordPropertyDescriptor recordPropertyDescriptor in recordPropertyDescriptors)
                    {
                        object defaultValue = recordPropertyDescriptor.DefaultValue;
                        if (recordPropertyDescriptor.GetValue(record) != null)
                        {
                            continue;
                        }

                        recordPropertyDescriptor.SetValue(record, defaultValue);
                    }
                }

                this.Records.CommitChanges();
            }

            if (this.PropertiesChanged != null)
            {
                this.PropertiesChanged();
            }
        }

        public void SetViews(ViewList viewList)
        {
            if (viewList == null)
            {
                throw new ArgumentNullException("viewList");
            }

            base.Data["ViewDefinitions"] = viewList.ToXml();
            if (this.ViewsChanged != null)
            {
                this.ViewsChanged();
            }
        }

        public override string ToString()
        {
            if (this.Name == null)
            {
                return string.Empty;
            }

            return this.Name;
        }

        public event Workspace.NameChangeHandler NameChanged;

        public event Workspace.PropertiesChangedHandler PropertiesChanged;

        public event Workspace.ReadOnlyChangedHandler ReadOnlyChanged;

        public event Workspace.SettingsChangedHandler SettingsChanged;

        public event Workspace.ViewsChangedHandler ViewsChanged;

        public struct FieldNames
        {
            public const string WorkspaceID = "WorkspaceID";

            public const string Name = "Name";

            public const string CustomPropertyDefinitions = "CustomPropertyDefinitions";

            public const string BaseType = "BaseType";

            public const string ViewDefinitions = "ViewDefinitions";

            public const string PropertySummaries = "PropertySummaries";

            public const string DateCreated = "DateCreated";

            public const string DateModified = "DateModified";
        }

        public delegate void NameChangeHandler(string sName);

        public delegate void PropertiesChangedHandler();

        public delegate void ReadOnlyChangedHandler();

        public delegate void SettingsChangedHandler();

        public delegate void ViewsChangedHandler();

        public class WorkspaceNodeView
        {
            private string m_sPropertyName;

            public string PropertyName
            {
                get { return this.m_sPropertyName; }
            }

            public WorkspaceNodeView(string sPropertyName)
            {
                this.m_sPropertyName = sPropertyName;
            }
        }
    }
}