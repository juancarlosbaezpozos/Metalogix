using Metalogix.Actions;
using Metalogix.Azure;
using Metalogix.Azure.Blob.Manager;
using Metalogix.Core;
using Metalogix.Core.OperationLog;
using Metalogix.Licensing;
using Metalogix.Office365;
using Metalogix.Office365.RoleMappings;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration.Pipeline;
using Metalogix.SharePoint.Actions.Migration.Pipeline.Events;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	public class AzureUploadManager : IOperationLogging, IUploadManager, ICommonGlobalManifestOperations, IUploadManagerStatusLog
	{
		private const int MaxBatchesToUploadDefault = 2;

		private const int MaxBatchesToUploadMinimum = 1;

		private const int BatchThresholdNoOfItemsDefault = 100;

		private const int BatchThresholdNoOfItemsMinimum = 25;

		private const long DefaultListItemSizeInBytes = 5000L;

		private const int BatchThresholdInMegaBytesDefault = 50;

		private const int BatchThresholdInMegaBytesMinimum = 1;

		private const int OneMegaByteInBytes = 1048576;

		private const string DocumentBinariesFolder = "Blobs";

		private const int ThreadWait5Seconds = 5000;

		private const int ThreadWait2Seconds = 2000;

		private const int DefaultWaitTimeInMsPolling5Seconds = 5000;

		private const int DefaultWaitTimeInMsPolling15Seconds = 15000;

		private const int RequestMigrationJobMaxRetryCount = 23040;

		private const int MaxRetryCountForPollingQueue5Seconds = 23040;

		private const int MaxRetryCountForPollingQueue15Seconds = 7680;

		private const int MaxThresholdCountForResubmissionDefault = 960;

		private const int MaxThresholdCountForResubmissionMinimum = 1;

		private const string PreviousJobMessage = "[Previous Job]";

		private bool _encryptAzureMigrationJobs;

		private Guid _uploadSessionId;

		private readonly StringBuilder _statusLog = new StringBuilder();

		private readonly object _statusLogLock = new object();

		private Metalogix.SharePoint.Actions.Migration.UploadManagerState _uploadManagerState;

		private long _currentlyRunningBatchUploadJobs;

		private int _totalBatchesGenerated;

		private volatile int _totalBatchesUploaded;

		private int _maxBatchesToUpload = 3;

		private long _threadPoolTaskCount;

		private int _batchThresholdNoOfItems;

		private int _batchThresholdInMegaBytes;

		private DateTime _startTime;

		private DateTime _endTime;

		private string _commonBlobContainerName;

		private string _tempBaseStorageDirectoryPath;

		private DirectoryInfo _baseTargetDirectory;

		private DirectoryInfo _documentBinariesDirectory;

		private Guid _siteId;

		private Guid _webId;

		private Guid _listId;

		private Guid _rootWebFolderId;

		private Guid _listFolderId;

		private Guid _attachmentsFolderId;

		private string _parentWebAbsoluteUrl;

		private string _parentWebServerRelativeUrl;

		private string _targetBasePath;

		private string _listName;

		private string _listTitle;

		private int _fileNumber;

		private long _totalBytes;

		private int _maxThresholdCountForResubmission;

		private readonly Dictionary<string, string> _folders = new Dictionary<string, string>();

		private readonly object _folderLockObj = new object();

		private readonly Queue<BaseManifestItem> _manifestItems = new Queue<BaseManifestItem>();

		private readonly object _manifestItemLockObj = new object();

		private readonly Queue<BaseManifestItem> _batchManifestItems = new Queue<BaseManifestItem>();

		private readonly object _saveDocumentLock = new object();

		private volatile bool _processing;

		private volatile bool _cancelRequested;

		private IMigrationPipeline _migrationPipeline;

		private IOperationLoggingManagement _operationLoggingManagement;

		private IAzureBlobStorageManager _azureBlobStorageManager;

		private IAzureContainerFactory _containerFactory;

		private readonly Queue<BatchManifestJob> _batchJobQueue = new Queue<BatchManifestJob>();

		private readonly Queue<int> _batchesStarted = new Queue<int>();

		private readonly object _batchesStartedLock = new object();

		private volatile QueueState _manifestItemsProcessState;

		private Thread _manifestItemsProcessingThread;

		private readonly ManualResetEvent _manifestItemsProcessWait = new ManualResetEvent(false);

		private readonly object _manifestItemsProcessWaitLock = new object();

		private volatile QueueState _requestMigrationJobState;

		private Thread _requestMigrationJobThread;

		private readonly ManualResetEvent _requestMigrationJobWait = new ManualResetEvent(false);

		private readonly object _requestMigrationJobWaitLock = new object();

		private readonly object _requestMigrationJobsLock = new object();

		private readonly Dictionary<int, RequestMigrationJob> _requestMigrationJobs = new Dictionary<int, RequestMigrationJob>();

		private readonly Dictionary<int, AzureMigrationHelper> _migrationHelpers = new Dictionary<int, AzureMigrationHelper>();

		private readonly object _uploadManagerStateLock = new object();

		private readonly object _manifestItemsProcessStateLock = new object();

		private readonly object _requestMigrationJobStateLock = new object();

		private List<Field> _fieldNames;

		private readonly object _dependencyFolderLock = new object();

		private Dictionary<string, DependencyFolder> _dependencyFolders = new Dictionary<string, DependencyFolder>();

		private int _userId;

		private Dictionary<int, ManifestUser> _users = new Dictionary<int, ManifestUser>();

		private Dictionary<string, int> _usernameIdCache = new Dictionary<string, int>();

		private readonly object _usersLock = new object();

		private Dictionary<int, ManifestGroup> _groups = new Dictionary<int, ManifestGroup>();

		private Dictionary<string, int> _groupnameIdCache = new Dictionary<string, int>();

		private readonly object _rolesLock = new object();

		private Dictionary<int, ManifestRole> _roles = new Dictionary<int, ManifestRole>();

		private IOperationState _operationState;

		private DateTime _targetFolderCreated;

		private DateTime _targetFolderLastModified;

		private BatchSizeMode _batchSizeMode;

		private string _listBaseTemplate;

		private string _listBaseType;

		private string _webTemplate;

		private InternalTestingMode _testMode;

		private int _itemId = 1;

		public int BatchThresholdInMegaBytes
		{
			get
			{
				return this._batchThresholdInMegaBytes;
			}
			private set
			{
				this._batchThresholdInMegaBytes = (value < 1 ? 1 : value);
			}
		}

		public int BatchThresholdNoOfItems
		{
			get
			{
				return this._batchThresholdNoOfItems;
			}
			private set
			{
				this._batchThresholdNoOfItems = (value < 25 ? 25 : value);
			}
		}

		public List<Field> FieldNames
		{
			get
			{
				return this._fieldNames;
			}
		}

		public ManifestList ListManifest
		{
			get;
			set;
		}

		public int ManifestItemCount
		{
			get
			{
				int count;
				lock (this._manifestItemLockObj)
				{
					count = this._manifestItems.Count;
				}
				return count;
			}
		}

		public QueueState ManifestItemsProcessState
		{
			get
			{
				QueueState queueState;
				lock (this._manifestItemsProcessStateLock)
				{
					queueState = this._manifestItemsProcessState;
				}
				return queueState;
			}
			set
			{
				lock (this._manifestItemsProcessStateLock)
				{
					this._manifestItemsProcessState = value;
					this.LogStatusLog(string.Format("ManifestItemsProcessState changed to {0}", (QueueState)this._manifestItemsProcessState));
				}
			}
		}

		public int MaxBatchesToUpload
		{
			get
			{
				return this._maxBatchesToUpload;
			}
			private set
			{
				this._maxBatchesToUpload = (value < 1 ? 2 : value);
			}
		}

		public int MaxThresholdCountForResubmission
		{
			get
			{
				return this._maxThresholdCountForResubmission;
			}
			private set
			{
				this._maxThresholdCountForResubmission = (value < 1 ? 960 : value);
			}
		}

		public QueueState RequestMigrationJobState
		{
			get
			{
				QueueState queueState;
				lock (this._requestMigrationJobStateLock)
				{
					queueState = this._requestMigrationJobState;
				}
				return queueState;
			}
			set
			{
				lock (this._requestMigrationJobStateLock)
				{
					this._requestMigrationJobState = value;
					this.LogStatusLog(string.Format("RequestMigrationJobState changed to {0}", (QueueState)this._requestMigrationJobState));
				}
			}
		}

		public Metalogix.SharePoint.Actions.Migration.UploadManagerState UploadManagerState
		{
			get
			{
				Metalogix.SharePoint.Actions.Migration.UploadManagerState uploadManagerState;
				lock (this._uploadManagerStateLock)
				{
					uploadManagerState = this._uploadManagerState;
				}
				return uploadManagerState;
			}
			set
			{
				lock (this._uploadManagerStateLock)
				{
					this._uploadManagerState = value;
					this.LogStatusLog(string.Format("UploadManagerState changed to {0}", this._uploadManagerState));
				}
			}
		}

		public AzureUploadManager(bool allowEncryption)
		{
			this._encryptAzureMigrationJobs = allowEncryption;
		}

		public void AddAlertToManifest(ManifestAlert alert)
		{
			this.EnqueueManifestItem(alert);
		}

		public void AddDependencyFolder(string folderItemId, string parentFolderId, string folderXml)
		{
			if (string.IsNullOrEmpty(folderItemId))
			{
				throw new ArgumentException("value cannot be empty or null", "folderItemId");
			}
			if (string.IsNullOrEmpty(parentFolderId))
			{
				throw new ArgumentException("value cannot be empty or null", "parentFolderId");
			}
			if (string.IsNullOrEmpty(folderXml))
			{
				throw new ArgumentException("value cannot be empty or null", "folderXml");
			}
			lock (this._dependencyFolderLock)
			{
				if (this._dependencyFolders.ContainsKey(folderItemId))
				{
					throw new ArgumentException(string.Format("'{0}' folderItemId already has already been added to the _dependencyFolders cache", folderItemId));
				}
				this._dependencyFolders.Add(folderItemId, new DependencyFolder(parentFolderId, folderXml));
			}
		}

		public void AddDiscussionItemToManifest(ManifestDiscussionItem manifestDiscussionItem)
		{
			this.AddParentFolderToManifest(manifestDiscussionItem);
			if (manifestDiscussionItem.HasVersioning)
			{
				foreach (ManifestListItem version in manifestDiscussionItem.Versions)
				{
					version.ParentFolderId = manifestDiscussionItem.ParentFolderId;
				}
			}
			this.EnqueueManifestItem(manifestDiscussionItem);
		}

		public void AddFileToManifest(ManifestFileItem manifestFileItem)
		{
			this.AddParentFolderToManifest(manifestFileItem);
			if (manifestFileItem.HasVersioning)
			{
				foreach (ManifestFileItem version in manifestFileItem.Versions)
				{
					version.ParentFolderId = manifestFileItem.ParentFolderId;
				}
			}
			this.EnqueueManifestItem(manifestFileItem);
		}

		public void AddFolderToManifest(ManifestFolderItem manifestFolderItem)
		{
			this.AddParentFolderToManifest(manifestFolderItem);
			string str = string.Concat(manifestFolderItem.TargetParentPath, "/", manifestFolderItem.Foldername);
			if (!this._folders.ContainsKey(str))
			{
				this._folders.Add(str, manifestFolderItem.ItemGuid.ToString());
			}
			if (manifestFolderItem.HasVersioning)
			{
				foreach (ManifestFolderItem version in manifestFolderItem.Versions)
				{
					version.ParentFolderId = manifestFolderItem.ParentFolderId;
				}
			}
			this.EnqueueManifestItem(manifestFolderItem);
		}

		public int AddGroup(ManifestGroup group)
		{
			int item = 0;
			lock (this._usersLock)
			{
				string lower = group.Name.ToLower();
				if (!this._groupnameIdCache.ContainsKey(lower))
				{
					this._userId++;
					group.GroupId = this._userId;
					this._groups.Add(this._userId, group);
					this._groupnameIdCache.Add(group.Name.ToLower(), this._userId);
					item = this._userId;
				}
				else
				{
					item = this._groupnameIdCache[lower];
				}
			}
			return item;
		}

		public void AddListItemToManifest(ManifestListItem manifestListItem)
		{
			this.AddParentFolderToManifest(manifestListItem);
			if (manifestListItem.HasVersioning)
			{
				foreach (ManifestListItem version in manifestListItem.Versions)
				{
					version.ParentFolderId = manifestListItem.ParentFolderId;
				}
			}
			this.EnqueueManifestItem(manifestListItem);
		}

		private void AddParentFolderToManifest(BaseManifestItem manifestItem)
		{
			string item;
			lock (this._folderLockObj)
			{
				if (!this._folders.ContainsKey(manifestItem.TargetParentPath))
				{
					this._folders.Add(manifestItem.TargetParentPath, Guid.NewGuid().ToString());
				}
				item = this._folders[manifestItem.TargetParentPath];
			}
			manifestItem.ParentFolderId = item;
		}

		public int AddRole(ManifestRole role)
		{
			int roleId = 0;
			lock (this._rolesLock)
			{
				if (!this._roles.ContainsKey(role.RoleId))
				{
					this._roles.Add(role.RoleId, role);
					roleId = role.RoleId;
				}
				else
				{
					roleId = role.RoleId;
				}
			}
			return roleId;
		}

		public int AddUser(ManifestUser user)
		{
			int item = 0;
			lock (this._usersLock)
			{
				string lower = user.Login.ToLower();
				if (!this._usernameIdCache.ContainsKey(lower))
				{
					this._userId++;
					user.UserId = this._userId;
					this._users.Add(this._userId, user);
					this._usernameIdCache.Add(user.Login.ToLower(), this._userId);
					item = this._userId;
				}
				else
				{
					item = this._usernameIdCache[lower];
				}
			}
			return item;
		}

		private void BatchManifestJobOnCompleted(object sender, BatchManifestJobOnCompletedEventArgs eventArgs)
		{
			Interlocked.Decrement(ref this._currentlyRunningBatchUploadJobs);
			this.LogStatusLog(string.Format("BatchManifestJobOnCompleted event -> entered for Batch {0}", eventArgs.BatchNo));
			lock (this._requestMigrationJobsLock)
			{
				if (!eventArgs.OperationSuccessful)
				{
					this._requestMigrationJobs[eventArgs.BatchNo].JobState = AzureToO365MigrationJobState.Error;
					if (eventArgs.WorkloadException != null)
					{
						Metalogix.Utilities.ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(eventArgs.WorkloadException);
						this._requestMigrationJobs[eventArgs.BatchNo].ExceptionDetail = exceptionMessageAndDetail;
						this.LogStatusLog(string.Format("BatchManifestJobOnCompleted Exception:{0}{1}", Environment.NewLine, exceptionMessageAndDetail.Detail));
					}
				}
				else
				{
					this._requestMigrationJobs[eventArgs.BatchNo].JobConfigurationXml = eventArgs.JobConfiguration;
					this._requestMigrationJobs[eventArgs.BatchNo].Uploaded = true;
					this._requestMigrationJobs[eventArgs.BatchNo].JobState = AzureToO365MigrationJobState.UploadedToAzure;
					lock (this._requestMigrationJobWaitLock)
					{
						this._requestMigrationJobWait.Set();
					}
				}
				this._totalBatchesUploaded++;
				this.LogStatusLog(string.Format("BatchManifestJobOnCompleted event -> set Batch {0} JobState to {1}", eventArgs.BatchNo, this._requestMigrationJobs[eventArgs.BatchNo].JobState));
			}
		}

		public void Cancel()
		{
			this.LogStatusLog("Cancel");
			this.UploadManagerState = Metalogix.SharePoint.Actions.Migration.UploadManagerState.CancelRequested;
			this.ManifestItemsProcessState = QueueState.CancelRequested;
			this.RequestMigrationJobState = QueueState.CancelRequested;
			this._cancelRequested = true;
		}

		private static string ConvertTemplateType(string listBaseTemplate)
		{
			string str = listBaseTemplate;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "CustomList")
				{
					return "GenericList";
				}
				if (str1 == "Issues")
				{
					return "IssueTracking";
				}
				if (str1 == "ProjectTasks")
				{
					return "GanttTasks";
				}
			}
			return listBaseTemplate;
		}

		private void CreateAllBatchesUploadedLogItem(object state)
		{
			Interlocked.Increment(ref this._threadPoolTaskCount);
			try
			{
				bool isOperationCancelled = this._operationState.IsOperationCancelled;
				while (!isOperationCancelled)
				{
					Thread.Sleep(5000);
					isOperationCancelled = this._operationState.IsOperationCancelled;
					if (isOperationCancelled || this._totalBatchesUploaded != this._totalBatchesGenerated)
					{
						continue;
					}
					LogItem logItem = new LogItem("All Batches Uploaded", this._listTitle, string.Empty, string.Empty, ActionOperationStatus.Running);
					int num = 0;
					int num1 = 0;
					lock (this._requestMigrationJobsLock)
					{
						foreach (KeyValuePair<int, RequestMigrationJob> _requestMigrationJob in this._requestMigrationJobs)
						{
							if (!_requestMigrationJob.Value.Uploaded)
							{
								num1++;
							}
							else
							{
								num++;
							}
						}
					}
					logItem.Information = string.Format("{0} Successful, {1} Failed [Total {2} generated]", num, num1, this._totalBatchesGenerated);
					this.LogStatusLog(string.Concat("CreateAllBatchesUploadedLogItem -> ", logItem.Information));
					logItem.Status = (num1 == 0 ? ActionOperationStatus.Completed : ActionOperationStatus.Failed);
					this.FireOperationStarted(logItem);
					this.FireOperationFinished(logItem);
					break;
				}
			}
			finally
			{
				Interlocked.Decrement(ref this._threadPoolTaskCount);
			}
		}

		private string DecryptContent(string encryptedContent, string iv)
		{
			string end;
			Exception exception = null;
			HashSet<string> strs = new HashSet<string>();
			Dictionary<int, RequestMigrationJob>.ValueCollection.Enumerator enumerator = this._requestMigrationJobs.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RequestMigrationJob current = enumerator.Current;
					try
					{
						byte[] encryptionKey = current.Container.EncryptionKey;
						string base64String = Convert.ToBase64String(encryptionKey);
						if (!strs.Contains(base64String))
						{
							strs.Add(base64String);
							RijndaelManaged rijndaelManaged = new RijndaelManaged()
							{
								Key = encryptionKey,
								IV = Convert.FromBase64String(iv)
							};
							RijndaelManaged rijndaelManaged1 = rijndaelManaged;
							using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(encryptedContent)))
							{
								using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged1.CreateDecryptor(), CryptoStreamMode.Read))
								{
									using (StreamReader streamReader = new StreamReader(cryptoStream))
									{
										end = streamReader.ReadToEnd();
										return end;
									}
								}
							}
						}
					}
					catch (CryptographicException cryptographicException)
					{
						exception = cryptographicException;
					}
				}
				throw new Exception(string.Concat("Failed to decrypt message '", encryptedContent, "'."), exception);
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return end;
		}

		private string DecryptMessage(Dictionary<string, string> encMsgProps)
		{
			string str;
			string str1;
			if (!encMsgProps.TryGetValue("IV", out str))
			{
				throw new Exception("IV is not found in the message properties which is required to decrypt the message content. Hence the data cannot be decrypted");
			}
			if (!encMsgProps.TryGetValue("Content", out str1))
			{
				throw new Exception("Content is not in message properties so there is nothing to decrypt.");
			}
			return this.DecryptContent(str1, str);
		}

		private AzureMigrationHelper DeleteDummyFile()
		{
			AzureMigrationHelper azureMigrationHelper;
			Dictionary<int, AzureMigrationHelper>.Enumerator enumerator = this._migrationHelpers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, AzureMigrationHelper> current = enumerator.Current;
					try
					{
						AzureMigrationHelper value = current.Value;
						value.DeleteDummyDatFile();
						azureMigrationHelper = value;
						return azureMigrationHelper;
					}
					catch
					{
					}
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return azureMigrationHelper;
		}

		private bool DeleteMigrationJob(RequestMigrationJob job, bool withRetry)
		{
			bool errorOccured = false;
			int num = 0;
			bool isOperationCancelled = this._operationState.IsOperationCancelled;
			while (!isOperationCancelled)
			{
				num++;
				this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - DeleteMigrationJob for Batch {0} [withRetry={1}]", job.BatchNo, withRetry));
				LogItem logItem = new LogItem("Delete Migration Job", string.Format("Batch {0}", job.BatchNo), "", "", ActionOperationStatus.Running);
				this.FireOperationStarted(logItem);
				try
				{
					try
					{
						string deleteMigrationJobConfigurationXml = job.DeleteMigrationJobConfigurationXml;
						OperationReportingResult operationReportingResult = new OperationReportingResult(this._migrationPipeline.DeleteMigrationJob(deleteMigrationJobConfigurationXml));
						if (!withRetry)
						{
							logItem.Information = (operationReportingResult.ErrorOccured ? "Errors, see details" : string.Empty);
						}
						else
						{
							logItem.Information = string.Format("Attempt {0}/{1}{2}", num, 23040, (operationReportingResult.ErrorOccured ? " Errors, see details" : string.Empty));
						}
						LogItem logItem1 = logItem;
						object[] newLine = new object[] { deleteMigrationJobConfigurationXml, Environment.NewLine, null, null, null };
						newLine[2] = operationReportingResult.ObjectXml ?? string.Empty;
						newLine[3] = Environment.NewLine;
						newLine[4] = operationReportingResult.AllReportElementsAsString;
						logItem1.Details = string.Format("Request:{0}{1}Result:{2}{3}{4}", newLine);
						logItem.Status = (operationReportingResult.ErrorOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						errorOccured = !operationReportingResult.ErrorOccured;
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
					}
				}
				finally
				{
					this.FireOperationFinished(logItem);
				}
				isOperationCancelled = this._operationState.IsOperationCancelled;
				if (!withRetry || errorOccured || isOperationCancelled || num == 23040)
				{
					break;
				}
				Thread.Sleep(5000);
			}
			return errorOccured;
		}

		private static TResponseType DeserializeJson<TResponseType>(string json)
		{
			return (new JavaScriptSerializer()).Deserialize<TResponseType>(json);
		}

		private void DownloadMigrationLogFiles(RequestMigrationJob job)
		{
			this.LogStatusLog(string.Format("DownloadMigrationLogFiles - Getting import log for Batch {0}", job.BatchNo));
			LogItem logItem = new LogItem("SPO Migration Import Log", string.Format("Batch {0}", job.BatchNo), "", "", ActionOperationStatus.Running);
			this.FireOperationStarted(logItem);
			bool flag = false;
			try
			{
				try
				{
					AzureMigrationHelper item = this._migrationHelpers[job.BatchNo];
					DownloadResponse importLog = item.GetImportLog(job.MigrationJobId);
					logItem.Information = "Please see details...";
					logItem.Details = importLog.Details;
					logItem.Status = (importLog.Success ? ActionOperationStatus.Completed : ActionOperationStatus.Warning);
					if (importLog.Details.Contains("Import did not complete") || importLog.Details.Contains("Unable to download"))
					{
						logItem.Status = ActionOperationStatus.Failed;
					}
					job.ImportLogContents = importLog.Details;
					job.ImportLogDownloaded = importLog.Success;
					if (Regex.IsMatch(importLog.Details, "Finished with [1-9]+ warnings"))
					{
						DownloadResponse warningLog = item.GetWarningLog(job.MigrationJobId);
						LogItem details = new LogItem("SPO Migration Warning Log", string.Format("Batch {0}", job.BatchNo), "", "", ActionOperationStatus.Running);
						this.FireOperationStarted(details);
						details.Details = warningLog.Details;
						details.Status = (warningLog.Success ? ActionOperationStatus.Completed : ActionOperationStatus.Warning);
						this.FireOperationFinished(details);
						this.LogStatusLog(string.Format("DownloadMigrationLogFiles - Warning log for Batch {0} {1}", job.BatchNo, details.Status.ToString().ToUpper()));
					}
					if (Regex.IsMatch(importLog.Details, "Finished with [1-9]+ errors"))
					{
						flag = true;
						DownloadResponse errorLog = item.GetErrorLog(job.MigrationJobId);
						LogItem logItem1 = new LogItem("SPO Migration Error Log", string.Format("Batch {0}", job.BatchNo), "", "", ActionOperationStatus.Running);
						this.FireOperationStarted(logItem1);
						logItem1.Details = errorLog.Details;
						logItem1.Status = (errorLog.Success ? ActionOperationStatus.Completed : ActionOperationStatus.Warning);
						this.FireOperationFinished(logItem1);
						this.LogStatusLog(string.Format("DownloadMigrationLogFiles - Error log for Batch {0} {1}", job.BatchNo, logItem1.Status.ToString().ToUpper()));
					}
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
			}
			finally
			{
				if (flag)
				{
					logItem.Status = ActionOperationStatus.Failed;
				}
				this.LogStatusLog(string.Format("DownloadMigrationLogFiles - Exiting for Batch {0}", job.BatchNo));
				this.FireOperationFinished(logItem);
			}
		}

		public void EndProcessing()
		{
			if (this.UploadManagerState != Metalogix.SharePoint.Actions.Migration.UploadManagerState.Processing)
			{
				return;
			}
			this.LogStatusLog("EndProcessing");
			this.UploadManagerState = Metalogix.SharePoint.Actions.Migration.UploadManagerState.EndProcessingCalled;
			this._processing = false;
			lock (this._manifestItemsProcessWaitLock)
			{
				this._manifestItemsProcessWait.Set();
			}
		}

		private void EnqueueManifestItem(BaseManifestItem manifestItem)
		{
			int count;
			lock (this._manifestItemLockObj)
			{
				this._manifestItems.Enqueue(manifestItem);
				count = this._manifestItems.Count;
			}
			if (count >= this.BatchThresholdNoOfItems)
			{
				lock (this._manifestItemsProcessWaitLock)
				{
					this._manifestItemsProcessWait.Set();
				}
			}
		}

		public void FireOperationFinished(LogItem operation)
		{
			if (this.OperationFinished != null)
			{
				this.OperationFinished(operation);
			}
		}

		public void FireOperationStarted(LogItem operation)
		{
			if (this.OperationStarted != null)
			{
				this.OperationStarted(operation);
			}
		}

		public void FireOperationUpdated(LogItem operation)
		{
			if (this.OperationUpdated != null)
			{
				this.OperationUpdated(operation);
			}
		}

		public string GetAllGroupsXml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				OmitXmlDeclaration = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
			{
				xmlWriter.WriteStartElement("Groups");
				lock (this._usersLock)
				{
					foreach (KeyValuePair<int, ManifestGroup> _group in this._groups)
					{
						xmlWriter.WriteStartElement("Group");
						xmlWriter.WriteAttributeString("Id", _group.Value.GroupId.ToString());
						xmlWriter.WriteAttributeString("Name", _group.Value.Name);
						xmlWriter.WriteAttributeString("Description", _group.Value.Description);
						xmlWriter.WriteAttributeString("Owner", _group.Value.Owner.ToString());
						bool ownerIsUser = _group.Value.OwnerIsUser;
						xmlWriter.WriteAttributeString("OwnerIsUser", ownerIsUser.ToString().ToLower());
						bool onlyAllowMembersViewMembership = _group.Value.OnlyAllowMembersViewMembership;
						xmlWriter.WriteAttributeString("OnlyAllowMembersViewMembership", onlyAllowMembersViewMembership.ToString().ToLower());
						xmlWriter.WriteAttributeString("RequestToJoinLeaveEmailSetting", _group.Value.RequestToJoinLeaveEmailSetting);
						foreach (ManifestGroupMember groupMember in _group.Value.GroupMembers)
						{
							xmlWriter.WriteStartElement("Member");
							xmlWriter.WriteAttributeString("UserId", groupMember.UserId.ToString());
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			return stringBuilder.ToString();
		}

		public string GetAllRolesXml(string webTemplate)
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				OmitXmlDeclaration = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
			{
				xmlWriter.WriteStartElement("Roles");
				lock (this._rolesLock)
				{
					foreach (KeyValuePair<int, ManifestRole> _role in this._roles)
					{
						xmlWriter.WriteStartElement("Role");
						xmlWriter.WriteAttributeString("RoleId", _role.Value.RoleId.ToString());
						int roleId = _role.Value.RoleId;
						RoleTemplateMapping targetRole = RoleMappingFile.GetTargetRole(roleId.ToString(), _role.Value.Title, webTemplate);
						if (targetRole == null)
						{
							xmlWriter.WriteAttributeString("Title", _role.Value.Title);
							xmlWriter.WriteAttributeString("Description", _role.Value.Description);
						}
						else
						{
							xmlWriter.WriteAttributeString("Title", targetRole.Title);
							xmlWriter.WriteAttributeString("Description", targetRole.Description);
						}
						xmlWriter.WriteAttributeString("PermMask", _role.Value.PermMask.ToString());
						bool isHidden = _role.Value.IsHidden;
						xmlWriter.WriteAttributeString("Hidden", isHidden.ToString().ToLower());
						if (string.IsNullOrEmpty(_role.Value.RoleOrder) || string.IsNullOrEmpty(_role.Value.RoleType))
						{
							int num = _role.Value.RoleId;
							RoleMapping role = RoleMappingFile.GetRole(num.ToString());
							xmlWriter.WriteAttributeString("RoleOrder", role.RoleOrder);
							xmlWriter.WriteAttributeString("Type", role.RoleType);
						}
						else
						{
							xmlWriter.WriteAttributeString("RoleOrder", _role.Value.RoleOrder);
							xmlWriter.WriteAttributeString("Type", _role.Value.RoleType);
						}
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			return stringBuilder.ToString();
		}

		public string GetAllUsersXml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				OmitXmlDeclaration = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
			{
				xmlWriter.WriteStartElement("Users");
				lock (this._usersLock)
				{
					foreach (KeyValuePair<int, ManifestUser> _user in this._users)
					{
						xmlWriter.WriteStartElement("User");
						xmlWriter.WriteAttributeString("Id", _user.Value.UserId.ToString());
						xmlWriter.WriteAttributeString("Name", _user.Value.Name);
						xmlWriter.WriteAttributeString("Login", _user.Value.Login);
						xmlWriter.WriteAttributeString("Email", _user.Value.Email);
						bool isDomainGroup = _user.Value.IsDomainGroup;
						xmlWriter.WriteAttributeString("IsDomainGroup", isDomainGroup.ToString().ToLower());
						bool isSiteAdmin = _user.Value.IsSiteAdmin;
						xmlWriter.WriteAttributeString("IsSiteAdmin", isSiteAdmin.ToString().ToLower());
						xmlWriter.WriteAttributeString("SystemId", _user.Value.SystemId);
						bool isDeleted = _user.Value.IsDeleted;
						xmlWriter.WriteAttributeString("IsDeleted", isDeleted.ToString().ToLower());
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			return stringBuilder.ToString();
		}

		public DependencyFolder GetDependencyFolder(string folderItemId)
		{
			DependencyFolder dependencyFolder;
			DependencyFolder item;
			if (string.IsNullOrEmpty(folderItemId))
			{
				throw new ArgumentException("value cannot be empty or null", "folderItemId");
			}
			lock (this._dependencyFolderLock)
			{
				if (this._dependencyFolders.ContainsKey(folderItemId))
				{
					item = this._dependencyFolders[folderItemId];
				}
				else
				{
					item = null;
				}
				dependencyFolder = item;
			}
			return dependencyFolder;
		}

		public int GetNextItemId()
		{
			return Interlocked.Increment(ref this._itemId);
		}

		public string GetStatusLog()
		{
			string str;
			lock (this._statusLogLock)
			{
				str = this._statusLog.ToString();
			}
			return str;
		}

		public int GetUserOrGroupIDByName(string principalName)
		{
			int item = 0;
			lock (this._usersLock)
			{
				string lower = principalName.ToLower();
				if (this._usernameIdCache.ContainsKey(lower))
				{
					item = this._usernameIdCache[lower];
				}
				else if (this._groupnameIdCache.ContainsKey(lower))
				{
					item = this._groupnameIdCache[lower];
				}
			}
			return item;
		}

		private Dictionary<string, string> GetValuesFromJson(string jsonMessage)
		{
			Dictionary<string, string> strs;
			try
			{
				string str = jsonMessage.Replace(",}", "}");
				strs = (new JavaScriptSerializer()).Deserialize<Dictionary<string, string>>(str);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				strs = new Dictionary<string, string>();
				this.LogStatusLog(string.Concat("GetValuesFromJson - Exception: ", exception.Message));
			}
			return strs;
		}

		public void Initialise(Guid uploadSessionId, int maxBatchesToUpload, int batchSizeThresholdNoOfItems, int batchSizeThresholdInMB, BatchSizeMode batchSizeMode, IAzureBlobStorageManager azureBlobStorageManager, IAzureContainerFactory containerFactory, int maxThresholdCountForResubmission, string internalTestingMode, string tempBaseStorageDirectoryPath, Guid webId, Guid siteId, Guid listId, Guid rootWebFolderId, Guid listFolderId, Guid attachmentsFolderId, string parentWebAbsoluteUrl, string parentWebServerRelativeUrl, string targetBasePath, string listName, string listTitle, IMigrationPipeline migrationPipeline, IOperationLoggingManagement operationLoggingManagement, IOperationState operationState, List<Field> fieldNames, DateTime targetFolderCreated, DateTime targetFolderLastModified, string listBaseTemplate, string listBaseType, string webTemplate)
		{
			this.LogStatusLog("Initialise");
			if (!File.Exists(ConsoleUtils.ProcessorPathName))
			{
				throw new FileNotFoundException(string.Format("The required Azure command processor '{0}' does not exist.", ConsoleUtils.ProcessorPathName));
			}
			this._currentlyRunningBatchUploadJobs = (long)0;
			this._uploadSessionId = uploadSessionId;
			this.MaxBatchesToUpload = maxBatchesToUpload;
			this.BatchThresholdNoOfItems = batchSizeThresholdNoOfItems;
			this.BatchThresholdInMegaBytes = batchSizeThresholdInMB;
			this.MaxThresholdCountForResubmission = maxThresholdCountForResubmission;
			this._testMode = internalTestingMode.ToEnumValue<InternalTestingMode>(InternalTestingMode.None);
			this._batchSizeMode = batchSizeMode;
			this._fieldNames = fieldNames;
			this._manifestItemsProcessingThread = null;
			this._migrationPipeline = migrationPipeline;
			this._operationLoggingManagement = operationLoggingManagement;
			this._operationState = operationState;
			this._azureBlobStorageManager = azureBlobStorageManager;
			this._containerFactory = containerFactory;
			this._targetFolderCreated = targetFolderCreated;
			this._targetFolderLastModified = targetFolderLastModified;
			this._commonBlobContainerName = string.Format("{0}-blobs", this._uploadSessionId.ToString("N").ToLower());
			this._listBaseTemplate = AzureUploadManager.ConvertTemplateType(listBaseTemplate);
			this._listBaseType = listBaseType;
			this._webTemplate = webTemplate;
			this._tempBaseStorageDirectoryPath = tempBaseStorageDirectoryPath;
			this._baseTargetDirectory = new DirectoryInfo(tempBaseStorageDirectoryPath);
			string str = Path.Combine(tempBaseStorageDirectoryPath, "Blobs");
			this._documentBinariesDirectory = new DirectoryInfo(str);
			if (!this._baseTargetDirectory.Exists)
			{
				this._baseTargetDirectory.Create();
			}
			if (!this._documentBinariesDirectory.Exists)
			{
				this._documentBinariesDirectory.Create();
			}
			this._webId = webId;
			this._siteId = siteId;
			this._listId = listId;
			this._rootWebFolderId = rootWebFolderId;
			this._listFolderId = listFolderId;
			this._attachmentsFolderId = attachmentsFolderId;
			this._parentWebAbsoluteUrl = parentWebAbsoluteUrl;
			this._parentWebServerRelativeUrl = parentWebServerRelativeUrl;
			this._targetBasePath = targetBasePath;
			this._listName = listName;
			this._listTitle = listTitle;
			this._folders.Add(this._targetBasePath, this._listFolderId.ToString());
			if (string.IsNullOrEmpty(this._parentWebServerRelativeUrl))
			{
				this._parentWebServerRelativeUrl = "/";
			}
			this._operationLoggingManagement.ConnectOperationLogging(this);
			StringBuilder stringBuilder = new StringBuilder();
			if (this._testMode != InternalTestingMode.None)
			{
				stringBuilder.AppendLine(string.Concat("InternalTestingMode = ", this._testMode.ToString()));
			}
			stringBuilder.AppendLine(string.Format("Azure Common Blob Container Name = {0}", this._commonBlobContainerName));
			stringBuilder.AppendLine(string.Format("MaxBatchesToUpload (simultaneously) = {0}", this.MaxBatchesToUpload));
			stringBuilder.AppendLine(string.Format("Batch Size Mode = {0}", this._batchSizeMode.ToString()));
			if (this._batchSizeMode != BatchSizeMode.NumberOfItems)
			{
				stringBuilder.AppendLine(string.Format("BatchSizeThreshold (MB) = {0}", this.BatchThresholdInMegaBytes));
			}
			else
			{
				stringBuilder.AppendLine(string.Format("BatchSizeThreshold (Items) = {0}", this.BatchThresholdNoOfItems));
			}
			stringBuilder.AppendLine(string.Format("Max Retry Count Threshold For Resubmission = {0}", this.MaxThresholdCountForResubmission));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("Local Temporary Storage Location = {0}", this._tempBaseStorageDirectoryPath));
			stringBuilder.AppendLine(string.Format("Local Document Binaries = {0}", str));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("WebId = {0}", this._webId));
			stringBuilder.AppendLine(string.Format("SiteId = {0}", this._siteId));
			stringBuilder.AppendLine(string.Format("ListId = {0}", this._listId));
			stringBuilder.AppendLine(string.Format("RootWebFolderId = {0}", this._rootWebFolderId));
			stringBuilder.AppendLine(string.Format("DocumentLibraryFolderId = {0}", this._listFolderId));
			stringBuilder.AppendLine(string.Format("ParentWebAbsoluteUrl = {0}", this._parentWebAbsoluteUrl));
			stringBuilder.AppendLine(string.Format("ParentWebServerRelativeUrl = {0}", this._parentWebServerRelativeUrl));
			stringBuilder.AppendLine(string.Format("TargetBasePath = {0}", this._targetBasePath));
			stringBuilder.AppendLine(string.Format("DocumentLibraryTitle = {0}", this._listTitle));
			this.InitializeBlobStorageEncryption(azureBlobStorageManager, stringBuilder);
			this.LogSpecificItem("AzureUploadManager.Initialise", this._listTitle, string.Empty, string.Empty, (this._testMode != InternalTestingMode.None ? "Internal Testing Mode active" : "Please see details for parameters"), stringBuilder.ToString(), ActionOperationStatus.Completed, (long)0);
		}

		private void InitializeBlobStorageEncryption(IAzureBlobStorageManager storageManager, StringBuilder log)
		{
			if (!this._encryptAzureMigrationJobs)
			{
				log.AppendLine(string.Format("EncryptAzureMigrationJobs = {0}", "False"));
				return;
			}
			log.AppendLine(string.Format("EncryptAzureMigrationJobs = {0}", "True"));
			storageManager.IsEncryptionUsed = this._encryptAzureMigrationJobs;
		}

		private static bool IsEncryptedMessage(Dictionary<string, string> msgProps)
		{
			string str;
			if (!msgProps.TryGetValue("Label", out str))
			{
				return false;
			}
			return string.Equals("Encrypted", str, StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsMicrosoftCustomer()
		{
			bool flag;
			bool flag1;
			try
			{
				MLLicense license = MLLicenseProvider.Instance.GetLicense(null, null, null, false) as MLLicense;
				if (license == null)
				{
					flag1 = false;
				}
				else
				{
					flag1 = (license.Organization.IndexOf("Microsoft", StringComparison.InvariantCultureIgnoreCase) > -1 ? true : license.Organization.IndexOf("FastTrack", StringComparison.InvariantCultureIgnoreCase) > -1);
				}
				flag = flag1;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		private void KeepAzureContainer(LogItem logItem, string reason)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(reason);
			stringBuilder.AppendFormat("Blob container name: {0}", this._commonBlobContainerName);
			stringBuilder.AppendLine().AppendLine();
			foreach (RequestMigrationJob value in this._requestMigrationJobs.Values)
			{
				if (string.IsNullOrEmpty(value.JobConfigurationXml))
				{
					stringBuilder.AppendLine(string.Format("Error occurred for batch {0}, hence not able to log message for the same", value.BatchNo));
				}
				else
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(value.JobConfigurationXml);
					foreach (XmlAttribute attribute in xmlDocument.DocumentElement.Attributes)
					{
						stringBuilder.AppendLine(string.Concat(attribute.Name, ": ", attribute.Value));
					}
					stringBuilder.AppendLine();
				}
			}
			logItem.Details = stringBuilder.ToString();
			logItem.Status = ActionOperationStatus.Skipped;
			this.FireOperationFinished(logItem);
		}

		private void LogSpecificItem(string operationName, string itemName, string sourceName, string targetName, string information, string details, ActionOperationStatus status, long totalBytes = 0L)
		{
			LogItem logItem = new LogItem(operationName, itemName, sourceName, targetName, ActionOperationStatus.Running);
			this.FireOperationStarted(logItem);
			logItem.Information = information;
			logItem.Details = details;
			logItem.LicenseDataUsed = totalBytes;
			logItem.Status = status;
			this.FireOperationFinished(logItem);
		}

		public void LogStatusLog(string message)
		{
			lock (this._statusLogLock)
			{
				StringBuilder stringBuilder = this._statusLog;
				DateTime now = DateTime.Now;
				stringBuilder.AppendLine(string.Format("{0}: {1}", now.ToString("yy-MM-dd HH.mm.ss.ffff"), message));
			}
		}

		private void MonitorJob(object requestMigrationJob)
		{
			RequestMigrationJob requestMigrationJob1 = (RequestMigrationJob)requestMigrationJob;
			if (requestMigrationJob1 == null)
			{
				throw new ArgumentNullException("requestMigrationJob", "null, unable to access object as the intended type");
			}
			Interlocked.Increment(ref this._threadPoolTaskCount);
			try
			{
				this.MonitorMigrationStatusForJob(requestMigrationJob1);
				if (!this._operationState.IsOperationCancelled)
				{
					this.DownloadMigrationLogFiles(requestMigrationJob1);
				}
				requestMigrationJob1.Imported = true;
				requestMigrationJob1.JobState = AzureToO365MigrationJobState.Complete;
			}
			finally
			{
				Interlocked.Decrement(ref this._threadPoolTaskCount);
			}
		}

		private void MonitorMigrationStatusForJob(RequestMigrationJob job)
		{
			bool flag;
			string str;
			object obj;
			int num = 5000;
			int num1 = 23040;
			long count = (long)0;
			this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - Getting status from reporting queue for Batch {0}", job.BatchNo));
			bool isOperationCancelled = false;
			bool flag1 = false;
			LogItem logItem = new LogItem("Monitor Migration Status", string.Format("Batch {0}", job.BatchNo), "", "", ActionOperationStatus.Running);
			this.FireOperationStarted(logItem);
			try
			{
				try
				{
					AzureMigrationHelper item = this._migrationHelpers[job.BatchNo];
					isOperationCancelled = this._operationState.IsOperationCancelled;
					int num2 = 0;
					StringBuilder stringBuilder = new StringBuilder();
					bool flag2 = false;
					bool flag3 = false;
					JobEndEvent jobEndEvent = null;
					int num3 = 0;
					string empty = string.Empty;
					bool flag4 = false;
					while (!flag3 && !isOperationCancelled)
					{
						Thread.Sleep(num);
						num2++;
						string fullAccessUri = item.ContainerInstance.GetReportingQueue().FullAccessUri;
						QueueMessagesResponse queueMessages = this._azureBlobStorageManager.GetQueueMessages(fullAccessUri, 32);
						count += (long)queueMessages.Messages.Count;
						this.ProcessReportingQueueMessages(queueMessages, job, stringBuilder, 250, ref num2, ref num3, ref num, ref num1, ref flag2, ref flag4, ref flag3, ref empty, ref jobEndEvent);
						this.ProcessJobEndMessage(queueMessages, job, stringBuilder, empty, ref flag3, ref flag2, jobEndEvent, item.TotalBytesProcessed);
						isOperationCancelled = this._operationState.IsOperationCancelled;
						if (flag3)
						{
							continue;
						}
						if (!flag4 || num2 != this.MaxThresholdCountForResubmission)
						{
							flag = (!flag4 || isOperationCancelled ? false : this._testMode == InternalTestingMode.AzureNoQueueResponseBatchSubmission);
						}
						else
						{
							flag = true;
						}
						bool flag5 = flag;
						if (flag5 && !isOperationCancelled)
						{
							if (this._testMode == InternalTestingMode.AzureNoQueueResponseBatchSubmission)
							{
								this._testMode = InternalTestingMode.None;
							}
							num2 = 0;
							if (!this.ReRequestMigrationJob(job))
							{
								flag1 = true;
								break;
							}
						}
						else if (!flag5 && isOperationCancelled)
						{
							this.DeleteMigrationJob(job, true);
						}
						if (num2 != num1)
						{
							continue;
						}
						flag1 = true;
						break;
					}
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
			}
			finally
			{
				isOperationCancelled = this._operationState.IsOperationCancelled;
				LogItem logItem1 = logItem;
				if (isOperationCancelled)
				{
					str = "Operation Cancelled";
				}
				else
				{
					str = (flag1 ? "Timed Out" : "Please see details...");
				}
				logItem1.Information = str;
				if (logItem.Exception == null)
				{
					logItem.Status = (isOperationCancelled || flag1 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
				}
				else
				{
					logItem.Status = ActionOperationStatus.Failed;
				}
				logItem.Details = string.Format("Total messages processed from reporting queue = {0}", count);
				if (logItem.Exception != null)
				{
					logItem.Details = string.Format("{0}{1}An error occurred while processing messages from the reporting queue.Error: '{2}'", logItem.Details, Environment.NewLine, logItem.Exception.Message);
				}
				object batchNo = job.BatchNo;
				if (isOperationCancelled)
				{
					obj = "Operation Cancelled";
				}
				else
				{
					obj = (flag1 ? "Timed Out" : "Completed");
				}
				this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - End of processing Batch {0} [{1}]", batchNo, obj));
				this.FireOperationFinished(logItem);
			}
		}

		private void ProcessJobEndMessage(QueueMessagesResponse queueMessagesResponse, RequestMigrationJob job, StringBuilder sb, string jobEndJobId, ref bool jobEnd, ref bool errors, JobEndEvent jobEndEvent, ulong totalBytesProcessed)
		{
			ActionOperationStatus actionOperationStatu;
			if (jobEnd)
			{
				bool flag = string.Equals(job.MigrationJobId, jobEndJobId, StringComparison.OrdinalIgnoreCase);
				string str = (flag ? string.Empty : " [Previous Job] ");
				long num = (long)0;
				if (jobEndEvent != null)
				{
					long.TryParse(jobEndEvent.BytesProcessed, out num);
				}
				if (num == (long)0)
				{
					num = (long)totalBytesProcessed;
					this.LogStatusLog("Could not retrieve data migrated from migration pipeline response. Hence using data that we actually processed during migration");
				}
				ReflectionEventPrinter reflectionEventPrinter = new ReflectionEventPrinter();
				string str1 = (jobEndEvent != null ? reflectionEventPrinter.PrintEvent(jobEndEvent) : string.Empty);
				this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - Log Item for JobEnd for Batch {0}, Total Bytes {1}", job.BatchNo, num));
				string str2 = string.Format("SPO Migration Ended{0}", str);
				string str3 = string.Format("Batch {0}", job.BatchNo);
				string str4 = str1;
				if (!queueMessagesResponse.Success || errors)
				{
					actionOperationStatu = (flag ? ActionOperationStatus.Failed : ActionOperationStatus.Warning);
				}
				else
				{
					actionOperationStatu = ActionOperationStatus.Completed;
				}
				this.LogSpecificItem(str2, str3, "", "", "ENDED, see details", str4, actionOperationStatu, num);
				if (!flag)
				{
					jobEnd = false;
					errors = false;
				}
			}
		}

		private void ProcessManifestItemsIntoBatches()
		{
			BaseManifestItem baseManifestItem;
			int num = 0;
			this.ManifestItemsProcessState = QueueState.Processing;
			this._batchManifestItems.Clear();
			long fileSize = (long)0;
			bool flag = false;
			long batchThresholdInMegaBytes = (long)(this.BatchThresholdInMegaBytes * 1048576);
			try
			{
				try
				{
					int num1 = 0;
					while (this.ManifestItemsProcessState == QueueState.Processing)
					{
						this._manifestItemsProcessWait.WaitOne(2000);
						int manifestItemCount = this.ManifestItemCount;
						int batchThresholdNoOfItems = 0;
						switch (this._batchSizeMode)
						{
							case BatchSizeMode.NumberOfItems:
							{
								if (manifestItemCount < this.BatchThresholdNoOfItems)
								{
									if (manifestItemCount <= 0)
									{
										break;
									}
									Metalogix.SharePoint.Actions.Migration.UploadManagerState uploadManagerState = this.UploadManagerState;
									Metalogix.SharePoint.Actions.Migration.UploadManagerState[] uploadManagerStateArray = new Metalogix.SharePoint.Actions.Migration.UploadManagerState[] { Metalogix.SharePoint.Actions.Migration.UploadManagerState.EndProcessingCalled, Metalogix.SharePoint.Actions.Migration.UploadManagerState.WaitingForCompletion };
									if (!uploadManagerState.In<Metalogix.SharePoint.Actions.Migration.UploadManagerState>(uploadManagerStateArray))
									{
										break;
									}
									batchThresholdNoOfItems = manifestItemCount;
									flag = true;
									break;
								}
								else
								{
									batchThresholdNoOfItems = this.BatchThresholdNoOfItems;
									flag = true;
									break;
								}
							}
							case BatchSizeMode.InMegaBytes:
							{
								if (manifestItemCount <= 0)
								{
									break;
								}
								batchThresholdNoOfItems = manifestItemCount;
								do
								{
									batchThresholdNoOfItems--;
									lock (this._manifestItemLockObj)
									{
										baseManifestItem = this._manifestItems.Dequeue();
									}
									if (baseManifestItem.ObjectType == ManifestObjectType.File)
									{
										ManifestFileItem manifestFileItem = baseManifestItem as ManifestFileItem;
										fileSize += (long)manifestFileItem.FileSize;
										if (manifestFileItem.HasVersioning)
										{
											manifestFileItem.Versions.ForEach((ManifestFileItem version) => fileSize += (long)version.FileSize);
										}
									}
									else if (baseManifestItem.ObjectType == ManifestObjectType.ListItem)
									{
										ManifestListItem manifestListItem = baseManifestItem as ManifestListItem;
										if (manifestListItem.Attachments.Count <= 0)
										{
											fileSize += (long)5000;
										}
										else
										{
											manifestListItem.Attachments.ForEach((ManifestAttachment attachment) => fileSize += (long)attachment.FileSize);
										}
									}
									else if (baseManifestItem.ObjectType == ManifestObjectType.Folder)
									{
										ManifestFolderItem manifestFolderItem = baseManifestItem as ManifestFolderItem;
										if (manifestFolderItem.Attachments.Count > 0)
										{
											manifestFolderItem.Attachments.ForEach((ManifestAttachment attachment) => fileSize += (long)attachment.FileSize);
										}
									}
									this._batchManifestItems.Enqueue(baseManifestItem);
									manifestItemCount = this.ManifestItemCount;
									if (fileSize < batchThresholdInMegaBytes)
									{
										if (manifestItemCount != 0)
										{
											continue;
										}
										Metalogix.SharePoint.Actions.Migration.UploadManagerState uploadManagerState1 = this.UploadManagerState;
										Metalogix.SharePoint.Actions.Migration.UploadManagerState[] uploadManagerStateArray1 = new Metalogix.SharePoint.Actions.Migration.UploadManagerState[] { Metalogix.SharePoint.Actions.Migration.UploadManagerState.EndProcessingCalled, Metalogix.SharePoint.Actions.Migration.UploadManagerState.WaitingForCompletion };
										if (!uploadManagerState1.In<Metalogix.SharePoint.Actions.Migration.UploadManagerState>(uploadManagerStateArray1))
										{
											continue;
										}
									}
									flag = true;
								}
								while (!flag && batchThresholdNoOfItems > 0);
								break;
							}
						}
						if (this.ManifestItemsProcessState == QueueState.Processing && flag)
						{
							flag = false;
							num++;
							num1++;
							IManifestBuilder manifest = new Manifest();
							manifest.InitialiseManifest(Path.Combine(this._tempBaseStorageDirectoryPath, string.Format("Batch-{0}", num1)), this._documentBinariesDirectory.FullName, this._webId, this._siteId, this._listId, this._rootWebFolderId, this._listFolderId, this._attachmentsFolderId, this._parentWebAbsoluteUrl, this._parentWebServerRelativeUrl, this._targetBasePath, this._listName, this._listTitle, this, this._targetFolderCreated, this._targetFolderLastModified, this._listBaseTemplate, this._listBaseType, this._webTemplate);
							IAzureContainerInstance azureContainerInstance = this._containerFactory.NewInstance(num1);
							lock (this._requestMigrationJobsLock)
							{
								RequestMigrationJob requestMigrationJob = new RequestMigrationJob(num1, azureContainerInstance, this);
								this._requestMigrationJobs.Add(num1, requestMigrationJob);
							}
							AzureMigrationHelper azureMigrationHelper = new AzureMigrationHelper(azureContainerInstance, this._azureBlobStorageManager, this._operationState, manifest, this._documentBinariesDirectory.FullName);
							this._migrationHelpers.Add(num1, azureMigrationHelper);
							BatchManifestJob batchManifestJob = new BatchManifestJob(this._operationLoggingManagement, num1, azureMigrationHelper, this._webId);
							switch (this._batchSizeMode)
							{
								case BatchSizeMode.NumberOfItems:
								{
									this.LogStatusLog(string.Format("ProcessManifestItemsIntoBatches - {0} Batch {1} contains {2} Items", this._batchSizeMode.ToString(), batchManifestJob.BatchNo, batchThresholdNoOfItems));
									do
									{
										lock (this._manifestItemLockObj)
										{
											batchManifestJob.ManifestItems.Enqueue(this._manifestItems.Dequeue());
											batchThresholdNoOfItems--;
										}
									}
									while (batchThresholdNoOfItems > 0);
									break;
								}
								case BatchSizeMode.InMegaBytes:
								{
									object[] str = new object[] { this._batchSizeMode.ToString(), batchManifestJob.BatchNo, this._batchManifestItems.Count, fileSize, null };
									float single = (float)fileSize / 1048576f;
									str[4] = single.ToString("F2");
									this.LogStatusLog(string.Format("ProcessManifestItemsIntoBatches - {0} Batch {1} contains {2} Items, Total Filesize in Bytes={3} (MB={4})", str));
									do
									{
										batchManifestJob.ManifestItems.Enqueue(this._batchManifestItems.Dequeue());
									}
									while (this._batchManifestItems.Count > 0);
									fileSize = (long)0;
									break;
								}
							}
							if (this.ManifestItemsProcessState == QueueState.Processing)
							{
								this.LogStatusLog(string.Format("ProcessManifestItemsIntoBatches - Adding Batch {0} to queue to process", batchManifestJob.BatchNo));
								this._batchJobQueue.Enqueue(batchManifestJob);
								batchManifestJob.OnCompleted += new EventHandler<BatchManifestJobOnCompletedEventArgs>(this.BatchManifestJobOnCompleted);
							}
						}
						if (this.ManifestItemsProcessState == QueueState.Processing && this._batchJobQueue.Count > 0)
						{
							long num2 = Interlocked.Read(ref this._currentlyRunningBatchUploadJobs);
							if (num2 < (long)this.MaxBatchesToUpload)
							{
								Interlocked.Increment(ref this._currentlyRunningBatchUploadJobs);
								BatchManifestJob batchManifestJob1 = this._batchJobQueue.Dequeue();
								lock (this._batchesStartedLock)
								{
									this._batchesStarted.Enqueue(batchManifestJob1.BatchNo);
								}
								this.LogStatusLog(string.Format("ProcessManifestItemsIntoBatches - Dequeued Batch {0} from queue and starting upload [currentlyUploading={1}, MaxBatchesToUpload={2}]", batchManifestJob1.BatchNo, num2, this.MaxBatchesToUpload));
								batchManifestJob1.Start();
							}
						}
						if (this._operationState.IsOperationCancelled)
						{
							this.ManifestItemsProcessState = QueueState.CancelRequested;
						}
						manifestItemCount = this.ManifestItemCount;
						if (this.ManifestItemsProcessState == QueueState.Processing)
						{
							Metalogix.SharePoint.Actions.Migration.UploadManagerState uploadManagerState2 = this.UploadManagerState;
							Metalogix.SharePoint.Actions.Migration.UploadManagerState[] uploadManagerStateArray2 = new Metalogix.SharePoint.Actions.Migration.UploadManagerState[] { Metalogix.SharePoint.Actions.Migration.UploadManagerState.EndProcessingCalled, Metalogix.SharePoint.Actions.Migration.UploadManagerState.WaitingForCompletion };
							if (uploadManagerState2.In<Metalogix.SharePoint.Actions.Migration.UploadManagerState>(uploadManagerStateArray2) && manifestItemCount == 0 && this._batchJobQueue.Count == 0 && fileSize == (long)0)
							{
								this.LogStatusLog("ProcessManifestItemsIntoBatches - No more batches or items to process");
								this.ManifestItemsProcessState = QueueState.Completed;
								this._totalBatchesGenerated = num;
								ThreadPool.QueueUserWorkItem(new WaitCallback(this.CreateAllBatchesUploadedLogItem), null);
							}
						}
						if (this.ManifestItemsProcessState != QueueState.Processing)
						{
							continue;
						}
						lock (this._manifestItemsProcessWaitLock)
						{
							this._manifestItemsProcessWait.Reset();
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogItem logItem = new LogItem("Process Manifest ", this._listTitle, string.Empty, string.Empty, ActionOperationStatus.Running)
					{
						Exception = exception
					};
					this.FireOperationStarted(logItem);
					this.FireOperationFinished(logItem);
					this.ManifestItemsProcessState = QueueState.Error;
				}
			}
			finally
			{
				this.LogStatusLog(string.Format("ProcessManifestItemsIntoBatches - Finally exiting - Total batches = {0}", num));
			}
		}

		private void ProcessReportingQueueMessages(QueueMessagesResponse queueMessagesResponse, RequestMigrationJob job, StringBuilder sb, int MaxMessageinLog, ref int retryCount, ref int messagesInLog, ref int waitTimeMilliSeconds, ref int maxRetryCountForPollingQueue, ref bool errors, ref bool jobStarted, ref bool jobEnd, ref string jobEndJobId, ref JobEndEvent jobEndEvent)
		{
			if (queueMessagesResponse.Messages.Count > 0)
			{
				retryCount = 0;
				foreach (QueueMessageResponse message in queueMessagesResponse.Messages)
				{
					string str = message.Message;
					Dictionary<string, string> valuesFromJson = this.GetValuesFromJson(str);
					if (AzureUploadManager.IsEncryptedMessage(valuesFromJson))
					{
						str = this.DecryptMessage(valuesFromJson);
						valuesFromJson = this.GetValuesFromJson(str);
					}
					string value = valuesFromJson.GetValue("JobId", "?");
					string value1 = valuesFromJson.GetValue("Event", "?");
					ReportingQueueMessageEvents enumValue = value1.ToEnumValue<ReportingQueueMessageEvents>(ReportingQueueMessageEvents.Unknown);
					bool flag = string.Equals(job.MigrationJobId, value, StringComparison.OrdinalIgnoreCase);
					string str1 = (flag ? string.Empty : " [Previous Job] ");
					ReflectionEventPrinter reflectionEventPrinter = new ReflectionEventPrinter();
					if (enumValue == ReportingQueueMessageEvents.JobQueued)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobQueued{0}found for Batch {1}", str1, job.BatchNo));
						waitTimeMilliSeconds = 15000;
						maxRetryCountForPollingQueue = 7680;
						JobQueuedEvent jobQueuedEvent = AzureUploadManager.DeserializeJson<JobQueuedEvent>(str);
						this.LogSpecificItem(string.Format("SPO Migration Queued{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "QUEUED, see details", reflectionEventPrinter.PrintEvent(jobQueuedEvent), ActionOperationStatus.Completed, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobLogFileCreate)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobLogFileCreate{0}found for Batch {1}", str1, job.BatchNo));
						waitTimeMilliSeconds = 15000;
						maxRetryCountForPollingQueue = 7680;
						JobLogFileCreateEvent jobLogFileCreateEvent = AzureUploadManager.DeserializeJson<JobLogFileCreateEvent>(str);
						this.LogSpecificItem(string.Format("SPO Migration JobLogFileCreate{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "JOBLOGFILECREATE, see details", reflectionEventPrinter.PrintEvent(jobLogFileCreateEvent), ActionOperationStatus.Completed, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobStart)
					{
						jobStarted = true;
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobStart{0}found for Batch {1}", str1, job.BatchNo));
						string value2 = valuesFromJson.GetValue("DBId", "?");
						JobStartEvent jobStartEvent = AzureUploadManager.DeserializeJson<JobStartEvent>(str);
						this.LogSpecificItem(string.Format("SPO Migration Started{0}", str1), string.Format("Batch {0}", job.BatchNo), "", value2, "STARTED, see details", reflectionEventPrinter.PrintEvent(jobStartEvent), ActionOperationStatus.Completed, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobCancel)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobCancel{0}found for Batch {1}", str1, job.BatchNo));
						this.LogSpecificItem(string.Format("SPO Migration Cancel{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "CANCEL, see details", str, ActionOperationStatus.Completed, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobRestart)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobRestart{0}found for Batch {1}", str1, job.BatchNo));
						this.LogSpecificItem(string.Format("SPO Migration Restarted{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "RESTARTED, see details", str, ActionOperationStatus.Completed, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobImportant)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobImportant{0}found for Batch {1}", str1, job.BatchNo));
						this.LogSpecificItem(string.Format("SPO Migration Important{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "IMPORTANT, see details", str, ActionOperationStatus.Warning, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobError)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobError{0}found for Batch {1}", str1, job.BatchNo));
						string str2 = valuesFromJson.GetValue("SourceListItemIntId", "?");
						string value3 = valuesFromJson.GetValue("Url", "?");
						this.LogSpecificItem(string.Format("SPO Migration Error{0}", str1), string.Format("Batch {0}", job.BatchNo), string.Concat("Item Id=", str2), value3, "ERROR, see details", str, (str1 == string.Empty ? ActionOperationStatus.Failed : ActionOperationStatus.Warning), (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobFatalError)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobFatalError{0}found for Batch {1}", str1, job.BatchNo));
						JobFatalErrorEvent jobFatalErrorEvent = AzureUploadManager.DeserializeJson<JobFatalErrorEvent>(str);
						this.LogSpecificItem(string.Format("SPO Migration Fatal Error{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "FATAL ERROR, see details", reflectionEventPrinter.PrintEvent(jobFatalErrorEvent), (str1 == string.Empty ? ActionOperationStatus.Failed : ActionOperationStatus.Warning), (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobWarning)
					{
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobWarning{0}found for Batch {1}", str1, job.BatchNo));
						JobWarningEvent jobWarningEvent = AzureUploadManager.DeserializeJson<JobWarningEvent>(str);
						this.LogSpecificItem(string.Format("SPO Migration Warning{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "WARNING, see details", reflectionEventPrinter.PrintEvent(jobWarningEvent), ActionOperationStatus.Warning, (long)0);
					}
					else if (enumValue == ReportingQueueMessageEvents.JobEnd)
					{
						jobEndEvent = AzureUploadManager.DeserializeJson<JobEndEvent>(str);
						jobEndJobId = valuesFromJson.GetValue("JobId", "?");
						jobEnd = true;
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - JobEnd{0}found for Batch {1}", str1, job.BatchNo));
						if (!Regex.IsMatch(str, "TotalErrors\":\"[1-9]+"))
						{
							continue;
						}
						errors = true;
					}
					else if (enumValue != ReportingQueueMessageEvents.JobProgress)
					{
						sb.AppendLine((flag ? str : string.Concat("[Previous Job]: ", str)));
						messagesInLog++;
					}
					else
					{
						JobProgressEvent jobProgressEvent = AzureUploadManager.DeserializeJson<JobProgressEvent>(str);
						this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - Log Item for JobProgress{0} for Batch {1}", str1, job.BatchNo));
						this.LogSpecificItem(string.Format("SPO Migration Progress{0}", str1), string.Format("Batch {0}", job.BatchNo), "", "", "PROGRESS, see details", reflectionEventPrinter.PrintEvent(jobProgressEvent), (queueMessagesResponse.Success ? ActionOperationStatus.Completed : ActionOperationStatus.Failed), (long)0);
					}
				}
			}
			if (!queueMessagesResponse.Success || messagesInLog >= MaxMessageinLog || jobEnd)
			{
				if (!queueMessagesResponse.Success)
				{
					if (sb.Length > 0)
					{
						sb.AppendLine();
					}
					sb.AppendLine(queueMessagesResponse.Details);
				}
				if (sb.Length > 0)
				{
					this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - Log Item for Messages (messages={0}) for Batch {1}", messagesInLog, job.BatchNo));
					this.LogSpecificItem("SPO Migration Messages", string.Format("Batch {0}", job.BatchNo), "", "", string.Format("{0} messages, see details", messagesInLog), sb.ToString(), (queueMessagesResponse.Success ? ActionOperationStatus.Completed : ActionOperationStatus.Failed), (long)0);
					messagesInLog = 0;
					sb.Length = 0;
				}
			}
		}

		private void ProcessRequestMigrationJobs()
		{
			this.RequestMigrationJobState = QueueState.Processing;
			int num = 0;
			try
			{
				try
				{
					while (this.RequestMigrationJobState == QueueState.Processing)
					{
						this._requestMigrationJobWait.WaitOne(5000);
						int num1 = 0;
						lock (this._batchesStartedLock)
						{
							if (this._batchesStarted.Count > 0)
							{
								num1 = this._batchesStarted.Peek();
							}
						}
						if (num1 > 0)
						{
							RequestMigrationJob item = null;
							lock (this._requestMigrationJobsLock)
							{
								item = this._requestMigrationJobs[num1];
							}
							switch (item.JobState)
							{
								case AzureToO365MigrationJobState.UploadedToAzure:
								{
									if (this.RequestMigrationJob(item))
									{
										num++;
										lock (this._batchesStartedLock)
										{
											if (this._batchesStarted.Count > 0)
											{
												this._batchesStarted.Dequeue();
											}
										}
									}
									if (item.JobState != AzureToO365MigrationJobState.RequestedMigrationFromAzure)
									{
										break;
									}
									item.JobState = AzureToO365MigrationJobState.MonitoringProgress;
									this.LogStatusLog(string.Format("ProcessRequestMigrationJobs - Thread pool queuing work item to MonitorJob for Batch {0}", item.BatchNo));
									ThreadPool.QueueUserWorkItem(new WaitCallback(this.MonitorJob), item);
									break;
								}
								case AzureToO365MigrationJobState.RetryCountReached:
								{
									this.LogStatusLog(string.Format("ProcessRequestMigrationJobs - Max Retry reached due to errors for Batch {0}", item.BatchNo));
									this.RequestMigrationJobState = QueueState.Error;
									break;
								}
								case AzureToO365MigrationJobState.Error:
								{
									this.LogStatusLog(string.Format("ProcessRequestMigrationJobs - Error State for Batch {0}", item.BatchNo));
									if (item.ExceptionDetail != null)
									{
										this.LogSpecificItem("Request Migration Job", string.Format("Batch {0}", item.BatchNo), "", "", "Please see details", item.ExceptionDetail.Detail, ActionOperationStatus.Failed, (long)0);
									}
									lock (this._batchesStartedLock)
									{
										if (this._batchesStarted.Count > 0)
										{
											this._batchesStarted.Dequeue();
										}
										break;
									}
									break;
								}
							}
						}
						if (this._operationState.IsOperationCancelled)
						{
							this.RequestMigrationJobState = QueueState.CancelRequested;
						}
						if (this.RequestMigrationJobState != QueueState.Processing)
						{
							continue;
						}
						lock (this._requestMigrationJobWaitLock)
						{
							this._requestMigrationJobWait.Reset();
						}
						if (this.ManifestItemsProcessState != QueueState.Completed)
						{
							continue;
						}
						lock (this._batchesStartedLock)
						{
							if (this._batchesStarted.Count == 0)
							{
								this.RequestMigrationJobState = QueueState.Completed;
							}
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogItem logItem = new LogItem("AzureUploadManager.ProcessRequestMigrationJobs", this._listTitle, string.Empty, string.Empty, ActionOperationStatus.Running)
					{
						Exception = exception
					};
					this.FireOperationStarted(logItem);
					this.FireOperationFinished(logItem);
				}
			}
			finally
			{
				this.LogStatusLog(string.Format("ProcessRequestMigrationJobs - Finally exiting - Total successfully requested = {0}", num));
			}
		}

		private void RemoveAzureStorageContainers()
		{
			if (this._requestMigrationJobs == null || this._requestMigrationJobs.Count == 0)
			{
				return;
			}
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder1 = new StringBuilder();
			string str = string.Concat(this._parentWebAbsoluteUrl, "/", this._targetBasePath);
			LogItem logItem = new LogItem("Removing Azure Containers", string.Empty, str, string.Empty, ActionOperationStatus.Running);
			this.FireOperationStarted(logItem);
			AzureMigrationHelper azureMigrationHelper = this.DeleteDummyFile();
			if (!SharePointConfigurationVariables.CleanAzureContainer)
			{
				this.KeepAzureContainer(logItem, "Skipping because CleanAzureContainer=false.");
				return;
			}
			if (this._containerFactory != null && !this._containerFactory.CanDelete)
			{
				this.KeepAzureContainer(logItem, "Skipping because target containers cannot be deleted. They will be cleaned up by the target server.");
				return;
			}
			DeleteContainerReponse deleteContainerReponse = null;
			if (azureMigrationHelper != null)
			{
				deleteContainerReponse = azureMigrationHelper.ContainerInstance.DeleteBlobContainer();
			}
			if (this._testMode == InternalTestingMode.AzureStorageContainersFailToDelete && deleteContainerReponse != null)
			{
				deleteContainerReponse.Success = false;
				deleteContainerReponse.Details = string.Format("Container could not be deleted (Simulated failure: {0} = {1})", typeof(InternalTestingMode).Name, this._testMode.ToString());
			}
			if (deleteContainerReponse == null || !deleteContainerReponse.Success)
			{
				flag = true;
				stringBuilder1.AppendLine(string.Format("Error removing blob container '{0}'", this._commonBlobContainerName));
				if (deleteContainerReponse != null)
				{
					stringBuilder1.AppendLine(deleteContainerReponse.Details);
				}
				stringBuilder1.AppendLine();
			}
			else
			{
				stringBuilder.AppendLine(string.Format("Removed blob container '{0}'", this._commonBlobContainerName));
			}
			bool flag1 = false;
			foreach (RequestMigrationJob value in this._requestMigrationJobs.Values)
			{
				if (!value.ImportLogDownloaded && !this._operationState.IsOperationCancelled)
				{
					continue;
				}
				AzureMigrationHelper item = this._migrationHelpers[value.BatchNo];
				string fullAccessUri = item.ContainerInstance.GetManifestContainer().FullAccessUri;
				DeleteContainerReponse deleteContainerReponse1 = item.ContainerInstance.DeleteManifestContainer();
				if (this._testMode == InternalTestingMode.AzureStorageContainersFailToDelete && !flag1)
				{
					flag1 = true;
					deleteContainerReponse1.Success = false;
					deleteContainerReponse1.Details = string.Format("Container could not be deleted (Simulated failure: {0} = {1})", typeof(InternalTestingMode).Name, this._testMode.ToString());
				}
				if (!deleteContainerReponse1.Success)
				{
					flag = true;
					stringBuilder1.AppendLine(string.Format("Error removing blob container '{0}'", fullAccessUri));
					stringBuilder1.AppendLine(deleteContainerReponse1.Details);
					stringBuilder1.AppendLine();
				}
				else
				{
					stringBuilder.AppendLine(string.Format("Removed blob container '{0}'", fullAccessUri));
				}
			}
			bool flag2 = false;
			foreach (RequestMigrationJob requestMigrationJob in this._requestMigrationJobs.Values)
			{
				AzureMigrationHelper item1 = this._migrationHelpers[requestMigrationJob.BatchNo];
				string fullAccessUri1 = item1.ContainerInstance.GetReportingQueue().FullAccessUri;
				DeleteQueueReponse deleteQueueReponse = item1.ContainerInstance.DeleteReportingQueue();
				if (this._testMode == InternalTestingMode.AzureStorageContainersFailToDelete && !flag2)
				{
					flag2 = true;
					deleteQueueReponse.Success = false;
					deleteQueueReponse.Details = string.Format("Queue could not be deleted (Simulated failure: {0} = {1})", typeof(InternalTestingMode).Name, this._testMode.ToString());
				}
				if (deleteQueueReponse.Success)
				{
					stringBuilder.AppendLine(string.Format("Removed queue '{0}'", fullAccessUri1));
				}
				else if (!deleteQueueReponse.Skipped)
				{
					flag = true;
					stringBuilder1.AppendLine(string.Format("Error removing queue '{0}'", fullAccessUri1));
					stringBuilder1.AppendLine(deleteQueueReponse.Details);
					stringBuilder1.AppendLine();
				}
				else
				{
					stringBuilder.AppendLine(string.Format("Skipped removing queue '{0}' because {1}", fullAccessUri1, deleteQueueReponse.Details));
					stringBuilder.AppendLine();
					logItem.Status = ActionOperationStatus.Skipped;
				}
			}
			if (flag)
			{
				object[] newLine = new object[] { "Successes:", Environment.NewLine, stringBuilder.ToString(), Environment.NewLine, "Failures:", Environment.NewLine, stringBuilder1.ToString() };
				logItem.Details = string.Format("{0}{1}{2}{3}{4}{5}{6}", newLine);
				logItem.Status = ActionOperationStatus.Warning;
			}
			else
			{
				logItem.Details = stringBuilder.ToString();
				logItem.Status = (logItem.Status == ActionOperationStatus.Running ? ActionOperationStatus.Completed : logItem.Status);
			}
			this.FireOperationFinished(logItem);
		}

		private bool RequestMigrationJob(RequestMigrationJob requestMigrationJob)
		{
			ActionOperationStatus actionOperationStatu;
			bool flag = false;
			string str = "00000000-0000-0000-0000-000000000000";
			StringBuilder stringBuilder = new StringBuilder();
			bool flag1 = false;
			string empty = string.Empty;
			string attributeValueAsString = string.Empty;
			RequestMigrationJob retryCount = requestMigrationJob;
			retryCount.RetryCount = retryCount.RetryCount + 1;
			LogItem logItem = new LogItem("Request Migration Job", string.Format("Batch {0}", requestMigrationJob.BatchNo), "", "", ActionOperationStatus.Running);
			this.FireOperationStarted(logItem);
			this.LogStatusLog(string.Format("ProcessRequestMigrationJobs - Requesting migration job for Batch {0}, attempt {1}", requestMigrationJob.BatchNo, requestMigrationJob.RetryCount));
			try
			{
				try
				{
					SasResource reportingQueue = requestMigrationJob.Container.GetReportingQueue();
					stringBuilder.AppendLine(string.Concat("Queue SAS Container: ", reportingQueue.MigrationUri));
					bool flag2 = AzureUploadManager.IsMicrosoftCustomer();
					OperationReportingResult operationReportingResult = new OperationReportingResult((this._encryptAzureMigrationJobs ? this._migrationPipeline.RequestMigrationJob(requestMigrationJob.JobConfigurationXml, flag2, requestMigrationJob.Container.EncryptionKey) : this._migrationPipeline.RequestMigrationJob(requestMigrationJob.JobConfigurationXml, flag2, null)));
					requestMigrationJob.Result = operationReportingResult;
					if (!operationReportingResult.ErrorOccured && !string.IsNullOrEmpty(operationReportingResult.ObjectXml))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(operationReportingResult.ObjectXml);
						XmlNode xmlNodes = xmlDocument.SelectSingleNode("//CreateMigrationJob");
						empty = xmlNodes.GetAttributeValueAsString("Value");
						attributeValueAsString = xmlNodes.GetAttributeValueAsString("TraceCorrelationId");
						stringBuilder.AppendLine(string.Format("JobId = '{0}' [TraceCorrelationId={1}]", empty, attributeValueAsString));
						if (string.Equals(empty, str))
						{
							flag1 = true;
						}
						else
						{
							flag = true;
							requestMigrationJob.MigrationJobId = empty;
							requestMigrationJob.MigrationJobCorrelationId = attributeValueAsString;
							requestMigrationJob.JobState = AzureToO365MigrationJobState.RequestedMigrationFromAzure;
						}
					}
					if (requestMigrationJob.JobState != AzureToO365MigrationJobState.RequestedMigrationFromAzure && requestMigrationJob.RetryCount == 23040)
					{
						requestMigrationJob.JobState = AzureToO365MigrationJobState.RetryCountReached;
					}
					logItem.Information = string.Format("Attempt {0}/{1}{2}", requestMigrationJob.RetryCount, 23040, (flag1 ? " [Zero Guid received, ensure migration pipeline is enabled in SPO]" : string.Empty));
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					if (!string.IsNullOrEmpty(operationReportingResult.ObjectXml))
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Response XML:");
						stringBuilder.AppendLine(operationReportingResult.ObjectXml);
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(operationReportingResult.AllReportElementsAsString);
					logItem.Details = stringBuilder.ToString();
					LogItem logItem1 = logItem;
					if (requestMigrationJob.JobState == AzureToO365MigrationJobState.RequestedMigrationFromAzure)
					{
						actionOperationStatu = ActionOperationStatus.Completed;
					}
					else
					{
						actionOperationStatu = (requestMigrationJob.JobState == AzureToO365MigrationJobState.RetryCountReached ? ActionOperationStatus.Failed : ActionOperationStatus.Warning);
					}
					logItem1.Status = actionOperationStatu;
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
			}
			finally
			{
				this.FireOperationFinished(logItem);
			}
			return flag;
		}

		private bool ReRequestMigrationJob(RequestMigrationJob job)
		{
			this.LogStatusLog(string.Format("MonitorMigrationStatusForJob - ReRequestMigrationJob for Batch {0}", job.BatchNo));
			this.LogSpecificItem("Re-Request Migration Job", string.Format("Batch {0}", job.BatchNo), "", "", "", string.Empty, ActionOperationStatus.Completed, (long)0);
			job.RetryCount = 0;
			job.JobState = AzureToO365MigrationJobState.ReRequestMigrationFromAzure;
			bool flag = false;
			bool isOperationCancelled = this._operationState.IsOperationCancelled;
			bool flag1 = false;
			bool flag2 = false;
			while (!flag && !isOperationCancelled)
			{
				if (!flag2)
				{
					flag2 = this.DeleteMigrationJob(job, false);
				}
				if (flag2)
				{
					flag = this.RequestMigrationJob(job);
					flag1 = flag;
				}
				isOperationCancelled = this._operationState.IsOperationCancelled;
				if (!flag && job.JobState == AzureToO365MigrationJobState.RetryCountReached)
				{
					break;
				}
				if (isOperationCancelled || flag)
				{
					continue;
				}
				Thread.Sleep(5000);
			}
			return flag1;
		}

		public string SaveDocument(byte[] fileContents)
		{
			lock (this._saveDocumentLock)
			{
				this._fileNumber++;
				this._totalBytes += (long)((int)fileContents.Length);
			}
			string str = string.Concat(this._fileNumber.ToString("X16"), ".dat");
			using (FileStream fileStream = File.Create(Path.Combine(this._documentBinariesDirectory.FullName, str)))
			{
				fileStream.Write(fileContents, 0, (int)fileContents.Length);
				fileStream.Flush();
				fileStream.Close();
			}
			return str;
		}

		private void SaveLogToFile()
		{
			try
			{
				string str = this._tempBaseStorageDirectoryPath;
				DateTime now = DateTime.Now;
				string str1 = Path.Combine(str, string.Format("{0}_UploadManagerStatus.txt", now.ToString("yyyy-MM-dd HH.mm.ss")));
				File.AppendAllText(str1, this.GetStatusLog());
			}
			catch (Exception exception)
			{
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, "AzureUploadManager.WaitForAllToComplete", true);
			}
		}

		public int SetGroupOwner(int groupId, int owner)
		{
			int num = 0;
			lock (this._usersLock)
			{
				ManifestGroup manifestGroup = null;
				if (this._groups.TryGetValue(groupId, out manifestGroup))
				{
					manifestGroup.Owner = owner;
				}
			}
			return num;
		}

		public void StartProcessing()
		{
			if (this.UploadManagerState != Metalogix.SharePoint.Actions.Migration.UploadManagerState.None)
			{
				return;
			}
			this._startTime = DateTime.Now;
			this.LogStatusLog("StartProcessing");
			this.LogStatusLog(string.Format("MaxBatchesToUpload (simultaneously) = {0}", this.MaxBatchesToUpload));
			this.LogStatusLog(string.Format("Batch Size Mode = {0}", this._batchSizeMode.ToString()));
			this.LogStatusLog(string.Format("Is Job Encrypted = {0}", this._encryptAzureMigrationJobs));
			if (this._batchSizeMode != BatchSizeMode.NumberOfItems)
			{
				this.LogStatusLog(string.Format("BatchSizeThreshold (MB) = {0}", this.BatchThresholdInMegaBytes));
			}
			else
			{
				this.LogStatusLog(string.Format("BatchSizeThreshold (Items) = {0}", this.BatchThresholdNoOfItems));
			}
			this.LogStatusLog(string.Format("Max Retry Count Threshold For Resubmission = {0}", this.MaxThresholdCountForResubmission));
			this.UploadManagerState = Metalogix.SharePoint.Actions.Migration.UploadManagerState.Processing;
			this.ManifestItemsProcessState = QueueState.None;
			this.RequestMigrationJobState = QueueState.None;
			this._processing = true;
			this._cancelRequested = false;
			this._batchJobQueue.Clear();
			this._manifestItems.Clear();
			this._requestMigrationJobs.Clear();
			lock (this._manifestItemsProcessWaitLock)
			{
				this._manifestItemsProcessWait.Reset();
			}
			lock (this._requestMigrationJobWaitLock)
			{
				this._requestMigrationJobWait.Reset();
			}
			this._manifestItemsProcessingThread = new Thread(new ThreadStart(this.ProcessManifestItemsIntoBatches));
			this._manifestItemsProcessingThread.Start();
			this._requestMigrationJobThread = new Thread(new ThreadStart(this.ProcessRequestMigrationJobs));
			this._requestMigrationJobThread.Start();
		}

		public void Test()
		{
		}

		public void WaitForAllToComplete()
		{
			if (this.UploadManagerState != Metalogix.SharePoint.Actions.Migration.UploadManagerState.EndProcessingCalled)
			{
				return;
			}
			this.LogStatusLog("WaitForAllToComplete");
			this.UploadManagerState = Metalogix.SharePoint.Actions.Migration.UploadManagerState.WaitingForCompletion;
			this._requestMigrationJobThread.Join();
			bool flag = (this.RequestMigrationJobState != QueueState.Completed ? false : this.ManifestItemsProcessState == QueueState.Completed);
			bool isOperationCancelled = this._operationState.IsOperationCancelled;
			while (flag && !isOperationCancelled)
			{
				int num = 0;
				int num1 = 0;
				lock (this._requestMigrationJobsLock)
				{
					foreach (KeyValuePair<int, RequestMigrationJob> _requestMigrationJob in this._requestMigrationJobs)
					{
						if (_requestMigrationJob.Value.JobState == AzureToO365MigrationJobState.Complete)
						{
							num++;
						}
						else if (_requestMigrationJob.Value.JobState != AzureToO365MigrationJobState.Error)
						{
							if (_requestMigrationJob.Value.JobState != AzureToO365MigrationJobState.RetryCountReached)
							{
								continue;
							}
							flag = false;
							break;
						}
						else
						{
							num1++;
						}
					}
					if (this._requestMigrationJobs.Count == num + num1)
					{
						flag = false;
					}
				}
				isOperationCancelled = this._operationState.IsOperationCancelled;
				if (!flag || isOperationCancelled)
				{
					continue;
				}
				Thread.Sleep(5000);
			}
			long num2 = Interlocked.Read(ref this._threadPoolTaskCount);
			while (num2 > (long)0)
			{
				num2 = Interlocked.Read(ref this._threadPoolTaskCount);
				Thread.Sleep(5000);
			}
			this._endTime = DateTime.Now;
			TimeSpan timeSpan = this._endTime - this._startTime;
			object[] objArray = new object[] { Math.Truncate(timeSpan.TotalSeconds), timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds };
			this.LogStatusLog(string.Format("Total Time Processing Within Upload Manager: {0}s ([dd:hh:mm:ss] {1:D2}:{2:D2}:{3:D2}:{4:D2})", objArray));
			object obj = this._totalBytes;
			float single = (float)this._totalBytes / 1048576f;
			this.LogStatusLog(string.Format("Total Document Bytes Processed: {0} (MB={1})", obj, single.ToString("F2")));
			this.SaveLogToFile();
			this.RemoveAzureStorageContainers();
			this._operationLoggingManagement.DisconnectOperationLogging(this);
		}

		public event ActionEventHandler OperationFinished;

		public event ActionEventHandler OperationStarted;

		public event ActionEventHandler OperationUpdated;
	}
}