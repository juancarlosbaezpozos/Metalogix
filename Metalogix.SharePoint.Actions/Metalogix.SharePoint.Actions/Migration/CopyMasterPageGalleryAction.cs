using Metalogix;
using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointMasterPageGallery", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.MasterPageGallery.ico")]
	[LaunchAsJob(true)]
	[MandatoryTransformers(new Type[] { typeof(PageLayoutGuidMapper) })]
	[MenuText("3:Paste Site Objects {0-Paste} > Master Page Gallery...")]
	[Name("Paste Master Page Gallery")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb), true)]
	[SubActionTypes(new Type[] { typeof(CopyRoleAssignmentsAction), typeof(CopyMasterPagesAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb), true)]
	[UsesStickySettings(true)]
	public class CopyMasterPageGalleryAction : PasteAction<PasteMasterPageGalleryOptions>
	{
		private TransformerDefinition<SPListItem, CopyMasterPageGalleryAction, SPListItemCollection, SPListItemCollection> listTransformerDefinition = new TransformerDefinition<SPListItem, CopyMasterPageGalleryAction, SPListItemCollection, SPListItemCollection>("SharePoint Master Page Gallery", false);

		public CopyMasterPageGalleryAction()
		{
		}

		private void CopyMasterPageFolderPermissionsTaskDelegate(object[] oParams)
		{
			SPFolder sPFolder = oParams[0] as SPFolder;
			SPFolder sPFolder1 = oParams[0] as SPFolder;
			if (sPFolder.HasUniquePermissions)
			{
				CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
				copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(copyRoleAssignmentsAction);
				object[] objArray = new object[] { sPFolder, sPFolder1, true };
				copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPFolder.ParentList.ParentWeb, sPFolder1.ParentList.ParentWeb), null);
			}
			base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyRoleAssignmentsAction).Name, sPFolder1.DirName, "/", sPFolder1.Name), false, false);
		}

		private void CopyMasterPageGaleryMetadata(SPList sourceGallery, SPList targetGallery)
		{
			if (base.SharePointOptions.OverwriteLists || base.SharePointOptions.UpdateLists || (base.SharePointOptions.UpdateListOptionsBitField & 1) > 0)
			{
				LogItem logItem = new LogItem("Copying Master Page Gallery Settings", sourceGallery.Name, sourceGallery.DisplayUrl, targetGallery.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceGallery.XML);
					XmlNode xmlNodes = XmlUtility.StringToXmlNode(targetGallery.XML);
					if (xmlNode.Attributes["EnableModeration"] != null)
					{
						((XmlElement)xmlNodes).SetAttribute("EnableModeration", xmlNode.Attributes["EnableModeration"].Value);
					}
					if (xmlNode.Attributes["EnableVersioning"] != null)
					{
						((XmlElement)xmlNodes).SetAttribute("EnableVersioning", xmlNode.Attributes["EnableVersioning"].Value);
					}
					if (bool.Parse(xmlNodes.Attributes["EnableModeration"].Value))
					{
						if (xmlNode.Attributes["MajorVersionLimit"] != null)
						{
							((XmlElement)xmlNodes).SetAttribute("MajorVersionLimit", xmlNode.Attributes["MajorVersionLimit"].Value);
						}
						if (xmlNode.Attributes["EnableMinorVersions"] != null)
						{
							((XmlElement)xmlNodes).SetAttribute("EnableMinorVersions", xmlNode.Attributes["EnableMinorVersions"].Value);
						}
						if (xmlNode.Attributes["MajorWithMinorVersionsLimit"] != null && xmlNode.Attributes["EnableModeration"] != null && (bool.Parse(xmlNodes.Attributes["EnableModeration"].Value) || bool.Parse(xmlNodes.Attributes["EnableMinorVersions"].Value)))
						{
							((XmlElement)xmlNodes).SetAttribute("MajorWithMinorVersionsLimit", xmlNode.Attributes["MajorWithMinorVersionsLimit"].Value);
						}
					}
					targetGallery.UpdateList(xmlNodes.OuterXml, false, false);
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
				base.FireOperationFinished(logItem);
			}
		}

		private void CopyMasterPageGalleryPermissions(SPList sourceGallery, SPList targetGallery)
		{
			Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
			string keyFor = base.PermissionsKeyFormatter.GetKeyFor(targetGallery.ParentWeb);
			object[] objArray = new object[] { sourceGallery, targetGallery };
			threadManager.QueueBufferedTask(keyFor, objArray, new ThreadedOperationDelegate(this.CopyMasterPageGalleryPermissionsTaskDelegate));
		}

		private void CopyMasterPageGalleryPermissionsTaskDelegate(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			SPList sPList = oParams[0] as SPList;
			SPList sPList1 = oParams[1] as SPList;
			if (sPList.HasUniquePermissions)
			{
				LogItem logItem = new LogItem("Copying master page gallery permissions", sPList.ParentList.Name, sPList.ParentList.DisplayUrl, sPList1.ParentList.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					base.SubActions.Add(copyRoleAssignmentsAction);
					if (!base.CheckForAbort())
					{
						copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
						object[] objArray = new object[] { sPList, sPList1, true };
						copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPList.ParentWeb, sPList1.ParentWeb), null);
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						return;
					}
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
				base.FireOperationFinished(logItem);
			}
			base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyRoleAssignmentsAction).Name, sPList1.DirName, "/", sPList1.Name), false, false);
		}

		private void CopyMasterPages(SPFolder sourceGallery, SPFolder targetGallery, bool bIsCopyRoot)
		{
			if (sourceGallery == null)
			{
				throw new Exception("Source master page gallery cannot be null");
			}
			if (targetGallery == null)
			{
				throw new Exception("Target master page gallery cannot be null");
			}
			SPListItemCollection items = sourceGallery.GetItems(false, ListItemQueryType.ListItem, null);
			if (base.SharePointOptions.CorrectingLinks)
			{
				base.LinkCorrector.PopulateForItemCopy(items, targetGallery);
			}
			this.listTransformerDefinition.BeginTransformation(this, sourceGallery.Items, targetGallery.Items, this.Options.Transformers);
			CopyMasterPagesAction copyMasterPagesAction = new CopyMasterPagesAction();
			copyMasterPagesAction.ActionOptions.SetFromOptions(this.ActionOptions);
			copyMasterPagesAction.ActionOptions.UpdateMasterPagesForUseBySpecificUIVersion = false;
			if (base.SharePointOptions.ItemCopyingMode == ListItemCopyMode.Overwrite)
			{
				string str = UrlUtils.EnsureLeadingSlash(targetGallery.ParentList.ParentWeb.MasterPageUrl);
				bool flag = (!targetGallery.Adapter.SharePointVersion.IsSharePoint2013OrLater ? false : targetGallery.Adapter.IsClientOM);
				List<SPListItem> sPListItems = new List<SPListItem>();
				foreach (SPListItem item in items)
				{
					using (SPListItem matchingItem = targetGallery.Items.GetMatchingItem(item, item.FileLeafRef, item.ParentRelativePath))
					{
						bool isDBGhosted = false;
						if (!(matchingItem != null) || !item.Name.EndsWith(".aspx") || !(item.Created.ToString("g") == item.Modified.ToString("g")))
						{
							if (item.Adapter.IsDB)
							{
								isDBGhosted = item.IsDBGhosted;
							}
							if (matchingItem != null && (item.Adapter.SharePointVersion.IsSharePoint2013 || item.Adapter.SharePointVersion.IsSharePoint2016) && flag)
							{
								string fileLeafRef = matchingItem.FileLeafRef;
								string fileLeafRef1 = item.FileLeafRef;
								if (fileLeafRef.EndsWith(".js", StringComparison.OrdinalIgnoreCase) && targetGallery.Items.GetMatchingItem(item, string.Concat(fileLeafRef1.Remove(fileLeafRef1.Length - 3), ".html"), item.ParentRelativePath) != null)
								{
									isDBGhosted = true;
								}
							}
							if (matchingItem != null && !isDBGhosted && !UrlUtils.Equal(UrlUtils.EnsureLeadingSlash(matchingItem.ServerRelativeUrl), str))
							{
								try
								{
									targetGallery.Items.DeleteItem(matchingItem);
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									LogItem logItem = new LogItem("Delete masterpage on target", matchingItem.Name, "", targetGallery.DisplayUrl, ActionOperationStatus.Skipped)
									{
										Information = exception.Message
									};
									base.FireOperationStarted(logItem);
									base.FireOperationFinished(logItem);
								}
							}
						}
						else
						{
							sPListItems.Add(item);
						}
					}
				}
				if (sPListItems.Count > 0)
				{
					this.SkipOOBPageLayouts(sourceGallery, targetGallery, sPListItems);
				}
			}
			base.SubActions.Add(copyMasterPagesAction);
			object[] objArray = new object[] { sourceGallery.Items, targetGallery.Items };
			copyMasterPagesAction.RunAsSubAction(objArray, new ActionContext(sourceGallery.ParentList.ParentWeb, targetGallery.ParentList.ParentWeb), null);
			SPFolderCollection subFolders = targetGallery.SubFolders;
			if (base.SharePointOptions.CopySubFolders)
			{
				SPFolderCollection sPFolderCollection = sourceGallery.SubFolders;
				sPFolderCollection.FetchData();
				foreach (SPFolder subFolder in sPFolderCollection)
				{
					if (!base.CheckForAbort())
					{
						LogItem xML = null;
						SPFolder sPFolder = null;
						try
						{
							try
							{
								xML = new LogItem("Adding Folder", subFolder.Name, subFolder.DisplayUrl, targetGallery.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(xML);
								sPFolder = subFolders.AddFolder(subFolder.XML, new AddFolderOptions(), AddFolderMode.Comprehensive);
								Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
								string str1 = string.Concat(typeof(CopyRoleAssignmentsAction).Name, sPFolder.DirName);
								object[] objArray1 = new object[] { subFolder, sPFolder };
								threadManager.QueueBufferedTask(str1, objArray1, new ThreadedOperationDelegate(this.CopyMasterPageFolderPermissionsTaskDelegate));
								if (!base.SharePointOptions.CheckResults)
								{
									xML.Status = ActionOperationStatus.Completed;
								}
								else
								{
									base.CompareNodes(subFolder, sPFolder, xML);
								}
								if (base.SharePointOptions.Verbose)
								{
									xML.SourceContent = subFolder.XML;
									xML.TargetContent = sPFolder.XML;
								}
							}
							catch (Exception exception3)
							{
								Exception exception2 = exception3;
								if (xML != null)
								{
									xML.Exception = exception2;
									xML.Status = ActionOperationStatus.Failed;
									xML.Information = string.Concat("Exception thrown: ", exception2.Message);
									xML.Details = exception2.StackTrace;
									xML.SourceContent = subFolder.XML;
								}
							}
						}
						finally
						{
							base.FireOperationFinished(xML);
						}
						if (xML == null || xML.Status != ActionOperationStatus.Completed)
						{
							continue;
						}
						CopyMasterPageGalleryAction copyMasterPageGalleryAction = new CopyMasterPageGalleryAction()
						{
							SharePointOptions = base.SharePointOptions
						};
						base.SubActions.Add(copyMasterPageGalleryAction);
						copyMasterPageGalleryAction.CopyMasterPages(subFolder, sPFolder, false);
					}
					else
					{
						return;
					}
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPList masterPageGallery = ((SPWeb)source[0]).MasterPageGallery;
			SPList sPList = ((SPWeb)target[0]).MasterPageGallery;
			base.InitializeAudienceMappings(masterPageGallery, sPList);
			this.CopyMasterPageGaleryMetadata(masterPageGallery, sPList);
			this.CopyMasterPageGalleryPermissions(masterPageGallery, sPList);
			this.CopyMasterPages(masterPageGallery, sPList, false);
			base.ThreadManager.SetBufferedTasks(base.PermissionsKeyFormatter.GetKeyFor(sPList.ParentWeb), false, false);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			bool flag = false;
			if ((int)oParams.Length >= 3 && oParams[2] != null && oParams[2] is bool)
			{
				flag = (bool)oParams[2];
			}
			this.CopyMasterPageGaleryMetadata((SPList)oParams[0], (SPList)oParams[1]);
			this.CopyMasterPageGalleryPermissions((SPList)oParams[0], (SPList)oParams[1]);
			this.CopyMasterPages(oParams[0] as SPFolder, oParams[1] as SPFolder, flag);
		}

		private void SkipOOBPageLayouts(SPFolder sourceGallery, SPFolder targetGallery, List<SPListItem> itemsToBeRemoved)
		{
			foreach (SPListItem sPListItem in itemsToBeRemoved)
			{
				try
				{
					sourceGallery.Items.RemoveByID(sPListItem.ID);
					LogItem logItem = new LogItem("Adding Page Layout", sPListItem.Name, sourceGallery.DisplayUrl, targetGallery.DisplayUrl, ActionOperationStatus.Skipped)
					{
						Information = "Skipped page layout migration because its already exists at the target."
					};
					LogItem logItem1 = logItem;
					base.FireOperationStarted(logItem1);
					base.FireOperationFinished(logItem1);
				}
				catch (Exception exception)
				{
					Trace.WriteLine(string.Format("Error occurred while skipping page layout migration. Error : '{0}'", exception));
				}
			}
		}
	}
}