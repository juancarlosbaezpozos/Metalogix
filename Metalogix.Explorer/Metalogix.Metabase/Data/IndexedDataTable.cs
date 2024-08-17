using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Metalogix.Metabase.Data
{
    [Serializable]
    public class IndexedDataTable : DataTable
    {
        protected Dictionary<string, HashIndex> m_dictIndices = new Dictionary<string, HashIndex>();

        public IndexedDataTable()
        {
            IndexedDataTable indexedDataTable = this;
            base.RowChanging += new DataRowChangeEventHandler(indexedDataTable.On_DataTable_RowChanging);
        }

        public void AddIndex(string sIndexName, DataColumn keyColumn, bool bUnique)
        {
            this.AddIndex(sIndexName, new HashIndex(keyColumn, bUnique));
        }

        public void AddIndex(string sIndexName, DataColumn colIndexColumn, DataColumn colIndexableValue,
            object[] indexableValues, bool bUnique)
        {
            this.AddIndex(sIndexName,
                new SelectiveHashIndex(colIndexableValue, colIndexColumn, indexableValues, bUnique));
        }

        public void AddIndex(string sIndexName, HashIndex index)
        {
            this.m_dictIndices[sIndexName] = index;
            this.BuildIndex(sIndexName);
        }

        public void BuildIndex(string sIndexName)
        {
            if (sIndexName == null)
            {
                return;
            }

            if (!this.m_dictIndices.ContainsKey(sIndexName))
            {
                return;
            }

            HashIndex hashIndex = null;
            this.m_dictIndices.TryGetValue(sIndexName, out hashIndex);
            if (hashIndex == null)
            {
                return;
            }

            hashIndex.Clear();
            for (int i = 0; i < base.Rows.Count; i++)
            {
                hashIndex.AddRowToIndex(base.Rows[i]);
            }
        }

        public ArrayList FindAllRows(string sIndexName, object keyValue)
        {
            if (sIndexName == null)
            {
                return null;
            }

            HashIndex hashIndex = null;
            this.m_dictIndices.TryGetValue(sIndexName, out hashIndex);
            if (hashIndex == null)
            {
                return null;
            }

            return hashIndex.FindAllRows(new object[] { keyValue });
        }

        public DataRow FindRow(string sIndexName, object keyValue)
        {
            HashIndex hashIndex;
            if (sIndexName == null)
            {
                return null;
            }

            if (!this.m_dictIndices.TryGetValue(sIndexName, out hashIndex))
            {
                return null;
            }

            return hashIndex.FindRow(new object[] { keyValue });
        }

        public HashIndex GetIndex(string sIndexName)
        {
            HashIndex hashIndex = null;
            this.m_dictIndices.TryGetValue(sIndexName, out hashIndex);
            return hashIndex;
        }

        protected virtual void On_DataTable_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            if (this.m_dictIndices.Count == 0)
            {
                return;
            }

            DataRowAction action = e.Action;
            if (action == DataRowAction.Change)
            {
                foreach (HashIndex value in this.m_dictIndices.Values)
                {
                    value.UpdateRowInIndex(e.Row);
                }
            }
            else if (action != DataRowAction.Commit)
            {
                if (action != DataRowAction.Add)
                {
                    return;
                }

                foreach (HashIndex hashIndex in this.m_dictIndices.Values)
                {
                    hashIndex.AddRowToIndex(e.Row);
                }
            }
            else if (e.Row.RowState == DataRowState.Deleted)
            {
                foreach (HashIndex value1 in this.m_dictIndices.Values)
                {
                    value1.RemoveRowFromIndex(e.Row);
                }
            }
        }
    }
}