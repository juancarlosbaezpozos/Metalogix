using Metalogix.Metabase;
using System;
using System.Xml;

namespace Metalogix.Explorer
{
    public interface Connection
    {
        string AdapterName { get; }

        string ConnectionString { get; }

        bool? IsSharePointOnline { get; }

        Metalogix.Metabase.MetabaseConnection MetabaseConnection { get; set; }

        Metalogix.Explorer.Node Node { get; }

        ConnectionStatus Status { get; }

        string VersionNumber { get; }

        void CheckConnection();

        Connection Clone();

        void Close();

        bool ConnectionEquals(XmlNode connXML);

        void UpdateLicensedStatus();
    }
}