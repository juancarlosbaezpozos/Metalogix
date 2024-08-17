using System;
using System.Collections;
using System.Data;

namespace Metalogix.Metabase.Data
{
    public class HashIndex
    {
        protected Hashtable m_hashIndex = new Hashtable();

        protected DataColumn m_colIndex;

        protected bool m_bUnique;

        protected HashIndex()
        {
        }

        public HashIndex(DataColumn colIndex, bool bUnique)
        {
            this.m_bUnique = bUnique;
            this.m_colIndex = colIndex;
        }

        private void Add(object keyValue, DataRow dataRow)
        {
            ArrayList item;
            if (keyValue == null || keyValue == DBNull.Value)
            {
                return;
            }

            if (this.m_bUnique)
            {
                if (this.m_hashIndex.Contains(keyValue))
                {
                    throw new DuplicateNameException("Duplicate key value found.");
                }

                this.m_hashIndex[keyValue] = dataRow;
                return;
            }

            if (this.m_hashIndex.Contains(keyValue))
            {
                item = (ArrayList)this.m_hashIndex[keyValue];
            }
            else
            {
                item = new ArrayList();
                this.m_hashIndex[keyValue] = item;
            }

            if (!item.Contains(dataRow))
            {
                item.Add(dataRow);
            }
        }

        public void AddRowToIndex(DataRow dataRow)
        {
            if (!this.IsIndexable(dataRow))
            {
                return;
            }

            this.Add(this.GetKey(dataRow), dataRow);
        }

        public void Clear()
        {
            if (this.m_hashIndex == null)
            {
                return;
            }

            this.m_hashIndex.Clear();
        }

        public ArrayList FindAllRows(object[] keyValues)
        {
            if (this.m_hashIndex == null)
            {
                throw new Exception("No index available");
            }

            if (this.m_bUnique)
            {
                throw new Exception("Index is unique. Cannot return multiple rows");
            }

            object key = this.GetKey(keyValues);
            if (!this.m_hashIndex.Contains(key))
            {
                return null;
            }

            return (ArrayList)this.m_hashIndex[key];
        }

        public DataRow FindRow(object[] keyValues)
        {
            if (this.m_hashIndex == null)
            {
                throw new Exception("No index available");
            }

            object key = this.GetKey(keyValues);
            if (!this.m_hashIndex.Contains(key))
            {
                return null;
            }

            if (this.m_bUnique)
            {
                return (DataRow)this.m_hashIndex[key];
            }

            ArrayList item = (ArrayList)this.m_hashIndex[key];
            if (item.Count == 0)
            {
                return null;
            }

            return (DataRow)item[0];
        }

        protected virtual object GetKey(DataRow dataRow)
        {
            if (dataRow.RowState != DataRowState.Deleted)
            {
                return dataRow[this.m_colIndex.ColumnName];
            }

            return dataRow[this.m_colIndex.ColumnName, DataRowVersion.Original];
        }

        protected virtual object GetKey(object[] objValues)
        {
            if (objValues == null || (int)objValues.Length == 0)
            {
                return null;
            }

            object obj = objValues[0];
            if (obj != null && obj.GetType() != this.m_colIndex.DataType)
            {
                obj = Convert.ChangeType(objValues[0], this.m_colIndex.DataType);
            }

            return obj;
        }

        protected virtual bool IsIndexable(DataRow dataRow)
        {
            return true;
        }

        private void Remove(object keyValue, DataRow dataRow)
        {
            if (keyValue == null)
            {
                return;
            }

            if (this.m_hashIndex.Contains(keyValue))
            {
                if (this.m_bUnique)
                {
                    this.m_hashIndex.Remove(keyValue);
                    return;
                }

                ArrayList item = (ArrayList)this.m_hashIndex[keyValue];
                item.Remove(dataRow);
                if (item.Count == 0)
                {
                    this.m_hashIndex.Remove(keyValue);
                }
            }
        }

        public void RemoveRowFromIndex(DataRow dataRow)
        {
            this.Remove(this.GetKey(dataRow), dataRow);
        }

        public virtual void UpdateRowInIndex(DataRow row)
        {
            if (!this.IsIndexable(row))
            {
                return;
            }

            object item = null;
            object obj = null;
            object item1 = null;
            if (row.HasVersion(DataRowVersion.Original))
            {
                item = row[this.m_colIndex.ColumnName, DataRowVersion.Original];
            }

            if (row.HasVersion(DataRowVersion.Current))
            {
                obj = row[this.m_colIndex.ColumnName, DataRowVersion.Current];
            }

            if (row.HasVersion(DataRowVersion.Proposed))
            {
                item1 = row[this.m_colIndex.ColumnName, DataRowVersion.Proposed];
            }

            if (item != null && obj != null && item != obj)
            {
                this.Remove(item, row);
                this.Add(obj, row);
            }

            if (obj != null && item1 != null && obj != item1)
            {
                this.Remove(obj, row);
                this.Add(item1, row);
            }
        }
    }
}