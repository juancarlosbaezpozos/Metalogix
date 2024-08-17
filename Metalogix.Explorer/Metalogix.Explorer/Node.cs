using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.ExternalConnections;
using Metalogix.Metabase;
using Metalogix.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Xml;

namespace Metalogix.Explorer
{
    public interface Node : Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
    {
        Metalogix.Permissions.AzureAdGraphCredentials AzureAdGraphCredentials { get; set; }

        NodeCollection Children { get; }

        bool ChildrenFetched { get; }

        Metalogix.Explorer.Connection Connection { get; }

        Metalogix.Permissions.Credentials Credentials { get; set; }

        string DisplayName { get; }

        string DisplayUrl { get; }

        string ErrorDescription { get; }

        string ErrorMsg { get; set; }

        System.Drawing.Image Image { get; }

        [IsSystem(true)] string ImageName { get; }

        bool InUseAsSource { get; }

        bool IsConnectionRoot { get; }

        string this[string sFieldName] { get; }

        string LinkableUrl { get; }

        [IsSystem(true)] bool LoadAutomatically { get; }

        Metalogix.Explorer.Location Location { get; }

        Metalogix.Metabase.MetabaseConnection MetabaseConnection { get; set; }

        string Name { get; }

        Node Parent { get; }

        string Path { get; }

        Metalogix.Metabase.Record Record { get; }

        string ServerRelativeUrl { get; }

        [IsSystem(true)] ConnectionStatus Status { get; }

        string Url { get; }

        string WorkspaceName { get; }

        string XML { get; }

        void AddExternalConnection(ExternalConnection connection);

        void CheckExternalConnections<T>();

        bool CheckWriteAccess(out string sFailureMessage);

        DummyNode CloneDummy();

        Workspace CreateOrGetWorkspace(string workspaceName);

        void DeleteWorkspace(string workspaceName);

        void FetchChildren();

        Dictionary<int, ExternalConnection> GetExternalConnectionsOfType<T>(bool recurseUp);

        Node GetNodeByPath(string sPath);

        Node GetNodeByUrl(string sURL);

        XmlNode GetNodeXML();

        Metalogix.Metabase.Record GetRecord();

        Metalogix.Metabase.Record GetRecord(Workspace workspace);

        void ReleaseChildren();

        void RemoveExternalConnection(ExternalConnection connection);

        void ResetActionSourceState();

        void SetAsActionSource();

        event NodeChildrenChangedHandler ChildrenChanged;

        event DisplayNameChangedHandler DisplayNameChanged;

        event SetBusyDisplayHandler SetBusyDisplay;

        event NodeStatusChangedHandler StatusChanged;
    }
}