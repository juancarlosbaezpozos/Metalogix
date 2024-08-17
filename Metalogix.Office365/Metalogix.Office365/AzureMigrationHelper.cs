using Metalogix.Actions;
using Metalogix.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Office365
{
    public class AzureMigrationHelper : IOperationState
    {
        private const int MaxUploadThreads = 4;

        private const int DefaultWaitTimeMSPolling10s = 10000;

        private const int MaxRetryAttemptsForPolling = 30;

        private const string DummyDatFileName = "0000000000000000.dat";

        private readonly IAzureContainerInstance _containerInstance;

        private readonly object _failedBlobLockObj;

        private readonly List<FileBlob> _failedBlobUploads;

        private readonly object _failedManifestLockObj;

        private readonly List<FileBlob> _failedManifestUploads;

        private readonly IManifestBuilder _manifestBuilder;

        private readonly IAzureBlobStorageManager _azureBlobStorageManager;

        private readonly string _documentBinariesDirectory;

        private readonly IOperationState _operationState;

        private readonly StringBuilder _status = new StringBuilder();

        private string _dummyDatFileFullPath = string.Empty;

        public IAzureContainerInstance ContainerInstance
        {
            get { return this._containerInstance; }
        }

        public bool IsOperationCancelled
        {
            get { return this._operationState.IsOperationCancelled; }
        }

        public string Status
        {
            get { return this._status.ToString(); }
        }

        public ulong TotalBytesProcessed
        {
            get { return this._manifestBuilder.Status.TotalSize; }
        }

        public AzureMigrationHelper(IAzureContainerInstance containerInstance,
            IAzureBlobStorageManager azureBlobStorageManager, IOperationState operationState,
            IManifestBuilder manifestBuilder, string documentBinariesDirectory)
        {
            this._containerInstance = containerInstance;
            this._azureBlobStorageManager = azureBlobStorageManager;
            this._operationState = operationState;
            this._manifestBuilder = manifestBuilder;
            this._failedBlobUploads = new List<FileBlob>();
            this._failedBlobLockObj = new object();
            this._failedManifestUploads = new List<FileBlob>();
            this._failedManifestLockObj = new object();
            this._documentBinariesDirectory = documentBinariesDirectory;
        }

        public void DeleteDummyDatFile()
        {
            if (File.Exists(this._dummyDatFileFullPath))
            {
                File.Delete(this._dummyDatFileFullPath);
            }
        }

        public DownloadResponse GetErrorLog(string migrationJobId)
        {
            return this.GetLog("Import", ".err", "Error", migrationJobId);
        }

        public DownloadResponse GetImportLog(string migrationJobId)
        {
            return this.GetLog("Import", ".log", "Import", migrationJobId);
        }

        private DownloadResponse GetLog(string filename, string extension, string logType, string migrationJobId)
        {
            DownloadResponse downloadResponse;
            this._status.Length = 0;
            bool flag = false;
            BlobInfo blobInfo = null;
            int num = 0;
            bool isOperationCancelled = this._operationState.IsOperationCancelled;
            string details = "";
            while (!flag && !isOperationCancelled)
            {
                num++;
                ListResponse listResponse =
                    this._azureBlobStorageManager.ListContainer(this._containerInstance.GetManifestContainer()
                        .FullAccessUri);
                if (listResponse.Success)
                {
                    blobInfo = listResponse.Blobs.FindLast((BlobInfo b) =>
                    {
                        if (!b.BlobName.StartsWith(filename) || !b.BlobName.Contains(migrationJobId))
                        {
                            return false;
                        }

                        return b.BlobName.EndsWith(extension);
                    });
                    flag = blobInfo != null;
                    if (!flag)
                    {
                        if (num == 30)
                        {
                            break;
                        }

                        Thread.Sleep(10000);
                    }

                    isOperationCancelled = this._operationState.IsOperationCancelled;
                }
                else
                {
                    details = listResponse.Details;
                    break;
                }
            }

            if (!flag)
            {
                DownloadResponse downloadResponse1 = new DownloadResponse()
                {
                    Success = false,
                    Details = (isOperationCancelled
                        ? "Operation Cancelled"
                        : string.Format("Unable to find {0} ({1}) log in the Manifest Container.", logType, extension))
                };
                downloadResponse = downloadResponse1;
                if (!string.IsNullOrEmpty(details))
                {
                    DownloadResponse downloadResponse2 = downloadResponse;
                    downloadResponse2.Details = string.Concat(downloadResponse2.Details, Environment.NewLine, details);
                }
            }
            else
            {
                string str = Path.Combine(this._manifestBuilder.TempStorageDirectory, blobInfo.BlobName);
                downloadResponse = this._azureBlobStorageManager.DownloadBlob(
                    this._containerInstance.GetManifestContainer().FullAccessUri, blobInfo.BlobName, str,
                    this._containerInstance.EncryptionKey);
                if (downloadResponse.Success)
                {
                    string str1 = File.ReadAllText(str);
                    object[] objArray = new object[] { logType, str, Environment.NewLine, str1 };
                    downloadResponse.Details = string.Format("{0} Log downloaded at {1}{2}{3}", objArray);
                }
            }

            return downloadResponse;
        }

        public string GetMigrationJobStatusConfiguration(string migrationJobId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
            {
                xmlWriter.WriteStartElement("GetMigrationJobStatus");
                xmlWriter.WriteAttributeString("MigrationJobId", migrationJobId);
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return stringBuilder.ToString();
        }

        public string GetRequestMigrationJobConfiguration(string targetWebId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
            {
                xmlWriter.WriteStartElement("RequestMigrationJob");
                xmlWriter.WriteAttributeString("WebId", targetWebId);
                xmlWriter.WriteAttributeString("AzureContainerSourceUri",
                    this._containerInstance.GetBlobContainer().MigrationUri);
                xmlWriter.WriteAttributeString("AzureContainerManifestUri",
                    this._containerInstance.GetManifestContainer().MigrationUri);
                xmlWriter.WriteAttributeString("AzureQueueReportUri",
                    this._containerInstance.GetReportingQueue().MigrationUri);
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return stringBuilder.ToString();
        }

        public DownloadResponse GetWarningLog(string migrationJobId)
        {
            return this.GetLog("Import", ".wrn", "Warning", migrationJobId);
        }

        public void ProcessQueueIntoManifest(Queue<BaseManifestItem> manifestItems)
        {
            this._manifestBuilder.CreateBasePackageXml();
            while (manifestItems.Count > 0)
            {
                BaseManifestItem baseManifestItem = manifestItems.Dequeue();
                switch (baseManifestItem.ObjectType)
                {
                    case ManifestObjectType.File:
                    {
                        this._manifestBuilder.AddFileToManifest(baseManifestItem as ManifestFileItem);
                        continue;
                    }
                    case ManifestObjectType.Folder:
                    {
                        this._manifestBuilder.AddFolderToManifest(baseManifestItem as ManifestFolderItem);
                        continue;
                    }
                    case ManifestObjectType.ListItem:
                    {
                        this._manifestBuilder.AddListItemToManifest(baseManifestItem as ManifestListItem);
                        continue;
                    }
                    case ManifestObjectType.DiscussionItem:
                    {
                        this._manifestBuilder.AddDiscussionItemToManifest(baseManifestItem as ManifestDiscussionItem);
                        continue;
                    }
                    case ManifestObjectType.Alert:
                    {
                        this._manifestBuilder.AddAlertToManifest(baseManifestItem as ManifestAlert);
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
            }

            this._manifestBuilder.SaveManifest();
        }

        public Response Upload()
        {
            this._status.Length = 0;
            Response response = new Response()
            {
                Success = true
            };
            try
            {
                if (this.IsOperationCancelled)
                {
                    response.Success = false;
                }

                Stopwatch stopwatch = new Stopwatch();
                if (response.Success)
                {
                    if (this._manifestBuilder.Status.FileList.Count != 0)
                    {
                        stopwatch.Start();
                        this.UploadFiles(this._manifestBuilder.Status.FileList,
                            this._manifestBuilder.Status.FileList.Count,
                            this._containerInstance.GetBlobContainer().FullAccessUri, this._failedBlobLockObj,
                            this._failedBlobUploads, true, this._containerInstance.EncryptionKey);
                        stopwatch.Stop();
                    }
                    else
                    {
                        this._dummyDatFileFullPath =
                            Path.Combine(this._documentBinariesDirectory, "0000000000000000.dat");
                        byte[] numArray = new byte[1];
                        if (!File.Exists(this._dummyDatFileFullPath))
                        {
                            using (FileStream fileStream = File.Create(this._dummyDatFileFullPath))
                            {
                                fileStream.Write(numArray, 0, (int)numArray.Length);
                                fileStream.Flush();
                                fileStream.Close();
                            }
                        }

                        List<FileBlob> fileBlobs = new List<FileBlob>();
                        FileBlob fileBlob = new FileBlob()
                        {
                            FileName = "0000000000000000.dat",
                            FullPath = this._dummyDatFileFullPath
                        };
                        fileBlobs.Add(fileBlob);
                        this.UploadFiles(fileBlobs, fileBlobs.Count,
                            this._containerInstance.GetBlobContainer().FullAccessUri, new object(),
                            new List<FileBlob>(), true, this._containerInstance.EncryptionKey);
                    }

                    int count = this._manifestBuilder.Status.FileList.Count - this._failedBlobUploads.Count;
                    this._status.AppendLine(string.Format("Uploaded {0} File Blobs (Documents) in {1}ms", count,
                        stopwatch.ElapsedMilliseconds));
                    if (this._failedBlobUploads.Count > 0)
                    {
                        this._status.AppendLine(
                            string.Format("Failed Blob Uploads: {0}", this._failedBlobUploads.Count));
                        foreach (FileBlob _failedBlobUpload in this._failedBlobUploads)
                        {
                            this._status.AppendLine(string.Format("-> {0} [{1}]", _failedBlobUpload.FileName,
                                _failedBlobUpload.FullPath));
                            this._status.AppendLine(string.Format("Error: {0}",
                                (_failedBlobUpload.Status != null
                                    ? _failedBlobUpload.Status.Details
                                    : "Status is null")));
                        }

                        response.Success = false;
                    }

                    if (this.IsOperationCancelled)
                    {
                        response.Success = false;
                    }
                }

                if (response.Success)
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    this.UploadFiles(this._manifestBuilder.Status.ManifestList,
                        this._manifestBuilder.Status.ManifestList.Count,
                        this._containerInstance.GetManifestContainer().FullAccessUri, this._failedManifestLockObj,
                        this._failedManifestUploads, false, this._containerInstance.EncryptionKey);
                    stopwatch.Stop();
                    int num = this._manifestBuilder.Status.ManifestList.Count - this._failedManifestUploads.Count;
                    this._status.AppendLine(string.Format("Uploaded {0} Manifest Files in {1}ms", num,
                        stopwatch.ElapsedMilliseconds));
                    if (this._failedManifestUploads.Count > 0)
                    {
                        this._status.AppendLine(string.Format("Failed Manifest Uploads: {0}",
                            this._failedManifestUploads.Count));
                        foreach (FileBlob _failedManifestUpload in this._failedManifestUploads)
                        {
                            this._status.AppendLine(string.Format("-> {0} [{1}]", _failedManifestUpload.FileName,
                                _failedManifestUpload.FullPath));
                            this._status.AppendLine(string.Format("Error: {0}",
                                (_failedManifestUpload.Status != null
                                    ? _failedManifestUpload.Status.Details
                                    : "Status is null")));
                        }

                        response.Success = false;
                    }
                }

                if (this.IsOperationCancelled)
                {
                    response.Success = false;
                }
            }
            finally
            {
                this._status.AppendLine(string.Format("Total Number of List(s): {0}",
                    this._manifestBuilder.Status.ListCount));
                this._status.AppendLine(string.Format("Total Number of Folders: {0}",
                    this._manifestBuilder.Status.FolderCount));
                this._status.AppendLine(string.Format("Total Number of Documents: {0}",
                    this._manifestBuilder.Status.FileList.Count));
                this._status.AppendLine(string.Format("Total Number of ListItems: {0}",
                    this._manifestBuilder.Status.ListItemCount));
                this._status.AppendLine(string.Format("Total Number of Objects: {0}",
                    this._manifestBuilder.Status.ObjectCount));
                this._status.AppendLine(string.Format("Total Number of Alert(s): {0}",
                    this._manifestBuilder.Status.AlertCount));
                this._status.AppendLine(string.Format("Total Size of Documents (bytes): {0}",
                    this._manifestBuilder.Status.TotalSize));
                this._status.AppendLine(string.Format("Total Number of Manifest Files: {0}",
                    this._manifestBuilder.Status.ManifestList.Count));
                this._status.AppendLine(string.Format("Total Number of Warnings: {0}",
                    this._manifestBuilder.Status.Warnings));
                this._status.AppendLine(string.Format("Total Number of Errors: {0}",
                    this._manifestBuilder.Status.Errors));
                this._status.AppendLine();
                this._status.AppendLine(string.Format("Storage SAS: {0}",
                    this._containerInstance.GetBlobContainer().FullAccessUri));
                this._status.AppendLine(string.Format("Manifest SAS: {0}",
                    this._containerInstance.GetManifestContainer().FullAccessUri));
                if (this.IsOperationCancelled)
                {
                    this._status.AppendLine();
                    this._status.AppendLine("Operation Cancelled!");
                }

                response.Details = this._status.ToString();
            }

            return response;
        }

        private void UploadFile(object objArray)
        {
            object[] objArray1 = objArray as object[];
            if (objArray1 != null)
            {
                FileBlob fileBlob = (FileBlob)objArray1[0];
                string str = (string)objArray1[2];
                object obj = objArray1[3];
                List<FileBlob> fileBlobs = (List<FileBlob>)objArray1[4];
                object obj1 = objArray1[5];
                ManualResetEvent manualResetEvent = (ManualResetEvent)objArray1[6];
                bool flag = (bool)objArray1[7];
                byte[] numArray = (byte[])objArray1[8];
                UploadResponse uploadResponse =
                    this._azureBlobStorageManager.UploadBlob(str, fileBlob.FileName, fileBlob.FullPath, numArray);
                if (!uploadResponse.Success)
                {
                    fileBlob.Status = uploadResponse;
                    lock (obj)
                    {
                        fileBlobs.Add(fileBlob);
                    }
                }
                else if (flag && !fileBlob.FullPath.Equals(this._dummyDatFileFullPath,
                             StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(fileBlob.FullPath);
                }

                lock (obj1)
                {
                    manualResetEvent.Set();
                }
            }
        }

        private void UploadFiles(IEnumerable<FileBlob> files, int totalFileCount, string containerUri,
            object failedBlobLockObj, List<FileBlob> failedBlobUploads, bool removeAfterUpload, byte[] encryptionKey)
        {
            if (totalFileCount == 0)
            {
                return;
            }

            bool flag = false;
            int num = 0;
            int num1 = (totalFileCount < 4 ? totalFileCount : 4);
            int num2 = 0;
            int num3 = 0;
            object obj = new object();
            ManualResetEvent[] manualResetEvent = new ManualResetEvent[num1];
            for (int i = 0; i < num1; i++)
            {
                manualResetEvent[i] = new ManualResetEvent(false);
            }

            foreach (FileBlob file in files)
            {
                if (!this._operationState.IsOperationCancelled)
                {
                    WaitCallback waitCallback = new WaitCallback(this.UploadFile);
                    object[] objArray = new object[]
                    {
                        file, num2, containerUri, failedBlobLockObj, failedBlobUploads, obj, manualResetEvent[num3],
                        removeAfterUpload, encryptionKey
                    };
                    ThreadPool.QueueUserWorkItem(waitCallback, objArray);
                    num++;
                    num2++;
                    if (num2 != num1)
                    {
                        num3++;
                    }
                    else
                    {
                        num3 = WaitHandle.WaitAny(manualResetEvent);
                        num2--;
                        if (num >= totalFileCount)
                        {
                            continue;
                        }

                        lock (obj)
                        {
                            manualResetEvent[num3].Reset();
                        }
                    }
                }
                else
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                WaitHandle.WaitAll(manualResetEvent);
            }
        }
    }
}