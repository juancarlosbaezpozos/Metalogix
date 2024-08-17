using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Metabase
{
    public class RecordCollection : List<Record>
    {
        private Type m_typeCollection;

        public Type CollectionType
        {
            get
            {
                if (this.m_typeCollection == null)
                {
                    return typeof(Record);
                }

                return this.m_typeCollection;
            }
        }

        public new object this[int index]
        {
            get { return base[index]; }
        }

        public RecordCollection()
        {
        }

        public void Add(Record item)
        {
            base.Add(item);
            if (item == null)
            {
                return;
            }

            this.UpdateCollectionType(item);
        }

        public void AddRange(IEnumerable<Record> collection)
        {
            base.AddRange(collection);
            foreach (Record record in collection)
            {
                this.UpdateCollectionType(record);
            }
        }

        private void UpdateCollectionType(Record rec)
        {
            if (this.m_typeCollection == null)
            {
                this.m_typeCollection = rec.GetType();
                return;
            }

            if (this.m_typeCollection != rec.GetType())
            {
                this.m_typeCollection = typeof(Record);
            }
        }
    }
}