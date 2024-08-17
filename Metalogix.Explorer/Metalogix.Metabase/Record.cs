using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace Metalogix.Metabase
{
    [Image("Metalogix.UI.Metabase.Icons.Empty.ico")]
    public class Record : MetabaseObject, ICustomTypeDescriptor, IDataboundObject, IHasParentWorkspace
    {
        protected readonly DataTable m_dataTableItemProperties;

        protected readonly RecordList m_list;

        protected ArrayList m_dataChildren;

        protected PropertyDescriptorCollection m_typeDescProperties;

        protected PropertyDescriptorCollection m_properties;

        private object m_oLockEditing = new object();

        private ArrayList ChildRows
        {
            get
            {
                if (this.m_dataChildren == null)
                {
                    this.m_dataChildren =
                        ((IndexedDataTable)this.m_dataTableItemProperties).FindAllRows("ItemID", this.ID);
                }

                return this.m_dataChildren;
            }
        }

        [Browsable(false)]
        [ColumnOrdinal(1)]
        [Description("Metalogix Unique Item Identifyer.")]
        [DisplayName("Metalogix Item ID")]
        [FillFactor(0)]
        public virtual string ID
        {
            get
            {
                if (base.Data.RowState == DataRowState.Deleted)
                {
                    return string.Empty;
                }

                return (string)base.Data["ItemID"];
            }
        }

        [Browsable(false)]
        public RecordList ParentList
        {
            get { return this.m_list; }
        }

        [Browsable(false)]
        [Category("Item Properties")]
        [ColumnOrdinal(25)]
        [Description("The Workspace to which this node belongs")]
        [DisplayName("Parent Workspace")]
        public Workspace ParentWorkspace
        {
            get { return this.ParentList.ParentWorkspace; }
        }

        [Browsable(false)]
        public virtual RecordPropertyValueList Properties
        {
            get { return new RecordPropertyValueList(this, this.m_list.Properties); }
        }

        internal DataTable PropertiesData
        {
            get { return this.m_dataTableItemProperties; }
        }

        [Browsable(false)]
        [Category("Item Properties")]
        [ColumnOrdinal(6)]
        [Description("Date this Record was added to the Workspace")]
        [DisplayName("Record Date Created")]
        [FillFactor(0)]
        public DateTime RecordDateCreated
        {
            get { return (DateTime)base.Data["CreationDate"]; }
        }

        [Browsable(false)]
        [Category("Item Properties")]
        [ColumnOrdinal(7)]
        [Description("Date this Record was last modified")]
        [DisplayName("Record Date Modified")]
        [FillFactor(0)]
        public DateTime RecordDateModified
        {
            get { return (DateTime)base.Data["ModificationDate"]; }
        }

        [Browsable(false)]
        [Category("Item Properties")]
        [ColumnOrdinal(1)]
        [DefaultColumnWidth(50)]
        [DefaultDisplaySetting(true, false)]
        [Description("The number ID of this node in the Workspace")]
        [DisplayName("Item #")]
        [FillFactor(0)]
        public virtual int RecordNum
        {
            get { return (int)base.Data["ItemNum"]; }
        }

        [Browsable(true)]
        [Category("File Location Properties")]
        [ColumnOrdinal(2)]
        [DataGridBrowseable(false)]
        [DefaultColumnWidth(300)]
        [DefaultDisplaySetting(true, true)]
        [Description("URL used to retrieve the document.")]
        [DisplayName("Source URL")]
        [FillFactor(0)]
        [Indexed(true)]
        [ListItemVisible(false)]
        [ReadOnly(true)]
        public Url SourceURL
        {
            get { return this.GetValue("SourceURL", typeof(Url)) as Url; }
            set { this.SetValue("SourceURL", value, typeof(Url)); }
        }

        internal Record(RecordList list, DataRow dataRow, DataTable dataTableItemProperties) : base(
            list.ParentWorkspace, dataRow)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list", "Item must have a parent list.");
            }

            if (dataTableItemProperties == null)
            {
                throw new ArgumentNullException("dataTableItemProperties", "Item must have a parent list.");
            }

            this.m_list = list;
            this.m_dataTableItemProperties = dataTableItemProperties;
            foreach (PropertyDescriptor property in list.Properties)
            {
                if (!(property is RecordPropertyDescriptor))
                {
                    continue;
                }

                RecordPropertyDescriptor recordPropertyDescriptor = (RecordPropertyDescriptor)property;
                if (recordPropertyDescriptor.DefaultValue == null || this.GetValue(recordPropertyDescriptor.Name,
                        recordPropertyDescriptor.PropertyType) != null)
                {
                    continue;
                }

                this.SetValue(recordPropertyDescriptor.Name, recordPropertyDescriptor.DefaultValue,
                    recordPropertyDescriptor.PropertyType);
                this.CommitChanges();
            }
        }

        public void BeginEdit()
        {
            base.Data.BeginEdit();
        }

        public void CommitChanges()
        {
            if (base.Data.RowState == DataRowState.Unchanged)
            {
                return;
            }

            bool suspendEventNotifiaction = this.ParentList.SuspendEventNotifiaction;
            try
            {
                this.ParentList.SuspendEventNotifiaction = true;
                base.Connection.Adapter.CommitItem(base.Data);
                base.Connection.Adapter.CommitItemProperties(this.ChildRows);
            }
            finally
            {
                this.ParentList.SuspendEventNotifiaction = suspendEventNotifiaction;
            }
        }

        public void Delete()
        {
            if (this.ChildRows == null || this.ChildRows.Count == 0)
            {
                return;
            }

            DataRow[] dataRowArray = new DataRow[this.ChildRows.Count];
            this.ChildRows.CopyTo(dataRowArray);
            DataRow[] dataRowArray1 = dataRowArray;
            for (int i = 0; i < (int)dataRowArray1.Length; i++)
            {
                dataRowArray1[i].Delete();
            }

            base.Data.Delete();
        }

        private object DeserializeValue(string sPropertyName, object objRawValue, Type type)
        {
            return Record.DeserializeValue(this, sPropertyName, objRawValue, type);
        }

        public static object DeserializeValue(Record record, string sPropertyName, object objRawValue, Type type)
        {
            string str;
            if (type.Equals(typeof(TextMoniker)))
            {
                if (record == null)
                {
                    throw new ArgumentNullException("record",
                        "Argument record cannot be null if value to be deserialized is TextMoniker");
                }

                if (objRawValue == null)
                {
                    str = null;
                }
                else
                {
                    str = objRawValue.ToString();
                }

                return new TextMoniker(sPropertyName, str, record);
            }

            if (typeof(ISmartDataType).IsAssignableFrom(type))
            {
                if (record == null)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(sPropertyName))
                {
                    return null;
                }

                RecordPropertyDescriptor item = record.GetProperties()[sPropertyName] as RecordPropertyDescriptor;
                if (item == null)
                {
                    return null;
                }

                return DataTypeUtils.CreateInstance(type, item, objRawValue as string);
            }

            if (type.Equals(typeof(Url)))
            {
                string str1 = objRawValue as string;
                if (string.IsNullOrEmpty(str1))
                {
                    return null;
                }

                return new Url(str1);
            }

            if (type.Equals(typeof(int)))
            {
                if (objRawValue == null || objRawValue is string && string.IsNullOrEmpty(objRawValue as string))
                {
                    return 0;
                }

                return Convert.ToInt32(objRawValue);
            }

            if (type.Equals(typeof(decimal)))
            {
                if (objRawValue != null && (!(objRawValue is string) || !string.IsNullOrEmpty(objRawValue as string)))
                {
                    return Convert.ToDecimal(objRawValue);
                }

                return new decimal(0);
            }

            if (type.Equals(typeof(bool)))
            {
                if (string.IsNullOrEmpty(objRawValue as string))
                {
                    return false;
                }

                return bool.Parse(objRawValue.ToString());
            }

            if (objRawValue == null || objRawValue == DBNull.Value)
            {
                return null;
            }

            if (Serializer.Instance.IsRegistered(type))
            {
                object obj = Serializer.Instance.Deserialize(objRawValue.ToString(), type.FullName);
                if (obj != null)
                {
                    return obj;
                }
            }

            return objRawValue;
        }

        public override void Dispose(bool bForceGarbageCollection)
        {
            this.m_dataChildren = null;
            this.m_properties = null;
            this.m_typeDescProperties = null;
            if (bForceGarbageCollection)
            {
                GC.Collect();
            }
        }

        public void EndEdit()
        {
            base.Data.EndEdit();
        }

        public override bool Equals(object obj)
        {
            bool flag;
            Record record = obj as Record;
            if (record == null || record.ParentList != this.ParentList)
            {
                return false;
            }

            try
            {
                flag = this.ID.Equals(record.ID, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        internal DataRow FindPropertyRow(string sPropertyName)
        {
            DataRow dataRow;
            try
            {
                if (this.ChildRows != null)
                {
                    foreach (DataRow childRow in this.ChildRows)
                    {
                        if ((string)childRow["PropertyName"] != sPropertyName)
                        {
                            continue;
                        }

                        dataRow = childRow;
                        return dataRow;
                    }

                    dataRow = null;
                }
                else
                {
                    dataRow = null;
                }
            }
            catch (Exception exception)
            {
                dataRow = null;
            }

            return dataRow;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, typeof(Record), true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        internal static int GetFieldMaxLength(Type fieldType)
        {
            if (fieldType.Equals(typeof(string)))
            {
                return 255;
            }

            if (fieldType.Equals(typeof(Url)))
            {
                return 2083;
            }

            return 0;
        }

        public override int GetHashCode()
        {
            string empty = string.Empty;
            try
            {
                empty = this.ID;
            }
            catch (DataException dataException)
            {
                empty = string.Empty;
            }

            return empty.GetHashCode();
        }

        internal static IndexedDataTable GetItemDataTable()
        {
            IndexedDataTable indexedDataTable = new IndexedDataTable()
            {
                TableName = "Items"
            };
            DataColumn dataColumn = new DataColumn("ItemID", typeof(string))
            {
                MaxLength = 36
            };
            indexedDataTable.Columns.Add(dataColumn);
            DataColumn dataColumn1 = new DataColumn("ItemNum", typeof(int))
            {
                MaxLength = -1
            };
            indexedDataTable.Columns.Add(dataColumn1);
            DataColumn dataColumn2 = new DataColumn("WorkspaceID", typeof(string))
            {
                MaxLength = 36
            };
            indexedDataTable.Columns.Add(dataColumn2);
            DataColumn dataColumn3 = new DataColumn("CreationDate", typeof(DateTime))
            {
                MaxLength = -1
            };
            indexedDataTable.Columns.Add(dataColumn3);
            DataColumn dataColumn4 = new DataColumn("ModificationDate", typeof(DateTime))
            {
                MaxLength = -1
            };
            indexedDataTable.Columns.Add(dataColumn4);
            return indexedDataTable;
        }

        internal static IndexedDataTable GetItemPropertiesDataTable()
        {
            IndexedDataTable indexedDataTable = new IndexedDataTable()
            {
                TableName = "ItemProperties"
            };
            DataColumn dataColumn = new DataColumn("ItemID", typeof(string))
            {
                MaxLength = 36
            };
            indexedDataTable.Columns.Add(dataColumn);
            DataColumn dataColumn1 = new DataColumn("PropertyName", typeof(string))
            {
                MaxLength = 50
            };
            indexedDataTable.Columns.Add(dataColumn1);
            DataColumn dataColumn2 = new DataColumn("ShortTextValue", typeof(string))
            {
                MaxLength = 255
            };
            indexedDataTable.Columns.Add(dataColumn2);
            DataColumn dataColumn3 = new DataColumn("LongTextValue", typeof(string))
            {
                MaxLength = -1
            };
            indexedDataTable.Columns.Add(dataColumn3);
            DataColumn dataColumn4 = new DataColumn("TextBlobValue", typeof(string))
            {
                MaxLength = 1
            };
            indexedDataTable.Columns.Add(dataColumn4);
            DataColumn dataColumn5 = new DataColumn("NumberValue", typeof(double))
            {
                MaxLength = -1
            };
            indexedDataTable.Columns.Add(dataColumn5);
            DataColumn dataColumn6 = new DataColumn("DateValue", typeof(DateTime))
            {
                MaxLength = -1
            };
            indexedDataTable.Columns.Add(dataColumn6);
            return indexedDataTable;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (this.m_properties == null)
            {
                this.m_properties = this.m_list.Properties;
                for (int i = this.m_properties.Count - 1; i >= 0; i--)
                {
                    if (!this.m_properties[i].IsBrowsable)
                    {
                        this.m_properties.RemoveAt(i);
                    }
                }
            }

            return this.m_properties;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        internal object GetValue(string sPropertyName, Type typeProperty)
        {
            DataRow dataRow = this.FindPropertyRow(sPropertyName);
            if (dataRow == null)
            {
                return this.DeserializeValue(sPropertyName, null, typeProperty);
            }

            object item = dataRow[RecordList.FieldValueColumnLookUp(typeProperty).ColumnName];
            return this.DeserializeValue(sPropertyName, item, typeProperty);
        }

        public void Refresh()
        {
            base.Data.RejectChanges();
            this.PropertiesData.RejectChanges();
        }

        public static string SerializeValue(string sValue, int iMaxStringLength)
        {
            string str = (sValue == null ? string.Empty : sValue.ToString());
            if (iMaxStringLength < 0 || str.Length <= iMaxStringLength)
            {
                return str;
            }

            return str.Substring(0, iMaxStringLength);
        }

        public static string SerializeValue(object oValue)
        {
            if (oValue == null || oValue == DBNull.Value)
            {
                return string.Empty;
            }

            if (oValue is TextMoniker)
            {
                return ((TextMoniker)oValue).GetFullText();
            }

            if (oValue is ISmartDataType)
            {
                return ((ISmartDataType)oValue).Serialize();
            }

            if (Serializer.Instance.IsRegistered(oValue.GetType()))
            {
                return Serializer.Instance.Serialize(oValue);
            }

            return oValue.ToString();
        }

        internal void SetPropertyValue(PropertyDescriptor property, object oValue)
        {
            lock (base.Data.Table)
            {
                this.BeginEdit();
                property.SetValue(this, oValue);
                this.EndEdit();
                this.CommitChanges();
            }
        }

        internal void SetValue(string sPropertyName, object oPropertyValue, Type typeProperty)
        {
            try
            {
                DataRow d = this.FindPropertyRow(sPropertyName);
                if (d == null)
                {
                    d = this.m_dataTableItemProperties.NewRow();
                    d["ItemID"] = this.ID;
                    d["PropertyName"] = sPropertyName;
                    this.m_dataTableItemProperties.Rows.Add(d);
                }

                Record.SetValueInternal(RecordList.FieldValueColumnLookUp(typeProperty), d, oPropertyValue);
                base.Data["ModificationDate"] = DateTime.Now;
            }
            catch (Exception exception)
            {
            }
        }

        internal static void SetValueInternal(DataColumn col, DataRow row, object oPropertyValue)
        {
            if (oPropertyValue == null || oPropertyValue == DBNull.Value)
            {
                row[col.ColumnName] = DBNull.Value;
                return;
            }

            row[col.ColumnName] = (oPropertyValue is string
                ? Record.SerializeValue((string)oPropertyValue, col.MaxLength)
                : Record.SerializeValue(oPropertyValue));
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return this.m_list.Properties;
        }

        internal struct Fields
        {
            public const string ItemsTable = "Items";

            public const string ItemPropertiesTable = "ItemProperties";

            public const string ItemRelation = "ItemRelation";

            public const string ItemID = "ItemID";

            public const string ItemNum = "ItemNum";

            public const string CreationDate = "CreationDate";

            public const string ModificationDate = "ModificationDate";

            public const string ShortTextValue = "ShortTextValue";

            public const string LongTextValue = "LongTextValue";

            public const string TextBlobValue = "TextBlobValue";

            public const string NumberValue = "NumberValue";

            public const string DateValue = "DateValue";

            public const string PropertyName = "PropertyName";

            public const string FolderID = "FolderID";
        }
    }
}