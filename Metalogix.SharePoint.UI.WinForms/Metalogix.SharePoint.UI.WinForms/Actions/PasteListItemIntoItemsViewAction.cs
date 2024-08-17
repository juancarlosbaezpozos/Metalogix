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
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Explorer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Actions
{
	[CmdletEnabled(true, "Copy-MLSharePointItem", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.UI.WinForms.Icons.Actions.Paste.ico")]
	[MenuText("Paste Selected Item... {0-Paste}")]
	[MenuTextPlural("Paste Selected Items... {0-Paste}", PluralCondition.MultipleSources)]
	[Name("Paste Selected Items")]
	[Shortcut(ShortcutAction.Paste)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPListItem), true)]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(ItemViewFolder))]
	public class PasteListItemIntoItemsViewAction : PasteListItemAction
	{
		public PasteListItemIntoItemsViewAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			Metalogix.SharePoint.ListType baseType;
			bool flag;
			bool flag1 = SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections);
			if (!flag1)
			{
				return false;
			}
			List<Metalogix.SharePoint.ListType> listTypes = new List<Metalogix.SharePoint.ListType>();
			if (!(sourceSelections[0] is SPListItem))
			{
				foreach (object sourceSelection in sourceSelections)
				{
					SPFolder sPFolder = sourceSelection as SPFolder;
					if (sPFolder == null)
					{
						continue;
					}
					Metalogix.SharePoint.ListType listType = sPFolder.ParentList.BaseType;
					if (listTypes.Contains(listType))
					{
						continue;
					}
					listTypes.Add(listType);
				}
			}
			else
			{
				listTypes.Add(((SPListItem)sourceSelections[0]).ParentList.BaseType);
			}
			List<Metalogix.SharePoint.ListType> listTypes1 = new List<Metalogix.SharePoint.ListType>();
			if (targetSelections[0] is SPFolder)
			{
				foreach (object targetSelection in targetSelections)
				{
					if (targetSelection is SPFolder)
					{
						continue;
					}
					ItemViewFolder itemViewFolder = targetSelection as ItemViewFolder;
					if (itemViewFolder == null)
					{
						continue;
					}
					SPFolder viewFolder = itemViewFolder.ViewFolder as SPFolder;
				}
			}
			List<Metalogix.SharePoint.ListType>.Enumerator enumerator = listTypes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Metalogix.SharePoint.ListType current = enumerator.Current;
					List<Metalogix.SharePoint.ListType>.Enumerator enumerator1 = listTypes1.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							if (current == enumerator1.Current)
							{
								continue;
							}
							flag = false;
							return flag;
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
				}
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
				Metalogix.SharePoint.ListType baseType1 = ((SPFolder)((ItemViewFolder)targetSelections[0]).ViewFolder).ParentList.BaseType;
				flag1 = (!flag1 ? false : baseType == baseType1);
				return flag1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public override ConfigurationResult Configure(ref IXMLAbleList source, ref IXMLAbleList target)
		{
			ConfigurationResult configurationResult;
			SPListItemCollection targetNodes = this.GetTargetNodes(source) as SPListItemCollection;
			if (targetNodes != null && targetNodes.ParentFolder != null)
			{
				SPFolder parentFolder = targetNodes.ParentFolder as SPFolder;
				if (parentFolder != null && parentFolder.IsOneNoteFolder)
				{
					FlatXtraMessageBox.Show("Cannot Copy: This action is not allowed for OneNote items. Only 'Copy Folder' / 'Paste Folder Content > All List Items and Folders...' action is allowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return ConfigurationResult.Cancel;
				}
			}
			NodeCollection nodeCollection = this.GetTargetNodes(target);
			if (nodeCollection.Count == 0)
			{
				return ConfigurationResult.Cancel;
			}
			NodeCollection nodeCollection1 = XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(source);
			NodeCollection nodeCollection2 = XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(nodeCollection);
			ActionConfigContext actionConfigContext = new ActionConfigContext(this, new ActionContext(nodeCollection1, nodeCollection2));
			Metalogix.Actions.ActionOptions actionOption = actionConfigContext.Action.Options.Clone();
			do
			{
				configurationResult = this.OpenDialog(source, nodeCollection);
			}
			while (configurationResult == ConfigurationResult.Switch);
			if (configurationResult == ConfigurationResult.Cancel)
			{
				actionConfigContext.Action.Options = actionOption;
			}
			PasteListItemOptions options = actionConfigContext.Action.Options as PasteListItemOptions;
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
			NodeCollection nodeCollection;
			if (target is ItemViewFolderList)
			{
				nodeCollection = ((ItemViewFolderList)target).ToNodeCollection();
			}
			else if (!(target is ItemViewFolder))
			{
				nodeCollection = target as NodeCollection;
			}
			else
			{
				Node[] viewFolderNode = new Node[] { ((ItemViewFolder)target).ViewFolderNode };
				nodeCollection = new NodeCollection(viewFolderNode);
			}
			return nodeCollection ?? new NodeCollection();
		}

		private ConfigurationResult OpenDialog(IXMLAbleList source, NodeCollection targetNodes)
		{
			bool isClientOM = false;
			if (targetNodes != null && targetNodes.Count > 0)
			{
				isClientOM = (targetNodes[0] as SPNode).Adapter.IsClientOM;
			}
			NodeCollection nodeCollection = XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(source);
			NodeCollection nodeCollection1 = XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(targetNodes);
			ActionConfigContext actionConfigContext = new ActionConfigContext(this, new ActionContext(nodeCollection, nodeCollection1));
			ScopableLeftNavigableTabsForm listItemDialog = CopyListItemHelper.GetListItemDialog(nodeCollection, nodeCollection1, isClientOM, actionConfigContext);
			listItemDialog.ShowDialog();
			return listItemDialog.ConfigurationResult;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			NodeCollection targetNodes = this.GetTargetNodes(target);
			if (targetNodes.Count == 0)
			{
				return;
			}
			this.InitializeSharePointCopy(source, targetNodes, base.SharePointOptions.ForceRefresh);
			if (base.CheckForAbort())
			{
				return;
			}
			PasteListItemAction pasteListItemAction = new PasteListItemAction()
			{
				SharePointOptions = base.SharePointOptions
			};
			pasteListItemAction.SharePointOptions.CopySubFolders = false;
			if (!base.CheckForAbort())
			{
				base.SubActions.Add(pasteListItemAction);
				foreach (SPFolder targetNode in targetNodes)
				{
					object[] objArray = new object[] { source, targetNode, null, true };
					pasteListItemAction.RunAsSubAction(objArray, new ActionContext(source, targetNodes), null);
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