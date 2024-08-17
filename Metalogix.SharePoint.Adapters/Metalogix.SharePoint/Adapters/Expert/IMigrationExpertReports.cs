using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters.Expert
{
    [ServiceContract]
    public interface IMigrationExpertReports
    {
        [OperationContract]
        string GetAddIns(string options);

        [OperationContract]
        string GetAdImportDcMappings(string profileDbConnectionString, string connName, string connType,
            string options);

        [OperationContract]
        string GetBcsApplications(string options);

        [OperationContract]
        string GetBrowserFileHandling(string options);

        [OperationContract]
        string GetCustomProfilePropertyMapping(string options);

        [OperationContract]
        string GetFarmSandboxSolutions(string options);

        [OperationContract]
        string GetFarmServerDetails(string options);

        [OperationContract]
        byte[] GetFarmSolutionBinary(string solutionName);

        [OperationContract]
        string GetFarmSolutions(string options);

        [OperationContract]
        string GetFileVersions(string options);

        [OperationContract]
        string GetInfopaths(string options);

        [OperationContract]
        string GetListWorkflowRunning2010(string listName);

        [OperationContract]
        string GetLockedSites(string options);

        [OperationContract]
        string GetSecureStorageApplications(string options);

        [OperationContract]
        byte[] GetSiteSolutionsBinary(string itemId);

        [OperationContract]
        string GetUserProfilePropertiesUsage(string profileDbConnectionString, string options);

        [OperationContract]
        string GetWebApplicationPolicies(string options);

        [OperationContract]
        string GetWorkflowAssociation2010(string options);

        [OperationContract]
        string GetWorkflowAssociation2013(string options);

        [OperationContract]
        string GetWorkflowRunning2010(string options);

        [OperationContract]
        string GetWorkflowRunning2013(string options);

        [OperationContract]
        string IsAppWebPartOnPage(Guid appProductId, string itemUrl);
    }
}