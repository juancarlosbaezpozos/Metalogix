using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions
{
	public static class MigrationUtils
	{
		public static object ChangeItemOptionsToUpdate(PasteListItemOptions listItemOptions, bool isFolder)
		{
			if (listItemOptions.MigrationMode == MigrationMode.Full || listItemOptions.MigrationMode == MigrationMode.Custom && listItemOptions.ItemCopyingMode == ListItemCopyMode.Overwrite)
			{
				listItemOptions.CheckModifiedDatesForItemsDocuments = false;
				listItemOptions.UpdateItems = true;
			}
			listItemOptions.MigrationMode = MigrationMode.Incremental;
			listItemOptions.ItemCopyingMode = ListItemCopyMode.Preserve;
			listItemOptions.UpdateItemOptionsBitField = 2147483647;
			if (isFolder)
			{
				PasteFolderOptions pasteFolderOption = listItemOptions as PasteFolderOptions;
				if (pasteFolderOption != null)
				{
					pasteFolderOption.UpdateFolderOptionsBitField = 2147483647;
					return pasteFolderOption;
				}
			}
			return listItemOptions;
		}

		public static LogItem GetLogItemDetails(LogItem sourceOperation, string result)
		{
			LogItem getMessageOfFirstErrorElement = sourceOperation;
			OperationReportingResult operationReportingResult = new OperationReportingResult(result);
			if (operationReportingResult.ErrorOccured)
			{
				getMessageOfFirstErrorElement.Status = ActionOperationStatus.Failed;
				getMessageOfFirstErrorElement.Information = operationReportingResult.GetMessageOfFirstErrorElement;
				getMessageOfFirstErrorElement.Details = operationReportingResult.AllReportElementsAsString;
			}
			else if (!operationReportingResult.WarningOccured)
			{
				if (operationReportingResult.HasInformation)
				{
					getMessageOfFirstErrorElement.Details = operationReportingResult.AllReportElementsAsString;
				}
				getMessageOfFirstErrorElement.Status = ActionOperationStatus.Completed;
			}
			else
			{
				getMessageOfFirstErrorElement.Status = ActionOperationStatus.Warning;
				getMessageOfFirstErrorElement.Information = "Please review details";
				getMessageOfFirstErrorElement.Details = operationReportingResult.Warnings.ToString();
			}
			return getMessageOfFirstErrorElement;
		}

		public static SPList GetMatchingList(SPList sourceList, SPWeb targetWeb)
		{
			return MigrationUtils.GetMatchingList(sourceList, targetWeb, null);
		}

		public static SPList GetMatchingList(SPList list, SPWeb web, TransformationTaskCollection transformCollection)
		{
			string str;
			SPList sPList;
			if (list == null)
			{
				return null;
			}
			if (list.BaseTemplate == ListTemplateType.O12Pages)
			{
				using (IEnumerator<Node> enumerator = web.Lists.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SPList current = (SPList)enumerator.Current;
						if (current.BaseTemplate != ListTemplateType.O12Pages)
						{
							continue;
						}
						sPList = current;
						return sPList;
					}
					str = (transformCollection == null ? list.Name : MigrationUtils.GetRenamedListName(list, web, transformCollection));
					if (string.IsNullOrEmpty(str))
					{
						return null;
					}
					return web.Lists[str];
				}
				return sPList;
			}
			str = (transformCollection == null ? list.Name : MigrationUtils.GetRenamedListName(list, web, transformCollection));
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			return web.Lists[str];
		}

		public static SPWeb GetMatchingWeb(SPWeb sourceWeb, SPWeb targetParentWeb)
		{
			return MigrationUtils.GetMatchingWeb(sourceWeb, targetParentWeb, null);
		}

		public static SPWeb GetMatchingWeb(SPWeb sourceWeb, SPWeb targetParentWeb, TransformationTaskCollection transformations)
		{
			if (sourceWeb == null || targetParentWeb == null)
			{
				return null;
			}
			string name = sourceWeb.Name;
			if (transformations != null)
			{
				TransformationTask task = transformations.GetTask(sourceWeb, new CompareDatesInUtc());
				if (task != null && task.ChangeOperations.ContainsKey("Name"))
				{
					name = task.ChangeOperations["Name"];
				}
			}
			return (SPWeb)targetParentWeb.SubWebs[name];
		}

		public static string GetRenamedFolderName(SPFolder sourceFolder, TransformationTaskCollection transformCollection)
		{
			string name = sourceFolder.Name;
			if (transformCollection != null)
			{
				TransformationTask task = transformCollection.GetTask(sourceFolder, new CompareDatesInUtc());
				if (task != null)
				{
					string str = task.PerformTransformation(string.Concat("<Folder FileLeafRef=\"", sourceFolder.Name, "\" />"));
					XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
					if (xmlNode.Attributes["FileLeafRef"] != null)
					{
						name = xmlNode.Attributes["FileLeafRef"].Value;
					}
				}
			}
			return name;
		}

		public static string GetRenamedListName(SPList sourceList, SPWeb targetWeb, TransformationTaskCollection transformCollection)
		{
			string value;
			string name = sourceList.Name;
			string str = string.Concat("<List Name=\"", sourceList.Name, "\" />");
			if (transformCollection != null || targetWeb.Template != null && string.Equals(targetWeb.Template.Name, "ENTERWIKI#0", StringComparison.OrdinalIgnoreCase) && sourceList.Name == "Wiki Pages")
			{
				string str1 = MigrationUtils.RenameListInXml(str, sourceList, targetWeb, transformCollection);
				XmlNode xmlNode = XmlUtility.StringToXmlNode(str1);
				if (xmlNode == null || xmlNode.Attributes["Name"] == null)
				{
					value = null;
				}
				else
				{
					value = xmlNode.Attributes["Name"].Value;
				}
				name = value;
			}
			return name;
		}

		public static bool IsListWithDefaultItems(SPList targetList)
		{
			if (targetList.Adapter.IsClientOM)
			{
				int num = 9;
				int num1 = 62;
				bool d = targetList.ParentWeb.Template.ID == num;
				bool flag = targetList.ParentWeb.Template.ID == num1;
				if (d || flag)
				{
					ListTemplateType baseTemplate = targetList.BaseTemplate;
					if (baseTemplate == ListTemplateType.BlogCategories || baseTemplate == ListTemplateType.BlogPosts || baseTemplate == ListTemplateType.CommunityCategories || baseTemplate == ListTemplateType.CommunityMembers)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsSource2007PublishingImageLibrary(SPListItemCollection sources)
		{
			SPList parentSPList = sources.ParentSPList;
			if (!parentSPList.Adapter.SharePointVersion.IsSharePoint2007 || !parentSPList.Name.Equals("PublishingImages", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return parentSPList.ParentWeb.HasPublishingFeature;
		}

		public static bool IsTargetAssetLibraryOf2013OrLater(SPFolder targetFolder)
		{
			if (targetFolder.ParentList.BaseTemplate != ListTemplateType.AssetLibrary)
			{
				return false;
			}
			return targetFolder.Adapter.SharePointVersion.IsSharePoint2013OrLater;
		}

		public static bool IsVideoFile(string fileType)
		{
			if (string.IsNullOrEmpty(fileType))
			{
				return false;
			}
			List<string> strs = new List<string>()
			{
				"3gp",
				"3gpp",
				"3g2",
				"3gp2",
				"asf",
				"mts",
				"m2ts",
				"avi",
				"mod",
				"ts",
				"ps",
				"vob",
				"xesc",
				"mp4",
				"m4a",
				"m4v",
				"mpeg",
				"mpg",
				"m2v",
				"isma",
				"ismv",
				"wmv",
				"flv",
				"mxf",
				"gxf",
				"dvr-ms",
				"mkv",
				"wav",
				"mov"
			};
			return strs.Contains(fileType);
		}

		public static bool ListIsBeingPreserved(SPList sourceList, SPWeb targetWeb, PasteListOptions listOptions, out SPList existingTargetList)
		{
			TransformationTaskCollection taskCollection;
			SPList sPList = sourceList;
			SPWeb sPWeb = targetWeb;
			if (listOptions.RenameSpecificNodes)
			{
				taskCollection = listOptions.TaskCollection;
			}
			else
			{
				taskCollection = null;
			}
			existingTargetList = MigrationUtils.GetMatchingList(sPList, sPWeb, taskCollection);
			bool updateListOptionsBitField = listOptions.UpdateListOptionsBitField > 0;
			if (!listOptions.OverwriteLists && existingTargetList != null && !updateListOptionsBitField)
			{
				return true;
			}
			return false;
		}

		public static string MapPrincipals(IDictionary<string, string> principalMappings, string sourcePrincipalNames)
		{
			if (string.IsNullOrEmpty(sourcePrincipalNames))
			{
				return sourcePrincipalNames;
			}
			char[] chrArray = new char[] { ',' };
			string[] strArrays = sourcePrincipalNames.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				string str1 = null;
				if (!principalMappings.TryGetValue(str, out str1))
				{
					str1 = str;
				}
				if (!string.IsNullOrEmpty(str1))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(str1);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ProcessUserFieldsForCopy(XmlNode fieldsNode, SPWeb sourceWeb, SPWeb targetWeb)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string empty = string.Empty;
			try
			{
				foreach (XmlNode d in fieldsNode.SelectNodes("//Field[@Type='User'or @Type='UserMulti' ]"))
				{
					empty = d.GetAttributeValueAsString("DisplayName");
					string attributeValueAsString = d.GetAttributeValueAsString("UserSelectionScope");
					if (string.IsNullOrEmpty(attributeValueAsString))
					{
						continue;
					}
					SPGroup item = null;
					foreach (SPGroup group in (IEnumerable<SecurityPrincipal>)sourceWeb.Groups)
					{
						if (!string.Equals(group.ID, attributeValueAsString, StringComparison.Ordinal))
						{
							continue;
						}
						item = (SPGroup)targetWeb.Groups[group.PrincipalName];
						if (item == null)
						{
							stringBuilder.AppendFormat("Group '{0}' not found at target. Hence field '{1}' may not function properly.", group.PrincipalName, empty);
							stringBuilder.AppendLine();
							break;
						}
						else
						{
							d.Attributes["UserSelectionScope"].Value = item.ID;
							break;
						}
					}
				}
			}
			catch (Exception exception)
			{
				stringBuilder.AppendFormat("Error occurred while processing field '{0}' Error: '{1}'", empty, exception.Message);
			}
			return stringBuilder.ToString();
		}

		public static string RenameListInXml(string sListXml, SPList sourceList, SPWeb targetWeb, TransformationTaskCollection transformCollection)
		{
			string str = sListXml;
			if (transformCollection != null)
			{
				TransformationTask task = transformCollection.GetTask(sourceList, new CompareDatesInUtc());
				if (task != null)
				{
					str = task.PerformTransformation(sListXml);
				}
			}
			if (targetWeb.Template != null && string.Equals(targetWeb.Template.Name, "ENTERWIKI#0", StringComparison.OrdinalIgnoreCase) && sourceList.Name == "Wiki Pages")
			{
				TransformationTask transformationTask = new TransformationTask();
				transformationTask.ChangeOperations.Add("Title", "Pages");
				transformationTask.ChangeOperations.Add("Name", "Pages");
				str = transformationTask.PerformTransformation(str);
			}
			return str;
		}

		public static string UpdateCheckedOutToFieldRefsInViewsXml(string viewsXml)
		{
			if (string.IsNullOrEmpty(viewsXml))
			{
				throw new ArgumentException("Cannot be null or empty.", "viewsXml");
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(viewsXml);
			XmlNodeList xmlNodeLists = xmlNode.SelectNodes("//FieldRef[@Name=\"LinkCheckedOutTitle\"]");
			if (xmlNodeLists.Count > 0)
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					xmlNodes.Attributes["Name"].Value = "CheckoutUser";
				}
				viewsXml = xmlNode.OuterXml;
			}
			return viewsXml;
		}

		public static string UpdateGuidsInFile(string fileContent, LinkCorrector linkCorrector)
		{
			return Regex.Replace(fileContent, "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}", (Match m) => linkCorrector.MapGuid(m.Value) ?? m.Value, RegexOptions.IgnoreCase);
		}
	}
}