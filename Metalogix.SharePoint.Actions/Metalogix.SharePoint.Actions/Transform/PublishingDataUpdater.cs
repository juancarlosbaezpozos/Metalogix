using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class PublishingDataUpdater : PreconfiguredTransformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>
	{
		private const string _WIKI_CONTENT_TEMPLATE = "<%@ Page Inherits=\"Microsoft.SharePoint.Publishing.TemplateRedirectionPage,Microsoft.SharePoint.Publishing,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c\" %> <%@ Reference VirtualPath=\"~TemplatePageUrl\" %> <%@ Reference VirtualPath=\"~masterurl/custom.master\" %><%@ Register Tagprefix=\"SharePoint\" Namespace=\"Microsoft.SharePoint.WebControls\" Assembly=\"Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" %><html xmlns:mso=\"urn:schemas-microsoft-com:office:office\" xmlns:msdt=\"uuid:C2F41010-65B3-11d1-A29F-00AA00C14882\"><head><!--[if gte mso 9]><SharePoint:CTFieldRefs runat=server Prefix=\"mso:\" FieldList=\"FileLeafRef,PublishingStartDate,PublishingExpirationDate,PublishingContactEmail,PublishingContactName,PublishingContactPicture,PublishingPageLayout,PublishingVariationGroupID,PublishingVariationRelationshipLinkFieldID,PublishingRollupImage,Audience,PublishingPageContent,AverageRating,RatingCount,Wiki_x0020_Page_x0020_CategoriesTaxHTField0,TaxCatchAllLabel,_dlc_DocId,_dlc_DocIdUrl,_dlc_DocIdPersistId\"><xml><mso:CustomDocumentProperties><mso:PublishingPageContent msdt:dt=\"string\">{0}</mso:PublishingPageContent><mso:PublishingPageLayout msdt:dt=\"string\">{1}</mso:PublishingPageLayout></mso:CustomDocumentProperties></xml></SharePoint:CTFieldRefs><![endif]--></head>";

		private SPPageLayout m_wikiLayout;

		private Dictionary<string, string> _publishingPageLayoutMapping = new Dictionary<string, string>();

		private readonly static string NEW_PUBLISHING_PAGE_CONTENT_STRING;

		public override string Name
		{
			get
			{
				return "Update Publishing Data";
			}
		}

		static PublishingDataUpdater()
		{
			PublishingDataUpdater.NEW_PUBLISHING_PAGE_CONTENT_STRING = "<%@ Page Language=\"C#\" Inherits=\"Microsoft.SharePoint.Publishing.TemplateRedirectionPage,Microsoft.SharePoint.Publishing, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" %><%@ Reference VirtualPath=\"~TemplatePageUrl\" %><%@ Reference VirtualPath=\"~masterurl/custom.master\" %><html></html>";
		}

		public PublishingDataUpdater()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (sources.ParentSPList.Adapter.SharePointVersion.IsSharePoint2007OrEarlier && targets.ParentSPList.BaseTemplate == ListTemplateType.O12Pages && targets.ParentSPList.Adapter.SharePointVersion.IsSharePoint2010)
			{
				this.SetEnterprisePageLayoutFromTarget(targets.ParentSPList);
			}
		}

		private XmlNode Create2010WikiXml(XmlNode node, string pubLayout)
		{
			string[] value = new string[] { "<ListItem ID=\"", node.Attributes["ID"].Value, "\"\r\n                        FileLeafRef=\"", node.Attributes["FileLeafRef"].Value, "\" Categories0=\"\"\r\n                        PublishingPageLayout=\"", pubLayout, "\"\r\n                        Modified=\"", node.Attributes["Modified"].Value, "\" Created=\"", node.Attributes["Created"].Value, "\"\r\n                        _ModerationStatus=\"", node.Attributes["_ModerationStatus"].Value, "\" Author=\"", node.Attributes["Author"].Value, "\"\r\n                        Editor=\"", node.Attributes["Editor"].Value, "\" _VersionLevel=\"", node.Attributes["_Level"].Value, "\"\r\n                        _VersionCheckinComment=\"\" _VersionString=\"", node.Attributes["_UIVersionString"].Value, "\" />" };
			return XmlUtility.StringToXmlNode(string.Concat(value));
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			this.m_wikiLayout = null;
			this._publishingPageLayoutMapping.Clear();
		}

		private byte[] GetNewPublishingPageData()
		{
			return Encoding.UTF8.GetBytes(PublishingDataUpdater.NEW_PUBLISHING_PAGE_CONTENT_STRING);
		}

		private SPPageLayout GetPageLayout(string pageLayout, SPFolder targetFolder)
		{
			Predicate<SPPageLayout> predicate = (SPPageLayout layout) => {
				if (layout == null)
				{
					return false;
				}
				return string.Equals(layout.Name, pageLayout, StringComparison.InvariantCultureIgnoreCase);
			};
			if (targetFolder.ParentList.ParentWeb.RootWeb == null || targetFolder.ParentList.ParentWeb.RootWeb.MasterPageGallery == null)
			{
				return null;
			}
			SPMasterPageGallery masterPageGallery = targetFolder.ParentList.ParentWeb.RootWeb.MasterPageGallery;
			return masterPageGallery.PageLayouts.Find(predicate);
		}

		private byte[] GetWikiBinary(string content, string pageLayout)
		{
			byte[] bytes;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.WriteElementString("Metalogix", content);
				}
				content = stringWriter.ToString();
				content = content.Replace("<Metalogix>", "");
				content = content.Replace("</Metalogix>", "");
				HttpUtility.HtmlAttributeEncode(content);
				content = string.Format("<%@ Page Inherits=\"Microsoft.SharePoint.Publishing.TemplateRedirectionPage,Microsoft.SharePoint.Publishing,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c\" %> <%@ Reference VirtualPath=\"~TemplatePageUrl\" %> <%@ Reference VirtualPath=\"~masterurl/custom.master\" %><%@ Register Tagprefix=\"SharePoint\" Namespace=\"Microsoft.SharePoint.WebControls\" Assembly=\"Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c\" %><html xmlns:mso=\"urn:schemas-microsoft-com:office:office\" xmlns:msdt=\"uuid:C2F41010-65B3-11d1-A29F-00AA00C14882\"><head><!--[if gte mso 9]><SharePoint:CTFieldRefs runat=server Prefix=\"mso:\" FieldList=\"FileLeafRef,PublishingStartDate,PublishingExpirationDate,PublishingContactEmail,PublishingContactName,PublishingContactPicture,PublishingPageLayout,PublishingVariationGroupID,PublishingVariationRelationshipLinkFieldID,PublishingRollupImage,Audience,PublishingPageContent,AverageRating,RatingCount,Wiki_x0020_Page_x0020_CategoriesTaxHTField0,TaxCatchAllLabel,_dlc_DocId,_dlc_DocIdUrl,_dlc_DocIdPersistId\"><xml><mso:CustomDocumentProperties><mso:PublishingPageContent msdt:dt=\"string\">{0}</mso:PublishingPageContent><mso:PublishingPageLayout msdt:dt=\"string\">{1}</mso:PublishingPageLayout></mso:CustomDocumentProperties></xml></SharePoint:CTFieldRefs><![endif]--></head>", content, pageLayout);
				bytes = Encoding.Default.GetBytes(content);
			}
			return bytes;
		}

		private static bool IsWebPartPage(SPListItem item)
		{
			return string.Equals(item["HTML_x0020_File_x0020_Type"], "SharePoint.WebPartPage.Document");
		}

		private bool IsWikiPageContentType(string contentTypeId)
		{
			if (string.IsNullOrEmpty(contentTypeId))
			{
				return false;
			}
			return contentTypeId.ToLower().StartsWith("0x010108");
		}

		private static string ParsePageLayoutName(string pageLayoutString)
		{
			string[] strArrays = new string[] { "," };
			string str = pageLayoutString.Split(strArrays, StringSplitOptions.None).First<string>((string part) => part.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase));
			if (string.IsNullOrEmpty(str))
			{
				throw new FormatException(string.Format(Resources.PublishingDataUpdater_ParsePageLayoutName_The_target_page_layout_name_could_not_be_set_as_its_URL_was_not_in_the_valid_format, pageLayoutString));
			}
			return HttpUtility.UrlDecode(Path.GetFileName(str));
		}

		private void RemoveWikiFieldFromItemXml(XmlNode itemXml)
		{
			XmlAttribute itemOf = itemXml.Attributes["WikiField"];
			if (itemOf != null)
			{
				itemXml.Attributes.Remove(itemOf);
			}
		}

		private void SetAppropriateLayoutFromTargetPageLayouts(SPListItem sourceItem, SPList targetList, XmlNode sourceItemXML)
		{
			string item = sourceItem["PublishingPageLayout"];
			string str = PublishingDataUpdater.ParsePageLayoutName(item);
			SPPageLayout pageLayout = null;
			if (!string.IsNullOrEmpty(str))
			{
				if (this._publishingPageLayoutMapping.ContainsKey(str))
				{
					sourceItemXML.Attributes["PublishingPageLayout"].Value = this._publishingPageLayoutMapping[str];
					return;
				}
				pageLayout = this.GetPageLayout(str, targetList);
				if (pageLayout != null)
				{
					this._publishingPageLayoutMapping.Add(str, item);
					sourceItemXML.Attributes["PublishingPageLayout"].Value = this._publishingPageLayoutMapping[str];
					return;
				}
			}
			if (!string.IsNullOrEmpty(str))
			{
				foreach (SPPageLayout sPPageLayout in 
					from page in ((targetList.ParentWeb.MasterPageGallery.PageLayouts.Count == 0 ? targetList.ParentWeb.RootSite.MasterPageGallery : targetList.ParentWeb.MasterPageGallery)).PageLayouts
					where page.ContentType != null
					select page)
				{
					if (sPPageLayout.ContentType.Name != sourceItem.GetContentType().Name)
					{
						continue;
					}
					this._publishingPageLayoutMapping.Add(str, sPPageLayout.DisplayUrl);
					sourceItemXML.Attributes["PublishingPageLayout"].Value = sPPageLayout.DisplayUrl;
					return;
				}
			}
			if (pageLayout == null)
			{
				throw new Exception(string.Format(Resources.PublishingDataUpdater_SetAppropriateLayoutFromTargetPageLayouts_PageLayoutNullException, str, sourceItem.GetContentType().Name));
			}
		}

		private void SetEnterprisePageLayoutFromTarget(SPList targetList)
		{
			SPPageLayout pageLayout = this.GetPageLayout("EnterpriseWiki.aspx", targetList);
			if (pageLayout == null)
			{
				throw new Exception(Resources.PublishingDataUpdater_SetEnterprisePageLayoutFromTarget_The_pagelayout__EnterpriseWikiaspx_could_not_be_found);
			}
			this.m_wikiLayout = pageLayout;
		}

		private bool ShouldRemoveWikiField(SPListItem item)
		{
			if (!PublishingDataUpdater.IsWebPartPage(item))
			{
				return false;
			}
			return !item.WebPartPage.HasEmbeddedWikiField;
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (sources.ParentSPList.IsDocumentLibrary && dataObject.ItemType != SPListItemType.Folder)
			{
				SPFolder parentFolder = targets.ParentFolder as SPFolder;
				XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
				bool isWebPartPage = false;
				if (!(dataObject.ParentList.Name == "Workflows") || dataObject.ParentList.BaseTemplate != ListTemplateType.NoCodeWorkflows)
				{
					isWebPartPage = dataObject.IsWebPartPage;
				}
				byte[] newPublishingPageData = null;
				if (isWebPartPage)
				{
					if (dataObject.HasPublishingPageLayout)
					{
						if (!dataObject.IsUnghostedPublishingPage)
						{
							newPublishingPageData = this.GetNewPublishingPageData();
						}
						else if (dataObject.Adapter.SharePointVersion.MajorVersion != parentFolder.Adapter.SharePointVersion.MajorVersion || action.SharePointOptions.ReattachPageLayouts)
						{
							newPublishingPageData = this.GetNewPublishingPageData();
							this.UnghostPublishingPageLayout(xmlNode);
							string str = (dataObject.Adapter.SharePointVersion.MajorVersion != parentFolder.Adapter.SharePointVersion.MajorVersion ? "Publishing page was copied between different SharePoint versions." : "Reattach page layouts option is turned on.");
							LogItem logItem = new LogItem("Copying Publishing Page", dataObject.Name, dataObject.DisplayUrl, parentFolder.DisplayUrl, ActionOperationStatus.Warning)
							{
								Information = string.Concat(str, " Customizations on this page cannot be preserved. Please, perform customizations manually.")
							};
							base.FireOperationStarted(logItem);
							base.FireOperationFinished(logItem);
						}
					}
					else if (dataObject.ParentList.BaseTemplate == ListTemplateType.ReportLibrary)
					{
						SPContentType contentType = dataObject.GetContentType();
						if (contentType != null && contentType.ContentTypeID.StartsWith("0x010100A2E3C117A0C5482FAEE3D57C48CB042F"))
						{
							int bestDashboardPageTemplateId = dataObject.WebPartPage.BestDashboardPageTemplateId;
							if (bestDashboardPageTemplateId >= 0 && bestDashboardPageTemplateId <= 2)
							{
								newPublishingPageData = parentFolder.Adapter.Reader.GetDashboardPageTemplate(bestDashboardPageTemplateId);
							}
							dataObject.Binary = newPublishingPageData;
						}
					}
					else if (this.ShouldRemoveWikiField(dataObject))
					{
						this.RemoveWikiFieldFromItemXml(xmlNode);
						int bestPageTemplateId = dataObject.WebPartPage.BestPageTemplateId;
						if (bestPageTemplateId >= 1 && bestPageTemplateId <= 8)
						{
							newPublishingPageData = parentFolder.Adapter.Reader.GetWebPartPageTemplate(bestPageTemplateId);
						}
					}
					if (newPublishingPageData != null)
					{
						dataObject.Binary = newPublishingPageData;
					}
				}
				if (dataObject.HasPublishingPageLayout && parentFolder.ParentList.ParentWeb.RootSite != null && parentFolder.ParentList.ParentWeb.RootSite.MasterPageGallery != null)
				{
					this.SetAppropriateLayoutFromTargetPageLayouts(dataObject, parentFolder.ParentList, xmlNode);
				}
				if (dataObject.HasWikiField && dataObject.Adapter.SharePointVersion.IsSharePoint2007OrEarlier && (parentFolder.ParentList.BaseTemplate == ListTemplateType.O12Pages || parentFolder.ParentList.BaseTemplate == ListTemplateType.WikiPageLibrary))
				{
					string value = xmlNode.Attributes["WikiField"].Value;
					byte[] wikiBinary = this.GetWikiBinary(value, string.Concat(parentFolder.ParentList.ParentWeb.RootWeb.ServerRelativeUrl, "/_catalogs/masterpage/EnterpriseWiki.aspx, Basic Page"));
					dataObject.Binary = wikiBinary;
					if (parentFolder.Adapter.SharePointVersion.IsSharePoint2010 && parentFolder.ParentList.BaseTemplate == ListTemplateType.O12Pages && this.m_wikiLayout != null)
					{
						string str1 = string.Format("{0}, {1}", this.m_wikiLayout.DisplayUrl, this.m_wikiLayout.Title);
						xmlNode = this.Create2010WikiXml(xmlNode, str1);
					}
				}
				dataObject.SetFullXML(xmlNode);
			}
			return dataObject;
		}

		private void UnghostPublishingPageLayout(XmlNode itemXML)
		{
			XmlAttribute itemOf = itemXML.Attributes["PublishingPageLayout"];
			if (itemOf != null)
			{
				string value = itemOf.Value;
				char[] chrArray = new char[] { ',' };
				string[] strArrays = value.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				if ((int)strArrays.Length != 2)
				{
					return;
				}
				itemOf.Value = strArrays[1];
			}
		}
	}
}