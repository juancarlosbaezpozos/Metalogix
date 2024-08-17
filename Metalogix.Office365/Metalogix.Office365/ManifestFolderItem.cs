using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestFolderItem : BaseManifestItemWithAttachments
    {
        public string Foldername { get; set; }

        public bool IsReferenceOnly { get; set; }

        public List<ManifestFolderItem> Versions { get; private set; }

        public ManifestFolderItem(bool hasVersioning)
        {
            List<ManifestFolderItem> manifestFolderItems;
            base.ObjectType = ManifestObjectType.Folder;
            base.HasVersioning = hasVersioning;
            if (hasVersioning)
            {
                manifestFolderItems = new List<ManifestFolderItem>();
            }
            else
            {
                manifestFolderItems = null;
            }

            this.Versions = manifestFolderItems;
        }
    }
}