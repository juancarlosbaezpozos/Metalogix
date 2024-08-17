using System;

namespace Metalogix.Office365
{
    public enum SPEventType
    {
        All = -1,
        Add = 1,
        Modify = 2,
        Delete = 4,
        Discussion = 4080
    }
}