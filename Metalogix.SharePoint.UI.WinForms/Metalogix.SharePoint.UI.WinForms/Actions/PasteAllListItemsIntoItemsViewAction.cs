using DevExpress.XtraEditors;
using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Explorer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Actions
{
	[CmdletEnabled(true, "Copy-MLAllListItems", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.UI.WinForms.Icons.Actions.Paste.ico")]
	[MenuText("Paste All List Items and Folders... ")]
	[Name("Paste All List Items")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPFolder), true)]
	[SubActionTypes(typeof(PasteAllListItemsAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(ItemViewFolder), true)]
	public class PasteAllListItemsIntoItemsViewAction : PasteAllListItemsAction
	{
		public PasteAllListItemsIntoItemsViewAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			Metalogix.SharePoint.ListType baseType;
			bool flag = SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections);
			if (flag)
			{
				if (!(sourceSelections[0] is SPListItem))
				{
					if (!(sourceSelections[0] is SPFolder))
					{
						return false;
					}
					baseType = ((SPFolder)sourceSelections[0]).ParentList.BaseType;
				}
				else
				{
					baseType = ((SPListItem)sourceSelections[0]).ParentList.BaseType;
				}
				Metalogix.SharePoint.ListType listType = ((SPFolder)((ItemViewFolder)targetSelections[0]).ViewFolder).ParentList.BaseType;
				flag = (!flag ? false : baseType == listType);
			}
			return flag;
		}

		public override ConfigurationResult Configure(ref IXMLAbleList source, ref IXMLAbleList target)
		{
			ConfigurationResult configurationResult;
			NodeCollection targetNodes = this.GetTargetNodes(target);
			if (targetNodes.Count == 0)
			{
				return ConfigurationResult.Cancel;
			}
			if (!SPUIUtils.NotifyDisabledSubactions(this, targetNodes))
			{
				return ConfigurationResult.Cancel;
			}
			SPFolder item = null;
			if (targetNodes[0] != null && targetNodes[0] is SPFolder)
			{
				item = (SPFolder)targetNodes[0];
			}
			NodeCollection nodeCollection = XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(source);
			NodeCollection nodeCollection1 = new NodeCollection(new Node[] { item });
			ActionConfigContext actionConfigContext = new ActionConfigContext(this, new ActionContext(nodeCollection, nodeCollection1));
			Metalogix.Actions.ActionOptions actionOption = actionConfigContext.Action.Options.Clone();
			do
			{
				configurationResult = this.OpenDialog(actionConfigContext);
			}
			while (configurationResult == ConfigurationResult.Switch);
			if (configurationResult == ConfigurationResult.Cancel)
			{
				actionConfigContext.Action.Options = actionOption;
			}
			PasteFolderOptions options = actionConfigContext.Action.Options as PasteFolderOptions;
			if (options != null)
			{
				options.IsFromAdvancedMode = null;
			}
			return configurationResult;
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			IXMLAbleList nodeCollection = targetSelections;
			if (targetSelections is ItemViewFolderList)
			{
				nodeCollection = ((ItemViewFolderList)targetSelections).ToNodeCollection();
			}
			return base.EnabledOn(sourceSelections, nodeCollection);
		}

		public override string GetPowershellCommand(string sourceXML, string targetXML, string jobsDb, string jobID = null)
		{
			targetXML = targetXML.Replace("Metalogix.UI.WinForms.Explorer.ItemViewFolderList, Metalogix.UI.WinForms,", "Metalogix.Explorer.NodeCollection, Metalogix.Explorer,");
			targetXML = targetXML.Replace("ItemViewFolderList", "NodeCollection");
			return base.GetPowershellCommand(sourceXML, targetXML, jobsDb, jobID);
		}

		private NodeCollection GetTargetNodes(IXMLAbleList target)
		{
			if (target is ItemViewFolderList)
			{
				return ((ItemViewFolderList)target).ToNodeCollection();
			}
			if (!(target is ItemViewFolder))
			{
				return new NodeCollection();
			}
			return new NodeCollection(new Node[] { ((ItemViewFolder)target).ViewFolderNode });
		}

		private ConfigurationResult OpenDialog(ActionConfigContext context)
		{
			ScopableLeftNavigableTabsForm copyFolderActionDialogBasicView;
			NodeCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			if (PasteActionUtils.CollectionContainsMultipleLists(targetsAsNodeCollection))
			{
				context.GetActionOptions<PasteFolderOptions>().ToggleOptionsForMultiSelectCase();
			}
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				SPNode item = targetsAsNodeCollection[0] as SPNode;
				if (item != null)
				{
					isClientOM = item.Adapter.IsClientOM;
				}
			}
			bool flag = SPUIUtils.ShouldShowAdvancedMode(context.ActionOptions);
			if (!flag)
			{
				copyFolderActionDialogBasicView = new CopyFolderActionDialogBasicView();
			}
			else
			{
				copyFolderActionDialogBasicView = new CopyFolderActionDialog(isClientOM);
			}
			copyFolderActionDialogBasicView.SourceNodes = sourcesAsNodeCollection;
			copyFolderActionDialogBasicView.TargetNodes = targetsAsNodeCollection;
			copyFolderActionDialogBasicView.Context = context.ActionContext;
			if (context.ActionContext.Targets.Count > 1)
			{
				copyFolderActionDialogBasicView.MultiSelectUI = true;
			}
			if (!flag)
			{
				copyFolderActionDialogBasicView.Action.JobID = context.Action.JobID;
				((CopyFolderActionDialogBasicView)copyFolderActionDialogBasicView).Options = context.GetActionOptions<PasteFolderOptions>();
			}
			else
			{
				((CopyFolderActionDialog)copyFolderActionDialogBasicView).Options = context.GetActionOptions<PasteFolderOptions>();
				copyFolderActionDialogBasicView.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
				copyFolderActionDialogBasicView.MinimumSize = new Size(792, copyFolderActionDialogBasicView.Height);
			}
			copyFolderActionDialogBasicView.ShowDialog();
			return copyFolderActionDialogBasicView.ConfigurationResult;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPFolder targetNode in this.GetTargetNodes(target))
			{
				try
				{
					Node[] nodeArray = new Node[] { targetNode };
					this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
					if (!base.CheckForAbort())
					{
						PasteAllListItemsAction pasteAllListItemsAction = new PasteAllListItemsAction();
						pasteAllListItemsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
						pasteAllListItemsAction.SharePointOptions.CopySubFolders = false;
						if (!base.CheckForAbort())
						{
							base.SubActions.Add(pasteAllListItemsAction);
							object[] objArray = new object[] { source, targetNode };
							pasteAllListItemsAction.RunAsSubAction(objArray, new ActionContext(null, targetNode.ParentList.ParentWeb), null);
						}
						targetNode.UpdateCurrentNode();
					}
					else
					{
						return;
					}
				}
				finally
				{
					targetNode.Dispose();
				}
			}
		}

		protected override void RunAfterAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.RunAfterAction(source, this.GetTargetNodes(target));
		}

		protected override void RunBeforeAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.RunBeforeAction(source, this.GetTargetNodes(target));
		}
	}
}