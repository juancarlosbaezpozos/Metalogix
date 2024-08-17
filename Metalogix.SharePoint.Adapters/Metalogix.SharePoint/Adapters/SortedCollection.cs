using System;
using System.Collections;
using System.ComponentModel;

namespace Metalogix.SharePoint.Adapters
{
    public class SortedCollection
    {
        private ArrayList m_list;

        public int Count
        {
            get { return this.m_list.Count; }
        }

        public SortedCollection(ICollection list, PropertyDescriptor pd)
        {
            this.m_list = new ArrayList();
            foreach (object obj in list)
            {
                this.m_list.Add(obj);
            }

            this.m_list.Sort(new SortedCollection.ReflectComparer(pd));
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_list.GetEnumerator();
        }

        private class ReflectComparer : IComparer
        {
            private PropertyDescriptor m_pd;

            public ReflectComparer(PropertyDescriptor pd)
            {
                this.m_pd = pd;
            }

            int System.Collections.IComparer.Compare(object x, object y)
            {
                object value = this.m_pd.GetValue(x);
                object obj = this.m_pd.GetValue(y);
                return string.Compare(value.ToString(), obj.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}