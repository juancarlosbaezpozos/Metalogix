using System;

namespace Metalogix.Licensing
{
    [Flags]
    public enum LicensedSharePointVersions
    {
        None = 0,
        SP2003 = 1,
        SP2007 = 2,
        SP2010 = 4,
        SP2013 = 8,
        SP2016 = 16,
        SPOnline = 32,
        All = 63
    }
}