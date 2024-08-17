using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration.Nintex;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Nintex;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Analyzable(true)]
	[MandatoryTransformers(new Type[] { typeof(SiteFeatureEnforcer) })]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SubActionTypes(new Type[] { typeof(PasteNavigationAction), typeof(PasteRolesAction), typeof(PasteAllSubSitesAction), typeof(CopyRoleAssignmentsAction), typeof(PasteSiteLists), typeof(CopyContentTypesAction), typeof(CopyWebPartPageAction), typeof(CopyWorkflowAssociationsAction), typeof(CopySiteColumnsAction), typeof(CopyMasterPagesAction) })]
	[SupportsThreeStateConfiguration(true)]
	public abstract class PasteSiteBaseAction<T> : PasteAction<T>, IPasteWebAction
	where T : PasteSiteOptions
	{
		private Dictionary<SPWeb, SPWeb> m_sourceTargetNavCopyMap;

		protected TransformerDefinition<SPListItem, PasteSiteAction, SPListItemCollection, SPListItemCollection> masterPageTransformerDefinition;

		protected TransformerDefinition<SPWeb, PasteSiteAction, SPWebCollection, SPWebCollection> webTransformerDefinition;

		public Dictionary<SPWeb, SPWeb> SourceTargetNavCopyMap
		{
			get
			{
				return this.m_sourceTargetNavCopyMap;
			}
			set
			{
				this.m_sourceTargetNavCopyMap = value;
			}
		}

		internal TransformerDefinition<SPWeb, PasteSiteAction, SPWebCollection, SPWebCollection> WebTransformerDefinition
		{
			get
			{
				return this.webTransformerDefinition;
			}
		}

		protected PasteSiteBaseAction()
		{
		}

		protected override void ConnectSubaction(Metalogix.Actions.Action subAction)
		{
			base.ConnectSubaction(subAction);
			if (subAction != null && typeof(IPasteWebAction).IsAssignableFrom(subAction.GetType()))
			{
				((IPasteWebAction)subAction).SourceTargetNavCopyMap = this.SourceTargetNavCopyMap;
			}
		}

		protected void CopyContentTypeWorkflowAssociationsTaskDelegate(object[] oParams)
		{
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			try
			{
				if (!base.CheckForAbort())
				{
					CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
					copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyWorkflowAssociationsAction);
					foreach (SPContentType contentType in sPWeb.ContentTypes)
					{
						if (!base.CheckForAbort())
						{
							SPContentType contentTypeByName = sPWeb1.ContentTypes.GetContentTypeByName(contentType.Name);
							if (contentTypeByName == null || contentType.ParentCollection.ParentWeb.RootSite.DisplayUrl == contentTypeByName.ParentCollection.ParentWeb.RootSite.DisplayUrl && contentType.ContentTypeID == contentTypeByName.ContentTypeID)
							{
								continue;
							}
							object[] objArray = new object[] { contentType, sPWeb1.ContentTypes.GetContentTypeByName(contentType.Name), null };
							copyWorkflowAssociationsAction.RunAsSubAction(objArray, new ActionContext(sPWeb, sPWeb1), null);
						}
						else
						{
							return;
						}
					}
				}
			}
			finally
			{
				sPWeb.Dispose();
				sPWeb1.Dispose();
			}
		}

		protected void CopyMasterPageSetting(SPWeb sourceWeb, SPWeb targetWeb, string sWebXML)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			try
			{
				string targetMasterPage = this.GetTargetMasterPage(sourceWeb, targetWeb, sourceWeb.MasterPageUrl);
				string str = this.GetTargetMasterPage(sourceWeb, targetWeb, sourceWeb.CustomMasterPageUrl);
				if (!string.IsNullOrEmpty(targetMasterPage) && !string.IsNullOrEmpty(str))
				{
					TransformationTask transformationTask = new TransformationTask();
					transformationTask.ChangeOperations.Add("MasterPage", targetMasterPage);
					transformationTask.ChangeOperations.Add("CustomMasterPage", str);
					sWebXML = transformationTask.PerformTransformation(sWebXML);
					targetWeb.Adapter.Writer.SetMasterPage(sWebXML);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem("Setting Master Page", sourceWeb.MasterPageUrl, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
		}

		protected void CopyNintexWorkflowsTaskDelegate(object[] oParams)
		{
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			try
			{
				if (!base.CheckForAbort())
				{
					if (sPWeb != null)
					{
						Node node1 = sPWeb.Lists.FirstOrDefault<Node>((Node node) => node is SPNintexWorkflowList);
						if (node1 != null)
						{
							PasteAllNintexWorkflows pasteAllNintexWorkflow = new PasteAllNintexWorkflows();
							pasteAllNintexWorkflow.SharePointOptions.SetFromOptions(base.SharePointOptions);
							base.SubActions.Add(pasteAllNintexWorkflow);
							object[] objArray = new object[] { node1, sPWeb1 };
							pasteAllNintexWorkflow.RunAsSubAction(objArray, new ActionContext(node1, sPWeb1), null);
						}
					}
				}
			}
			finally
			{
				if (sPWeb != null)
				{
					sPWeb.Dispose();
				}
				if (sPWeb1 != null)
				{
					sPWeb1.Dispose();
				}
			}
		}

		private void CopyOtherContent(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			PasteCustomContent pasteCustomContent = new PasteCustomContent();
			pasteCustomContent.Options.SetFromOptions(this.Options);
			base.SubActions.Add(pasteCustomContent);
			pasteCustomContent.RunAsSubAction(oParams, new ActionContext(oParams[0] as SPWeb, oParams[1] as SPWeb), null);
		}

		protected internal void CopySiteNavigationStructure(SPNode target)
		{
			if (base.SharePointOptions.RunNavigationStructureCopy)
			{
				PasteNavigationAction pasteNavigationAction = new PasteNavigationAction();
				pasteNavigationAction.SharePointOptions.CopyCurrentNavigation = base.SharePointOptions.CopyCurrentNavigation;
				pasteNavigationAction.SharePointOptions.CopyGlobalNavigation = base.SharePointOptions.CopyGlobalNavigation;
				pasteNavigationAction.SharePointOptions.Recursive = false;
				pasteNavigationAction.SharePointOptions.TaskCollection = base.SharePointOptions.TaskCollection;
				base.SubActions.Add(pasteNavigationAction);
				object[] sourceTargetNavCopyMap = new object[] { this.SourceTargetNavCopyMap, false };
				pasteNavigationAction.RunAsSubAction(sourceTargetNavCopyMap, new ActionContext(null, target), null);
			}
		}

		protected void CopyWebPermissionsTaskDelegate(object[] oParams)
		{
			bool flag;
			if (base.CheckForAbort())
			{
				return;
			}
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			bool flag1 = (bool)oParams[2];
			bool flag2 = (bool)oParams[3];
			if (flag1 && (base.SharePointOptions.CopySitePermissions || base.SharePointOptions.CopyListPermissions || base.SharePointOptions.CopyItemPermissions || base.SharePointOptions.CopyFolderPermissions))
			{
				flag = true;
			}
			else if (flag1)
			{
				flag = false;
			}
			else if ((!base.SharePointOptions.CopySitePermissions || !base.SharePointOptions.UpdateSites || (base.SharePointOptions.UpdateSiteOptionsBitField & 4) <= 0) && (!base.SharePointOptions.CopyListPermissions || !base.SharePointOptions.UpdateLists || (base.SharePointOptions.UpdateListOptionsBitField & 8) <= 0))
			{
				flag = (!base.SharePointOptions.CopyItemPermissions || !base.SharePointOptions.UpdateItems ? false : (base.SharePointOptions.UpdateItemOptionsBitField & 2) > 0);
			}
			else
			{
				flag = true;
			}
			if (flag && (sPWeb.HasUniquePermissions || flag2 && base.SharePointOptions.CopyRootPermissions))
			{
				LogItem logItem = new LogItem("Copying site permissions", sPWeb.Name, sPWeb.DisplayUrl, sPWeb1.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					if (!base.CheckForAbort())
					{
						base.SubActions.Add(copyRoleAssignmentsAction);
						object[] objArray = new object[] { sPWeb, sPWeb1, !flag1 };
						copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPWeb, sPWeb1), null);
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
			base.ThreadManager.SetBufferedTasks(base.PermissionsKeyFormatter.GetKeyFor(sPWeb1), false, false);
			base.ThreadManager.SetBufferedTasks(base.GetAccessRequestListCopyBufferKey(sPWeb1), false, false);
		}

		protected void CopyWebWorkflowAssociationsTaskDelegate(object[] oParams)
		{
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			try
			{
				if (!base.CheckForAbort())
				{
					CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
					copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyWorkflowAssociationsAction);
					object[] objArray = new object[] { sPWeb, sPWeb1 };
					copyWorkflowAssociationsAction.RunAsSubAction(objArray, new ActionContext(sPWeb, sPWeb1), null);
				}
			}
			finally
			{
				sPWeb.Dispose();
				sPWeb1.Dispose();
			}
		}

		protected void CopyWelcomePageWebPartsTaskDelegate(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			SPWeb sPWeb = (SPWeb)oParams[0];
			SPWeb sPWeb1 = (SPWeb)oParams[1];
			CopyWebPartPageAction copyWebPartPageAction = new CopyWebPartPageAction();
			copyWebPartPageAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(copyWebPartPageAction);
			object[] objArray = new object[] { sPWeb, sPWeb1 };
			copyWebPartPageAction.RunAsSubAction(objArray, new ActionContext(sPWeb, sPWeb1), null);
			sPWeb.Dispose();
			sPWeb1.Dispose();
		}

		public override bool GetCollectionsViolateSourceTargetRestrictions(IXMLAbleList source, IXMLAbleList target, out string failureMessage)
		{
			bool flag;
			if (base.GetCollectionsViolateSourceTargetRestrictions(source, target, out failureMessage))
			{
				return true;
			}
			if (source == null || target == null)
			{
				return false;
			}
			Type type = typeof(SPWeb);
			foreach (object obj in source)
			{
				if (obj != null && type.IsAssignableFrom(obj.GetType()))
				{
					continue;
				}
				flag = false;
				return flag;
			}
			foreach (object obj1 in target)
			{
				if (obj1 != null && type.IsAssignableFrom(obj1.GetType()))
				{
					continue;
				}
				flag = false;
				return flag;
			}
			IEnumerator enumerator = target.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPWeb current = (SPWeb)enumerator.Current;
					IEnumerator enumerator1 = source.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							SPWeb sPWeb = (SPWeb)enumerator1.Current;
							if (!UrlUtils.Equal(current.DisplayUrl, sPWeb.DisplayUrl) || !string.Equals(current.ID, sPWeb.ID, StringComparison.OrdinalIgnoreCase))
							{
								if (!current.IsChildOf(sPWeb))
								{
									continue;
								}
								failureMessage = Metalogix.SharePoint.Actions.Properties.Resources.CannotRunTargetIsChildSiteOfSource;
								flag = true;
								return flag;
							}
							else
							{
								failureMessage = Metalogix.Actions.Properties.Resources.CannotRunTargetIsSameAsSource;
								flag = true;
								return flag;
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator1 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				return false;
			}
			finally
			{
				IDisposable disposable1 = enumerator as IDisposable;
				if (disposable1 != null)
				{
					disposable1.Dispose();
				}
			}
			return flag;
		}

		private bool GetMasterPageUrls(SPWeb sourceWeb, SPWeb targetWeb, string pageUrl, out string sourceMasterPageUrl, out string targetPageFolderUrl, out string targetPageName)
		{
			sourceMasterPageUrl = null;
			targetPageFolderUrl = null;
			targetPageName = null;
			StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, UrlUtils.EnsureLeadingSlash(sourceWeb.RootSite.MasterPageGallery.ServerRelativeUrl));
			StandardizedUrl standardizedUrl1 = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, pageUrl);
			StandardizedUrl standardizedUrl2 = StandardizedUrl.StandardizeUrl(targetWeb.Adapter, UrlUtils.EnsureLeadingSlash(targetWeb.RootSite.MasterPageGallery.ServerRelativeUrl));
			sourceMasterPageUrl = standardizedUrl1.ServerRelative;
			int num = sourceMasterPageUrl.IndexOf("/_catalogs/masterpage/");
			if (num < 0)
			{
				return false;
			}
			string str = sourceMasterPageUrl.Substring(num + "/_catalogs/masterpage/".Length).TrimStart(new char[] { '/' });
			sourceMasterPageUrl = string.Concat(standardizedUrl.ServerRelative, "/", str);
			string serverRelative = standardizedUrl2.ServerRelative;
			targetPageFolderUrl = standardizedUrl1.ServerRelative;
			targetPageFolderUrl = string.Concat(standardizedUrl2.ServerRelative, '/', str);
			int num1 = targetPageFolderUrl.LastIndexOf('/');
			if (num1 < 0)
			{
				return false;
			}
			targetPageName = targetPageFolderUrl.Substring(num1 + 1, targetPageFolderUrl.Length - (num1 + 1) - ".master".Length);
			targetPageFolderUrl = targetPageFolderUrl.Substring(0, num1);
			return true;
		}

		private SPListItem GetSourceMasterPage(SPWeb sourceWeb, string sourceMasterPageUrl)
		{
			SPListItemCollection items = sourceWeb.RootSite.MasterPageGallery.GetItems(true, ListItemQueryType.ListItem);
			return items.GetItemByServerRelativeUrl(sourceMasterPageUrl);
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(this.webTransformerDefinition);
			return supportedDefinitions;
		}

		private string GetTargetMasterPage(SPWeb sourceWeb, SPWeb targetWeb, string MasterPageUrl)
		{
			string str;
			string str1;
			string str2;
			SPListItem sPListItem;
			string str3 = null;
			if (this.GetMasterPageUrls(sourceWeb, targetWeb, MasterPageUrl, out str, out str1, out str2))
			{
				SPListItem sourceMasterPage = this.GetSourceMasterPage(sourceWeb, str);
				if (sourceMasterPage != null && this.GetTargetMasterPage(sourceWeb, targetWeb, sourceMasterPage, str1, str2, out sPListItem))
				{
					str3 = UrlUtils.EnsureLeadingSlash(sPListItem.FileRef);
				}
			}
			return str3;
		}

		private bool GetTargetMasterPage(SPWeb sourceWeb, SPWeb targetWeb, SPListItem sourceMasterPage, string targetFolderUrl, string masterPageName, out SPListItem targetMasterPage)
		{
			SPListItemCollection items = targetWeb.RootSite.MasterPageGallery.GetItems(true, ListItemQueryType.ListItem);
			string[] strArrays = new string[] { targetFolderUrl, string.Concat(masterPageName, ".master") };
			targetMasterPage = items.GetItemByServerRelativeUrl(UrlUtils.ConcatUrls(strArrays));
			Metalogix.DataStructures.Generic.Set<string> webUIVersionAsSet = MasterPageMigrationUtils.GetWebUIVersionAsSet(targetWeb);
			if (targetMasterPage != null && MasterPageMigrationUtils.GetMasterPageUIVersion(targetMasterPage).IntersectsWith(webUIVersionAsSet))
			{
				return true;
			}
			string targetPageName = MasterPageMigrationUtils.GetTargetPageName(masterPageName, MasterPageMigrationUtils.GetMasterPageUIVersion(sourceMasterPage), webUIVersionAsSet, sourceWeb.Adapter.SharePointVersion, targetWeb.Adapter.SharePointVersion);
			if (string.Equals(targetPageName, masterPageName))
			{
				return false;
			}
			string[] strArrays1 = new string[] { targetFolderUrl, string.Concat(targetPageName, ".master") };
			targetMasterPage = items.GetItemByServerRelativeUrl(UrlUtils.ConcatUrls(strArrays1));
			if (targetMasterPage != null && MasterPageMigrationUtils.GetMasterPageUIVersion(targetMasterPage).IntersectsWith(webUIVersionAsSet))
			{
				return true;
			}
			return false;
		}

		protected override void InitializeSharePointCopy(IXMLAbleList source, IXMLAbleList target, bool bRefresh)
		{
			base.InitializeSharePointCopy(source, target, bRefresh);
			if (this.SourceTargetNavCopyMap != null)
			{
				this.SourceTargetNavCopyMap.Clear();
			}
		}

		private bool IsSourceAdapterOM(SPWeb sourceWeb)
		{
			if (sourceWeb.Adapter.IsClientOM || sourceWeb.Adapter.IsDB)
			{
				return false;
			}
			return !sourceWeb.Adapter.IsNws;
		}

		private string MapAssociateGroups(SPWeb targetWeb, string srcAssociateGroups)
		{
			string str = "";
			if (!string.IsNullOrEmpty(srcAssociateGroups))
			{
				string str1 = srcAssociateGroups.Trim();
				char[] chrArray = new char[] { ';' };
				string[] strArrays = str1.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				XmlNode xML = targetWeb.ToXML();
				string str2 = (xML.Attributes["AssociateGroups"] != null ? xML.Attributes["AssociateGroups"].Value : string.Empty);
				string[] strArrays1 = strArrays;
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
					string item = strArrays1[i].Trim();
					if (base.PrincipalMappings.ContainsKey(item))
					{
						item = base.PrincipalMappings[item];
					}
					string str3 = str2.Trim();
					char[] chrArray1 = new char[] { ';' };
					if (!str3.Split(chrArray1).Contains(item))
					{
						str = string.Concat(str, item, ';');
					}
				}
			}
			return str.TrimEnd(new char[] { ';' });
		}

		protected string MapReferencedGroups(string sSourceXML, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string value;
			string str;
			string value1;
			string str1;
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sSourceXML);
			if (xmlNode.Attributes["OwnerGroup"] != null)
			{
				value = xmlNode.Attributes["OwnerGroup"].Value;
			}
			else
			{
				value = null;
			}
			string str2 = value;
			if (xmlNode.Attributes["MemberGroup"] != null)
			{
				str = xmlNode.Attributes["MemberGroup"].Value;
			}
			else
			{
				str = null;
			}
			string str3 = str;
			if (xmlNode.Attributes["VisitorGroup"] != null)
			{
				value1 = xmlNode.Attributes["VisitorGroup"].Value;
			}
			else
			{
				value1 = null;
			}
			string str4 = value1;
			if (xmlNode.Attributes["AssociateGroups"] != null)
			{
				str1 = xmlNode.Attributes["AssociateGroups"].Value;
			}
			else
			{
				str1 = null;
			}
			string str5 = str1;
			string[] strArrays = new string[] { str2, str3, str4 };
			List<SPGroup> sPGroups = new List<SPGroup>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str6 = strArrays1[i];
				if (!string.IsNullOrEmpty(str6) && !base.PrincipalMappings.ContainsKey(str6))
				{
					sPGroups.Add((SPGroup)sourceWeb.Groups[str6]);
				}
			}
			if (str5 != null)
			{
				string str7 = str5.Trim();
				char[] chrArray = new char[] { ';' };
				string[] strArrays2 = str7.TrimEnd(chrArray).Split(new char[] { ';' });
				for (int j = 0; j < (int)strArrays2.Length; j++)
				{
					string str8 = strArrays2[j];
					if (!string.IsNullOrEmpty(str8) && !base.PrincipalMappings.ContainsKey(str8))
					{
						SPGroup item = (SPGroup)sourceWeb.Groups[str8];
						if (item != null)
						{
							sPGroups.Add(item);
						}
					}
				}
			}
			base.EnsurePrincipalExistence(null, sPGroups.ToArray(), targetWeb, null, sourceWeb);
			if (!string.IsNullOrEmpty(str2) && base.PrincipalMappings.ContainsKey(str2))
			{
				xmlNode.Attributes["OwnerGroup"].Value = base.PrincipalMappings[str2];
			}
			if (!string.IsNullOrEmpty(str3) && base.PrincipalMappings.ContainsKey(str3))
			{
				xmlNode.Attributes["MemberGroup"].Value = base.PrincipalMappings[str3];
			}
			if (!string.IsNullOrEmpty(str4) && base.PrincipalMappings.ContainsKey(str4))
			{
				xmlNode.Attributes["VisitorGroup"].Value = base.PrincipalMappings[str4];
			}
			if (!string.IsNullOrEmpty(str5))
			{
				xmlNode.Attributes["AssociateGroups"].Value = this.MapAssociateGroups(targetWeb, str5);
			}
			return xmlNode.OuterXml;
		}

		protected string MapWebTemplate(string sSourceXML, string sourceTemplateName, SPWebTemplateCollection targetTemplates, bool bIsCopyRoot)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sSourceXML);
			if (bIsCopyRoot)
			{
				if (base.SharePointOptions.ChangeWebTemplate)
				{
					SPWebTemplate item = targetTemplates[base.SharePointOptions.WebTemplateName];
					xmlDocument.DocumentElement.Attributes["WebTemplateID"].Value = item.ID.ToString();
					xmlDocument.DocumentElement.Attributes["WebTemplateConfig"].Value = item.Config.ToString();
					if (xmlDocument.DocumentElement.Attributes["WebTemplateName"] != null)
					{
						xmlDocument.DocumentElement.Attributes["WebTemplateName"].Value = item.Name.ToString();
					}
				}
			}
			else if (base.SharePointOptions.MapChildWebTemplates && base.SharePointOptions.WebTemplateMappingTable.ContainsKey(sourceTemplateName))
			{
				SPWebTemplate sPWebTemplate = targetTemplates[base.SharePointOptions.WebTemplateMappingTable[sourceTemplateName]];
				if (sPWebTemplate != null)
				{
					xmlDocument.DocumentElement.Attributes["WebTemplateID"].Value = sPWebTemplate.ID.ToString();
					xmlDocument.DocumentElement.Attributes["WebTemplateConfig"].Value = sPWebTemplate.Config.ToString();
					if (xmlDocument.DocumentElement.Attributes["WebTemplateName"] != null)
					{
						xmlDocument.DocumentElement.Attributes["WebTemplateName"].Value = sPWebTemplate.Name.ToString();
					}
				}
			}
			return xmlDocument.OuterXml;
		}

		protected void MigrateCustomContent(SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (base.SharePointOptions.CopyCustomContent)
			{
				Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
				object[] objArray = new object[] { sourceWeb, targetWeb };
				TaskDefinition taskDefinition = threadManager.QueueTask(objArray, new ThreadedOperationDelegate(this.CopyOtherContent));
				base.ThreadManager.WaitForTask(taskDefinition);
			}
		}

		protected void MigrateNintexWorkflows(SPWeb sourceWeb, SPWeb targetWeb)
		{
			if ((!this.IsSourceAdapterOM(sourceWeb) || !targetWeb.Adapter.SharePointVersion.IsSharePointOnline ? false : sourceWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater) && (base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations))
			{
				Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
				string name = typeof(PasteAllNintexWorkflows).Name;
				object[] objArray = new object[] { sourceWeb, targetWeb };
				threadManager.QueueBufferedTask(name, objArray, new ThreadedOperationDelegate(this.CopyNintexWorkflowsTaskDelegate));
			}
		}

		protected void SetWelcomePage(SPWeb sourceWeb, SPWeb targetWeb)
		{
			string str;
			LogItem logItem = null;
			try
			{
				try
				{
					int length = sourceWeb.ServerRelativeUrl.Length + (sourceWeb.ServerRelativeUrl.Length == 1 ? 0 : 1);
					if (sourceWeb.WelcomePageUrl != null)
					{
						str = sourceWeb.WelcomePageUrl.Remove(0, length);
					}
					else
					{
						str = null;
					}
					string str1 = str;
					SPWebPartPage welcomePage = (new SPWebPartPage()).GetWelcomePage(sourceWeb, this);
					bool flag = (!targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater || !targetWeb.Template.Name.Equals("ENTERWIKI#0", StringComparison.OrdinalIgnoreCase) ? false : welcomePage.FileLeafRef.Equals("Home.aspx", StringComparison.OrdinalIgnoreCase));
					if (welcomePage != null && !string.IsNullOrEmpty(str1) && (!str1.Equals("default.aspx", StringComparison.OrdinalIgnoreCase) || sourceWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater) && !flag)
					{
						SPList parentList = welcomePage.ParentList;
						T sharePointOptions = base.SharePointOptions;
						SPList matchingList = MigrationUtils.GetMatchingList(parentList, targetWeb, sharePointOptions.TaskCollection);
						if (matchingList != null && !welcomePage.ParentList.Name.Equals(matchingList.Name, StringComparison.OrdinalIgnoreCase))
						{
							str1 = str1.ToLower().Replace(welcomePage.ParentList.Name.ToLower(), matchingList.Name);
						}
						targetWeb.SetWelcomePage(str1);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem = new LogItem("Setting welcome page issue", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					logItem.Exception = exception;
					logItem.Status = ActionOperationStatus.Warning;
				}
			}
			finally
			{
				if (logItem != null)
				{
					base.FireOperationFinished(logItem);
				}
			}
		}
	}
}