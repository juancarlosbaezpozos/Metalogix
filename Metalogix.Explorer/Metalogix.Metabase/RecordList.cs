using Metalogix.Explorer;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Threading;

namespace Metalogix.Metabase
{
    public class RecordList : ITypedList, IBindingList, IList, ICollection, IEnumerable, IDataboundList
    {
        public static DataTable s_dataTableLookup;

        public static string s_lock;

        protected Node m_owner;

        protected DataTable m_dataTableItems;

        protected DataTable m_dataTableItemProperties;

        protected Workspace m_workspace;

        protected bool m_bSuspendEventNotification;

        private bool m_bFillFactorsUpToDate;

        protected Metalogix.Metabase.Data.ListView m_listView;

        public bool AllowEdit
        {
            get { return true; }
        }

        public bool AllowNew
        {
            get { return false; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public int Count
        {
            get { return this.m_dataTableItems.DefaultView.Count; }
        }

        public bool FillFactorsUpToDate
        {
            get { return this.m_bFillFactorsUpToDate; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsSorted
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= this.m_dataTableItems.DefaultView.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                DataRow row = this.m_dataTableItems.DefaultView[index].Row;
                return RecordList.CreateItem(this, row, this.m_dataTableItemProperties);
            }
            set
            {
                if (index < 0 || index >= this.m_dataTableItems.DefaultView.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public Metalogix.Metabase.Data.ListView ListView
        {
            get
            {
                if (this.m_listView == null)
                {
                    this.m_listView = new Metalogix.Metabase.Data.ListView(this);
                }

                return this.m_listView;
            }
        }

        public Workspace ParentWorkspace
        {
            get { return this.m_workspace; }
        }

        public RecordPropertyDescriptorList Properties
        {
            get
            {
                if (this.m_workspace == null)
                {
                    return null;
                }

                return (RecordPropertyDescriptorList)this.m_workspace.GetProperties();
            }
        }

        public ListSortDirection SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public bool SupportsSorting
        {
            get { return false; }
        }

        internal bool SuspendEventNotifiaction
        {
            get { return this.m_bSuspendEventNotification; }
            set { this.m_bSuspendEventNotification = value; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        static RecordList()
        {
            RecordList.s_lock = string.Empty;
        }

        public RecordList(Workspace workspace, RecordPropertyDescriptorList properties, DataSet dataSetItems)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException("workspace");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            if (dataSetItems == null)
            {
                throw new ArgumentNullException("dataSetItems");
            }

            this.m_workspace = workspace;
            this.m_workspace.PropertiesChanged +=
                new Workspace.PropertiesChangedHandler(this.On_workspace_PropertiesChanged);
            this.m_dataTableItems = dataSetItems.Tables["Items"];
            if (this.m_dataTableItems == null)
            {
                throw new Exception("No items data table provided.");
            }

            this.m_dataTableItems.DefaultView.ListChanged +=
                new ListChangedEventHandler(this.On_dataTableItems_DefaultView_ItemsChanged);
            this.m_dataTableItemProperties = dataSetItems.Tables["ItemProperties"];
            if (this.m_dataTableItems == null)
            {
                throw new Exception("No node properties data table provided.");
            }

            this.EnsureIndexedProperties();
        }

        public int Add(object value)
        {
            Record record = value as Record;
            if (record == null)
            {
                return -1;
            }

            DataRow dataRow = this.GenerateDataRow(record);
            this.m_dataTableItems.Rows.Add(dataRow);
            return this.Count - 1;
        }

        public void AddIndex(PropertyDescriptor pd)
        {
            DataColumn dataColumn = RecordList.FieldValueColumnLookUp(pd.PropertyType);
            if (dataColumn == null)
            {
                return;
            }

            IndexedDataTable mDataTableItemProperties = (IndexedDataTable)this.m_dataTableItemProperties;
            SelectiveHashIndex index = (SelectiveHashIndex)mDataTableItemProperties.GetIndex(dataColumn.ColumnName);
            if (index == null)
            {
                DataColumn item = this.m_dataTableItemProperties.Columns["PropertyName"];
                object[] name = new object[] { pd.Name };
                index = new SelectiveHashIndex(item, dataColumn, name, false);
                mDataTableItemProperties.AddIndex(dataColumn.ColumnName, index);
            }

            index.AddIndexableValue(pd.Name);
            mDataTableItemProperties.BuildIndex(dataColumn.ColumnName);
        }

        public object AddNew()
        {
            return this.AddNew(Guid.NewGuid());
        }

        public Record AddNew(Guid id)
        {
            if (this.ParentWorkspace.ReadOnly)
            {
                throw new UnauthorizedAccessException("RecordList is readonly.");
            }

            bool mBSuspendEventNotification = this.m_bSuspendEventNotification;
            this.m_bSuspendEventNotification = true;
            DateTime now = DateTime.Now;
            DataRow str = this.m_dataTableItems.NewRow();
            str["ItemID"] = id.ToString();
            str["ItemNum"] = this.ParentWorkspace.Connection.Adapter.IncrementItemNum();
            str["CreationDate"] = now;
            str["ModificationDate"] = now;
            str["WorkspaceID"] = this.ParentWorkspace.ID.ToString();
            this.m_dataTableItems.Rows.Add(str);
            this.m_bSuspendEventNotification = mBSuspendEventNotification;
            Record record = RecordList.CreateItem(this, str, this.m_dataTableItemProperties);
            this.FireListChangedEvent(this,
                new ListChangedEventArgs2(ListChangedType.ItemAdded, this.Count - 1, -1, record, false, false));
            return record;
        }

        public Record AddNew(Guid id, string sourceUrl)
        {
            Record url = this.AddNew(id);
            url.BeginEdit();
            url.SourceURL = new Url(sourceUrl);
            url.EndEdit();
            url.CommitChanges();
            return url;
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException(
                "RecordList does not support method: ApplySort(PropertyDescriptor property, ListSortDirection direction)");
        }

        public void BatchDelete(object[] arrObjects)
        {
            if (arrObjects == null)
            {
                throw new ArgumentNullException("arrObjects");
            }

            if ((int)arrObjects.Length == 0)
            {
                return;
            }

            if ((int)arrObjects.Length != 1)
            {
                this.BeginEdit();
                bool flag = false;
                for (int i = 0; i < (int)arrObjects.Length; i++)
                {
                    Record record = arrObjects[i] as Record;
                    if (record != null)
                    {
                        record.Delete();
                        flag = true;
                    }
                }

                if (!flag)
                {
                    this.EndEdit();
                }
                else
                {
                    this.EndEdit(new ListChangedEventArgs(ListChangedType.Reset, 0));
                }
            }
            else
            {
                Record record1 = arrObjects[0] as Record;
                if (record1 != null)
                {
                    record1.Delete();
                }
            }

            this.CommitChanges();
        }

        internal void BatchDeleteProperty(PropertyDescriptor pd)
        {
            if (pd == null)
            {
                throw new ArgumentNullException("pd");
            }

            foreach (DataRow dataRow in new ArrayList(this.m_dataTableItemProperties.Rows))
            {
                if (!dataRow["PropertyName"].ToString().Equals(pd.Name))
                {
                    continue;
                }

                dataRow.Delete();
            }

            this.CommitChanges();
        }

        public void BatchUpdate(object[] arrObjects, PropertyDescriptor property, object oValue)
        {
            if (arrObjects == null)
            {
                throw new ArgumentNullException("arrObjects");
            }

            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (oValue == null)
            {
                throw new ArgumentNullException("oValue");
            }

            this.BeginEdit();
            object[] objArray = arrObjects;
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                object obj = objArray[i];
                if (obj == null)
                {
                    throw new ArgumentException("arrObjects contains null items.");
                }

                Record record = obj as Record;
                if (record == null)
                {
                    throw new ArgumentException("oSelected is not a sub-type of Item.");
                }

                RecordPropertyValue item = record.Properties[property.Name];
                item.Value = oValue;
                if (typeof(TextMoniker).IsAssignableFrom(item.Type))
                {
                    RecordPropertyDescriptor recordPropertyDescriptor =
                        record.GetProperties()[property.Name] as RecordPropertyDescriptor;
                    if (recordPropertyDescriptor != null)
                    {
                        recordPropertyDescriptor.SetValue(obj, oValue);
                    }
                }
            }

            this.EndEdit(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, property));
            this.CommitChanges();
        }

        public void BeginEdit()
        {
            this.m_bSuspendEventNotification = true;
        }

        public void Clear()
        {
            if (this.ParentWorkspace.ReadOnly)
            {
                throw new UnauthorizedAccessException("Workspace is readonly.");
            }

            if (this.Count == 0)
            {
                return;
            }

            this.BeginEdit();
            foreach (Record record in this)
            {
                record.Delete();
            }

            this.EndEdit(new ListChangedEventArgs(ListChangedType.Reset, 0));
            this.CommitChanges();
            this.m_dataTableItems.Clear();
        }

        public void CommitChanges()
        {
            this.m_bSuspendEventNotification = true;
            this.ParentWorkspace.Connection.Adapter.CommitItems(this.m_dataTableItems, this.m_dataTableItemProperties);
            this.ParentWorkspace.Connection.Adapter.CommitItemProperties(this.m_dataTableItemProperties);
            this.m_bSuspendEventNotification = false;
        }

        public bool Contains(PropertyDescriptor property, object key)
        {
            return this.FindItem(property, key) != null;
        }

        public bool Contains(object value)
        {
            Record record = value as Record;
            if (record == null || record.Data == null || record.Data.RowState == DataRowState.Deleted)
            {
                return false;
            }

            return ((IndexedDataTable)this.m_dataTableItems).FindRow("ItemID", record.ID) != null;
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index is less than the lower bounds of the array");
            }

            if (array.Length < index + this.Count)
            {
                throw new ArgumentException(
                    "The number of nodes in the RecordList is greater than the available space from index to the end of the destination array.");
            }

            for (int i = 0; i < this.Count; i++)
            {
                int num = index;
                index = num + 1;
                array.SetValue(this[i], num);
            }
        }

        internal static Record CreateItem(RecordList itemList, DataRow row, DataTable dataTableItemProperties)
        {
            return new Record(itemList, row, dataTableItemProperties);
        }

        public void EndEdit()
        {
            this.m_bSuspendEventNotification = false;
        }

        public void EndEdit(ListChangedEventArgs e)
        {
            this.m_bSuspendEventNotification = false;
            this.FireListChangedEvent(this, e);
        }

        private void EnsureIndexedProperties()
        {
            if (this.m_workspace == null || this.m_dataTableItemProperties == null)
            {
                return;
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this.m_workspace.BaseType))
            {
                IndexedAttribute item = (IndexedAttribute)property.Attributes[typeof(IndexedAttribute)];
                if (item == null || !item.Indexed)
                {
                    continue;
                }

                this.AddIndex(property);
            }
        }

        internal static DataColumn FieldValueColumnLookUp(Type type)
        {
            return RecordList.FieldValueColumnLookUp(type, true);
        }

        internal static DataColumn FieldValueColumnLookUp(Type type, bool bShortVal)
        {
            lock (RecordList.s_lock)
            {
                if (RecordList.s_dataTableLookup == null)
                {
                    RecordList.s_dataTableLookup = Record.GetItemPropertiesDataTable();
                }
            }

            if (type.Equals(typeof(DateTime)))
            {
                return RecordList.s_dataTableLookup.Columns["DateValue"];
            }

            if (type.Equals(typeof(int)) || type.Equals(typeof(short)) || type.Equals(typeof(long)) ||
                type.Equals(typeof(float)) || type.Equals(typeof(double)) || type.Equals(typeof(uint)) ||
                type.Equals(typeof(ushort)) || type.Equals(typeof(ulong)) || type.Equals(typeof(decimal)))
            {
                return RecordList.s_dataTableLookup.Columns["NumberValue"];
            }

            if (type.Equals(typeof(bool)) || type.IsEnum)
            {
                return RecordList.s_dataTableLookup.Columns["ShortTextValue"];
            }

            if (type.Equals(typeof(Url)) || Serializer.Instance.IsRegistered(type) ||
                typeof(ISmartDataType).IsAssignableFrom(type))
            {
                return RecordList.s_dataTableLookup.Columns["LongTextValue"];
            }

            if (bShortVal || type.Equals(typeof(string)))
            {
                return RecordList.s_dataTableLookup.Columns["ShortTextValue"];
            }

            if (typeof(TextMoniker).IsAssignableFrom(type))
            {
                return RecordList.s_dataTableLookup.Columns["TextBlobValue"];
            }

            return RecordList.s_dataTableLookup.Columns["LongTextValue"];
        }

        public int Find(PropertyDescriptor property, object key)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (key == null)
            {
                return -1;
            }

            for (int i = 0; i < this.Count; i++)
            {
                object value = property.GetValue(this[i]);
                if (value == null)
                {
                    break;
                }

                if (value.Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }

        public Record FindByGUID(Guid GUID)
        {
            string str = GUID.ToString();
            char[] chrArray = new char[] { '{', '}' };
            return this.FindByGUID(str.Trim(chrArray));
        }

        public Record FindByGUID(string sGUID)
        {
            if (string.IsNullOrEmpty(sGUID))
            {
                throw new ArgumentNullException("sGUID");
            }

            DataRow dataRow = ((IndexedDataTable)this.m_dataTableItems).FindRow("ItemID", sGUID);
            if (dataRow == null)
            {
                return null;
            }

            return RecordList.CreateItem(this, dataRow, this.m_dataTableItemProperties);
        }

        public object FindItem(PropertyDescriptor property, object key)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (key == null)
            {
                return null;
            }

            DataColumn dataColumn = RecordList.FieldValueColumnLookUp(property.PropertyType);
            DataRow dataRow =
                ((IndexedDataTable)this.m_dataTableItemProperties).FindRow(dataColumn.ColumnName, key.ToString());
            if (dataRow == null)
            {
                return null;
            }

            return this.FindByGUID(dataRow["ItemID"].ToString());
        }

        public ArrayList FindItems(PropertyDescriptor property, object key)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (key == null)
            {
                return null;
            }

            ArrayList arrayLists = new ArrayList(0);
            DataColumn dataColumn = RecordList.FieldValueColumnLookUp(property.PropertyType);
            ArrayList arrayLists1 =
                ((IndexedDataTable)this.m_dataTableItemProperties).FindAllRows(dataColumn.ColumnName, key);
            if (arrayLists1 != null && arrayLists1.Count > 0)
            {
                arrayLists = new ArrayList(arrayLists1.Count);
                foreach (DataRow dataRow in arrayLists1)
                {
                    string str = dataRow["ItemID"].ToString();
                    arrayLists.Add(this.FindByGUID(str));
                }
            }

            return arrayLists;
        }

        private void FireListChangedEvent(object sender, ListChangedEventArgs e)
        {
            if (this.m_bSuspendEventNotification)
            {
                return;
            }

            this.m_bFillFactorsUpToDate = false;
            if (this.ListChanged != null)
            {
                this.ListChanged(sender, e);
            }
        }

        internal DataRow GenerateDataRow(Record item)
        {
            DataRow str = this.m_dataTableItems.NewRow();
            str["ItemID"] = item.ID.ToString();
            str["CreationDate"] = item.RecordDateCreated;
            str["ModificationDate"] = item.RecordDateModified;
            return str;
        }

        public IEnumerator GetEnumerator()
        {
            return new RecordList.RecordListEnumerator(this);
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return this.Properties;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return this.GetType().FullName;
        }

        public int IndexOf(object value)
        {
            Record record = value as Record;
            if (record == null)
            {
                return -1;
            }

            for (int i = 0; i < this.Count; i++)
            {
                if (((Record)this[i]).ID == record.ID)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException("RecordList does not support method: Insert(int index, object value)");
        }

        private void On_dataTableItems_DefaultView_ItemsChanged(object sender, ListChangedEventArgs e)
        {
            this.FireListChangedEvent(sender, e);
        }

        private void On_workspace_PropertiesChanged()
        {
            this.EnsureIndexedProperties();
        }

        public void RejectChanges()
        {
            this.m_dataTableItemProperties.RejectChanges();
            this.m_dataTableItems.RejectChanges();
        }

        public void Remove(object value)
        {
            Record record = value as Record;
            if (record != null && record.ParentList == this)
            {
                record.Data.Delete();
            }
        }

        public void RemoveAt(int index)
        {
            if (this.ParentWorkspace.ReadOnly)
            {
                throw new UnauthorizedAccessException("Workspace is readonly.");
            }

            if (index < 0 || index >= this.m_dataTableItems.DefaultView.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            DataRowView item = this.m_dataTableItems.DefaultView[index];
            if (item != null)
            {
                item.Delete();
            }
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
        }

        public void RemoveSort()
        {
        }

        public void UpdateFillFactors()
        {
            try
            {
                DateTime now = DateTime.Now;
                this.m_bFillFactorsUpToDate = true;
                Dictionary<string, int> strs = new Dictionary<string, int>();
                for (int i = 0; i < this.Count; i++)
                {
                    foreach (PropertyDescriptor property in this.Properties)
                    {
                        object value = property.GetValue(this[i]);
                        int num = 0;
                        if (value != null)
                        {
                            num = (string.IsNullOrEmpty(value.ToString()) ? 0 : 1);
                        }

                        if (!strs.ContainsKey(property.Name))
                        {
                            strs.Add(property.Name, num);
                        }
                        else
                        {
                            strs[property.Name] = strs[property.Name] + num;
                        }
                    }
                }

                foreach (KeyValuePair<string, int> str in strs)
                {
                    PropertyDescriptor item = this.Properties[str.Key];
                    if (item == null)
                    {
                        continue;
                    }

                    FillFactorAttribute fillFactorAttribute =
                        (FillFactorAttribute)item.Attributes[typeof(FillFactorAttribute)];
                    if (fillFactorAttribute == null)
                    {
                        continue;
                    }

                    fillFactorAttribute.FillFactor = (double)str.Value * 100 / (double)this.Count;
                }
            }
            catch (Exception exception)
            {
            }
        }

        public event ListChangedEventHandler ListChanged;

        private delegate void ListChangedThreadDelegate(object sender, ListChangedEventArgs e);

        private class RecordListEnumerator : IEnumerator
        {
            private int iPos;

            private RecordList m_List;

            public object Current
            {
                get
                {
                    if (this.m_List == null || this.iPos < 0 || this.iPos >= this.m_List.m_dataTableItems.Rows.Count)
                    {
                        return null;
                    }

                    return RecordList.CreateItem(this.m_List, this.m_List.m_dataTableItems.Rows[this.iPos],
                        this.m_List.m_dataTableItemProperties);
                }
            }

            public RecordListEnumerator(RecordList list)
            {
                this.m_List = list;
            }

            public bool MoveNext()
            {
                if (this.m_List == null)
                {
                    return false;
                }

                this.iPos++;
                return this.iPos < this.m_List.m_dataTableItems.Rows.Count;
            }

            public void Reset()
            {
                this.iPos = -1;
            }
        }
    }
}