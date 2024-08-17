using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.DB
{
    public sealed class UtilityLauncherException : Exception
    {
        public string Args { get; private set; }

        public UtilityLauncherException(string args, string message) : base(message)
        {
            this.Args = args;
        }
    }
}