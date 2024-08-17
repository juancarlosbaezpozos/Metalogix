using System;

namespace Metalogix
{
    [Flags]
    public enum ResourceScope
    {
        ApplicationAndUserSpecific = 1,
        ApplicationSpecific = 2,
        Application = 3,
        UserSpecific = 4,
        User = 5,
        EnvironmentSpecific = 8,
        Environment = 15
    }
}