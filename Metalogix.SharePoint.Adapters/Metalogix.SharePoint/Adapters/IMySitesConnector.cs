using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface IMySitesConnector
    {
        [OperationContract]
        string GetPersonalSite(string email);

        [OperationContract]
        bool HasPersonalSite(string email);

        [OperationContract]
        void ProvisionPersonalSites(string[] emails);

        [OperationContract]
        void RemovePersonalSite(string email);
    }
}