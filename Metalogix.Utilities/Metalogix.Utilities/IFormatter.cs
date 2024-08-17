using System;

namespace Metalogix.Utilities
{
    public interface IFormatter
    {
        string FormatData(long? lData, string sUnits);
    }
}