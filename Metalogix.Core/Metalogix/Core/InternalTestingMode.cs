using System;

namespace Metalogix.Core
{
    public enum InternalTestingMode
    {
        None,
        AzureNoQueueResponseBatchSubmission,
        AzureStorageContainersFailToDelete
    }
}