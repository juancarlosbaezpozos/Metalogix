using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "CSOM2013ServiceManager.IServiceManager")]
    public interface IServiceManager
    {
        [OperationContract(Action = "http://tempuri.org/IServiceManager/EndService",
            ReplyAction = "http://tempuri.org/IServiceManager/EndServiceResponse")]
        void EndService();

        [OperationContract(Action = "http://tempuri.org/IServiceManager/GetSharePointOnlineCookie",
            ReplyAction = "http://tempuri.org/IServiceManager/GetSharePointOnlineCookieResponse")]
        string GetSharePointOnlineCookie(string url, string userName, string password);

        [OperationContract(Action = "http://tempuri.org/IServiceManager/InitializeService",
            ReplyAction = "http://tempuri.org/IServiceManager/InitializeServiceResponse")]
        void InitializeService(string sXML, bool bRequireManualShutdown);
    }
}