using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public abstract class BaseListItemDataUpdater<TAction, TActionOptions> : PreconfiguredTransformer<SPListItem, TAction, SPListItemCollection, SPListItemCollection>
	where TAction : PasteAction<TActionOptions>
	where TActionOptions : PasteListItemOptions
	{
		private const byte TEXT_FIELD_MAX_LENGTH = 255;

		private const byte URL_FIELD_MAX_LENGTH = 255;

		private const int CommunitySiteTemplateId = 62;

		public override string Name
		{
			get
			{
				return "List Item Reference Updating";
			}
		}

		protected BaseListItemDataUpdater()
		{
		}

		public override void BeginTransformation(TAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		private void CopyReferencedUsersForItem(TAction action, SPListItem sourceItem, SPFolder targetFolder)
		{
			try
			{
				action.EnsurePrincipalExistence(sourceItem.GetReferencedPrincipals(), targetFolder.ParentList.ParentWeb);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string name = null;
				string displayUrl = null;
				if (sourceItem.ParentFolder != null && sourceItem.ParentFolder.GetType().IsAssignableFrom(typeof(SPFolder)))
				{
					name = sourceItem.ParentFolder.Name;
					displayUrl = sourceItem.ParentFolder.DisplayUrl;
				}
				else if (sourceItem.ParentList != null)
				{
					name = sourceItem.ParentList.Name;
					displayUrl = sourceItem.ParentList.DisplayUrl;
				}
				LogItem logItem = new LogItem(string.Concat("Could not fetch referenced principals: ", exception.Message), name, displayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = exception
				};
				base.FireOperationStarted(logItem);
			}
		}

		private void CreateAuthorField(XmlNode itemXML)
		{
			if (itemXML.Attributes["MyAuthor"] != null && itemXML.Attributes["Author"] == null)
			{
				XmlAttribute attributeValueAsString = itemXML.OwnerDocument.CreateAttribute("Author");
				attributeValueAsString.Value = itemXML.GetAttributeValueAsString("MyAuthor");
				itemXML.Attributes.Append(attributeValueAsString);
			}
		}

		public override void EndTransformation(TAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		private SPContentType GetContentType(string contentTypeName, string contentTypeId, SPListItemCollection targets)
		{
			SPContentType item = targets.ParentSPList.ParentWeb.ContentTypes[contentTypeId] ?? targets.ParentSPList.ParentWeb.ContentTypes.GetContentTypeByName(contentTypeName);
			return item;
		}

		private static void MapItemUserFieldData(IDictionary<string, string> principalMappings, SPListItem sourceItem, XmlNode xml)
		{
			SPFieldCollection fields = (SPFieldCollection)sourceItem.ParentList.Fields;
			string[] strArrays = new string[] { "User", "UserMulti" };
			BaseListItemDataUpdater<TAction, TActionOptions>.MapUserFields(principalMappings, xml, fields.GetFieldsOfTypes(strArrays));
		}

		public static void MapUserFields(IDictionary<string, string> principalMappings, XmlNode itemXML, SPFieldCollection userFields)
		{
			bool flag = false;
			bool flag1 = false;
			foreach (SPField userField in userFields)
			{
				if (userField.Name == "Created_x0020_By")
				{
					flag = true;
				}
				if (userField.Name == "Modified_x0020_By")
				{
					flag1 = true;
				}
				if (itemXML.Attributes[userField.Name] == null)
				{
					continue;
				}
				itemXML.Attributes[userField.Name].Value = MigrationUtils.MapPrincipals(principalMappings, itemXML.Attributes[userField.Name].Value);
			}
			if (!flag && itemXML.Attributes["Created_x0020_By"] != null)
			{
				itemXML.Attributes["Created_x0020_By"].Value = MigrationUtils.MapPrincipals(principalMappings, itemXML.Attributes["Created_x0020_By"].Value);
			}
			if (!flag1 && itemXML.Attributes["Modified_x0020_By"] != null)
			{
				itemXML.Attributes["Modified_x0020_By"].Value = MigrationUtils.MapPrincipals(principalMappings, itemXML.Attributes["Modified_x0020_By"].Value);
			}
		}

		public static void ModifyListItemTaskXml(XmlNode ndItemXml)
		{
			if (ndItemXml.Attributes["WorkflowInstanceID"] != null)
			{
				if (ndItemXml.Attributes["WorkflowInstanceIDBackup"] == null)
				{
					XmlAttribute xmlAttribute = ndItemXml.OwnerDocument.CreateAttribute("WorkflowInstanceIDBackup");
					xmlAttribute.Value = (ndItemXml.Attributes["WorkflowInstanceID"].Value.StartsWith("{") ? ndItemXml.Attributes["WorkflowInstanceID"].Value : string.Concat("{", ndItemXml.Attributes["WorkflowInstanceID"].Value, "}"));
					ndItemXml.Attributes.Append(xmlAttribute);
				}
				else
				{
					ndItemXml.Attributes["WorkflowInstanceIDBackup"].Value = ndItemXml.Attributes["WorkflowInstanceID"].Value;
				}
				ndItemXml.Attributes.Remove(ndItemXml.Attributes["WorkflowInstanceID"]);
			}
			if (ndItemXml.Attributes["WorkflowListId"] != null)
			{
				ndItemXml.Attributes.Remove(ndItemXml.Attributes["WorkflowListId"]);
			}
		}

		private void SetPostedByFieldForDiscussionList(SPFolder targetFolder, XmlNode itemXML)
		{
			try
			{
				if (itemXML.Attributes["Author"] != null && !string.IsNullOrEmpty(itemXML.GetAttributeValueAsString("Author")))
				{
					SPList listByTitle = targetFolder.ParentList.ParentWeb.Lists.GetListByTitle("Community Members");
					if (listByTitle != null && listByTitle.Items.Count > 0)
					{
						SPListItem sPListItem = listByTitle.Items.Cast<SPListItem>().FirstOrDefault<SPListItem>((SPListItem i) => Convert.ToString(i["Member"]).Equals(itemXML.GetAttributeValueAsString("Author"), StringComparison.InvariantCultureIgnoreCase));
						if (sPListItem != null)
						{
							itemXML.Attributes["MemberLookup"].Value = Convert.ToString(sPListItem.ID);
						}
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Format("Failed to set PostedBy column value for list '{0}' inside web '{1}'. Error '{2}'", targetFolder.ParentList.DisplayName, targetFolder.ParentList.ParentWeb.Url, exception.ToString()));
			}
		}

		private void SetTitleFieldForMembersList(XmlNode itemXML, SPFolder targetFolder)
		{
			try
			{
				string attributeValueAsString = itemXML.GetAttributeValueAsString("Member");
				if (!string.IsNullOrEmpty(attributeValueAsString))
				{
					SecurityPrincipal item = targetFolder.ParentList.ParentWeb.SiteUsers[attributeValueAsString];
					if (item != null)
					{
						itemXML.Attributes["Title"].Value = ((SPUser)item).Name;
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Format("Failed to set Title column value for list '{0}' inside web '{1}'. Error '{2}'", targetFolder.ParentList.DisplayName, targetFolder.ParentList.ParentWeb.Url, exception.ToString()));
			}
		}

		public override SPListItem Transform(SPListItem dataObject, TAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
			SPFolder parentFolder = targets.ParentFolder as SPFolder;
			if (parentFolder == null)
			{
				LogItem logItem = new LogItem("Transformer Casting Error", dataObject.Name, dataObject.DisplayUrl, targets.ParentSPList.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = new ArgumentException("The target item collection does not belong to a SharePoint folder")
				};
				LogItem logItem1 = logItem;
				base.FireOperationStarted(logItem1);
				base.FireOperationFinished(logItem1);
			}
			if (!parentFolder.Adapter.SharePointVersion.IsSharePointOnline || !action.SharePointOptions.UseAzureOffice365Upload || !dataObject.ParentList.IsMigrationPipelineSupported || !parentFolder.ParentList.IsMigrationPipelineSupportedForTarget)
			{
				this.CopyReferencedUsersForItem(action, dataObject, parentFolder);
			}
			try
			{
				BaseListItemDataUpdater<TAction, TActionOptions>.MapItemUserFieldData(action.PrincipalMappings, dataObject, xmlNode);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem2 = new LogItem("Unexpected user mapping failure", dataObject.Name, dataObject.DisplayUrl, parentFolder.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = exception
				};
				LogItem logItem3 = logItem2;
				base.FireOperationStarted(logItem3);
				base.FireOperationFinished(logItem3);
			}
			if (action.SharePointOptions.CorrectingLinks)
			{
				try
				{
					LinkCorrector linkCorrector = action.LinkCorrector;
					SPFieldCollection fields = sources.ParentSPList.Fields as SPFieldCollection;
					TActionOptions sharePointOptions = action.SharePointOptions;
					linkCorrector.UpdateLinksInListItem(dataObject, parentFolder, xmlNode, fields, sharePointOptions.LinkCorrectTextFields);
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					LogItem logItem4 = new LogItem("Performing Link Correction", dataObject.Name, dataObject.DisplayUrl, parentFolder.DisplayUrl, ActionOperationStatus.Failed)
					{
						Information = string.Format("An error occurred while performing link correction for '{0}'. Error: '{1}'", dataObject.Name, exception2.Message),
						Details = exception2.ToString()
					};
					LogItem logItem5 = logItem4;
					base.FireOperationStarted(logItem5);
					base.FireOperationFinished(logItem5);
				}
			}
			action.MapListItemAudiences(xmlNode, dataObject.ParentList, parentFolder);
			if (action.SharePointOptions.CopyWorkflowInstanceData && (dataObject.ParentList.BaseTemplate == ListTemplateType.Tasks || dataObject.ParentList.BaseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy))
			{
				BaseListItemDataUpdater<TAction, TActionOptions>.ModifyListItemTaskXml(xmlNode);
			}
			this.UpdateFieldValueInListItem(dataObject, xmlNode, targets);
			if (parentFolder.Adapter.SharePointVersion.IsSharePoint2016OrLater && dataObject.ParentList.BaseTemplate.Equals(ListTemplateType.DiscussionBoard) && dataObject.ParentList.ParentWeb.Template.ID == 62)
			{
				this.CreateAuthorField(xmlNode);
			}
			if (parentFolder.ParentList.ParentWeb.WebTemplateID == 62)
			{
				ListTemplateType baseTemplate = parentFolder.ParentList.BaseTemplate;
				if (baseTemplate.Equals(ListTemplateType.CommunityMembers))
				{
					this.SetTitleFieldForMembersList(xmlNode, parentFolder);
				}
				else if (baseTemplate.Equals(ListTemplateType.DiscussionBoard))
				{
					this.SetPostedByFieldForDiscussionList(parentFolder, xmlNode);
				}
			}
			BaseListItemDataUpdater<TAction, TActionOptions>.UpdateVideoFileContentType(sources, xmlNode, parentFolder);
			if (dataObject.ItemType == SPListItemType.Folder && SPUtils.IsOneNoteFeatureEnabled(dataObject.ParentList) && SPUtils.IsDefaultOneNoteFolder(dataObject) && xmlNode.Attributes["FileLeafRef"] != null)
			{
				xmlNode.Attributes["FileLeafRef"].Value = SPUtils.GetUpdatedOneNoteFolderName(parentFolder.ParentList.ParentWeb);
			}
			dataObject.SetFullXML(xmlNode);
			return dataObject;
		}

		private string TruncateValue(string value, int maxLegth)
		{
			if (value.Length <= maxLegth)
			{
				return value;
			}
			return value.Substring(0, maxLegth);
		}

		private void UpdateFieldValueInListItem(SPListItem dataObject, XmlNode itemXML, SPListItemCollection targets)
		{
			SPFieldCollection fieldCollection = dataObject.ParentList.FieldCollection;
			string[] strArrays = new string[] { "ContentTypeIdFieldType", "URL", "Text" };
			SPFieldCollection fieldsOfTypes = fieldCollection.GetFieldsOfTypes(strArrays);
			if (fieldsOfTypes == null || fieldsOfTypes.Count == 0)
			{
				return;
			}
			foreach (SPField fieldsOfType in fieldsOfTypes)
			{
				string attributeValueAsString = itemXML.GetAttributeValueAsString(fieldsOfType.Name);
				string type = fieldsOfType.Type;
				string str = type;
				if (type == null)
				{
					continue;
				}
				if (str == "Text")
				{
					if (string.IsNullOrEmpty(attributeValueAsString))
					{
						continue;
					}
					int attributeValueAsInt = 0;
					SPField fieldByName = targets.ParentSPList.FieldCollection.GetFieldByName(fieldsOfType.Name);
					if (fieldByName != null)
					{
						attributeValueAsInt = fieldByName.FieldXML.GetAttributeValueAsInt("MaxLength");
					}
					if (attributeValueAsInt == 0)
					{
						attributeValueAsInt = 255;
					}
					attributeValueAsString = this.TruncateValue(attributeValueAsString, attributeValueAsInt);
					itemXML.Attributes[fieldsOfType.Name].Value = attributeValueAsString;
				}
				else if (str == "URL")
				{
					if (string.IsNullOrEmpty(attributeValueAsString))
					{
						continue;
					}
					string[] strArrays1 = attributeValueAsString.Split(new char[] { ',' });
					if (!string.IsNullOrEmpty(strArrays1[0].Trim()) || !string.IsNullOrEmpty(strArrays1[1].Trim()))
					{
						if (strArrays1[0].Length <= 255 && ((int)strArrays1.Length <= 1 || strArrays1[1].Length <= 255))
						{
							continue;
						}
						attributeValueAsString = string.Concat(this.TruncateValue(strArrays1[0], 255), ", ", ((int)strArrays1.Length > 1 ? this.TruncateValue(strArrays1[1].Trim(), 255) : ""));
						itemXML.Attributes[fieldsOfType.Name].Value = attributeValueAsString;
					}
					else
					{
						itemXML.Attributes[fieldsOfType.Name].Value = string.Empty;
					}
				}
				else if (str == "ContentTypeIdFieldType")
				{
					if (string.IsNullOrEmpty(attributeValueAsString))
					{
						continue;
					}
					string[] strArrays2 = new string[] { ";#" };
					string[] strArrays3 = attributeValueAsString.Split(strArrays2, StringSplitOptions.RemoveEmptyEntries);
					if ((int)strArrays3.Length < 2)
					{
						continue;
					}
					SPContentType contentType = this.GetContentType(strArrays3[0], strArrays3[1], targets);
					if (contentType == null)
					{
						continue;
					}
					attributeValueAsString = string.Concat(";#", contentType.Name, string.Concat(";#", contentType.ContentTypeID), ";#");
					itemXML.Attributes[fieldsOfType.Name].Value = attributeValueAsString;
				}
			}
		}

		private static void UpdateVideoFileContentType(SPListItemCollection sources, XmlNode itemXml, SPFolder targetFolder)
		{
			if (itemXml.Attributes != null && MigrationUtils.IsVideoFile(itemXml.GetAttributeValueAsString("File_x0020_Type")) && MigrationUtils.IsSource2007PublishingImageLibrary(sources) && MigrationUtils.IsTargetAssetLibraryOf2013OrLater(targetFolder))
			{
				itemXml.Attributes["ContentTypeId"].Value = "0x0101009148F5A04DDD49CBA7127AADA5FB792B00291D173ECE694D56B19D111489C4369D";
				itemXml.Attributes["ContentType"].Value = "Video";
			}
		}
	}
}