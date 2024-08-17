using System;

namespace Metalogix.SharePoint.Adapters.Enums
{
    [Flags]
    public enum ViewTypes : long
    {
        None = 0,
        Html = 1,
        ClientModified = 2,
        TabularView = 4,
        Hidden = 8,
        LockWeb = 16,
        ReadOnly = 32,
        FailIfEmpty = 64,
        FreeForm = 128,
        FileDialog = 256,
        FileDialogTemplates = 512,
        AggregationView = 1024,
        Grid = 2048,
        Recursive = 4096,
        Recurrence = 8193,
        Contributor = 16384,
        Moderator = 32768,
        Threaded = 65536,
        Chart = 131072,
        Personal = 262144,
        Calendar = 524288,
        Default = 1048576,
        FilesOnly = 2097152,
        Ordered = 4194304,
        Mobile = 8388608,
        DefaultMobile = 16777216,
        IncludeVersions = 33554432,
        Gantt = 67108864,
        IncludeRootFolder = 134217728,
        DefaultViewForContentType = 268435456,
        HideUnapproved = 536870912,
        RequiresClientIntegration = 1073741824,
        Unknown = 2147483648
    }
}