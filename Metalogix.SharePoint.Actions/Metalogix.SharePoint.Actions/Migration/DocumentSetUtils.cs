using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	public static class DocumentSetUtils
	{
		public static void CopyXVersionsInSnapShot(PasteListItemAction action, XmlNode snapShotCollectionNode)
		{
			if (action.SharePointOptions.CopyMaxVersions)
			{
				XmlNodeList xmlNodeLists = snapShotCollectionNode.SelectNodes("//Snapshot");
				if (xmlNodeLists != null)
				{
					int maximumVersionCount = action.SharePointOptions.MaximumVersionCount;
					int num = 0;
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						num++;
						if (num <= maximumVersionCount || xmlNodes.ParentNode == null)
						{
							continue;
						}
						xmlNodes.ParentNode.RemoveChild(xmlNodes);
					}
				}
			}
		}

		public static string GetSnapShotPropertyFromMetaInfo(string metaInfoProperty)
		{
			int num = metaInfoProperty.IndexOf("snapshots:SW|", StringComparison.InvariantCultureIgnoreCase);
			string empty = string.Empty;
			if (num > -1)
			{
				int num1 = metaInfoProperty.IndexOf("</SnapshotCollection>", StringComparison.InvariantCultureIgnoreCase);
				if (num1 > -1)
				{
					int length = metaInfoProperty.Substring(num + 13, num1 - num + 8).Length;
					empty = metaInfoProperty.Substring(num + 13, length);
				}
			}
			return empty;
		}

		public static bool IsDocSetVersionHistoryMigrationSupported(PasteListItemAction action, SPList sourceList, SPFolder targetFolder, SPListItem sourceItem)
		{
			bool flag = (!sourceList.Adapter.SharePointVersion.IsSharePoint2010OrLater || sourceList.Adapter.IsNws || sourceList.Adapter.IsDB ? false : !sourceList.Adapter.IsClientOM);
			bool flag1 = (!targetFolder.Adapter.SharePointVersion.IsSharePoint2013OrLater ? false : targetFolder.Adapter.IsClientOM);
			bool flag2 = (!sourceList.IsDocumentLibrary ? false : sourceItem.IsDocumentSet);
			if (action.SharePointOptions.CopyVersions && flag && flag1)
			{
				return flag2;
			}
			return false;
		}

		public static void UpdateFieldsInSnapShot(PasteListItemAction action, SPList sourceList, SPListItemCollection targetItems, XmlNode snapShotCollectionNode)
		{
			XmlNodeList xmlNodeLists = snapShotCollectionNode.SelectNodes("//Snapshot/Fields/Field");
			if (xmlNodeLists != null)
			{
				foreach (XmlNode str in xmlNodeLists)
				{
					string attributeValueAsString = str.GetAttributeValueAsString("Id");
					if (string.IsNullOrEmpty(attributeValueAsString))
					{
						continue;
					}
					SPField targetField = PasteActionUtils.GetTargetField(action, sourceList, targetItems.ParentSPList, attributeValueAsString);
					if (targetField == null || str.Attributes == null)
					{
						str.ParentNode.RemoveChild(str);
					}
					else
					{
						str.Attributes["Id"].Value = targetField.ID.ToString();
						string innerText = str.InnerText;
						if (string.IsNullOrEmpty(innerText))
						{
							continue;
						}
						if (targetField.IsUserField || targetField.IsLookupField || targetField.IsTaxonomyField)
						{
							str.ParentNode.RemoveChild(str);
						}
						else
						{
							if (!targetField.IsUrlField)
							{
								continue;
							}
							char[] chrArray = new char[] { ',' };
							string[] strArrays = innerText.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
							if ((int)strArrays.Length <= 1)
							{
								str.InnerText = action.LinkCorrector.CorrectUrl(innerText);
							}
							else
							{
								for (int i = 0; i < (int)strArrays.Length; i++)
								{
									strArrays[i] = action.LinkCorrector.CorrectUrl(strArrays[i].Trim());
								}
								str.InnerText = string.Join(", ", strArrays);
							}
						}
					}
				}
			}
		}

		public static void UpdateItemsInSnapShot(PasteListItemAction action, SPList sourceList, SPListItemCollection targetItems, SPListItem targetListItem, XmlNode snapShotCollectionNode)
		{
			XmlNodeList xmlNodeLists = snapShotCollectionNode.SelectNodes("//Items/Item");
			if (xmlNodeLists != null)
			{
				foreach (XmlNode str in xmlNodeLists)
				{
					string attributeValueAsString = str.GetAttributeValueAsString("Url");
					string attributeValueAsString1 = str.GetAttributeValueAsString("Guid");
					Guid empty = Guid.Empty;
					if (action.GuidMappings.ContainsKey(new Guid(attributeValueAsString1)))
					{
						empty = action.GuidMappings[new Guid(attributeValueAsString1)];
					}
					SPListItem itemByGuid = null;
					if (empty != Guid.Empty)
					{
						itemByGuid = targetItems.GetItemByGuid(empty);
					}
					if (itemByGuid == null)
					{
						string name = targetListItem.Name;
						string fileName = attributeValueAsString;
						int num = attributeValueAsString.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
						if (num > -1)
						{
							string str1 = attributeValueAsString.Substring(0, num);
							fileName = Path.GetFileName(attributeValueAsString);
							name = string.Format("{0}/{1}", name, str1);
						}
						itemByGuid = targetItems.GetItemByFileNameAndDir(fileName, name);
					}
					if (!(itemByGuid != null) || str.Attributes == null)
					{
						string attributeValueAsString2 = str.GetAttributeValueAsString("Id");
						XmlNodeList xmlNodeLists1 = snapShotCollectionNode.SelectNodes("//Snapshot/SnapshotItems/SnapshotItem");
						if (str.ParentNode == null || xmlNodeLists1 == null)
						{
							continue;
						}
						foreach (XmlNode xmlNodes in xmlNodeLists1.Cast<XmlNode>().Where<XmlNode>((XmlNode snapShotIndividualItemNode) => {
							if (snapShotIndividualItemNode.ParentNode == null)
							{
								return false;
							}
							return snapShotIndividualItemNode.GetAttributeValueAsString("Id").Equals(attributeValueAsString2, StringComparison.InvariantCultureIgnoreCase);
						}))
						{
							if (xmlNodes.ParentNode == null)
							{
								continue;
							}
							xmlNodes.ParentNode.RemoveChild(xmlNodes);
						}
						str.ParentNode.RemoveChild(str);
					}
					else
					{
						str.Attributes["Guid"].Value = itemByGuid.UniqueId.ToString();
					}
				}
			}
		}

		public static string UpdateMappingsInSnapShot(PasteListItemAction action, SPList sourceList, SPListItemCollection targetItems, SPListItem targetItem, string snapShot)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(snapShot);
			if (xmlNode == null)
			{
				return string.Empty;
			}
			DocumentSetUtils.CopyXVersionsInSnapShot(action, xmlNode);
			DocumentSetUtils.UpdateUserInSnapShot(action, targetItem, xmlNode);
			DocumentSetUtils.UpdateItemsInSnapShot(action, sourceList, targetItems, targetItem, xmlNode);
			DocumentSetUtils.UpdateFieldsInSnapShot(action, sourceList, targetItems, xmlNode);
			return xmlNode.OuterXml;
		}

		public static string UpdateTargetMetaInfoProperty(SPListItem targetItem, string updatedSnapShot)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(targetItem.XML);
			if (xmlNode != null)
			{
				string attributeValueAsString = xmlNode.GetAttributeValueAsString("MetaInfo");
				if (!string.IsNullOrEmpty(attributeValueAsString))
				{
					string snapShotPropertyFromMetaInfo = DocumentSetUtils.GetSnapShotPropertyFromMetaInfo(attributeValueAsString);
					if (!string.IsNullOrEmpty(snapShotPropertyFromMetaInfo))
					{
						return attributeValueAsString.Replace(snapShotPropertyFromMetaInfo, updatedSnapShot);
					}
					if (xmlNode.Attributes != null)
					{
						return string.Format("{0}snapshots:SW|{1}", attributeValueAsString, updatedSnapShot);
					}
				}
			}
			return string.Empty;
		}

		public static void UpdateUserInSnapShot(PasteListItemAction action, SPListItem targetItem, XmlNode snapShotCollectionNode)
		{
			XmlNodeList xmlNodeLists = snapShotCollectionNode.SelectNodes("//Snapshot");
			if (xmlNodeLists != null)
			{
				SPUserCollection siteUsers = targetItem.ParentList.ParentWeb.SiteUsers;
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					string attributeValueAsString = xmlNodes.GetAttributeValueAsString("By");
					string str = SPGlobalMappings.Map(Regex.Unescape(attributeValueAsString), targetItem.Adapter.SharePointVersion.IsSharePointOnline);
					if (!siteUsers.Contains(str) && action.SharePointOptions.MapMissingUsers)
					{
						string mapMissingUsersToLoginName = action.SharePointOptions.MapMissingUsersToLoginName;
						if (!string.IsNullOrEmpty(mapMissingUsersToLoginName) && siteUsers.Contains(mapMissingUsersToLoginName))
						{
							str = mapMissingUsersToLoginName;
						}
					}
					if (xmlNodes.Attributes == null)
					{
						continue;
					}
					xmlNodes.Attributes["By"].Value = (str.Contains("\\") ? str.Replace("\\", "\\\\") : str);
				}
			}
		}
	}
}