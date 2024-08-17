using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebPartPage : Metalogix.DataStructures.IComparable, IOperationLogging
	{
		private string m_sServerRelativeUrl = null;

		private SPWebPartCollection m_WebParts = null;

		private XmlNode m_WebPartConnections = null;

		private SharePointAdapter m_Adapter = null;

		private XmlNode m_WebPartPageXml = null;

		private SPList m_ParentList = null;

		private SPWeb m_ParentWeb = null;

		private SPListItem m_AssociatedListItem = null;

		private SPWebPartZoneSet m_ZonesOnPage = new SPWebPartZoneSet();

		private object m_oLockRecord = new object();

		private Record m_Record = null;

		private bool? m_bHasEmbeddedWikiField = null;

		private readonly static string[] m_sFieldsThatSupportEmbedding;

		private bool m_bListItemCheckNecessary;

		private object m_oListItemCheckLock = new object();

		private SPWebPartZoneSet m_ZonesFromHtml = null;

		public SharePointAdapter Adapter
		{
			get
			{
				return this.m_Adapter;
			}
		}

		public int BestDashboardPageTemplateId
		{
			get
			{
				return SPWebPartPageTemplate.DetermineBestDashboardTemplateId(this.Zones);
			}
		}

		public int BestPageTemplateId
		{
			get
			{
				return SPWebPartPageTemplate.DetermineBestTemplateId(this.Zones);
			}
		}

		public string DisplayUrl
		{
			get
			{
				string str = string.Concat(this.Adapter.ServerDisplayName, "/", this.ServerRelativeUrl);
				return str;
			}
		}

		public string EmbeddedContentFieldContent
		{
			get
			{
				string item;
				Record virtualRecord = this.GetVirtualRecord();
				if (virtualRecord != null)
				{
					RecordPropertyValue recordPropertyValue = virtualRecord.Properties["EmbeddedContent"];
					if (recordPropertyValue != null)
					{
						TextMoniker value = recordPropertyValue.Value as TextMoniker;
						if (value != null)
						{
							string fullText = value.GetFullText();
							if (!string.IsNullOrEmpty(fullText))
							{
								item = fullText;
								return item;
							}
						}
					}
				}
				if ((this.ListItem == null ? false : this.ParentList != null))
				{
					List<string> strs = new List<string>(SPWebPartPage.m_sFieldsThatSupportEmbedding);
					foreach (SPField field in this.ParentList.Fields)
					{
						if (strs.Contains(field.Name))
						{
							item = this.ListItem[field.Name];
							return item;
						}
					}
					item = null;
				}
				else
				{
					item = null;
				}
				return item;
			}
		}

		public string FileDirRef
		{
			get
			{
				string value;
				if (this.PageXml.Attributes["FileDirRef"] == null)
				{
					value = null;
				}
				else
				{
					value = this.PageXml.Attributes["FileDirRef"].Value;
				}
				return value;
			}
		}

		public string FileLeafRef
		{
			get
			{
				string value;
				if (this.PageXml.Attributes["FileLeafRef"] == null)
				{
					value = null;
				}
				else
				{
					value = this.PageXml.Attributes["FileLeafRef"].Value;
				}
				return value;
			}
		}

		public bool HasEmbeddedWikiField
		{
			get
			{
				bool flag = false;
				if (!this.m_bHasEmbeddedWikiField.HasValue)
				{
					this.m_bHasEmbeddedWikiField = new bool?(false);
					XmlNode xmlNodes = this.m_WebPartPageXml.SelectSingleNode(".//HasEmbeddedWikiField");
					if (!(xmlNodes == null ? true : !bool.TryParse(xmlNodes.InnerText, out flag)))
					{
						this.m_bHasEmbeddedWikiField = new bool?(flag);
					}
					else if (!string.IsNullOrEmpty(this.EmbeddedContentFieldContent))
					{
						this.m_bHasEmbeddedWikiField = new bool?(true);
					}
					if (this.m_bHasEmbeddedWikiField.Value)
					{
						this.m_bHasEmbeddedWikiField = new bool?(this.GetPageBinaryIncludesEmbeddedField());
					}
				}
				return this.m_bHasEmbeddedWikiField.Value;
			}
		}

		private string Id
		{
			get
			{
				string value;
				if (this.PageXml.Attributes["UniqueId"] == null)
				{
					value = null;
				}
				else
				{
					value = this.PageXml.Attributes["UniqueId"].Value;
				}
				return value;
			}
		}

		public SPListItem ListItem
		{
			get
			{
				if (this.m_AssociatedListItem == null)
				{
					this.EnsureListItemAndListProperties();
				}
				return this.m_AssociatedListItem;
			}
		}

		public IOperationLoggingManagement OperationLoggingManagement
		{
			get;
			set;
		}

		private string PageLayout
		{
			get
			{
				string value;
				if (this.PageXml.Attributes["PageLayout"] != null)
				{
					value = this.PageXml.Attributes["PageLayout"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		private XmlNode PageXml
		{
			get
			{
				return this.m_WebPartPageXml;
			}
		}

		public SPList ParentList
		{
			get
			{
				if (this.m_ParentList == null)
				{
					this.EnsureListItemAndListProperties();
				}
				return this.m_ParentList;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_ParentWeb;
			}
		}

		public string ServerRelativeUrl
		{
			get
			{
				string str = string.Concat(this.FileDirRef, "/", this.FileLeafRef);
				return str.TrimStart(new char[] { '/', '\\' });
			}
		}

		private string TemplateFile
		{
			get
			{
				string value;
				if (this.PageXml.Attributes["TemplateFile"] != null)
				{
					value = this.PageXml.Attributes["TemplateFile"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public XmlNode WebPartConnections
		{
			get
			{
				return this.m_WebPartConnections;
			}
		}

		public SPWebPartCollection WebParts
		{
			get
			{
				return this.m_WebParts;
			}
		}

		public string Xml
		{
			get
			{
				return this.m_WebPartPageXml.OuterXml;
			}
		}

		public SPWebPartZoneSet Zones
		{
			get
			{
				return SPWebPartZoneSet.Union(this.ZonesInUse, this.ZonesFromPage);
			}
		}

		private SPWebPartZoneSet ZonesFromHtml
		{
			get
			{
				if (this.m_ZonesFromHtml == null)
				{
					this.m_ZonesFromHtml = new SPWebPartZoneSet();
					XmlNode xmlNodes = this.m_WebPartPageXml.SelectSingleNode(".//ZonesFromAspx");
					if ((xmlNodes == null ? false : !string.IsNullOrEmpty(xmlNodes.InnerText)))
					{
						string[] strArrays = xmlNodes.InnerText.Split(new char[] { ',' });
						this.m_ZonesFromHtml.AddZones(strArrays);
					}
				}
				return this.m_ZonesFromHtml;
			}
		}

		public SPWebPartZoneSet ZonesFromPage
		{
			get
			{
				return this.m_ZonesOnPage;
			}
		}

		public SPWebPartZoneSet ZonesInUse
		{
			get
			{
				SPWebPartZoneSet sPWebPartZoneSet = new SPWebPartZoneSet();
				foreach (SPWebPart mWebPart in this.m_WebParts)
				{
					sPWebPartZoneSet.AddZone(mWebPart.Zone);
				}
				return sPWebPartZoneSet;
			}
		}

		static SPWebPartPage()
		{
			SPWebPartPage.m_sFieldsThatSupportEmbedding = new string[] { "WikiField", "PublishingPageContent" };
		}

		public SPWebPartPage(SPWeb web, XmlNode webPartPageXml)
		{
			this.m_sServerRelativeUrl = web.ServerRelativeUrl;
			this.InitializeWebPartPage(web, null, webPartPageXml.OuterXml);
		}

		public SPWebPartPage(SPWeb web, string sWebPartPageXml)
		{
			this.m_sServerRelativeUrl = web.ServerRelativeUrl;
			this.InitializeWebPartPage(web, null, sWebPartPageXml);
		}

		public SPWebPartPage(SPListItem webPartPageItem, IOperationLoggingManagement getAction)
		{
			this.ConnectOperationLogging(getAction);
			this.m_sServerRelativeUrl = webPartPageItem.ServerRelativeUrl;
			string webPartPage = webPartPageItem.Adapter.Reader.GetWebPartPage(this.m_sServerRelativeUrl);
			OperationReportingResult operationReportingResult = new OperationReportingResult(webPartPage);
			this.LogWebPartsFailureInJobLogs(operationReportingResult, this.m_sServerRelativeUrl);
			this.m_ParentList = webPartPageItem.ParentList;
			this.InitializeWebPartPage(webPartPageItem.ParentList.ParentWeb, webPartPageItem, operationReportingResult.ObjectXml);
		}

		public SPWebPartPage(SPView view, SPList parentList, IOperationLoggingManagement getAction)
		{
			this.ConnectOperationLogging(getAction);
			string url = view.Url;
			this.m_ParentList = view.ParentList;
			if (!url.StartsWith("/"))
			{
				string serverRelativeUrl = parentList.ParentWeb.ServerRelativeUrl;
				char[] chrArray = new char[] { '/' };
				url = string.Format("/{0}/{1}", serverRelativeUrl.Trim(chrArray), url);
			}
			this.m_sServerRelativeUrl = url;
			string webPartPage = parentList.ParentWeb.Adapter.Reader.GetWebPartPage(this.m_sServerRelativeUrl);
			OperationReportingResult operationReportingResult = new OperationReportingResult(webPartPage);
			this.LogWebPartsFailureInJobLogs(operationReportingResult, this.m_sServerRelativeUrl);
			this.InitializeWebPartPage(parentList.ParentWeb, null, operationReportingResult.ObjectXml);
		}

		public SPWebPartPage()
		{
		}

		public void AddWebPart(string sWebPartXml)
		{
			this.AddWebParts(string.Concat("<WebParts>", sWebPartXml, "</WebParts>"), null);
		}

		public void AddWebParts(string sWebPartsXml)
		{
			this.AddWebParts(sWebPartsXml, null);
		}

		public void AddWebParts(string sWebPartsXml, string sEmbeddedHtmlContent)
		{
			Record virtualRecord = this.GetVirtualRecord();
			if (virtualRecord == null)
			{
				if (this.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				if (AdapterConfigurationVariables.ListViewToXsltViewConversion)
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebPartsXml);
					if ((xmlNode.OwnerDocument == null ? false : xmlNode.Attributes != null))
					{
						XmlAttribute str = xmlNode.OwnerDocument.CreateAttribute(XmlAttributeNames.ListViewToXsltViewConversion.ToString());
						str.Value = true.ToString();
						xmlNode.Attributes.Append(str);
						sWebPartsXml = xmlNode.OuterXml;
					}
				}
				if (this.Adapter.SharePointVersion.IsSharePoint2013OrLater)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(sWebPartsXml);
					this.UpdateCategoriesAndLinksWebPartsXml(xmlDocument);
					sWebPartsXml = xmlDocument.OuterXml;
				}
				try
				{
					this.ParentWeb.AquireWebPartLock();
					this.Adapter.Writer.AddWebParts(sWebPartsXml, this.ServerRelativeUrl, sEmbeddedHtmlContent);
				}
				finally
				{
					this.ParentWeb.ReleaseWebPartLock();
				}
				this.UpdateWebPartCollection();
			}
			else
			{
				if (!string.IsNullOrEmpty(sEmbeddedHtmlContent))
				{
					PropertyDescriptor propertyDescriptor = virtualRecord.ParentWorkspace.EnsureProperty("EmbeddedContent", typeof(TextMoniker), "EditScripts");
					(propertyDescriptor.GetValue(virtualRecord) as TextMoniker).SetFullText(sEmbeddedHtmlContent);
				}
				XmlNode xmlNodes = XmlUtility.StringToXmlNode(sWebPartsXml);
				XmlNode xmlNode1 = XmlUtility.StringToXmlNode(this.m_WebParts.Xml);
				foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes("./*[local-name()='WebPart']"))
				{
					XmlUtility.CloneXMLNodeInto(xmlNodes1, xmlNode1, true);
				}
				this.m_WebParts = new SPWebPartCollection(this, xmlNode1);
			}
			this.UpdateWebPartsInPageXml(virtualRecord);
		}

		internal XmlNode AttachVirtualData(XmlNode sourceNode, string sPropertyName)
		{
			XmlNode xmlNodes = sourceNode;
			try
			{
				Record virtualRecord = this.GetVirtualRecord();
				if (virtualRecord != null)
				{
					xmlNodes = MetabaseUtility.ProcessEditScript(sourceNode, virtualRecord, sPropertyName);
				}
			}
			catch (Exception exception)
			{
			}
			return xmlNodes;
		}

		public void CloseAllWebParts()
		{
			Record virtualRecord = this.GetVirtualRecord();
			if (virtualRecord == null)
			{
				if (this.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				try
				{
					this.ParentWeb.AquireWebPartLock();
					this.Adapter.Writer.CloseWebParts(this.ServerRelativeUrl);
				}
				finally
				{
					this.ParentWeb.ReleaseWebPartLock();
				}
				this.UpdateWebPartCollection();
			}
			else
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(this.m_WebParts.Xml);
				foreach (XmlNode str in xmlNode.SelectNodes("./*[local-name()='WebPart']/*[local-name() = 'IsIncluded']"))
				{
					str.InnerText = false.ToString();
				}
				this.m_WebParts = new SPWebPartCollection(this, xmlNode);
			}
			this.UpdateWebPartsInPageXml(virtualRecord);
		}

		private void ConnectOperationLogging(IOperationLoggingManagement getAction)
		{
			if (getAction != null)
			{
				this.OperationLoggingManagement = getAction;
				this.OperationLoggingManagement.ConnectOperationLogging(this);
			}
		}

		public void DeleteAllWebParts()
		{
			Record virtualRecord = this.GetVirtualRecord();
			if (virtualRecord == null)
			{
				if (this.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				try
				{
					this.ParentWeb.AquireWebPartLock();
					this.Adapter.Writer.DeleteWebParts(this.ServerRelativeUrl);
				}
				finally
				{
					this.ParentWeb.ReleaseWebPartLock();
				}
				this.UpdateWebPartCollection();
			}
			else
			{
				this.m_WebParts = new SPWebPartCollection(this, "<WebParts />");
			}
			this.UpdateWebPartsInPageXml(virtualRecord);
		}

		public void DeleteWebPart(string sId)
		{
			if (!string.IsNullOrEmpty(sId))
			{
				Record virtualRecord = this.GetVirtualRecord();
				if (virtualRecord == null)
				{
					if (this.Adapter.Writer == null)
					{
						throw new Exception("The underlying SharePoint adapter does not support write operations");
					}
					try
					{
						this.ParentWeb.AquireWebPartLock();
						this.Adapter.Writer.DeleteWebPart(this.ServerRelativeUrl, sId);
					}
					finally
					{
						this.ParentWeb.ReleaseWebPartLock();
					}
					this.UpdateWebPartCollection();
				}
				else
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(this.m_WebParts.Xml);
					XmlNode xmlNodes = xmlNode.SelectSingleNode(string.Concat("./*[local-name()='WebPart' and @ID = '", sId, "']"));
					if (xmlNodes == null)
					{
						xmlNodes = xmlNode.SelectSingleNode(string.Concat("./*[local-name()='webParts' and @ID = '", sId, "']"));
					}
					if (xmlNodes != null)
					{
						xmlNode.RemoveChild(xmlNodes);
					}
					this.m_WebParts = new SPWebPartCollection(this, xmlNode);
				}
				this.UpdateWebPartsInPageXml(virtualRecord);
			}
		}

		private void EnsureListItemAndListProperties()
		{
			lock (this.m_oListItemCheckLock)
			{
				if (this.m_bListItemCheckNecessary)
				{
					this.m_bListItemCheckNecessary = false;
					SPListItem nodeByUrl = this.ParentWeb.GetNodeByUrl(this.DisplayUrl) as SPListItem;
					if (nodeByUrl != null)
					{
						this.m_AssociatedListItem = nodeByUrl;
						this.m_ParentList = this.m_AssociatedListItem.ParentList;
					}
				}
			}
		}

		public void FireOperationFinished(LogItem operation)
		{
			if (this.OperationFinished != null)
			{
				this.OperationFinished(operation);
			}
		}

		public void FireOperationStarted(LogItem operation)
		{
			if (this.OperationStarted != null)
			{
				this.OperationStarted(operation);
			}
		}

		public void FireOperationUpdated(LogItem operation)
		{
			if (this.OperationUpdated != null)
			{
				this.OperationUpdated(operation);
			}
		}

		public int GetNextAvailablePartOrder(string sZone)
		{
			int partOrder = 0;
			List<SPWebPart> webPartsInZone = this.WebParts.GetWebPartsInZone(sZone);
			if (webPartsInZone.Count > 0)
			{
				SPWebPartCollection.SortWebPartsByPartOrder(webPartsInZone, false);
				partOrder = webPartsInZone[0].PartOrder + 1;
			}
			return partOrder;
		}

	    public Dictionary<string, int> GetNextAvailablePartOrders()
	    {
	        Dictionary<string, int> dictionary = new Dictionary<string, int>();
	        foreach (string current in this.Zones)
	        {
	            dictionary[current] = 0;
	        }
	        Dictionary<string, List<SPWebPart>> webPartsSortedByZoneAndPartOrder = SPWebPartCollection.GetWebPartsSortedByZoneAndPartOrder(this.WebParts, false);
	        foreach (string current in webPartsSortedByZoneAndPartOrder.Keys)
	        {
	            int num = 0;
	            foreach (SPWebPart current2 in webPartsSortedByZoneAndPartOrder[current])
	            {
	                if (current2.IsViewWebPart && num == 0)
	                {
	                    num = current2.PartOrder;
	                }
	                else if (current2.PartOrder + 1 > num)
	                {
	                    num = current2.PartOrder + 1;
	                }
	            }
	            dictionary[current] = num;
	        }
	        return dictionary;
	    }

        private bool GetPageBinaryIncludesEmbeddedField()
		{
			string str;
			bool flag;
			if (!(this.ListItem == null))
			{
				try
				{
					str = Encoding.UTF8.GetString(this.ListItem.Binary);
				}
				catch
				{
					flag = false;
					return flag;
				}
				flag = str.IndexOf("<SharePoint:EmbeddedFormField", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private Record GetVirtualRecord()
		{
			if (this.m_Record == null)
			{
				lock (this.m_oLockRecord)
				{
					if (this.m_Record == null)
					{
						Record record = null;
						SPNode mAssociatedListItem = null;
						if (this.m_AssociatedListItem != null)
						{
							mAssociatedListItem = this.m_AssociatedListItem;
						}
						else if (this.m_ParentList == null)
						{
							mAssociatedListItem = this.m_ParentWeb;
						}
						else
						{
							mAssociatedListItem = this.m_ParentList;
						}
						if ((mAssociatedListItem == null ? false : mAssociatedListItem.WriteVirtually))
						{
							MetabaseConnection virtualConnection = mAssociatedListItem.VirtualConnection;
							string str = string.Concat(mAssociatedListItem.WorkspaceGuid, "SPWebPartPage");
							Workspace workspace = virtualConnection.GetWorkspace(str);
							if (workspace == null)
							{
								workspace = virtualConnection.CreateWorkspace(str);
							}
							record = workspace.FetchSingleRecord(this.m_sServerRelativeUrl);
						}
						this.m_Record = record;
					}
				}
			}
			return this.m_Record;
		}

		public SPWebPartPage GetWebPartPage(SPWeb web, string pageUrl, IOperationLoggingManagement getAction)
		{
			this.ConnectOperationLogging(getAction);
			string webPartPage = web.Adapter.Reader.GetWebPartPage(pageUrl);
			OperationReportingResult operationReportingResult = new OperationReportingResult(webPartPage);
			this.LogWebPartsFailureInJobLogs(operationReportingResult, pageUrl);
			return new SPWebPartPage(web, operationReportingResult.ObjectXml);
		}

		public SPWebPartPage GetWelcomePage(SPWeb web, IOperationLoggingManagement getAction)
		{
			this.ConnectOperationLogging(getAction);
			OperationReportingResult operationReportingResult = null;
			SPWebPartPage sPWebPartPage = null;
			string empty = string.Empty;
			if (web.WelcomePageUrl != null)
			{
				string str = Utils.CombineUrls(web.Url, web.WelcomePageUrl);
				SPListItem nodeByUrl = web.GetNodeByUrl(str) as SPListItem;
				empty = web.WelcomePageUrl;
				if (!(nodeByUrl != null))
				{
					operationReportingResult = new OperationReportingResult(web.Adapter.Reader.GetWebPartPage(web.WelcomePageUrl));
					sPWebPartPage = new SPWebPartPage(web, operationReportingResult.ObjectXml);
				}
				else
				{
					sPWebPartPage = new SPWebPartPage(nodeByUrl, getAction);
				}
			}
			else if (web.DefaultPageInRoot)
			{
				empty = "/default.aspx";
				ISharePointReader reader = web.Adapter.Reader;
				string serverRelativeUrl = web.ServerRelativeUrl;
				char[] chrArray = new char[] { '/' };
				operationReportingResult = new OperationReportingResult(reader.GetWebPartPage(string.Concat(serverRelativeUrl.TrimEnd(chrArray), empty)));
				sPWebPartPage = new SPWebPartPage(web, operationReportingResult.ObjectXml);
			}
			this.LogWebPartsFailureInJobLogs(operationReportingResult, empty);
			return sPWebPartPage;
		}

		private void InitializeWebPartCollection(XmlNode webParts)
		{
			if ((this.Adapter.IsDB || this.Adapter.IsNws ? false : !this.Adapter.IsClientOM))
			{
				XmlNode xmlNodes = webParts.SelectSingleNode("./*[local-name() = 'webPartConnections']");
				if (xmlNodes != null)
				{
					this.m_WebPartConnections = xmlNodes;
					xmlNodes.ParentNode.RemoveChild(xmlNodes);
				}
				this.m_WebParts = new SPWebPartCollection(this, webParts.OuterXml);
			}
			else
			{
				this.m_WebParts = new SPWebPartCollection(this, webParts);
			}
		}

		private void InitializeWebPartPage(SPWeb web, SPListItem webPartPageItem, string sWebPartPageXml)
		{
			this.m_ParentWeb = web;
			this.m_Adapter = web.Adapter;
			this.m_AssociatedListItem = webPartPageItem;
			this.m_bListItemCheckNecessary = webPartPageItem == null;
			if (string.IsNullOrEmpty(sWebPartPageXml))
			{
				throw new Exception(string.Concat("Error: Attempt to create web part page without an internal XML definition. On web: ", this.ParentWeb.ServerRelativeUrl));
			}
			this.m_WebPartPageXml = this.AttachVirtualData(XmlUtility.StringToXmlNode(sWebPartPageXml), "WebPartXML");
			this.InitializeWebPartCollection(this.m_WebPartPageXml.SelectSingleNode("./*[local-name() = 'WebParts']"));
			this.InitializeZoneList();
		}

		private void InitializeZoneList()
		{
			if (this.ZonesFromHtml.IsEmpty())
			{
				SPWebPartZoneSet availableZonesFromTemplate = SPWebPartPageTemplate.GetAvailableZonesFromTemplate(this.TemplateFile);
				if (!availableZonesFromTemplate.IsEmpty())
				{
					this.m_ZonesOnPage = availableZonesFromTemplate;
				}
				else if (this.PageLayout == null)
				{
					this.m_ZonesOnPage = SPWebPartPageTemplate.GetAvailableZonesForTemplateId(SPWebPartPageTemplate.DetermineBestTemplateId(this.ZonesInUse));
				}
				else
				{
					availableZonesFromTemplate = SPWebPartPageTemplate.GetAvailableZonesFromTemplate(this.PageLayout);
					if (!availableZonesFromTemplate.IsEmpty())
					{
						this.m_ZonesOnPage = availableZonesFromTemplate;
					}
				}
			}
			else
			{
				this.m_ZonesOnPage = this.ZonesFromHtml;
			}
		}

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag = true;
			if (!(targetComparable is SPWebPartPage))
			{
				throw new Exception("An SPWebPartPage can only be compared to another SPWebPartPage.");
			}
			SPWebPartPage sPWebPartPage = (SPWebPartPage)targetComparable;
			if (!this.Zones.IsEqual(sPWebPartPage.Zones))
			{
				flag = false;
				differencesOutput.Write(string.Concat("The available web part zones are different: Source ", this.Zones.ToString(), ", Target ", sPWebPartPage.Zones.ToString()), "Web part zones");
			}
			XmlNode pageXml = sPWebPartPage.PageXml;
			foreach (XmlAttribute attribute in this.PageXml.Attributes)
			{
				if ((attribute.Name == "UniqueId" || attribute.Name == "FileDirRef" || attribute.Name == "MetaInfo" || attribute.Name == "TemplateFile" || attribute.Name == "DocFlags" || attribute.Name == "Content" || attribute.Name == "Unghosted" || attribute.Name == "HasPersonalizedView" ? false : !(attribute.Name == "HasStream")))
				{
					XmlAttribute itemOf = pageXml.Attributes[attribute.Name];
					if (itemOf == null)
					{
						differencesOutput.Write(string.Concat("The attribute: '", attribute.Name, "' is missing. "), attribute.Name, DifferenceStatus.Missing);
						flag = false;
					}
					else if (attribute.Value.ToLower() != itemOf.Value.ToLower())
					{
						differencesOutput.Write(string.Concat("The attribute value: '", attribute.Name, "' is different. "), attribute.Name);
						flag = false;
					}
				}
			}
			return flag;
		}

		public static bool IsWebPartPage(SPListItem pageItem)
		{
			bool flag;
			bool flag1 = false;
			if (pageItem.ServerRelativeUrl.ToLower().EndsWith(".aspx"))
			{
				XmlNode xmlNode = null;
				xmlNode = XmlUtility.StringToXmlNode(pageItem.XML);
				bool flag2 = false;
				if (!(xmlNode.Attributes["HTML_x0020_File_x0020_Type"] == null ? true : !(xmlNode.Attributes["HTML_x0020_File_x0020_Type"].Value == "SharePoint.Link")))
				{
					flag1 = false;
				}
				else if ((xmlNode.Attributes["HTML_x0020_File_x0020_Type"] == null ? true : !(xmlNode.Attributes["HTML_x0020_File_x0020_Type"].Value == "SharePoint.WebPartPage.Document")))
				{
					if (!pageItem.Adapter.SharePointVersion.IsSharePoint2010OrLater || pageItem.GetContentType() == null || string.Equals(pageItem.GetContentType().Name, "Redirect Page", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
					else
					{
						flag = (pageItem.HasPublishingPageContent ? false : !pageItem.HasWikiField);
					}
					if (!flag)
					{
						flag1 = true;
					}
					else if (!(!bool.TryParse(pageItem.Adapter.Reader.HasWebParts(pageItem.ConstantServerRelativeUrl), out flag2) ? true : !flag2))
					{
						flag1 = true;
					}
					else if (pageItem.HasPublishingPageLayout)
					{
						flag1 = true;
					}
					else if ((xmlNode.Attributes["_SetupPath"] == null ? true : !SPWebPartPageTemplate.IsWebPartPageTemplateFile(xmlNode.Attributes["_SetupPath"].Value)))
					{
						try
						{
							if ((new SPWebPartPage(pageItem, null)).Zones.Count > 0)
							{
								flag1 = true;
							}
						}
						catch
						{
							flag1 = false;
						}
					}
					else
					{
						flag1 = true;
					}
				}
				else
				{
					flag1 = true;
				}
			}
			else
			{
				flag1 = false;
			}
			return flag1;
		}

		public static bool IsWebPartPage(SharePointAdapter adapter, string pageUrl)
		{
			bool flag = false;
			bool.TryParse(adapter.Reader.HasWebParts(pageUrl), out flag);
			if (adapter == null)
			{
				throw new ArgumentNullException("adapter");
			}
			return flag;
		}

		private void LogWebPartsFailureInJobLogs(OperationReportingResult opResult, string itemName)
		{
			string empty = string.Empty;
			string getMessageOfFirstErrorElement = string.Empty;
			if (opResult != null)
			{
				if (opResult.WarningOccured)
				{
					empty = opResult.GetAllWarningsAsString;
					getMessageOfFirstErrorElement = string.Format("Error occurred while retrieving web parts from page '{0}'", itemName);
				}
				else if (opResult.ErrorOccured)
				{
					empty = opResult.GetAllErrorsAsString;
					getMessageOfFirstErrorElement = opResult.GetMessageOfFirstErrorElement;
				}
				if (!string.IsNullOrEmpty(empty))
				{
					LogItem logItem = new LogItem("Retrieving Web Parts", itemName, string.Empty, string.Empty, ActionOperationStatus.Failed);
					this.FireOperationStarted(logItem);
					logItem.Information = getMessageOfFirstErrorElement;
					logItem.Details = empty;
					this.FireOperationFinished(logItem);
				}
			}
		}

		internal void SaveVirtualData(XmlNode originalNode, XmlNode changedNode, string sPropertyName)
		{
			this.SaveVirtualData(originalNode, changedNode, sPropertyName, this.GetVirtualRecord());
		}

		internal void SaveVirtualData(XmlNode originalNode, XmlNode changedNode, string sPropertyName, Record record)
		{
			if (record != null)
			{
				MetabaseUtility.SaveXMLDiffAsProperty(originalNode, changedNode, record, sPropertyName, false);
				record.CommitChanges();
			}
		}

		private void UpdateCategoriesAndLinksWebPartsXml(XmlDocument webPartXmlDoc)
		{
			XmlNodeList xmlNodeLists = webPartXmlDoc.FirstChild.SelectNodes("./*[local-name()='WebPart'][*[local-name()='BaseTemplateID' and text()!='-1']]");
			try
			{
				if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
				{
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						short num = -1;
						short.TryParse(xmlNodes.SelectSingleNode("./*[local-name()='BaseTemplateID']").InnerXml, out num);
						XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./*[local-name()='ListViewXml']");
						xmlNodes1.InnerXml = HttpUtility.HtmlDecode(xmlNodes1.InnerXml);
						XmlNode empty = xmlNodes1.SelectSingleNode("./*[local-name()='View']");
						if (num == 103)
						{
							XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./*[local-name()='FrameType']");
							if ((xmlNodes2 == null ? false : xmlNodes2.InnerXml.Equals("None", StringComparison.InvariantCultureIgnoreCase)))
							{
								xmlNodes2.InnerXml = "Default";
							}
							if ((empty == null ? false : empty.Attributes["BaseViewID"] != null))
							{
								empty.Attributes["BaseViewID"].Value = "0";
							}
						}
						else if (num == 303)
						{
							if (!this.Adapter.IsClientOM)
							{
								empty.InnerXml = string.Empty;
							}
						}
						xmlNodes1.InnerXml = HttpUtility.HtmlEncode(xmlNodes1.InnerXml);
					}
				}
			}
			catch (Exception exception)
			{
			}
		}

		private void UpdateWebPartCollection()
		{
			string webPartsOnPage = this.m_Adapter.Reader.GetWebPartsOnPage(this.ServerRelativeUrl);
			if ((this.Adapter.IsDB || this.Adapter.IsNws ? false : !this.Adapter.IsClientOM))
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(webPartsOnPage);
				XmlNode xmlNodes = xmlNode.SelectSingleNode("./*[local-name() = 'webPartConnections']");
				if (xmlNodes != null)
				{
					this.m_WebPartConnections = xmlNodes;
					xmlNodes.ParentNode.RemoveChild(xmlNodes);
				}
				this.m_WebParts = new SPWebPartCollection(this, xmlNode.OuterXml);
			}
			else
			{
				this.m_WebParts = new SPWebPartCollection(this, webPartsOnPage);
			}
		}

		private void UpdateWebPartsInPageXml(Record virtualRecord)
		{
			string outerXml = null;
			if (virtualRecord != null)
			{
				outerXml = this.m_WebPartPageXml.OuterXml;
			}
			XmlNode innerXml = this.m_WebPartPageXml.SelectSingleNode("./*[local-name() = 'WebParts']");
			if (innerXml == null)
			{
				innerXml = this.m_WebPartPageXml.OwnerDocument.CreateElement("WebParts");
				this.m_WebPartPageXml.AppendChild(innerXml);
			}
			try
			{
				innerXml.InnerXml = XmlUtility.StringToXmlNode(this.m_WebParts.Xml).InnerXml;
			}
			catch
			{
			}
			if (virtualRecord != null)
			{
				this.SaveVirtualData(XmlUtility.StringToXmlNode(outerXml), this.m_WebPartPageXml, "WebPartXML", virtualRecord);
			}
		}

		public event ActionEventHandler OperationFinished;

		public event ActionEventHandler OperationStarted;

		public event ActionEventHandler OperationUpdated;
	}
}