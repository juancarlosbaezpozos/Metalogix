using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class ServiceManagerClient :
        ClientBase<Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager.IServiceManager>,
        Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager.IServiceManager
    {
        public ServiceManagerClient()
        {
        }

        public ServiceManagerClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public ServiceManagerClient(string endpointConfigurationName, string remoteAddress) : base(
            endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceManagerClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(
            endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceManagerClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        public void EndService()
        {
            base.Channel.EndService();
        }

        public string GetSharePointOnlineCookie(string url, string userName, string password)
        {
            return base.Channel.GetSharePointOnlineCookie(url, userName, password);
        }

        public void InitializeService(string sXML, bool bRequireManualShutdown)
        {
            base.Channel.InitializeService(sXML, bRequireManualShutdown);
        }
    }
}