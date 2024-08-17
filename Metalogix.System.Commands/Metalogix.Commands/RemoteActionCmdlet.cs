using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.Explorer;
using Metalogix.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Metalogix.Commands
{
	public abstract class RemoteActionCmdlet : ActionCmdlet
	{
		[Parameter(Mandatory=false, HelpMessage="Specify the subject name of the certificate to use for encryption from the 'Personal' store.  If multiple certificates are found, then the first one will be used.")]
		public string Certificate
		{
			get;
			set;
		}

		[Parameter(Mandatory=false, HelpMessage="Queues the current job for remote execution.")]
		public SwitchParameter RunRemotely
		{
			get;
			set;
		}

		protected RemoteActionCmdlet()
		{
		}

		private void DisposeJobList()
		{
			try
			{
				if (this.m_jobList != null)
				{
					this.m_jobList.Dispose();
					this.m_jobList = null;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.WriteError(new ErrorRecord(exception, "Error finalizing operation", ErrorCategory.InvalidOperation, this.ActionType));
			}
		}

		protected override void EndProcessing()
		{
			if (this.RunRemotely)
			{
				this.ExecuteRemotely();
				return;
			}
			base.EndProcessing();
		}

		private void EnsureRemoteSchedulerIsRunning()
		{
			if (!RemoteJobScheduler.Instance.IsJobRunning)
			{
				RemoteJobScheduler.Instance.Run();
			}
		}

		private void ExecuteRemotely()
		{
			string str;
			Metalogix.Jobs.Job job;
			try
			{
				try
				{
					this.SetSourceAndTarget();
					if (!this.ProcessParameters())
					{
						base.WriteError(new ErrorRecord(new ArgumentException("The parameters as defined cannot be processed for the action"), "ArgumentError", ErrorCategory.InvalidArgument, base.Action));
						this.DisposeJobList();
						return;
					}
					else if (!base.Action.GetCollectionsViolateSourceTargetRestrictions(this.SourceCollection, this.TargetCollection, out str))
					{
						job = (string.IsNullOrEmpty(base.JobID) ? new Metalogix.Jobs.Job(base.Action, this.SourceCollection, this.TargetCollection) : new Metalogix.Jobs.Job(base.Action, this.SourceCollection, this.TargetCollection, base.JobID));
						this.m_job = job;
						if (this.m_jobList != null)
						{
							this.m_jobList.Jobs.Add(job);
							this.m_jobList.Jobs.Update();
						}
						this.QueueForRemoteExecution();
						this.EnsureRemoteSchedulerIsRunning();
						base.WriteObject(new JobSummary(this.m_job));
						this.m_job = null;
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
					Logging.LogExceptionToTextFileWithEventLogBackup(exception, string.Format("Exception occured running PowerShell Cmdlet: {0}", base.CmdLetName), true);
					base.WriteError(new ErrorRecord(exception, exception.Message, ErrorCategory.InvalidOperation, this));
				}
			}
			finally
			{
				this.DisposeJobList();
			}
		}

		private X509Certificate2 GetCertificate()
		{
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			x509Store.Open(OpenFlags.ReadOnly);
			X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, this.Certificate, false);
			if (x509Certificate2Collection.Count == 0)
			{
				throw new Exception(string.Concat("Failed to find certificate with name ", this.Certificate, " from the Personal store."));
			}
			return x509Certificate2Collection[0];
		}

		private string GetScriptFile()
		{
			if (!Directory.Exists(Metalogix.Actions.Remoting.Utils.MetalogixTempPath))
			{
				FileUtils.CreateDirectory(Metalogix.Actions.Remoting.Utils.MetalogixTempPath, null, null);
			}
			return string.Concat(Metalogix.Actions.Remoting.Utils.MetalogixTempPath, "\\", this.m_job.JobID, (PowerShellUtils.IsPowerShellInstalled ? ".ps1" : ".txt"));
		}

		protected override bool ProcessParameters()
		{
			if (!base.ProcessParameters())
			{
				return false;
			}
			if (!this.RunRemotely)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(this.Certificate))
			{
				return true;
			}
			base.WriteError(new ErrorRecord(new ArgumentException("Required parameters for sending email was missing", "CertificateName"), "", ErrorCategory.InvalidArgument, null));
			return false;
		}

		private void QueueForRemoteExecution()
		{
			X509Certificate2 certificate = this.GetCertificate();
			string scriptFile = this.GetScriptFile();
			Metalogix.Jobs.Job[] mJob = new Metalogix.Jobs.Job[] { this.m_job };
			PowerShellUtils.CreatePowerShellScript(mJob, Cryptography.ProtectionScope.Certificate, scriptFile, certificate, true);
			RemoteJobScheduler.Instance.UpdateJobInfo(this.m_job.JobID, ActionStatus.Queued, null);
			RemoteJobScheduler.Instance.ProcessJobQueue(this, null);
		}

		private void SetSourceAndTarget()
		{
			if (base.Target == null)
			{
				base.Target = base.GetNodeFromLocation();
				return;
			}
			if (base.Action.SourceCardinality == Cardinality.Zero)
			{
				this.m_sourceNodes.Clear();
				return;
			}
			if (this.m_sourceNodes.Count == 0 && this.m_source == null)
			{
				Node nodeFromLocation = base.GetNodeFromLocation();
				if (nodeFromLocation != null)
				{
					this.m_sourceNodes.Add(nodeFromLocation);
				}
			}
		}
	}
}