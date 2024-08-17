using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Metalogix.Metabase.Data
{
    public class IndexedList : ArrayList
    {
        private Dictionary<int, int> m_oldIndices = new Dictionary<int, int>();

        private PropertyDescriptor m_primaryKey;

        public PropertyDescriptor PrimaryKey
        {
            get { return this.m_primaryKey; }
            set { this.m_primaryKey = value; }
        }

        public IndexedList(PropertyDescriptor primaryKey)
        {
            this.m_primaryKey = primaryKey;
        }

        public int Add(object o, int iOriginalIndex)
        {
            int num = base.Add(o);
            this.m_oldIndices[iOriginalIndex] = num;
            return num;
        }

        public int IndexOf(int iOriginalIndex)
        {
            if (!this.m_oldIndices.ContainsKey(iOriginalIndex))
            {
                return -1;
            }

            return this.m_oldIndices[iOriginalIndex];
        }

        public void Sort(ListSortDirection direction)
        {
            try
            {
                this.Sort(new IndexedList.IndexedListComparer(this.m_primaryKey));
                if (direction == ListSortDirection.Descending)
                {
                    this.Reverse();
                }
            }
            catch (Exception exception)
            {
            }
        }

        private class IndexedListComparer : IComparer
        {
            private PropertyDescriptor m_primaryKey;

            public IndexedListComparer(PropertyDescriptor primaryKey)
            {
                this.m_primaryKey = primaryKey;
            }

            public int Compare(object objInput1, object objInput2)
            {
                if (objInput1 == objInput2)
                {
                    return 0;
                }

                object value = this.m_primaryKey.GetValue(objInput1);
                object obj = this.m_primaryKey.GetValue(objInput2);
                if (value == obj)
                {
                    return 0;
                }

                if (value == null || value is DBNull)
                {
                    return -1;
                }

                if (obj == null || obj is DBNull)
                {
                    return 1;
                }

                IComparable comparable = value as IComparable;
                if (comparable != null)
                {
                    return comparable.CompareTo(obj);
                }

                string str = value.ToString();
                if (str == null)
                {
                    return -1;
                }

                return str.CompareTo(obj.ToString());
            }
        }
    }
}