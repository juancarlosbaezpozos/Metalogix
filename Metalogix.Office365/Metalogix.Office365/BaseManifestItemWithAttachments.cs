using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class BaseManifestItemWithAttachments : BaseManifestItem
    {
        public List<ManifestAttachment> Attachments { get; private set; }

        public BaseManifestItemWithAttachments()
        {
            this.Attachments = new List<ManifestAttachment>();
        }
    }
}