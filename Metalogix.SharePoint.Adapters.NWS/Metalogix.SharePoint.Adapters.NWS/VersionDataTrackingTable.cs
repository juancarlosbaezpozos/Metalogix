using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.NWS
{
    public class VersionDataTrackingTable : IEnumerable<DataRow>, IEnumerable
    {
        private readonly DataTable _data;

        private readonly SortedList<Version, DataRow> _rowIndex;

        public ReadOnlyCollection<Version> ExistingVersions
        {
            get { return new ReadOnlyCollection<Version>(this._rowIndex.Keys); }
        }

        public DataRow this[string versionString]
        {
            get { return this.GetRowForVersion(versionString); }
        }

        public DataRowCollection Rows
        {
            get { return this._data.Rows; }
        }

        public VersionDataTrackingTable()
        {
            this._data = new DataTable();
            this._rowIndex = new SortedList<Version, DataRow>();
        }

        public IEnumerator<DataRow> GetEnumerator()
        {
            return (IEnumerator<DataRow>)this._data.Rows.GetEnumerator();
        }

        public DataRow GetRowForVersion(string versionString)
        {
            return this.GetRowForVersion(versionString, false);
        }

        public DataRow GetRowForVersion(string versionString, bool createIfNotFound)
        {
            DataRow dataRow;
            Version version = this.ParseVersionString(versionString);
            if (!this._rowIndex.TryGetValue(version, out dataRow))
            {
                if (!createIfNotFound)
                {
                    return null;
                }

                dataRow = this._data.NewRow();
                this._rowIndex.Add(version, dataRow);
                this._data.Rows.InsertAt(dataRow, this._rowIndex.IndexOfKey(version));
            }

            return dataRow;
        }

        private Version ParseVersionString(string versionString)
        {
            if (string.IsNullOrEmpty(versionString) || !char.IsNumber(versionString[0]))
            {
                throw new ArgumentException("Version string not in correct format.");
            }

            if (versionString.IndexOf('.') < 0)
            {
                versionString = string.Concat(versionString, '.');
            }

            if (!char.IsNumber(versionString[versionString.Length - 1]))
            {
                versionString = string.Concat(versionString, '0');
            }

            return new Version(versionString);
        }

        public void SetValue(string versionString, string fieldName, string value)
        {
            this.SetValue(versionString, fieldName, value, true);
        }

        public void SetValue(string versionString, string fieldName, string value, bool overwriteExistingValue)
        {
            DataRow rowForVersion = this.GetRowForVersion(versionString, true);
            DataColumn item = this._data.Columns[fieldName];
            if (item == null)
            {
                item = new DataColumn(fieldName);
                this._data.Columns.Add(item);
            }

            if (!overwriteExistingValue && rowForVersion[item] != DBNull.Value)
            {
                return;
            }

            rowForVersion[item] = value;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._data.Rows.GetEnumerator();
        }
    }
}