using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointAllSubsites", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.SubSites.ico")]
	[MenuText("2:Paste Site Content {0-Paste} > All Subsites...")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste all subsites")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SubActionTypes(new Type[] { typeof(PasteNavigationAction), typeof(CopySiteAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteAllSubSitesAction : PasteSiteBaseAction<PasteSiteOptions>
	{
		public PasteAllSubSitesAction()
		{
		}

		private void CopySubSites(SPWeb sourceWeb, SPWeb targetWeb, bool bIsCopyRoot)
		{
			PasteSiteAction pasteSiteAction = new PasteSiteAction();
			pasteSiteAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteSiteAction);
			List<TaskDefinition> taskDefinitions = new List<TaskDefinition>();
			pasteSiteAction.WebTransformerDefinition.BeginTransformation(pasteSiteAction, sourceWeb.SubWebs, targetWeb.SubWebs, pasteSiteAction.SharePointOptions.Transformers);
			foreach (SPWeb subWeb in sourceWeb.SubWebs)
			{
				if (base.CheckForAbort())
				{
					break;
				}
				if (subWeb.DisplayUrl == targetWeb.DisplayUrl)
				{
					continue;
				}
				TaskDefinition taskDefinition = pasteSiteAction.CopySite(subWeb, targetWeb, false);
				if (taskDefinition == null)
				{
					continue;
				}
				taskDefinitions.Add(taskDefinition);
			}
			if (!bIsCopyRoot)
			{
				sourceWeb.Collapse();
				targetWeb.Collapse();
			}
			base.ThreadManager.WaitForTasks(taskDefinitions);
			pasteSiteAction.WebTransformerDefinition.EndTransformation(pasteSiteAction, sourceWeb.SubWebs, targetWeb.SubWebs, pasteSiteAction.SharePointOptions.Transformers);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPWeb sPWeb in target)
			{
				try
				{
					Node[] nodeArray = new Node[] { sPWeb };
					this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
					foreach (SPWeb sPWeb1 in source)
					{
						base.InitializeAudienceMappings(sPWeb1, sPWeb);
						if (base.CheckForAbort())
						{
							break;
						}
						this.CopySubSites(sPWeb1, sPWeb, true);
						sPWeb1.Dispose();
					}
					if (base.SharePointOptions.RunNavigationStructureCopy)
					{
						PasteNavigationAction pasteNavigationAction = new PasteNavigationAction();
						pasteNavigationAction.SharePointOptions.CopyCurrentNavigation = base.SharePointOptions.CopyCurrentNavigation;
						pasteNavigationAction.SharePointOptions.CopyGlobalNavigation = base.SharePointOptions.CopyGlobalNavigation;
						pasteNavigationAction.SharePointOptions.Recursive = false;
						pasteNavigationAction.SharePointOptions.TaskCollection = base.SharePointOptions.TaskCollection;
						base.SubActions.Add(pasteNavigationAction);
						object[] sourceTargetNavCopyMap = new object[] { base.SourceTargetNavCopyMap, false };
						pasteNavigationAction.RunAsSubAction(sourceTargetNavCopyMap, new ActionContext(null, sPWeb), null);
					}
				}
				finally
				{
					sPWeb.Dispose();
				}
			}
			base.StartCommonWorkflowBufferedTasks();
			base.ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
			base.ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 3)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.CopySubSites(oParams[0] as SPWeb, oParams[1] as SPWeb, (bool)oParams[2]);
		}
	}
}