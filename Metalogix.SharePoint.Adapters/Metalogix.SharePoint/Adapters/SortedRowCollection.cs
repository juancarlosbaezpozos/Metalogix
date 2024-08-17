using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    public class SortedRowCollection : IEnumerable
    {
        private ArrayList m_list;

        public int Count
        {
            get { return this.m_list.Count; }
        }

        public object this[int index]
        {
            get { return this.m_list[index]; }
        }

        public SortedRowCollection(ICollection list, string sColumnName)
        {
            this.m_list = new ArrayList();
            foreach (object obj in list)
            {
                this.m_list.Add(obj);
            }

            this.m_list.Sort(new SortedRowCollection.RowComparer(sColumnName));
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_list.GetEnumerator();
        }

        private class RowComparer : IComparer
        {
            private string m_sColumnName;

            public RowComparer(string sColumnName)
            {
                this.m_sColumnName = sColumnName;
            }

            int System.Collections.IComparer.Compare(object x, object y)
            {
                object item = ((DataRow)x)[this.m_sColumnName];
                object obj = ((DataRow)y)[this.m_sColumnName];
                if (typeof(IComparable).IsAssignableFrom(item.GetType()) &&
                    typeof(IComparable).IsAssignableFrom(obj.GetType()))
                {
                    return ((IComparable)item).CompareTo(obj);
                }

                if (item.GetType().IsAssignableFrom(typeof(DBNull)) && !obj.GetType().IsAssignableFrom(typeof(DBNull)))
                {
                    return -1;
                }

                if (!item.GetType().IsAssignableFrom(typeof(DBNull)) && obj.GetType().IsAssignableFrom(typeof(DBNull)))
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}