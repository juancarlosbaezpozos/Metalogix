using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.ObjectResolution;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	public sealed class PasteActionUtils
	{
		public PasteActionUtils()
		{
		}

		public static bool CheckNodeNameCollisions(IXMLAbleList nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				SPNode item = nodes[i] as SPNode;
				if (item != null)
				{
					for (int j = i + 1; j < nodes.Count; j++)
					{
						SPNode sPNode = nodes[j] as SPNode;
						if (sPNode != null && sPNode.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool CollectionContainsMultipleLists(NodeCollection nodes)
		{
			bool flag;
			if (nodes == null)
			{
				return false;
			}
			bool flag1 = false;
			using (IEnumerator<Node> enumerator = nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SPNode current = (SPNode)enumerator.Current;
					if (!typeof(SPList).IsAssignableFrom(current.GetType()))
					{
						if (typeof(SPFolder).IsAssignableFrom(current.GetType()) || typeof(SPListItem).IsAssignableFrom(current.GetType()))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					else if (!flag1)
					{
						flag1 = true;
					}
					else
					{
						flag = true;
						return flag;
					}
				}
				return false;
			}
			return flag;
		}

		public static void CopyItemLevelWebParts(Metalogix.Actions.Action action, PasteListItemOptions options, Dictionary<SPListItem, SPListItem> docsWithUncopiedWebParts)
		{
			if (options.CopyDocumentWebParts && docsWithUncopiedWebParts.Count > 0)
			{
				foreach (SPListItem key in docsWithUncopiedWebParts.Keys)
				{
					CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction();
					copyWebPartsAction.SharePointOptions.SetFromOptions(options);
					SPListItem item = docsWithUncopiedWebParts[key];
					action.SubActions.Add(copyWebPartsAction);
					object[] objArray = new object[] { key, item };
					copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(key.ParentList.ParentWeb, item.ParentList.ParentWeb), null);
				}
				docsWithUncopiedWebParts.Clear();
			}
		}

		private static void GetAllFilterExpressions(IFilterExpression expression, ref List<FilterExpression> list)
		{
			FilterExpressionList filterExpressionList = expression as FilterExpressionList;
			if (filterExpressionList == null)
			{
				FilterExpression filterExpression = expression as FilterExpression;
				if (filterExpression != null)
				{
					list.Add(filterExpression);
				}
			}
			else
			{
				foreach (IFilterExpression filterExpression1 in filterExpressionList)
				{
					PasteActionUtils.GetAllFilterExpressions(filterExpression1, ref list);
				}
			}
		}

		private static string GetFieldFromColumnMapping(PasteListItemAction action, SPList sourceList, SPField sourceFieldById)
		{
			string empty = string.Empty;
			if (action.SharePointOptions.ContentTypeApplicationObjects != null)
			{
				foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in action.SharePointOptions.ContentTypeApplicationObjects)
				{
					if (!contentTypeApplicationObject.AppliesTo(sourceList) || contentTypeApplicationObject.ColumnMappings == null || !contentTypeApplicationObject.ColumnMappings.ContainsColumnChanges || contentTypeApplicationObject.ColumnMappings.Count <= 0)
					{
						continue;
					}
					string name = sourceFieldById.Name;
					if (string.IsNullOrEmpty(name) || contentTypeApplicationObject.ColumnMappings[name] == null)
					{
						continue;
					}
					empty = contentTypeApplicationObject.ColumnMappings[name].Target.Target;
					break;
				}
			}
			return empty;
		}

		private static SPField GetFieldFromOriginalXML(SPList sourceList, string sourceFieldGuid)
		{
			SPField sPField = null;
			if (sourceList.OriginalFieldsSchemaXML.Contains(sourceFieldGuid))
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceList.OriginalFieldsSchemaXML);
				Guid guid = new Guid(sourceFieldGuid);
				string str = string.Format("/Fields/Field[@ID=\"{0}\"]", guid.ToString("B"));
				XmlNode xmlNodes = xmlNode.SelectSingleNode(str);
				if (xmlNodes != null)
				{
					sPField = new SPField(xmlNodes);
				}
			}
			return sPField;
		}

		private static string GetFieldsToFetchInTerse(SPFolder sourceFolder, PasteListItemOptions options)
		{
			List<string> strs;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<Fields>");
			if (!sourceFolder.Adapter.SharePointVersion.IsSharePoint2003 || sourceFolder.ParentList.IsDocumentLibrary)
			{
				string[] strArrays = new string[] { "ID", "Modified", "FileRef", "FSObjType" };
				strs = new List<string>(strArrays);
				stringBuilder.Append("<Field Name=\"ID\" ColName=\"tp_ID\" Type=\"Counter\"/><Field ColName=\"tp_Modified\" Name=\"Modified\" Type=\"DateTime\"/><Field Name=\"FileRef\" Type=\"File\" /><Field Name=\"FSObjType\" ColName=\"Type\" Type=\"Lookup\" FromBaseType=\"TRUE\" />");
			}
			else
			{
				strs = new List<string>(new string[] { "ID", "Modified" });
				stringBuilder.Append("<Field Name=\"ID\" ColName=\"tp_ID\" Type=\"Counter\"/><Field ColName=\"tp_Modified\" Name=\"Modified\" Type=\"DateTime\"/>");
			}
			if (SharePointConfigurationVariables.IncludeFilteringColumnsInTerseData && options.FilterItems && options.ItemFilterExpression != null)
			{
				List<string> strs1 = new List<string>();
				List<FilterExpression> filterExpressions = new List<FilterExpression>();
				PasteActionUtils.GetAllFilterExpressions(options.ItemFilterExpression, ref filterExpressions);
				foreach (FilterExpression filterExpression in filterExpressions)
				{
					if (strs.Contains(filterExpression.Property) || strs1.Contains(filterExpression.Property))
					{
						continue;
					}
					strs1.Add(filterExpression.Property);
				}
				if (strs1 != null)
				{
					foreach (SPField field in sourceFolder.ParentList.Fields)
					{
						if (!strs1.Contains(field.Name) || strs.Contains(field.Name))
						{
							continue;
						}
						stringBuilder.Append(field.FieldXML.OuterXml);
						strs.Add(field.Name);
					}
				}
			}
			if ((sourceFolder.ParentList.BaseTemplate == ListTemplateType.Tasks || sourceFolder.ParentList.BaseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy) && sourceFolder.Adapter.SharePointVersion.IsSharePoint2007OrLater && !strs.Contains("ContentTypeId"))
			{
				stringBuilder.Append("<Field Name=\"ContentTypeId\" />");
			}
			stringBuilder.Append("</Fields>");
			return stringBuilder.ToString();
		}

		public static SPField GetTargetField(PasteListItemAction action, SPList sourceList, SPList targetList, string sourceFieldGuid)
		{
			Guid empty = Guid.Empty;
			if (action.GuidMappings.ContainsKey(new Guid(sourceFieldGuid)))
			{
				empty = action.GuidMappings[new Guid(sourceFieldGuid)];
			}
			if (empty != Guid.Empty)
			{
				return targetList.FieldCollection.GetFieldById(empty);
			}
			return PasteActionUtils.GetTargetFieldInternal(action, sourceList, targetList, sourceFieldGuid);
		}

		public static SPField GetTargetFieldInternal(PasteListItemAction action, SPList sourceList, SPList targetList, string sourceFieldGuid)
		{
			SPField fieldById = sourceList.FieldCollection.GetFieldById(new Guid(sourceFieldGuid)) ?? PasteActionUtils.GetFieldFromOriginalXML(sourceList, sourceFieldGuid);
			if (fieldById == null)
			{
				return null;
			}
			string fieldFromColumnMapping = PasteActionUtils.GetFieldFromColumnMapping(action, sourceList, fieldById);
			string str = (!string.IsNullOrEmpty(fieldFromColumnMapping) ? fieldFromColumnMapping : fieldById.Name);
			if (str == null)
			{
				return null;
			}
			return targetList.FieldCollection.GetFieldByName(str);
		}

		public static SPListItemCollection GetTerseItemData(SPFolder sourceFolder, PasteFolderOptions options)
		{
			if (sourceFolder is SPDiscussionList)
			{
				return ((SPDiscussionList)sourceFolder).DiscussionItems;
			}
			GetListItemOptions getListItemOption = new GetListItemOptions()
			{
				IncludePermissionsInheritance = (options.CopyFolderPermissions ? true : options.CopyItemPermissions),
				IncludeExternalizationData = options.ShallowCopyExternalizedData
			};
			ListItemQueryType listItemQueryType = (ListItemQueryType)((options.CopyListItems ? 1 : 0) + (options.CopySubFolders ? 2 : 0));
			return sourceFolder.GetTerseItems(options.CopySubFolders, listItemQueryType, PasteActionUtils.GetFieldsToFetchInTerse(sourceFolder, options), getListItemOption);
		}

		public static void IsMatchingContainer(Node sourceNode, Node targetNode, out bool bIsMatch, out bool bCouldMatch)
		{
			if (sourceNode is SPServer)
			{
				bIsMatch = targetNode is SPServer;
				bCouldMatch = false;
				return;
			}
			if (sourceNode is SPSite || sourceNode is SPWeb)
			{
				bIsMatch = (targetNode is SPWeb ? true : targetNode is SPSite);
				bCouldMatch = targetNode is SPServer;
				return;
			}
			SPFolder sPFolder = sourceNode as SPFolder;
			if (sPFolder != null)
			{
				if (targetNode is SPWeb || targetNode is SPSite || targetNode is SPServer)
				{
					bIsMatch = false;
					bCouldMatch = true;
					return;
				}
				SPFolder sPFolder1 = targetNode as SPFolder;
				if (sPFolder1 == null)
				{
					bIsMatch = false;
					bCouldMatch = false;
					return;
				}
				if (!(sPFolder.ParentList is SPDiscussionList) && !(sPFolder1.ParentList is SPDiscussionList))
				{
					bool baseType = sPFolder.ParentList.BaseType == sPFolder1.ParentList.BaseType;
					bool flag = baseType;
					bCouldMatch = baseType;
					bIsMatch = flag;
					return;
				}
				if (!(sPFolder.ParentList is SPDiscussionList) || !(sPFolder1.ParentList is SPDiscussionList))
				{
					bIsMatch = false;
					bCouldMatch = false;
					return;
				}
				if (sPFolder.ParentList == sPFolder && sPFolder1.ParentList == sPFolder1 || sPFolder.ParentList != sPFolder && sPFolder1.ParentList != sPFolder1)
				{
					bIsMatch = true;
					bCouldMatch = false;
					return;
				}
			}
			bIsMatch = false;
			bCouldMatch = true;
		}

		public static SPNode MapNodeByAssociations(SPNode sourceNode)
		{
			SPNode sPNode = null;
			return PasteActionUtils.MapNodeByAssociations(sourceNode, false, out sPNode);
		}

		public static SPNode MapNodeByAssociations(SPNode sourceNode, bool bReturnClosestMatch, out SPNode matchNode)
		{
			SPNode parent = sourceNode;
			List<SPNode> sPNodes = new List<SPNode>();
			SPNode node = null;
			while (parent != null && node == null)
			{
				sPNodes.Insert(0, parent);
				string xML = parent.Location.ToXML();
				string str = null;
				if (Metalogix.Explorer.Settings.ConnectionAssociations.ContainsKey(xML))
				{
					str = Metalogix.Explorer.Settings.ConnectionAssociations[xML].ToString();
				}
				else if (Metalogix.Explorer.Settings.ConnectionAssociations.ContainsValue(xML))
				{
					foreach (object key in Metalogix.Explorer.Settings.ConnectionAssociations.Keys)
					{
						if (Metalogix.Explorer.Settings.ConnectionAssociations[key].ToString() != xML)
						{
							continue;
						}
						str = key.ToString();
						break;
					}
				}
				if (str == null)
				{
					parent = (SPNode)parent.Parent;
				}
				else
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(str);
					node = (SPNode)(new Location(xmlDocument.DocumentElement)).GetNode();
				}
			}
			if (node == null)
			{
				matchNode = sourceNode;
			}
			else
			{
				int num = 1;
				SPNode item = null;
				matchNode = parent;
				while (num < sPNodes.Count)
				{
					if (node != null)
					{
						parent = sPNodes[num];
						item = (SPNode)node.Children[parent.Name];
						if (bReturnClosestMatch && item == null)
						{
							break;
						}
						matchNode = parent;
						node = item;
						num++;
					}
					else
					{
						break;
					}
				}
			}
			Metalogix.Explorer.Settings.SaveAssociatedConnections();
			return node;
		}

		public static void RefreshNode(SubActionCollection subActions, SPNode node)
		{
			if (node == null)
			{
				return;
			}
			SPFolder sPFolder = node as SPFolder;
			if (sPFolder != null)
			{
				node = sPFolder.ParentList.ParentWeb;
			}
			NodeCollection nodeCollection = new NodeCollection(new Node[] { node });
			RefreshAction refreshAction = new RefreshAction();
			subActions.Add(refreshAction);
			object[] objArray = new object[] { nodeCollection };
			refreshAction.RunAsSubAction(objArray, new ActionContext(new XMLAbleList(), nodeCollection), null);
		}
	}
}