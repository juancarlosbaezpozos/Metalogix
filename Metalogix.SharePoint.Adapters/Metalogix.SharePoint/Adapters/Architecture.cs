using System;

namespace Metalogix.SharePoint.Adapters
{
    public enum Architecture : ushort
    {
        PROCESSOR_ARCHITECTURE_X86 = 0,
        PROCESSOR_ARCHITECTURE_IA64 = 6,
        PROCESSOR_ARCHITECTURE_AMD64 = 9,
        PROCESSOR_ARCHITECTURE_UNKNOWN = 65535
    }
}