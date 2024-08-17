using System;

namespace Metalogix.SharePoint.Adapters
{
    public interface IDBReader : ISharePointReader, ISharePointAdapterCommand
    {
        string ConnectionString { get; }

        string CustomTemplatePath { get; }

        string Database { get; }

        string HostHeader { get; set; }

        void CheckConnection(bool bDoSomething);

        string GetSQLDatabaseList();

        bool SetCustomTemplatePath(string sCustomTemplatePath);

        bool SetLinkName(string sHostName);
    }
}