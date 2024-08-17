using Metalogix.Actions;
using Metalogix.Core.Support;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Interfaces;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public abstract class SPBaseServer : SPConnection, ITaxonomyConnection, IHasSupportInfo
	{
		private string m_sSystemInfo;

		private string m_sServerVersion;

		private bool? m_bStoragePointPresent = null;

		private SPSiteCollection m_sites;

		private SPWebApplicationCollection m_webApps;

		private SPLanguageCollection m_languages;

		private SPSiteQuotaCollection m_quotas;

		private SPTermStoreCollection _termStores = null;

		private readonly object _termStoresLock = new object();

		private SPAudienceCollection m_audiences = null;

		public SPAudienceCollection Audiences
		{
			get
			{
				if (this.m_audiences == null)
				{
					try
					{
						this.m_audiences = this.GetAudienceCollection(false);
					}
					catch (Exception exception)
					{
						throw new Exception(string.Concat("Could not fetch audiences: ", exception.Message));
					}
				}
				return this.m_audiences;
			}
		}

		public override string DisplayName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(base.Adapter.ServerDisplayName);
				stringBuilder.Append(" (");
				stringBuilder.Append((this.ShowAllSites ? "Farm " : "Web App "));
				stringBuilder.Append((base.Adapter.ServerType == "Auto Detect" ? "" : string.Concat("- ", base.Adapter.ServerType, " ")));
				stringBuilder.Append("- ");
				stringBuilder.Append(base.Adapter.LoggedInAs);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
		}

		public override string ImageName
		{
			get
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
				string str1 = string.Format(this.ImageResourceName, base.Adapter.DisplayedShortName, str, empty);
				return str1;
			}
		}

		public abstract string ImageResourceName
		{
			get;
		}

		public SPLanguageCollection Languages
		{
			get
			{
				if (this.m_languages == null)
				{
					this.m_languages = new SPLanguageCollection(this);
					this.m_languages.FetchData();
				}
				return this.m_languages;
			}
		}

		public override string Name
		{
			get
			{
				return base.Adapter.Server;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				return string.Empty;
			}
		}

		public string ServerVersion
		{
			get
			{
				if (this.m_sServerVersion == null)
				{
					this.m_sServerVersion = base.Adapter.GetServerVersion();
				}
				return this.m_sServerVersion;
			}
		}

		public bool ShowAllSites
		{
			get
			{
				return this.m_bShowAllSites;
			}
			set
			{
				this.m_bShowAllSites = value;
			}
		}

		public SPSiteQuotaCollection SiteQuotaTemplates
		{
			get
			{
				if (this.m_quotas == null)
				{
					this.m_quotas = this.GetSiteQuotaTemplates(false);
				}
				return this.m_quotas;
			}
		}

		public virtual SPSiteCollection Sites
		{
			get
			{
				if (this.m_sites == null)
				{
					SPSiteCollection sPSiteCollection = new SPSiteCollection(this);
					sPSiteCollection.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_Sites_CollectionChanged);
					sPSiteCollection.FetchData(this.ShowAllSites, null);
					this.m_sites = sPSiteCollection;
				}
				return this.m_sites;
			}
		}

		public bool StoragePointPresent
		{
			get
			{
				if (!this.m_bStoragePointPresent.HasValue)
				{
					bool flag = false;
					bool.TryParse(base.Adapter.Reader.StoragePointAvailable(string.Empty), out flag);
					this.m_bStoragePointPresent = new bool?(flag);
				}
				return this.m_bStoragePointPresent.Value;
			}
		}

		public string SystemInfo
		{
			get
			{
				if (this.m_sSystemInfo == null)
				{
					this.m_sSystemInfo = base.Adapter.SystemInfo.ToString();
				}
				return this.m_sSystemInfo;
			}
		}

		public SPTermStoreCollection TermStores
		{
			get
			{
				lock (this._termStoresLock)
				{
					if (this._termStores == null)
					{
						SPTermStoreCollection sPTermStoreCollection = new SPTermStoreCollection(this);
						sPTermStoreCollection.FetchData();
						this._termStores = sPTermStoreCollection;
					}
				}
				return this._termStores;
			}
		}

		public SPWebApplication WebApplication
		{
			get
			{
				SPWebApplication @default;
				try
				{
					@default = this.WebApplications.Default;
				}
				catch (Exception exception)
				{
					throw exception;
				}
				return @default;
			}
		}

		public SPWebApplicationCollection WebApplications
		{
			get
			{
				if (this.m_webApps == null)
				{
					this.m_webApps = new SPWebApplicationCollection(this);
					this.m_webApps.FetchData();
				}
				return this.m_webApps;
			}
		}

		public override string XML
		{
			get
			{
				return string.Empty;
			}
		}

		public SPBaseServer(XmlNode connectionNode) : base(connectionNode)
		{
		}

		public SPBaseServer(SharePointAdapter adapter) : base(adapter, null)
		{
		}

		public override bool AnalyzeChurn(DateTime pivotDate, bool bRecursive, out long lByteschanged, out long lItemsChanged)
		{
			lByteschanged = (long)0;
			lItemsChanged = (long)0;
			long num = (long)0;
			long num1 = (long)0;
			bool flag = false;
			foreach (SPSite site in this.Sites)
			{
				if (site.AnalyzeChurn(pivotDate, bRecursive, out num1, out num))
				{
					flag = true;
					lByteschanged += num1;
					lItemsChanged += lItemsChanged;
				}
			}
			return flag;
		}

		protected override void ClearChildNodes()
		{
			this.m_sites = null;
		}

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			this.m_webApps = null;
			this.m_languages = null;
			this.m_audiences = null;
		}

		public override void Close()
		{
			base.Close();
			if (this.m_sites != null)
			{
				foreach (SPSite mSite in this.m_sites)
				{
					mSite.Close();
				}
			}
		}

		private void EnsureNodeChecked(Metalogix.Explorer.Node node)
		{
			SPSite sPSite = node as SPSite;
			if ((sPSite == null ? false : sPSite.Status == ConnectionStatus.NotChecked))
			{
				sPSite.FetchChildren();
			}
		}

		protected override Metalogix.Explorer.Node[] FetchChildNodes()
		{
			Metalogix.Explorer.Node[] item = new Metalogix.Explorer.Node[this.Sites.Count];
			for (int i = 0; i < this.Sites.Count; i++)
			{
				item[i] = (SPNode)this.Sites[i];
			}
			return item;
		}

		internal SPAudienceCollection GetAudienceCollection(bool bAlwaysRefetch)
		{
			SPAudienceCollection sPAudienceCollections;
			if ((this.m_audiences == null ? true : bAlwaysRefetch))
			{
				string audiences = base.Adapter.Reader.GetAudiences();
				if (string.IsNullOrEmpty(audiences))
				{
					sPAudienceCollections = null;
				}
				else
				{
					sPAudienceCollections = new SPAudienceCollection(this, audiences);
				}
			}
			else
			{
				sPAudienceCollections = this.m_audiences;
			}
			return sPAudienceCollections;
		}

		public override Metalogix.Explorer.Node GetNodeByPath(string sPath)
		{
			Metalogix.Explorer.Node nodeByPath = base.GetNodeByPath(sPath);
			this.EnsureNodeChecked(nodeByPath);
			return nodeByPath;
		}

		public override Metalogix.Explorer.Node GetNodeByUrl(string sURL)
		{
			Metalogix.Explorer.Node nodeByUrl = this.Sites.GetNodeByUrl(sURL);
			this.EnsureNodeChecked(nodeByUrl);
			return nodeByUrl;
		}

		internal SPSiteQuotaCollection GetSiteQuotaTemplates(bool bAlwaysRefetch)
		{
			SPSiteQuotaCollection sPSiteQuotaCollections;
			if ((this.m_quotas == null ? true : bAlwaysRefetch))
			{
				string siteQuotaTemplates = base.Adapter.Reader.GetSiteQuotaTemplates();
				if (string.IsNullOrEmpty(siteQuotaTemplates))
				{
					sPSiteQuotaCollections = null;
				}
				else
				{
					sPSiteQuotaCollections = new SPSiteQuotaCollection(this, XmlUtility.StringToXmlNode(siteQuotaTemplates));
				}
			}
			else
			{
				sPSiteQuotaCollections = this.m_quotas;
			}
			return sPSiteQuotaCollections;
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable comparableNode, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected void On_Sites_CollectionChanged(NodeCollectionChangeType changeType, Metalogix.Explorer.Node changedNode)
		{
			base.SetChildren(null);
		}

		public override void UpdateCurrentNode()
		{
			this.ClearExcessNodeData();
			base.UpdateCurrentNode();
		}

		public bool ValidateUser(string sUserIdentifier, bool bCanBeDomainGroup, out string sUserName)
		{
			bool flag;
			sUserName = "";
			if (base.Adapter.Writer == null)
			{
				flag = false;
			}
			else
			{
				string str = base.Adapter.Writer.ValidateUserInfo(sUserIdentifier, bCanBeDomainGroup);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(str);
				XmlNode xmlNodes = xmlDocument.SelectSingleNode("//UserValidation");
				bool flag1 = bool.Parse(xmlNodes.Attributes["IsValid"].Value);
				if (flag1)
				{
					sUserName = (xmlNodes.Attributes["FullName"] != null ? xmlNodes.Attributes["FullName"].Value : sUserIdentifier);
				}
				flag = flag1;
			}
			return flag;
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