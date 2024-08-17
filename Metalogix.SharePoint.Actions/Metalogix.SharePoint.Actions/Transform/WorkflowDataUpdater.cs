using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.Workflow;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class WorkflowDataUpdater : PreconfiguredTransformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>
	{
		public override string Name
		{
			get
			{
				return "Update Workflow Data";
			}
		}

		public WorkflowDataUpdater()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		private string DetermineUnmappedGuidActivities(XmlNode sourceXOMLDocumentElement, XmlNamespaceManager nmspcMgr)
		{
			MatchCollection matchCollections = Regex.Matches(sourceXOMLDocumentElement.OuterXml, "(<[^</\\x22]+?:[A-Za-z0-9]*[\\x20\\>])", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Dictionary<string, string> strs = new Dictionary<string, string>();
			Dictionary<string, string> strs1 = new Dictionary<string, string>();
			foreach (Match match in matchCollections)
			{
				string[] strArrays = match.Value.Split("<: >".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if ((int)strArrays.Length <= 1)
				{
					continue;
				}
				string str = strArrays[1];
				if (!strs.ContainsKey(str))
				{
					if (string.IsNullOrEmpty(nmspcMgr.LookupNamespace(strArrays[0])))
					{
						continue;
					}
					strs.Add(str, strArrays[0]);
				}
				else
				{
					if (strs[str] != strArrays[0])
					{
						string workflowErrorNamespaceDictionary = Resources.Workflow_ErrorNamespaceDictionary;
						object[] workflowDetermineAll = new object[] { Resources.Workflow_DetermineAll, str.ToString(), strs[str], strArrays[0] };
						throw new Exception(string.Format(workflowErrorNamespaceDictionary, workflowDetermineAll));
					}
					strs[str] = strArrays[0];
				}
			}
			List<string> strs2 = new List<string>();
			List<string> strs3 = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in strs)
			{
				XmlNodeList xmlNodeLists = sourceXOMLDocumentElement.SelectNodes(string.Format(".//{0}:{1}", keyValuePair.Value, keyValuePair.Key), nmspcMgr);
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					if (WorkflowRespository.Activities[keyValuePair.Key] != null)
					{
						continue;
					}
					MatchCollection matchCollections1 = Regex.Matches(xmlNodes.OuterXml, "\\{[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})\\}", RegexOptions.Singleline);
					if (matchCollections1.Count == 0)
					{
						continue;
					}
					strs2.Clear();
					foreach (Match match1 in matchCollections1)
					{
						if (strs2.Contains(match1.Value))
						{
							continue;
						}
						strs2.Add(match1.Value);
					}
					foreach (string str1 in strs2)
					{
						foreach (XmlNode attribute in xmlNodes.Attributes)
						{
							if (!attribute.Value.Contains(str1))
							{
								continue;
							}
							string str2 = string.Format("{0}.{1}:{2}", xmlNodes.LocalName, attribute.Name, attribute.Value);
							if (strs3.Contains(str2))
							{
								continue;
							}
							strs3.Add(str2);
						}
					}
				}
			}
			if (strs3.Count <= 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format(Resources.Workflow_UnmappedHeading, strs3.Count, Environment.NewLine, Environment.NewLine));
			strs3.ForEach((string e) => stringBuilder.AppendLine(e));
			return stringBuilder.ToString();
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		private XmlNamespaceManager GetNamespaceManger(XmlDocument xmlDoc)
		{
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlDoc.NameTable);
			foreach (Match match in Regex.Matches(xmlDoc.OuterXml, "((xmlns:?)([^=]*)=([\"][^\"]*[\"]))", RegexOptions.IgnoreCase | RegexOptions.Singleline))
			{
				if (string.IsNullOrEmpty(match.Groups[3].Value) || !string.IsNullOrEmpty(xmlNamespaceManagers.LookupNamespace(match.Groups[3].Value)))
				{
					continue;
				}
				xmlNamespaceManagers.AddNamespace(match.Groups[3].Value, (string.IsNullOrEmpty(match.Groups[4].Value) ? string.Empty : match.Groups[4].Value.Trim("\" ".ToCharArray())));
			}
			return xmlNamespaceManagers;
		}

		private string GetTargetContentType(PasteListItemAction action, string sourceContentTypeId, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string contentTypeID;
			try
			{
				if (!action.WorkflowMappings.ContainsKey(sourceContentTypeId))
				{
					foreach (Node list in sourceWeb.Lists)
					{
						SPList sPList = (SPList)list;
						if (sPList.BaseTemplate != ListTemplateType.Tasks)
						{
							continue;
						}
						SPContentType item = sPList.ContentTypes[sourceContentTypeId];
						if (item == null)
						{
							continue;
						}
						SPList item1 = targetWeb.Lists[sPList.Name];
						if (item1 == null)
						{
							continue;
						}
						SPContentType contentTypeByName = item1.ContentTypes.GetContentTypeByName(item.Name);
						if (contentTypeByName == null)
						{
							continue;
						}
						action.WorkflowMappings.Add(sourceContentTypeId, contentTypeByName.ContentTypeID);
						contentTypeID = contentTypeByName.ContentTypeID;
						return contentTypeID;
					}
					return sourceContentTypeId;
				}
				else
				{
					contentTypeID = action.WorkflowMappings[sourceContentTypeId];
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("An error occurred while fetching target content type for workflow mapping. Exception : {0}", exception));
				contentTypeID = sourceContentTypeId;
			}
			return contentTypeID;
		}

		private string GetTargetField(PasteListItemAction action, XmlNode associationNode, SPWeb sourceWeb, SPWeb targetWeb, string sourceFieldId)
		{
			string str;
			try
			{
				if (!action.GuidMappings.ContainsKey(new Guid(sourceFieldId)))
				{
					if (associationNode != null)
					{
						string str1 = Convert.ToString(new Guid(associationNode.GetAttributeValueAsString("ListID")));
						if (!string.IsNullOrEmpty(str1))
						{
							SPList listByGuid = sourceWeb.Lists.GetListByGuid(str1);
							if (listByGuid != null)
							{
								SPList item = targetWeb.Lists[listByGuid.Title];
								if (item != null)
								{
									SPField targetFieldInternal = PasteActionUtils.GetTargetFieldInternal(action, listByGuid, item, sourceFieldId);
									if (targetFieldInternal != null)
									{
										action.AddGuidMappings(new Guid(sourceFieldId), targetFieldInternal.ID);
										str = targetFieldInternal.ID.ToString();
										return str;
									}
								}
							}
						}
					}
					return sourceFieldId;
				}
				else
				{
					Guid guid = action.GuidMappings[new Guid(sourceFieldId)];
					str = guid.ToString("b");
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("Error occurred while fetching target field for workflow mapping. Exception : {0}", exception));
				str = sourceFieldId;
			}
			return str;
		}

		private string GetTargetList(PasteListItemAction action, string sourceListId, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string empty = string.Empty;
			try
			{
				empty = SPUtils.GetTargetList(action.GuidMappings, sourceListId, sourceWeb, targetWeb);
				if (!action.GuidMappings.ContainsKey(new Guid(sourceListId)))
				{
					action.GuidMappings.Add(new Guid(sourceListId), new Guid(empty));
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("Error occurred while fetching target list for workflow mapping. Exception : {0}", exception));
			}
			return empty;
		}

		private string MapAspxWorkflowData(PasteListItemAction action, SPListItem sourceItem, string constructedString)
		{
			string fileDirRef = sourceItem.FileDirRef;
			StringBuilder stringBuilder = new StringBuilder(constructedString);
			if (action.WorkflowMappings.ContainsKey(string.Concat(fileDirRef, "BaseID")))
			{
				string[] strArrays = action.WorkflowMappings[string.Concat(fileDirRef, "BaseID")].Split(new char[] { ',' });
				stringBuilder.Replace(strArrays[0], strArrays[1]);
			}
			if (action.WorkflowMappings.ContainsKey(string.Concat(fileDirRef, "ListID")))
			{
				string[] strArrays1 = action.WorkflowMappings[string.Concat(fileDirRef, "ListID")].Split(new char[] { ',' });
				stringBuilder.Replace(strArrays1[0], strArrays1[1]);
			}
			if (action.WorkflowMappings.ContainsKey(string.Concat(fileDirRef, "TaskListID")))
			{
				string[] strArrays2 = action.WorkflowMappings[string.Concat(fileDirRef, "TaskListID")].Split(new char[] { ',' });
				stringBuilder.Replace(strArrays2[0], strArrays2[1]);
			}
			return stringBuilder.ToString();
		}

		private string MapAttributeGuid(PasteListItemAction action, string sAttribute, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string upperInvariant;
			try
			{
				if (!sAttribute.StartsWith("{}"))
				{
					Guid guid = new Guid(sAttribute);
					upperInvariant = this.GetTargetList(action, guid.ToString(), sourceWeb, targetWeb).ToUpperInvariant();
				}
				else
				{
					Guid guid1 = new Guid(sAttribute.Substring(2, sAttribute.Length - 2));
					upperInvariant = string.Concat("{}{", this.GetTargetList(action, guid1.ToString(), sourceWeb, targetWeb).ToUpperInvariant(), "}");
				}
			}
			catch
			{
				upperInvariant = sAttribute;
			}
			return upperInvariant;
		}

		private void MapCAMLGuids(PasteListItemAction action, XmlNode ndActivity, SPWeb sourceWeb, SPWeb targetWeb)
		{
			try
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(ndActivity.Attributes["CamlQuery"].Value);
				foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//List"))
				{
					try
					{
						if (xmlNodes.Attributes["ID"] != null && !string.IsNullOrEmpty(xmlNodes.Attributes["ID"].Value))
						{
							Guid guid = new Guid(xmlNodes.Attributes["ID"].Value);
							xmlNodes.Attributes["ID"].Value = string.Concat("{", this.GetTargetList(action, guid.ToString(), sourceWeb, targetWeb), "}");
						}
					}
					catch
					{
					}
				}
				ndActivity.Attributes["CamlQuery"].Value = xmlNode.OuterXml;
			}
			catch (Exception exception)
			{
			}
		}

		private XmlNode MapNintexWorkflowItemData(PasteListItemAction action, SPListItem sourceItem, SPFolder targetFolder, XmlNode ndWorkflowItem)
		{
			XmlNode xmlNode;
			string outerXml = ndWorkflowItem.OuterXml;
			try
			{
				if (ndWorkflowItem.Attributes["NintexWorkflowID"] != null && !string.IsNullOrEmpty(ndWorkflowItem.Attributes["NintexWorkflowID"].Value) && action.WorkflowMappings.ContainsKey(ndWorkflowItem.Attributes["NintexWorkflowID"].Value))
				{
					ndWorkflowItem.Attributes["NintexWorkflowID"].Value = action.WorkflowMappings[ndWorkflowItem.Attributes["NintexWorkflowID"].Value];
				}
				if (ndWorkflowItem.Attributes["NWAssociatedWebID"] != null && !string.IsNullOrEmpty(ndWorkflowItem.Attributes["NWAssociatedWebID"].Value) && action.GuidMappings.ContainsKey(new Guid(ndWorkflowItem.Attributes["NWAssociatedWebID"].Value)))
				{
					XmlAttribute itemOf = ndWorkflowItem.Attributes["NWAssociatedWebID"];
					Guid item = action.GuidMappings[new Guid(ndWorkflowItem.Attributes["NWAssociatedWebID"].Value)];
					itemOf.Value = item.ToString();
				}
				if (ndWorkflowItem.Attributes["AssociatedListID"] != null && !string.IsNullOrEmpty(ndWorkflowItem.Attributes["AssociatedListID"].Value) && action.GuidMappings.ContainsKey(new Guid(ndWorkflowItem.Attributes["AssociatedListID"].Value)))
				{
					XmlAttribute str = ndWorkflowItem.Attributes["AssociatedListID"];
					Guid guid = action.GuidMappings[new Guid(ndWorkflowItem.Attributes["AssociatedListID"].Value)];
					str.Value = guid.ToString();
				}
				if (ndWorkflowItem.Attributes["AssociatedContentType"] != null && !string.IsNullOrEmpty(ndWorkflowItem.Attributes["AssociatedContentType"].Value) && action.WorkflowMappings.ContainsKey(ndWorkflowItem.Attributes["AssociatedContentType"].Value))
				{
					ndWorkflowItem.Attributes["AssociatedContentType"].Value = action.WorkflowMappings[ndWorkflowItem.Attributes["AssociatedContentType"].Value];
				}
				xmlNode = ndWorkflowItem;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem("Workflow Mapping", sourceItem.Name, sourceItem.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running)
				{
					Exception = exception
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
				xmlNode = XmlUtility.StringToXmlNode(outerXml);
			}
			return xmlNode;
		}

		private void MapWorkflowFileData(PasteListItemAction action, SPListItem sourceItem, SharePointVersion targetVersion, SPWeb targetWeb)
		{
			string lowerInvariant = sourceItem.Name.ToLowerInvariant();
			if (lowerInvariant.Contains(".xoml") || lowerInvariant.Contains(".aspx"))
			{
				UTF8Encoding uTF8Encoding = new UTF8Encoding();
				bool flag = false;
				byte[] binary = sourceItem.Binary;
				string str = (sourceItem.IsCurrentVersion ? string.Empty : sourceItem.VersionString);
				string str1 = uTF8Encoding.GetString(binary);
				if (sourceItem.Name.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase))
				{
					str1 = this.MapXmlWorkflowData(action, sourceItem, str1, targetWeb);
					flag = true;
				}
				else if (sourceItem.Name.EndsWith(".xoml.rules", StringComparison.OrdinalIgnoreCase))
				{
					str1 = this.MapXomlRulesWorkflowData(action, sourceItem, str1, targetVersion, targetWeb);
					flag = true;
				}
				else if (sourceItem.Name.EndsWith(".xoml", StringComparison.OrdinalIgnoreCase))
				{
					str1 = this.MapXomlWorkflowData(action, sourceItem, str1, str, targetVersion, targetWeb);
					flag = true;
				}
				else if (sourceItem.Name.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
				{
					str1 = this.MapAspxWorkflowData(action, sourceItem, str1);
					flag = true;
				}
				if (flag)
				{
					sourceItem.Binary = Encoding.UTF8.GetBytes(str1);
				}
			}
		}

		private string MapXmlWorkflowData(PasteListItemAction action, SPListItem sourceItem, string constructedString, SPWeb targetWeb)
		{
			string outerXml;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(constructedString);
				XmlNode documentElement = xmlDocument.DocumentElement;
				SPWeb parentWeb = sourceItem.ParentList.ParentWeb;
				try
				{
					string value = "";
					XmlNode xmlNodes = documentElement.SelectSingleNode(".//Template");
					if (!action.WorkflowMappings.ContainsKey(string.Concat(sourceItem.FileDirRef, "BaseID")))
					{
						Guid guid = Guid.NewGuid();
						if (action.WorkflowMappings.ContainsKey(xmlNodes.Attributes["BaseID"].Value.ToUpperInvariant()))
						{
							guid = new Guid(action.WorkflowMappings[xmlNodes.Attributes["BaseID"].Value]);
						}
						value = xmlNodes.Attributes["BaseID"].Value;
						xmlNodes.Attributes["BaseID"].Value = string.Concat("{", guid, "}");
						action.WorkflowMappings.Add(string.Concat(sourceItem.FileDirRef, "BaseID"), string.Concat(value, ",", xmlNodes.Attributes["BaseID"].Value));
					}
					else
					{
						string[] strArrays = action.WorkflowMappings[string.Concat(sourceItem.FileDirRef, "BaseID")].Split(new char[] { ',' });
						xmlNodes.Attributes["BaseID"].Value = strArrays[1];
					}
					if (!string.IsNullOrEmpty(value) && !action.WorkflowMappings.ContainsKey(value))
					{
						action.WorkflowMappings.Add(value.ToUpperInvariant(), xmlNodes.Attributes["BaseID"].Value.ToUpperInvariant());
					}
					if (xmlNodes.Attributes["ContentTypeID"] != null && !string.IsNullOrEmpty(xmlNodes.Attributes["ContentTypeID"].Value))
					{
						string targetContentType = this.GetTargetContentType(action, xmlNodes.Attributes["ContentTypeID"].Value, parentWeb, targetWeb);
						if (xmlNodes.Attributes["Category"] != null && !string.IsNullOrEmpty(xmlNodes.Attributes["Category"].Value) && xmlNodes.Attributes["Category"].Value.Contains(xmlNodes.Attributes["ContentTypeID"].Value))
						{
							xmlNodes.Attributes["Category"].Value = xmlNodes.Attributes["Category"].Value.Replace(xmlNodes.Attributes["ContentTypeID"].Value, targetContentType);
						}
						xmlNodes.Attributes["ContentTypeID"].Value = targetContentType;
					}
					if (xmlNodes.Attributes["DocLibID"] != null && !string.IsNullOrEmpty(xmlNodes.Attributes["DocLibID"].Value))
					{
						xmlNodes.Attributes["DocLibID"].Value = string.Concat("{", this.GetTargetList(action, sourceItem.ParentList.ID, parentWeb, targetWeb), "}");
					}
					XmlNode xmlNodes1 = documentElement.SelectSingleNode(".//Association");
					foreach (XmlNode targetField in documentElement.SelectNodes(".//Extended/Fields/Field"))
					{
						string attributeValueAsString = targetField.GetAttributeValueAsString("ID");
						if (string.IsNullOrEmpty(attributeValueAsString))
						{
							continue;
						}
						targetField.Attributes["ID"].Value = this.GetTargetField(action, xmlNodes1, parentWeb, targetWeb, attributeValueAsString);
					}
					if (xmlNodes1 != null)
					{
						if (xmlNodes1.Attributes["ListID"] != null && !string.IsNullOrEmpty(xmlNodes1.Attributes["ListID"].Value))
						{
							string str = xmlNodes1.Attributes["ListID"].Value;
							xmlNodes1.Attributes["ListID"].Value = string.Concat("{", this.GetTargetList(action, str, parentWeb, targetWeb), "}");
							if (!action.WorkflowMappings.ContainsKey(string.Concat(sourceItem.FileDirRef, "ListID")))
							{
								action.WorkflowMappings.Add(string.Concat(sourceItem.FileDirRef, "ListID"), string.Concat(str, ",", xmlNodes1.Attributes["ListID"].Value));
							}
						}
						if (xmlNodes1.Attributes["TaskListID"] != null && !string.IsNullOrEmpty(xmlNodes1.Attributes["TaskListID"].Value))
						{
							string value1 = xmlNodes1.Attributes["TaskListID"].Value;
							xmlNodes1.Attributes["TaskListID"].Value = string.Concat("{", this.GetTargetList(action, value1, parentWeb, targetWeb).ToUpperInvariant(), "}");
							if (!action.WorkflowMappings.ContainsKey(string.Concat(sourceItem.FileDirRef, "TaskListID")))
							{
								action.WorkflowMappings.Add(string.Concat(sourceItem.FileDirRef, "TaskListID"), string.Concat(value1, ",", xmlNodes1.Attributes["TaskListID"].Value));
							}
						}
						if (xmlNodes1.Attributes["HistoryListID"] != null && !string.IsNullOrEmpty(xmlNodes1.Attributes["HistoryListID"].Value))
						{
							string str1 = xmlNodes1.Attributes["HistoryListID"].Value;
							xmlNodes1.Attributes["HistoryListID"].Value = string.Concat("{", this.GetTargetList(action, str1, parentWeb, targetWeb).ToUpperInvariant(), "}");
							if (!action.WorkflowMappings.ContainsKey(string.Concat(sourceItem.FileDirRef, "HistoryListID")))
							{
								action.WorkflowMappings.Add(string.Concat(sourceItem.FileDirRef, "HistoryListID"), string.Concat(str1, ",", xmlNodes1.Attributes["HistoryListID"].Value));
							}
						}
					}
				}
				catch
				{
					outerXml = constructedString;
					return outerXml;
				}
				foreach (XmlNode targetContentType1 in documentElement.SelectNodes(".//ContentType | .//TaskContentType"))
				{
					try
					{
						targetContentType1.Attributes["ContentTypeID"].Value = this.GetTargetContentType(action, targetContentType1.Attributes["ContentTypeID"].Value, parentWeb, targetWeb);
						if (targetContentType1.Attributes["SourceID"] != null && targetContentType1.Attributes["SourceID"].Value != null)
						{
							targetContentType1.Attributes["SourceID"].Value = string.Concat("{", this.GetTargetList(action, targetContentType1.Attributes["SourceID"].Value, parentWeb, targetWeb), "}");
						}
						if (targetContentType1.Attributes["ID"] != null && targetContentType1.Attributes["ID"].Value != null)
						{
							targetContentType1.Attributes["ID"].Value = string.Concat("{", Guid.NewGuid(), "}");
						}
					}
					catch
					{
					}
				}
				outerXml = documentElement.OuterXml;
			}
			catch
			{
				outerXml = constructedString;
			}
			return outerXml;
		}

		private string MapXomlRulesWorkflowData(PasteListItemAction action, SPListItem sourceItem, string constructedString, SharePointVersion targetVersion, SPWeb targetWeb)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string str = constructedString;
			str = this.ReplaceVersionInformation(targetVersion, str, sourceItem.Adapter.SharePointVersion);
			xmlDocument.LoadXml(str);
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManagers.AddNamespace("ns1", "clr-namespace:System;Assembly=mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			XmlNode documentElement = xmlDocument.DocumentElement;
			foreach (XmlNode item in documentElement.SelectNodes(".//ns1:String", xmlNamespaceManagers))
			{
				string innerText = item.InnerText;
				if (action.PrincipalMappings.ContainsKey(innerText))
				{
					item.InnerText = action.PrincipalMappings[innerText];
				}
				if (!item.InnerText.Contains("{}{"))
				{
					continue;
				}
				item.InnerText = this.MapAttributeGuid(action, item.InnerText, sourceItem.ParentList.ParentWeb, targetWeb);
			}
			return documentElement.OuterXml;
		}

		private string MapXomlWorkflowData(PasteListItemAction action, SPListItem sourceItem, string constructedString, string versionNo, SharePointVersion targetVersion, SPWeb targetWeb)
		{
			XmlDocument xmlDocument = new XmlDocument();
			Dictionary<string, string> strs = new Dictionary<string, string>();
			SPWeb parentWeb = sourceItem.ParentList.ParentWeb;
			string str = constructedString;
			str = this.ReplaceVersionInformation(targetVersion, str, sourceItem.Adapter.SharePointVersion);
			xmlDocument.LoadXml(str);
			XmlNamespaceManager namespaceManger = this.GetNamespaceManger(xmlDocument);
			XmlNode documentElement = xmlDocument.DocumentElement;
			string str1 = this.DetermineUnmappedGuidActivities(documentElement, namespaceManger);
			if (!string.IsNullOrEmpty(str1) || action.SharePointOptions.Verbose)
			{
				string str2 = (string.IsNullOrEmpty(versionNo) ? string.Empty : string.Format(Resources.Workflow_AnalyseSourceWorkflowVersion, versionNo));
				LogItem logItem = new LogItem(string.Format(Resources.Workflow_AnalyseSourceWorkflow, str2), sourceItem.FileLeafRef, sourceItem.EncodedAbsUrl, string.Empty, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				if (string.IsNullOrEmpty(str1))
				{
					logItem.Status = ActionOperationStatus.Completed;
				}
				else
				{
					logItem.Information = Resources.Workflow_ActivitiesGuidCharacteristics;
					logItem.Details = str1;
					logItem.Status = ActionOperationStatus.Warning;
				}
				base.FireOperationFinished(logItem);
			}
			bool flag = false;
			if (documentElement.Attributes["xmlns:ns1"] != null)
			{
				flag = documentElement.Attributes["xmlns:ns1"].Value.Contains("Nintex.Workflow.Activities");
			}
			foreach (string sharePointWorkflowActivityName in WorkflowRespository.Activities.SharePointWorkflowActivityNames)
			{
				MatchCollection matchCollections = Regex.Matches(xmlDocument.OuterXml, string.Format("<[^</]+?:{0}[\\x20\\>]", sharePointWorkflowActivityName.ToString()), RegexOptions.IgnoreCase);
				foreach (Match match in matchCollections)
				{
					string[] strArrays = match.Value.Split("<:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (!strs.ContainsKey(sharePointWorkflowActivityName))
					{
						strs.Add(sharePointWorkflowActivityName, strArrays[0]);
					}
					else
					{
						if (strs[sharePointWorkflowActivityName] != strArrays[0])
						{
							string workflowErrorNamespaceDictionary = Resources.Workflow_ErrorNamespaceDictionary;
							object[] objArray = new object[] { WorkflowActivityType.SharePoint.ToString(), sharePointWorkflowActivityName.ToString(), strs[sharePointWorkflowActivityName], strArrays[0] };
							throw new Exception(string.Format(workflowErrorNamespaceDictionary, objArray));
						}
						strs[sharePointWorkflowActivityName] = strArrays[0];
					}
				}
			}
			if (flag)
			{
				foreach (string nintexWorkflowActivityName in WorkflowRespository.Activities.NintexWorkflowActivityNames)
				{
					MatchCollection matchCollections1 = Regex.Matches(xmlDocument.OuterXml, string.Format("<[^</]+?:{0}[\\x20\\>]", nintexWorkflowActivityName.ToString()), RegexOptions.IgnoreCase);
					foreach (Match match1 in matchCollections1)
					{
						string[] strArrays1 = match1.Value.Split("<:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						if (!strs.ContainsKey(nintexWorkflowActivityName))
						{
							strs.Add(nintexWorkflowActivityName, strArrays1[0]);
						}
						else
						{
							if (strs[nintexWorkflowActivityName] != strArrays1[0])
							{
								string workflowErrorNamespaceDictionary1 = Resources.Workflow_ErrorNamespaceDictionary;
								object[] objArray1 = new object[] { WorkflowActivityType.Nintex.ToString(), nintexWorkflowActivityName.ToString(), strs[nintexWorkflowActivityName], strArrays1[0] };
								throw new Exception(string.Format(workflowErrorNamespaceDictionary1, objArray1));
							}
							strs[nintexWorkflowActivityName] = strArrays1[0];
						}
					}
				}
			}
			foreach (string key in strs.Keys)
			{
				WorkflowActivity item = WorkflowRespository.Activities[key];
				XmlNodeList xmlNodeLists = documentElement.SelectNodes(string.Format(".//{0}:{1}", strs[key], key.ToString()), namespaceManger);
				foreach (XmlNode targetContentType in xmlNodeLists)
				{
					try
					{
						foreach (WorkflowActivityAttribute attribute in item.Attributes)
						{
							if (targetContentType.Attributes[attribute.Name] == null || string.IsNullOrEmpty(targetContentType.Attributes[attribute.Name].Value))
							{
								continue;
							}
							switch (attribute.Operation)
							{
								case WorkflowActivityAttributeOperation.Global:
								{
									string value = targetContentType.Attributes[attribute.Name].Value;
									targetContentType.Attributes[attribute.Name].Value = (action.PrincipalMappings.ContainsKey(value) ? action.PrincipalMappings[value] : value);
									continue;
								}
								case WorkflowActivityAttributeOperation.Guid:
								{
									targetContentType.Attributes[attribute.Name].Value = this.MapAttributeGuid(action, targetContentType.Attributes[attribute.Name].Value, parentWeb, targetWeb);
									continue;
								}
								case WorkflowActivityAttributeOperation.Workflow:
								{
									if (!action.WorkflowMappings.ContainsKey(targetContentType.Attributes[attribute.Name].Value))
									{
										if (!attribute.Name.Equals("ContentTypeId", StringComparison.OrdinalIgnoreCase))
										{
											continue;
										}
										targetContentType.Attributes[attribute.Name].Value = this.GetTargetContentType(action, targetContentType.Attributes[attribute.Name].Value, parentWeb, targetWeb);
										continue;
									}
									else
									{
										targetContentType.Attributes[attribute.Name].Value = action.WorkflowMappings[targetContentType.Attributes[attribute.Name].Value];
										continue;
									}
								}
								case WorkflowActivityAttributeOperation.CAML:
								{
									this.MapCAMLGuids(action, targetContentType, parentWeb, targetWeb);
									continue;
								}
								case WorkflowActivityAttributeOperation.UserMapping:
								{
									try
									{
										string attributeValueAsString = targetContentType.GetAttributeValueAsString(attribute.Name);
										XmlDocument xmlDocument1 = new XmlDocument();
										xmlDocument1.LoadXml(attributeValueAsString);
										XmlNamespaceManager xmlNamespaceManagers = this.GetNamespaceManger(xmlDocument1);
										MatchCollection matchCollections2 = Regex.Matches(xmlDocument1.OuterXml, string.Format("<[^</]+?:{0}[\\x20\\>]", attribute.InnerXmlNodeName), RegexOptions.IgnoreCase);
										string empty = string.Empty;
										if (matchCollections2.Count > 0)
										{
											string[] strArrays2 = matchCollections2[0].Value.Split("<:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
											if ((int)strArrays2.Length > 0)
											{
												empty = strArrays2[0];
											}
										}
										XmlNodeList xmlNodeLists1 = xmlDocument1.DocumentElement.SelectNodes(string.Format(".//{0}:{1}", empty, attribute.InnerXmlNodeName), xmlNamespaceManagers);
										foreach (XmlNode xmlNodes in xmlNodeLists1)
										{
											string innerText = xmlNodes.InnerText;
											xmlNodes.InnerText = (action.PrincipalMappings.ContainsKey(innerText) ? action.PrincipalMappings[innerText] : innerText);
										}
										targetContentType.Attributes[attribute.Name].Value = xmlDocument1.OuterXml;
										continue;
									}
									catch (Exception exception1)
									{
										Exception exception = exception1;
										string str3 = (string.IsNullOrEmpty(versionNo) ? string.Empty : string.Format(Resources.Workflow_AnalyseSourceWorkflowVersion, versionNo));
										LogItem logItem1 = new LogItem(string.Format(Resources.Workflow_AnalyseSourceWorkflow, str3), sourceItem.FileLeafRef, sourceItem.EncodedAbsUrl, string.Empty, ActionOperationStatus.Failed);
										base.FireOperationStarted(logItem1);
										logItem1.Exception = exception;
										logItem1.Information = string.Format("An error occurred while applying user mapping for activity {0} in workflow xoml file : {1}", item.Name, sourceItem.Url);
										base.FireOperationFinished(logItem1);
										continue;
									}
									break;
								}
								default:
								{
									continue;
								}
							}
						}
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						object[] objArray2 = new object[] { item.ActivityType.ToString(), key.ToString(), strs[key], exception2.Message };
						throw new Exception(string.Format("Error mapping values for {0} workflow activity '{1}' [{2}] : {3}", objArray2), exception2);
					}
				}
			}
			return documentElement.OuterXml;
		}

		private string ReplaceVersionInformation(SharePointVersion targetVersion, string constructedString, SharePointVersion sourceVersion)
		{
			string str = constructedString;
			if (targetVersion.MajorVersion > sourceVersion.MajorVersion)
			{
				if (targetVersion.IsSharePoint2010OrLater)
				{
					str = str.Replace("Assembly=Microsoft.SharePoint.WorkflowActions, Version=12.0.0.0,", "Assembly=Microsoft.SharePoint.WorkflowActions, Version=14.0.0.0,").Replace("Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=12.0.0.0,", "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=14.0.0.0,").Replace("QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=12.0.0.0", "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=14.0.0.0");
				}
				if (targetVersion.IsSharePoint2013OrLater)
				{
					str = str.Replace("Assembly=Microsoft.SharePoint.WorkflowActions, Version=14.0.0.0,", "Assembly=Microsoft.SharePoint.WorkflowActions, Version=15.0.0.0,").Replace("Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=14.0.0.0,", "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=15.0.0.0,").Replace("QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=14.0.0.0", "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=15.0.0.0");
				}
				if (targetVersion.IsSharePoint2016OrLater)
				{
					str = str.Replace("Assembly=Microsoft.SharePoint.WorkflowActions, Version=15.0.0.0,", "Assembly=Microsoft.SharePoint.WorkflowActions, Version=16.0.0.0,").Replace("Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=15.0.0.0,", "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=16.0.0.0,").Replace("QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=15.0.0.0", "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=16.0.0.0");
				}
			}
			return str;
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (sources.ParentSPList.IsDocumentLibrary && dataObject.ItemType != SPListItemType.Folder && (dataObject.ParentList.Name == "Workflows" && dataObject.ParentList.BaseTemplate == ListTemplateType.NoCodeWorkflows || dataObject.ParentList.Name == "NintexWorkflows" && dataObject.ParentList.BaseTemplate == ListTemplateType.NintexWorkflows || dataObject.ParentList.Name.Equals("wfpub", StringComparison.InvariantCultureIgnoreCase) && dataObject.ParentList.BaseTemplate == ListTemplateType.NoCodePublicWorkflow))
			{
				SPList parentList = targets.ParentList as SPList;
				if (parentList == null)
				{
					throw new ArgumentNullException("parentList", "Unable to determine target parent list, required to obtain target SharePoint version");
				}
				this.MapWorkflowFileData(action, dataObject, parentList.Adapter.SharePointVersion, parentList.ParentWeb);
				if (dataObject.ParentList.Name == "NintexWorkflows" && dataObject.Name.EndsWith(".xoml.wfconfig.xml"))
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
					xmlNode = this.MapNintexWorkflowItemData(action, dataObject, targets.ParentFolder as SPFolder, xmlNode);
					dataObject.SetFullXML(xmlNode);
				}
			}
			return dataObject;
		}
	}
}