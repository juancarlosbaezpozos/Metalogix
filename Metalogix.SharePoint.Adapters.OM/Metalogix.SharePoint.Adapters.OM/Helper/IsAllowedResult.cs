using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.OM.Helper
{
    internal class IsAllowedResult
    {
        public bool Allowed { get; set; }

        public string ErrorMessage { get; set; }

        public IsAllowedResult()
        {
            this.Allowed = false;
            this.ErrorMessage = string.Empty;
        }
    }
}