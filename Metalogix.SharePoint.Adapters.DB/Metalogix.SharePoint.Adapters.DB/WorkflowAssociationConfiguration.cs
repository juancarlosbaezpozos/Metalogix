using System;

namespace Metalogix.SharePoint.Adapters.DB
{
    public enum WorkflowAssociationConfiguration
    {
        None = 0,
        AutoStartAdd = 1,
        AutoStartChange = 2,
        AutoStartColumnChange = 4,
        AllowManualStart = 8,
        HasStatusColumn = 16,
        LockItem = 32,
        Declarative = 64,
        NoNewWorkflows = 128,
        MarkedForDelete = 512,
        GloballyDisabled = 1024,
        CompressInstanceData = 4096,
        SiteOverQuota = 8192,
        SiteWriteLocked = 16384,
        AllowAsyncManualStart = 32768
    }
}