using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface IServiceManager
    {
        [OperationContract]
        void EndService();

        [OperationContract]
        string GetSharePointOnlineCookie(string url, string userName, string password);

        [OperationContract]
        void InitializeService(string sXML, bool bRequireManualShutdown);
    }
}