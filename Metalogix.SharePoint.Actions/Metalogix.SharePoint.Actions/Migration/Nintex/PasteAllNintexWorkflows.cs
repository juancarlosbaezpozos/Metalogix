using Metalogix;
using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Common;
using Metalogix.SharePoint.Common.Workflow2013;
using Metalogix.SharePoint.Nintex;
using Metalogix.SharePoint.Nintex.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Nintex
{
	[ActionConfigRequired(false)]
	[Image("Metalogix.SharePoint.Icons.Nintex.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint)]
	[MenuText("3:Paste Site Objects {0-Paste} > Paste Nintex Workflows...")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste Nintex Workflows")]
	[Shortcut(ShortcutAction.Paste)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPNintexWorkflowList))]
	[SubActionTypes(typeof(PasteNintexListWorkflow))]
	[SupportsThreeStateConfiguration(false)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class PasteAllNintexWorkflows : PasteAction<PasteNintexWorkflowOptions>
	{
		public PasteAllNintexWorkflows()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return false;
		}

		private SPList FindTargetList(SPNintexWorkflow workflow, SPWeb target)
		{
			Guid associatedListID = workflow.AssociatedListID;
			if (base.GuidMappings.ContainsKey(associatedListID))
			{
				associatedListID = base.GuidMappings[associatedListID];
			}
			return target.Lists.Cast<SPList>().FirstOrDefault<SPList>((SPList node) => new Guid(node.ID) == associatedListID);
		}

		private bool IsWorkflowExists(SPNintexWorkflow source, SPWeb target)
		{
			OperationReportingResult operationReportingResult = null;
			ISP2013WorkflowAdapter writer = target.Adapter.Writer as ISP2013WorkflowAdapter;
			if (writer != null)
			{
				SP2013WorkflowDefinition sP2013WorkflowDefinition = new SP2013WorkflowDefinition()
				{
					Name = source.DisplayName
				};
				string str = writer.DeleteSP2013Workflows(SP2013Utils.Serialize<SP2013WorkflowDefinition>(sP2013WorkflowDefinition));
				if (!string.IsNullOrEmpty(str))
				{
					operationReportingResult = new OperationReportingResult(str);
				}
			}
			if (operationReportingResult == null || !operationReportingResult.ErrorOccured)
			{
				return false;
			}
			this.LogOperationItem(source.WorkflowType, source.Name, source.Url, target.Url, ActionOperationStatus.Failed, "Unable to delete Nintex workflow", operationReportingResult.AllReportElementsAsString);
			return true;
		}

		private void LogMessage(string operation, string itemName, string source, string target, ActionOperationStatus status, string information, string details)
		{
			LogItem logItem = new LogItem(operation, itemName, source, target, status)
			{
				Information = information,
				Details = details
			};
			LogItem logItem1 = logItem;
			base.FireOperationStarted(logItem1);
			base.FireOperationFinished(logItem1);
		}

		private void LogOperationItem(NintexWorkflowType wfType, string itemName, string source, string target, ActionOperationStatus status, string information, string details)
		{
			string str = wfType.ToString();
			string str1 = itemName;
			if (wfType == NintexWorkflowType.GloballyReusable)
			{
				str = "Globally Reusable";
				str1 = "Globally Reusable";
			}
			string str2 = string.Format("Copy Nintex {0} Workflow", str);
			this.LogMessage(str2, str1, source, target, status, information, details);
		}

		private void MigrateListWorkflow(SPNintexWorkflow source, SPWeb target)
		{
			if (source == null)
			{
				return;
			}
			PasteNintexListWorkflow pasteNintexListWorkflow = new PasteNintexListWorkflow();
			pasteNintexListWorkflow.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteNintexListWorkflow);
			try
			{
				SPList sPList = this.FindTargetList(source, target);
				if (sPList != null)
				{
					object[] objArray = new object[] { source, sPList };
					pasteNintexListWorkflow.RunAsSubAction(objArray, new ActionContext(source, sPList), null);
				}
				else
				{
					string str = string.Format("Skipping list workflow {0} because the target associated list is not found.", source.Name);
					this.LogOperationItem(source.WorkflowType, source.Name, source.Url, target.Url, ActionOperationStatus.Skipped, str, string.Empty);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str1 = "Failed to migrate Nintex list workflows.";
				this.LogOperationItem(source.WorkflowType, source.Name, source.Url, target.Url, ActionOperationStatus.Failed, str1, exception.ToString());
			}
		}

		private void MigrateSiteWorkflow(SPNintexWorkflow source, SPWeb target)
		{
			if (source == null)
			{
				return;
			}
			try
			{
				if (!this.IsWorkflowExists(source, target))
				{
					PasteNintexSiteWorkflow pasteNintexSiteWorkflow = new PasteNintexSiteWorkflow();
					pasteNintexSiteWorkflow.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(pasteNintexSiteWorkflow);
					object[] objArray = new object[] { source, target };
					pasteNintexSiteWorkflow.RunAsSubAction(objArray, new ActionContext(source, target), null);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = "Failed to migrate Nintex Site workflows.";
				this.LogOperationItem(source.WorkflowType, source.Name, source.Url, target.Url, ActionOperationStatus.Failed, str, exception.ToString());
			}
		}

		private void MigrateWorkflows(SPNintexWorkflowList source, SPWeb target)
		{
			try
			{
				foreach (Node subFolder in source.SubFolders)
				{
					if (!base.CheckForAbort())
					{
						SPNintexWorkflow sPNintexWorkflow = subFolder as SPNintexWorkflow;
						if (sPNintexWorkflow == null)
						{
							continue;
						}
						switch (sPNintexWorkflow.WorkflowType)
						{
							case NintexWorkflowType.Site:
							{
								if (!base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations)
								{
									continue;
								}
								this.MigrateSiteWorkflow(sPNintexWorkflow, target);
								continue;
							}
							case NintexWorkflowType.List:
							{
								if (!base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations)
								{
									continue;
								}
								this.MigrateListWorkflow(sPNintexWorkflow, target);
								continue;
							}
							case NintexWorkflowType.Reusable:
							case NintexWorkflowType.GloballyReusable:
							{
								this.SkipReusableWorkflow(sPNintexWorkflow, target);
								continue;
							}
						}
						this.WarnNotSupportedWorkflow(sPNintexWorkflow, target);
					}
					else
					{
						return;
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = "Failed to migrate Nintex workflows.";
				this.LogMessage("Migrate Nintex Workflows", source.Title, source.Url, target.Url, ActionOperationStatus.Failed, str, exception.ToString());
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPNintexWorkflowList item = source[0] as SPNintexWorkflowList;
			this.MigrateWorkflows(item, target[0] as SPWeb);
		}

		protected override void RunOperation(object[] oParams)
		{
			SPNintexWorkflowList sPNintexWorkflowList = oParams[0] as SPNintexWorkflowList;
			this.MigrateWorkflows(sPNintexWorkflowList, oParams[1] as SPWeb);
		}

		private void SkipReusableWorkflow(SPNintexWorkflow source, SPWeb target)
		{
			if (source == null)
			{
				return;
			}
			string str = "Reusable Nintex workflows are not currently supported on SharePoint Online";
			if (source.WorkflowType == NintexWorkflowType.GloballyReusable)
			{
				str = string.Format("Globally {0} so all Globally Reusable Nintex workflows will be skipped during migration.", str);
			}
			this.LogOperationItem(source.WorkflowType, source.Name, source.Url, target.Url, ActionOperationStatus.Skipped, str, string.Empty);
		}

		private void WarnNotSupportedWorkflow(SPNintexWorkflow source, SPWeb target)
		{
			if (source == null)
			{
				return;
			}
			string str = string.Format("This workflow type '{0}' is not supported.", source.WorkflowType);
			this.LogOperationItem(source.WorkflowType, source.Name, source.Url, target.Url, ActionOperationStatus.Warning, str, string.Empty);
		}
	}
}