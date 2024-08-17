using Metalogix.Azure;
using Metalogix.Core.OperationLog;
using Metalogix.Office365;
using Metalogix.Utilities;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	public class RequestMigrationJob
	{
		private readonly object _lock = new object();

		private AzureToO365MigrationJobState _jobState;

		private IUploadManagerStatusLog _uploadManagerStatusLog;

		private string _jobConfigurationXml;

		private Metalogix.Office365.MigrationJobState _migrationJobState;

		private string _migrationJobStateCorrelationId;

		private string _migrationJobId;

		private string _migrationJobCorrelationId;

		private Metalogix.Utilities.ExceptionDetail _exceptionDetail;

		private OperationReportingResult _result;

		private bool _uploaded;

		private bool _imported;

		private int _retryCount;

		private string _importLogPath;

		private string _importLogContents;

		public int BatchNo
		{
			get;
			private set;
		}

		public IAzureContainerInstance Container
		{
			get;
			private set;
		}

		public string DeleteMigrationJobConfigurationXml
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
				{
					OmitXmlDeclaration = true
				};
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
				{
					xmlWriter.WriteStartElement("DeleteMigrationJob");
					xmlWriter.WriteAttributeString("MigrationJobId", this._migrationJobId);
					xmlWriter.WriteEndElement();
					xmlWriter.Flush();
					xmlWriter.Close();
				}
				return stringBuilder.ToString();
			}
		}

		public Metalogix.Utilities.ExceptionDetail ExceptionDetail
		{
			get
			{
				Metalogix.Utilities.ExceptionDetail exceptionDetail;
				lock (this._lock)
				{
					exceptionDetail = this._exceptionDetail;
				}
				return exceptionDetail;
			}
			set
			{
				lock (this._lock)
				{
					this._exceptionDetail = value;
				}
			}
		}

		public bool Imported
		{
			get
			{
				bool flag;
				lock (this._lock)
				{
					flag = this._imported;
				}
				return flag;
			}
			set
			{
				lock (this._lock)
				{
					this._imported = value;
				}
			}
		}

		public string ImportLogContents
		{
			get
			{
				string str;
				lock (this._lock)
				{
					str = this._importLogContents;
				}
				return str;
			}
			set
			{
				lock (this._lock)
				{
					this._importLogContents = value;
				}
			}
		}

		public bool ImportLogDownloaded
		{
			get;
			set;
		}

		public string ImportLogPath
		{
			get
			{
				string str;
				lock (this._lock)
				{
					str = this._importLogPath;
				}
				return str;
			}
			set
			{
				lock (this._lock)
				{
					this._importLogPath = value;
				}
			}
		}

		public string JobConfigurationXml
		{
			get
			{
				string str;
				lock (this._lock)
				{
					str = this._jobConfigurationXml;
				}
				return str;
			}
			set
			{
				lock (this._lock)
				{
					this._jobConfigurationXml = value;
				}
			}
		}

		public AzureToO365MigrationJobState JobState
		{
			get
			{
				AzureToO365MigrationJobState azureToO365MigrationJobState;
				lock (this._lock)
				{
					azureToO365MigrationJobState = this._jobState;
				}
				return azureToO365MigrationJobState;
			}
			set
			{
				lock (this._lock)
				{
					this._jobState = value;
					if (this._uploadManagerStatusLog != null)
					{
						this._uploadManagerStatusLog.LogStatusLog(string.Format("Batch {0} JobState changed to {1}", this.BatchNo, this._jobState.ToString()));
					}
				}
			}
		}

		public string MigrationJobCorrelationId
		{
			get
			{
				string str;
				lock (this._lock)
				{
					str = this._migrationJobCorrelationId;
				}
				return str;
			}
			set
			{
				lock (this._lock)
				{
					this._migrationJobCorrelationId = value;
				}
			}
		}

		public string MigrationJobId
		{
			get
			{
				string str;
				lock (this._lock)
				{
					str = this._migrationJobId;
				}
				return str;
			}
			set
			{
				lock (this._lock)
				{
					this._migrationJobId = value;
				}
			}
		}

		public Metalogix.Office365.MigrationJobState MigrationJobState
		{
			get
			{
				Metalogix.Office365.MigrationJobState migrationJobState;
				lock (this._lock)
				{
					migrationJobState = this._migrationJobState;
				}
				return migrationJobState;
			}
			set
			{
				lock (this._lock)
				{
					this._migrationJobState = value;
				}
			}
		}

		public string MigrationJobStateCorrelationId
		{
			get
			{
				string str;
				lock (this._lock)
				{
					str = this._migrationJobStateCorrelationId;
				}
				return str;
			}
			set
			{
				lock (this._lock)
				{
					this._migrationJobStateCorrelationId = value;
				}
			}
		}

		public OperationReportingResult Result
		{
			get
			{
				OperationReportingResult operationReportingResult;
				lock (this._lock)
				{
					operationReportingResult = this._result;
				}
				return operationReportingResult;
			}
			set
			{
				lock (this._lock)
				{
					this._result = value;
				}
			}
		}

		public int RetryCount
		{
			get
			{
				int num;
				lock (this._lock)
				{
					num = this._retryCount;
				}
				return num;
			}
			set
			{
				lock (this._lock)
				{
					this._retryCount = value;
				}
			}
		}

		public bool Uploaded
		{
			get
			{
				bool flag;
				lock (this._lock)
				{
					flag = this._uploaded;
				}
				return flag;
			}
			set
			{
				lock (this._lock)
				{
					this._uploaded = value;
				}
			}
		}

		public RequestMigrationJob(int batchNo, IAzureContainerInstance container, IUploadManagerStatusLog uploadManagerStatusLog)
		{
			this.BatchNo = batchNo;
			this.Container = container;
			this._uploadManagerStatusLog = uploadManagerStatusLog;
			this._jobConfigurationXml = string.Empty;
			this._migrationJobId = string.Empty;
			this._migrationJobCorrelationId = string.Empty;
			this._migrationJobState = Metalogix.Office365.MigrationJobState.None;
			this._migrationJobStateCorrelationId = string.Empty;
			this._importLogPath = string.Empty;
			this._importLogContents = string.Empty;
			this._jobState = AzureToO365MigrationJobState.None;
			this._uploaded = false;
			this._imported = false;
			this._retryCount = 0;
		}
	}
}