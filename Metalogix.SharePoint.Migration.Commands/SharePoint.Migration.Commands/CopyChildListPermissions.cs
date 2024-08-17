using Metalogix;
using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.Jobs;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLChildListPermissions")]
	public class CopyChildListPermissions : ActionCmdlet
	{
		private string m_sListName;

		protected override Type ActionType
		{
			get
			{
				return typeof(Metalogix.SharePoint.Actions.Migration.CopyListPermissions);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operations should clear any existing role assignments prior to copying the source role assignments.")]
		public SwitchParameter ClearRoleAssignments
		{
			get
			{
				return this.Options.ClearRoleAssignments;
			}
			set
			{
				this.Options.ClearRoleAssignments = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for folders.")]
		public SwitchParameter CopyFolderPermissions
		{
			get
			{
				return this.Options.CopyItemPermissions;
			}
			set
			{
				this.Options.CopyItemPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for list item and documents.")]
		public SwitchParameter CopyItemPermissions
		{
			get
			{
				return this.Options.CopyItemPermissions;
			}
			set
			{
				this.Options.CopyItemPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for lists.")]
		public SwitchParameter CopyListPermissions
		{
			get
			{
				return this.Options.CopyListPermissions;
			}
			set
			{
				this.Options.CopyListPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy the permissions for the highest-level objects copied, regardless of inheritance.")]
		public SwitchParameter CopyRootPermissions
		{
			get
			{
				return this.Options.CopyRootPermissions;
			}
			set
			{
				this.Options.CopyRootPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="A comma-delimited list of list names to copy permissions for. If null, permissions will be copied for all matched lists.")]
		public string ListNames
		{
			get
			{
				return this.m_sListName;
			}
			set
			{
				this.m_sListName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operations should map role assignments strictly by name, skipping role assignments without a match.")]
		public SwitchParameter MapGroupsByName
		{
			get
			{
				return this.Options.MapRolesByName;
			}
			set
			{
				this.Options.MapRolesByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operations should map role assignments strictly by name, skipping role assignments without a match.")]
		public SwitchParameter MapRolesByName
		{
			get
			{
				return this.Options.MapRolesByName;
			}
			set
			{
				this.Options.MapRolesByName = value;
			}
		}

		private CopyPermissionsOptions Options
		{
			get
			{
				return base.Action.Options as CopyPermissionsOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the automatic permission level mappings should be overridden.")]
		public SwitchParameter OverrideRoleMappings
		{
			get
			{
				return this.Options.OverrideRoleMappings;
			}
			set
			{
				this.Options.OverrideRoleMappings = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a collection of name to name role mappings, as well as conditional logic used to determine which objects to apply them on.")]
		public ConditionalMappingCollection RoleAssignmentMappings
		{
			get
			{
				return this.Options.RoleAssignmentMappings;
			}
			set
			{
				this.Options.RoleAssignmentMappings = value;
			}
		}

		public CopyChildListPermissions()
		{
		}

		private void CopyListRoleAssignments(SPList sourceList, SPList targetList, JobHistoryDb jobList)
		{
			if (sourceList != null && targetList != null)
			{
				NodeCollection nodeCollection = new NodeCollection(new Node[] { sourceList });
				NodeCollection nodeCollection1 = new NodeCollection(new Node[] { targetList });
				if (jobList != null)
				{
					jobList.Jobs.Add(new Metalogix.Jobs.Job(base.Action, nodeCollection, nodeCollection1));
				}
				base.Action.Run(nodeCollection, nodeCollection1);
			}
		}

		protected override bool ProcessParameters()
		{
			if (!(base.Source is SPWeb))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source is not a web"), "ArgumentError", ErrorCategory.InvalidArgument, base.Source));
			}
			if (!(base.Target is SPWeb))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target is not a web"), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			try
			{
				if (base.Target == null)
				{
					base.Target = base.GetNodeFromLocation();
				}
				else if (base.Source == null)
				{
					base.Source = base.GetNodeFromLocation();
				}
				if (!this.ProcessParameters())
				{
					base.WriteError(new ErrorRecord(new ArgumentException("The parameters as defined cannot be processed for the action"), "ArgumentError", ErrorCategory.InvalidArgument, base.Action));
				}
				else
				{
					JobHistoryDb jobHistoryDb = null;
					if (!string.IsNullOrEmpty(base.JobDatabase))
					{
						this.m_jobList = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlServer, base.JobDatabase);
					}
					else if (base.JobFile != null)
					{
						jobHistoryDb = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, (base.JobFile.EndsWith(".lst") ? base.JobFile : string.Concat(base.JobFile, ".lst")));
					}
					SPWeb source = base.Source as SPWeb;
					SPWeb target = base.Target as SPWeb;
					CopyUsersAction copyUsersAction = new CopyUsersAction();
					copyUsersAction.SharePointOptions.SetFromOptions(base.Action.Options);
					if (jobHistoryDb != null)
					{
						jobHistoryDb.Jobs.Add(new Metalogix.Jobs.Job(copyUsersAction, this.SourceCollection, this.TargetCollection));
					}
					copyUsersAction.Run(source.SiteUsers, this.TargetCollection);
					CopyGroupsAction copyGroupsAction = new CopyGroupsAction();
					copyGroupsAction.SharePointOptions.SetFromOptions(base.Action.Options);
					if (jobHistoryDb != null)
					{
						jobHistoryDb.Jobs.Add(new Metalogix.Jobs.Job(copyGroupsAction, this.SourceCollection, this.TargetCollection));
					}
					copyGroupsAction.Run(source.Groups, this.TargetCollection);
					if (jobHistoryDb != null)
					{
						jobHistoryDb.Jobs.Add(new Metalogix.Jobs.Job(base.Action, this.SourceCollection, this.TargetCollection));
					}
					base.Action.OperationFinished += new ActionEventHandler(this.Write_Action_OperationFinished);
					if (this.ListNames == null)
					{
						foreach (SPList list in source.Lists)
						{
							this.CopyListRoleAssignments(list, target.Lists[list.Name], jobHistoryDb);
						}
					}
					else
					{
						string[] strArrays = this.ListNames.Trim().Split(new char[] { ',' });
						for (int i = 0; i < (int)strArrays.Length; i++)
						{
							string str = strArrays[i];
							this.CopyListRoleAssignments(source.Lists[str], target.Lists[str], jobHistoryDb);
						}
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.WriteError(new ErrorRecord(exception, exception.Message, ErrorCategory.InvalidOperation, this));
			}
		}

		protected override void Run()
		{
		}
	}
}