using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Metalogix.Metabase.Data
{
    public class ListView : IBindingList, IList, ICollection, IEnumerable, ITypedList, IFilterableList, IDataboundList,
        ISelectableList
    {
        private IList m_baseList;

        private IndexedList m_viewList;

        private bool m_isSorted;

        private bool m_isFiltered;

        private PropertyDescriptor m_sortProperty;

        private PropertyDescriptorCollection m_properties;

        private ListSortDirection m_sortDirection;

        private FilterExpressionList m_filters = new FilterExpressionList();

        private int m_iFiltersAddedByExplorer;

        private PropertyDescriptor m_selectedProperty;

        private int m_iCurrentIndex = -1;

        private int[] m_selectedIndices = new int[0];

        private object m_oCurrentObject;

        public object[] AllObjects
        {
            get
            {
                if (this.WorkingList == null)
                {
                    return null;
                }

                object[] objArray = new object[this.WorkingList.Count];
                this.WorkingList.CopyTo(objArray, 0);
                return objArray;
            }
        }

        public bool AllowEdit
        {
            get
            {
                IBindingList mBaseList = this.m_baseList as IBindingList;
                if (mBaseList == null)
                {
                    return true;
                }

                return mBaseList.AllowEdit;
            }
        }

        public bool AllowNew
        {
            get
            {
                IBindingList mBaseList = this.m_baseList as IBindingList;
                if (mBaseList == null)
                {
                    return false;
                }

                return mBaseList.AllowNew;
            }
        }

        public bool AllowRemove
        {
            get
            {
                IBindingList mBaseList = this.m_baseList as IBindingList;
                if (mBaseList == null)
                {
                    return false;
                }

                return mBaseList.AllowRemove;
            }
        }

        public IList BaseList
        {
            get { return this.m_baseList; }
        }

        private bool BaseListSupportsChangeNotification
        {
            get
            {
                if (!(this.m_baseList is IBindingList))
                {
                    return false;
                }

                return ((IBindingList)this.m_baseList).SupportsChangeNotification;
            }
        }

        public int Count
        {
            get { return this.WorkingList.Count; }
        }

        public int CurrentIndex
        {
            get { return this.m_iCurrentIndex; }
            set
            {
                if (value >= this.Count)
                {
                    return;
                }

                bool mICurrentIndex = this.m_iCurrentIndex != value;
                this.m_iCurrentIndex = value;
                if (mICurrentIndex && this.CurrentIndexChanged != null)
                {
                    this.CurrentIndexChanged(this, null);
                }
            }
        }

        public object CurrentObject
        {
            get { return JustDecompileGenerated_get_CurrentObject(); }
            set { JustDecompileGenerated_set_CurrentObject(value); }
        }

        public object JustDecompileGenerated_get_CurrentObject()
        {
            if (this.CurrentIndex < 0 || this.CurrentIndex >= this.WorkingList.Count)
            {
                return this.m_oCurrentObject;
            }

            return this[this.CurrentIndex];
        }

        public void JustDecompileGenerated_set_CurrentObject(object value)
        {
            this.m_oCurrentObject = value;
        }

        public PropertyFilterExpression[] Filters
        {
            get
            {
                PropertyFilterExpression[] propertyFilterExpressionArray =
                    new PropertyFilterExpression[this.m_filters.Count];
                this.m_filters.CopyTo(propertyFilterExpressionArray);
                return propertyFilterExpressionArray;
            }
        }

        public int FiltersAddedByExplorer
        {
            get { return this.m_iFiltersAddedByExplorer; }
            set { this.m_iFiltersAddedByExplorer = value; }
        }

        public string FilterString
        {
            get { return this.m_filters.ToString(); }
        }

        public bool IsFiltered
        {
            get { return this.m_isFiltered; }
        }

        public bool IsFixedSize
        {
            get { return this.m_baseList.IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return this.m_baseList.IsReadOnly; }
        }

        public bool IsSorted
        {
            get { return this.m_isSorted; }
        }

        public bool IsSynchronized
        {
            get { return this.m_baseList.IsSynchronized; }
        }

        public bool IsTransformed
        {
            get { return this.m_viewList != null; }
        }

        public object this[int index]
        {
            get { return this.WorkingList[index]; }
            set
            {
                this.m_baseList[(this.IsTransformed ? this.m_baseList.IndexOf(this.m_viewList[index]) : index)] = value;
            }
        }

        public int[] SelectedIndices
        {
            get { return this.m_selectedIndices; }
            set
            {
                this.m_selectedIndices = value;
                if (this.SelectedIndicesChanged != null)
                {
                    this.SelectedIndicesChanged(this, null);
                }
            }
        }

        public object[] SelectedObjects
        {
            get
            {
                ArrayList arrayLists = new ArrayList();
                for (int i = 0; i < (int)this.SelectedIndices.Length; i++)
                {
                    if (this.SelectedIndices[i] >= 0 && this.SelectedIndices[i] < this.Count)
                    {
                        arrayLists.Add(this[this.SelectedIndices[i]]);
                    }
                }

                object[] objArray = new object[arrayLists.Count];
                arrayLists.CopyTo(objArray);
                return objArray;
            }
        }

        public PropertyDescriptor SelectedProperty
        {
            get { return this.m_selectedProperty; }
            set { this.m_selectedProperty = value; }
        }

        public ListSortDirection SortDirection
        {
            get { return this.m_sortDirection; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return this.m_sortProperty; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get
            {
                IBindingList mBaseList = this.m_baseList as IBindingList;
                if (mBaseList == null)
                {
                    return false;
                }

                return mBaseList.SupportsSearching;
            }
        }

        public bool SupportsSorting
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return this.m_baseList.SyncRoot; }
        }

        private IList WorkingList
        {
            get
            {
                if (this.m_viewList != null)
                {
                    return this.m_viewList;
                }

                return this.m_baseList;
            }
        }

        public ListView(IList list)
        {
            this.m_baseList = list;
            if (this.m_baseList is IBindingList)
            {
                ((IBindingList)this.m_baseList).ListChanged += new ListChangedEventHandler(this.On_baseListChanged);
            }
        }

        public ListView(IList list, PropertyDescriptorCollection props)
        {
            this.m_baseList = list;
            this.m_properties = props;
            if (this.m_baseList is IBindingList)
            {
                ((IBindingList)this.m_baseList).ListChanged += new ListChangedEventHandler(this.On_baseListChanged);
            }
        }

        public int Add(object value)
        {
            int num = this.m_baseList.Add(value);
            if (!this.BaseListSupportsChangeNotification)
            {
                this.On_baseListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, num));
            }

            return num;
        }

        public void AddFilter(PropertyFilterExpression filter)
        {
            this.m_filters.Add(filter);
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            this.FireFiltersAddedEvent();
        }

        public void AddIndex(PropertyDescriptor property)
        {
            IBindingList mBaseList = this.m_baseList as IBindingList;
            if (mBaseList != null)
            {
                mBaseList.AddIndex(property);
            }
        }

        public object AddNew()
        {
            IBindingList mBaseList = this.m_baseList as IBindingList;
            if (mBaseList == null)
            {
                return null;
            }

            return mBaseList.AddNew();
        }

        private void ApplyFilterSort()
        {
            bool count = this.m_filters.Count > 0;
            bool mSortProperty = this.m_sortProperty != null;
            if (count || mSortProperty)
            {
                IndexedList indexedList = new IndexedList(this.m_sortProperty);
                for (int i = 0; i < this.m_baseList.Count; i++)
                {
                    object item = this.m_baseList[i];
                    if (!count || this.m_filters.EvaluateAll(item))
                    {
                        indexedList.Add(item, i);
                    }
                }

                if (mSortProperty)
                {
                    indexedList.Sort(this.m_sortDirection);
                }

                this.m_viewList = indexedList;
            }
            else
            {
                this.m_viewList = null;
            }

            this.m_isFiltered = count;
            this.m_isSorted = mSortProperty;
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            this.m_sortProperty = property;
            this.m_sortDirection = direction;
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public void BatchDelete(object[] arrObjects)
        {
            IDataboundList mBaseList = this.m_baseList as IDataboundList;
            if (mBaseList != null)
            {
                mBaseList.BatchDelete(arrObjects);
            }
        }

        public void BatchUpdate(object[] arrObjects, PropertyDescriptor property, object oValue)
        {
            IDataboundList mBaseList = this.m_baseList as IDataboundList;
            if (mBaseList != null)
            {
                mBaseList.BatchUpdate(arrObjects, property, oValue);
            }
        }

        public void BeginEdit()
        {
            IDataboundList mBaseList = this.m_baseList as IDataboundList;
            if (mBaseList != null)
            {
                mBaseList.BeginEdit();
            }
        }

        public void Clear()
        {
            this.m_baseList.Clear();
            if (!this.BaseListSupportsChangeNotification)
            {
                this.On_baseListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            }
        }

        public void ClearFilters(bool removeBaseFilters)
        {
            this.m_filters.Clear(removeBaseFilters);
            this.m_viewList = null;
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public void ClearFilters(bool bRemoveBaseFilters, bool bRemoveActionFilters)
        {
            this.m_filters.Clear(bRemoveBaseFilters, bRemoveActionFilters);
            this.m_viewList = null;
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public void ClearFilters(bool bRemoveNormalFilters, bool bRemoveBaseFilters, bool bRemoveActionFilters)
        {
            this.m_filters.Clear(bRemoveNormalFilters, bRemoveBaseFilters, bRemoveActionFilters);
            this.m_viewList = null;
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public void ClearFilterSort()
        {
            this.m_filters.Clear(false);
            this.m_sortProperty = null;
            this.m_viewList = null;
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            this.FireFiltersAddedEvent();
        }

        public void CommitChanges()
        {
            IDataboundList mBaseList = this.m_baseList as IDataboundList;
            if (mBaseList != null)
            {
                mBaseList.CommitChanges();
            }
        }

        public bool Contains(object value)
        {
            return this.WorkingList.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            this.WorkingList.CopyTo(array, index);
        }

        public void EndEdit()
        {
            IDataboundList mBaseList = this.m_baseList as IDataboundList;
            if (mBaseList != null)
            {
                mBaseList.EndEdit();
            }
        }

        public int Find(PropertyDescriptor property, object key)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            PropertyFilterExpression propertyFilterExpression = new PropertyFilterExpression(property, key.ToString());
            return this.Find(propertyFilterExpression, 0, false);
        }

        public int Find(PropertyFilterExpression filterExpression, int startRow, bool searchAllProperties)
        {
            if (filterExpression == null)
            {
                throw new ArgumentNullException("filterExpression");
            }

            if (searchAllProperties)
            {
                PropertyDescriptorCollection itemProperties = this.GetItemProperties(null);
                for (int i = startRow; i < this.WorkingList.Count; i++)
                {
                    foreach (PropertyDescriptor property in itemProperties)
                    {
                        filterExpression.Property = property;
                        if (filterExpression.Evaluate(this[i]))
                        {
                            return i;
                        }
                    }
                }
            }
            else
            {
                for (int j = startRow; j < this.WorkingList.Count; j++)
                {
                    if (filterExpression.Evaluate(this[j]))
                    {
                        return j;
                    }
                }
            }

            return -1;
        }

        private void FireFiltersAddedEvent()
        {
            if (this.FiltersAdded != null)
            {
                this.FiltersAdded(this, new EventArgs());
            }
        }

        private void FireListChangedEvent(object sender, ListChangedEventArgs e)
        {
            if (this.ListChanged != null)
            {
                this.ListChanged(sender, e);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.WorkingList.GetEnumerator();
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (this.m_properties != null)
            {
                return this.m_properties;
            }

            ITypedList mBaseList = this.m_baseList as ITypedList;
            if (mBaseList != null)
            {
                return mBaseList.GetItemProperties(listAccessors);
            }

            if (this.m_baseList.Count <= 0)
            {
                return new PropertyDescriptorCollection(null);
            }

            return TypeDescriptor.GetProperties(this.m_baseList[0]);
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            ITypedList mBaseList = this.m_baseList as ITypedList;
            if (mBaseList != null)
            {
                return mBaseList.GetListName(listAccessors);
            }

            return this.m_baseList.GetType().ToString();
        }

        public int IndexOf(object value)
        {
            return this.WorkingList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.m_baseList.Insert((this.IsTransformed ? this.m_baseList.IndexOf(this.m_viewList[index]) : index),
                value);
            if (!this.BaseListSupportsChangeNotification)
            {
                this.On_baseListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            }
        }

        private void On_baseListChanged(object sender, ListChangedEventArgs e)
        {
            if (!this.IsTransformed)
            {
                this.FireListChangedEvent(this, e);
            }
            else
            {
                ListChangedEventArgs2 listChangedEventArgs2 = e as ListChangedEventArgs2;
                bool flag = (listChangedEventArgs2 == null ? false : listChangedEventArgs2.Invoked);
                bool flag1 = (listChangedEventArgs2 == null ? false : listChangedEventArgs2.SoftReset);
                switch (e.ListChangedType)
                {
                    case ListChangedType.Reset:
                    {
                        if (!flag1)
                        {
                            this.m_viewList = null;
                            this.ApplyFilterSort();
                        }

                        this.FireListChangedEvent(this,
                            new ListChangedEventArgs2(ListChangedType.Reset, -1, -1, null, flag1, flag));
                        return;
                    }
                    case ListChangedType.ItemAdded:
                    {
                        int num = this.m_viewList.IndexOf(e.NewIndex);
                        object obj = (listChangedEventArgs2 != null
                            ? listChangedEventArgs2.Item
                            : this.m_baseList[e.NewIndex]);
                        int num1 = (num < 0 ? this.m_viewList.Add(obj, e.NewIndex) : num);
                        this.ApplyFilterSort();
                        this.FireListChangedEvent(this,
                            new ListChangedEventArgs2(ListChangedType.ItemAdded, num1, -1, obj, false, flag));
                        return;
                    }
                    case ListChangedType.ItemDeleted:
                    {
                        this.m_viewList = null;
                        this.ApplyFilterSort();
                        this.FireListChangedEvent(this,
                            new ListChangedEventArgs2(ListChangedType.Reset, -1, -1, null, false, flag));
                        return;
                    }
                    case ListChangedType.ItemMoved:
                    {
                        break;
                    }
                    case ListChangedType.ItemChanged:
                    {
                        int num2 = this.WorkingList.IndexOf(e.NewIndex);
                        this.FireListChangedEvent(this,
                            new ListChangedEventArgs2(ListChangedType.ItemChanged, num2, -1, null, false, flag));
                        return;
                    }
                    default:
                    {
                        return;
                    }
                }
            }
        }

        public void Remove(object value)
        {
            int num = -1;
            if (!this.BaseListSupportsChangeNotification)
            {
                num = this.m_baseList.IndexOf(value);
            }

            this.m_baseList.Remove(value);
            if (!this.BaseListSupportsChangeNotification)
            {
                this.On_baseListChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
            }
        }

        public void RemoveAt(int index)
        {
            int num = (this.IsTransformed ? this.m_baseList.IndexOf(this.m_viewList[index]) : index);
            this.m_baseList.RemoveAt(num);
            if (!this.BaseListSupportsChangeNotification)
            {
                this.On_baseListChanged(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, num, 0));
            }
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            IBindingList mBaseList = this.m_baseList as IBindingList;
            if (mBaseList != null)
            {
                mBaseList.RemoveIndex(property);
            }
        }

        public void RemoveSort()
        {
            this.m_sortProperty = null;
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public void UpdateFilters(PropertyFilterExpression filter)
        {
            this.m_filters.Clear(true);
            this.m_viewList = null;
            this.m_filters.Add(filter);
            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            this.FireFiltersAddedEvent();
        }

        public void UpdateFilters(FilterExpressionList filterList)
        {
            this.m_filters.Clear(true);
            this.m_viewList = null;
            foreach (PropertyFilterExpression propertyFilterExpression in filterList)
            {
                this.m_filters.Add(propertyFilterExpression);
            }

            this.ApplyFilterSort();
            this.FireListChangedEvent(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            this.FireFiltersAddedEvent();
        }

        public event EventHandler CurrentIndexChanged;

        public event EventHandler FiltersAdded;

        public event ListChangedEventHandler ListChanged;

        public event EventHandler SelectedIndicesChanged;
    }
}