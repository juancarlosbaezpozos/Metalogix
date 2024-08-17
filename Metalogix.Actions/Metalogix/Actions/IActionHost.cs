using System;

namespace Metalogix.Actions
{
    public interface IActionHost
    {
        Type[] LicensedActions { get; }
    }
}