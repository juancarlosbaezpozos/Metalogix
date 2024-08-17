using System;
using System.Collections.Generic;

namespace Metalogix.Office365
{
    public class PackagingStatus
    {
        private readonly List<FileBlob> _fileList = new List<FileBlob>();

        private readonly List<FileBlob> _manifestList = new List<FileBlob>();

        public int ObjectCount;

        public int ListCount;

        public int ListItemCount;

        public int FileCount;

        public ulong TotalSize;

        public int FolderCount;

        public int MaxURLLength;

        public int MaxDirNameLength;

        public int MaxFileNameLength;

        public int URLLengthWarnings;

        public int DirNameLengthWarnings;

        public int FileNameLengthWarnings;

        public int FileTypeWarnings;

        public int FileNameWarnings;

        public int Warnings;

        public int Errors;

        public int AlertCount;

        public List<FileBlob> FileList
        {
            get { return this._fileList; }
        }

        public List<FileBlob> ManifestList
        {
            get { return this._manifestList; }
        }

        public PackagingStatus()
        {
        }

        public void AddFile(string fileName, string filePath)
        {
            FileBlob fileBlob = new FileBlob()
            {
                FileName = fileName,
                FullPath = filePath
            };
            this._fileList.Add(fileBlob);
        }

        public void AddManifest(string fileName, string filePath)
        {
            FileBlob fileBlob = new FileBlob()
            {
                FileName = fileName,
                FullPath = filePath
            };
            this._manifestList.Add(fileBlob);
        }
    }
}