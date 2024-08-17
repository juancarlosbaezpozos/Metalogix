using Metalogix.DataResolution.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Metalogix.DataResolution
{
    public class FolderDataResolver : DataResolver<FolderResolverOptions>
    {
        private readonly object s_ensureActionTemplateFolderLock = new object();

        public FolderDataResolver(string dataFolderPath)
        {
            base.ResolverOptions.FolderPath = dataFolderPath;
        }

        public FolderDataResolver()
        {
        }

        public override void ClearAllData()
        {
            foreach (string availableDataKey in this.GetAvailableDataKeys())
            {
                this.DeleteDataAtKey(availableDataKey);
            }
        }

        public override void DeleteDataAtKey(string key)
        {
            string folderPath = base.ResolverOptions.FolderPath;
            FileInfo fileInfo = new FileInfo(this.GetFilePathFromKey(key, folderPath));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        private void EnsureDataFolderExists(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                return;
            }

            lock (this.s_ensureActionTemplateFolderLock)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
            }
        }

        public override IEnumerable<string> GetAvailableDataKeys()
        {
            string folderPath = base.ResolverOptions.FolderPath;
            this.EnsureDataFolderExists(folderPath);
            List<string> strs = new List<string>();
            string[] files = Directory.GetFiles(folderPath, "*.xml");
            for (int i = 0; i < (int)files.Length; i++)
            {
                strs.Add(Path.GetFileNameWithoutExtension(files[i]));
            }

            return strs;
        }

        public override byte[] GetDataAtKey(string key)
        {
            string folderPath = base.ResolverOptions.FolderPath;
            this.EnsureDataFolderExists(folderPath);
            string filePathFromKey = this.GetFilePathFromKey(key, folderPath);
            if (!File.Exists(filePathFromKey))
            {
                return new byte[0];
            }

            return File.ReadAllBytes(filePathFromKey);
        }

        private string GetFilePathFromKey(string key, string folderPath)
        {
            return Path.Combine(folderPath, string.Concat(key, ".xml"));
        }

        public override void WriteDataAtKey(string key, byte[] data)
        {
            string folderPath = base.ResolverOptions.FolderPath;
            this.EnsureDataFolderExists(folderPath);
            File.WriteAllBytes(this.GetFilePathFromKey(key, folderPath), data);
        }
    }
}