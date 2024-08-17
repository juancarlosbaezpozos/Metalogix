using Metalogix.Utilities;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public abstract class SPWebPart
	{
		internal const string ZONE_ID = "ZoneID";

		internal const string PART_ORDER = "PartOrder";

		internal const string IS_INCLUDED = "IsIncluded";

		protected XmlNode m_Xml = null;

		private bool? m_bClosed = null;

		private bool? m_bIsViewWebPart = null;

		protected string m_sTypeName = null;

		protected string m_sAssembly = null;

		public abstract string Assembly
		{
			get;
		}

		public abstract string CatalogIconImageUrl
		{
			get;
			set;
		}

		public abstract string HelpUrl
		{
			get;
			set;
		}

		public string Id
		{
			get
			{
				XmlAttribute itemOf = this.m_Xml.Attributes["ID"];
				return (itemOf != null ? itemOf.Value : "");
			}
		}

		public bool IsClosed
		{
			get
			{
				if (!this.m_bClosed.HasValue)
				{
					this.m_bClosed = new bool?(false);
					XmlNode xmlNodes = this.m_Xml.SelectSingleNode("./*[local-name() = 'IsIncluded']");
					if (xmlNodes != null)
					{
						bool flag = true;
						if (bool.TryParse(xmlNodes.InnerXml, out flag))
						{
							this.m_bClosed = new bool?(!flag);
						}
					}
				}
				return this.m_bClosed.Value;
			}
		}

		public bool IsViewWebPart
		{
			get
			{
				string value;
				bool flag;
				if (!this.m_bIsViewWebPart.HasValue)
				{
					if ((this.TypeName == "Microsoft.SharePoint.WebPartPages.ListViewWebPart" ? false : !(this.TypeName == "Microsoft.SharePoint.WebPartPages.XsltListViewWebPart")))
					{
						this.m_bIsViewWebPart = new bool?(false);
					}
					else if (!this.HasProperty("IsViewWebPart"))
					{
						if (this.TypeName == "Microsoft.SharePoint.WebPartPages.ListViewWebPart")
						{
							if (this.HasProperty("ListViewXml"))
							{
								value = this.GetValue("ListViewXml");
							}
							else
							{
								value = null;
							}
						}
						else if (this.HasProperty("XmlDefinition"))
						{
							value = this.GetValue("XmlDefinition");
						}
						else
						{
							value = null;
						}
						if (!string.IsNullOrEmpty(value))
						{
							try
							{
								XmlNode xmlNode = XmlUtility.StringToXmlNode(value);
								XmlAttribute itemOf = xmlNode.Attributes["DisplayName"];
								if ((itemOf == null ? false : !string.IsNullOrEmpty(itemOf.Value)))
								{
									XmlAttribute xmlAttribute = xmlNode.Attributes["Hidden"];
									if (xmlAttribute == null)
									{
										this.m_bIsViewWebPart = new bool?(true);
									}
									else if ((!bool.TryParse(xmlAttribute.Value, out flag) ? true : !flag))
									{
										this.m_bIsViewWebPart = new bool?(true);
									}
									else
									{
										this.m_bIsViewWebPart = new bool?(false);
									}
								}
								else
								{
									this.m_bIsViewWebPart = new bool?(false);
								}
							}
							catch
							{
								this.m_bIsViewWebPart = new bool?(false);
							}
						}
						else
						{
							this.m_bIsViewWebPart = new bool?(false);
						}
					}
					else
					{
						this.m_bIsViewWebPart = new bool?(bool.Parse(this.GetValue("IsViewWebPart")));
					}
				}
				return this.m_bIsViewWebPart.Value;
			}
		}

		public string this[string sPropertyName]
		{
			get
			{
				return this.GetValue(sPropertyName);
			}
			set
			{
				this.SetValue(sPropertyName, value);
			}
		}

		public int PartOrder
		{
			get
			{
				int num = 0;
				XmlNode xmlNodes = this.m_Xml.SelectSingleNode("./*[local-name() = 'PartOrder']");
				if (xmlNodes != null)
				{
					int.TryParse(xmlNodes.InnerXml, out num);
				}
				return num;
			}
			set
			{
				XmlNode str = this.m_Xml.SelectSingleNode("./*[local-name() = 'PartOrder']");
				if (str == null)
				{
					str = this.m_Xml.OwnerDocument.CreateElement("PartOrder", this.m_Xml.NamespaceURI);
					this.m_Xml.AppendChild(str);
				}
				str.InnerText = value.ToString();
			}
		}

		public abstract string TitleIconUrl
		{
			get;
			set;
		}

		public abstract string TitleUrl
		{
			get;
			set;
		}

		public abstract string TypeName
		{
			get;
		}

		public string Xml
		{
			get
			{
				return this.m_Xml.OuterXml;
			}
		}

		public string Zone
		{
			get
			{
				XmlNode xmlNodes = this.m_Xml.SelectSingleNode("./*[local-name() = 'ZoneID']");
				return (xmlNodes == null ? "Left" : xmlNodes.InnerXml);
			}
			set
			{
				XmlNode xmlNodes = this.m_Xml.SelectSingleNode("./*[local-name() = 'ZoneID']");
				if (xmlNodes == null)
				{
					xmlNodes = this.m_Xml.OwnerDocument.CreateElement("ZoneID", this.m_Xml.NamespaceURI);
					this.m_Xml.AppendChild(xmlNodes);
				}
				xmlNodes.InnerText = value;
			}
		}

		protected SPWebPart(XmlNode webPartXml)
		{
			this.m_Xml = webPartXml.Clone();
		}

		protected SPWebPart(string sWebPartXml)
		{
			this.m_Xml = XmlUtility.StringToXmlNode(sWebPartXml);
		}

		public abstract SPWebPart Clone();

		public static SPWebPart CreateWebPart(XmlNode webPartXml)
		{
			SPWebPart sPWebPartV2;
			if (webPartXml.InnerXml.IndexOf("<webPart xmlns=\"http://schemas.microsoft.com/WebPart/v3\">") < 0)
			{
				sPWebPartV2 = new SPWebPartV2(webPartXml);
			}
			else
			{
				sPWebPartV2 = new SPWebPartV3(webPartXml);
			}
			return sPWebPartV2;
		}

		public static SPWebPart CreateWebPart(string sWebPartXml)
		{
			return SPWebPart.CreateWebPart(XmlUtility.StringToXmlNode(sWebPartXml));
		}

		public void DeleteProperty(string sPropertyName)
		{
			if (this.HasProperty(sPropertyName))
			{
				this.DeletePropertyFromWebPart(sPropertyName);
			}
		}

		protected abstract void DeletePropertyFromWebPart(string sPropertyName);

		protected abstract string GetValue(string sPropertyName);

		public bool HasProperty(string sPropertyName)
		{
			return this[sPropertyName] != null;
		}

		protected abstract void SetValue(string sPropertyName, string sValue);

		public static class Types
		{
			public const string DataViewWebPart = "Microsoft.SharePoint.WebPartPages.DataViewWebPart";

			public const string DataFormWebPart = "Microsoft.SharePoint.WebPartPages.DataFormWebPart";

			public const string ContentEditor = "Microsoft.SharePoint.WebPartPages.ContentEditorWebPart";

			public const string ListView = "Microsoft.SharePoint.WebPartPages.ListViewWebPart";

			public const string Image = "Microsoft.SharePoint.WebPartPages.ImageWebPart";

			public const string PageViewer = "Microsoft.SharePoint.WebPartPages.PageViewerWebPart";

			public const string ListForm = "Microsoft.SharePoint.WebPartPages.ListFormWebPart";

			public const string XsltListForm = "Microsoft.SharePoint.WebPartPages.XsltListFormWebPart";

			public const string RSSAggregator = "Microsoft.SharePoint.Portal.WebControls.RSSAggregatorWebPart";

			public const string SiteDocuments = "Microsoft.SharePoint.Portal.WebControls.SiteDocuments";

			public const string BlogAdminWebPart = "Microsoft.SharePoint.WebPartPages.BlogAdminWebPart";

			public const string BlogMonthQuickLaunch = "Microsoft.SharePoint.WebPartPages.BlogMonthQuickLaunch";

			public const string RightBodySectionSearchBox = "Microsoft.SharePoint.Portal.WebControls.RightBodySectionSearchBox";

			public const string BreadCrumbTrail = "Microsoft.SharePoint.Portal.WebControls.BreadCrumbTrail";

			public const string PageTabs = "Microsoft.SharePoint.Meetings.PageTabsWebPart";

			public const string ToolPaneManager = "Microsoft.SharePoint.Meetings.CustomToolPaneManager";

			public const string TitleBar = "Microsoft.SharePoint.WebPartPages.TitleBarWebPart";

			public const string IndicatorWebPart = "Microsoft.SharePoint.Portal.WebControls.IndicatorWebpart";

			public const string KpiListWebPart = "Microsoft.SharePoint.Portal.WebControls.KPIListWebPart";

			public const string ContentByQuery = "Microsoft.SharePoint.Publishing.WebControls.ContentByQueryWebPart";

			public const string TableOfContents = "Microsoft.SharePoint.Publishing.WebControls.TableOfContentsWebPart";

			public const string ContactDetail = "Microsoft.SharePoint.Portal.WebControls.ContactFieldControl";

			public const string SummaryLinks = "Microsoft.SharePoint.Publishing.WebControls.SummaryLinkWebPart";

			public const string TasksAndToolsWebPart = "Microsoft.SharePoint.Portal.WebControls.TasksAndToolsWebPart";

			public const string MembersWebPart = "Microsoft.SharePoint.WebPartPages.MembersWebPart";

			public const string ExcelWebAccessWebPart = "Microsoft.Office.Excel.WebUI.ExcelWebRenderer";

			public const string XsltListView = "Microsoft.SharePoint.WebPartPages.XsltListViewWebPart";

			public const string SocialCommentWebPart = "Microsoft.SharePoint.Portal.WebControls.SocialCommentWebPart";

			public const string TagCloudWebPart = "Microsoft.SharePoint.Portal.WebControls.TagCloudWebPart";

			public const string PictureLibrarySlideshowWebPart = "Microsoft.SharePoint.WebPartPages.PictureLibrarySlideshowWebPart";

			public const string TimelineWebPart = "Microsoft.SharePoint.WebPartPages.SPTimelineWebPart";

			public const string SpListFilterWebPart = "Microsoft.SharePoint.Portal.WebControls.SpListFilterWebPart";

			public const string RefinementScriptWebPart = "Microsoft.Office.Server.Search.WebControls.RefinementScriptWebPart";
		}
	}
}