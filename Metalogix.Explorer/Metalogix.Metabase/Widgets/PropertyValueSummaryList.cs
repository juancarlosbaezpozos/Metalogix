using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.Reflection;

namespace Metalogix.Metabase.Widgets
{
    public class PropertyValueSummaryList : IEnumerable, IStringHash
    {
        protected Hashtable m_hashItems = new Hashtable();

        protected ArrayList m_listItems = new ArrayList();

        public int Count
        {
            get { return this.m_hashItems.Count; }
        }

        public PropertyValueSummary this[string strKey]
        {
            get { return (PropertyValueSummary)this.m_hashItems[strKey]; }
        }

        public PropertyValueSummary this[int iIndex]
        {
            get { return (PropertyValueSummary)this.m_listItems[iIndex]; }
        }

        public PropertyValueSummaryList()
        {
        }

        public virtual void Add(PropertyValueSummary propertyValueSummary)
        {
            string propertyStringValue = propertyValueSummary.PropertyStringValue;
            if (this.m_hashItems.ContainsKey(propertyStringValue))
            {
                throw new Exception("This key already exists.");
            }

            this.m_hashItems.Add(propertyStringValue, propertyValueSummary);
            this.m_listItems.Add(propertyValueSummary);
        }

        public virtual void Clear()
        {
            this.m_hashItems.Clear();
            this.m_listItems.Clear();
        }

        public bool ContainsKey(string strKey)
        {
            return this.m_hashItems.ContainsKey(strKey);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return this.m_listItems.GetEnumerator();
        }

        public void Remove(PropertySummary propertySummary)
        {
            this.m_hashItems.Remove(propertySummary.PropertyStringValue);
            this.m_listItems.Remove(propertySummary);
        }
    }
}