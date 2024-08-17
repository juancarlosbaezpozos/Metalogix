using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.BCS;
using Metalogix.SharePoint.Common.Enums;
using Metalogix.SharePoint.Interfaces;
using Metalogix.SharePoint.Workflow2013;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Image("Metalogix.SharePoint.Icons.GenericList.ico")]
	[Name("List or Library")]
	[PluralName("Lists and Libraries")]
	public class SPList : SPFolder, List, Folder, ISupportSP2013Workflows
	{
		public static bool SHOW_LIST_INTERNAL_NAME;

		private Dictionary<string, string> m_constantProperties = null;

		private SPWeb m_parentWeb = null;

		private string _originalFieldsSchemaXML;

		private bool? _isMigrationPipelineSupported = null;

		private bool? _isMigrationPipelineSupportedForTarget = null;

		private string m_sID = null;

		private string m_sName = null;

		private DateTime m_dtCreated;

		private DateTime m_dtModified;

		private string m_sTitle = null;

		private string m_sBaseTemplate = null;

		private ListTemplateType m_baseTemplate = ListTemplateType.DocumentLibrary;

		private Metalogix.SharePoint.ListType m_baseType = Metalogix.SharePoint.ListType.DocumentLibrary;

		private int m_iItemCount = 0;

		private string _columnDefaultValue;

		private object _lockColumnDefaultValue = new object();

		private string m_sDirName = null;

		private Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sp2013WorkflowCollection;

		private object m_oLockUniquePermissions = new object();

		private bool? m_bHasUniquePermissions = null;

		private object _predictedNextAvailableIDLock = new object();

		private int _predictedNextAvailableID = 1;

		private object m_oLockRoles = new object();

		private SPRoleCollection m_roles = null;

		private SPFolderBasic m_formsFolder = null;

		private object m_oLockFormsFolder = new object();

		private SPViewCollection m_views;

		private readonly object m_oLockViews = new object();

		private SPWorkflowAssociationCollection m_WorkflowAssociations = null;

		private readonly object m_oLockWorkflowAssociations = new object();

		private bool _includePreviousWorkflowVersions = false;

		private readonly object m_oLockFields = new object();

		private SPFieldCollection m_fields;

		private SPContentTypeCollection m_contentTypes = null;

		private readonly object m_oLockContentTypes = new object();

		private List<string> _contentTypesOrder = null;

		private readonly object _lockContentTypesOrder = new object();

		private object m_oLockRoleAssignments = new object();

		private SPRoleAssignmentCollection m_roleAssignments = null;

		private SPAlertCollection m_alerts = null;

		private readonly object m_oLockAlerts = new object();

		private SPExternalContentType m_externalContentType;

		private readonly object m_oLockExternalContentType = new object();

		public SPAlertCollection Alerts
		{
			get
			{
				SPAlertCollection mAlerts;
				lock (this.m_oLockAlerts)
				{
					if (this.m_alerts != null)
					{
						mAlerts = this.m_alerts;
					}
					else
					{
						this.m_alerts = new SPAlertCollection(this);
						this.m_alerts.FetchData();
						mAlerts = this.m_alerts;
					}
				}
				return mAlerts;
			}
		}

		public bool? AllowContentTypes
		{
			get
			{
				bool? nullable;
				if (this.ListXML.Attributes["AllowContentTypes"] == null)
				{
					nullable = null;
				}
				else
				{
					nullable = new bool?(bool.Parse(this.ListXML.Attributes["AllowContentTypes"].Value));
				}
				return nullable;
			}
		}

		public ListTemplateType BaseTemplate
		{
			get
			{
				return this.m_baseTemplate;
			}
		}

		public Metalogix.SharePoint.ListType BaseType
		{
			get
			{
				return this.m_baseType;
			}
		}

		public string ColumnDefaultValue
		{
			get
			{
				string str;
				lock (this._lockColumnDefaultValue)
				{
					if (this._columnDefaultValue == null)
					{
						this._columnDefaultValue = (base.Adapter.IsClientOM ? this.GetColumnDefaultSettings() : base.Adapter.Reader.ExecuteCommand(SharePointAdapterCommands.GetColumnDefaultSettings.ToString(), this.ID));
					}
					str = this._columnDefaultValue;
				}
				return str;
			}
		}

		internal override string ConstantDirName
		{
			get
			{
				string item = this.m_constantProperties["DirName"];
				return string.Concat((item == "" ? "" : string.Concat(item, "/")), this.ConstantName);
			}
		}

		public string ConstantID
		{
			get
			{
				return this.m_constantProperties["ID"];
			}
		}

		public override string ConstantName
		{
			get
			{
				return this.m_constantProperties["Name"];
			}
		}

		public SPContentTypeCollection ContentTypes
		{
			get
			{
				SPContentTypeCollection mContentTypes;
				lock (this.m_oLockContentTypes)
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

		public bool ContentTypesEnabled
		{
			get
			{
				bool flag;
				XmlAttribute itemOf = this.ListXML.Attributes["ContentTypesEnabled"];
				flag = (itemOf != null ? itemOf.Value == "True" : false);
				return flag;
			}
		}

		public List<string> ContentTypesOrder
		{
			get
			{
				List<string> strs;
				lock (this._lockContentTypesOrder)
				{
					if (this._contentTypesOrder == null)
					{
						XmlNode xmlNodes = this._xmlNode.SelectSingleNode("./ContentTypeOrder");
						List<string> strs1 = new List<string>();
						if (xmlNodes != null)
						{
							foreach (XmlNode xmlNodes1 in xmlNodes)
							{
								strs1.Add(xmlNodes1.Attributes["Name"].Value);
							}
						}
						this._contentTypesOrder = strs1;
					}
					strs = this._contentTypesOrder;
				}
				return strs;
			}
		}

		public override DateTime Created
		{
			get
			{
				return this.m_dtCreated;
			}
		}

		public override string CreatedBy
		{
			get
			{
				return null;
			}
		}

		public string DataSourceFinderName
		{
			get
			{
				string innerText;
				if (this.BaseTemplate != ListTemplateType.ExternalList)
				{
					innerText = null;
				}
				else if (this.ListXML.Attributes["SpecificFinder"] != null)
				{
					innerText = this.ListXML.Attributes["SpecificFinder"].InnerText;
				}
				else
				{
					innerText = null;
				}
				return innerText;
			}
		}

		public override string DirName
		{
			get
			{
				return string.Concat((this.m_sDirName == "" ? "" : string.Concat(this.m_sDirName, "/")), this.Name);
			}
		}

		public override string DisplayName
		{
			get
			{
				string str;
				string title = this.Title;
				if (SPList.SHOW_LIST_INTERNAL_NAME)
				{
					title = this.Name;
				}
				if (this.ItemCount <= 0)
				{
					str = title;
				}
				else
				{
					object[] itemCount = new object[] { title, " (", this.ItemCount, " item", null, null };
					itemCount[4] = (this.ItemCount > 1 ? "s" : "");
					itemCount[5] = ")";
					str = string.Concat(itemCount);
				}
				return str;
			}
		}

		public bool? EnableAssignToEmail
		{
			get
			{
				bool? nullable;
				if (this.ListXML.Attributes["EnableAssignToEmail"] != null)
				{
					nullable = new bool?(this.ListXML.Attributes["EnableAssignToEmail"].Value.Equals("True", StringComparison.OrdinalIgnoreCase));
				}
				else
				{
					nullable = null;
				}
				return nullable;
			}
		}

		public bool EnableAttachments
		{
			get
			{
				bool flag;
				flag = (this.ListXML.Attributes["EnableAttachments"] != null ? this.ListXML.Attributes["EnableAttachments"].Value.Equals("True", StringComparison.OrdinalIgnoreCase) : false);
				return flag;
			}
		}

		public bool EnableMinorVersions
		{
			get
			{
				return (this.ListXML.Attributes["EnableMinorVersions"] != null ? this.ListXML.Attributes["EnableMinorVersions"].Value.Equals("True", StringComparison.OrdinalIgnoreCase) : false);
			}
			set
			{
				if (this.ListXML.Attributes["EnableMinorVersions"] == null)
				{
					throw new ArgumentException("Property does not exists.", "EnableMinorVersions");
				}
				this.ListXML.Attributes["EnableMinorVersions"].Value = value.ToString();
				this.UpdateSettings(this.ListXML.OuterXml);
			}
		}

		public bool EnableModeration
		{
			get
			{
				bool flag = this.ListXML.Attributes["EnableModeration"].Value.Equals("True", StringComparison.OrdinalIgnoreCase);
				return flag;
			}
			set
			{
				if (this.ListXML.Attributes["EnableModeration"] == null)
				{
					throw new ArgumentException("Property does not exists.", "EnableModeration");
				}
				this.ListXML.Attributes["EnableModeration"].Value = value.ToString();
				this.UpdateSettings(this.ListXML.OuterXml);
			}
		}

		public bool EnableVersioning
		{
			get
			{
				return JustDecompileGenerated_get_EnableVersioning();
			}
			set
			{
				JustDecompileGenerated_set_EnableVersioning(value);
			}
		}

		public bool JustDecompileGenerated_get_EnableVersioning()
		{
			bool flag = this.ListXML.Attributes["EnableVersioning"].Value.Equals("True", StringComparison.OrdinalIgnoreCase);
			return flag;
		}

		public void JustDecompileGenerated_set_EnableVersioning(bool value)
		{
			if (this.ListXML.Attributes["EnableVersioning"] == null)
			{
				throw new ArgumentException("Property does not exists.", "EnableVersioning");
			}
			this.ListXML.Attributes["EnableVersioning"].Value = value.ToString();
			this.UpdateSettings(this.ListXML.OuterXml);
		}

		public SPExternalContentType ExternalContentType
		{
			get
			{
				SPExternalContentType mExternalContentType;
				string value;
				string str;
				string value1;
				if (this.BaseTemplate == ListTemplateType.ExternalList)
				{
					lock (this.m_oLockExternalContentType)
					{
						if (this.m_externalContentType == null)
						{
							if (this.ListXML.Attributes["EntityNamespace"] != null)
							{
								value = this.ListXML.Attributes["EntityNamespace"].Value;
							}
							else
							{
								value = null;
							}
							string str1 = value;
							if (this.ListXML.Attributes["Entity"] != null)
							{
								str = this.ListXML.Attributes["Entity"].Value;
							}
							else
							{
								str = null;
							}
							string str2 = str;
							if ((this.ParentWeb.ExternalContentTypes == null || str1 == null ? false : str2 != null))
							{
								this.m_externalContentType = this.ParentWeb.ExternalContentTypes[str1, str2];
							}
							if ((this.m_externalContentType != null ? false : str2 != null))
							{
								if (this._xmlNode.Attributes["LobSystemInstance"] != null)
								{
									value1 = this._xmlNode.Attributes["LobSystemInstance"].Value;
								}
								else
								{
									value1 = null;
								}
								string str3 = value1;
								this.m_externalContentType = SPExternalContentType.CreateFromTerseData(this.ParentWeb, str3, str2, str1);
							}
							mExternalContentType = this.m_externalContentType;
						}
						else
						{
							mExternalContentType = this.m_externalContentType;
						}
					}
				}
				else
				{
					mExternalContentType = null;
				}
				return mExternalContentType;
			}
		}

		public SPFieldCollection FieldCollection
		{
			get
			{
				SPFieldCollection mFields;
				lock (this.m_oLockFields)
				{
					if (this.m_fields == null)
					{
						this.m_fields = this.GetFields(false);
					}
					mFields = this.m_fields;
				}
				return mFields;
			}
		}

		public Metalogix.Explorer.FieldCollection Fields
		{
			get
			{
				return this.FieldCollection;
			}
		}

		public override string FolderPath
		{
			get
			{
				return "";
			}
		}

		public bool ForceCheckout
		{
			get
			{
				bool flag;
				flag = (this.ListXML.Attributes["ForceCheckout"] != null ? this.ListXML.Attributes["ForceCheckout"].Value.Equals("True", StringComparison.OrdinalIgnoreCase) : false);
				return flag;
			}
			set
			{
				if (this.ListXML.Attributes["ForceCheckout"] == null)
				{
					throw new ArgumentException("Property does not exists.", "ForceCheckout");
				}
				this.ListXML.Attributes["ForceCheckout"].Value = value.ToString();
				this.UpdateSettings(this.ListXML.OuterXml);
			}
		}

		public SPFolderBasic FormsFolder
		{
			get
			{
				SPFolderBasic mFormsFolder;
				lock (this.m_oLockFormsFolder)
				{
					if (this.m_formsFolder == null)
					{
						this.m_formsFolder = this.GetFormsFolder(false);
					}
					mFormsFolder = this.m_formsFolder;
				}
				return mFormsFolder;
			}
		}

		public bool HasDependencies
		{
			get
			{
				XmlNode listXML = this.ListXML;
				XmlNodeList xmlNodeLists = listXML.SelectNodes("//Field[@Type='Lookup']");
				XmlNodeList xmlNodeLists1 = listXML.SelectNodes("//Field[@Type='LookupMulti']");
				return ((xmlNodeLists.Count > 0 ? false : xmlNodeLists1.Count <= 0) ? false : true);
			}
		}

		private bool HasFetched
		{
			get
			{
				return this._hasFetched;
			}
		}

		public override bool HasUniquePermissions
		{
			get
			{
				bool value;
				bool flag = bool.Parse(this.GetListXML(false).Attributes["HasUniquePermissions"].Value);
				lock (this.m_oLockUniquePermissions)
				{
					if (!this.m_bHasUniquePermissions.HasValue)
					{
						this.m_bHasUniquePermissions = new bool?(flag);
					}
					value = this.m_bHasUniquePermissions.Value;
				}
				return value;
			}
		}

		public string ID
		{
			get
			{
				return this.m_sID;
			}
		}

		public override string ImageName
		{
			get
			{
				string str;
				ListTemplateType baseTemplate = this.BaseTemplate;
				if (baseTemplate <= ListTemplateType.MeetingObjectives)
				{
					if (baseTemplate <= ListTemplateType.TasksWithTimelineAndHierarchy)
					{
						switch (baseTemplate)
						{
							case ListTemplateType.CustomList:
							{
								str = "Metalogix.SharePoint.Icons.GenericList.ico";
								break;
							}
							case ListTemplateType.DocumentLibrary:
							{
								str = "Metalogix.SharePoint.Icons.DocumentLibrary.ico";
								break;
							}
							case ListTemplateType.Survey:
							{
								str = "Metalogix.SharePoint.Icons.Survey.ico";
								break;
							}
							case ListTemplateType.Links:
							{
								str = "Metalogix.SharePoint.Icons.Links.ico";
								break;
							}
							case ListTemplateType.Announcements:
							{
								str = "Metalogix.SharePoint.Icons.Announcements.ico";
								break;
							}
							case ListTemplateType.Contacts:
							{
								str = "Metalogix.SharePoint.Icons.Contacts.ico";
								break;
							}
							case ListTemplateType.Events:
							{
								str = "Metalogix.SharePoint.Icons.Events.ico";
								break;
							}
							case ListTemplateType.Tasks:
							{
								str = "Metalogix.SharePoint.Icons.Tasks.ico";
								break;
							}
							case ListTemplateType.DiscussionBoard:
							{
								str = "Metalogix.SharePoint.Icons.DiscussionBoard.ico";
								break;
							}
							case ListTemplateType.PictureLibrary:
							{
								str = "Metalogix.SharePoint.Icons.PictureLibrary.ico";
								break;
							}
							default:
							{
								if (baseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy)
								{
									goto case ListTemplateType.Tasks;
								}
								str = "Metalogix.SharePoint.Icons.GenericList.ico";
								return str;
							}
						}
					}
					else if (baseTemplate == ListTemplateType.MeetingAgenda)
					{
						str = "Metalogix.SharePoint.Icons.Agenda.ico";
					}
					else
					{
						if (baseTemplate != ListTemplateType.MeetingObjectives)
						{
							str = "Metalogix.SharePoint.Icons.GenericList.ico";
							return str;
						}
						str = "Metalogix.SharePoint.Icons.Objectives.ico";
					}
				}
				else if (baseTemplate <= ListTemplateType.ExternalList)
				{
					switch (baseTemplate)
					{
						case ListTemplateType.BlogPosts:
						{
							str = "Metalogix.SharePoint.Icons.Posts.ico";
							break;
						}
						case ListTemplateType.BlogComments:
						{
							str = "Metalogix.SharePoint.Icons.Comments.ico";
							break;
						}
						case ListTemplateType.BlogCategories:
						{
							str = "Metalogix.SharePoint.Icons.Categories.ico";
							break;
						}
						default:
						{
							if (baseTemplate == ListTemplateType.ExternalList)
							{
								str = "Metalogix.SharePoint.Icons.ExternalList.ico";
								break;
							}
							else
							{
								str = "Metalogix.SharePoint.Icons.GenericList.ico";
								return str;
							}
						}
					}
				}
				else if (baseTemplate == ListTemplateType.AssetLibrary)
				{
					str = "Metalogix.SharePoint.Icons.AssetLibrary2.ico";
				}
				else
				{
					if (baseTemplate != ListTemplateType.Issues)
					{
						str = "Metalogix.SharePoint.Icons.GenericList.ico";
						return str;
					}
					str = "Metalogix.SharePoint.Icons.Issues.ico";
				}
				return str;
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

		public bool IsDocumentLibrary
		{
			get
			{
				return this.BaseType == Metalogix.SharePoint.ListType.DocumentLibrary;
			}
		}

		public bool IsMigrationPipelineSupported
		{
			get
			{
				if (!this._isMigrationPipelineSupported.HasValue)
				{
					this._isMigrationPipelineSupported = new bool?(this.DetermineMigrationPipelineSupport());
				}
				return this._isMigrationPipelineSupported.Value;
			}
		}

		public bool IsMigrationPipelineSupportedForTarget
		{
			get
			{
				if (!this._isMigrationPipelineSupportedForTarget.HasValue)
				{
					this._isMigrationPipelineSupportedForTarget = new bool?(this.BaseTemplate != ListTemplateType.AssetLibrary);
				}
				return this._isMigrationPipelineSupportedForTarget.Value;
			}
		}

		public bool IsPublishingLibrary
		{
			get
			{
				return (this.BaseTemplate == ListTemplateType.O12Pages || this.BaseTemplate == ListTemplateType.WikiPageLibrary || this.BaseTemplate == ListTemplateType.ReportLibrary ? true : this.BaseTemplate == ListTemplateType.MasterPageCatalog);
			}
		}

		public bool IsUsingMigrationPipeline
		{
			get;
			set;
		}

		public bool IsWorkflowLibrary
		{
			get
			{
				return (this.BaseTemplate == ListTemplateType.NoCodeWorkflows || this.BaseTemplate == ListTemplateType.NoCodePublicWorkflow || this.BaseTemplate == ListTemplateType.WorkflowProcess ? true : this.BaseTemplate == ListTemplateType.NintexWorkflows);
			}
		}

		public new int ItemCount
		{
			get
			{
				return this.m_iItemCount;
			}
		}

		public override int ItemID
		{
			get
			{
				return -1;
			}
		}

		public string ListSettingsXML
		{
			get
			{
				return this.ListXML.CloneNode(true).OuterXml;
			}
		}

		private XmlNode ListXML
		{
			get
			{
				XmlNode xmlNodes;
				lock (this._lockXml)
				{
					this.FetchData();
					xmlNodes = this._xmlNode;
				}
				return xmlNodes;
			}
		}

		public override DateTime Modified
		{
			get
			{
				return this.m_dtModified;
			}
		}

		public override string ModifiedBy
		{
			get
			{
				return null;
			}
		}

		public bool MultipleDataList
		{
			get
			{
				return (this.ListXML.Attributes["MultipleDataList"] == null ? false : bool.Parse(this.ListXML.Attributes["MultipleDataList"].Value));
			}
		}

		public override string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public int NextAvailableID
		{
			get
			{
				this.FetchData();
				int num = Convert.ToInt32(this.ListXML.Attributes["NextAvailableID"].Value);
				return num;
			}
		}

		public bool OnQuickLaunch
		{
			get
			{
				bool flag;
				XmlAttribute itemOf = this.ListXML.Attributes["OnQuickLaunch"];
				flag = (itemOf != null ? bool.Parse(itemOf.Value) : false);
				return flag;
			}
		}

		public string OriginalFieldsSchemaXML
		{
			get
			{
				return this._originalFieldsSchemaXML;
			}
		}

		public override SPFolder ParentFolder
		{
			get
			{
				return null;
			}
		}

		public override SPList ParentList
		{
			get
			{
				return this;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public int PredictedNextAvailableID
		{
			get
			{
				return this._predictedNextAvailableID;
			}
			private set
			{
				this._predictedNextAvailableID = value;
			}
		}

		public string RawBaseTemplate
		{
			get
			{
				return this.m_sBaseTemplate;
			}
		}

		public override RoleAssignmentCollection RoleAssignments
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

		public override RoleCollection Roles
		{
			get
			{
				RoleCollection mRoles;
				lock (this.m_oLockRoles)
				{
					if (this.m_roles == null)
					{
						this.m_roles = new SPRoleCollection(this, this.m_parentWeb);
						this.m_roles.FetchData();
					}
					if ((this.m_roles.Count != 0 ? true : this.m_parentWeb == null))
					{
						mRoles = this.m_roles;
					}
					else if (!this.m_bInUseAsSource.HasValue)
					{
						mRoles = this.m_parentWeb.Roles;
					}
					else
					{
						this.m_roles.IgnoreList = true;
						this.m_roles.FetchData();
						mRoles = this.m_roles;
					}
				}
				return mRoles;
			}
		}

		public Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection SP2013WorkflowCollection
		{
			get
			{
				Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sP2013WorkflowCollection = this.sp2013WorkflowCollection;
				if (sP2013WorkflowCollection == null)
				{
					Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection sP2013WorkflowCollection1 = new Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection(base.Adapter, SP2013WorkflowScopeType.List);
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

		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
		}

		public override string Url
		{
			get
			{
				string str = string.Concat(this.ParentWeb.Url, "/", this.Name);
				return str;
			}
		}

		public SPViewCollection Views
		{
			get
			{
				SPViewCollection mViews;
				lock (this.m_oLockViews)
				{
					if (this.m_views == null)
					{
						this.m_views = this.GetViews(false);
					}
					mViews = this.m_views;
				}
				return mViews;
			}
		}

		public override string WebRelativeUrl
		{
			get
			{
				if (this.m_sWebRelativeUrl == null)
				{
					string serverRelativeUrl = this.ServerRelativeUrl;
					char[] chrArray = new char[] { '/' };
					string str = serverRelativeUrl.Trim(chrArray);
					string serverRelativeUrl1 = this.ParentWeb.ServerRelativeUrl;
					chrArray = new char[] { '/' };
					string str1 = str.Substring(serverRelativeUrl1.Trim(chrArray).Length);
					chrArray = new char[] { '/' };
					this.m_sWebRelativeUrl = str1.Trim(chrArray);
				}
				return this.m_sWebRelativeUrl;
			}
		}

		public SPWorkflowAssociationCollection WorkflowAssociations
		{
			get
			{
				SPWorkflowAssociationCollection mWorkflowAssociations;
				lock (this.m_oLockWorkflowAssociations)
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
				return this.ListXML.OuterXml;
			}
		}

		static SPList()
		{
			SPList.SHOW_LIST_INTERNAL_NAME = false;
		}

		public SPList(SPWeb parentWeb, XmlNode listXML) : base(parentWeb.Adapter, parentWeb)
		{
			this.m_parentWeb = parentWeb;
			this.StoreQuickProperties(listXML);
			Dictionary<string, string> strs = new Dictionary<string, string>();
			this.SaveConstantProperties(strs);
			this.m_constantProperties = strs;
		}

		public SPList(SPWeb parentWeb, XmlNode listXML, bool isFullXml) : this(parentWeb, listXML)
		{
			if (isFullXml)
			{
				this._xmlNode = listXML;
				this._hasFetched = true;
			}
		}

		public override bool AnalyzeChurn(DateTime pivotDate, bool bRecursive, out long lByteschanged, out long lItemsChanged)
		{
			bool flag;
			lByteschanged = (long)0;
			lItemsChanged = (long)0;
			if (this.ParentWeb.IsSearchable)
			{
				string str = base.Adapter.Reader.AnalyzeChurn(pivotDate, this.ID, -1, bRecursive);
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

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			lock (this._lockXml)
			{
				this._xmlNode = null;
				this._hasFetched = false;
			}
			this.ReleasePermissionsData(true);
			this.LoadConstantProperties(this.m_constantProperties);
			lock (this.m_oLockViews)
			{
				this.m_views = null;
			}
			lock (this.m_oLockFields)
			{
				this.m_fields = null;
			}
			lock (this.m_oLockContentTypes)
			{
				this.m_contentTypes = null;
			}
			lock (this.m_oLockWorkflowAssociations)
			{
				this.m_WorkflowAssociations = null;
			}
			lock (this.m_oLockAlerts)
			{
				this.m_alerts = null;
			}
			lock (this.m_oLockExternalContentType)
			{
				this.m_externalContentType = null;
			}
			lock (this.m_oLockFormsFolder)
			{
				this.m_formsFolder = null;
			}
			lock (this._lockColumnDefaultValue)
			{
				this._columnDefaultValue = null;
			}
		}

		private bool ContainsValidItemTypesForMigrationPipeline()
		{
			string str;
			bool flag;
			bool flag1;
			if (base.Adapter.IsDB)
			{
				str = string.Format((base.Adapter.SharePointVersion.IsSharePoint2003 ? "{0}  And d.Extension ='aspx' {1}" : "{0}  And (d.Extension ='aspx' Or d.ProgId like 'InfoPath%') {1}"), Environment.NewLine, Environment.NewLine);
				flag1 = !this.GetItemsByQuery(str, null, true, true).Any<Node>();
			}
			else if (!base.Adapter.IsNws)
			{
				string str1 = base.Adapter.Reader.IsListContainsInfoPathOrAspxItem(this.ID);
				bool.TryParse(str1, out flag);
				flag1 = !flag;
			}
			else if (this.ItemCount >= 5000)
			{
				flag1 = !base.AllItems.Any<Node>((Node node) => (string.Equals(node["File_x0020_Type"], "aspx", StringComparison.OrdinalIgnoreCase) ? true : string.Equals(node["HTML_x0020_File_x0020_Type"], "InfoPath", StringComparison.OrdinalIgnoreCase)));
			}
			else
			{
				str = string.Concat("<Where><Or><Eq><FieldRef Name = 'File_x0020_Type' /><Value Type= 'Text'>aspx</Value></Eq>", "<BeginsWith><FieldRef Name = 'HTML_x0020_File_x0020_Type' /><Value Type= 'Text'>InfoPath</Value></BeginsWith></Or></Where>");
				flag1 = !this.GetItemsByQuery(str, null, true, true).Any<Node>();
			}
			return flag1;
		}

		private static bool ContainsView(XmlNode targetXML, string sName)
		{
			bool flag;
			foreach (XmlNode xmlNodes in targetXML.SelectNodes("//Views/View"))
			{
				if (xmlNodes.Attributes["DisplayName"].Value.ToLower() == sName.ToLower())
				{
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		public override void Delete()
		{
			if (this.ParentWeb == null)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				base.Adapter.Writer.DeleteList(this.ID);
			}
			else
			{
				this.ParentWeb.Lists.DeleteList(this.ID);
			}
		}

		private bool DetermineMigrationPipelineSupport()
		{
			bool flag;
			if (!(this.ListXML.GetAttributeValueAsBoolean("Hidden") ? false : !AdapterConfigurationVariables.ListNamesToIgnoreForAzure.Contains(this.Name.ToLowerInvariant())))
			{
				flag = false;
			}
			else if (!(AdapterConfigurationVariables.ListNamesToIncludeForAzure == null ? true : !AdapterConfigurationVariables.ListNamesToIncludeForAzure.Contains(this.Name.ToLowerInvariant())))
			{
				flag = true;
			}
			else if (this.IsSupportedListTemplateForMigrationPipeline())
			{
				flag = true;
			}
			else if (!this.IsSupportedLibraryTemplateForMigrationPipeline())
			{
				flag = (!this.IsCustomizedLibraryTemplate() ? false : true);
			}
			else
			{
				flag = this.ContainsValidItemTypesForMigrationPipeline();
			}
			return flag;
		}

		protected override void FetchData()
		{
			if (!this._hasFetched)
			{
				lock (this._lockXml)
				{
					if (!this.HasFetched)
					{
						XmlNode listXML = this.GetListXML(true);
						this.StoreQuickProperties(listXML);
						this._xmlNode = listXML;
						this._hasFetched = true;
					}
				}
			}
		}

		private string GetColumnDefaultSettings()
		{
			string resultXml;
			try
			{
				byte[] document = base.Adapter.Reader.GetDocument(null, this.ServerRelativeUrl, "/Forms/client_LocationBasedDefaults.html", 1);
				if ((document == null ? false : (int)document.Length > 0))
				{
					OperationReporting operationReporting = new OperationReporting();
					operationReporting.Start();
					operationReporting.LogObjectXml(Encoding.UTF8.GetString(document));
					operationReporting.End();
					resultXml = operationReporting.ResultXml;
					return resultXml;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if ((exception.Message.Contains("File does not exist") ? false : !exception.Message.Contains("File Not Found")))
				{
					throw;
				}
			}
			resultXml = null;
			return resultXml;
		}

		internal SPContentTypeCollection GetContentTypes(bool bAlwaysRefetch)
		{
			SPContentTypeCollection mContentTypes;
			lock (this.m_oLockContentTypes)
			{
				if ((this.m_contentTypes == null ? false : !bAlwaysRefetch))
				{
					mContentTypes = this.m_contentTypes;
					return mContentTypes;
				}
			}
			SPContentTypeCollection sPContentTypeCollections = new SPContentTypeCollection(this);
			sPContentTypeCollections.FetchData();
			mContentTypes = sPContentTypeCollections;
			return mContentTypes;
		}

		internal SPFieldCollection GetFields(bool bAlwaysRefetch)
		{
			SPFieldCollection mFields;
			lock (this.m_oLockFields)
			{
				if ((this.m_fields == null ? false : !bAlwaysRefetch))
				{
					mFields = this.m_fields;
					return mFields;
				}
			}
			XmlNode xmlNodes = this.GetListXML(bAlwaysRefetch).SelectSingleNode("./Fields");
			mFields = new SPFieldCollection(this, xmlNodes);
			return mFields;
		}

		internal SPFolderBasic GetFormsFolder(bool bAlwaysRefetch)
		{
			SPFolderBasic mFormsFolder;
			lock (this.m_oLockFormsFolder)
			{
				if ((this.m_formsFolder == null ? false : !bAlwaysRefetch))
				{
					mFormsFolder = this.m_formsFolder;
					return mFormsFolder;
				}
			}
			if (this.BaseType != Metalogix.SharePoint.ListType.DocumentLibrary)
			{
				mFormsFolder = new SPFolderBasic(this.ParentWeb, this.WebRelativeUrl);
			}
			else
			{
				SPWeb parentWeb = this.ParentWeb;
				string[] webRelativeUrl = new string[] { this.WebRelativeUrl, "Forms" };
				mFormsFolder = new SPFolderBasic(parentWeb, UrlUtils.ConcatUrls(webRelativeUrl));
			}
			return mFormsFolder;
		}

		public SPListItemCollection GetItemsByQuery(string query, string fields = null, bool includeExternalizationData = true, bool includePermissionsInheritance = true)
		{
			SPListItemCollection itemCollection = base.GetItemCollection(true, (SPListItemCollection items) => items.FetchDataByQuery(query, fields, includeExternalizationData, includePermissionsInheritance));
			return itemCollection;
		}

		public byte[] GetLibraryDocumentTemplate()
		{
			string value;
			string str;
			byte[] document = null;
			try
			{
				if (this.IsDocumentLibrary)
				{
					XmlNode listXML = this.ListXML;
					bool itemOf = listXML.Attributes["DocTemplateUrl"] != null;
					bool flag = listXML.Attributes["DocTemplateId"] != null;
					if ((itemOf ? true : flag))
					{
						if (flag)
						{
							value = listXML.Attributes["DocTemplateId"].Value;
						}
						else
						{
							value = null;
						}
						string str1 = value;
						if (itemOf)
						{
							str = listXML.Attributes["DocTemplateUrl"].Value;
						}
						else
						{
							str = null;
						}
						string str2 = str;
						string str3 = null;
						string str4 = null;
						if (!string.IsNullOrEmpty(str2))
						{
							Utils.ParseUrlForLeafName(str2, out str3, out str4);
							document = base.Adapter.Reader.GetDocument(str1, str3, str4, 1);
						}
					}
				}
			}
			catch (Exception exception)
			{
			}
			return document;
		}

		public XmlNode GetListXML(bool bAlwaysRefetch)
		{
			XmlNode xmlNodes;
			lock (this._lockXml)
			{
				if ((this._xmlNode == null ? false : !bAlwaysRefetch))
				{
					xmlNodes = this._xmlNode;
					return xmlNodes;
				}
			}
			string list = this.ParentWeb.Adapter.Reader.GetList(this.ConstantID);
			OperationReportingResult operationReportingResult = new OperationReportingResult(list);
			if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
			{
				OperationReportingException operationReportingException = new OperationReportingException(string.Format("GetList - No ObjectXml [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
				throw operationReportingException;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(operationReportingResult.ObjectXml);
			if (xmlDocument.FirstChild != null)
			{
				this._originalFieldsSchemaXML = xmlDocument.FirstChild.SelectSingleNode(".//Fields").OuterXml;
			}
			xmlNodes = base.AttachVirtualData(xmlDocument.FirstChild, "XML");
			return xmlNodes;
		}

		public SecurityPrincipalCollection GetReferencedPrincipals()
		{
			Hashtable hashtables = new Hashtable();
			foreach (SPAlert alert in this.Alerts)
			{
				string user = alert.User;
				if (!string.IsNullOrEmpty(user))
				{
					if (!hashtables.ContainsKey(user))
					{
						hashtables.Add(user, user);
					}
				}
			}
			return SPUtils.GetReferencedPrincipals(hashtables, this.Principals, false);
		}

		internal override SPRoleAssignmentCollection GetRoleAssignmentCollection(bool bAlwaysRefetch)
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
			str = (!this.HasUniquePermissions ? this.ParentWeb.GetRoleAssignmentCollection(false).ToXML() : base.Adapter.Reader.GetRoleAssignments(this.ConstantID, -1));
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			XmlNode xmlNodes = base.AttachVirtualData(xmlDocument.DocumentElement, "RoleAssignments");
			mRoleAssignments = new SPRoleAssignmentCollection(this, xmlNodes);
			return mRoleAssignments;
		}

		internal SPViewCollection GetViews(bool bAlwaysRefetch)
		{
			SPViewCollection mViews;
			lock (this.m_oLockViews)
			{
				if ((this.m_views == null ? false : !bAlwaysRefetch))
				{
					mViews = this.m_views;
					return mViews;
				}
			}
			XmlNode xmlNodes = this.GetListXML(bAlwaysRefetch).SelectSingleNode("//Views");
			mViews = new SPViewCollection(this, xmlNodes);
			return mViews;
		}

		internal SPWorkflowAssociationCollection GetWorkflowAssociations(bool bAlwaysRefetch)
		{
			SPWorkflowAssociationCollection mWorkflowAssociations;
			lock (this.m_oLockWorkflowAssociations)
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

		private bool IsCustomizedLibraryTemplate()
		{
			int num;
			bool flag;
			flag = ((this.BaseType != Metalogix.SharePoint.ListType.DocumentLibrary || base.IsDocumentSet ? true : this.ListXML.GetAttributeValueAsBoolean("IsCatalog")) ? false : int.TryParse(this.BaseTemplate.ToString(), out num));
			return flag;
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable comparableNode, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			int num;
			string name;
			bool flag1;
			bool flag2;
			string value;
			string str;
			bool flag3;
			bool flag4;
			bool flag5 = true;
			string[] strArrays = new string[] { "MajorVersionLimit", "MajorWithMinorVersionsLimit", "DraftVersionVisibility", "ContentTypesEnabled", "ReadSecurity", "WriteSecurity", "BrowserEnabledDocuments", "SendToLocationName", "SendToLocationUrl", "EmailAlias", "Folders", "OnQuickLaunch" };
			string[] strArrays1 = strArrays;
			strArrays = new string[] { "EnableSyndication", "NoCrawl", "EnableAssignToEmail" };
			string[] strArrays2 = strArrays;
			strArrays = new string[] { "EmailAttachmentFolder", "EmailOverWrite", "EmailSaveOriginal", "EmailSaveMeetings", "EmailUseSecurity", "RssLimitDescriptionLength", "RssChannelTitle", "RssChannelDescription", "RssChannelImageUrl", "RssItemLimit", "RssDayLimit", "RssDocumentAsEnclosure", "RssDocumentAsLink" };
			string[] strArrays3 = strArrays;
			if (comparableNode is SPList)
			{
				SPList sPList = (SPList)comparableNode;
				XmlNode listXML = this.ListXML;
				XmlNode xmlNodes = sPList.ListXML;
				bool flag6 = false;
				if (listXML.Attributes.Count < xmlNodes.Attributes.Count)
				{
					listXML = sPList.ListXML;
					xmlNodes = this.ListXML;
					flag6 = true;
				}
				foreach (XmlAttribute attribute in listXML.Attributes)
				{
					if (!(attribute.Name == "ID"))
					{
						if (!(attribute.Name == "NextAvailableID"))
						{
							if ((attribute.Name == "DirName" || attribute.Name == "Created" || attribute.Name == "Modified" ? false : !(attribute.Name == "ItemCount")))
							{
								if (!(attribute.Name == "FeatureId"))
								{
									if (!string.Equals(attribute.Name, "DocTemplateUrl", StringComparison.OrdinalIgnoreCase))
									{
										if (!string.Equals(attribute.Name, "DocTemplateId", StringComparison.OrdinalIgnoreCase))
										{
											XmlAttribute itemOf = xmlNodes.Attributes[attribute.Name];
											if (itemOf == null)
											{
												if (attribute.Name.Equals("WelcomePage"))
												{
													continue;
												}
												else if (!(attribute.Name == "EmailAlias"))
												{
													if (!base.Adapter.SharePointVersion.IsSharePoint2003)
													{
														flag2 = true;
													}
													else
													{
														flag2 = (attribute.Name == "MajorWithMinorVersionsLimit" || attribute.Name == "SendToLocationName" || attribute.Name == "SendToLocationUrl" || attribute.Name == "ContentTypesEnabled" ? false : !(attribute.Name == "DraftVersionVisibility"));
													}
													if (flag2)
													{
														flag = false;
														num = 0;
														while (num < (int)strArrays2.Length)
														{
															if (!(strArrays2[num] == attribute.Name))
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
														if (!flag)
														{
															num = 0;
															while (num < (int)strArrays3.Length)
															{
																if (!(strArrays3[num] == attribute.Name))
																{
																	num++;
																}
																else
																{
																	flag = true;
																	break;
																}
															}
														}
														if (!flag)
														{
															differencesOutput.Write(string.Concat("The attribute: '", attribute.Name, "' is missing. "), attribute.Name, DifferenceStatus.Missing);
															flag5 = false;
															continue;
														}
														else if (flag)
														{
															continue;
														}
													}
													else
													{
														continue;
													}
												}
												else
												{
													differencesOutput.Write("The attribute: 'EmailAlias' is missing. This could be because the address was already in use.", "EmailAlias", DifferenceStatus.Missing);
													flag5 = false;
													continue;
												}
											}
											if (attribute.Value != itemOf.Value)
											{
												if (!(attribute.Name != "BaseType" ? true : !(listXML.Attributes["BaseTemplate"].Value == "108")))
												{
													differencesOutput.Write("The 'BaseType' attribute value is different. This is expected behaviour in the case of discussion lists. ", "BaseType", DifferenceStatus.Difference, true);
													continue;
												}
												else if (!(attribute.Name != "Name" ? true : attribute.Value.IndexOf("(") < 0))
												{
													differencesOutput.Write("The 'Name' attribute value is different. This is expected behaviour when the source list contains a bracket. ", "Name", DifferenceStatus.Difference, true);
													continue;
												}
												else if (!(attribute.Name != "EnableModeration" ? true : this.BaseTemplate != ListTemplateType.PictureLibrary))
												{
													differencesOutput.Write("The 'EnableModeration' attribute value is different. Note: this value may not be set on 'Picture Libraries' in SharePoint V3. ", "EnableModeration", DifferenceStatus.Difference, true);
													continue;
												}
												else if (!attribute.Name.Equals("EnableAssignToEmail", StringComparison.OrdinalIgnoreCase))
												{
													if (attribute.Name == "FeatureId")
													{
														string str1 = attribute.Value.Replace("{", "");
														str1 = str1.Replace("}", "");
														if (itemOf.Value.Replace("{", "").Replace("}", "").ToUpper() == str1.ToUpper())
														{
															continue;
														}
													}
													flag = false;
													if (options.Level == CompareLevel.Moderate)
													{
														num = 0;
														while (num < (int)strArrays1.Length)
														{
															if (!(strArrays1[num] == attribute.Name))
															{
																num++;
															}
															else
															{
																differencesOutput.Write(string.Concat("The attribute ", attribute.Name, " is different. Note: this attribute can't preserved when writing to a native web service."), attribute.Name);
																flag = true;
																break;
															}
														}
													}
													if (!flag)
													{
														differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. "), attribute.Name);
														flag5 = false;
													}
												}
												else
												{
													differencesOutput.Write("The 'EnableAssignToEmail' attribute value is different. Note: This attribute is not copied in general migration copying, but only in an independent list e-mail notification settings copy action. ", "EnableAssignToEmail", DifferenceStatus.Difference, true);
													continue;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				SPFieldCollection fields = (SPFieldCollection)this.Fields;
				SPFieldCollection sPFieldCollections = (SPFieldCollection)sPList.Fields;
				if (flag6)
				{
					fields = (SPFieldCollection)sPList.Fields;
					sPFieldCollections = (SPFieldCollection)this.Fields;
				}
				if (fields.Count > sPFieldCollections.Count)
				{
					if (!flag6)
					{
						fields = (SPFieldCollection)sPList.Fields;
						listXML = sPList.ListXML;
						sPFieldCollections = (SPFieldCollection)this.Fields;
						xmlNodes = this.ListXML;
					}
					if (flag6)
					{
						fields = (SPFieldCollection)this.Fields;
						listXML = this.ListXML;
						sPFieldCollections = (SPFieldCollection)sPList.Fields;
						xmlNodes = sPList.ListXML;
					}
				}
				foreach (SPField field in fields)
				{
					name = field.Name;
					if (field.FieldXML.Attributes["Type"] != null)
					{
						if (field.Type == "Computed")
						{
							continue;
						}
					}
					else if (xmlNodes.SelectSingleNode(string.Concat("./Fields/Field[@Name='", name, "']")) != null)
					{
						differencesOutput.Write(string.Concat("The field with InternalName: '", name, "' is missing on the source but exists on the target. "), name, DifferenceStatus.Missing);
						flag5 = false;
						continue;
					}
					string displayName = field.DisplayName;
					if (displayName != null)
					{
						SPField fieldByNames = sPFieldCollections.GetFieldByNames(displayName, name);
						if (fieldByNames == null)
						{
							if (this.BaseType != Metalogix.SharePoint.ListType.DiscussionForum)
							{
								flag3 = true;
							}
							else
							{
								flag3 = (displayName == "Ordering" ? false : !(displayName == "Thread ID"));
							}
							if (flag3)
							{
								differencesOutput.Write(string.Concat("The field with DisplayName: '", displayName, "' is missing. "), displayName, DifferenceStatus.Missing);
								flag5 = false;
								continue;
							}
							else
							{
								differencesOutput.Write(string.Concat("The field with DisplayName: '", displayName, "' is missing. This is expected behaviour in the case of discussion lists. "), displayName, DifferenceStatus.Missing, true);
								continue;
							}
						}
						else if (fieldByNames.DisplayName != null)
						{
							if (fieldByNames.Name != name)
							{
								if ((fieldByNames.Name.ToString() != "CheckoutUser" ? true : !(name.ToString() == "CheckedOutTitle")))
								{
									differencesOutput.Write(string.Concat("The internal field name is different on the field: '", displayName, "'. "), displayName, DifferenceStatus.Difference, true);
								}
								else
								{
									continue;
								}
							}
							if ((field.Type == "Lookup" ? true : field.Type == "LookupMulti"))
							{
								if (field.FieldXML.Attributes["TargetListName"] != null)
								{
									value = field.FieldXML.Attributes["TargetListName"].Value;
								}
								else
								{
									value = null;
								}
								string str2 = value;
								if (fieldByNames.FieldXML.Attributes["TargetListName"] != null)
								{
									str = fieldByNames.FieldXML.Attributes["TargetListName"].Value;
								}
								else
								{
									str = null;
								}
								if (str2 != str)
								{
									differencesOutput.Write(string.Concat("The target lookup field with DisplayName: '", displayName, "' doesn't point to a list on the target with the same name as the source lookup field. "), displayName);
									flag5 = false;
									continue;
								}
							}
						}
						else
						{
							differencesOutput.Write(string.Concat("The field with InternalName: '", name, "' is missing on the source but exists on the target. "), name, DifferenceStatus.Missing);
							flag5 = false;
							continue;
						}
					}
					else if (xmlNodes.SelectSingleNode(string.Concat("./Fields/Field[@Name='", name, "']")) == null)
					{
						differencesOutput.Write(string.Concat("The field with InternalName: '", name, "' is missing. "), name, DifferenceStatus.Missing);
						if (!(name == "IssueID" ? false : !(name == "RelatedID")))
						{
							continue;
						}
						else if ((name == "ThreadID" || name == "Ordering" ? this.BaseType != Metalogix.SharePoint.ListType.DiscussionForum : true))
						{
							flag5 = false;
							continue;
						}
						else
						{
							continue;
						}
					}
				}
				XmlNodeList xmlNodeLists = listXML.SelectNodes("./Views/View");
				XmlNodeList xmlNodeLists1 = xmlNodes.SelectNodes("./Views/View");
				if (xmlNodeLists.Count > xmlNodeLists1.Count)
				{
					xmlNodeLists = xmlNodeLists1;
					xmlNodes = listXML;
				}
				foreach (XmlNode xmlNodes1 in xmlNodeLists)
				{
					name = xmlNodes1.Attributes["DisplayName"].Value;
					if (string.IsNullOrEmpty(name))
					{
						flag4 = false;
					}
					else
					{
						flag4 = (xmlNodes1.Attributes["Hidden"] == null ? true : !xmlNodes1.Attributes["Hidden"].Value.Equals("true", StringComparison.CurrentCultureIgnoreCase));
					}
					if (flag4)
					{
						if ((SPList.ContainsView(xmlNodes, name) ? false : name != ""))
						{
							differencesOutput.Write(string.Concat("The target list is missing the view: '", name, "'."), name, DifferenceStatus.Missing);
							flag5 = false;
						}
					}
				}
				flag1 = flag5;
			}
			else
			{
				if (!(comparableNode is SPFolder))
				{
					throw new Exception("An SPList can only be compared to another SPList");
				}
				flag1 = comparableNode.IsEqual(this, differencesOutput, options);
			}
			return flag1;
		}

		private bool IsSupportedLibraryTemplateForMigrationPipeline()
		{
			return ((!this.IsDocumentLibrary || this.BaseTemplate != ListTemplateType.DocumentLibrary && this.BaseTemplate != ListTemplateType.MySiteDocumentLibrary && this.BaseTemplate != ListTemplateType.PrivateDocumentLibrary && this.BaseTemplate != ListTemplateType.PersonalDocumentLibrary || base.IsDocumentSet ? true : this.ListXML.GetAttributeValueAsBoolean("IsCatalog")) ? false : true);
		}

		private bool IsSupportedListTemplateForMigrationPipeline()
		{
			bool flag;
			ListTemplateType baseTemplate = this.BaseTemplate;
			if (baseTemplate > ListTemplateType.ProjectTasks)
			{
				if (baseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy || baseTemplate == ListTemplateType.Issues)
				{
					flag = true;
					return flag;
				}
				flag = false;
				return flag;
			}
			else
			{
				switch (baseTemplate)
				{
					case ListTemplateType.CustomList:
					case ListTemplateType.Survey:
					case ListTemplateType.Links:
					case ListTemplateType.Announcements:
					case ListTemplateType.Contacts:
					case ListTemplateType.Events:
					case ListTemplateType.Tasks:
					case ListTemplateType.DiscussionBoard:
					{
						break;
					}
					case ListTemplateType.DocumentLibrary:
					{
						flag = false;
						return flag;
					}
					default:
					{
						if (baseTemplate == ListTemplateType.ProjectTasks)
						{
							break;
						}
						flag = false;
						return flag;
					}
				}
			}
			flag = true;
			return flag;
		}

		private void LoadConstantProperties(Dictionary<string, string> constantProperties)
		{
			string str;
			if (constantProperties.TryGetValue("ID", out str))
			{
				this.m_sID = str;
			}
			if (constantProperties.TryGetValue("DirName", out str))
			{
				this.m_sDirName = str;
			}
			if (constantProperties.TryGetValue("Name", out str))
			{
				this.m_sName = str;
			}
			if (constantProperties.TryGetValue("Title", out str))
			{
				this.m_sTitle = str;
			}
			if (constantProperties.TryGetValue("Created", out str))
			{
				this.m_dtCreated = Utils.ParseDateAsUtc(str);
			}
			if (constantProperties.TryGetValue("Modified", out str))
			{
				this.m_dtModified = Utils.ParseDateAsUtc(str);
			}
			if (constantProperties.TryGetValue("BaseTemplate", out str))
			{
				this.m_sBaseTemplate = str;
				this.m_baseTemplate = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), this.m_sBaseTemplate);
			}
			if (constantProperties.TryGetValue("BaseType", out str))
			{
				this.m_baseType = (Metalogix.SharePoint.ListType)Enum.Parse(typeof(Metalogix.SharePoint.ListType), str);
			}
			if (constantProperties.TryGetValue("HasUniquePermissions", out str))
			{
				this.m_bHasUniquePermissions = new bool?(bool.Parse(str));
			}
		}

		public void RefreshFields(bool bAlwaysRefetch)
		{
			lock (this.m_oLockFields)
			{
				this.m_fields = this.GetFields(bAlwaysRefetch);
			}
		}

		public string RefreshXML()
		{
			this.FetchData();
			return this.XML;
		}

		public override void ReleasePermissionsData()
		{
			this.ReleasePermissionsData(true);
		}

		private void ReleasePermissionsData(bool bReleaseStateVariable)
		{
			lock (this.m_oLockRoles)
			{
				this.m_roles = null;
			}
			lock (this.m_oLockRoleAssignments)
			{
				if (this.m_roleAssignments != null)
				{
					this.m_roleAssignments.OnCollectionChanged -= new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
				}
				this.m_roleAssignments = null;
			}
			if (bReleaseStateVariable)
			{
				lock (this.m_oLockUniquePermissions)
				{
					this.m_bHasUniquePermissions = null;
				}
			}
		}

		public void ReorderContentTypes(string[] sContentTypeNames)
		{
			try
			{
				if ((int)sContentTypeNames.Length > 1)
				{
					base.Adapter.Writer.ReorderContentTypes(this.ID, sContentTypeNames);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (exception.Message.Contains("Ordering of content types is not available using an CSOM connection"))
				{
					throw new ArgumentException(exception.Message, exception);
				}
				throw;
			}
			this.m_contentTypes = null;
		}

		private void RoleAssignmentCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			lock (this.m_oLockUniquePermissions)
			{
				this.m_bHasUniquePermissions = new bool?(true);
			}
			if (this.HasFetched)
			{
				if (this.ListXML.Attributes["HasUniquePermissions"] != null)
				{
					this.ListXML.Attributes["HasUniquePermissions"].Value = "True";
				}
			}
		}

		private void SaveConstantProperties(Dictionary<string, string> constantProperties)
		{
			constantProperties.Clear();
			constantProperties.Add("ID", this.m_sID);
			constantProperties.Add("DirName", this.m_sDirName);
			constantProperties.Add("Name", this.m_sName);
			constantProperties.Add("Title", this.m_sTitle);
			constantProperties.Add("Created", Utils.FormatDateToUTC(this.m_dtCreated));
			constantProperties.Add("Modified", Utils.FormatDateToUTC(this.m_dtModified));
			constantProperties.Add("BaseTemplate", this.m_sBaseTemplate);
			constantProperties.Add("BaseType", this.m_baseType.ToString());
			if (this.m_bHasUniquePermissions.HasValue)
			{
				constantProperties.Add("HasUniquePermissions", this.m_bHasUniquePermissions.ToString());
			}
		}

		public void SetXML(XmlNode xml)
		{
			this._xmlNode = xml;
			this.StoreQuickProperties(xml);
			this.m_fields = null;
			this.m_views = null;
		}

		private void StoreQuickProperties(XmlNode listXML)
		{
			this.m_sID = listXML.Attributes["ID"].Value;
			this.m_sDirName = listXML.Attributes["DirName"].Value;
			this.m_sName = listXML.Attributes["Name"].Value;
			this.m_sTitle = listXML.Attributes["Title"].Value;
			this.m_dtCreated = Utils.ParseDateAsUtc(listXML.Attributes["Created"].Value);
			this.m_dtModified = Utils.ParseDateAsUtc(listXML.Attributes["Modified"].Value);
			this.m_sBaseTemplate = listXML.Attributes["BaseTemplate"].Value;
			this.m_baseTemplate = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), this.m_sBaseTemplate);
			string value = listXML.Attributes["BaseType"].Value;
			this.m_baseType = (Metalogix.SharePoint.ListType)Enum.Parse(typeof(Metalogix.SharePoint.ListType), value);
			this.m_iItemCount = Convert.ToInt32(listXML.Attributes["ItemCount"].Value);
			if (listXML.Attributes["HasUniquePermissions"] != null)
			{
				lock (this.m_oLockUniquePermissions)
				{
					this.m_bHasUniquePermissions = new bool?(bool.Parse(listXML.Attributes["HasUniquePermissions"].Value));
				}
			}
		}

		public override void UpdateCurrentNode()
		{
			this.ClearExcessNodeData();
			if (base.HasEventClients)
			{
				this.FetchData();
				base.UpdateCurrentNode();
			}
		}

		public OperationReportingResult UpdateList(string sListXML, bool bUpdateFields, bool bUpdateViews)
		{
			return this.UpdateList(sListXML, null, bUpdateFields, bUpdateViews, false);
		}

		public void UpdateList(string sListXML, string sViewXml, bool bUpdateFields, bool bUpdateViews)
		{
			this.UpdateList(sListXML, sViewXml, bUpdateFields, bUpdateViews, false);
		}

		public OperationReportingResult UpdateList(string sListXML, string sViewXml, bool bUpdateFields, bool bUpdateViews, bool bCopyEnableAssignToEmail)
		{
			OperationReportingResult operationReportingResult = this.UpdateList(sListXML, sViewXml, bUpdateFields, bUpdateViews, bCopyEnableAssignToEmail, null);
			return operationReportingResult;
		}

		public OperationReportingResult UpdateList(string sListXML, string sViewXml, bool bUpdateFields, bool bUpdateViews, bool bCopyEnableAssignToEmail, byte[] templateFileBytes)
		{
			UpdateListOptions updateListOption = new UpdateListOptions()
			{
				CopyFields = bUpdateFields,
				CopyViews = bUpdateViews,
				UpdateFieldTypes = bUpdateFields,
				CopyEnableAssignToEmail = bCopyEnableAssignToEmail
			};
			return this.UpdateList(sListXML, updateListOption, sViewXml, templateFileBytes);
		}

		public OperationReportingResult UpdateList(string sListXML, UpdateListOptions options, string sViewXml = null, byte[] templateFileBytes = null)
		{
			XmlNode innerXml;
			OperationReportingResult operationReportingResult;
			if (AdapterConfigurationVariables.MigrateLanguageSettings)
			{
				sListXML = XmlUtility.AddLanguageSettingsAttribute(sListXML, "List", null);
				sListXML = XmlUtility.AddLanguageSettingsAttribute(sListXML, "List/Fields/Field", null);
			}
			if ((!AdapterConfigurationVariables.MigrateLanguageSettings || !AdapterConfigurationVariables.MigrateLanguageSettingForViews ? false : options.CopyViews))
			{
				SPUtils.SetLanguageResourcesCollection(sListXML, this.ParentWeb);
			}
			if (!this.WriteVirtually)
			{
				if (this.ParentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				string str = this.ParentWeb.Adapter.Writer.UpdateList(this.ConstantID, sListXML, sViewXml, options, templateFileBytes);
				OperationReportingResult operationReportingResult1 = new OperationReportingResult(str);
				if (string.IsNullOrEmpty(operationReportingResult1.ObjectXml))
				{
					OperationReportingException operationReportingException = new OperationReportingException(string.Format("Error [{0}]", operationReportingResult1.GetMessageOfFirstErrorElement), operationReportingResult1.AllReportElementsAsString);
					throw operationReportingException;
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(operationReportingResult1.ObjectXml);
				this.m_fields = null;
				this.m_views = null;
				this._hasFetched = false;
				this._xmlNode = null;
				this.StoreQuickProperties(xmlDocument.DocumentElement);
				this.SaveConstantProperties(this.m_constantProperties);
				operationReportingResult = operationReportingResult1;
			}
			else
			{
				if (base.GetVirtualRecord() != null)
				{
					XmlNode listXML = this.ListXML;
					XmlNode xmlNode = XmlUtility.StringToXmlNode(sListXML);
					if (!options.CopyViews)
					{
						innerXml = xmlNode.SelectSingleNode("./Views");
						innerXml.InnerXml = listXML.SelectSingleNode("./Views").InnerXml;
					}
					else if (!string.IsNullOrEmpty(sViewXml))
					{
						innerXml = xmlNode.SelectSingleNode("./Views");
						innerXml.InnerXml = XmlUtility.StringToXmlNode(sViewXml).InnerXml;
					}
					if (!options.CopyFields)
					{
						XmlNode xmlNodes = xmlNode.SelectSingleNode("./Fields");
						xmlNodes.InnerXml = listXML.SelectSingleNode("./Fields").InnerXml;
					}
					base.SaveVirtualData(listXML, xmlNode, "XML");
					this._xmlNode = xmlNode;
					this.StoreQuickProperties(xmlNode);
					if ((!options.CopyFields ? false : this.m_fields != null))
					{
						this.m_fields.ResetFields(xmlNode.SelectSingleNode("./Fields"));
					}
					if ((!options.CopyViews ? false : this.m_views != null))
					{
						this.m_views.ResetViews(xmlNode.SelectSingleNode("./Views"));
					}
				}
				operationReportingResult = null;
			}
			return operationReportingResult;
		}

		internal void UpdatePredictedNextAvailableID(SPListItem item)
		{
			if (!(item == null))
			{
				int d = item.ID + 1;
				if (d > this.PredictedNextAvailableID)
				{
					lock (this._predictedNextAvailableIDLock)
					{
						if (d > this.PredictedNextAvailableID)
						{
							this.PredictedNextAvailableID = d;
						}
						else
						{
							return;
						}
					}
				}
			}
		}

		public override void UpdateSettings(string sListXML)
		{
			this.UpdateList(sListXML, null, true, true, false);
		}
	}
}