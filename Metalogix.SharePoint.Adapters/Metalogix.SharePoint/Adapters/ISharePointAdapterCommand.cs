using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface ISharePointAdapterCommand
    {
        [OperationContract]
        string ExecuteCommand(string commandName, string commandConfigurationXml);
    }
}