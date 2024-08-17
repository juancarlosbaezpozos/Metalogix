using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Metabase
{
    public class RecordPropertyValueList
    {
        private Record m_item;

        private RecordPropertyDescriptorList m_propertyList;

        public int Count
        {
            get { return this.m_propertyList.Count; }
        }

        public RecordPropertyValue this[int index]
        {
            get
            {
                PropertyDescriptor item = this.m_propertyList[index];
                return new RecordPropertyValue(this.m_item, item);
            }
        }

        public RecordPropertyValue this[string strPropertyName]
        {
            get
            {
                PropertyDescriptor propertyDescriptor = this.m_propertyList.Find(strPropertyName);
                if (propertyDescriptor == null)
                {
                    return null;
                }

                return new RecordPropertyValue(this.m_item, propertyDescriptor);
            }
        }

        internal RecordPropertyValueList(Record item, RecordPropertyDescriptorList propertyList)
        {
            this.m_item = item;
            this.m_propertyList = propertyList;
        }

        public IEnumerator GetEnumerator()
        {
            return new RecordPropertyValueList.ItemPropertyValueListEnumerator(this);
        }

        private class ItemPropertyValueListEnumerator : IEnumerator
        {
            private int iPos;

            private RecordPropertyValueList m_list;

            public object Current
            {
                get
                {
                    if (this.m_list == null)
                    {
                        return null;
                    }

                    if (this.iPos < 0 || this.iPos >= this.m_list.Count)
                    {
                        return null;
                    }

                    return this.m_list[this.iPos];
                }
            }

            public ItemPropertyValueListEnumerator(RecordPropertyValueList list)
            {
                this.m_list = list;
            }

            public bool MoveNext()
            {
                if (this.m_list == null)
                {
                    return false;
                }

                this.iPos++;
                return this.iPos < this.m_list.Count;
            }

            public void Reset()
            {
                this.iPos = 0;
            }
        }
    }
}