using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestListItem : BaseManifestItemWithAttachments
    {
        public string Filename { get; set; }

        public long FileSize { get; set; }

        public List<ManifestListItem> Versions { get; private set; }

        public ManifestListItem(bool hasVersioning)
        {
            List<ManifestListItem> manifestListItems;
            base.ObjectType = ManifestObjectType.ListItem;
            if (hasVersioning)
            {
                manifestListItems = new List<ManifestListItem>();
            }
            else
            {
                manifestListItems = null;
            }

            this.Versions = manifestListItems;
            base.HasVersioning = hasVersioning;
        }
    }
}