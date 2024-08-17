using System;

namespace Metalogix
{
    [Flags]
    public enum AssemblyTiers
    {
        Referenced = 1,
        Signed = 2,
        Unsigned = 4
    }
}