using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ContentTypeFieldMapper : PreconfiguredTransformer<SPContentType, CopyContentTypesAction, SPContentTypeCollection, SPContentTypeCollection>
	{
		public override string Name
		{
			get
			{
				return "Content Type Field Mapper";
			}
		}

		public ContentTypeFieldMapper()
		{
		}

		public override void BeginTransformation(CopyContentTypesAction action, SPContentTypeCollection sources, SPContentTypeCollection targets)
		{
		}

		private bool ContentTypeXMLContainsFieldReference(XmlNode ctNode, Guid fieldID, string fieldName)
		{
			if (fieldID != Guid.Empty && ctNode.SelectSingleNode(string.Format(".//FieldRefs/FieldRef[@ID='{0}']", fieldID.ToString("B"))) != null)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(fieldName) && ctNode.SelectSingleNode(string.Format(".//FieldRefs/FieldRef[@Name='{0}']", fieldName)) != null)
			{
				return true;
			}
			return false;
		}

		private XmlNode CreateFieldReference(XmlDocument parentDocument, SPField field)
		{
			XmlNode xmlNodes = parentDocument.CreateElement("FieldRef");
			XmlAttribute str = parentDocument.CreateAttribute("ID");
			XmlAttribute name = parentDocument.CreateAttribute("Name");
			XmlAttribute xmlAttribute = parentDocument.CreateAttribute("Required");
			XmlAttribute str1 = parentDocument.CreateAttribute("Hidden");
			XmlAttribute xmlAttribute1 = parentDocument.CreateAttribute("ReadOnly");
			xmlNodes.Attributes.Append(str);
			xmlNodes.Attributes.Append(name);
			xmlNodes.Attributes.Append(xmlAttribute);
			xmlNodes.Attributes.Append(str1);
			xmlNodes.Attributes.Append(xmlAttribute1);
			str.Value = field.ID.ToString("B");
			name.Value = field.Name;
			xmlAttribute.Value = field.Required.ToString();
			str1.Value = field.Hidden.ToString();
			xmlAttribute1.Value = (field.FieldXML.Attributes["ReadOnly"] == null ? "False" : field.FieldXML.Attributes["ReadOnly"].Value);
			return xmlNodes;
		}

		public override void EndTransformation(CopyContentTypesAction action, SPContentTypeCollection sources, SPContentTypeCollection targets)
		{
		}

		private XmlNode GetFieldReferenceFromContentTypeXml(XmlNode ctNode, Guid fieldID, string fieldName)
		{
			XmlNode xmlNodes = null;
			if (fieldID != Guid.Empty)
			{
				xmlNodes = ctNode.SelectSingleNode(string.Format(".//FieldRefs/FieldRef[@ID='{0}']", fieldID.ToString("B")));
			}
			if (xmlNodes == null && !string.IsNullOrEmpty(fieldName))
			{
				xmlNodes = ctNode.SelectSingleNode(string.Format(".//FieldRefs/FieldRef[@Name='{0}']", fieldName));
			}
			return xmlNodes;
		}

		private bool LinkCorrectDocumentTemplate(CopyContentTypesAction action, XmlNode docTemplateNode, StringBuilder verboseDetails)
		{
			bool flag = false;
			if (docTemplateNode != null)
			{
				XmlAttribute itemOf = docTemplateNode.Attributes["TargetName"];
				if (itemOf != null && !string.IsNullOrEmpty(itemOf.Value))
				{
					string str = action.LinkCorrector.CorrectUrl(itemOf.Value);
					if (!string.Equals(str, itemOf.Value, StringComparison.OrdinalIgnoreCase))
					{
						if (action.SharePointOptions.Verbose && verboseDetails != null)
						{
							string fSPrefixFieldInfoAndValue = Resources.FS_PrefixFieldInfoAndValue;
							string name = docTemplateNode.Name;
							string name1 = itemOf.Name;
							CultureInfo currentCulture = CultureInfo.CurrentCulture;
							string fSLinkCorrectedFromTo = Resources.FS_LinkCorrectedFromTo;
							object[] value = new object[] { itemOf.Value, str };
							verboseDetails.AppendLine(string.Format(fSPrefixFieldInfoAndValue, name, name1, string.Format(currentCulture, fSLinkCorrectedFromTo, value)));
						}
						itemOf.Value = str;
						flag = true;
					}
				}
			}
			return flag;
		}

		private bool LinkCorrectXmlDocumentFormUrls(CopyContentTypesAction action, XmlNodeList xmlDocumentNodes, StringBuilder verboseDetails)
		{
			bool flag = false;
			if (xmlDocumentNodes == null || xmlDocumentNodes.Count == 0)
			{
				return false;
			}
			foreach (XmlNode xmlDocumentNode in xmlDocumentNodes)
			{
				XmlNode item = xmlDocumentNode["FormUrls"];
				if (item == null)
				{
					continue;
				}
				foreach (XmlNode childNode in item.ChildNodes)
				{
					if (string.IsNullOrEmpty(childNode.InnerText))
					{
						continue;
					}
					string str = action.LinkCorrector.CorrectUrl(childNode.InnerText);
					if (string.Equals(str, childNode.InnerText, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					if (action.SharePointOptions.Verbose && verboseDetails != null)
					{
						string fSPrefixFieldInfoAndValue = Resources.FS_PrefixFieldInfoAndValue;
						string name = childNode.ParentNode.Name;
						string name1 = childNode.Name;
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string fSLinkCorrectedFromTo = Resources.FS_LinkCorrectedFromTo;
						object[] innerText = new object[] { childNode.InnerText, str };
						verboseDetails.AppendLine(string.Format(fSPrefixFieldInfoAndValue, name, name1, string.Format(currentCulture, fSLinkCorrectedFromTo, innerText)));
					}
					childNode.InnerText = str;
					flag = true;
				}
			}
			return flag;
		}

		public override SPContentType Transform(SPContentType dataObject, CopyContentTypesAction action, SPContentTypeCollection sources, SPContentTypeCollection targets)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			if (targets == null)
			{
				throw new ArgumentNullException("targets");
			}
			StringBuilder stringBuilder = null;
			if (action.SharePointOptions.Verbose)
			{
				stringBuilder = new StringBuilder(1024);
			}
			bool flag = false;
			XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
			if (targets.ParentList != null)
			{
				if (sources.ParentList.FieldCollection.TaxonomyFieldsExist)
				{
					if (action.SharePointOptions.Verbose && stringBuilder != null)
					{
						stringBuilder.AppendLine(Resources.ExcludeSpecificFieldRefAddMessage);
					}
					string str = TransformationRepository.GenerateKey("RemovedNoteFields", sources.ParentList.ID.ToString());
					Dictionary<string, string> valuesForKey = action.TransformationRepository.GetValuesForKey(str);
					if (valuesForKey != null)
					{
						Guid guid = new Guid("1390a86a-23da-45f0-8efe-ef36edadfb39");
						Guid guid1 = new Guid("23f27201-bee3-471e-b2e7-b64fd8b7ca38");
						foreach (KeyValuePair<string, string> keyValuePair in valuesForKey)
						{
							Guid guid2 = (string.IsNullOrEmpty(keyValuePair.Key) ? Guid.Empty : new Guid(keyValuePair.Key));
							XmlNode fieldReferenceFromContentTypeXml = this.GetFieldReferenceFromContentTypeXml(xmlNode, guid2, keyValuePair.Value);
							if (fieldReferenceFromContentTypeXml == null)
							{
								continue;
							}
							if (!object.Equals(guid2, guid) || this.GetFieldReferenceFromContentTypeXml(xmlNode, guid1, "TaxKeyword") != null)
							{
								XmlAttribute itemOf = fieldReferenceFromContentTypeXml.Attributes["Exclude"];
								if (itemOf == null)
								{
									itemOf = xmlNode.OwnerDocument.CreateAttribute("Exclude");
									fieldReferenceFromContentTypeXml.Attributes.Append(itemOf);
								}
								itemOf.Value = "True";
								if (action.SharePointOptions.Verbose && stringBuilder != null)
								{
									stringBuilder.AppendLine(string.Format("- {0}", keyValuePair.Value));
								}
								flag = true;
							}
							else
							{
								fieldReferenceFromContentTypeXml.ParentNode.RemoveChild(fieldReferenceFromContentTypeXml);
								flag = true;
							}
						}
					}
				}
				foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//FieldRef | .//Field"))
				{
					XmlAttribute xmlAttribute = xmlNodes.Attributes["ID"];
					if (xmlAttribute == null)
					{
						continue;
					}
					Guid guid3 = new Guid(xmlAttribute.Value);
					if (!action.GuidMappings.ContainsKey(guid3))
					{
						continue;
					}
					Guid item = action.GuidMappings[guid3];
					xmlAttribute.Value = item.ToString("B");
					flag = true;
					if (!action.SharePointOptions.Verbose)
					{
						continue;
					}
					string str1 = (xmlNodes.Attributes["Name"] != null ? xmlNodes.Attributes["Name"].Value : "?");
					string fSPrefixFieldInfoAndValue = Resources.FS_PrefixFieldInfoAndValue;
					string name = xmlNodes.Name;
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string fSVerboseGuidMapFromTo = Resources.FS_VerboseGuidMapFromTo;
					object[] objArray = new object[] { guid3.ToString(), null };
					Guid item1 = action.GuidMappings[guid3];
					objArray[1] = item1.ToString();
					stringBuilder.AppendLine(string.Format(fSPrefixFieldInfoAndValue, name, str1, string.Format(currentCulture, fSVerboseGuidMapFromTo, objArray)));
				}
			}
			else
			{
				SPFieldCollection availableColumns = targets.ParentWeb.AvailableColumns;
				this.UpdateSiteContentTypeFieldReferences(xmlNode, availableColumns, action, ref flag, stringBuilder);
			}
			if (this.LinkCorrectDocumentTemplate(action, xmlNode.SelectSingleNode(".//DocumentTemplate"), stringBuilder))
			{
				flag = true;
			}
			if (this.LinkCorrectXmlDocumentFormUrls(action, xmlNode.SelectNodes(".//XmlDocuments/XmlDocument"), stringBuilder))
			{
				flag = true;
			}
			if (flag)
			{
				dataObject = sources.AddOrUpdateContentType(dataObject.Name, xmlNode.OuterXml, (dataObject.ParentContentType != null ? dataObject.ParentContentType.Name : string.Empty));
				if (action.SharePointOptions.Verbose)
				{
					LogItem logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Completed)
					{
						Information = Resources.SuccessReviewDetails
					};
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					object[] transform = new object[] { Resources.Transform, Resources.ContentType, dataObject.Name, Environment.NewLine };
					stringBuilder.Insert(0, string.Format(cultureInfo, "{0} {1} '{2}':{3}", transform));
					logItem.Details = stringBuilder.ToString();
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
			}
			return dataObject;
		}

		private void UpdateSiteContentTypeFieldReferences(XmlNode ctNode, SPFieldCollection targetSiteColumns, CopyContentTypesAction action, ref bool transformationApplied, StringBuilder verboseDetails)
		{
			List<XmlNode> xmlNodes = new List<XmlNode>();
			List<SPField> sPFields = new List<SPField>();
			List<SPField> sPFields1 = new List<SPField>();
			List<SPField> sPFields2 = new List<SPField>();
			foreach (XmlNode xmlNodes1 in ctNode.SelectNodes(".//FieldRefs/FieldRef"))
			{
				SPField fieldById = null;
				bool flag = false;
				XmlAttribute itemOf = xmlNodes1.Attributes["ID"];
				if (itemOf != null)
				{
					Guid guid = new Guid(itemOf.Value);
					if (!action.GuidMappings.ContainsKey(guid))
					{
						fieldById = targetSiteColumns.GetFieldById(guid);
						if (fieldById != null)
						{
							itemOf.Value = fieldById.ID.ToString("B");
							flag = true;
						}
					}
					else
					{
						Guid item = action.GuidMappings[guid];
						itemOf.Value = item.ToString("B");
						fieldById = targetSiteColumns.GetFieldById(item);
						if (fieldById != null)
						{
							flag = true;
						}
					}
					if (flag && fieldById != null && action.SharePointOptions.Verbose && verboseDetails != null && !object.Equals(guid, fieldById.ID))
					{
						string str = (xmlNodes1.Attributes["Name"] != null ? xmlNodes1.Attributes["Name"].Value : "?");
						string fSPrefixFieldInfoAndValue = Resources.FS_PrefixFieldInfoAndValue;
						string name = xmlNodes1.Name;
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string fSVerboseGuidMapFromTo = Resources.FS_VerboseGuidMapFromTo;
						object[] objArray = new object[] { guid.ToString(), null };
						objArray[1] = fieldById.ID.ToString();
						verboseDetails.AppendLine(string.Format(fSPrefixFieldInfoAndValue, name, str, string.Format(currentCulture, fSVerboseGuidMapFromTo, objArray)));
					}
				}
				if (!flag)
				{
					XmlAttribute xmlAttribute = xmlNodes1.Attributes["Name"];
					if (xmlAttribute != null)
					{
						fieldById = targetSiteColumns.GetFieldByName(xmlAttribute.Value);
						if (fieldById != null)
						{
							if (itemOf == null)
							{
								itemOf = ctNode.OwnerDocument.CreateAttribute("ID");
								xmlNodes1.Attributes.Append(itemOf);
							}
							itemOf.Value = fieldById.ID.ToString("B");
							flag = true;
						}
					}
				}
				if (!flag)
				{
					xmlNodes.Add(xmlNodes1);
				}
				if (fieldById != null && fieldById.IsTaxonomyField)
				{
					sPFields1.Add(fieldById);
				}
				transformationApplied = true;
			}
			foreach (XmlNode xmlNodes2 in ctNode.SelectNodes(".//Field"))
			{
				SPField fieldByName = null;
				XmlAttribute itemOf1 = ctNode.Attributes["ID"];
				if (itemOf1 != null)
				{
					Guid guid1 = new Guid(itemOf1.Value);
					if (action.GuidMappings.ContainsKey(guid1))
					{
						fieldByName = targetSiteColumns.GetFieldById(action.GuidMappings[guid1]);
						if (fieldByName == null)
						{
							fieldByName = targetSiteColumns.GetFieldById(guid1);
							if (fieldByName != null)
							{
								sPFields.Add(fieldByName);
								xmlNodes.Add(xmlNodes2);
								transformationApplied = true;
								continue;
							}
							else if (fieldByName != null && action.SharePointOptions.Verbose && verboseDetails != null && !object.Equals(guid1, fieldByName.ID))
							{
								string str1 = (xmlNodes2.Attributes["Name"] != null ? xmlNodes2.Attributes["Name"].Value : "?");
								string fSPrefixFieldInfoAndValue1 = Resources.FS_PrefixFieldInfoAndValue;
								string name1 = xmlNodes2.Name;
								CultureInfo cultureInfo = CultureInfo.CurrentCulture;
								string fSVerboseGuidMapFromTo1 = Resources.FS_VerboseGuidMapFromTo;
								object[] objArray1 = new object[] { guid1.ToString(), null };
								objArray1[1] = fieldByName.ID.ToString();
								verboseDetails.AppendLine(string.Format(fSPrefixFieldInfoAndValue1, name1, str1, string.Format(cultureInfo, fSVerboseGuidMapFromTo1, objArray1)));
							}
						}
						else
						{
							sPFields.Add(fieldByName);
							xmlNodes.Add(xmlNodes2);
							transformationApplied = true;
							continue;
						}
					}
				}
				XmlAttribute xmlAttribute1 = xmlNodes2.Attributes["Name"];
				if (xmlAttribute1 != null)
				{
					fieldByName = targetSiteColumns.GetFieldByName(xmlAttribute1.Value);
					if (fieldByName != null)
					{
						sPFields.Add(fieldByName);
						xmlNodes.Add(xmlNodes2);
						transformationApplied = true;
						continue;
					}
				}
				if (fieldByName == null || !fieldByName.IsTaxonomyField || sPFields1.Contains(fieldByName))
				{
					continue;
				}
				sPFields1.Add(fieldByName);
			}
			if (sPFields1.Count > 0)
			{
				SPField sPField = targetSiteColumns.GetFieldByName("TaxCatchAll");
				if (sPField != null)
				{
					if (!this.ContentTypeXMLContainsFieldReference(ctNode, Guid.Empty, "TaxCatchAll"))
					{
						sPFields.Add(sPField);
						transformationApplied = true;
					}
					sPFields2.Add(sPField);
				}
				SPField fieldByName1 = targetSiteColumns.GetFieldByName("TaxCatchAllLabel");
				if (fieldByName1 != null)
				{
					if (!this.ContentTypeXMLContainsFieldReference(ctNode, Guid.Empty, "TaxCatchAllLabel"))
					{
						sPFields.Add(fieldByName1);
						transformationApplied = true;
					}
					sPFields2.Add(fieldByName1);
				}
				foreach (SPField sPField1 in sPFields1)
				{
					SPField fieldById1 = targetSiteColumns.GetFieldById(sPField1.TaxonomyHiddenTextField);
					if (fieldById1 == null)
					{
						continue;
					}
					if (!this.ContentTypeXMLContainsFieldReference(ctNode, sPField1.TaxonomyHiddenTextField, null))
					{
						sPFields.Add(fieldById1);
						transformationApplied = true;
					}
					sPFields2.Add(fieldById1);
				}
			}
			foreach (XmlNode xmlNode in xmlNodes)
			{
				xmlNode.ParentNode.RemoveChild(xmlNode);
			}
			if (sPFields.Count > 0)
			{
				XmlNode xmlNodes3 = ctNode.SelectSingleNode(".//FieldRefs");
				if (xmlNodes3 == null)
				{
					xmlNodes3 = ctNode.OwnerDocument.CreateElement("FieldRefs");
					ctNode.AppendChild(xmlNodes3);
				}
				foreach (SPField sPField2 in sPFields)
				{
					xmlNodes3.AppendChild(this.CreateFieldReference(xmlNodes3.OwnerDocument, sPField2));
				}
			}
			if (action.SharePointOptions.Verbose && verboseDetails != null && sPFields2.Count > 0)
			{
				verboseDetails.AppendLine(Resources.ExcludeSpecificFieldRefAddMessage);
			}
			foreach (SPField sPField3 in sPFields2)
			{
				XmlNode fieldReferenceFromContentTypeXml = this.GetFieldReferenceFromContentTypeXml(ctNode, sPField3.ID, sPField3.Name);
				if (fieldReferenceFromContentTypeXml == null)
				{
					continue;
				}
				XmlAttribute itemOf2 = fieldReferenceFromContentTypeXml.Attributes["Exclude"];
				if (itemOf2 == null)
				{
					itemOf2 = ctNode.OwnerDocument.CreateAttribute("Exclude");
					fieldReferenceFromContentTypeXml.Attributes.Append(itemOf2);
				}
				itemOf2.Value = "True";
				if (!action.SharePointOptions.Verbose || verboseDetails == null)
				{
					continue;
				}
				verboseDetails.AppendLine(string.Format("- {0}", sPField3.Name));
			}
		}
	}
}