using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestList : BaseManifestItem
    {
        public string ServerRelativeURL { get; set; }

        public ManifestList()
        {
            this.ServerRelativeURL = string.Empty;
            base.ObjectType = ManifestObjectType.List;
        }
    }
}