using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Blocker;
using Metalogix.Core;
using Metalogix.Explorer;
using Metalogix.Jobs;
using Metalogix.Telemetry;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Metalogix.Commands
{
	public abstract class ActionCmdlet : AssemblyBindingCmdlet
	{
		private bool _isInitialized;

		private bool isRemoteJob;

		private bool m_bQuiet;

		private string m_sJobFile;

		protected internal Metalogix.Explorer.Node m_source;

		private Metalogix.Explorer.Node m_target;

		private string m_sJobDatabase;

		private string _jobID;

		private string m_sCmdLetName;

		protected internal List<Metalogix.Explorer.Node> m_sourceNodes = new List<Metalogix.Explorer.Node>();

		private IXMLAbleList m_sourceCollection;

		private IXMLAbleList m_targetCollection;

		protected Metalogix.Actions.Action m_action;

		protected JobHistoryDb m_jobList;

		private System.Collections.Generic.Queue<object> m_events = new System.Collections.Generic.Queue<object>();

		private AutoResetEvent m_resetEvent = new AutoResetEvent(false);

		private bool m_actionFinished;

		private Exception m_actionException;

		protected internal Metalogix.Jobs.Job m_job;

		private string m_sActionSource;

		private string m_sActionTarget;

		public Metalogix.Actions.Action Action
		{
			get
			{
				if (this.m_action == null)
				{
					this.SetupAction();
					this.m_action = this.CreateAction();
				}
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		protected abstract Type ActionType
		{
			get;
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The connection string of a jobs database to use for logging. If the connection string is invalid, an exception is thrown.  If 'AgentDatabase' is specified, then it will take precedence over other job database settings.")]
		public string AgentDatabase
		{
			get;
			set;
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A semicolon delimited list of email addresses to BCC on the job completion email.")]
		public string BCCEmailAddress
		{
			get
			{
				return this.Action.Options.BCCEmailAddress;
			}
			set
			{
				this.Action.Options.BCCEmailAddress = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A semicolon delimited list of email addresses to CC on the job completion email.")]
		public string CCEmailAddress
		{
			get
			{
				return this.Action.Options.CCEmailAddress;
			}
			set
			{
				this.Action.Options.CCEmailAddress = value;
			}
		}

		public string CmdLetName
		{
			get
			{
				if (this.m_sCmdLetName == null)
				{
					object[] customAttributes = base.GetType().GetCustomAttributes(typeof(CmdletAttribute), false);
					CmdletAttribute cmdletAttribute = (CmdletAttribute)customAttributes[0];
					this.m_sCmdLetName = (cmdletAttribute != null ? string.Format("{0}-{1}", cmdletAttribute.VerbName, cmdletAttribute.NounName) : base.GetType().ToString());
				}
				return this.m_sCmdLetName;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The absolute file path of the html template to use for the job completion email when the job fails.")]
		public string EmailFailureTemplateFilePath
		{
			get
			{
				return this.Action.Options.EmailFailureTemplateFilePath;
			}
			set
			{
				this.Action.Options.EmailFailureTemplateFilePath = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A password to use for the specified user when connecting to the specified email server. If no user name is set, default credentials will be used.")]
		public string EmailPassword
		{
			get
			{
				return this.Action.Options.EmailPassword;
			}
			set
			{
				this.Action.Options.EmailPassword = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="An email server to use for sending job completion emails.")]
		public string EmailServer
		{
			get
			{
				return this.Action.Options.EmailServer;
			}
			set
			{
				this.Action.Options.EmailServer = value;
				this.Action.Options.SendEmail = true;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A custom subject for the job completion email.")]
		public string EmailSubject
		{
			get
			{
				return this.Action.Options.EmailSubject;
			}
			set
			{
				this.Action.Options.EmailSubject = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The absolute file path of the html template to use for the job completion email when the job is successful.")]
		public string EmailSuccessTemplateFilePath
		{
			get
			{
				return this.Action.Options.EmailSuccessTemplateFilePath;
			}
			set
			{
				this.Action.Options.EmailSuccessTemplateFilePath = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A user name to use when connecting to the specified email server. If not set, default credentials will be used.")]
		public string EmailUserName
		{
			get
			{
				return this.Action.Options.EmailUserName;
			}
			set
			{
				this.Action.Options.EmailUserName = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="True if user wants secure SMTP connection.")]
		public SwitchParameter EnableSslForEmail
		{
			get
			{
				return this.Action.Options.EnableSslForEmail;
			}
			set
			{
				this.Action.Options.EnableSslForEmail = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The email address of the mailbox from which the job completion emails will be sent.")]
		public string FromEmailAddress
		{
			get
			{
				return this.Action.Options.FromEmailAddress;
			}
			set
			{
				this.Action.Options.FromEmailAddress = value;
				this.Action.Options.SendEmail = true;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The connection string of a jobs database to use for logging. If the connection string is invalid, an exception is thrown.  If both 'JobFile' and 'JobDatabase' parameters are specified, then the 'JobDatabase' parameter will take precedence and the job will only be written to the job database.")]
		public string JobDatabase
		{
			get
			{
				return this.m_sJobDatabase;
			}
			set
			{
				this.m_sJobDatabase = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of a job file to use for logging. If the file does not exist it will be created.")]
		public string JobFile
		{
			get
			{
				return this.m_sJobFile;
			}
			set
			{
				this.m_sJobFile = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The job ID")]
		public string JobID
		{
			get
			{
				return this._jobID;
			}
			set
			{
				this._jobID = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="If set, the operation will not report progress to the PowerShell console.")]
		public SwitchParameter Quiet
		{
			get
			{
				return this.m_bQuiet;
			}
			set
			{
				this.m_bQuiet = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The source node for the actions.")]
		public Metalogix.Explorer.Node Source
		{
			get
			{
				return this.m_source;
			}
			set
			{
				this.m_source = value;
				this.m_sourceCollection = null;
			}
		}

		protected virtual IXMLAbleList SourceCollection
		{
			get
			{
				if (this.m_sourceCollection == null)
				{
					if (this.m_sourceNodes.Count != 0)
					{
						this.m_sourceCollection = new NodeCollection(this.m_sourceNodes.ToArray());
					}
					else if (this.m_source == null)
					{
						this.m_sourceCollection = new NodeCollection(new Metalogix.Explorer.Node[0]);
					}
					else
					{
						Metalogix.Explorer.Node[] mSource = new Metalogix.Explorer.Node[] { this.m_source };
						this.m_sourceCollection = new NodeCollection(mSource);
					}
				}
				return this.m_sourceCollection;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The target node for the actions.")]
		public Metalogix.Explorer.Node Target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
				this.m_targetCollection = null;
			}
		}

		protected virtual IXMLAbleList TargetCollection
		{
			get
			{
				if (this.m_targetCollection == null)
				{
					if (this.Target != null)
					{
						Metalogix.Explorer.Node[] target = new Metalogix.Explorer.Node[] { this.Target };
						this.m_targetCollection = new NodeCollection(target);
					}
					else
					{
						this.m_targetCollection = new NodeCollection(new Metalogix.Explorer.Node[0]);
					}
				}
				return this.m_targetCollection;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A semicolon delimited list of email addresses to which the job completion email will be sent.")]
		public string ToEmailAddress
		{
			get
			{
				return this.Action.Options.ToEmailAddress;
			}
			set
			{
				this.Action.Options.ToEmailAddress = value;
				this.Action.Options.SendEmail = true;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The collection of data transformers which the action needs to run. Note that if the passed-in value for this parameter does not include transformers which are considered mandatory, they will be added automatically.")]
		public TransformerCollection Transformers
		{
			get
			{
				return this.Action.Options.Transformers;
			}
			set
			{
				this.Action.Options.Transformers = value;
			}
		}

		protected ActionCmdlet() : base(false)
		{
		}

		protected override void BeginProcessing()
		{
			try
			{
				this.SetupAction();
				if (!string.IsNullOrEmpty(this.AgentDatabase))
				{
					this.AgentDatabase = ActionCmdlet.DecryptJobDBConnString(this.AgentDatabase);
					this.m_jobList = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.Agent, this.AgentDatabase);
				}
				else if (!string.IsNullOrEmpty(this.JobDatabase))
				{
					this.JobDatabase = ActionCmdlet.DecryptJobDBConnString(this.JobDatabase);
					this.m_jobList = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlServer, this.JobDatabase);
				}
				else if (!string.IsNullOrEmpty(this.JobFile))
				{
					this.m_jobList = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, (this.JobFile.EndsWith(".lst") ? this.JobFile : string.Concat(this.JobFile, ".lst")));
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.WriteError(new ErrorRecord(exception, "Error initializing operation", ErrorCategory.InvalidOperation, this.ActionType));
			}
		}

		protected virtual Metalogix.Actions.Action CreateAction()
		{
			Metalogix.Actions.Action action = Activator.CreateInstance(this.ActionType) as Metalogix.Actions.Action;
			this.TurnOffSwitches(action.Options);
			return action;
		}

		public static string DecryptJobDBConnString(string connectionString)
		{
			SecureString secureString;
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
			if (!sqlConnectionStringBuilder.IntegratedSecurity)
			{
				if (!Cryptography.IsEncryptedWithAESProvider(sqlConnectionStringBuilder.Password, out secureString))
				{
					secureString = Cryptography.DecryptTextWithCert(sqlConnectionStringBuilder.Password);
				}
				if (!secureString.IsNullOrEmpty())
				{
					sqlConnectionStringBuilder.Password = secureString.ToInsecureString();
				}
			}
			return sqlConnectionStringBuilder.ConnectionString;
		}

		protected override void EndProcessing()
		{
			string str;
			Metalogix.Jobs.Job job;
			try
			{
				if (this.Target == null)
				{
					this.Target = this.GetNodeFromLocation();
				}
				else if (this.Action.SourceCardinality == Cardinality.Zero)
				{
					this.m_sourceNodes.Clear();
				}
				else if (this.m_sourceNodes.Count == 0 && this.m_source == null)
				{
					Metalogix.Explorer.Node nodeFromLocation = this.GetNodeFromLocation();
					if (nodeFromLocation != null)
					{
						this.m_sourceNodes.Add(nodeFromLocation);
					}
				}
				if (!this.ProcessParameters())
				{
					base.WriteError(new ErrorRecord(new ArgumentException("The parameters as defined cannot be processed for the action"), "ArgumentError", ErrorCategory.InvalidArgument, this.Action));
				}
				else if (!this.Action.GetCollectionsViolateSourceTargetRestrictions(this.SourceCollection, this.TargetCollection, out str))
				{
					if (string.IsNullOrEmpty(this.JobID))
					{
						job = new Metalogix.Jobs.Job(this.Action, this.SourceCollection, this.TargetCollection);
					}
					else
					{
						job = new Metalogix.Jobs.Job(this.Action, this.SourceCollection, this.TargetCollection, this.JobID);
						this.isRemoteJob = true;
					}
					this.m_job = job;
					if (this.m_jobList != null)
					{
						this.m_jobList.Jobs.Add(job);
						this.m_jobList.Jobs.Update();
					}
					job.ActionBlocked += new ActionBlockerHandler(this.On_Action_Blocked);
					job.ActionStarting += new ActionStartedEventHandler(this.On_Action_Started);
					job.ActionOperationFinishedEvent += new ActionEventHandler(this.On_Action_OperationFinished);
					this.m_actionFinished = false;
					this.m_actionException = null;
					ActionCmdlet actionCmdlet = this;
					Thread thread = new Thread(new ThreadStart(actionCmdlet.Run));
					thread.Start();
					while (!this.m_actionFinished)
					{
						this.m_resetEvent.WaitOne();
						while (this.m_events.Count > 0)
						{
							object obj = this.m_events.Dequeue();
							if (obj == null)
							{
								continue;
							}
							ActionCmdlet.ActionStartedData actionStartedDatum = obj as ActionCmdlet.ActionStartedData;
							if (actionStartedDatum == null)
							{
								ActionCmdlet.ActionBlockedData actionBlockedDatum = obj as ActionCmdlet.ActionBlockedData;
								if (actionBlockedDatum == null)
								{
									LogItem logItem = obj as LogItem;
									if (logItem == null)
									{
										continue;
									}
									this.Write_Action_OperationFinished(logItem);
								}
								else
								{
									this.Write_Action_Blocked(actionBlockedDatum.ChangeType, actionBlockedDatum.Message);
								}
							}
							else
							{
								this.Write_Action_Started(actionStartedDatum.Action, actionStartedDatum.SourceString, actionStartedDatum.TargetString);
							}
						}
					}
					thread.Join();
					if (this.isRemoteJob)
					{
						Thread.Sleep((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
					}
					if (this.m_actionException != null)
					{
						throw this.m_actionException;
					}
					this.m_job = null;
					this.m_sActionSource = null;
					this.m_sActionTarget = null;
					base.WriteObject(new JobSummary(job));
				}
				else
				{
					base.WriteError(new ErrorRecord(new Exception(str), "Input collections violate restrictions", ErrorCategory.InvalidArgument, this));
					return;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, string.Format("Exception occured running PowerShell Cmdlet: {0}", this.CmdLetName), true);
				base.WriteError(new ErrorRecord(exception, exception.Message, ErrorCategory.InvalidOperation, this));
			}
			try
			{
				if (this.m_jobList != null)
				{
					this.m_jobList.Dispose();
					this.m_jobList = null;
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				base.WriteError(new ErrorRecord(exception2, "Error finalizing operation", ErrorCategory.InvalidOperation, this.ActionType));
			}
			this.TeardownAction();
		}

		protected Metalogix.Explorer.Node GetNodeFromLocation()
		{
			PSDriveInfo current = base.SessionState.Drive.Current;
			if (current == null && !this.m_bQuiet)
			{
				base.WriteVerbose("No drive info");
			}
			if (!typeof(NodeDriveInfo).IsAssignableFrom(current.GetType()))
			{
				return null;
			}
			return ((NodeDriveInfo)current).GetCurrentNode();
		}

		private void InitializeTelemetry()
		{
			if (TelemetryConfigurationVariables.TelemetryOptIn)
			{
				Type type = ((IEnumerable<Type>)ApplicationData.MainAssembly.GetTypes()).FirstOrDefault<Type>((Type t) => t.Name.Equals("Telemetry", StringComparison.InvariantCultureIgnoreCase));
				if (type != null)
				{
					MethodInfo method = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
					if (method != null)
					{
						method.Invoke(null, null);
					}
				}
			}
		}

		private void On_Action_Blocked(object sender, ActionBlockerEventArgs args)
		{
			if (!this.m_bQuiet)
			{
				System.Collections.Generic.Queue<object> mEvents = this.m_events;
				ActionCmdlet.ActionBlockedData actionBlockedDatum = new ActionCmdlet.ActionBlockedData()
				{
					ChangeType = args.ChangeType,
					Message = args.Message
				};
				mEvents.Enqueue(actionBlockedDatum);
				this.m_resetEvent.Set();
			}
		}

		protected void On_Action_OperationFinished(LogItem operation)
		{
			if (!this.m_bQuiet)
			{
				this.m_events.Enqueue(operation);
				this.m_resetEvent.Set();
			}
		}

		protected void On_Action_Started(Metalogix.Actions.Action sender, string sSourceString, string sTargetString)
		{
			if (!this.m_bQuiet)
			{
				System.Collections.Generic.Queue<object> mEvents = this.m_events;
				ActionCmdlet.ActionStartedData actionStartedDatum = new ActionCmdlet.ActionStartedData()
				{
					Action = sender,
					SourceString = sSourceString,
					TargetString = sTargetString
				};
				mEvents.Enqueue(actionStartedDatum);
				this.m_resetEvent.Set();
			}
		}

		protected virtual bool ProcessParameters()
		{
			if (this.Action.Options.SendEmail)
			{
				string str = null;
				if (string.IsNullOrEmpty(this.Action.Options.EmailServer))
				{
					str = "EmailServer";
				}
				else if (!string.IsNullOrEmpty(this.Action.Options.EmailUserName) && string.IsNullOrEmpty(this.Action.Options.EmailPassword))
				{
					str = "EmailPassword";
				}
				if (string.IsNullOrEmpty(this.Action.Options.ToEmailAddress))
				{
					str = "ToEmailAddress";
				}
				else if (string.IsNullOrEmpty(this.Action.Options.FromEmailAddress))
				{
					str = "FromEmailAddress";
				}
				if (str != null)
				{
					base.WriteError(new ErrorRecord(new ArgumentException("Required parameters for sending email was missing", str), "", ErrorCategory.InvalidArgument, null));
					return false;
				}
			}
			return true;
		}

		protected override void ProcessRecord()
		{
			if (this.Source != null)
			{
				this.m_sourceNodes.Add(this.Source);
			}
		}

		protected virtual void Run()
		{
			try
			{
				try
				{
					this.Action.IsRemoteJob = this.isRemoteJob;
					this.Action.IsUsingPowerShell = true;
					this.Action.Run(this.SourceCollection, this.TargetCollection);
				}
				catch (Exception exception)
				{
					this.m_actionException = exception;
				}
			}
			finally
			{
				this.m_actionFinished = true;
				this.m_resetEvent.Set();
			}
		}

		private void SetupAction()
		{
			if (this._isInitialized)
			{
				return;
			}
			base.InitializeMainAssembly();
			base.InitializeLicense();
			this.SetupEnvironmentPath();
			this._isInitialized = true;
			this.InitializeTelemetry();
		}

		private void SetupEnvironmentPath()
		{
			string str = (new StringBuilder()).AppendLine("$temp = $env:Path").AppendFormat("$env:Path = $env:Path + \";{0};{0}\\AMD64;{0}\\x86\";", Path.GetDirectoryName(ApplicationData.MainAssembly.Location)).ToString();
			base.InvokeCommand.InvokeScript(str, false, PipelineResultTypes.None, null, null);
		}

		private void TeardownAction()
		{
			if (!this._isInitialized)
			{
				return;
			}
			this.TeardownEnvironmentPath();
			base.RevertLicense();
			this._isInitialized = false;
		}

		private void TeardownEnvironmentPath()
		{
			base.InvokeCommand.InvokeScript("$env:Path = $temp", false, PipelineResultTypes.None, null, null);
		}

		protected virtual void TurnOffSwitches(object optionsObject)
		{
			PropertyInfo[] properties = optionsObject.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
				{
					bool flatten = false;
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(CmdletParameterFlattenAttribute), true);
					if ((int)customAttributes.Length > 0)
					{
						flatten = ((CmdletParameterFlattenAttribute)customAttributes[0]).Flatten;
					}
					if (flatten)
					{
						this.TurnOffSwitches(propertyInfo.GetValue(optionsObject, null));
					}
				}
				else if (propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(bool))
				{
					object[] objArray = propertyInfo.GetCustomAttributes(typeof(CmdletEnabledParameterAttribute), true);
					bool flag = true;
					if ((int)objArray.Length > 0)
					{
						CmdletEnabledParameterAttribute cmdletEnabledParameterAttribute = (CmdletEnabledParameterAttribute)objArray[0];
						if (!cmdletEnabledParameterAttribute.CmdletEnabledParameter && cmdletEnabledParameterAttribute.ConditionalPropertyName == null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						propertyInfo.SetValue(optionsObject, false, null);
					}
				}
			}
		}

		protected void Write_Action_Blocked(ActionBlockerChangeType changeType, string message)
		{
			if (!this.m_bQuiet)
			{
				if (changeType == ActionBlockerChangeType.Blocked)
				{
					ProgressRecord progressRecord = new ProgressRecord(2, "Migration Paused", message)
					{
						RecordType = ProgressRecordType.Processing
					};
					base.WriteProgress(progressRecord);
					return;
				}
				ProgressRecord progressRecord1 = new ProgressRecord(2, "Migration Paused", message)
				{
					RecordType = ProgressRecordType.Completed
				};
				base.WriteProgress(progressRecord1);
			}
		}

		protected void Write_Action_OperationFinished(LogItem operation)
		{
			if (!this.m_bQuiet)
			{
				if (string.IsNullOrEmpty(operation.Source) && string.IsNullOrEmpty(operation.Target))
				{
					this.WriteOperationInfoOnly(operation);
					return;
				}
				this.WriteOperation(operation);
			}
		}

		protected void Write_Action_Started(Metalogix.Actions.Action sender, string sSourceString, string sTargetString)
		{
			this.m_sActionSource = sSourceString;
			this.m_sActionTarget = sTargetString;
			if (!this.m_bQuiet)
			{
				object[] status = new object[] { this.Action.Status, this.Action.Name, sSourceString, sTargetString };
				string str = string.Format("{0}: {1} from {2} to {3}", status);
				base.WriteProgress(new ProgressRecord(0, this.Action.Name, str));
			}
		}

		private void WriteOperation(LogItem operation)
		{
			string str = string.Format("{0} from {1} to {2}", operation.Operation, operation.Source, operation.Target);
			base.WriteProgress(new ProgressRecord(1, (string.IsNullOrEmpty(operation.Operation) ? " " : operation.Operation), str));
			if (operation.Status != ActionOperationStatus.Completed && operation.Status != ActionOperationStatus.Running)
			{
				base.WriteWarning(string.Concat(str, " - ", operation.Message));
			}
		}

		private void WriteOperationInfoOnly(LogItem operation)
		{
			string str = (string.IsNullOrEmpty(operation.Information) ? " " : operation.Information);
			base.WriteProgress(new ProgressRecord(1, (string.IsNullOrEmpty(operation.Operation) ? " " : operation.Operation), str));
		}

		private class ActionBlockedData
		{
			public ActionBlockerChangeType ChangeType
			{
				get;
				set;
			}

			public string Message
			{
				get;
				set;
			}

			public ActionBlockedData()
			{
			}
		}

		private class ActionStartedData
		{
			public Metalogix.Actions.Action Action;

			public string SourceString;

			public string TargetString;

			public ActionStartedData()
			{
			}
		}
	}
}