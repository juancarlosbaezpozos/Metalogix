using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Blocker;
using Metalogix.Actions.StatusProviders;
using Metalogix.Core;
using Metalogix.Core.OperationLog;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Actions.Migration.HealthScore;
using Metalogix.SharePoint.Actions.Migration.Nintex;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Migration.StatusProviders;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.Workflow;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Metalogix.Actions.AllowsSameSourceTarget(false)]
	[CompletionDetailsOrderProvider(typeof(MigrationDetailOrderProvider))]
	[RequiresWriteAccess(true)]
	[StatusLabelProvider(typeof(MigrationStatusLabelProvider))]
	[StatusSummaryProvider(typeof(MigrationStatusSummaryProvider))]
	[SubActionTypes(new Type[] { typeof(CopyUsersAction), typeof(CopyGroupsAction) })]
	[UsesStickySettings(true)]
	public abstract class PasteAction<T> : SharePointAction<T>, IPasteAction
	where T : SharePointActionOptions
	{
		protected const string _CALENDAR_OVERLAY_BUFFER_KEY = "CalendarOverlayLinkCorrection";

		protected const string COPY_DOCUMENT_TEMPLATES_FOR_CONTENT_TYPES = "CopyDocumentTemplatesforContentTypes";

		protected const string _COPY_LIST_ITEMS_COMPLETED_KEY = "CopyListItemsCompleted";

		protected const string _COPY_DOCSET_VERSION_HISTORY_KEY = "CopyDocumentSetVersionHistory";

		public bool IsValidationSettingDisablingRequired;

		private HealthCheckTimer _healthCheckTimer;

		private Metalogix.SharePoint.Migration.LinkCorrector m_LinkCorrector;

		protected ThreadSafeDictionary<string, string> m_principalMappings=new ThreadSafeDictionary<string, string>();

		protected Dictionary<string, string> m_workflowMappings;

		protected Dictionary<string, string> m_workflowItemMappings;

		protected Dictionary<string, Dictionary<string, string>> m_workflowViewMappings;

		protected Dictionary<Guid, Guid> m_guidMappings;

		private Dictionary<SPListItem, SPListItem> m_WebPartPagesNotCopied;

		private IActionBlocker _healthScoreBlocker;

		private readonly static string AUDIENCE_FIELD_TYPE;

		private readonly static string AUDIENCE_WEBPART_PROPERTY_NAME;

		private Dictionary<string, string> m_audienceIDMappings;

		public Dictionary<string, string> AudienceIDMappings
		{
			get
			{
				return this.m_audienceIDMappings;
			}
			set
			{
				this.m_audienceIDMappings = value;
			}
		}

		public Dictionary<Guid, Guid> GuidMappings
		{
			get
			{
				if (this.m_guidMappings == null)
				{
					this.m_guidMappings = new Dictionary<Guid, Guid>();
					foreach (KeyValuePair<Guid, Guid> globalGuidMapping in SPGlobalMappings.GlobalGuidMappings)
					{
						this.m_guidMappings.Add(globalGuidMapping.Key, globalGuidMapping.Value);
					}
				}
				return this.m_guidMappings;
			}
			set
			{
				this.m_guidMappings = value;
			}
		}

		public override string LicensingDescriptor
		{
			get
			{
				return Resources.Bytes_Copied;
			}
		}

		public Metalogix.SharePoint.Migration.LinkCorrector LinkCorrector
		{
			get
			{
				if (this.m_LinkCorrector == null)
				{
					this.m_LinkCorrector = new Metalogix.SharePoint.Migration.LinkCorrector();
					foreach (KeyValuePair<string, string> globalUrlMapping in SPGlobalMappings.GlobalUrlMappings)
					{
						this.m_LinkCorrector.AddUserSpecifiedMapping(globalUrlMapping.Key, globalUrlMapping.Value, null, null);
					}
				}
				return this.m_LinkCorrector;
			}
			set
			{
				this.m_LinkCorrector = value;
			}
		}

		protected IPermissionsKeyFormatter PermissionsKeyFormatter
		{
			get
			{
				return new PermissionsBufferedTaskKeyFormatter();
			}
		}

		public ThreadSafeDictionary<string, string> PrincipalMappings
		{
			get
			{
				return this.m_principalMappings;
			}
			set
			{
				this.m_principalMappings = value;
			}
		}

		protected override IThreadingStrategy ThreadingStrategy
		{
			get
			{
				return StaticThreadingStrategy.Instance;
			}
		}

		public string ValidationSettingsXml
		{
			get;
			set;
		}

		public Dictionary<SPListItem, SPListItem> WebPartPagesNotCopiedAtItemsLevel
		{
			get
			{
				if (this.m_WebPartPagesNotCopied == null)
				{
					this.m_WebPartPagesNotCopied = new Dictionary<SPListItem, SPListItem>();
				}
				return this.m_WebPartPagesNotCopied;
			}
		}

		public Dictionary<string, string> WorkflowItemMappings
		{
			get
			{
				if (this.m_workflowItemMappings == null)
				{
					this.m_workflowItemMappings = new Dictionary<string, string>();
				}
				return this.m_workflowItemMappings;
			}
			set
			{
				this.m_workflowItemMappings = value;
			}
		}

		public Dictionary<string, string> WorkflowMappings
		{
			get
			{
				if (this.m_workflowMappings == null)
				{
					this.m_workflowMappings = new Dictionary<string, string>();
				}
				return this.m_workflowMappings;
			}
			set
			{
				this.m_workflowMappings = value;
			}
		}

		public Dictionary<string, Dictionary<string, string>> WorkflowViewMappings
		{
			get
			{
				if (this.m_workflowViewMappings == null)
				{
					this.m_workflowViewMappings = new Dictionary<string, Dictionary<string, string>>();
				}
				return this.m_workflowViewMappings;
			}
			set
			{
				this.m_workflowViewMappings = value;
			}
		}

		static PasteAction()
		{
			PasteAction<T>.AUDIENCE_FIELD_TYPE = "TargetTo";
			PasteAction<T>.AUDIENCE_WEBPART_PROPERTY_NAME = "IsIncludedFilter";
		}

		protected PasteAction()
		{
		}

		private void AddAdditionalTelemetryInfo(IXMLAbleList target)
		{
			try
			{
				SPNode item = target[0] as SPNode;
				IDictionary<string, string> extendedTelemetryData = base.ExtendedTelemetryData;
				string str = string.Concat(this.Name, "_IsIncrementalMigration");
				T sharePointOptions = base.SharePointOptions;
				bool flag = sharePointOptions.MigrationMode.Equals(MigrationMode.Incremental);
				extendedTelemetryData.Add(str, flag.ToString());
				if (item != null && item.Adapter.IsClientOM)
				{
					Dictionary<string, string> dictionary = base.ConvertOptionsXmlToDictionary(base.SharePointOptions.ToXML());
					if (dictionary != null && Convert.ToBoolean(dictionary.GetValue("UseAzureOffice365Upload", "")))
					{
						string str1 = "Azure";
						if (string.IsNullOrEmpty(SharePointConfigurationVariables.UploadManagerAzureStorageConnectionString) && Convert.ToBoolean(dictionary.GetValue("EncryptAzureMigrationJobs", "")))
						{
							str1 = "SPO Container";
						}
						base.ExtendedTelemetryData.Add(string.Concat(this.Name, "_MigrationPipeline"), str1);
					}
				}
				if (this.Options.TelemetryLogs != null && this.Options.TelemetryLogs.Count > 0)
				{
					foreach (KeyValuePair<string, string> telemetryLog in this.Options.TelemetryLogs)
					{
						base.ExtendedTelemetryData.Add(string.Concat(this.Name, string.Format("_{0}", telemetryLog.Key)), telemetryLog.Value);
					}
					this.Options.TelemetryLogs.Clear();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Logging.LogMessageToMetalogixGlobalLogFile(string.Format("Error occured while adding additional telemetry information for action '{0}' , Error: '{1}'", this.Name, exception));
			}
		}

		public void AddGuidMappings(string sSourceGuid, string sTargetGuid)
		{
			if (string.IsNullOrEmpty(sSourceGuid) || string.IsNullOrEmpty(sTargetGuid))
			{
				return;
			}
			this.AddGuidMappings(new Guid(sSourceGuid), new Guid(sTargetGuid));
		}

		public void AddGuidMappings(Guid sourceGuid, Guid targetGuid)
		{
			lock (this.GuidMappings)
			{
				if (!this.GuidMappings.ContainsKey(sourceGuid))
				{
					this.GuidMappings.Add(sourceGuid, targetGuid);
				}
				else
				{
					this.GuidMappings[sourceGuid] = targetGuid;
				}
			}
		}

		protected void ApplyNewContentType(SPList sourceList, SPList targetList, PasteListItemOptions options)
		{
			if (options.ContentTypeApplicationObjects != null)
			{
				List<ContentTypeApplicationOptionsCollection> contentTypeApplicationOptionsCollections = new List<ContentTypeApplicationOptionsCollection>();
				foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in options.ContentTypeApplicationObjects)
				{
					if (!contentTypeApplicationObject.AppliesTo(sourceList))
					{
						continue;
					}
					contentTypeApplicationOptionsCollections.Add(contentTypeApplicationObject);
					break;
				}
				if (contentTypeApplicationOptionsCollections.Count > 0)
				{
					bool flag = true;
					try
					{
						SPContentTypeCollection contentTypes = targetList.ContentTypes;
						SPContentTypeCollection sPContentTypeCollections = targetList.ParentWeb.ContentTypes;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						flag = false;
						LogItem logItem = new LogItem("Initializing Content Types", targetList.Name, sourceList.DisplayUrl, targetList.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Failed;
						base.FireOperationFinished(logItem);
					}
					if (flag)
					{
						CopyContentTypesAction copyContentTypesAction = new CopyContentTypesAction();
						copyContentTypesAction.Options.SetFromOptions(this.Options);
						base.SubActions.Add(copyContentTypesAction);
						object[] objArray = new object[] { targetList, contentTypeApplicationOptionsCollections[0] };
						copyContentTypesAction.RunAsSubAction("ApplyNewContentTypes", objArray, new ActionContext(null, targetList.ParentWeb));
					}
				}
			}
		}

		internal static bool CheckPreservingItems(SPList sourceList, PasteListItemOptions options)
		{
			if (options.ItemCopyingMode != ListItemCopyMode.Preserve)
			{
				return false;
			}
			if (sourceList.IsDocumentLibrary)
			{
				return true;
			}
			return options.PreserveItemIDs;
		}

		protected override void CleanUp()
		{
			if (base.SharePointOptions.PersistMappings)
			{
				if (this.m_guidMappings != null)
				{
					CommonSerializableTable<Guid, Guid> commonSerializableTable = new CommonSerializableTable<Guid, Guid>(this.GuidMappings.Count);
					foreach (KeyValuePair<Guid, Guid> mGuidMapping in this.m_guidMappings)
					{
						commonSerializableTable.Add(mGuidMapping.Key, mGuidMapping.Value);
					}
					SPGlobalMappings.GlobalGuidMappings = commonSerializableTable;
				}
				if (this.LinkCorrector != null)
				{
					SPGlobalMappings.GlobalUrlMappings = this.LinkCorrector.GetMappings(true, true);
				}
				SPGlobalMappings.Save();
			}
			base.CleanUp();
			if (this.PrincipalMappings != null)
			{
				this.PrincipalMappings.Clear();
			}
			if (this.AudienceIDMappings != null)
			{
				this.AudienceIDMappings.Clear();
			}
			if (this.LinkCorrector != null)
			{
				this.LinkCorrector.ClearMappings();
			}
			this.m_guidMappings = null;
		}

		public Dictionary<Guid, Guid> CloneGuidMappings()
		{
			Dictionary<Guid, Guid> guids;
			lock (this.GuidMappings)
			{
				guids = new Dictionary<Guid, Guid>(this.GuidMappings);
			}
			return guids;
		}

		protected void CompareNodes(Metalogix.DataStructures.IComparable sourceComparable, Metalogix.DataStructures.IComparable targetComparable, LogItem results)
		{
			if (!base.SharePointOptions.CheckResults)
			{
				results.Status = ActionOperationStatus.Completed;
				return;
			}
			DifferenceLog differenceLogs = new DifferenceLog();
			if (!sourceComparable.IsEqual(targetComparable, differenceLogs, base.SharePointOptions.CompareOptions))
			{
				results.Status = ActionOperationStatus.Different;
			}
			else
			{
				results.Status = ActionOperationStatus.Completed;
			}
			if (differenceLogs.ToString() != "")
			{
				results.Information = differenceLogs.ToStringFormat();
			}
		}

		protected override void ConnectSubaction(Metalogix.Actions.Action subAction)
		{
			base.ConnectSubaction(subAction);
			if (subAction != null && typeof(IPasteAction).IsAssignableFrom(subAction.GetType()))
			{
				((IPasteAction)subAction).PrincipalMappings = this.PrincipalMappings;
				((IPasteAction)subAction).AudienceIDMappings = this.AudienceIDMappings;
				((IPasteAction)subAction).LinkCorrector = this.LinkCorrector;
				((IPasteAction)subAction).GuidMappings = this.GuidMappings;
				((IPasteAction)subAction).WorkflowMappings = this.WorkflowMappings;
				((IPasteAction)subAction).WorkflowItemMappings = this.WorkflowItemMappings;
				((IPasteAction)subAction).WorkflowViewMappings = this.WorkflowViewMappings;
			}
		}

		private void CopyAccessRequestList(object[] parameters)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			if (parameters == null || (int)parameters.Length != 3)
			{
				throw new Exception("Cannot trigger access request list copy. Incorrect number of parameters.");
			}
			SPWeb sPWeb = parameters[0] as SPWeb;
			SPWeb sPWeb1 = parameters[1] as SPWeb;
			PasteSiteOptions pasteSiteOption = parameters[2] as PasteSiteOptions;
			if (sPWeb == null || sPWeb1 == null)
			{
				throw new Exception("Cannot trigger access request list copy. Parameters did not contain source and target webs.");
			}
			CopyAccessRequestListAction copyAccessRequestListAction = new CopyAccessRequestListAction();
			copyAccessRequestListAction.Options.SetFromOptions(pasteSiteOption);
			base.SubActions.Add(copyAccessRequestListAction);
			object[] objArray = new object[] { sPWeb, sPWeb1 };
			copyAccessRequestListAction.RunAsSubAction(objArray, new ActionContext(sPWeb, sPWeb1), null);
		}

		private void CopyViewPageWebParts(object[] parameters)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = null;
			try
			{
				if (parameters == null || (int)parameters.Length != 5)
				{
					throw new Exception("Cannot trigger view page web part copy. Incorrect number of parameters.");
				}
				SPView sPView = parameters[0] as SPView;
				SPView sPView1 = parameters[1] as SPView;
				SPList sPList = parameters[2] as SPList;
				SPList sPList1 = parameters[3] as SPList;
				WebPartOptions webPartOption = parameters[4] as WebPartOptions;
				if (sPView == null || sPView1 == null || sPList == null || sPList1 == null)
				{
					throw new Exception("Cannot trigger view page web part copy. Parameters did not contain source and target views and lists.");
				}
				logItem = new LogItem("Copying View Page Web Parts", sPView.DisplayName, sPList.Url, sPList1.Url, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction();
				copyWebPartsAction.SharePointOptions.SetFromOptions(webPartOption);
				SPWebPartPage sPWebPartPage = new SPWebPartPage(sPView, sPList, this);
				SPWebPartPage sPWebPartPage1 = new SPWebPartPage(sPView1, sPList1, this);
				if (sPWebPartPage1.WebParts.Count > 1)
				{
					if (webPartOption.ExistingWebPartsAction == ExistingWebPartsProtocol.Delete)
					{
						int count = sPWebPartPage1.WebParts.Count;
						for (int i = 1; i < count; i++)
						{
							SPWebPart item = sPWebPartPage1.WebParts[1] as SPWebPart;
							string innerText = XmlUtility.StringToXmlNode(item.Xml).SelectSingleNode("./*[local-name()='ID']").InnerText;
							char[] chrArray = new char[] { 'g', '\u005F' };
							string str = innerText.TrimStart(chrArray).Replace('\u005F', '-');
							sPWebPartPage1.DeleteWebPart(str);
						}
					}
					else if (webPartOption.ExistingWebPartsAction == ExistingWebPartsProtocol.Close)
					{
						sPWebPartPage1.CloseAllWebParts();
					}
				}
				if (sPWebPartPage.WebParts.Count > 1)
				{
					SPWebPart viewWebPart = this.GetViewWebPart(sPWebPartPage);
					if (viewWebPart != null)
					{
						sPWebPartPage.DeleteWebPart(viewWebPart.Id);
					}
					base.SubActions.Add(copyWebPartsAction);
					object[] objArray = new object[] { sPWebPartPage, sPWebPartPage1, logItem };
					copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(sPList.ParentWeb, sPList1.ParentWeb), null);
				}
				if (logItem.Status != ActionOperationStatus.Failed)
				{
					logItem.Status = ActionOperationStatus.Completed;
				}
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (logItem == null)
				{
					logItem = new LogItem("Copying web parts on view page", "", "", "", ActionOperationStatus.Failed);
				}
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
		}

		private HealthScoreBlocker CreateHealthScoreBlocker(IXMLAbleList source, IXMLAbleList target)
		{
			string fullPath = Path.GetFullPath("serverhealth.txt");
			if (this.ShouldUseFileServerHealthScore(fullPath))
			{
				return HealthScoreUtils.CreateFileHealthScoreBlocker(fullPath);
			}
			return HealthScoreUtils.CreateSiteHealthScoreBlocker(new ActionContext(source, target));
		}

		public void DisableValidationSettings(SPList sourceList, SPFolder targetFolder, bool isValidationSettingDisablingRequired)
		{
			if (sourceList.Adapter.SharePointVersion.IsSharePoint2010OrLater && targetFolder.Adapter.IsClientOM && isValidationSettingDisablingRequired)
			{
				string d = targetFolder.ParentList.ID;
				string str = targetFolder.ParentList.ParentWeb.Adapter.Writer.DisableValidationSettings(d);
				OperationReportingResult operationReportingResult = new OperationReportingResult(str);
				if (!string.IsNullOrEmpty(operationReportingResult.ObjectXml))
				{
					LogItem logItem = new LogItem("Disable Validation Settings", sourceList.Name, sourceList.DisplayUrl, targetFolder.ParentList.ParentWeb.DisplayUrl, ActionOperationStatus.Completed);
					if (!operationReportingResult.ErrorOccured)
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(operationReportingResult.ObjectXml);
						XmlNode firstChild = xmlDocument.FirstChild;
						XmlNodeList xmlNodeLists = firstChild.SelectNodes(".//Field");
						if (xmlNodeLists != null && xmlNodeLists.Count > 0)
						{
							this.ValidationSettingsXml = firstChild.OuterXml;
						}
					}
					else
					{
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Information = operationReportingResult.GetAllErrorsAsString;
					}
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
			}
		}

		protected override void DisconnectSubaction(Metalogix.Actions.Action subAction)
		{
			base.DisconnectSubaction(subAction);
		    var action = subAction as IPasteAction;
		    if (action != null)
			{
                if(System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                //((IPasteAction)subAction).PrincipalMappings = new ThreadSafeDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			    action.PrincipalMappings = new ThreadSafeDictionary<string, string>();
                action.AudienceIDMappings = new Dictionary<string, string>();
				action.LinkCorrector = null;
				action.GuidMappings = null;
				action.WorkflowMappings = null;
				action.WorkflowItemMappings = null;
				action.WorkflowViewMappings = null;
			}
		}

		public void EnableValidationSettings(SPList sourceList, SPWeb targetWeb)
		{
			if (!string.IsNullOrEmpty(this.ValidationSettingsXml) && sourceList.Adapter.SharePointVersion.IsSharePoint2010OrLater && targetWeb.Adapter.IsClientOM)
			{
				LogItem logItem = new LogItem("Enable Validation Settings", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Completed);
				try
				{
					try
					{
						string str = targetWeb.Adapter.Writer.EnableValidationSettings(this.ValidationSettingsXml);
						OperationReportingResult operationReportingResult = new OperationReportingResult(str);
						if (operationReportingResult.ErrorOccured)
						{
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Information = operationReportingResult.GetAllErrorsAsString;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Exception = exception;
					}
				}
				finally
				{
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
			}
		}

		protected void EnsureAudiencedGroupExistence(List<string> sourceGroupNames, SPNode source, SPNode target)
		{
			SPWeb parentWeb = null;
			if (typeof(SPWeb).IsAssignableFrom(source.GetType()))
			{
				parentWeb = (SPWeb)source;
			}
			else if (typeof(SPFolder).IsAssignableFrom(source.GetType()))
			{
				parentWeb = ((SPFolder)source).ParentList.ParentWeb;
			}
			else if (typeof(SPListItem).IsAssignableFrom(target.GetType()))
			{
				parentWeb = ((SPListItem)source).ParentList.ParentWeb;
			}
			SPWeb sPWeb = null;
			if (typeof(SPWeb).IsAssignableFrom(target.GetType()))
			{
				sPWeb = (SPWeb)target;
			}
			else if (typeof(SPFolder).IsAssignableFrom(target.GetType()))
			{
				sPWeb = ((SPFolder)target).ParentList.ParentWeb;
			}
			else if (typeof(SPListItem).IsAssignableFrom(target.GetType()))
			{
				sPWeb = ((SPListItem)target).ParentList.ParentWeb;
			}
			List<SPGroup> sPGroups = new List<SPGroup>();
			List<SPUser> sPUsers = new List<SPUser>();
			foreach (string sourceGroupName in sourceGroupNames)
			{
				SPGroup item = parentWeb.Groups[sourceGroupName] as SPGroup;
				if (item == null)
				{
					continue;
				}
				if (!sPGroups.Contains(item))
				{
					sPGroups.Add(item);
				}
				while (item.Owner != null && (item.OwnerIsUser && !sPUsers.Contains((SPUser)item.Owner) || !item.OwnerIsUser && !sPGroups.Contains((SPGroup)item.Owner)))
				{
					if (!item.OwnerIsUser)
					{
						item = (SPGroup)item.Owner;
						sPGroups.Add(item);
					}
					else
					{
						sPUsers.Add((SPUser)item.Owner);
					}
				}
			}
			this.EnsurePrincipalExistence(sPUsers.ToArray(), sPGroups.ToArray(), sPWeb, null, null);
		}

		internal void EnsureManagedMetadataExistence(SPFieldCollection fieldCollection, SPWeb sourceWeb, SPWeb targetWeb, bool resolveManagedMetadataByName)
		{
			if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				if (!resolveManagedMetadataByName)
				{
					this.EnsureManagedMetadataExistenceIdApproachMigration(fieldCollection.GetReferencedManagedMetadataForIdStyleMigration(), targetWeb);
				}
				else
				{
					if (!fieldCollection.TaxonomyFieldsExist)
					{
						return;
					}
					IDictionary<Guid, IList<Guid>> requiredManagedMetadata = this.GetRequiredManagedMetadata(fieldCollection.GetTaxonomyFields());
					if (requiredManagedMetadata.Count > 0)
					{
						CopyTaxonomyAction copyTaxonomyAction = new CopyTaxonomyAction();
						copyTaxonomyAction.Options.SetFromOptions(this.Options);
						base.SubActions.Add(copyTaxonomyAction);
						object[] objArray = new object[] { requiredManagedMetadata, sourceWeb, targetWeb };
						copyTaxonomyAction.RunAsSubAction(objArray, new ActionContext(null, targetWeb), null);
						return;
					}
				}
			}
		}

		private void EnsureManagedMetadataExistenceIdApproachMigration(string referencedManagedMetadataFields, SPWeb targetNode)
		{
			if (!string.IsNullOrEmpty(referencedManagedMetadataFields))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(referencedManagedMetadataFields);
				if (xmlDocument.DocumentElement.HasChildNodes)
				{
					CopyTaxonomyAction copyTaxonomyAction = new CopyTaxonomyAction();
					copyTaxonomyAction.Options.SetFromOptions(this.Options);
					base.SubActions.Add(copyTaxonomyAction);
					object[] objArray = new object[] { referencedManagedMetadataFields, targetNode };
					copyTaxonomyAction.RunAsSubAction(objArray, new ActionContext(null, targetNode), null);
				}
			}
		}

		internal void EnsurePrincipalExistence(SecurityPrincipalCollection principals, SPWeb targetWeb)
		{
			List<SPUser> sPUsers = new List<SPUser>();
			List<SPGroup> sPGroups = new List<SPGroup>();
			foreach (SecurityPrincipal principal in (IEnumerable<SecurityPrincipal>)principals)
			{
				if (!(principal is SPUser))
				{
					if (!(principal is SPGroup))
					{
						continue;
					}
					sPGroups.Add((SPGroup)principal);
				}
				else
				{
					sPUsers.Add((SPUser)principal);
				}
			}
			this.EnsurePrincipalExistence(sPUsers.ToArray(), sPGroups.ToArray(), targetWeb, null, null);
		}

		internal void EnsurePrincipalExistence(SPUser[] users, SPGroup[] groups, SPWeb targetWeb, IUploadManager uploadManager = null, SPWeb sourceWeb = null)
		{
			ActionContext actionContext = new ActionContext(null, targetWeb);
			if (users != null && (int)users.Length > 0 && (!users.All<SPUser>((SPUser user) => this.PrincipalMappings.ContainsKey(user.PrincipalName)) || uploadManager != null || targetWeb.IsMySiteTemplate))
			{
				CopyUsersAction copyUsersAction = new CopyUsersAction();
				copyUsersAction.Options.SetFromOptions(this.Options);
				base.SubActions.Add(copyUsersAction);
				object[] sPUserCollection = new object[] { new SPUserCollection(users), targetWeb, uploadManager };
				copyUsersAction.RunAsSubAction(sPUserCollection, actionContext, null);
			}
			if (groups != null && (int)groups.Length > 0 && (!groups.All<SPGroup>((SPGroup group) => this.PrincipalMappings.ContainsKey(group.PrincipalName)) || uploadManager != null))
			{
				CopyGroupsAction copyGroupsAction = new CopyGroupsAction();
				copyGroupsAction.SharePointOptions.SetFromOptions(this.Options);
				base.SubActions.Add(copyGroupsAction);
				object[] objArray = new object[] { groups, targetWeb, uploadManager, sourceWeb };
				copyGroupsAction.RunAsSubAction(objArray, actionContext, null);
			}
		}

		protected string GetAccessRequestListCopyBufferKey(SPWeb targetWeb)
		{
			return string.Concat(typeof(CopyAccessRequestListAction).Name, targetWeb.ID);
		}

		protected static ColumnMappings GetColumnMappings(SPList sourceList, PasteListItemOptions options)
		{
			ColumnMappings columnMapping = null;
			if (options.ColumnMappings != null && (options.MapColumns || options.ColumnMappings.AutoMapCreatedAndModified || options.ColumnMappings.FieldsFilter != null && options.FilterFields) && options.ColumnMappings.ContainsColumnChanges)
			{
				columnMapping = ColumnMappings.MergeMappings(columnMapping, options.ColumnMappings);
			}
			if (options.ListFieldsFilterExpression != null && options.FilterFields)
			{
				ColumnMappings columnMapping1 = new ColumnMappings()
				{
					FieldsFilter = options.ListFieldsFilterExpression
				};
				columnMapping = ColumnMappings.MergeMappings(columnMapping, columnMapping1);
			}
			if (options.ApplyNewContentTypes && options.ContentTypeApplicationObjects != null)
			{
				foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in options.ContentTypeApplicationObjects)
				{
					if (!contentTypeApplicationObject.AppliesTo(sourceList))
					{
						continue;
					}
					if (contentTypeApplicationObject.ColumnMappings == null || !contentTypeApplicationObject.ColumnMappings.ContainsColumnChanges)
					{
						break;
					}
					columnMapping = ColumnMappings.MergeMappings(columnMapping, contentTypeApplicationObject.ColumnMappings);
					break;
				}
			}
			return columnMapping;
		}

		protected string GetDocSetVersionHistoryCopyBufferKey(SPList targetList)
		{
			return string.Format("{0}_{1}", "CopyDocumentSetVersionHistory", targetList.ID);
		}

		protected string GetListItemCopyCompletedBufferKey(SPList targetList)
		{
			return string.Concat("CopyListItemsCompleted", targetList.ParentWeb.ID, targetList.ID);
		}

		private IDictionary<Guid, IList<Guid>> GetRequiredManagedMetadata(SPFieldCollection taxonomyTypeFields)
		{
			bool flag;
			Dictionary<Guid, IList<Guid>> guids = new Dictionary<Guid, IList<Guid>>();
			foreach (SPField taxonomyTypeField in taxonomyTypeFields)
			{
				Metalogix.Transformers.TransformationRepository transformationRepository = base.TransformationRepository;
				Guid termSetId = taxonomyTypeField.TermSetId;
				string valueForKey = transformationRepository.GetValueForKey("$TSPKR$", termSetId.ToString("D"));
				if (base.TransformationRepository.DoesParentKeyExist(taxonomyTypeField.TermSetId.ToString("D").ToLower()))
				{
					flag = true;
				}
				else
				{
					flag = (valueForKey == null ? false : base.TransformationRepository.DoesParentKeyExist(valueForKey));
				}
				if (flag || !(taxonomyTypeField.TermSetId != Guid.Empty))
				{
					continue;
				}
				if (!guids.ContainsKey(taxonomyTypeField.TermstoreId))
				{
					guids.Add(taxonomyTypeField.TermstoreId, new List<Guid>());
				}
				if (guids[taxonomyTypeField.TermstoreId].Contains(taxonomyTypeField.TermSetId))
				{
					continue;
				}
				guids[taxonomyTypeField.TermstoreId].Add(taxonomyTypeField.TermSetId);
			}
			return guids;
		}

		private SPWebPart GetViewWebPart(SPWebPartPage sourcePage)
		{
			SPWebPart item = null;
			if (!sourcePage.Adapter.IsDB)
			{
				item = sourcePage.WebParts[0] as SPWebPart;
			}
			else
			{
				foreach (SPWebPart webPart in sourcePage.WebParts)
				{
					XmlNode xmlNodes = XmlUtility.StringToXmlNode(webPart.Xml).SelectSingleNode("//*[name() = 'DisplayName']");
					if (xmlNodes == null || string.IsNullOrEmpty(xmlNodes.InnerText))
					{
						XmlNode xmlNodes1 = XmlUtility.StringToXmlNode(webPart.Xml).SelectSingleNode("//*[name() = 'WebPart']");
						if (xmlNodes1 == null)
						{
							continue;
						}
						XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlNodes1.OwnerDocument.NameTable);
						xmlNamespaceManagers.AddNamespace("tns", "http://schemas.microsoft.com/WebPart/v3");
						xmlNodes = xmlNodes1.SelectSingleNode("/WebPart/tns:webPart/tns:data/tns:properties/tns:property[@name=\"DisplayName\"]", xmlNamespaceManagers);
						if (xmlNodes == null || string.IsNullOrEmpty(xmlNodes.InnerText))
						{
							continue;
						}
						item = webPart;
						break;
					}
					else
					{
						item = webPart;
						break;
					}
				}
			}
			return item;
		}

		protected string GetWebPartCopyBufferKey(SPWeb targetWeb)
		{
			return string.Concat(typeof(CopyWebPartsAction).Name, targetWeb.ID);
		}

		public void InitializeAudienceMappings(SPNode source, SPNode target)
		{
			if (!base.SharePointOptions.MapAudiences)
			{
				return;
			}
			try
			{
				SPAudienceCollection audienceCollection = SPAudienceCollection.GetAudienceCollection(source);
				SPAudienceCollection sPAudienceCollections = SPAudienceCollection.GetAudienceCollection(target);
				if (audienceCollection != null && sPAudienceCollections != null)
				{
					this.m_audienceIDMappings.Clear();
					foreach (SPAudience sPAudience in audienceCollection)
					{
						SPAudience sPAudience1 = null;
						foreach (SPAudience sPAudience2 in sPAudienceCollections)
						{
							if (sPAudience.Name != sPAudience2.Name)
							{
								continue;
							}
							sPAudience1 = sPAudience2;
							break;
						}
						if (sPAudience1 == null)
						{
							continue;
						}
						this.AudienceIDMappings.Add(sPAudience.ID.ToString(), sPAudience1.ID.ToString());
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem("Failed to Initialize Audience Mappings", "", source.DisplayUrl, target.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = exception
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
			}
		}

		private void InitializeLinkCorrector(SPNode sourceNode, SPNode targetNode)
		{
			SharePointAdapter adapter;
			SharePointAdapter sharePointAdapter;
			if (this.LinkCorrector != null)
			{
				this.LinkCorrector.ClearMappings();
				SPNode sPNode = sourceNode;
				SPNode sPNode1 = targetNode;
				foreach (KeyValuePair<string, string> globalUrlMapping in SPGlobalMappings.GlobalUrlMappings)
				{
					Metalogix.SharePoint.Migration.LinkCorrector linkCorrector = this.LinkCorrector;
					string key = globalUrlMapping.Key;
					string value = globalUrlMapping.Value;
					if (sPNode == null)
					{
						adapter = null;
					}
					else
					{
						adapter = sPNode.Adapter;
					}
					if (sPNode1 == null)
					{
						sharePointAdapter = null;
					}
					else
					{
						sharePointAdapter = sPNode1.Adapter;
					}
					linkCorrector.AddUserSpecifiedMapping(key, value, adapter, sharePointAdapter);
				}
				foreach (KeyValuePair<Guid, Guid> globalGuidMapping in SPGlobalMappings.GlobalGuidMappings)
				{
					Metalogix.SharePoint.Migration.LinkCorrector linkCorrector1 = this.LinkCorrector;
					Guid guid = globalGuidMapping.Key;
					linkCorrector1.AddGuidMapping(guid.ToString("D"), globalGuidMapping.Value.ToString("D"));
				}
			}
		}

		public void InitializePrincipalMappings(ISecurableObject source, ISecurableObject target, bool mapByName)
		{
			SecurityPrincipal securityPrincipal;
			string str;
			string principalName;
			this.m_principalMappings.Clear();
			SecurityPrincipalCollection principals = source.Principals;
			SecurityPrincipalCollection securityPrincipalCollection = target.Principals;
			foreach (SecurityPrincipal principal in (IEnumerable<SecurityPrincipal>)principals)
			{
				securityPrincipal = (!(principal is SPUser) ? principal : SPGlobalMappings.Map(principal, false));
				SecurityPrincipal item = null;
				if (mapByName || securityPrincipal is SPUser)
				{
					SPUser sPUser = securityPrincipal as SPUser;
					if (sPUser == null)
					{
						item = securityPrincipalCollection[securityPrincipal];
					}
					else
					{
						string lowerInvariant = sPUser.LoginName.ToLowerInvariant();
						bool flag = Utils.SwitchUserNameFormat(lowerInvariant, out str);
						foreach (SecurityPrincipal principal1 in (IEnumerable<SecurityPrincipal>)securityPrincipalCollection)
						{
							SPUser sPUser1 = principal1 as SPUser;
							if (sPUser1 == null)
							{
								continue;
							}
							string lowerInvariant1 = sPUser1.LoginName.ToLowerInvariant();
							if (lowerInvariant1 != lowerInvariant)
							{
								if (!flag || !(lowerInvariant1 == str))
								{
									continue;
								}
								item = sPUser1;
							}
							else
							{
								item = sPUser1;
								break;
							}
						}
					}
				}
				else
				{
					item = securityPrincipalCollection.MapSecurityPrincipal(principal);
				}
				ThreadSafeDictionary<string, string> mPrincipalMappings = this.m_principalMappings;
				string principalName1 = principal.PrincipalName;
				if (item != null)
				{
					principalName = item.PrincipalName;
				}
				else
				{
					principalName = null;
				}
				mPrincipalMappings.Add(principalName1, principalName);
			}
		}

		protected virtual void InitializeSharePointCopy(IXMLAbleList source, IXMLAbleList target, bool bRefresh)
		{
			ExplorerNode item;
			ExplorerNode explorerNode;
			if (source == null || source.Count <= 0)
			{
				item = null;
			}
			else
			{
				item = source[0] as ExplorerNode;
			}
			ExplorerNode explorerNode1 = item;
			if (target == null || target.Count <= 0)
			{
				explorerNode = null;
			}
			else
			{
				explorerNode = target[0] as ExplorerNode;
			}
			ExplorerNode explorerNode2 = explorerNode;
			if (bRefresh && explorerNode1 != null)
			{
				this.RefreshSourceNodes(source, explorerNode1, explorerNode2);
			}
			this.InitializeLinkCorrector(explorerNode1 as SPNode, explorerNode2 as SPNode);
		}

		protected internal void InitializeWorkflow()
		{
			if (!(this.Options is PasteSiteOptions))
			{
				return;
			}
			PasteSiteOptions options = (PasteSiteOptions)this.Options;
			if (options.CopyWebOOBWorkflowAssociations || options.CopyWebSharePointDesignerNintexWorkflowAssociations || options.CopyListOOBWorkflowAssociations || options.CopyListSharePointDesignerNintexWorkflowAssociations || options.CopyContentTypeOOBWorkflowAssociations || options.CopyContentTypeSharePointDesignerNintexWorkflowAssociations)
			{
				if (!WorkflowRespository.Initialised)
				{
					LogItem logItem = new LogItem(Resources.Workflow_InitialiseRepository, string.Empty, string.Empty, string.Empty, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					try
					{
						try
						{
							string workflowRepositoryActivityInfo = Resources.Workflow_RepositoryActivityInfo;
							string str = WorkflowRespository.Activities.ActivityNames.Count.ToString();
							int noOfSupportedDefaultActivities = WorkflowRespository.Activities.NoOfSupportedDefaultActivities;
							logItem.Information = string.Format(workflowRepositoryActivityInfo, str, noOfSupportedDefaultActivities.ToString());
							if (options.Verbose)
							{
								StringBuilder stringBuilder = new StringBuilder();
								stringBuilder.AppendLine(Resources.Workflow_RepositoryVerboseHeading);
								stringBuilder.AppendLine();
								foreach (WorkflowActivity activity in WorkflowRespository.Activities.Activities)
								{
									stringBuilder.AppendLine(string.Format(Resources.Workflow_RepositoryVerboseDetail, activity.Name, activity.Attributes.Count, activity.ActivityType.ToString()));
								}
								logItem.Details = stringBuilder.ToString();
							}
							logItem.Status = ActionOperationStatus.Completed;
						}
						catch (Exception exception)
						{
							logItem.Exception = exception;
							logItem.Information = string.Format(Resources.Workflow_InitialiseRepositoryError, Environment.NewLine, logItem.Information);
							logItem.Status = ActionOperationStatus.Warning;
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
				}
				this.WorkflowMappings.Clear();
				if (options.CopyWorkflowInstanceData)
				{
					this.WorkflowItemMappings.Clear();
				}
			}
		}

		private bool IsActionRunning()
		{
			return base.Status == ActionStatus.Running;
		}

		private static bool IsHealthScoreDisabled()
		{
			return false;
		}

		protected void LogTelemetryForWorkflows(string workflowType)
		{
			int num;
			if (!this.ActionOptions.TelemetryLogs.ContainsKey(workflowType))
			{
				this.ActionOptions.TelemetryLogs.Add(workflowType, "1");
			}
			else
			{
				T actionOptions = this.ActionOptions;
				string value = actionOptions.TelemetryLogs.GetValue(workflowType, "");
				int.TryParse(value, out num);
				if (num > 0)
				{
					T str = this.ActionOptions;
					int num1 = num + 1;
					str.TelemetryLogs[workflowType] = num1.ToString();
					return;
				}
			}
		}

		public string MapAudienceString(string sAudiences, SPNode source, SPNode target)
		{
			if (!base.SharePointOptions.MapAudiences || sAudiences == null || sAudiences == "" || (this.PrincipalMappings == null || this.PrincipalMappings.Count == 0) && (this.AudienceIDMappings == null || this.AudienceIDMappings.Count == 0))
			{
				return sAudiences;
			}
			List<string> strs = new List<string>();
			List<string> strs1 = new List<string>();
			string str = "";
			if (!source.Adapter.SharePointVersion.IsSharePoint2007OrLater)
			{
				char[] chrArray = new char[] { ',' };
				string[] strArrays = sAudiences.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str1 = strArrays[i];
					char[] chrArray1 = new char[] { '\'' };
					strs.Add(str1.Trim(chrArray1));
				}
			}
			else
			{
				string[] strArrays1 = new string[] { ";;" };
				string[] strArrays2 = sAudiences.Split(strArrays1, StringSplitOptions.None);
				if ((int)strArrays2.Length > 0 && !string.IsNullOrEmpty(strArrays2[0]))
				{
					string str2 = strArrays2[0];
					char[] chrArray2 = new char[] { ',' };
					string[] strArrays3 = str2.Split(chrArray2, StringSplitOptions.RemoveEmptyEntries);
					if (strArrays3 != null && (int)strArrays3.Length > 0)
					{
						strs.AddRange(strArrays3);
					}
				}
				if ((int)strArrays2.Length > 1 && !string.IsNullOrEmpty(strArrays2[1]))
				{
					str = strArrays2[1];
				}
				if ((int)strArrays2.Length > 2 && !string.IsNullOrEmpty(strArrays2[2]))
				{
					string str3 = strArrays2[2];
					char[] chrArray3 = new char[] { ',' };
					string[] strArrays4 = str3.Split(chrArray3, StringSplitOptions.RemoveEmptyEntries);
					if (strArrays4 != null && (int)strArrays4.Length > 0)
					{
						strs1.AddRange(strArrays4);
					}
				}
			}
			List<string> strs2 = null;
			if (this.AudienceIDMappings == null)
			{
				strs2 = new List<string>();
			}
			else
			{
				strs2 = new List<string>();
				foreach (string str4 in strs)
				{
					if (!this.AudienceIDMappings.ContainsKey(str4))
					{
						continue;
					}
					string item = this.AudienceIDMappings[str4];
					if (strs2.Contains(item))
					{
						continue;
					}
					strs2.Add(item);
				}
			}
			if (source.Adapter.SharePointVersion.IsSharePoint2007 && target.Adapter.SharePointVersion.IsSharePoint2010 || source.Adapter.SharePointVersion.IsSharePoint2010 && target.Adapter.SharePointVersion.IsSharePoint2007)
			{
				str = "";
			}
			List<string> strs3 = null;
			if (this.PrincipalMappings == null)
			{
				strs3 = strs1;
			}
			else
			{
				this.EnsureAudiencedGroupExistence(strs1, source, target);
				strs3 = new List<string>();
				foreach (string str5 in strs1)
				{
					if (!this.PrincipalMappings.ContainsKey(str5))
					{
						continue;
					}
					string item1 = this.PrincipalMappings[str5];
					if (strs3.Contains(item1))
					{
						continue;
					}
					strs3.Add(item1);
				}
			}
			string str6 = "";
			foreach (string str7 in strs2)
			{
				if (str6 != "")
				{
					str6 = string.Concat(str6, ",");
				}
				str6 = string.Concat(str6, str7);
			}
			str6 = string.Concat(str6, ";;", str, ";;");
			foreach (string str8 in strs3)
			{
				if (!str6.EndsWith(";;"))
				{
					str6 = string.Concat(str6, ",");
				}
				str6 = string.Concat(str6, str8);
			}
			return str6;
		}

		public bool MapListItemAudiences(ref string sListItemXml, SPList sourceList, SPNode target)
		{
			if (!base.SharePointOptions.MapAudiences || (this.AudienceIDMappings == null || this.AudienceIDMappings.Count == 0) && (this.PrincipalMappings == null || this.PrincipalMappings.Count == 0))
			{
				return false;
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXml);
			bool flag = this.MapListItemAudiences(xmlNode, sourceList, target);
			if (flag)
			{
				sListItemXml = xmlNode.OuterXml;
			}
			return flag;
		}

		public bool MapListItemAudiences(XmlNode node, SPList sourceList, SPNode target)
		{
			if (!base.SharePointOptions.MapAudiences || (this.AudienceIDMappings == null || this.AudienceIDMappings.Count == 0) && (this.PrincipalMappings == null || this.PrincipalMappings.Count == 0))
			{
				return false;
			}
			string name = null;
			foreach (SPField field in sourceList.Fields)
			{
				if (field.Type != PasteAction<T>.AUDIENCE_FIELD_TYPE)
				{
					continue;
				}
				name = field.Name;
				break;
			}
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			XmlAttribute itemOf = node.Attributes[name];
			if (itemOf == null || string.IsNullOrEmpty(itemOf.Value))
			{
				return false;
			}
			string str = this.MapAudienceString(itemOf.Value, sourceList, target);
			if (itemOf.Value == str)
			{
				return false;
			}
			itemOf.Value = str;
			return true;
		}

		public bool MapNavigationAudiences(ref string sNavigationXml)
		{
			if (!base.SharePointOptions.MapAudiences || (this.AudienceIDMappings == null || this.AudienceIDMappings.Count == 0) && (this.PrincipalMappings == null || this.PrincipalMappings.Count == 0))
			{
				return false;
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sNavigationXml);
			bool flag = this.MapNavigationAudiences(xmlNode);
			if (flag)
			{
				sNavigationXml = xmlNode.OuterXml;
			}
			return flag;
		}

		public bool MapNavigationAudiences(XmlNode node)
		{
			if (base.SharePointOptions.MapAudiences && (this.AudienceIDMappings != null && this.AudienceIDMappings.Count != 0 || this.PrincipalMappings != null && this.PrincipalMappings.Count != 0))
			{
				return true;
			}
			return false;
		}

		internal string MapPrincipal(string principalName)
		{
			if (!this.PrincipalMappings.ContainsKey(principalName))
			{
				return principalName;
			}
			return this.PrincipalMappings[principalName];
		}

		public bool MapWebPartAudiences(ref SPWebPart webPart, SPNode source, SPNode target)
		{
			if (webPart == null || (this.AudienceIDMappings == null || this.AudienceIDMappings.Count == 0) && (this.PrincipalMappings == null || this.PrincipalMappings.Count == 0))
			{
				return false;
			}
			if (!webPart.HasProperty(PasteAction<T>.AUDIENCE_WEBPART_PROPERTY_NAME))
			{
				return false;
			}
			string item = webPart[PasteAction<T>.AUDIENCE_WEBPART_PROPERTY_NAME];
			if (string.IsNullOrEmpty(item))
			{
				return false;
			}
			string str = this.MapAudienceString(item, source, target);
			if (item == str)
			{
				return false;
			}
			webPart[PasteAction<T>.AUDIENCE_WEBPART_PROPERTY_NAME] = str;
			return true;
		}

		private void OnHealthChecked(IDictionary<string, ServerHealthInformation> healthInformations)
		{
			if (!this.IsActionRunning())
			{
				return;
			}
			LogItem logItem = HealthScoreUtils.CreateServerHealthScoreLogItem(healthInformations);
			base.FireOperationStarted(logItem);
			base.FireOperationFinished(logItem);
		}

		protected void QueueAccessRequestListCopies(SPWeb sourceWeb, SPWeb targetWeb, PasteSiteOptions options)
		{
			if (options.CopyAccessRequestSettings && sourceWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater && !sourceWeb.Adapter.IsClientOM && targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater && !targetWeb.Adapter.IsClientOM)
			{
				string accessRequestListCopyBufferKey = this.GetAccessRequestListCopyBufferKey(targetWeb);
				Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
				object[] objArray = new object[] { sourceWeb, targetWeb, options };
				threadManager.QueueBufferedTask(accessRequestListCopyBufferKey, objArray, new ThreadedOperationDelegate(this.CopyAccessRequestList));
			}
		}

		protected void QueueListViewWebPartCopies(SPList sourceList, SPList targetList, WebPartOptions options)
		{
			if (!options.CopyViewWebParts || sourceList.Adapter.SharePointVersion.IsSharePoint2003)
			{
				return;
			}
			Dictionary<SPView, SPView> sPViews = new Dictionary<SPView, SPView>();
			foreach (SPView view in sourceList.Views)
			{
				if (view.IsWebPartView)
				{
					continue;
				}
				foreach (SPView sPView in targetList.Views)
				{
					if (sPView.IsWebPartView || !(view.DisplayName == sPView.DisplayName))
					{
						continue;
					}
					sPViews.Add(view, sPView);
					break;
				}
			}
			string webPartCopyBufferKey = this.GetWebPartCopyBufferKey(targetList.ParentWeb);
			foreach (KeyValuePair<SPView, SPView> keyValuePair in sPViews)
			{
				Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
				object[] key = new object[] { keyValuePair.Key, keyValuePair.Value, sourceList, targetList, options };
				threadManager.QueueBufferedTask(webPartCopyBufferKey, key, new ThreadedOperationDelegate(this.CopyViewPageWebParts));
			}
		}

		protected void RefreshSourceNode(SPNode sourceNode, SPNode targetNode, string nodeTypeName)
		{
			LogItem logItem = null;
			try
			{
				logItem = new LogItem(string.Concat("Refreshing ", nodeTypeName), sourceNode.DisplayName, sourceNode.DisplayUrl, (targetNode != null ? targetNode.DisplayUrl : ""), ActionOperationStatus.Running)
				{
					WriteToJobDatabase = false
				};
				base.FireOperationStarted(logItem);
				RefreshAction refreshAction = new RefreshAction();
				Node[] nodeArray = new Node[] { sourceNode };
				refreshAction.Run(null, new NodeCollection(nodeArray));
			}
			finally
			{
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
		}

		private void RefreshSourceNodes(IXMLAbleList source, Node sourceNode, Node targetNode)
		{
			LogItem logItem = null;
			try
			{
				logItem = new LogItem("Refreshing Source Node", sourceNode.DisplayName, sourceNode.DisplayUrl, (targetNode != null ? targetNode.DisplayUrl : ""), ActionOperationStatus.Running)
				{
					WriteToJobDatabase = false
				};
				base.FireOperationStarted(logItem);
				(new RefreshAction()).Run(null, source);
			}
			finally
			{
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}

		protected override void RunAfterAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.AddAdditionalTelemetryInfo(target);
			if (this._healthScoreBlocker != null)
			{
				base.ActionBlockers.Remove(this._healthScoreBlocker);
				this._healthScoreBlocker = null;
			}
			if (this._healthCheckTimer == null)
			{
				return;
			}
			this._healthCheckTimer.Stop();
			this._healthCheckTimer.Dispose();
			this._healthCheckTimer = null;
		}

		protected override void RunBeforeAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (PasteAction<T>.IsHealthScoreDisabled())
			{
				return;
			}
			if (HealthScoreUtils.NeitherSourceNorTargetSupportsHealthMonitor(source, target))
			{
				LogItem logItem = HealthScoreUtils.CreateHealthScoreCheckSkippedLogItem();
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
				return;
			}
			this._healthCheckTimer = new HealthCheckTimer(new ActionContext(source, target));
			this._healthCheckTimer.OnHealthChecked += new HealthCheckTimer.HealthCheckHandler(this.OnHealthChecked);
			this._healthCheckTimer.Start(new HealthCheckTimerAppConfigSettings());
			this._healthScoreBlocker = this.CreateHealthScoreBlocker(source, target);
			base.ActionBlockers.Add(this._healthScoreBlocker);
		}

		protected void RunPreCopyListUpdate(SPList sourceList, SPFolder targetFolder, PasteListItemOptions options)
		{
			try
			{
				this.LinkCorrector.AddWebMappings(sourceList.ParentWeb, targetFolder.ParentList.ParentWeb);
			}
			catch (ArgumentException argumentException)
			{
			}
			if (options.ColumnMappings != null && (options.MapColumns || options.ColumnMappings.AutoMapCreatedAndModified) && options.ColumnMappings.ContainsColumnAdditions)
			{
				SPList sPList = (targetFolder is SPList ? (SPList)targetFolder : targetFolder.ParentList);
				XmlNode xmlNodes = sPList.GetListXML(false).Clone();
				string outerXml = xmlNodes.OuterXml;
				LogItem logItem = new LogItem("Updating List Schema", sPList.Name, sourceList.DisplayUrl, targetFolder.ParentList.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					try
					{
						bool flag = false;
						if (options.ApplyNewContentTypes && options.ContentTypeApplicationObjects != null && options.ContentTypeApplicationObjects != null)
						{
							foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in options.ContentTypeApplicationObjects)
							{
								if (!contentTypeApplicationObject.AppliesTo(sourceList) || contentTypeApplicationObject.Data.Count <= 0)
								{
									continue;
								}
								PasteAction<T>.SetContentTypesEnabled(xmlNodes);
								flag = true;
							}
						}
						ColumnMappings columnMappings = PasteAction<T>.GetColumnMappings(sourceList, options);
						if (columnMappings != null && columnMappings.ModifyListXML(xmlNodes, sourceList, sPList.ParentWeb, options.FilterFields))
						{
							flag = true;
						}
						if (flag)
						{
							targetFolder.ParentList.UpdateSettings(xmlNodes.OuterXml);
							logItem.Status = ActionOperationStatus.Completed;
						}
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
						logItem.SourceContent = outerXml;
						logItem.TargetContent = xmlNodes.OuterXml;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
			}
		}

		protected void SetAllListItemCopyCompletedBufferKeysForWeb(SPWeb targetWeb)
		{
			base.ThreadManager.SetBufferedTasksWithWildCard(string.Concat("CopyListItemsCompleted", targetWeb.ID, ".*"), false, false);
		}

		protected static void SetContentTypesEnabled(ref string sListXml)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sListXml);
			PasteAction<T>.SetContentTypesEnabled(xmlNode);
			sListXml = xmlNode.OuterXml;
		}

		protected static void SetContentTypesEnabled(XmlNode listNode)
		{
			XmlAttribute itemOf = listNode.Attributes["ContentTypesEnabled"];
			if (itemOf == null)
			{
				itemOf = listNode.OwnerDocument.CreateAttribute("ContentTypesEnabled");
				listNode.Attributes.Append(itemOf);
			}
			itemOf.Value = "True";
		}

		private bool ShouldUseFileServerHealthScore(string serverHealthFile)
		{
			return false;
		}

		protected internal void StartCommonWorkflowBufferedTasks()
		{
			base.ThreadManager.SetBufferedTasks("CopyGloballyReusableWorkflowTemplateFiles", false, true);
			base.ThreadManager.SetBufferedTasks("CopyWorkflowTemplateFiles", false, true);
			base.ThreadManager.SetBufferedTasks(typeof(CopyWorkflowAssociationsAction).Name, false, false);
			base.ThreadManager.SetBufferedTasks(typeof(PasteAllNintexWorkflows).Name, false, false);
		}
	}
}