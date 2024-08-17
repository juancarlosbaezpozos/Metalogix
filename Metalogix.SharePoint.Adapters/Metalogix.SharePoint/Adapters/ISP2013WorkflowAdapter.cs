using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface ISP2013WorkflowAdapter
    {
        [OperationContract]
        string DeleteSP2013Workflows(string configurationXml);

        [OperationContract]
        string GetSP2013Workflows(string configurationXml);

        [OperationContract]
        string IsWorkflowServicesInstanceAvailable();

        [OperationContract]
        string MigrateSP2013Workflows(string configurationXml);
    }
}