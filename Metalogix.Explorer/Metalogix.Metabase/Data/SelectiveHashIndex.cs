using System;
using System.Collections;
using System.Data;

namespace Metalogix.Metabase.Data
{
    public class SelectiveHashIndex : HashIndex
    {
        private Hashtable m_hashIndexableValues;

        private DataColumn m_colIndexableValue;

        public SelectiveHashIndex(DataColumn colIndexableValue, DataColumn colIndex, object[] indexableValues,
            bool bUnique)
        {
            this.m_hashIndexableValues = new Hashtable();
            if (indexableValues != null && (int)indexableValues.Length > 0)
            {
                object[] objArray = indexableValues;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    object obj = objArray[i];
                    if (obj != null)
                    {
                        this.m_hashIndexableValues[obj] = 1;
                    }
                }
            }

            this.m_colIndexableValue = colIndexableValue;
            this.m_colIndex = colIndex;
            this.m_bUnique = bUnique;
        }

        public void AddIndexableValue(object obj)
        {
            if (obj == null)
            {
                return;
            }

            this.m_hashIndexableValues[obj] = 1;
        }

        protected override bool IsIndexable(DataRow dataRow)
        {
            if (dataRow.RowState == DataRowState.Deleted)
            {
                return false;
            }

            object item = dataRow[this.m_colIndexableValue];
            if (item == null || item == DBNull.Value)
            {
                return false;
            }

            return this.m_hashIndexableValues.Contains(item);
        }

        public void RemoveIndexableValue(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (this.m_hashIndexableValues.Contains(obj))
            {
                this.m_hashIndexableValues.Remove(obj);
            }
        }
    }
}