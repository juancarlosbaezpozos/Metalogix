using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(false, null, null)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.PortalListings.ico")]
	[MenuText("3:Paste Site Objects {0-Paste} > Portal Listings...")]
	[Name("Paste Portal Listings")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class PastePortalListingsAction : PasteAction<PastePortalListingsOptions>
	{
		private const string TITLE_BAR_WEB_PART = "<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.microsoft.com/WebPart/v2\" ID=\"3990f9a1-d9a1-4dfe-870a-25c7a8d9aaab\" Embedded=\"False\"><Title>Web Part Page Title Bar</Title><FrameType>None</FrameType><Description /><IsIncluded>true</IsIncluded><ZoneID>TitleBar</ZoneID><PartOrder>1</PartOrder><FrameState>Normal</FrameState><Height /><Width /><AllowRemove>false</AllowRemove><AllowZoneChange>true</AllowZoneChange><AllowMinimize>false</AllowMinimize><AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit><AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink /><HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall /><MissingAssembly>Cannot import this Web Part.</MissingAssembly><PartImageLarge /><IsIncludedFilter /><Assembly>Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly><TypeName>Microsoft.SharePoint.WebPartPages.TitleBarWebPart</TypeName><Image xmlns=\"http://schemas.microsoft.com/WebPart/v2/TitleBar\">{0}</Image><HeaderTitle xmlns=\"http://schemas.microsoft.com/WebPart/v2/TitleBar\">{1}</HeaderTitle><ZoneID xmlns=\"\">TitleBar</ZoneID><PartOrder xmlns=\"\">1</PartOrder><IsIncluded xmlns=\"\">true</IsIncluded><SharePointSourceVersion xmlns=\"\">12.0.0.6520</SharePointSourceVersion></WebPart>";

		private const string CONTENT_EDITOR_WEB_PART = "<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.microsoft.com/WebPart/v2\" ID=\"df11d14e-93c0-4cdd-8176-acd6e0e80aae\" Embedded=\"False\"><Title>{0}</Title><FrameType>None</FrameType><Description>Use for formatted text, tables, and images.</Description><IsIncluded>true</IsIncluded><ZoneID>FullPage</ZoneID><PartOrder>0</PartOrder><FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove><AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize><AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit><AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink /><HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall /><MissingAssembly>Cannot import this Web Part.</MissingAssembly><PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter /><Assembly>Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly><TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName><ContentLink xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /><Content xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\"><![CDATA[{1}]]></Content><PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /><ZoneID xmlns=\"\">FullPage</ZoneID><PartOrder xmlns=\"\">0</PartOrder><IsIncluded xmlns=\"\">true</IsIncluded><SharePointSourceVersion xmlns=\"\">12.0.0.6520</SharePointSourceVersion></WebPart>";

		public PastePortalListingsAction()
		{
		}

		private void AddListingHtmlBlobToDataLibrary(SPPortalListing listing, string sImageUrl, SPWeb targetWeb, ref SPList targetDataLib)
		{
			LogItem logItem = new LogItem("Adding listing data to data library", null, null, null, ActionOperationStatus.Running)
			{
				Operation = "Adding listing html content to data library"
			};
			base.FireOperationStarted(logItem);
			try
			{
				if (targetDataLib == null)
				{
					LogItem logItem1 = new LogItem("Locating portal listing data library", null, null, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem1);
					try
					{
						targetDataLib = this.CreateNewDataLibraryOrRetrieveExisting(targetWeb, base.SharePointOptions.OverwriteLists, true, logItem1);
						logItem1.Status = ActionOperationStatus.Completed;
						base.FireOperationFinished(logItem1);
					}
					catch (Exception exception)
					{
						logItem1.Exception = exception;
						base.FireOperationFinished(logItem1);
						throw new Exception("Failed to create data library");
					}
					LogItem logItem2 = new LogItem("Fetching data library data", null, null, targetDataLib.DisplayUrl, ActionOperationStatus.Running);
					try
					{
						targetDataLib.Items.FetchTerseData(true, ListItemQueryType.ListItem, null, new GetListItemOptions());
						logItem2.Status = ActionOperationStatus.Completed;
						base.FireOperationFinished(logItem2);
					}
					catch (Exception exception1)
					{
						logItem2.Exception = exception1;
						base.FireOperationFinished(logItem2);
						throw new Exception("Failed to fetch data library data");
					}
				}
				SPListItemCollection items = targetDataLib.Items;
				Guid listingID = listing.ListingID;
				string str = string.Concat(listingID.ToString().ToLower(), ".aspx");
				SPListItem sPListItem = null;
				foreach (SPListItem item in items)
				{
					if (!str.Equals(item.FileLeafRef, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					sPListItem = item;
					break;
				}
				if (sPListItem != null)
				{
					items.DeleteItem(sPListItem);
				}
				StringBuilder stringBuilder = new StringBuilder();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
				xmlTextWriter.WriteStartElement("ListItem");
				xmlTextWriter.WriteAttributeString("ID", "-1");
				xmlTextWriter.WriteAttributeString("FileLeafRef", str);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Flush();
				AddDocumentOptions addDocumentOption = new AddDocumentOptions()
				{
					AllowDBWriting = false,
					Overwrite = true,
					PreserveID = false
				};
				byte[] webPartPageTemplate = targetWeb.Adapter.Reader.GetWebPartPageTemplate(1);
				SPListItem sPListItem1 = items.AddDocument(stringBuilder.ToString(), addDocumentOption, webPartPageTemplate);
				SPWebPartPage sPWebPartPage = new SPWebPartPage(sPListItem1, this);
				string str1 = string.Format("<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.microsoft.com/WebPart/v2\" ID=\"3990f9a1-d9a1-4dfe-870a-25c7a8d9aaab\" Embedded=\"False\"><Title>Web Part Page Title Bar</Title><FrameType>None</FrameType><Description /><IsIncluded>true</IsIncluded><ZoneID>TitleBar</ZoneID><PartOrder>1</PartOrder><FrameState>Normal</FrameState><Height /><Width /><AllowRemove>false</AllowRemove><AllowZoneChange>true</AllowZoneChange><AllowMinimize>false</AllowMinimize><AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit><AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink /><HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall /><MissingAssembly>Cannot import this Web Part.</MissingAssembly><PartImageLarge /><IsIncludedFilter /><Assembly>Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly><TypeName>Microsoft.SharePoint.WebPartPages.TitleBarWebPart</TypeName><Image xmlns=\"http://schemas.microsoft.com/WebPart/v2/TitleBar\">{0}</Image><HeaderTitle xmlns=\"http://schemas.microsoft.com/WebPart/v2/TitleBar\">{1}</HeaderTitle><ZoneID xmlns=\"\">TitleBar</ZoneID><PartOrder xmlns=\"\">1</PartOrder><IsIncluded xmlns=\"\">true</IsIncluded><SharePointSourceVersion xmlns=\"\">12.0.0.6520</SharePointSourceVersion></WebPart>", sImageUrl, HttpUtility.HtmlEncode(listing["Title"]));
				string str2 = string.Format("<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://schemas.microsoft.com/WebPart/v2\" ID=\"df11d14e-93c0-4cdd-8176-acd6e0e80aae\" Embedded=\"False\"><Title>{0}</Title><FrameType>None</FrameType><Description>Use for formatted text, tables, and images.</Description><IsIncluded>true</IsIncluded><ZoneID>FullPage</ZoneID><PartOrder>0</PartOrder><FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove><AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize><AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit><AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink /><HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall /><MissingAssembly>Cannot import this Web Part.</MissingAssembly><PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter /><Assembly>Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly><TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName><ContentLink xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /><Content xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\"><![CDATA[{1}]]></Content><PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /><ZoneID xmlns=\"\">FullPage</ZoneID><PartOrder xmlns=\"\">0</PartOrder><IsIncluded xmlns=\"\">true</IsIncluded><SharePointSourceVersion xmlns=\"\">12.0.0.6520</SharePointSourceVersion></WebPart>", listing["Title"], listing["HtmlBlob"]);
				sPWebPartPage.AddWebPart(str1);
				sPWebPartPage.AddWebPart(str2);
				logItem.Status = ActionOperationStatus.Completed;
				logItem.LicenseDataUsed = (long)((int)webPartPageTemplate.Length) + SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception2)
			{
				logItem.Exception = exception2;
				base.FireOperationFinished(logItem);
				throw;
			}
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = sourceSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if ((enumerator.Current as SPWeb).Adapter.IsPortal2003Connection)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		private int CompareDatesIgnoreMilliseconds(DateTime date1, DateTime date2)
		{
			if (date1.Millisecond != 0)
			{
				date1 = new DateTime(date1.Year, date1.Month, date1.Day, date1.Hour, date1.Minute, date1.Second, date1.Kind);
			}
			if (date2.Millisecond != 0)
			{
				date2 = new DateTime(date2.Year, date2.Month, date2.Day, date2.Hour, date2.Minute, date2.Second, date2.Kind);
			}
			return date1.CompareTo(date2);
		}

		public void CopyPortalListings(SPWeb sourceWeb, SPWeb targetWeb)
		{
			SPPortalListingCollection portalListings;
			SPListItemCollection items;
			if (!sourceWeb.Adapter.IsPortal2003Connection)
			{
				return;
			}
			SPList sPList = null;
			SPList sPList1 = null;
			try
			{
				LogItem logItem = new LogItem("Fetching source listings", null, sourceWeb.DisplayUrl, null, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					portalListings = sourceWeb.PortalListings;
					portalListings.FetchTerseData(true);
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
					base.FireOperationFinished(logItem);
					return;
				}
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
				if (portalListings.Count != 0 || base.SharePointOptions.PropagateItemDeletions)
				{
					if (portalListings.Count > 0)
					{
						LogItem logItem1 = new LogItem("Sorting portal listings", null, null, null, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem1);
						portalListings.Sort();
						logItem1.Status = ActionOperationStatus.Completed;
						base.FireOperationFinished(logItem1);
					}
					LogItem logItem2 = new LogItem("Checking for existing portal listings list", null, null, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem2);
					try
					{
						sPList = this.CreateNewListOrRetrieveExisting(sourceWeb, targetWeb, base.SharePointOptions.OverwriteLists, portalListings.Count > 0, logItem2);
					}
					catch (Exception exception1)
					{
						logItem2.Exception = exception1;
						base.FireOperationFinished(logItem2);
						return;
					}
					logItem2.Status = ActionOperationStatus.Completed;
					base.FireOperationFinished(logItem2);
					if (sPList != null)
					{
						LogItem logItem3 = new LogItem("Fetching existing target items", null, null, sPList.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem3);
						try
						{
							items = sPList.Items;
							this.FetchTargetListTerseData(items);
						}
						catch (Exception exception2)
						{
							logItem3.Exception = exception2;
							base.FireOperationFinished(logItem3);
							return;
						}
						logItem3.Status = ActionOperationStatus.Completed;
						base.FireOperationFinished(logItem3);
						foreach (SPPortalListing portalListing in portalListings)
						{
							Guid listingID = portalListing.ListingID;
							LogItem objectTypeSize = new LogItem("Checking for existing target", listingID.ToString(), sourceWeb.DisplayUrl, sPList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(objectTypeSize);
							try
							{
								SPListItem sPListItem = this.FindTargetItem(portalListing, items);
								if (!(sPListItem != null) || base.SharePointOptions.ItemCopyingMode == ListItemCopyMode.Overwrite || base.SharePointOptions.UpdateItems && this.CompareDatesIgnoreMilliseconds(portalListing.Modified, sPListItem.Modified) > 0)
								{
									objectTypeSize.Operation = "Fetching listing data";
									base.FireOperationUpdated(objectTypeSize);
									portalListing.FetchFullData(true);
									if (!portalListing.HasOwnContent || portalListing.GetHasProperty("HtmlBlob"))
									{
										if (sPListItem != null)
										{
											if (base.SharePointOptions.ItemCopyingMode != ListItemCopyMode.Overwrite)
											{
												string item = this.PreparePortalListingForUpgradeToItem(portalListing, new int?(sPListItem.ID), sourceWeb, targetWeb, ref sPList1, objectTypeSize);
												objectTypeSize.Operation = "Updating existing listing";
												base.FireOperationUpdated(objectTypeSize);
												sPListItem.UpdateListItem(item, null, null);
												objectTypeSize.LicenseDataUsed = SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
											}
											else
											{
												objectTypeSize.Operation = "Deleting existing listing";
												base.FireOperationUpdated(objectTypeSize);
												items.DeleteItem(sPListItem);
												sPListItem = null;
											}
										}
										if (sPListItem == null)
										{
											int? nullable = null;
											string str = this.PreparePortalListingForUpgradeToItem(portalListing, nullable, sourceWeb, targetWeb, ref sPList1, objectTypeSize);
											objectTypeSize.Operation = "Adding new listing";
											base.FireOperationUpdated(objectTypeSize);
											AddListItemOptions addListItemOption = new AddListItemOptions()
											{
												Overwrite = true,
												PreserveID = false
											};
											items.AddItem("", str, null, null, addListItemOption);
											objectTypeSize.LicenseDataUsed = SPObjectSizes.GetObjectTypeSize(typeof(SPListItem));
										}
										objectTypeSize.Status = ActionOperationStatus.Completed;
									}
									else
									{
										objectTypeSize.Operation = "Listing Skipped";
										objectTypeSize.Details = "The listing was skipped because it has it's own html content which could not be retrieved. This normally occurs because the Native Web Services adapter does not support these types of listings.";
										objectTypeSize.Status = ActionOperationStatus.Skipped;
										base.FireOperationUpdated(objectTypeSize);
										continue;
									}
								}
								else
								{
									objectTypeSize.Operation = "Listing Skipped";
									objectTypeSize.Details = "This listing already exists on the target.";
									objectTypeSize.Status = ActionOperationStatus.Skipped;
									base.FireOperationFinished(objectTypeSize);
									continue;
								}
							}
							catch (Exception exception3)
							{
								objectTypeSize.Exception = exception3;
							}
							base.FireOperationFinished(objectTypeSize);
						}
						if (!base.SharePointOptions.OverwriteLists && base.SharePointOptions.PropagateItemDeletions)
						{
							LogItem logItem4 = new LogItem("Checking for listing deletions", null, sourceWeb.DisplayUrl, sPList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem4);
							List<SPListItem> sPListItems = new List<SPListItem>();
							List<SPListItem> sPListItems1 = new List<SPListItem>();
							try
							{
								foreach (SPListItem item1 in items)
								{
									if (this.FindSourceItem(item1, portalListings) != null)
									{
										continue;
									}
									sPListItems.Add(item1);
								}
								if (sPList1 == null)
								{
									sPList1 = this.CreateNewDataLibraryOrRetrieveExisting(targetWeb, false, false, null);
									if (sPList1 != null)
									{
										sPList1.Items.FetchTerseData(true, ListItemQueryType.ListItem, null, new GetListItemOptions());
									}
								}
								if (sPList1 != null)
								{
									foreach (SPListItem sPListItem1 in sPListItems)
									{
										string str1 = sPListItem1["ListingID"];
										if (string.IsNullOrEmpty(str1))
										{
											continue;
										}
										char[] chrArray = new char[] { '{', '}' };
										str1 = string.Concat(str1.Trim(chrArray), ".aspx");
										foreach (SPListItem item2 in sPList1.Items)
										{
											if (!str1.Equals(item2.FileLeafRef, StringComparison.OrdinalIgnoreCase))
											{
												continue;
											}
											sPListItems1.Add(item2);
											break;
										}
									}
								}
								logItem4.Status = ActionOperationStatus.Completed;
								base.FireOperationFinished(logItem4);
							}
							catch (Exception exception4)
							{
								logItem4.Exception = exception4;
								base.FireOperationFinished(logItem4);
							}
							if (logItem4.Status == ActionOperationStatus.Completed && sPListItems.Count > 0)
							{
								LogItem logItem5 = new LogItem("Propagating deletions", string.Concat(sPListItems.Count, " item", (sPListItems.Count > 1 ? "s" : "")), "", sPList.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem5);
								try
								{
									items.DeleteItems(sPListItems.ToArray());
									if (sPListItems1.Count > 0)
									{
										sPList1.Items.DeleteItems(sPListItems1.ToArray());
									}
									logItem5.Status = ActionOperationStatus.Completed;
									base.FireOperationFinished(logItem5);
								}
								catch (Exception exception5)
								{
									logItem5.Exception = exception5;
									base.FireOperationFinished(logItem5);
								}
							}
						}
					}
				}
			}
			finally
			{
				if (sPList != null)
				{
					sPList.Dispose();
				}
				if (sPList1 != null)
				{
					sPList1.Dispose();
				}
			}
		}

		private SPList CreateNewDataLibraryOrRetrieveExisting(SPWeb targetWeb, bool bOverwrite, bool bCreateIfNotExists, LogItem dataLibraryOperation)
		{
			SPList sPList = null;
			foreach (SPList list in targetWeb.Lists)
			{
				if (!list.Name.Equals("portallistingsdata", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				sPList = list;
				break;
			}
			if (sPList != null)
			{
				if (sPList.BaseTemplate != ListTemplateType.DocumentLibrary)
				{
					throw new Exception("The portal listing upgrade location for listings with their own content is currently occupied by another list.");
				}
				if (bOverwrite && bCreateIfNotExists)
				{
					if (dataLibraryOperation != null)
					{
						dataLibraryOperation.Operation = "Deleting existing data library";
						base.FireOperationUpdated(dataLibraryOperation);
					}
					sPList.Delete();
					sPList = null;
				}
			}
			if (sPList == null && bCreateIfNotExists)
			{
				if (dataLibraryOperation != null)
				{
					dataLibraryOperation.Operation = "Creating new portal listing data library";
					base.FireOperationUpdated(dataLibraryOperation);
				}
				StringBuilder stringBuilder = new StringBuilder();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
				xmlTextWriter.WriteStartElement("List");
				xmlTextWriter.WriteAttributeString("Name", "PortalListingsData");
				xmlTextWriter.WriteAttributeString("Title", "Portal Listings Data");
				xmlTextWriter.WriteAttributeString("Description", "A library that stores the data of portal listings that have their own content.");
				xmlTextWriter.WriteAttributeString("BaseTemplate", "101");
				xmlTextWriter.WriteAttributeString("OnQuickLaunch", "False");
				xmlTextWriter.WriteAttributeString("EnableModeration", "False");
				xmlTextWriter.WriteAttributeString("EnableVersioning", "False");
				xmlTextWriter.WriteAttributeString("EnableMinorVersions", "False");
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Flush();
				xmlTextWriter.Close();
				string str = stringBuilder.ToString();
				sPList = targetWeb.Lists.AddList(str, new AddListOptions());
				dataLibraryOperation.LicenseDataUsed = SPObjectSizes.GetObjectTypeSize(typeof(SPList));
			}
			return sPList;
		}

		private SPList CreateNewListOrRetrieveExisting(SPWeb sourceWeb, SPWeb targetWeb, bool bOverwrite, bool bCreateIfNotExists, LogItem operation)
		{
			SPList sPList = null;
			foreach (SPList list in targetWeb.Lists)
			{
				if (!list.Name.Equals("portallistings", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				sPList = list;
				break;
			}
			if (sPList != null)
			{
				if (!this.EnsurePortalListingSchema(sPList))
				{
					throw new Exception("The portal listing upgrade location is currently occupied by another list.");
				}
				if (bOverwrite && bCreateIfNotExists)
				{
					operation.Operation = "Deleting existing list";
					base.FireOperationUpdated(operation);
					sPList.Delete();
					sPList = null;
				}
			}
			if (sPList == null && bCreateIfNotExists)
			{
				operation.Operation = "Creating new portal listings list";
				base.FireOperationUpdated(operation);
				XmlNode portalListingListXML = this.GetPortalListingListXML(sourceWeb);
				AddListOptions addListOption = new AddListOptions()
				{
					DeletePreExistingViews = true
				};
				sPList = targetWeb.Lists.AddList(portalListingListXML.OuterXml, addListOption);
				operation.LicenseDataUsed = SPObjectSizes.GetObjectTypeSize(typeof(SPList));
			}
			return sPList;
		}

		private bool EnsurePortalListingSchema(SPList list)
		{
			if (list.BaseTemplate != ListTemplateType.Links)
			{
				return false;
			}
			bool flag = false;
			foreach (SPField field in list.Fields)
			{
				if (field.Name != "ListingID")
				{
					continue;
				}
				flag = true;
				break;
			}
			return flag;
		}

		private void FetchTargetListTerseData(SPListItemCollection targetCollection)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			stringBuilder.Append("<Fields>");
			stringBuilder.Append("<Field Name=\"ID\" ColName=\"tp_ID\" Type=\"Counter\"/><Field ColName=\"tp_Modified\" Name=\"Modified\" Type=\"DateTime\"/>");
			foreach (SPField field in targetCollection.ParentList.Fields)
			{
				if (field.Name != "ListingID")
				{
					continue;
				}
				stringBuilder.Append(field.FieldXML.OuterXml);
			}
			stringBuilder.Append("</Fields>");
			targetCollection.FetchTerseData(true, ListItemQueryType.ListItem, stringBuilder.ToString(), new GetListItemOptions());
		}

		private SPPortalListing FindSourceItem(SPListItem targetItem, SPPortalListingCollection listingCollection)
		{
			string item = targetItem["ListingID"];
			if (string.IsNullOrEmpty(item))
			{
				return null;
			}
			return listingCollection[new Guid(item)];
		}

		private SPListItem FindTargetItem(SPPortalListing sourceListing, SPListItemCollection targetCollection)
		{
			SPListItem sPListItem;
			string str = sourceListing.ListingID.ToString();
			using (IEnumerator<Node> enumerator = targetCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SPListItem current = (SPListItem)enumerator.Current;
					string item = current["ListingID"];
					if (string.IsNullOrEmpty(item))
					{
						continue;
					}
					char[] chrArray = new char[] { '{', '}' };
					if (!item.Trim(chrArray).Equals(str, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					sPListItem = current;
					return sPListItem;
				}
				return null;
			}
			return sPListItem;
		}

		private XmlNode GetPortalListingListXML(SPWeb sourceWeb)
		{
			byte[] resource = MigrationResourceManager.GetResource(MigrationResourceManager.PortalListingListKey);
			string str = Encoding.UTF8.GetString(resource);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (sourceWeb.PortalListings.Groups.Count > 0)
			{
				XmlNode xmlNodes = documentElement.SelectSingleNode(".//Fields/Field[@Name=\"Group\"]/CHOICES");
				if (xmlNodes == null)
				{
					throw new Exception("Failed to locate group field choice node in portal listing list template.");
				}
				XmlNode name = documentElement.SelectSingleNode(".//Fields/Field[@Name=\"Group\"]/Default");
				if (name == null)
				{
					throw new Exception("Failed to locate group field default node in portal listing list template.");
				}
				XmlNode xmlNodes1 = documentElement.SelectSingleNode(".//Fields/Field[@Name=\"Highlight\"]/Formula");
				if (xmlNodes1 == null)
				{
					throw new Exception("Failed to locate highlight field formula node in portal listing list template.");
				}
				xmlNodes.RemoveAll();
				foreach (SPPortalListingGroup group in sourceWeb.PortalListings.Groups)
				{
					XmlNode name1 = xmlNodes.OwnerDocument.CreateElement("CHOICE");
					name1.InnerText = group.Name;
					xmlNodes.AppendChild(name1);
					if (group.ID == 0)
					{
						name.InnerText = group.Name;
					}
					if (group.Order != 0)
					{
						continue;
					}
					xmlNodes1.InnerText = string.Concat("=(Group=\"", group.Name, "\")");
				}
			}
			return documentElement;
		}

		private string PreparePortalListingForUpgradeToItem(SPPortalListing listing, int? iItemID, SPWeb sourceWeb, SPWeb targetWeb, ref SPList portalListingDataLib, LogItem portalListingOperation)
		{
			string asListItemXml = listing.GetAsListItemXml(iItemID);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(asListItemXml);
			XmlNode documentElement = xmlDocument.DocumentElement;
			XmlAttribute itemOf = documentElement.Attributes["URL"];
			if (itemOf != null)
			{
				itemOf.Value = base.LinkCorrector.CorrectDelimitedString(itemOf.Value, ",");
			}
			XmlAttribute xmlAttribute = documentElement.Attributes["Image"];
			if (xmlAttribute != null)
			{
				xmlAttribute.Value = base.LinkCorrector.CorrectDelimitedString(xmlAttribute.Value, ",");
			}
			XmlAttribute itemOf1 = documentElement.Attributes["Icon"];
			if (itemOf1 != null)
			{
				itemOf1.Value = base.LinkCorrector.CorrectDelimitedString(itemOf1.Value, ",");
			}
			XmlAttribute xmlAttribute1 = documentElement.Attributes["PublishingStartDate"];
			if (xmlAttribute1 != null)
			{
				xmlAttribute1.Value = this.ShiftDateTime(xmlAttribute1.Value, targetWeb);
			}
			XmlAttribute itemOf2 = documentElement.Attributes["PublishingExpirationDate"];
			if (itemOf2 != null)
			{
				itemOf2.Value = this.ShiftDateTime(itemOf2.Value, targetWeb);
			}
			List<SPUser> sPUsers = new List<SPUser>(3);
			XmlAttribute xmlAttribute2 = documentElement.Attributes["Author"];
			if (xmlAttribute2 != null)
			{
				SPUser item = sourceWeb.SiteUsers[xmlAttribute2.Value] as SPUser;
				if (item != null)
				{
					sPUsers.Add(item);
				}
			}
			XmlAttribute itemOf3 = documentElement.Attributes["Editor"];
			if (itemOf3 != null)
			{
				SPUser sPUser = sourceWeb.SiteUsers[itemOf3.Value] as SPUser;
				if (sPUser != null)
				{
					sPUsers.Add(sPUser);
				}
			}
			SPUser sPUser1 = null;
			if (listing.IsPersonListing)
			{
				portalListingOperation.Operation = "Checking for user mappings";
				base.FireOperationUpdated(portalListingOperation);
				sPUser1 = null;
				string str = listing["PersonSID"];
				foreach (SPUser siteUser in (IEnumerable<SecurityPrincipal>)sourceWeb.SiteUsers)
				{
					if (!siteUser.SID.Equals(str, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					sPUser1 = siteUser;
					break;
				}
				if (sPUser1 != null)
				{
					sPUsers.Add(sPUser1);
				}
			}
			base.EnsurePrincipalExistence(sPUsers.ToArray(), null, targetWeb, null, null);
			if (xmlAttribute2 != null)
			{
				xmlAttribute2.Value = base.MapPrincipal(xmlAttribute2.Value);
			}
			if (itemOf3 != null)
			{
				itemOf3.Value = base.MapPrincipal(itemOf3.Value);
			}
			if (sPUser1 != null)
			{
				string loginName = sPUser1.LoginName;
				string str1 = base.MapPrincipal(loginName);
				if (!loginName.Equals(str1, StringComparison.OrdinalIgnoreCase))
				{
					SPUser item1 = targetWeb.SiteUsers[str1] as SPUser;
					if (item1 != null)
					{
						string str2 = string.Concat("mailto:", item1.Email, ", ", (string.IsNullOrEmpty(item1.Name) ? item1.LoginName : item1.Name));
						documentElement.Attributes["URL"].Value = str2;
					}
				}
			}
			else if (listing.HasOwnContent)
			{
				string str3 = targetWeb.ServerRelativeUrl.Trim(new char[] { '/' });
				XmlAttribute xmlAttribute3 = documentElement.Attributes["URL"];
				string[] lower = new string[] { (string.IsNullOrEmpty(str3) ? "" : string.Concat("/", str3)), "/PortalListingsData/", null, null, null };
				Guid listingID = listing.ListingID;
				lower[2] = listingID.ToString().ToLower();
				lower[3] = ".aspx, ";
				lower[4] = listing["Title"];
				xmlAttribute3.Value = string.Concat(lower);
				this.AddListingHtmlBlobToDataLibrary(listing, (documentElement.Attributes["Image"] == null ? "" : documentElement.Attributes["Image"].Value), targetWeb, ref portalListingDataLib);
			}
			return documentElement.OuterXml;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, base.SharePointOptions.ForceRefresh);
			base.LinkCorrector.Scope = LinkCorrectionScope.MigrationOnly;
			using (SPWeb item = source[0] as SPWeb)
			{
				foreach (SPWeb sPWeb in target)
				{
					try
					{
						base.LinkCorrector.ClearMappings();
						base.LinkCorrector.PopulateForSiteContentCopy(item, sPWeb, base.SharePointOptions.TaskCollection, true);
						this.CopyPortalListings(item, sPWeb);
					}
					finally
					{
						sPWeb.Dispose();
					}
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.CopyPortalListings(oParams[0] as SPWeb, oParams[1] as SPWeb);
		}

		private string ShiftDateTime(string sDateTime, SPWeb targetWeb)
		{
			if (string.IsNullOrEmpty(sDateTime))
			{
				return sDateTime;
			}
			DateTime dateTime = Utils.ParseDateAsUtc(sDateTime);
			DateTime localTime = targetWeb.TimeZone.UtcToLocalTime(dateTime);
			DateTime dateTime1 = dateTime.Subtract(localTime.Subtract(dateTime));
			return Utils.FormatDate(Utils.MakeTrueUTCDateTime(dateTime1));
		}
	}
}