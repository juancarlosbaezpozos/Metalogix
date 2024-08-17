using Metalogix.Metabase.DataTypes;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Metalogix.Metabase.Interfaces
{
    public interface IMetabaseAdapter : IDisposable
    {
        Metalogix.Metabase.Interfaces.AdapterCallWrapper AdapterCallWrapper { get; }

        string AdapterContext { get; }

        string AdapterType { get; }

        DbTransaction BeginTransaction();

        void Close();

        void CommitItem(DataRow row);

        void CommitItemData(DataSet dataSet);

        void CommitItemProperties(ArrayList arrayRows);

        void CommitItemProperties(DataTable dataTable);

        void CommitItems(DataTable dataTableItems, DataTable dataTableItemProperties);

        void CommitWorkspace(DataRow row);

        void CommitWorkspaces(DataTable dataTable);

        void ExpandTextStorage(string sPropertyName, string workspaceId);

        DataSet FetchItems(string workspaceId);

        DataSet FetchSingleItem(string workspaceId, string sourceUrl);

        DataTable FetchWorkspaceList();

        int IncrementItemNum();

        string LoadTextMoniker(TextMoniker txtMon);

        void Open();

        void SaveTextMoniker(TextMoniker txtMon, string sFullText);
    }
}