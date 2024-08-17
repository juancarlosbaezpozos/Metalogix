using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface IMigrationPipeline
    {
        [OperationContract]
        string DeleteMigrationJob(string jobConfiguration);

        [OperationContract]
        string GetMigrationJobStatus(string jobConfiguration);

        [OperationContract]
        string ProvisionMigrationContainer();

        [OperationContract]
        string ProvisionMigrationQueue();

        [OperationContract]
        string RequestMigrationJob(string jobConfiguration, bool isMicrosoftCustomer, byte[] encryptionKey = null);

        [OperationContract]
        string ResolvePrincipals(string principal);
    }
}