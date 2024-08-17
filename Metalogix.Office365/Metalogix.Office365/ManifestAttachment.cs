using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class ManifestAttachment
    {
        public string Filename { get; set; }

        public int FileSize { get; set; }

        public string LocalFilename { get; set; }

        public ManifestAttachment()
        {
        }
    }
}