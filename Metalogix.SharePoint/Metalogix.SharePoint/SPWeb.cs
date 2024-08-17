using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Core.Support;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Explorer.Attributes;
using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.BCS;
using Metalogix.SharePoint.Common;
using Metalogix.SharePoint.Common.Enums;
using Metalogix.SharePoint.Interfaces;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.Workflow2013;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Image("Metalogix.SharePoint.Icons.SPSite-Valid.ico")]
	[Name("Site")]
	[PluralName("Sites")]
	[UserFriendlyNodeName("SharePoint Site")]
	public class SPWeb : SPConnection, ISecurableObject, ITaxonomySiteConnection, ITaxonomyConnection, IHasSupportInfo, ISupportSP2013Workflows
	{
		public const string PUBLISHING_FEATURE_GUID = "94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb";

		private const string NINTEX_FEATURE_GUID = "0561d315-d5db-4736-929e-26da142812c5";

		public const string FORMS_SERVICES_GUID = "c88c4ff1-dbf5-4649-ad9f-c6c426ebcbf5";

		public const string SHAREPOINT2007_WORKFLOWS_GUID = "c845ed8d-9ce5-448c-bd3e-ea71350ce45b";

		public const string OFFICE_SHAREPOINT_SERVER_STANDARD_SITE_FEATURE_GUID = "0be49fe9-9bc9-409d-abf9-702753bd878d";

		public const string SHAREPOINT2010_DOCUMENT_SET_GUID = "3bae86a2-776d-499d-9db8-fa4cdc7884f8";

		public const string WIKI_WELCOME_PAGE_GUID = "8c6a6980-c3d9-440e-944c-77f93bc65a7e";

		public const string ONENOTE2010_NOTEBOOK_GUID = "f151bb39-7c3b-414f-bb36-6bf18872052f";

		private const string SHAREPOINT2013_WORKFLOWS_GUID = "0af5989a-3aea-4519-8ab0-85d91abe39ff";

		private const string SHAREPOINT2013_WORKFLOWS_HISTORY_GUID = "00bfea71-4ea5-48d4-a4ad-305cf7030140";

		private const string DocumentIDFeatureGuid = "b50e3104-6812-424f-a011-cc90e6327318";

		private const string C_IMAGENAME = "Metalogix.SharePoint.Icons.SPWeb-{0}-{1}{2}.ico";

		public static bool SHOW_WEB_URL;

		private readonly static string[] PUBLISHING_INFRASTRUCTURE_GUIDS;

		private SPWeb m_parentWeb = null;

		private XmlNode m_webXML;

		private object _webXmlLock = new object();

		private readonly object _webPartLock = new object();

		private Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sp2013WorkflowCollection;

		private volatile string m_ID = null;

		private string m_sTitle;

		private string m_sServerRelativeUrl;

		private string m_sName;

		private int m_iLocale;

		private int m_iLanguage;

		private SPWebTemplate m_webTemplate;

		private int m_iWebTemplateID;

		private bool m_bIsSearchable = false;

		private int m_iMaximumFileSize = 0;

		private Guid m_webApplicationId;

		private TimeZoneInformation m_timeZone = null;

		private readonly object m_oLockUniquePermissions = new object();

		private bool? m_bHasUniquePermissions = null;

		private bool? m_bHasPublishingFeature = null;

		private bool? hasNintextFeature = null;

		private bool? m_bHasFormsService = null;

		private bool? m_bPublishingInfrastructureActive = null;

		private bool? m_bWikiWelcomePageActive = null;

		private bool? m_bSharePoint2007WorkflowsFeature = null;

		private bool? m_bSharePoint2013WorkflowsFeature = null;

		private bool? m_bHasSharePoint2010DocumentSetFeature = null;

		private bool? _hasOneNote2010NotebookFeature = null;

		private readonly ReaderWriterLock _lockQuickProperties = new ReaderWriterLock();

		private readonly object m_oLockCachedRootSite = new object();

		private SPSite m_cachedRootSite = null;

		private SPList _listTemplateGallery;

		private readonly object _listTemplateGalleryLock = new object();

		private SPList _accessRequestList;

		private readonly object _accessRequestListLock = new object();

		private SPMasterPageGallery m_masterPageGallery;

		private readonly object _masterPageGalleryLock = new object();

		private SPComposedLooksGallery composedLooksGallery;

		private readonly object _composedLooksGalleryLock = new object();

		private SPList userInformationList;

		private readonly object _userInformationList = new object();

		private object m_oLockSubwebs = new object();

		private SPWebCollection m_subWebs = null;

		private object m_oLockLists = new object();

		private SPListCollection m_lists = null;

		private SPTermStoreCollection _termStores = null;

		private readonly object _termStoresLock = new object();

		private bool? m_bStoragePointPresent = null;

		private SPExternalContentTypeCollection m_externalContentTypes = null;

		private readonly object _externalContentTypesLock = new object();

		private static string[] s_navigationAttributes;

		private static string[] s_rootWebNavigationAttributes;

		private readonly object m_oLockSiteUser = new object();

		private SPUserCollection m_users = null;

		private readonly object m_oLockGroups = new object();

		private SPGroupCollection m_groups = null;

		private readonly object m_oLockRoles = new object();

		private SPRoleCollection m_roles = null;

		private readonly object m_oLockRoleAssignments = new object();

		private SPRoleAssignmentCollection m_roleAssignments = null;

		private SPFieldCollection m_siteColumns = null;

		private readonly object _siteColumnLock = new object();

		private SPFieldCollection m_availableColumns = null;

		private readonly object _availableColumnsLock = new object();

		private SPMeetingInstanceCollection m_meetingInstances = null;

		private readonly object _meetingInstLock = new object();

		private bool m_bSiteCannotContainMeetingInstances = false;

		private SPContentTypeCollection m_contentTypes = null;

		private readonly object _contentTypesLock = new object();

		private SPListTemplateCollection m_listTemplates = null;

		private readonly object _listTemplatesLock = new object();

		private SPAudienceCollection m_audienceCollection = null;

		private readonly object _audienceCollectionLock = new object();

		private SPAlertCollection m_alerts = null;

		private readonly object _alertsLock = new object();

		private SPWorkflowAssociationCollection m_WorkflowAssociations = null;

		private readonly object _workflowAscLock = new object();

		private bool _includePreviousWorkflowVersions = false;

		private SPWebTemplateCollection m_Templates = null;

		private readonly object _templatesLock = new object();

		private List<ViewLanguageResource> _languageResourcesForViews;

		private SPList m_PagesLibrary = null;

		private readonly object _pagesLibraryLock = new object();

		private SPList m_BlogPostsList = null;

		private readonly object _blogPostsListLock = new object();

		private SPNavigationRoot m_navigation = null;

		private readonly object _navigationLock = new object();

		private SPPortalListingCollection m_portalListings = null;

		private readonly object _portalListingsLock = new object();

		private SPFolderBasic m_rootFolder = null;

		private readonly object _rootFolderLock = new object();

		public SPList AccessRequestList
		{
			get
			{
				SPList sPList;
				lock (this._accessRequestListLock)
				{
					if (this._accessRequestList == null)
					{
						string lists = base.Adapter.Reader.GetLists();
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(lists);
						foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes(".//List"))
						{
							if (xmlNodes.Attributes["BaseTemplate"].Value.Equals(160.ToString()))
							{
								this._accessRequestList = new SPList(this, xmlNodes);
								break;
							}
						}
					}
					sPList = this._accessRequestList;
				}
				return sPList;
			}
		}

		public SPAlertCollection Alerts
		{
			get
			{
				SPAlertCollection mAlerts;
				lock (this._alertsLock)
				{
					if (this.m_alerts == null)
					{
						this.m_alerts = this.GetAlerts(false);
					}
					mAlerts = this.m_alerts;
				}
				return mAlerts;
			}
		}

		public SPAudienceCollection Audiences
		{
			get
			{
				SPAudienceCollection mAudienceCollection;
				if (!(base.Parent == null ? false : !this.m_bInUseAsSource.HasValue))
				{
					lock (this._audienceCollectionLock)
					{
						if (this.m_audienceCollection == null)
						{
							try
							{
								this.m_audienceCollection = this.GetAudienceCollection(false);
							}
							catch (Exception exception)
							{
								throw new Exception(string.Concat("Could not fetch audiences: ", exception.Message));
							}
						}
						mAudienceCollection = this.m_audienceCollection;
					}
				}
				else if (!(base.Parent is SPServer))
				{
					mAudienceCollection = (!(base.Parent is SPTenant) ? this.RootWeb.Audiences : ((SPTenant)base.Parent).Audiences);
				}
				else
				{
					mAudienceCollection = ((SPServer)base.Parent).Audiences;
				}
				return mAudienceCollection;
			}
		}

		public SPFieldCollection AvailableColumns
		{
			get
			{
				SPFieldCollection mAvailableColumns;
				lock (this._availableColumnsLock)
				{
					if (this.m_availableColumns == null)
					{
						this.m_availableColumns = this.GetAvailableColumns(false);
						this.m_availableColumns.FieldCollectionChanged += new SPFieldCollection.FieldCollectionChangeEventHandler(this.FieldCollectionChangedHandler);
					}
					mAvailableColumns = this.m_availableColumns;
				}
				return mAvailableColumns;
			}
		}

		public SPList BlogPostsList
		{
			get
			{
				SPList mBlogPostsList;
				lock (this._blogPostsListLock)
				{
					if (this.m_BlogPostsList == null)
					{
						IEnumerator<SPList> enumerator = this.Lists.GetListsByListTemplate(ListTemplateType.BlogPosts).GetEnumerator();
						try
						{
							if (enumerator.MoveNext())
							{
								this.m_BlogPostsList = enumerator.Current;
							}
						}
						finally
						{
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
					}
					mBlogPostsList = this.m_BlogPostsList;
				}
				return mBlogPostsList;
			}
		}

		protected SPSite CachedRootSite
		{
			get
			{
				SPSite mCachedRootSite;
				lock (this.m_oLockCachedRootSite)
				{
					if (this.m_cachedRootSite != null)
					{
						mCachedRootSite = this.m_cachedRootSite;
						return mCachedRootSite;
					}
				}
				SPWeb parent = base.Parent as SPWeb;
				if (parent == null)
				{
					mCachedRootSite = null;
				}
				else
				{
					mCachedRootSite = parent.CachedRootSite;
				}
				return mCachedRootSite;
			}
		}

		public SPComposedLooksGallery ComposedLooksGallery
		{
			get
			{
				SPComposedLooksGallery sPComposedLooksGallery;
				lock (this._composedLooksGalleryLock)
				{
					if (this.composedLooksGallery == null)
					{
						string lists = base.Adapter.Reader.GetLists();
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(lists);
						foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes(".//List"))
						{
							ListTemplateType listTemplateType = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), xmlNodes.Attributes["BaseTemplate"].Value);
							if ((!xmlNodes.Attributes["Name"].Value.Equals("design") ? false : listTemplateType == ListTemplateType.DesignCatalog))
							{
								this.composedLooksGallery = new SPComposedLooksGallery(this, xmlNodes);
								break;
							}
						}
					}
					sPComposedLooksGallery = this.composedLooksGallery;
				}
				return sPComposedLooksGallery;
			}
		}

		public bool ConnectedAsSiteAdmin
		{
			get
			{
				string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("IsSiteAdmin");
				return (string.IsNullOrEmpty(attributeValueFromFullXml) ? false : bool.Parse(attributeValueFromFullXml));
			}
		}

		public SPContentTypeCollection ContentTypes
		{
			get
			{
				SPContentTypeCollection mContentTypes;
				lock (this._contentTypesLock)
				{
					if (this.m_contentTypes == null)
					{
						this.m_contentTypes = this.GetContentTypes(false);
					}
					mContentTypes = this.m_contentTypes;
				}
				return mContentTypes;
			}
		}

		public SPUser CreatedBy
		{
			get
			{
				return this.SiteUsers.GetByLoginName(this.CreatedByUserName);
			}
		}

		public string CreatedByUserName
		{
			get
			{
				return this.GetAttributeValueFromFullXml("Author");
			}
		}

		public DateTime CreatedDate
		{
			get
			{
				DateTime dateTime;
				DateTime dateTime1;
				dateTime1 = (!Utils.TryParseDateAsUtc(this.GetAttributeValueFromFullXml("CreatedDate"), out dateTime) ? DateTime.MinValue : dateTime);
				return dateTime1;
			}
		}

		public string CustomMasterPageUrl
		{
			get
			{
				return this.GetAttributeValueFromFullXml("CustomMasterPage");
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.GetAttributeValueFromFullXml("DatabaseName");
			}
		}

		public string DatabaseServer
		{
			get
			{
				return this.GetAttributeValueFromFullXml("DatabaseServerName");
			}
		}

		public bool DefaultPageInRoot
		{
			get
			{
				bool flag = false;
				ISharePointReader reader = base.Adapter.Reader;
				string serverRelativeUrl = this.ServerRelativeUrl;
				char[] chrArray = new char[] { '/' };
				bool.TryParse(reader.HasDocument(string.Concat(serverRelativeUrl.TrimEnd(chrArray), "/default.aspx")), out flag);
				return flag;
			}
		}

		public override string DisplayName
		{
			get
			{
				string str;
				if (base.Parent != null)
				{
					string title = this.Title;
					if (SPWeb.SHOW_WEB_URL)
					{
						title = (this.LinkableUrl != null ? this.LinkableUrl : this.DisplayUrl);
					}
					str = (title == null ? base.Adapter.Url : string.Concat(title, (this.GetWebTemplate(false) != null ? string.Concat(" (", this.GetWebTemplate(false).Title, ")") : "")));
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append(base.Adapter.Url);
					stringBuilder.Append(" (");
					if (this.GetWebTemplate(false) != null)
					{
						stringBuilder.Append(string.Concat(this.GetWebTemplate(false).Title, " - "));
					}
					stringBuilder.Append(base.Adapter.LoggedInAs);
					stringBuilder.Append(")");
					str = stringBuilder.ToString();
				}
				return str;
			}
		}

		public override string DisplayUrl
		{
			get
			{
				string str;
				if (base.Adapter == null)
				{
					str = null;
				}
				else if ((base.Adapter.ServerDisplayName == null ? false : !(base.Adapter.ServerDisplayName.Trim() == "")))
				{
					string serverDisplayName = base.Adapter.ServerDisplayName;
					if (!string.IsNullOrEmpty(this.ServerRelativeUrl))
					{
						str = (!this.ServerRelativeUrl.StartsWith("/") ? string.Concat(serverDisplayName, "/", this.ServerRelativeUrl) : string.Concat(serverDisplayName, this.ServerRelativeUrl));
					}
					else
					{
						str = (base.Connection.Node != this ? serverDisplayName : base.Adapter.Url);
					}
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public SharePoint2013ExperienceVersion ExperienceVersion
		{
			get
			{
				SharePoint2013ExperienceVersion sharePoint2013ExperienceVersion;
				if (base.Adapter.SharePointVersion.IsSharePoint2013OrLater)
				{
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("ExperienceVersion");
					if (!string.IsNullOrEmpty(attributeValueFromFullXml))
					{
						sharePoint2013ExperienceVersion = (SharePoint2013ExperienceVersion)int.Parse(attributeValueFromFullXml);
					}
					else
					{
						sharePoint2013ExperienceVersion = SharePoint2013ExperienceVersion.SP2013;
					}
				}
				else
				{
					sharePoint2013ExperienceVersion = SharePoint2013ExperienceVersion.NotApplicable;
				}
				return sharePoint2013ExperienceVersion;
			}
		}

		public SPExternalContentTypeCollection ExternalContentTypes
		{
			get
			{
				SPExternalContentTypeCollection mExternalContentTypes;
				if ((this.m_parentWeb == null ? true : this.m_bInUseAsSource.HasValue))
				{
					lock (this._externalContentTypesLock)
					{
						if (this.m_externalContentTypes == null)
						{
							this.m_externalContentTypes = new SPExternalContentTypeCollection(this);
						}
						mExternalContentTypes = this.m_externalContentTypes;
					}
				}
				else
				{
					mExternalContentTypes = this.m_parentWeb.ExternalContentTypes;
				}
				return mExternalContentTypes;
			}
		}

		protected XmlNode FullXML
		{
			get
			{
				return this.GetFullXML(false);
			}
		}

		public SPGroupCollection Groups
		{
			get
			{
				SPGroupCollection mGroups;
				if ((this.m_parentWeb == null ? true : this.m_bInUseAsSource.HasValue))
				{
					lock (this.m_oLockGroups)
					{
						if (this.m_groups == null)
						{
							this.m_groups = this.GetGroups(false);
						}
					}
					mGroups = this.m_groups;
				}
				else
				{
					mGroups = this.m_parentWeb.Groups;
				}
				return mGroups;
			}
		}

		public bool HasFormsServiceFeature
		{
			get
			{
				if (!this.m_bHasFormsService.HasValue)
				{
					bool flag = false;
					if (!base.Adapter.IsDB)
					{
						string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
						if (attributeValueFromFullXml != null)
						{
							char[] chrArray = new char[] { ',' };
							List<string> strs = new List<string>(attributeValueFromFullXml.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
							flag = strs.Contains("c88c4ff1-dbf5-4649-ad9f-c6c426ebcbf5");
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = false;
					}
					this.m_bHasFormsService = new bool?(flag);
				}
				return this.m_bHasFormsService.Value;
			}
		}

		protected bool HasFullXML
		{
			get
			{
				return this.m_webXML != null;
			}
		}

		public bool HasNintextFeature
		{
			get
			{
				if (!this.hasNintextFeature.HasValue)
				{
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
					if (!string.IsNullOrEmpty(attributeValueFromFullXml))
					{
						string lower = attributeValueFromFullXml.ToLower();
						char[] chrArray = new char[] { ',' };
						List<string> strs = new List<string>(lower.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
						this.hasNintextFeature = new bool?(strs.Contains("0561d315-d5db-4736-929e-26da142812c5"));
					}
					else
					{
						this.hasNintextFeature = new bool?(false);
					}
				}
				return this.hasNintextFeature.Value;
			}
		}

		public bool HasOneNote2010NotebookFeature
		{
			get
			{
				if (!this._hasOneNote2010NotebookFeature.HasValue)
				{
					bool flag = false;
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteFeatures");
					if (attributeValueFromFullXml != null)
					{
						char[] chrArray = new char[] { ',' };
						List<string> strs = new List<string>(attributeValueFromFullXml.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
						flag = strs.Contains("f151bb39-7c3b-414f-bb36-6bf18872052f");
					}
					else
					{
						flag = false;
					}
					this._hasOneNote2010NotebookFeature = new bool?(flag);
				}
				return this._hasOneNote2010NotebookFeature.Value;
			}
		}

		public bool HasPublishingFeature
		{
			get
			{
				if (!this.m_bHasPublishingFeature.HasValue)
				{
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteFeatures");
					if (attributeValueFromFullXml != null)
					{
						string lower = attributeValueFromFullXml.ToLower();
						char[] chrArray = new char[] { ',' };
						List<string> strs = new List<string>(lower.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
						this.m_bHasPublishingFeature = new bool?(strs.Contains("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb"));
					}
					else
					{
						this.m_bHasPublishingFeature = new bool?(false);
					}
				}
				return this.m_bHasPublishingFeature.Value;
			}
		}

		public bool HasSharePoint2007WorkflowsFeature
		{
			get
			{
				if (!this.m_bSharePoint2007WorkflowsFeature.HasValue)
				{
					bool flag = false;
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
					if (attributeValueFromFullXml != null)
					{
						char[] chrArray = new char[] { ',' };
						List<string> strs = new List<string>(attributeValueFromFullXml.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
						flag = strs.Contains("c845ed8d-9ce5-448c-bd3e-ea71350ce45b");
					}
					else
					{
						flag = false;
					}
					this.m_bSharePoint2007WorkflowsFeature = new bool?(flag);
				}
				return this.m_bSharePoint2007WorkflowsFeature.Value;
			}
		}

		public bool HasSharePoint2010DocumentSetFeature
		{
			get
			{
				if (!this.m_bHasSharePoint2010DocumentSetFeature.HasValue)
				{
					bool flag = false;
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
					if (attributeValueFromFullXml != null)
					{
						char[] chrArray = new char[] { ',' };
						List<string> strs = new List<string>(attributeValueFromFullXml.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
						flag = strs.Contains("3bae86a2-776d-499d-9db8-fa4cdc7884f8");
					}
					else
					{
						flag = false;
					}
					this.m_bHasSharePoint2010DocumentSetFeature = new bool?(flag);
				}
				return this.m_bHasSharePoint2010DocumentSetFeature.Value;
			}
		}

		public bool HasSharePointDocumentIDFeature
		{
			get
			{
				bool flag;
				string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
				if (string.IsNullOrEmpty(attributeValueFromFullXml))
				{
					flag = false;
				}
				else
				{
					char[] chrArray = new char[] { ',' };
					List<string> strs = new List<string>(attributeValueFromFullXml.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
					flag = strs.Contains("b50e3104-6812-424f-a011-cc90e6327318");
				}
				return flag;
			}
		}

		public bool HasUniquePermissions
		{
			get
			{
				bool value;
				lock (this.m_oLockUniquePermissions)
				{
					if (!this.m_bHasUniquePermissions.HasValue)
					{
						this.m_bHasUniquePermissions = new bool?(bool.Parse(this.GetFullXML(false).Attributes["HasUniquePermissions"].Value));
					}
					value = this.m_bHasUniquePermissions.Value;
				}
				return value;
			}
		}

		public bool HasUniqueRoles
		{
			get
			{
				string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("HasUniqueRoles");
				return (string.IsNullOrEmpty(attributeValueFromFullXml) ? false : bool.Parse(attributeValueFromFullXml));
			}
		}

		public string ID
		{
			get
			{
				string mID;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mID = this.m_ID;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mID;
			}
		}

		[IsSystem(true)]
		public override string ImageName
		{
			get
			{
				string imageName;
				if ((base.Parent == null || base.Parent is SPServer ? false : !(base.Parent is SPTenant)))
				{
					imageName = base.ImageName;
				}
				else
				{
					string str = "Connecting";
					if ((base.Status == ConnectionStatus.Valid || base.Status == ConnectionStatus.Invalid ? true : base.Status == ConnectionStatus.Warning))
					{
						str = base.Status.ToString();
					}
					string empty = string.Empty;
					if ((base.Status != ConnectionStatus.Valid || !base.Adapter.IsReadOnly() ? false : !base.Adapter.IsDB))
					{
						empty = "-ReadOnly";
					}
					imageName = string.Format("Metalogix.SharePoint.Icons.SPWeb-{0}-{1}{2}.ico", base.Adapter.DisplayedShortName, str, empty);
				}
				return imageName;
			}
		}

		public bool IncludePreviousWorkflowVersions
		{
			get
			{
				return this._includePreviousWorkflowVersions;
			}
			set
			{
				this._includePreviousWorkflowVersions = value;
			}
		}

		public override bool IsConnectionRoot
		{
			get
			{
				return (base.Parent == null ? true : base.Parent is SPBaseServer);
			}
		}

		public bool IsMySiteTemplate
		{
			get
			{
				return this.Template.Name.StartsWith("SPSPERS", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		public bool IsPublishingTemplate
		{
			get
			{
				return (this.Template.Name == "CMSPUBLISHING#0" || this.Template.Name == "BLANKINTERNET#0" || this.Template.Name == "BLANKINTERNETCONTAINER#0" ? true : this.Template.Name == "BLANKINTERNET#2");
			}
		}

		public bool IsReadOnly
		{
			get
			{
				string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("IsReadOnly");
				return (string.IsNullOrEmpty(attributeValueFromFullXml) ? false : bool.Parse(attributeValueFromFullXml));
			}
		}

		public bool IsRootWeb
		{
			get
			{
				return ((string.IsNullOrEmpty(this.SiteCollectionServerRelativeUrl) ? true : !(this.SiteCollectionServerRelativeUrl.ToLower() == this.ServerRelativeUrl.ToLower())) ? false : true);
			}
		}

		public bool IsSearchable
		{
			get
			{
				bool mBIsSearchable;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mBIsSearchable = this.m_bIsSearchable;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mBIsSearchable;
			}
		}

		public bool IsSharePoint2013WorkflowsAvailable
		{
			get
			{
				List<string> strs;
				bool value;
				char[] chrArray;
				if (base.Adapter.SharePointVersion.IsSharePoint2013OrLater)
				{
					if (!this.m_bSharePoint2013WorkflowsFeature.HasValue)
					{
						bool flag = false;
						string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
						if (attributeValueFromFullXml != null)
						{
							chrArray = new char[] { ',' };
							strs = new List<string>(attributeValueFromFullXml.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
							flag = strs.Contains("0af5989a-3aea-4519-8ab0-85d91abe39ff");
						}
						if (!flag)
						{
							string str = this.GetAttributeValueFromFullXml("SiteFeatures");
							if (str != null)
							{
								chrArray = new char[] { ',' };
								strs = new List<string>(str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
								flag = strs.Contains("00bfea71-4ea5-48d4-a4ad-305cf7030140");
							}
						}
						if (flag)
						{
							flag = this.SP2013WorkflowCollection.IsWorkflowServicesInstanceAvailable();
						}
						this.m_bSharePoint2013WorkflowsFeature = new bool?(flag);
					}
					value = this.m_bSharePoint2013WorkflowsFeature.Value;
				}
				else
				{
					value = false;
				}
				return value;
			}
		}

		public bool IsWikiTemplate
		{
			get
			{
				return this.Template.Name == "ENTERWIKI#0";
			}
		}

		public int Language
		{
			get
			{
				int mILanguage;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mILanguage = this.m_iLanguage;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mILanguage;
			}
		}

		public List<ViewLanguageResource> LanguageResourcesForViews
		{
			get
			{
				if (this._languageResourcesForViews == null)
				{
					this._languageResourcesForViews = new List<ViewLanguageResource>();
				}
				return this._languageResourcesForViews;
			}
		}

		public DateTime LastItemModified
		{
			get
			{
				DateTime createdDate;
				DateTime dateTime;
				if (!Utils.TryParseDateAsUtc(this.GetAttributeValueFromFullXml("LastItemModifiedDate"), out createdDate))
				{
					createdDate = this.CreatedDate;
					foreach (SPList list in this.Lists)
					{
						if (list.Modified > createdDate)
						{
							createdDate = list.Modified;
						}
					}
					dateTime = createdDate;
				}
				else
				{
					dateTime = createdDate;
				}
				return dateTime;
			}
		}

		public SPListCollection Lists
		{
			get
			{
				lock (this.m_oLockLists)
				{
					if (this.m_lists == null)
					{
						this.m_lists = new SPListCollection(this);
						this.m_lists.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_lists_CollectionChanged);
						try
						{
							DateTime now = DateTime.Now;
							this.m_lists.FetchData();
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							throw new ServerProblem(string.Concat("Exception thrown fetching Lists on Web: '", this.DisplayUrl, "' ", exception.Message));
						}
					}
				}
				return this.m_lists;
			}
		}

		public SPList ListTemplateGallery
		{
			get
			{
				SPList sPList;
				lock (this._listTemplateGalleryLock)
				{
					if (this._listTemplateGallery == null)
					{
						string lists = base.Adapter.Reader.GetLists();
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(lists);
						foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes(".//List"))
						{
							if (xmlNodes.Attributes["Name"].Value.Equals("lt"))
							{
								this._listTemplateGallery = new SPList(this, xmlNodes);
								break;
							}
						}
					}
					sPList = this._listTemplateGallery;
				}
				return sPList;
			}
		}

		public SPListTemplateCollection ListTemplates
		{
			get
			{
				SPListTemplateCollection mListTemplates;
				lock (this._listTemplatesLock)
				{
					if (this.m_listTemplates == null)
					{
						try
						{
							this.m_listTemplates = this.GetListTemplates(false);
						}
						catch (Exception exception)
						{
							throw new Exception(string.Concat("Could not fetch list templates: ", exception.Message));
						}
					}
					mListTemplates = this.m_listTemplates;
				}
				return mListTemplates;
			}
		}

		public int Locale
		{
			get
			{
				int mILocale;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mILocale = this.m_iLocale;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mILocale;
			}
		}

		public SPMasterPageGallery MasterPageGallery
		{
			get
			{
				SPMasterPageGallery mMasterPageGallery;
				lock (this._masterPageGalleryLock)
				{
					if (this.m_masterPageGallery == null)
					{
						string lists = base.Adapter.Reader.GetLists();
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(lists);
						foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes(".//List"))
						{
							if (xmlNodes.Attributes["Name"].Value.Equals("masterpage"))
							{
								this.m_masterPageGallery = new SPMasterPageGallery(this, xmlNodes);
								break;
							}
						}
					}
					mMasterPageGallery = this.m_masterPageGallery;
				}
				return mMasterPageGallery;
			}
		}

		public string MasterPageUrl
		{
			get
			{
				return this.GetAttributeValueFromFullXml("MasterPage");
			}
		}

		public int MaximumFileSize
		{
			get
			{
				int mIMaximumFileSize;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mIMaximumFileSize = this.m_iMaximumFileSize;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mIMaximumFileSize;
			}
		}

		public SPMeetingInstanceCollection MeetingInstances
		{
			get
			{
				SPMeetingInstanceCollection mMeetingInstances;
				lock (this._meetingInstLock)
				{
					if (this.m_meetingInstances == null)
					{
						this.m_meetingInstances = this.GetMeetingInstances(false);
					}
					mMeetingInstances = this.m_meetingInstances;
				}
				return mMeetingInstances;
			}
		}

		public override string Name
		{
			get
			{
				string mSName;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mSName = this.m_sName;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mSName;
			}
		}

		public SPNavigationRoot Navigation
		{
			get
			{
				SPNavigationRoot mNavigation;
				lock (this._navigationLock)
				{
					if (this.m_navigation == null)
					{
						this.m_navigation = new SPNavigationRoot(this);
					}
					mNavigation = this.m_navigation;
				}
				return mNavigation;
			}
		}

		public SPList PagesLibrary
		{
			get
			{
				SPList mPagesLibrary;
				lock (this._pagesLibraryLock)
				{
					if (this.m_PagesLibrary == null)
					{
						foreach (SPList list in this.Lists)
						{
							if (list.BaseTemplate == ListTemplateType.O12Pages)
							{
								this.m_PagesLibrary = list;
								break;
							}
						}
					}
					mPagesLibrary = this.m_PagesLibrary;
				}
				return mPagesLibrary;
			}
		}

		public bool ParserEnabled
		{
			get
			{
				string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("ParserEnabled");
				return (string.IsNullOrEmpty(attributeValueFromFullXml) ? true : bool.Parse(attributeValueFromFullXml));
			}
		}

		public SPPortalListingCollection PortalListings
		{
			get
			{
				SPPortalListingCollection mPortalListings;
				if (base.Adapter.IsPortal2003Connection)
				{
					lock (this._portalListingsLock)
					{
						if (this.m_portalListings == null)
						{
							this.m_portalListings = new SPPortalListingCollection(this);
						}
						mPortalListings = this.m_portalListings;
					}
				}
				else
				{
					mPortalListings = null;
				}
				return mPortalListings;
			}
		}

		public SecurityPrincipalCollection Principals
		{
			get
			{
				SecurityPrincipalCollection[] siteUsers = new SecurityPrincipalCollection[] { this.SiteUsers, this.Groups };
				return new MultiTypeSecurityPrincipalCollection(siteUsers);
			}
		}

		public bool PublishingInfrastructureActive
		{
			get
			{
				bool value;
				if (!this.m_bPublishingInfrastructureActive.HasValue)
				{
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
					if (attributeValueFromFullXml == null)
					{
						value = false;
						return value;
					}
					string lower = attributeValueFromFullXml.ToLower();
					char[] chrArray = new char[] { ',' };
					List<string> strs = new List<string>(lower.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
					bool flag = true;
					string[] pUBLISHINGINFRASTRUCTUREGUIDS = SPWeb.PUBLISHING_INFRASTRUCTURE_GUIDS;
					for (int i = 0; i < (int)pUBLISHINGINFRASTRUCTUREGUIDS.Length; i++)
					{
						flag &= strs.Contains(pUBLISHINGINFRASTRUCTUREGUIDS[i]);
					}
					this.m_bPublishingInfrastructureActive = new bool?(flag);
				}
				value = this.m_bPublishingInfrastructureActive.Value;
				return value;
			}
		}

		public string QuotaID
		{
			get
			{
				return this.GetAttributeValueFromFullXml("QuotaID");
			}
		}

		public int QuotaStorageLimit
		{
			get
			{
				int num;
				int.TryParse(this.GetAttributeValueFromFullXml("QuotaStorageLimit"), out num);
				return num;
			}
		}

		public int QuotaStorageWarning
		{
			get
			{
				int num;
				int.TryParse(this.GetAttributeValueFromFullXml("QuotaStorageWarning"), out num);
				return num;
			}
		}

		public RoleAssignmentCollection RoleAssignments
		{
			get
			{
				RoleAssignmentCollection mRoleAssignments;
				lock (this.m_oLockRoleAssignments)
				{
					if (this.m_roleAssignments == null)
					{
						this.m_roleAssignments = this.GetRoleAssignmentCollection(false);
						this.m_roleAssignments.RoleAssignmentCollectionChanged += new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
					}
					mRoleAssignments = this.m_roleAssignments;
				}
				return mRoleAssignments;
			}
		}

		public RoleCollection Roles
		{
			get
			{
				RoleCollection mRoles;
				lock (this.m_oLockRoles)
				{
					if (this.m_roles == null)
					{
						this.m_roles = this.GetRoles(false);
					}
					mRoles = this.m_roles;
				}
				return mRoles;
			}
		}

		public SPFolderBasic RootFolder
		{
			get
			{
				SPFolderBasic mRootFolder;
				lock (this._rootFolderLock)
				{
					if (this.m_rootFolder == null)
					{
						this.m_rootFolder = new SPFolderBasic(this);
					}
					mRootFolder = this.m_rootFolder;
				}
				return mRootFolder;
			}
		}

		public SPSite RootSite
		{
			get
			{
				SPSite mCachedRootSite;
				SPSite cachedRootSite = this.CachedRootSite;
				if (cachedRootSite == null)
				{
					SPWeb rootWeb = this.RootWeb;
					if (!(rootWeb is SPSite))
					{
						lock (this.m_oLockCachedRootSite)
						{
							string site = base.Adapter.Reader.GetSite(false);
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(site);
							XmlNode xmlNodes = xmlDocument.SelectSingleNode("//Site");
							SharePointAdapter value = base.Adapter.Clone();
							value.Url = xmlNodes.Attributes["Url"].Value;
							value.WebID = xmlNodes.Attributes["ID"].Value;
							this.m_cachedRootSite = new SPSite(value, null, xmlNodes);
							mCachedRootSite = this.m_cachedRootSite;
						}
					}
					else
					{
						mCachedRootSite = (SPSite)rootWeb;
					}
				}
				else
				{
					mCachedRootSite = cachedRootSite;
				}
				return mCachedRootSite;
			}
		}

		public string RootSiteGUID
		{
			get
			{
				string str;
				SPWeb parent = base.Parent as SPWeb;
				str = ((this.HasFullXML || parent == null ? true : this.IsConnectionRoot) ? this.GetAttributeValueFromFullXml("RootSiteGUID") : parent.RootSiteGUID);
				return str;
			}
		}

		public SPWeb RootWeb
		{
			get
			{
				return ((base.Parent == null ? false : !(base.Parent is SPBaseServer)) ? ((SPWeb)base.Parent).RootWeb : this);
			}
		}

		public string RootWebGUID
		{
			get
			{
				string str;
				SPWeb parent = base.Parent as SPWeb;
				str = ((this.HasFullXML || parent == null ? true : this.IsConnectionRoot) ? this.GetAttributeValueFromFullXml("RootWebGUID") : parent.RootWebGUID);
				return str;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				string empty;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					if (this.m_sServerRelativeUrl == null)
					{
						this.EnsureQuickProperties();
						if (this.m_sServerRelativeUrl == null)
						{
							empty = string.Empty;
							return empty;
						}
					}
					empty = this.m_sServerRelativeUrl;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return empty;
			}
		}

		public string SiteCollectionServerRelativeUrl
		{
			get
			{
				return this.GetAttributeValueFromFullXml("SiteCollectionServerRelativeUrl");
			}
		}

		public SPFieldCollection SiteColumns
		{
			get
			{
				SPFieldCollection mSiteColumns;
				lock (this._siteColumnLock)
				{
					if (this.m_siteColumns == null)
					{
						this.m_siteColumns = this.GetSiteColumns(false);
						this.m_siteColumns.FieldCollectionChanged += new SPFieldCollection.FieldCollectionChangeEventHandler(this.FieldCollectionChangedHandler);
					}
					mSiteColumns = this.m_siteColumns;
				}
				return mSiteColumns;
			}
		}

		public SPUserCollection SiteUsers
		{
			get
			{
				return this.GetSiteUsers(false);
			}
		}

		public Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection SP2013WorkflowCollection
		{
			get
			{
				Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sP2013WorkflowCollection = this.sp2013WorkflowCollection;
				if (sP2013WorkflowCollection == null)
				{
					Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sP2013WorkflowCollection1 = new Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection(base.Adapter, SP2013WorkflowScopeType.Site);
					Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sP2013WorkflowCollection2 = sP2013WorkflowCollection1;
					this.sp2013WorkflowCollection = sP2013WorkflowCollection1;
					sP2013WorkflowCollection = sP2013WorkflowCollection2;
				}
				return sP2013WorkflowCollection;
			}
			set
			{
				this.sp2013WorkflowCollection = value;
			}
		}

		public bool StoragePointPresent
		{
			get
			{
				bool value;
				if (this.m_parentWeb == null)
				{
					if (!this.m_bStoragePointPresent.HasValue)
					{
						bool flag = false;
						bool.TryParse(base.Adapter.Reader.StoragePointAvailable(string.Empty), out flag);
						this.m_bStoragePointPresent = new bool?(flag);
					}
					value = this.m_bStoragePointPresent.Value;
				}
				else
				{
					value = this.m_parentWeb.StoragePointPresent;
				}
				return value;
			}
		}

		public SPWebCollection SubWebs
		{
			get
			{
				SPWebCollection mSubWebs;
				lock (this.m_oLockSubwebs)
				{
					if (this.m_subWebs == null)
					{
						this.m_subWebs = new SPWebCollection(this);
						this.m_subWebs.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_subWebs_CollectionChanged);
						this.m_subWebs.FetchData();
					}
					mSubWebs = this.m_subWebs;
				}
				return mSubWebs;
			}
		}

		public string TaxonomyListGUID
		{
			get
			{
				return this.GetAttributeValueFromFullXml("TaxonomyListGUID");
			}
		}

		public SPWebTemplate Template
		{
			get
			{
				return this.GetWebTemplate(true);
			}
		}

		public SPWebTemplateCollection Templates
		{
			get
			{
				SPWebTemplateCollection templates;
				if ((base.Parent == null ? false : !(base.Parent is SPBaseServer)))
				{
					templates = ((SPWeb)base.Parent).Templates;
				}
				else
				{
					lock (this._templatesLock)
					{
						if (this.m_Templates == null)
						{
							try
							{
								SPWebTemplateCollection sPWebTemplateCollections = new SPWebTemplateCollection(this);
								sPWebTemplateCollections.FetchData();
								this.m_Templates = sPWebTemplateCollections;
							}
							catch
							{
								templates = new SPWebTemplateCollection(this);
								return templates;
							}
						}
						templates = this.m_Templates;
					}
				}
				return templates;
			}
		}

		public SPTermStoreCollection TermStores
		{
			get
			{
				SPTermStoreCollection termStores;
				if ((this.m_parentWeb == null ? true : this.m_bInUseAsSource.HasValue))
				{
					lock (this._termStoresLock)
					{
						if (this._termStores == null)
						{
							SPTermStoreCollection sPTermStoreCollection = new SPTermStoreCollection(this);
							sPTermStoreCollection.FetchData();
							this._termStores = sPTermStoreCollection;
						}
						termStores = this._termStores;
					}
				}
				else
				{
					termStores = this.m_parentWeb.TermStores;
				}
				return termStores;
			}
		}

		public TimeZoneInformation TimeZone
		{
			get
			{
				TimeZoneInformation mTimeZone;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mTimeZone = this.m_timeZone;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mTimeZone;
			}
		}

		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
		}

		public string UIVersion
		{
			get
			{
				return this.GetAttributeValueFromFullXml("UIVersion");
			}
		}

		public SPList UserInformationList
		{
			get
			{
				SPList sPList;
				lock (this._userInformationList)
				{
					if (this.userInformationList == null)
					{
						string lists = base.Adapter.Reader.GetLists();
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(lists);
						foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes(".//List"))
						{
							ListTemplateType listTemplateType = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), xmlNodes.Attributes["BaseTemplate"].Value);
							if ((xmlNodes.Attributes["Name"].Value != "users" ? false : listTemplateType == ListTemplateType.UserInformation))
							{
								this.userInformationList = new SPList(this, xmlNodes);
								break;
							}
						}
					}
					sPList = this.userInformationList;
				}
				return sPList;
			}
		}

		public Guid WebApplicationId
		{
			get
			{
				Guid mWebApplicationId;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mWebApplicationId = this.m_webApplicationId;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mWebApplicationId;
			}
		}

		public virtual string WebName
		{
			get
			{
				return this.Name;
			}
		}

		public int WebTemplateID
		{
			get
			{
				int mIWebTemplateID;
				this._lockQuickProperties.AcquireReaderLock(-1);
				try
				{
					this.EnsureQuickProperties();
					mIWebTemplateID = this.m_iWebTemplateID;
				}
				finally
				{
					this._lockQuickProperties.ReleaseReaderLock();
				}
				return mIWebTemplateID;
			}
		}

		public SPWebPartPage WelcomePage
		{
			get
			{
				return (new SPWebPartPage()).GetWelcomePage(this, null);
			}
		}

		public bool WelcomePageInPagesLibrary
		{
			get
			{
				bool flag = false;
				if ((this.WelcomePageUrl == null ? false : this.PagesLibrary != null))
				{
					string welcomePageUrl = this.WelcomePageUrl;
					char[] chrArray = new char[] { '/', '\\' };
					string[] strArrays = welcomePageUrl.Split(chrArray);
					int length = (int)strArrays.Length;
					if ((length < 2 || !strArrays[length - 1].ToLower().EndsWith(".aspx") ? false : strArrays[length - 2].ToLower() == this.PagesLibrary.Name.ToLower()))
					{
						flag = true;
					}
				}
				return flag;
			}
		}

		public string WelcomePageUrl
		{
			get
			{
				string str;
				string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("WelcomePage");
				if (string.IsNullOrEmpty(attributeValueFromFullXml))
				{
					str = attributeValueFromFullXml;
				}
				else
				{
					string[] serverRelativeUrl = new string[] { this.ServerRelativeUrl, attributeValueFromFullXml };
					str = UrlUtils.ConcatUrls(serverRelativeUrl);
				}
				return str;
			}
		}

		public bool WikiWelcomePageActive
		{
			get
			{
				if (!this.m_bWikiWelcomePageActive.HasValue)
				{
					bool flag = false;
					string attributeValueFromFullXml = this.GetAttributeValueFromFullXml("SiteCollFeatures");
					if (attributeValueFromFullXml != null)
					{
						string lower = attributeValueFromFullXml.ToLower();
						char[] chrArray = new char[] { ',' };
						List<string> strs = new List<string>(lower.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
						flag = strs.Contains("8c6a6980-c3d9-440e-944c-77f93bc65a7e");
					}
					else
					{
						flag = false;
					}
					this.m_bWikiWelcomePageActive = new bool?(flag);
				}
				return this.m_bWikiWelcomePageActive.Value;
			}
		}

		public SPWorkflowAssociationCollection WorkflowAssociations
		{
			get
			{
				SPWorkflowAssociationCollection mWorkflowAssociations;
				lock (this._workflowAscLock)
				{
					if (this.m_WorkflowAssociations == null)
					{
						this.m_WorkflowAssociations = this.GetWorkflowAssociations(false);
					}
					mWorkflowAssociations = this.m_WorkflowAssociations;
				}
				return mWorkflowAssociations;
			}
		}

		public override string XML
		{
			get
			{
				return this.FullXML.OuterXml;
			}
		}

		static SPWeb()
		{
			SPWeb.SHOW_WEB_URL = false;
			string[] strArrays = new string[] { "a392da98-270b-4e85-9769-04c0fde267aa", "f6924d36-2fa8-4f0b-b16d-06b7250180fa", "89e0306d-453b-4ec5-8d68-42067cdbf98e", "d3f51be2-38a8-4e44-ba84-940d35be1566", "aebc918d-b20f-4a11-a1db-9ed84d79c87e" };
			SPWeb.PUBLISHING_INFRASTRUCTURE_GUIDS = strArrays;
			strArrays = new string[] { "IncludeSubSitesInNavigation", "IncludePagesInNavigation", "NavigationSortAscending", "NavigationShowSiblings", "DisplayShowHideRibbonActionId", "NavigationOrderingMethod", "NavigationAutomaticSortingMethod", "GlobalDynamicChildLimit", "CurrentDynamicChildLimit", "GlobalNavigationIncludeTypes", "CurrentNavigationIncludeTypes", "QuickLaunchEnabled", "TreeViewEnabled" };
			SPWeb.s_navigationAttributes = strArrays;
			strArrays = new string[] { "InheritGlobalNavigation", "InheritCurrentNavigation" };
			SPWeb.s_rootWebNavigationAttributes = strArrays;
		}

		public SPWeb(XmlNode connectionNode) : base(connectionNode)
		{
		}

		public SPWeb(SharePointAdapter adapter) : base(adapter, null)
		{
		}

		public SPWeb(SharePointAdapter adapter, SPNode parent) : base(adapter, parent)
		{
		}

		public SPWeb(SPWeb parentWeb, SharePointAdapter adapter, XmlNode webXML) : base(adapter, parentWeb)
		{
			this.m_parentWeb = parentWeb;
			this.StoreQuickProperties(webXML);
		}

		public override bool AnalyzeChurn(DateTime pivotDate, bool bRecursive, out long lByteschanged, out long lItemsChanged)
		{
			bool flag;
			lByteschanged = (long)0;
			lItemsChanged = (long)0;
			if (this.IsSearchable)
			{
				string str = base.Adapter.Reader.AnalyzeChurn(pivotDate, null, -1, bRecursive);
				if (!string.IsNullOrEmpty(str))
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
					string str1 = (xmlNode.Attributes["ItemsChanged"] != null ? xmlNode.Attributes["ItemsChanged"].Value : "0");
					string str2 = (xmlNode.Attributes["BytesChanged"] != null ? xmlNode.Attributes["BytesChanged"].Value : "0");
					bool flag1 = long.TryParse(str1, out lItemsChanged);
					flag1 = long.TryParse(str2, out lByteschanged);
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		public void Apply2013Theme()
		{
			char[] chrArray;
			if (base.Adapter.Writer == null)
			{
				throw new Exception(Resources.TargetIsReadOnly);
			}
			SPListItem currentItem = this.ComposedLooksGallery.CurrentItem;
			string serverRelative = null;
			string str = null;
			string serverRelative1 = null;
			if (string.IsNullOrEmpty(currentItem["ThemeUrl"]))
			{
				serverRelative = string.Concat(this.SiteCollectionServerRelativeUrl, "/_catalogs/theme/15/Palette001.spcolor");
			}
			else
			{
				string item = currentItem["ThemeUrl"];
				chrArray = new char[] { ',' };
				serverRelative = item.Split(chrArray)[0];
			}
			if (!string.IsNullOrEmpty(currentItem["FontSchemeUrl"]))
			{
				string item1 = currentItem["FontSchemeUrl"];
				chrArray = new char[] { ',' };
				str = item1.Split(chrArray)[0];
			}
			if (!string.IsNullOrEmpty(currentItem["ImageUrl"]))
			{
				string str1 = currentItem["ImageUrl"];
				chrArray = new char[] { ',' };
				serverRelative1 = str1.Split(chrArray)[0];
			}
			serverRelative = StandardizedUrl.StandardizeUrl(base.Adapter, serverRelative).ServerRelative;
			str = StandardizedUrl.StandardizeUrl(base.Adapter, str).ServerRelative;
			serverRelative1 = StandardizedUrl.StandardizeUrl(base.Adapter, serverRelative1).ServerRelative;
			base.Adapter.Writer.Apply2013Theme(serverRelative, str, serverRelative1);
		}

		public void AquireWebPartLock()
		{
			Monitor.Enter(this._webPartLock);
		}

		protected void AttachVirtualColumnData(XmlNode fetchedNode, bool bAllAvailable)
		{
			if (this.WriteVirtually)
			{
				if (bAllAvailable)
				{
					SPWeb parent = base.Parent as SPWeb;
					if (parent != null)
					{
						parent.AttachVirtualColumnData(fetchedNode, bAllAvailable);
					}
				}
				Metalogix.Metabase.Record virtualRecord = base.GetVirtualRecord();
				if (virtualRecord != null)
				{
					RecordPropertyValue item = virtualRecord.Properties["Columns"];
					if (item != null)
					{
						TextMoniker value = item.Value as TextMoniker;
						if (value != null)
						{
							string fullText = value.GetFullText();
							if (!string.IsNullOrEmpty(fullText))
							{
								SPFieldCollection.UpdateFieldsXML(fetchedNode, XmlUtility.StringToXmlNode(fullText));
							}
						}
					}
				}
			}
		}

		protected override void ClearChildNodes()
		{
			lock (this.m_oLockLists)
			{
				if (this.m_lists != null)
				{
					this.m_lists.OnNodeCollectionChanged -= new NodeCollectionChangedHandler(this.On_lists_CollectionChanged);
					this.m_lists = null;
				}
			}
			lock (this.m_oLockSubwebs)
			{
				if (this.m_subWebs != null)
				{
					this.m_subWebs.OnNodeCollectionChanged -= new NodeCollectionChangedHandler(this.On_subWebs_CollectionChanged);
					this.m_subWebs = null;
				}
			}
			if (this.m_WorkflowAssociations != null)
			{
				this.m_WorkflowAssociations = null;
			}
			this.m_bHasPublishingFeature = null;
			this.m_bPublishingInfrastructureActive = null;
			this.m_bHasFormsService = null;
		}

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			lock (this.m_oLockCachedRootSite)
			{
				this.m_cachedRootSite = null;
			}
			this.ReleasePermissionsData(false);
			lock (this._contentTypesLock)
			{
				this.m_contentTypes = null;
			}
			this.ClearHeldWebXml();
			lock (this._listTemplatesLock)
			{
				this.m_listTemplates = null;
			}
			lock (this._siteColumnLock)
			{
				if (this.m_siteColumns != null)
				{
					this.m_siteColumns.FieldCollectionChanged -= new SPFieldCollection.FieldCollectionChangeEventHandler(this.FieldCollectionChangedHandler);
				}
				this.m_siteColumns = null;
			}
			lock (this._availableColumnsLock)
			{
				if (this.m_availableColumns != null)
				{
					this.m_availableColumns.FieldCollectionChanged -= new SPFieldCollection.FieldCollectionChangeEventHandler(this.FieldCollectionChangedHandler);
				}
				this.m_availableColumns = null;
			}
			lock (this._audienceCollectionLock)
			{
				this.m_audienceCollection = null;
			}
			lock (this._meetingInstLock)
			{
				this.m_meetingInstances = null;
			}
			this.m_bSiteCannotContainMeetingInstances = false;
			lock (this._masterPageGalleryLock)
			{
				this.m_masterPageGallery = null;
			}
			lock (this._navigationLock)
			{
				this.m_navigation = null;
			}
			lock (this._portalListingsLock)
			{
				this.m_portalListings = null;
			}
			lock (this._termStoresLock)
			{
				this._termStores = null;
			}
			lock (this._alertsLock)
			{
				this.m_alerts = null;
			}
			lock (this._externalContentTypesLock)
			{
				this.m_externalContentTypes = null;
			}
			lock (this._blogPostsListLock)
			{
				this.m_BlogPostsList = null;
			}
			lock (this._rootFolderLock)
			{
				this.m_rootFolder = null;
			}
			lock (this._pagesLibraryLock)
			{
				this.m_PagesLibrary = null;
			}
			lock (this._portalListingsLock)
			{
				this.m_portalListings = null;
			}
			lock (this._templatesLock)
			{
				this.m_Templates = null;
			}
			lock (this._termStoresLock)
			{
				this._termStores = null;
			}
			lock (this._workflowAscLock)
			{
				this.m_WorkflowAssociations = null;
			}
			this.m_adapter.Refresh();
		}

		protected void ClearHeldWebXml()
		{
			lock (this._webXmlLock)
			{
				this.m_webXML = null;
			}
		}

		private string ConstructSiteColumnFieldSchemaXml(XmlNode fieldSchemaXml)
		{
			string str;
			XmlNodeList xmlNodeLists = fieldSchemaXml.SelectNodes(string.Format("./Field[@SourceID='{0}']", string.Concat("{", this.ID, "}")));
			if (xmlNodeLists.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
				{
					OmitXmlDeclaration = true
				};
				XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting);
				try
				{
					xmlWriter.WriteStartElement("Fields");
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						xmlWriter.WriteRaw(xmlNodes.OuterXml);
					}
					xmlWriter.WriteEndElement();
					xmlWriter.Flush();
				}
				finally
				{
					if (xmlWriter != null)
					{
						((IDisposable)xmlWriter).Dispose();
					}
				}
				str = stringBuilder.ToString();
			}
			else
			{
				str = string.Empty;
			}
			return str;
		}

		public void Delete()
		{
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				base.Adapter.Writer.DeleteWeb(this.ServerRelativeUrl);
			}
		}

		public void EnsureLatestNavigationData()
		{
			lock (this._navigationLock)
			{
				if (this.m_navigation != null)
				{
					this.Navigation.RefreshData();
				}
				else
				{
					SPNavigationRoot navigation = this.Navigation;
				}
			}
		}

		protected virtual bool EnsureQuickProperties()
		{
			bool flag;
			try
			{
				if (this.m_ID == null)
				{
					LockCookie writerLock = new LockCookie();
					if (!this._lockQuickProperties.IsReaderLockHeld)
					{
						this._lockQuickProperties.AcquireWriterLock(-1);
					}
					else
					{
						writerLock = this._lockQuickProperties.UpgradeToWriterLock(-1);
					}
					try
					{
						if (this.m_ID == null)
						{
							if (!this.HasFullXML)
							{
								this.StoreQuickProperties(this.GetTerseData());
							}
							else
							{
								this.StoreQuickProperties(this.GetFullXML(false));
							}
							flag = true;
							return flag;
						}
					}
					finally
					{
						if (!(writerLock != new LockCookie()))
						{
							this._lockQuickProperties.ReleaseWriterLock();
						}
						else
						{
							this._lockQuickProperties.DowngradeFromWriterLock(ref writerLock);
						}
					}
				}
				flag = false;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		protected override Metalogix.Explorer.Node[] FetchChildNodes()
		{
			int i;
			SPUserCollection siteUsers = this.SiteUsers;
			this.EnsureQuickProperties();
			int count = this.SubWebs.Count + this.Lists.Count;
			Metalogix.Explorer.Node[] item = new Metalogix.Explorer.Node[count];
			for (i = 0; i < this.SubWebs.Count; i++)
			{
				item[i] = (Metalogix.Explorer.Node)this.SubWebs[i];
			}
			for (i = 0; i < this.Lists.Count; i++)
			{
				item[i + this.SubWebs.Count] = (Metalogix.Explorer.Node)this.Lists[i];
			}
			return item;
		}

		protected virtual string FetchData()
		{
			return base.Adapter.Reader.GetWeb(true);
		}

		private void FieldCollectionChangedHandler(SPFieldCollection sender, XmlNode fieldSchemaXml)
		{
			lock (this._availableColumnsLock)
			{
				if ((this.m_availableColumns == null ? false : sender == this.m_availableColumns))
				{
					lock (this._siteColumnLock)
					{
						if (this.m_siteColumns != null)
						{
							this.m_siteColumns.UpdateInternalFieldSchema(this.ConstructSiteColumnFieldSchemaXml(fieldSchemaXml));
							return;
						}
					}
				}
			}
			lock (this._siteColumnLock)
			{
				if ((this.m_siteColumns == null ? false : sender == this.m_siteColumns))
				{
					lock (this._availableColumnsLock)
					{
						if (this.m_availableColumns != null)
						{
							this.m_availableColumns.UpdateInternalFieldSchema(this.ConstructSiteColumnFieldSchemaXml(fieldSchemaXml));
							return;
						}
					}
				}
			}
		}

		internal SPAlertCollection GetAlerts(bool bAlwaysRefetch)
		{
			SPAlertCollection mAlerts;
			lock (this._alertsLock)
			{
				if ((this.m_alerts == null ? false : !bAlwaysRefetch))
				{
					mAlerts = this.m_alerts;
					return mAlerts;
				}
			}
			SPAlertCollection sPAlertCollection = new SPAlertCollection(this);
			sPAlertCollection.FetchData();
			mAlerts = sPAlertCollection;
			return mAlerts;
		}

		protected string GetAttributeValueFromFullXml(string atrName)
		{
			string str;
			string value;
			XmlNode fullXML = this.FullXML;
			if (fullXML == null)
			{
				str = null;
			}
			else
			{
				XmlAttribute itemOf = fullXML.Attributes[atrName];
				if (itemOf == null)
				{
					value = null;
				}
				else
				{
					value = itemOf.Value;
				}
				str = value;
			}
			return str;
		}

		internal SPAudienceCollection GetAudienceCollection(bool bAlwaysRefetch)
		{
			SPAudienceCollection mAudienceCollection;
			if (!(base.Parent == null ? false : !this.m_bInUseAsSource.HasValue))
			{
				lock (this._audienceCollectionLock)
				{
					if ((this.m_audienceCollection == null ? false : !bAlwaysRefetch))
					{
						mAudienceCollection = this.m_audienceCollection;
						return mAudienceCollection;
					}
				}
				string audiences = base.Adapter.Reader.GetAudiences();
				if (string.IsNullOrEmpty(audiences))
				{
					mAudienceCollection = null;
				}
				else
				{
					mAudienceCollection = new SPAudienceCollection(this, audiences);
				}
			}
			else if (!(base.Parent is SPServer))
			{
				mAudienceCollection = (!(base.Parent is SPTenant) ? this.RootWeb.GetAudienceCollection(bAlwaysRefetch) : ((SPTenant)base.Parent).GetAudienceCollection(bAlwaysRefetch));
			}
			else
			{
				mAudienceCollection = ((SPServer)base.Parent).GetAudienceCollection(bAlwaysRefetch);
			}
			return mAudienceCollection;
		}

		public SPFieldCollection GetAvailableColumns(bool bAlwaysRefetch)
		{
			SPFieldCollection mAvailableColumns;
			lock (this._availableColumnsLock)
			{
				if ((this.m_availableColumns == null ? false : !bAlwaysRefetch))
				{
					mAvailableColumns = this.m_availableColumns;
					return mAvailableColumns;
				}
			}
			string fields = base.Adapter.Reader.GetFields(null, true);
			XmlNode xmlNode = XmlUtility.StringToXmlNode(fields);
			this.AttachVirtualColumnData(xmlNode, true);
			mAvailableColumns = new SPFieldCollection(this, xmlNode.SelectSingleNode("//Fields"));
			return mAvailableColumns;
		}

		private SPContentTypeCollection GetContentTypes(bool bAlwaysRefetch)
		{
			SPContentTypeCollection mContentTypes;
			if ((this.m_contentTypes == null ? true : bAlwaysRefetch))
			{
				SPContentTypeCollection sPContentTypeCollections = new SPContentTypeCollection(this);
				sPContentTypeCollections.FetchData();
				mContentTypes = sPContentTypeCollections;
			}
			else
			{
				mContentTypes = this.m_contentTypes;
			}
			return mContentTypes;
		}

		protected internal XmlNode GetFullXML(bool bAlwaysRefetch)
		{
			XmlNode xmlNodes;
			XmlNode mWebXML = this.m_webXML;
			if ((mWebXML == null ? true : bAlwaysRefetch))
			{
				lock (this._webXmlLock)
				{
					mWebXML = this.m_webXML;
					if (!(mWebXML == null ? true : bAlwaysRefetch))
					{
						xmlNodes = mWebXML;
						return xmlNodes;
					}
					else if (base.Status != ConnectionStatus.Invalid)
					{
						string str = this.FetchData();
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(str);
						bool flag = mWebXML == null;
						mWebXML = base.AttachVirtualData(xmlDocument.DocumentElement, "XML");
						if ((!flag ? false : mWebXML != null))
						{
							this.StoreQuickProperties(mWebXML);
							this.m_webXML = mWebXML;
						}
					}
				}
				xmlNodes = mWebXML;
			}
			else
			{
				xmlNodes = mWebXML;
			}
			return xmlNodes;
		}

		internal SPGroupCollection GetGroups(bool bAlwaysRefetch)
		{
			SPGroupCollection mGroups;
			if ((this.m_parentWeb == null ? true : this.m_bInUseAsSource.HasValue))
			{
				lock (this.m_oLockGroups)
				{
					if ((this.m_groups == null ? false : !bAlwaysRefetch))
					{
						mGroups = this.m_groups;
						return mGroups;
					}
				}
				SPGroupCollection sPGroupCollection = new SPGroupCollection(this);
				sPGroupCollection.FetchData();
				mGroups = sPGroupCollection;
			}
			else
			{
				mGroups = this.m_parentWeb.GetGroups(bAlwaysRefetch);
			}
			return mGroups;
		}

		internal SPListTemplateCollection GetListTemplates(bool bAlwaysRefetch)
		{
			SPListTemplateCollection mListTemplates;
			if ((this.m_listTemplates == null ? true : bAlwaysRefetch))
			{
				SPListTemplateCollection sPListTemplateCollection = new SPListTemplateCollection(this);
				sPListTemplateCollection.FetchData();
				mListTemplates = sPListTemplateCollection;
			}
			else
			{
				mListTemplates = this.m_listTemplates;
			}
			return mListTemplates;
		}

		public SPMeetingInstanceCollection GetMeetingInstances(bool bAlwaysRefetch)
		{
			SPMeetingInstanceCollection mMeetingInstances;
			if (!(this.m_meetingInstances == null ? true : bAlwaysRefetch))
			{
				mMeetingInstances = this.m_meetingInstances;
			}
			else if (!this.m_bSiteCannotContainMeetingInstances)
			{
				XmlNode xmlNodes = this.GetFullXML(bAlwaysRefetch).SelectSingleNode(".//MeetingInstances");
				if (xmlNodes != null)
				{
					mMeetingInstances = new SPMeetingInstanceCollection(this, xmlNodes);
				}
				else
				{
					this.m_bSiteCannotContainMeetingInstances = true;
					mMeetingInstances = null;
				}
			}
			else
			{
				mMeetingInstances = null;
			}
			return mMeetingInstances;
		}

		public override Metalogix.Explorer.Node GetNodeByUrl(string sURL)
		{
			Metalogix.Explorer.Node node;
			if (!(this.DisplayUrl.ToUpper() == sURL.ToUpper()))
			{
				Metalogix.Explorer.Node nodeByUrl = base.Children.GetNodeByUrl(sURL);
				if ((nodeByUrl != null ? false : base.Adapter.GetServerVersion().StartsWith("10")))
				{
					lock (this.m_oLockSubwebs)
					{
						foreach (SPWeb subWeb in this.SubWebs)
						{
							if (!subWeb.DisplayUrl.ToUpper().StartsWith(this.DisplayUrl.ToUpper()))
							{
								nodeByUrl = subWeb.GetNodeByUrl(sURL);
							}
							if (nodeByUrl != null)
							{
								break;
							}
						}
					}
				}
				node = nodeByUrl;
			}
			else
			{
				node = this;
			}
			return node;
		}

		internal SPRoleAssignmentCollection GetRoleAssignmentCollection(bool bAlwaysRefetch)
		{
			SPRoleAssignmentCollection mRoleAssignments;
			lock (this.m_oLockRoleAssignments)
			{
				if ((this.m_roleAssignments == null ? false : !bAlwaysRefetch))
				{
					mRoleAssignments = this.m_roleAssignments;
					return mRoleAssignments;
				}
			}
			string str = null;
			str = ((this.HasUniquePermissions || this.m_parentWeb == null ? false : !this.m_bInUseAsSource.HasValue) ? this.m_parentWeb.GetRoleAssignmentCollection(bAlwaysRefetch).ToXML() : base.Adapter.Reader.GetRoleAssignments(null, -1));
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			XmlNode xmlNodes = base.AttachVirtualData(xmlDocument.DocumentElement, "RoleAssignments");
			mRoleAssignments = new SPRoleAssignmentCollection(this, xmlNodes);
			return mRoleAssignments;
		}

		internal SPRoleCollection GetRoles(bool bAlwaysRefetch)
		{
			SPRoleCollection mRoles;
			lock (this.m_oLockRoles)
			{
				if ((this.m_roles == null ? false : !bAlwaysRefetch))
				{
					mRoles = this.m_roles;
					return mRoles;
				}
			}
			SPRoleCollection sPRoleCollection = new SPRoleCollection(this);
			sPRoleCollection.FetchData();
			mRoles = sPRoleCollection;
			return mRoles;
		}

		public SPFieldCollection GetSiteColumns(bool bAlwaysRefetch)
		{
			SPFieldCollection mSiteColumns;
			lock (this._siteColumnLock)
			{
				if ((this.m_siteColumns == null ? false : !bAlwaysRefetch))
				{
					mSiteColumns = this.m_siteColumns;
					return mSiteColumns;
				}
			}
			string fields = base.Adapter.Reader.GetFields(null, false);
			XmlNode xmlNode = XmlUtility.StringToXmlNode(fields);
			this.AttachVirtualColumnData(xmlNode, false);
			mSiteColumns = new SPFieldCollection(this, xmlNode.SelectSingleNode("//Fields"));
			return mSiteColumns;
		}

		internal SPUserCollection GetSiteUsers(bool bAlwaysRefetch)
		{
			SPUserCollection mUsers;
			if ((this.m_parentWeb == null ? true : this.m_bInUseAsSource.HasValue))
			{
				lock (this.m_oLockSiteUser)
				{
					if (this.m_users == null)
					{
						this.m_users = new SPUserCollection(this);
						this.m_users.FetchData();
					}
					else if (bAlwaysRefetch)
					{
						this.m_users.FetchData();
					}
					mUsers = this.m_users;
				}
			}
			else
			{
				mUsers = this.m_parentWeb.GetSiteUsers(bAlwaysRefetch);
			}
			return mUsers;
		}

		protected virtual XmlNode GetTerseData()
		{
			XmlNode xmlNode;
			if (base.Status != ConnectionStatus.Invalid)
			{
				xmlNode = XmlUtility.StringToXmlNode(base.Adapter.Reader.GetWeb(false));
			}
			else
			{
				xmlNode = null;
			}
			return xmlNode;
		}

		protected SPWebTemplate GetWebTemplate(bool bForceFetching)
		{
			SPWebTemplate mWebTemplate;
			lock (this._webXmlLock)
			{
				if ((this.HasFullXML || this.m_webTemplate != null ? false : bForceFetching))
				{
					XmlNode fullXML = this.FullXML;
					fullXML = null;
					mWebTemplate = this.m_webTemplate;
				}
				else
				{
					mWebTemplate = this.m_webTemplate;
				}
			}
			return mWebTemplate;
		}

		internal SPWorkflowAssociationCollection GetWorkflowAssociations(bool bAlwaysRefetch)
		{
			SPWorkflowAssociationCollection mWorkflowAssociations;
			lock (this._workflowAscLock)
			{
				if ((this.m_WorkflowAssociations == null ? false : !bAlwaysRefetch))
				{
					mWorkflowAssociations = this.m_WorkflowAssociations;
					return mWorkflowAssociations;
				}
			}
			SPWorkflowAssociationCollection sPWorkflowAssociationCollection = new SPWorkflowAssociationCollection(this);
			sPWorkflowAssociationCollection.FetchData(this._includePreviousWorkflowVersions);
			mWorkflowAssociations = sPWorkflowAssociationCollection;
			return mWorkflowAssociations;
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable comparableNode, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			int num;
			Set<string> set;
			Set<string> set1;
			bool flag1;
			bool flag2;
			bool flag3;
			bool flag4 = true;
			string[] strArrays = new string[] { "InheritsMasterPage", "InheritsCustomMasterPage", "NoCrawl", "AllowRSSFeeds", "QuickLaunchEnabled", "TreeViewEnabled", "RegionalSortOrder", "TimeZone", "Calendar", "AlternateCalendar", "FirstDayOfWeek", "ShowWeeks", "FirstWeekOfYear", "WorkDays", "WorkDayStartHour", "WorkDayEndHour", "TimeFormat", "ParserEnabled", "AdjustHijriDays", "ASPXPageIndexMode", "PublishingFeatureActivated", "InheritGlobalNavigation", "RequestAccessEnabled", "RequestAccessEmail", "QuotaID", "QuotaStorageLimit", "QuotaStorageWarning", "InheritsAlternateCssUrl", "AlternateCssUrl" };
			string[] strArrays1 = strArrays;
			strArrays = new string[] { "InheritsMasterPage", "InheritsCustomMasterPage", "NoCrawl", "AllowRSSFeeds", "QuickLaunchEnabled", "TreeViewEnabled", "Locale", "RegionalSortOrder", "TimeZone", "Calendar", "AlternateCalendar", "FirstDayOfWeek", "ShowWeeks", "FirstWeekOfYear", "WorkDays", "WorkDayStartHour", "WorkDayEndHour", "TimeFormat", "ParserEnabled", "AdjustHijriDays", "ASPXPageIndexMode", "SiteTheme", "InheritGlobalNavigation", "RequestAccessEnabled", "RequestAccessEmail", "QuotaID", "QuotaStorageLimit", "QuotaStorageWarning" };
			string[] strArrays2 = strArrays;
			if (!(comparableNode is SPWeb))
			{
				throw new Exception("An SPWeb can only be compared to another SPWeb");
			}
			SPWeb sPWeb = (SPWeb)comparableNode;
			XmlNode xML = this.ToXML();
			XmlNode xmlNodes = sPWeb.ToXML();
			if (xmlNodes.Attributes.Count > xML.Attributes.Count)
			{
				xmlNodes = this.ToXML();
				xML = sPWeb.ToXML();
			}
			foreach (XmlAttribute attribute in xML.Attributes)
			{
				if (!(comparableNode is SPSite))
				{
					flag1 = true;
				}
				else
				{
					flag1 = (attribute.Name == "WebTemplateConfig" || attribute.Name == "HasUniquePermissions" || attribute.Name == "InheritsMasterPage" || attribute.Name == "NoCrawl" || attribute.Name == "SiteTheme" || attribute.Name == "SiteID" || attribute.Name == "SiteFeatures" || attribute.Name == "AdjustHijriDays" || attribute.Name == "Url" || attribute.Name == "DiskUsed" || attribute.Name == "Owner" ? false : !(attribute.Name == "SecondaryOwner"));
				}
				if (flag1)
				{
					if (comparableNode is SPSite)
					{
						flag2 = true;
					}
					else
					{
						flag2 = (attribute.Name == "QuotaID" || attribute.Name == "QuotaStorageLimit" ? false : !(attribute.Name == "QuotaStorageWarning"));
					}
					if (flag2)
					{
						if (!(attribute.Name == "ID"))
						{
							if ((!(attribute.Name == "WebTemplateConfig") || xML.Attributes["WebTemplateID"] == null ? true : !(xML.Attributes["WebTemplateID"].Value == "14483")))
							{
								if (!(attribute.Name == "ServerRelativeUrl"))
								{
									if (!(attribute.Name == "IsSearchable"))
									{
										if (!(attribute.Name == "DatabaseName"))
										{
											if (!(attribute.Name == "DatabaseServerName"))
											{
												if (!(attribute.Name == "SiteCollectionServerRelativeUrl"))
												{
													if ((attribute.Name == "WebApplicationId" || attribute.Name == "WebApplication" ? false : !(attribute.Name == "ManagedPath")))
													{
														if (!(attribute.Name == "RootWebGUID"))
														{
															if (!(attribute.Name == "RootSiteGUID"))
															{
																if (!(attribute.Name == "TaxonomyListGUID"))
																{
																	if (!(attribute.Name == "MasterPage"))
																	{
																		if (!(attribute.Name == "WelcomePage"))
																		{
																			if (!(attribute.Name == "SiteCollFeatures"))
																			{
																				if ((attribute.Name == "AuditFlags" || attribute.Name == "TrimAuditLog" || attribute.Name == "AuditLogTrimmingCallout" || attribute.Name == "AuditLogTrimmingRetention" ? false : !(attribute.Name == "AuditLogReportStorageLocation")))
																				{
																					if (!(attribute.Name == "CustomMasterPage"))
																					{
																						if (!string.Equals(attribute.Name, "IsSiteAdmin", StringComparison.OrdinalIgnoreCase))
																						{
																							if ((base.Adapter.SharePointVersion.IsSharePoint2010 || sPWeb.Adapter.SharePointVersion.IsSharePoint2010 ? !(attribute.Name == "SiteFeatures") : true))
																							{
																								if (!string.Equals(attribute.Name, "MaximumFileSize", StringComparison.OrdinalIgnoreCase))
																								{
																									flag3 = true;
																								}
																								else
																								{
																									flag3 = (!string.Equals(base.Adapter.AdapterShortName, "OM", StringComparison.OrdinalIgnoreCase) ? false : string.Equals(sPWeb.Adapter.AdapterShortName, "OM", StringComparison.OrdinalIgnoreCase));
																								}
																								if (flag3)
																								{
																									XmlAttribute itemOf = xmlNodes.Attributes[attribute.Name];
																									if (itemOf == null)
																									{
																										flag = false;
																										num = 0;
																										while (num < (int)strArrays1.Length)
																										{
																											if (!(strArrays1[num] == attribute.Name))
																											{
																												num++;
																											}
																											else
																											{
																												differencesOutput.Write(string.Concat("The attribute: '", attribute.Name, "' is missing. Note: this attribute can't preserved using native web services. "), attribute.Name, DifferenceStatus.Missing, true);
																												flag = true;
																												break;
																											}
																										}
																										if (attribute.Name == "SiteTheme")
																										{
																											differencesOutput.Write(string.Concat("The attribute: '", attribute.Name, "' is missing. Note: this attribute can't preserved using native web services. "), attribute.Name, DifferenceStatus.Missing, true);
																											continue;
																										}
																										else if (!flag)
																										{
																											differencesOutput.Write(string.Concat("The attribute: '", attribute.Name, "' is missing. "), attribute.Name, DifferenceStatus.Missing);
																											flag4 = false;
																											continue;
																										}
																										else if (flag)
																										{
																											continue;
																										}
																									}
																									if (!(attribute.Name != "SiteFeatures" ? true : !(attribute.Value != itemOf.Value)))
																									{
																										set = new Set<string>()
																										{
																											attribute.Value
																										};
																										set1 = new Set<string>()
																										{
																											itemOf.Value
																										};
																										set -= set1;
																										differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different."), attribute.Name);
																										differencesOutput.Write(string.Concat("Feature Guids: ", set[0], " are different"), set[0].ToString());
																										if (options.Level != CompareLevel.Moderate)
																										{
																											flag4 = false;
																										}
																									}
																									else if (!(attribute.Name != "SiteCollFeatures" ? true : !(attribute.Value != itemOf.Value)))
																									{
																										set = new Set<string>()
																										{
																											attribute.Value
																										};
																										set1 = new Set<string>()
																										{
																											itemOf.Value
																										};
																										set -= set1;
																										differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different."), attribute.Name);
																										differencesOutput.Write(string.Concat("Feature Guids: ", set[0], " are different"), set[0].ToString());
																										if (options.Level != CompareLevel.Moderate)
																										{
																											flag4 = false;
																										}
																									}
																									else if (attribute.Value != itemOf.Value)
																									{
																										flag = false;
																										if (options.Level == CompareLevel.Moderate)
																										{
																											num = 0;
																											while (num < (int)strArrays2.Length)
																											{
																												if (!(strArrays2[num] == attribute.Name))
																												{
																													num++;
																												}
																												else
																												{
																													differencesOutput.Write(string.Concat("The attribute ", attribute.Name, " is different. Note: this attribute can't be preserved when writing to a native web service."), attribute.Name, DifferenceStatus.Difference, true);
																													flag = true;
																													break;
																												}
																											}
																										}
																										if ((attribute.Name == "OwnerGroup" || attribute.Name == "MemberGroup" || attribute.Name == "VisitorGroup" ? true : attribute.Name == "AssociateGroups"))
																										{
																											differencesOutput.Write(string.Concat("The attribute ", attribute.Name, " is different."), attribute.Name, DifferenceStatus.Difference, true);
																											flag = true;
																										}
																										if (!flag)
																										{
																											differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. "), attribute.Name);
																											flag4 = false;
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return flag4;
		}
        
	    private void MergeFeatures(XmlNode finalNode, XmlNode newNode, string sFeatureAttribueName)
	    {
	        XmlAttribute xmlAttribute = newNode.Attributes[sFeatureAttribueName];
	        XmlAttribute xmlAttribute2 = finalNode.Attributes[sFeatureAttribueName];
	        if (xmlAttribute2 != null)
	        {
	            if (xmlAttribute == null)
	            {
	                xmlAttribute = newNode.OwnerDocument.CreateAttribute(sFeatureAttribueName);
	                xmlAttribute.Value = xmlAttribute2.Value;
	                newNode.Attributes.Append(xmlAttribute);
	            }
	            else
	            {
	                string[] array = xmlAttribute.Value.Split(new char[]
	                {
	                    ','
	                });
	                List<string> list = new List<string>(xmlAttribute2.Value.Split(new char[]
	                {
	                    ','
	                }));
	                string[] array2 = array;
	                for (int i = 0; i < array2.Length; i++)
	                {
	                    string text = array2[i];
	                    if (!list.Contains(text))
	                    {
	                        list.Add(text);
	                    }
	                }
	                StringBuilder stringBuilder = new StringBuilder(list.Count * 37);
	                foreach (string text in list)
	                {
	                    if (stringBuilder.Length > 0)
	                    {
	                        stringBuilder.Append(",");
	                    }
	                    //string text;
	                    stringBuilder.Append(text);
	                }
	                xmlAttribute.Value = stringBuilder.ToString();
	            }
	        }
	        XmlUtility.MatchAttributeState(finalNode, newNode, sFeatureAttribueName);
	    }

        private void On_lists_CollectionChanged(NodeCollectionChangeType changeType, Metalogix.Explorer.Node changedNode)
		{
			base.SetChildren(null);
		}

		private void On_subWebs_CollectionChanged(NodeCollectionChangeType changeType, Metalogix.Explorer.Node changedNode)
		{
			base.SetChildren(null);
		}

		public virtual void ReleasePermissionsData()
		{
			this.ReleasePermissionsData(true);
		}

		private void ReleasePermissionsData(bool bReleaseStateVariable)
		{
			lock (this.m_oLockSiteUser)
			{
				this.m_users = null;
			}
			lock (this.m_oLockGroups)
			{
				this.m_groups = null;
			}
			lock (this.m_oLockRoleAssignments)
			{
				if (this.m_roleAssignments != null)
				{
					this.m_roleAssignments.RoleAssignmentCollectionChanged -= new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
				}
				this.m_roleAssignments = null;
			}
			lock (this.m_oLockRoles)
			{
				this.m_roles = null;
			}
			if (bReleaseStateVariable)
			{
				this.m_bHasUniquePermissions = null;
			}
		}

		public void ReleaseWebPartLock()
		{
			Monitor.Exit(this._webPartLock);
		}

		private void RoleAssignmentCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			lock (this.m_oLockUniquePermissions)
			{
				this.m_bHasUniquePermissions = new bool?(true);
			}
			lock (this._webXmlLock)
			{
				if ((this.m_webXML == null ? false : this.m_webXML.Attributes["HasUniquePermissions"] != null))
				{
					this.m_webXML.Attributes["HasUniquePermissions"].Value = "True";
				}
			}
		}

		public void SetDocumentParsing(bool bParserEnabled)
		{
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception(Resources.TargetIsReadOnly);
				}
				base.Adapter.Writer.SetDocumentParsing(bParserEnabled);
				this.ClearHeldWebXml();
			}
			else
			{
				lock (this._webXmlLock)
				{
					string xML = this.XML;
					XmlNode xmlNode = XmlUtility.StringToXmlNode(xML);
					XmlAttribute itemOf = xmlNode.Attributes["ParserEnabled"];
					if (itemOf == null)
					{
						itemOf = xmlNode.OwnerDocument.CreateAttribute("ParserEnabled");
						xmlNode.Attributes.Append(itemOf);
					}
					itemOf.Value = bParserEnabled.ToString();
					base.SaveVirtualData(XmlUtility.StringToXmlNode(xML), xmlNode, "XML");
					this.m_webXML = xmlNode;
				}
			}
		}

		public void SetWelcomePage(string sWebRelativeWelcomePageUrl)
		{
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception(Resources.TargetIsReadOnly);
				}
				base.Adapter.Writer.SetWelcomePage(sWebRelativeWelcomePageUrl);
			}
			else
			{
				lock (this._webXmlLock)
				{
					string xML = this.XML;
					XmlNode xmlNode = XmlUtility.StringToXmlNode(xML);
					XmlAttribute itemOf = xmlNode.Attributes["WelcomePage"];
					if (itemOf == null)
					{
						itemOf = xmlNode.OwnerDocument.CreateAttribute("WelcomePage");
						xmlNode.Attributes.Append(itemOf);
					}
					itemOf.Value = sWebRelativeWelcomePageUrl;
					base.SaveVirtualData(XmlUtility.StringToXmlNode(xML), xmlNode, "XML");
					this.m_webXML = xmlNode;
				}
			}
		}

		protected virtual void StoreQuickProperties(XmlNode quickXML)
		{
			List<string> strs;
			char[] chrArray;
			string value;
			string str;
			if (quickXML.Attributes["ServerRelativeUrl"] != null)
			{
				this.m_sServerRelativeUrl = quickXML.Attributes["ServerRelativeUrl"].Value;
			}
			if (quickXML.Attributes["Name"] != null)
			{
				this.m_sName = quickXML.Attributes["Name"].Value;
			}
			if (quickXML.Attributes["Title"] != null)
			{
				this.m_sTitle = quickXML.Attributes["Title"].Value;
			}
			if (quickXML.Attributes["ID"] != null)
			{
				this.m_ID = quickXML.Attributes["ID"].Value;
			}
			if (quickXML.Attributes["Locale"] != null)
			{
				this.m_iLocale = Convert.ToInt32(quickXML.Attributes["Locale"].Value);
			}
			if (quickXML.Attributes["Language"] != null)
			{
				this.m_iLanguage = Convert.ToInt32(quickXML.Attributes["Language"].Value);
			}
			if ((quickXML.Attributes["WebTemplateID"] == null ? false : quickXML.Attributes["WebTemplateConfig"] != null))
			{
				this.m_iWebTemplateID = Convert.ToInt32(quickXML.Attributes["WebTemplateID"].Value);
				int num = Convert.ToInt32(quickXML.Attributes["WebTemplateConfig"].Value);
				this.m_webTemplate = this.Templates.Find(this.m_iWebTemplateID, num);
				if (this.m_webTemplate == null)
				{
					this.m_webTemplate = SPWebTemplate.AllTemplates.Find(1, 1);
				}
			}
			if (quickXML.Attributes["IsSearchable"] != null)
			{
				bool.TryParse(quickXML.Attributes["IsSearchable"].Value, out this.m_bIsSearchable);
			}
			if ((quickXML.Attributes["MaximumFileSize"] == null ? false : quickXML.Attributes["MaximumFileSize"].Value.Length > 0))
			{
				this.m_iMaximumFileSize = Convert.ToInt32(quickXML.Attributes["MaximumFileSize"].Value);
			}
			if (quickXML.Attributes["HasUniquePermissions"] != null)
			{
				lock (this.m_oLockUniquePermissions)
				{
					this.m_bHasUniquePermissions = new bool?(bool.Parse(quickXML.Attributes["HasUniquePermissions"].Value));
				}
			}
			if (quickXML.Attributes["TimeZone"] == null)
			{
				this.m_timeZone = TimeZoneInformation.GetLocalTimeZone();
			}
			else
			{
				try
				{
					this.m_timeZone = TimeZoneInformation.GetTimeZone(Convert.ToInt32(quickXML.Attributes["TimeZone"].Value));
				}
				catch
				{
					this.m_timeZone = TimeZoneInformation.GetLocalTimeZone();
				}
			}
			if (quickXML.Attributes["WebApplicationId"] != null)
			{
				this.m_webApplicationId = new Guid(quickXML.Attributes["WebApplicationId"].Value);
			}
			if (quickXML.Attributes["SiteFeatures"] != null)
			{
				value = quickXML.Attributes["SiteFeatures"].Value;
			}
			else
			{
				value = null;
			}
			string str1 = value;
			if (str1 != null)
			{
				string lower = str1.ToLower();
				chrArray = new char[] { ',' };
				strs = new List<string>(lower.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
				this.m_bHasPublishingFeature = new bool?(strs.Contains("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb"));
			}
			if (quickXML.Attributes["SiteCollFeatures"] != null)
			{
				str = quickXML.Attributes["SiteCollFeatures"].Value;
			}
			else
			{
				str = null;
			}
			string str2 = str;
			if (str2 != null)
			{
				bool flag = true;
				string lower1 = str2.ToLower();
				chrArray = new char[] { ',' };
				strs = new List<string>(lower1.Split(chrArray, StringSplitOptions.RemoveEmptyEntries));
				string[] pUBLISHINGINFRASTRUCTUREGUIDS = SPWeb.PUBLISHING_INFRASTRUCTURE_GUIDS;
				for (int i = 0; i < (int)pUBLISHINGINFRASTRUCTUREGUIDS.Length; i++)
				{
					flag &= strs.Contains(pUBLISHINGINFRASTRUCTUREGUIDS[i]);
				}
				this.m_bHasFormsService = new bool?(strs.Contains("c88c4ff1-dbf5-4649-ad9f-c6c426ebcbf5"));
				this.m_bPublishingInfrastructureActive = new bool?(flag);
			}
		}

		public XmlNode ToXML()
		{
			return this.FullXML;
		}

		public void Update(string sXml)
		{
			this.Update(sXml, new UpdateWebOptions());
		}

		public void Update(string sXml, UpdateWebOptions options)
		{
			XmlNode documentElement = null;
			if (AdapterConfigurationVariables.MigrateLanguageSettings)
			{
				sXml = XmlUtility.AddLanguageSettingsAttribute(sXml, "Web", "Site");
			}
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception(Resources.TargetIsReadOnly);
				}
				string str = base.Adapter.Writer.UpdateWeb(sXml, options);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(str);
				documentElement = xmlDocument.DocumentElement;
				this.ClearHeldWebXml();
			}
			else
			{
				lock (this._webXmlLock)
				{
					XmlNode fullXML = this.FullXML;
					XmlNode xmlNode = XmlUtility.StringToXmlNode(sXml);
					XmlNode xmlNodes = this.UpdateVirtually(fullXML, xmlNode, options);
					base.SaveVirtualData(fullXML, xmlNodes, "XML");
					this.m_webXML = xmlNodes;
					documentElement = xmlNodes;
				}
			}
			this.m_meetingInstances = null;
			this.m_navigation = null;
			this.m_portalListings = null;
			this.m_bHasPublishingFeature = null;
			this.m_bPublishingInfrastructureActive = null;
			this.StoreQuickProperties(documentElement);
		}

		public override void UpdateCurrentNode()
		{
			this.ClearExcessNodeData();
			if ((base.Parent != null ? true : base.Status == ConnectionStatus.Valid))
			{
				this.StoreQuickProperties(this.GetTerseData());
			}
			base.FireDisplayNameChanged();
		}

		public void UpdateGroupQuickLaunch(string groupQL)
		{
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception(Resources.TargetIsReadOnly);
				}
				string[] url = new string[] { "<Web Url='", this.Url, "' AssociateGroups='", groupQL, "'/>" };
				string str = string.Concat(url);
				base.Adapter.Writer.UpdateGroupQuickLaunch(str);
			}
			else
			{
				lock (this._webXmlLock)
				{
					string xML = this.XML;
					XmlNode xmlNode = XmlUtility.StringToXmlNode(xML);
					XmlAttribute itemOf = xmlNode.Attributes["AssociateGroups"];
					if (itemOf == null)
					{
						itemOf = xmlNode.OwnerDocument.CreateAttribute("AssociateGroups");
						xmlNode.Attributes.Append(itemOf);
					}
					itemOf.Value = groupQL;
					base.SaveVirtualData(XmlUtility.StringToXmlNode(xML), xmlNode, "XML");
					this.m_webXML = xmlNode;
				}
			}
		}

		public OperationReportingResult UpdateSiteCollectionSettings(string sXml, UpdateSiteCollectionOptions options)
		{
			OperationReportingResult operationReportingResult = null;
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception(Resources.TargetIsReadOnly);
				}
				string str = base.Adapter.Writer.UpdateSiteCollectionSettings(sXml, options);
				operationReportingResult = new OperationReportingResult(str);
				if (options.UpdateSiteAdmins)
				{
					this.GetSiteUsers(true);
				}
				if (options.UpdateSiteQuota)
				{
					this.ClearHeldWebXml();
				}
			}
			return operationReportingResult;
		}

		private XmlNode UpdateVirtually(XmlNode oldNode, XmlNode newNode, UpdateWebOptions options)
		{
			string[] sNavigationAttributes;
			int i;
			XmlNode xmlNode = null;
			if (!options.CopyCoreMetaData)
			{
				xmlNode = XmlUtility.StringToXmlNode(oldNode.OuterXml);
				if (options.ApplyMasterPage)
				{
					XmlUtility.MatchAttributeState(xmlNode, newNode, "MasterPage");
					XmlUtility.MatchAttributeState(xmlNode, newNode, "CustomMasterPage");
				}
				if (options.ApplyTheme)
				{
					XmlUtility.MatchAttributeState(xmlNode, newNode, "SiteTheme");
				}
				if (options.CopyAccessRequestSettings)
				{
					XmlUtility.MatchAttributeState(xmlNode, newNode, "RequestAccessEmail");
				}
				if (options.CopyAssociatedGroupSettings)
				{
					XmlUtility.MatchAttributeState(xmlNode, newNode, "OwnerGroup");
					XmlUtility.MatchAttributeState(xmlNode, newNode, "MemberGroup");
					XmlUtility.MatchAttributeState(xmlNode, newNode, "VisitorGroup");
					XmlUtility.MatchAttributeState(xmlNode, newNode, "AssociateGroups");
				}
				if (options.CopyFeatures)
				{
					if (!options.MergeFeatures)
					{
						XmlUtility.MatchAttributeState(xmlNode, newNode, "SiteFeatures");
						if (this.RootWebGUID == this.ID)
						{
							XmlUtility.MatchAttributeState(xmlNode, newNode, "SiteCollFeatures");
						}
					}
					else
					{
						this.MergeFeatures(xmlNode, newNode, "SiteFeatures");
						if (this.RootWebGUID == this.ID)
						{
							this.MergeFeatures(xmlNode, newNode, "SiteCollFeatures");
						}
					}
				}
				if (options.CopyNavigation)
				{
					sNavigationAttributes = SPWeb.s_navigationAttributes;
					for (i = 0; i < (int)sNavigationAttributes.Length; i++)
					{
						XmlUtility.MatchAttributeState(xmlNode, newNode, sNavigationAttributes[i]);
					}
					if (this.RootWebGUID == this.ID)
					{
						sNavigationAttributes = SPWeb.s_rootWebNavigationAttributes;
						for (i = 0; i < (int)sNavigationAttributes.Length; i++)
						{
							XmlUtility.MatchAttributeState(xmlNode, newNode, sNavigationAttributes[i]);
						}
					}
				}
				if (options.PreserveUIVersion)
				{
					XmlUtility.MatchAttributeState(xmlNode, newNode, "UIVersion");
				}
			}
			else
			{
				xmlNode = newNode;
				if (!options.ApplyMasterPage)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "MasterPage");
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "CustomMasterPage");
				}
				if (!options.ApplyTheme)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "SiteTheme");
				}
				if (!options.CopyAccessRequestSettings)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "RequestAccessEmail");
				}
				if (!options.CopyAssociatedGroupSettings)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "OwnerGroup");
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "MemberGroup");
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "VisitorGroup");
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "AssociateGroups");
				}
				if (!options.CopyFeatures)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "SiteFeatures");
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "SiteCollFeatures");
				}
				else if (this.RootWebGUID != this.ID)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "SiteCollFeatures");
				}
				if (!options.CopyNavigation)
				{
					sNavigationAttributes = SPWeb.s_navigationAttributes;
					for (i = 0; i < (int)sNavigationAttributes.Length; i++)
					{
						XmlUtility.MatchAttributeState(xmlNode, oldNode, sNavigationAttributes[i]);
					}
					sNavigationAttributes = SPWeb.s_rootWebNavigationAttributes;
					for (i = 0; i < (int)sNavigationAttributes.Length; i++)
					{
						XmlUtility.MatchAttributeState(xmlNode, oldNode, sNavigationAttributes[i]);
					}
				}
				else if (this.RootWebGUID != this.ID)
				{
					sNavigationAttributes = SPWeb.s_rootWebNavigationAttributes;
					for (i = 0; i < (int)sNavigationAttributes.Length; i++)
					{
						XmlUtility.MatchAttributeState(xmlNode, oldNode, sNavigationAttributes[i]);
					}
				}
				if (!options.PreserveUIVersion)
				{
					XmlUtility.MatchAttributeState(xmlNode, oldNode, "UIVersion");
				}
			}
			return xmlNode;
		}

		public void WriteSupportInfo(TextWriter output)
		{
			if (base.Adapter != null)
			{
				output.WriteLine("SharePoint Version: {0}", base.Adapter.SharePointVersion);
				output.WriteLine("SharePoint Adapter: {0}", base.Adapter.AdapterShortName);
				output.WriteLine("SharePoint User: {0}", (this.Credentials.IsDefault ? Environment.UserName : this.Credentials.UserName));
				if (base.Adapter.AuthenticationInitializer != null)
				{
					output.WriteLine("SharePoint Authentication: {0}", base.Adapter.AuthenticationInitializer.MenuText);
				}
			}
		}
	}
}