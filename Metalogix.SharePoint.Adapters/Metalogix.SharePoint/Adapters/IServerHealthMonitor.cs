using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface IServerHealthMonitor
    {
        [OperationContract]
        string GetServerHealth();
    }
}