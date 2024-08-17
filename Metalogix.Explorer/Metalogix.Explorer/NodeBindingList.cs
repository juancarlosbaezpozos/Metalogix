using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Metalogix.Explorer
{
    public class NodeBindingList : IList<Node>, ICollection<Node>, IEnumerable<Node>, IBindingList, IList, ICollection,
        IEnumerable
    {
        private bool m_bSorted;

        private PropertyDescriptor m_pdSort;

        private ListSortDirection m_sortDirection;

        private List<Node> m_baseList = new List<Node>();

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
            get { return false; }
        }

        public int Count
        {
            get { return this.m_baseList.Count; }
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
            get { return this.m_bSorted; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public Node this[int index]
        {
            get { return this.m_baseList[index]; }
            set { this.m_baseList[index] = value; }
        }

        public ListSortDirection SortDirection
        {
            get { return this.m_sortDirection; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return this.m_pdSort; }
        }

        public bool SupportsChangeNotification
        {
            get { return false; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public bool SupportsSorting
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        object System.Collections.IList.this[int index]
        {
            get { return this.m_baseList[index]; }
            set { this.m_baseList[index] = (Node)value; }
        }

        public NodeBindingList()
        {
        }

        public void Add(Node item)
        {
            this.m_baseList.Add(item);
        }

        public int Add(object value)
        {
            int count = this.m_baseList.Count;
            this.m_baseList.Add((Node)value);
            return count;
        }

        public void AddIndex(PropertyDescriptor property)
        {
        }

        public object AddNew()
        {
            throw new NotSupportedException();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            this.m_bSorted = true;
            this.m_pdSort = property;
            this.m_sortDirection = direction;
            this.m_baseList.Sort(new NodeBindingList.NodeComparer(property, direction));
        }

        public void Clear()
        {
            this.m_baseList.Clear();
        }

        public bool Contains(Node item)
        {
            return this.m_baseList.Contains(item);
        }

        public bool Contains(object value)
        {
            return this.m_baseList.Contains(value as Node);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            this.m_baseList.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            Node[] nodeArray = new Node[this.m_baseList.Count];
            this.m_baseList.CopyTo(nodeArray, index);
            nodeArray.CopyTo(array, index);
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return this.m_baseList.GetEnumerator();
        }

        public int IndexOf(Node item)
        {
            return this.m_baseList.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            return this.m_baseList.IndexOf(value as Node);
        }

        public void Insert(int index, Node item)
        {
            this.m_baseList.Insert(index, item);
        }

        public void Insert(int index, object value)
        {
            this.m_baseList.Insert(index, (Node)value);
        }

        public bool Remove(Node item)
        {
            return this.m_baseList.Remove(item);
        }

        public void Remove(object value)
        {
            this.m_baseList.Remove((Node)value);
        }

        public void RemoveAt(int index)
        {
            this.m_baseList.RemoveAt(index);
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
        }

        public void RemoveSort()
        {
            this.m_bSorted = false;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_baseList.GetEnumerator();
        }

        public event ListChangedEventHandler ListChanged;

        private class NodeComparer : IComparer<Node>
        {
            private PropertyDescriptor m_pdComparison;

            private ListSortDirection m_sortDirection;

            public NodeComparer(PropertyDescriptor pdComparison, ListSortDirection sortDirection)
            {
                this.m_pdComparison = pdComparison;
                this.m_sortDirection = sortDirection;
            }

            public int Compare(Node x, Node y)
            {
                if (x == y)
                {
                    return 0;
                }

                object value = this.m_pdComparison.GetValue(x);
                object obj = this.m_pdComparison.GetValue(y);
                if (value == obj)
                {
                    return 0;
                }

                int num = 0;
                if (value is IComparable)
                {
                    num = ((IComparable)value).CompareTo(obj);
                }
                else if (obj is IComparable)
                {
                    num = -((IComparable)obj).CompareTo(value);
                }
                else if (value == null && obj == null)
                {
                    num = 0;
                }
                else if (value == null)
                {
                    num = -1;
                }
                else if (obj != null)
                {
                    num = (!value.Equals(obj) ? 1 : 0);
                }
                else
                {
                    num = 1;
                }

                if (this.m_sortDirection == ListSortDirection.Descending)
                {
                    num = -num;
                }

                return num;
            }
        }
    }
}