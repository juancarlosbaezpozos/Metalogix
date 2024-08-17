using Metalogix.Actions;
using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.Taxonomy.Generic;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ListItemColumnMapper : BaseColumnMapper<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>
	{
		public const string C_REGEX_TAXONOMY_VALUE = "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})";

		private ColumnMappings m_columnMappings;

		public ListItemColumnMapper()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			this.m_columnMappings = base.GetColumnMappings(sources.ParentSPList, action.SharePointOptions);
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			this.m_columnMappings = null;
		}

		private void ListItemFieldValueToManagedMetadata(XmlNode itemXml, SPFolder targetFolder)
		{
			IList<string> strs;
			if (itemXml == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			try
			{
				try
				{
					SPList sPList = (targetFolder is SPList ? (SPList)targetFolder : targetFolder.ParentList);
					foreach (ListSummaryItem totalMapping in this.m_columnMappings.TotalMappings)
					{
						string target = totalMapping.Source.Target;
						string internalNameFromDisplayName = totalMapping.Target.Target;
						if (totalMapping.Target.IsNew)
						{
							internalNameFromDisplayName = SPField.GetInternalNameFromDisplayName(internalNameFromDisplayName);
						}
						foreach (SPField field in (SPFieldCollection)sPList.Fields)
						{
							if (!field.IsTaxonomyField || !(internalNameFromDisplayName == field.Name))
							{
								continue;
							}
							XmlAttribute itemOf = itemXml.Attributes[target];
							string[] value = new string[0];
							StringBuilder stringBuilder1 = new StringBuilder();
							if (FieldToManagedMetadata.IsFieldTypeMulti(totalMapping.Source.TargetType))
							{
								string str = itemOf.Value;
								string[] strArrays = new string[] { FieldToManagedMetadata.MultiTypeSplitCharacters(totalMapping.Source.TargetType) };
								value = str.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
							}
							else if (FieldToManagedMetadata.IsTextOrNoteFieldType(totalMapping.Source.TargetType))
							{
								if (itemOf.Value.Contains(Environment.NewLine))
								{
									string value1 = itemOf.Value;
									string[] newLine = new string[] { Environment.NewLine };
									value = value1.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
								}
								else if (!itemOf.Value.Contains("\n"))
								{
									value = new string[] { itemOf.Value };
								}
								else
								{
									string str1 = itemOf.Value;
									string[] strArrays1 = new string[] { "\n" };
									value = str1.Split(strArrays1, StringSplitOptions.RemoveEmptyEntries);
								}
							}
							else if (FieldToManagedMetadata.IsTaxonomyFieldType(totalMapping.Source.TargetType))
							{
								string str2 = Regex.Replace(itemOf.Value, "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})", string.Empty);
								string[] strArrays2 = new string[] { ";" };
								value = str2.Split(strArrays2, StringSplitOptions.RemoveEmptyEntries);
							}
							else if (!FieldToManagedMetadata.IsChoiceFieldType(totalMapping.Source.TargetType))
							{
								stringBuilder.AppendLine();
								CultureInfo invariantCulture = CultureInfo.InvariantCulture;
								object[] targetType = new object[] { totalMapping.Source.TargetType };
								stringBuilder.AppendLine(string.Format(invariantCulture, "Conversion from column type <{0}> to managed metadata is not supported.", targetType));
								break;
							}
							else
							{
								value = new string[] { itemOf.Value };
							}
							if ((int)value.Length > 1 && field.Type != "TaxonomyFieldTypeMulti")
							{
								stringBuilder.AppendLine("There are multiple values for the source column and the target column does not allow multiple values:");
								CultureInfo cultureInfo = CultureInfo.InvariantCulture;
								object[] objArray = new object[] { target, itemOf.Value, internalNameFromDisplayName };
								stringBuilder.AppendLine(string.Format(cultureInfo, "{0}: <{1}> --> {2}", objArray));
							}
							SPTermStore item = targetFolder.ParentList.ParentWeb.TermStores[field.TermstoreId];
							strs = (field.AnchorId != Guid.Empty ? item.GetTermHierarchy(field.TermSetId, field.AnchorId) : item.GetTermSetHierarchy(field.TermSetId, true));
							if (strs.Count != 0)
							{
								SPTermGroup sPTermGroup = item.Groups[strs[0]];
								SPTermSetItem term = sPTermGroup.TermSets[strs[1]];
								if (strs.Count > 2)
								{
									for (int i = 2; i < strs.Count; i++)
									{
										term = term.GetTerm(strs[i]);
									}
								}
								string[] strArrays3 = value;
								for (int j = 0; j < (int)strArrays3.Length; j++)
								{
									string str3 = strArrays3[j];
									if (!string.IsNullOrEmpty(str3))
									{
										string str4 = TransformUtils.SanitiseForTaxonomy(str3);
										bool flag1 = false;
										bool flag2 = false;
										FieldToManagedMetadata.EnsureTermExists(this, term, ref flag1, ref flag2, strs[0], item.Name, strs[1], str4, sPList.Name);
										if (flag1)
										{
											break;
										}
										SPTerm sPTerm = term.GetTerm(str4);
										StringBuilder stringBuilder2 = stringBuilder1;
										CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
										object[] namePipeGuidString = new object[] { sPTerm.ToNamePipeGuidString(), null };
										namePipeGuidString[1] = ((int)value.Length == 1 ? string.Empty : Environment.NewLine);
										stringBuilder2.Append(string.Format(invariantCulture1, "{0}{1}", namePipeGuidString));
									}
								}
								if ((int)value.Length <= 1)
								{
									itemOf.Value = stringBuilder1.ToString();
									break;
								}
								else
								{
									stringBuilder1.Replace(Environment.NewLine, ";");
									string str5 = stringBuilder1.ToString();
									char[] chrArray = new char[] { ';' };
									itemOf.Value = str5.TrimEnd(chrArray);
									break;
								}
							}
							else
							{
								stringBuilder.AppendLine();
								CultureInfo cultureInfo1 = CultureInfo.InvariantCulture;
								object[] objArray1 = new object[] { internalNameFromDisplayName };
								stringBuilder.AppendLine(string.Format(cultureInfo1, "The target column <{0}> is not bound to a termset in managed metadata.", objArray1));
								break;
							}
						}
					}
				}
				catch (Exception exception)
				{
					ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(exception);
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Exception Details:");
					}
					stringBuilder.AppendLine(exceptionMessageAndDetail.Message);
					stringBuilder.AppendLine(exceptionMessageAndDetail.Detail);
					flag = true;
				}
			}
			finally
			{
				if (stringBuilder.Length > 0)
				{
					LogItem logItem = new LogItem("Column Mapping", targetFolder.DisplayName, "", "", ActionOperationStatus.Running)
					{
						Status = (flag ? ActionOperationStatus.Failed : ActionOperationStatus.Warning),
						Information = (flag ? "Error occured, please see details" : "Please see details"),
						Details = stringBuilder.ToString()
					};
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
			}
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (this.m_columnMappings != null)
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
				SPFolder parentFolder = targets.ParentFolder as SPFolder;
				this.ListItemFieldValueToManagedMetadata(xmlNode, parentFolder);
				this.m_columnMappings.ModifyListItemXML(xmlNode, dataObject.ParentFolder, parentFolder, action.SharePointOptions.FilterFields);
				dataObject.SetFullXML(xmlNode);
			}
			return dataObject;
		}
	}
}