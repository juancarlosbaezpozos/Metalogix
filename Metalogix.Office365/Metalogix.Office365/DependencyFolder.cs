using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class DependencyFolder
    {
        public string FolderXml { get; private set; }

        public string ParentFolderId { get; private set; }

        public DependencyFolder(string parentFolderId, string folderXml)
        {
            this.ParentFolderId = parentFolderId;
            this.FolderXml = folderXml;
        }
    }
}