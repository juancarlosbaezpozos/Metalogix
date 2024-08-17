using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestFileItem : BaseManifestItem
    {
        public string Filename { get; set; }

        public int FileSize { get; set; }

        public string LocalFilename { get; set; }

        public List<ManifestFileItem> Versions { get; private set; }

        public ManifestFileItem(bool hasVersioning)
        {
            List<ManifestFileItem> manifestFileItems;
            base.ObjectType = ManifestObjectType.File;
            if (hasVersioning)
            {
                manifestFileItems = new List<ManifestFileItem>();
            }
            else
            {
                manifestFileItems = null;
            }

            this.Versions = manifestFileItems;
            base.HasVersioning = hasVersioning;
        }
    }
}