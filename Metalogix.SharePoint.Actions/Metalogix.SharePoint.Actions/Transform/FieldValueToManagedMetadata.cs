using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data.Filters;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class FieldValueToManagedMetadata : Transformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection, FieldToManagedMetadataOptionCollection>
	{
		public const string C_REGEX_TAXONOMY_VALUE = "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})";

		public override string Name
		{
			get
			{
				return "Field Value To Managed Metadata";
			}
		}

		public override bool ReadOnly
		{
			get
			{
				this.m_bReadOnly = true;
				return this.m_bReadOnly;
			}
			set
			{
				this.m_bReadOnly = true;
			}
		}

		public FieldValueToManagedMetadata()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			string[] strArrays;
			SPListItem sPListItem;
			string[] value;
			string empty;
			ActionOperationStatus actionOperationStatu;
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
			if (targets.ParentSPList == null)
			{
				throw new ArgumentNullException("targets.ParentSPList");
			}
			if (targets.ParentSPList.ParentWeb == null)
			{
				throw new ArgumentNullException("targets.ParentSPList.ParentWeb");
			}
			if (targets.ParentSPList.ParentWeb.Adapter == null)
			{
				throw new ArgumentNullException("targets.ParentSPList.ParentWeb.Adapter");
			}
			List<FieldToManagedMetadataOption> fieldToManagedMetadataOptions = new List<FieldToManagedMetadataOption>();
			foreach (FieldToManagedMetadataOption item in base.Options.Items)
			{
				if (item.ListFilterExpression != null)
				{
					if (!item.ListFilterExpression.Evaluate(dataObject.ParentList))
					{
						continue;
					}
					fieldToManagedMetadataOptions.Add(item);
				}
				else
				{
					fieldToManagedMetadataOptions.Add(item);
				}
			}
			if (fieldToManagedMetadataOptions.Count == 0)
			{
				return dataObject;
			}
			bool flag = false;
			bool flag1 = false;
			LogItem logItem = null;
			try
			{
				try
				{
					if (targets.ParentSPList.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
					{
						SPWeb parentWeb = targets.ParentSPList.ParentWeb;
						if (action.ActionOptions.ForceRefresh && parentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && action.TransformationRepository.GetValueForKey("PropertyBag", "TermStoreRefreshed") == null)
						{
							FieldToManagedMetadata.RefreshTermstore(this, parentWeb, action.TransformationRepository);
						}
						string str = TransformationRepository.GenerateKey(typeof(SiteColumnToManagedMetadata).Name, dataObject.ParentList.ParentWeb.RootWebGUID);
						string str1 = TransformationRepository.GenerateKey(typeof(FieldToManagedMetadata).Name, dataObject.ParentList.ID);
						if (!action.TransformationRepository.DoesParentKeyExist(str1))
						{
							string list = dataObject.Adapter.Reader.GetList(dataObject.ParentList.ID);
							OperationReportingResult operationReportingResult = new OperationReportingResult(list);
							if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
							{
								OperationReportingException operationReportingException = new OperationReportingException(string.Format("GetList - No ObjectXml [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
								throw operationReportingException;
							}
							action.TransformationRepository.Add(str1, "listXml", operationReportingResult.ObjectXml);
						}
						bool flag2 = false;
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(dataObject.XML);
						StringBuilder stringBuilder = null;
						bool flag3 = false;
						if (action.SharePointOptions.Verbose)
						{
							stringBuilder = new StringBuilder(4096);
						}
						XmlDocument xmlDocument1 = new XmlDocument();
						xmlDocument1.LoadXml(action.TransformationRepository.GetValueForKey(str1, "listXml"));
						foreach (FieldToManagedMetadataOption fieldToManagedMetadataOption in fieldToManagedMetadataOptions)
						{
							flag3 = true;
							if (flag)
							{
								break;
							}
							foreach (SPField fieldCollection in dataObject.ParentList.FieldCollection)
							{
								if (flag)
								{
									break;
								}
								if (!fieldToManagedMetadataOption.ListFieldFilterExpression.Evaluate(fieldCollection) || fieldToManagedMetadataOption.CreateNewField && (!dataObject.ParentList.FieldCollection.FieldNameExists(fieldToManagedMetadataOption.NewFieldName) || !dataObject.ParentList.FieldCollection.FieldDisplayNameExists(fieldToManagedMetadataOption.NewFieldDisplayName)) && (!targets.ParentSPList.FieldCollection.FieldNameExists(fieldToManagedMetadataOption.NewFieldName) || !targets.ParentSPList.FieldCollection.FieldDisplayNameExists(fieldToManagedMetadataOption.NewFieldDisplayName)))
								{
									continue;
								}
								string type = fieldCollection.Type;
								string str2 = FieldToManagedMetadata.DetermineTargetFieldType(type, true);
								XmlElement documentElement = xmlDocument1.DocumentElement;
								CultureInfo invariantCulture = CultureInfo.InvariantCulture;
								object[] name = new object[] { fieldCollection.Name };
								XmlNode xmlNodes = documentElement.SelectSingleNode(string.Format(invariantCulture, "Fields/Field[@Name='{0}']", name));
								string str3 = (xmlNodes != null ? xmlNodes.Attributes["Type"].Value : type);
								if (string.IsNullOrEmpty(str2))
								{
									continue;
								}
								if (fieldToManagedMetadataOption.ListFilterExpression == null)
								{
									if (xmlNodes != null)
									{
										empty = (xmlNodes.Attributes["ID"] != null ? xmlNodes.Attributes["ID"].Value : string.Empty);
									}
									else
									{
										empty = string.Empty;
									}
									string str4 = empty;
									flag3 = (fieldCollection.FieldXML.Attributes["SourceID"] == null ? false : !string.IsNullOrEmpty(action.TransformationRepository.GetValueForKey(str, str4)));
								}
								if (!flag3)
								{
									continue;
								}
								if (fieldToManagedMetadataOption.CreateNewField)
								{
									SPField fieldByName = dataObject.ParentList.FieldCollection.GetFieldByName(fieldToManagedMetadataOption.NewFieldName);
									if (fieldByName != null && !FieldToManagedMetadata.IsTaxonomyFieldType(fieldByName.Type))
									{
										continue;
									}
								}
								CultureInfo currentCulture = CultureInfo.CurrentCulture;
								name = new object[] { targets.ParentSPList.Name, fieldCollection.Name };
								string str5 = string.Format(currentCulture, "{0}.{1}", name);
								bool flag4 = false;
								XmlAttribute itemOf = xmlDocument.DocumentElement.Attributes[fieldCollection.Name];
								if (itemOf == null || fieldToManagedMetadataOption.CreateNewField && string.Equals(fieldCollection.Name, fieldToManagedMetadataOption.NewFieldName) && Regex.IsMatch(itemOf.Value, "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})"))
								{
									continue;
								}
								if (logItem == null)
								{
									logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
								}
								string str6 = (fieldToManagedMetadataOption.CreateNewField ? fieldToManagedMetadataOption.NewFieldName : fieldCollection.Name);
								XmlAttribute xmlAttribute = xmlDocument.DocumentElement.Attributes[str6];
								if (xmlAttribute == null)
								{
									xmlAttribute = xmlDocument.CreateAttribute(str6);
									flag4 = true;
								}
								SPTermStore sPTermStore = targets.ParentSPList.ParentWeb.TermStores[fieldToManagedMetadataOption.TargetTermstore];
								if (sPTermStore != null)
								{
									FieldToManagedMetadata.EnsureTermGroupExists(this, sPTermStore, ref flag, ref flag1, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermstore, str5);
									if (flag)
									{
										break;
									}
									SPTermGroup sPTermGroup = sPTermStore.Groups[fieldToManagedMetadataOption.TargetGroup];
									FieldToManagedMetadata.EnsureTermSetExists(this, sPTermGroup, ref flag, ref flag1, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermstore, fieldToManagedMetadataOption.TargetTermSet, str5);
									if (flag)
									{
										break;
									}
									SPTermSet sPTermSet = sPTermGroup.TermSets[fieldToManagedMetadataOption.TargetTermSet];
									FieldToManagedMetadata.EnsureTermAnchorExists(this, sPTermSet, fieldToManagedMetadataOption, ref flag, ref flag1, str5);
									if (flag)
									{
										break;
									}
									SPTermSetItem anchorTerm = FieldToManagedMetadata.GetAnchorTerm(sPTermSet, fieldToManagedMetadataOption);
									StringBuilder stringBuilder1 = new StringBuilder();
									if (FieldToManagedMetadata.IsFieldTypeMulti(str3))
									{
										string value1 = itemOf.Value;
										value = new string[] { FieldToManagedMetadata.MultiTypeSplitCharacters(str3) };
										strArrays = value1.Split(value, StringSplitOptions.RemoveEmptyEntries);
									}
									else if (!FieldToManagedMetadata.IsTextOrNoteFieldType(str3))
									{
										if (!FieldToManagedMetadata.IsTaxonomyFieldType(str3))
										{
											value = new string[] { itemOf.Value };
											strArrays = value;
										}
										else
										{
											string str7 = Regex.Replace(itemOf.Value, "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})", string.Empty);
											value = new string[] { ";" };
											strArrays = str7.Split(value, StringSplitOptions.RemoveEmptyEntries);
										}
									}
									else if (itemOf.Value.Contains(Environment.NewLine))
									{
										string value2 = itemOf.Value;
										value = new string[] { Environment.NewLine };
										strArrays = value2.Split(value, StringSplitOptions.RemoveEmptyEntries);
									}
									else if (!itemOf.Value.Contains("\n"))
									{
										value = new string[] { itemOf.Value };
										strArrays = value;
									}
									else
									{
										string value3 = itemOf.Value;
										value = new string[] { "\n" };
										strArrays = value3.Split(value, StringSplitOptions.RemoveEmptyEntries);
									}
									if (FieldToManagedMetadata.IsLookupFieldType(str3) && (int)strArrays.Length > 0)
									{
										string str8 = (xmlNodes.Attributes["WebId"] != null ? xmlNodes.Attributes["WebId"].Value : string.Empty);
										string str9 = (xmlNodes.Attributes["ShowField"] != null ? xmlNodes.Attributes["ShowField"].Value : string.Empty);
										string str10 = (xmlNodes.Attributes["TargetListName"] != null ? xmlNodes.Attributes["TargetListName"].Value : string.Empty);
										SPList sPList = FieldToManagedMetadata.FindLookupList(dataObject.ParentList.ParentWeb, str10, str8, true);
										if (sPList == null)
										{
											LogItem logItem1 = new LogItem(this.Name, Resources.ObtainingLookupValue, str5, string.Empty, ActionOperationStatus.Warning);
											CultureInfo cultureInfo = CultureInfo.CurrentCulture;
											string fSLookupListNotFound = Resources.FS_LookupListNotFound;
											name = new object[] { str10, str8, str9 };
											logItem1.Information = string.Format(cultureInfo, fSLookupListNotFound, name);
											base.FireOperationStarted(logItem1);
											base.FireOperationFinished(logItem1);
											flag1 = true;
										}
										else
										{
											StringBuilder stringBuilder2 = new StringBuilder(512);
											XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
											{
												OmitXmlDeclaration = true
											};
											using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder2, xmlWriterSetting))
											{
												xmlWriter.WriteStartElement("Fields");
												xmlWriter.WriteStartElement("Field");
												xmlWriter.WriteAttributeString("Name", str9);
												xmlWriter.WriteEndElement();
												xmlWriter.WriteEndElement();
												xmlWriter.Flush();
											}
											for (int i = 0; i < (int)strArrays.Length; i++)
											{
												if (!string.IsNullOrEmpty(strArrays[i]))
												{
													string str11 = TransformationRepository.GenerateKey(string.Format("{0}.{1}", str10, str9), strArrays[i]);
													string valueForKey = action.TransformationRepository.GetValueForKey(str1, str11);
													if (valueForKey == null)
													{
														string listItems = sPList.Adapter.Reader.GetListItems(sPList.ConstantID, strArrays[i], stringBuilder2.ToString(), null, false, ListItemQueryType.ListItem, null, new GetListItemOptions());
														XmlDocument xmlDocument2 = new XmlDocument();
														xmlDocument2.LoadXml(listItems);
														valueForKey = xmlDocument2.DocumentElement.SelectSingleNode("ListItem").Attributes[str9].Value;
														action.TransformationRepository.Add(str1, str11, valueForKey);
													}
													strArrays[i] = valueForKey;
												}
											}
										}
									}
									if ((int)strArrays.Length > 0 && fieldToManagedMetadataOption.UsingItemFieldValueFilter)
									{
										bool flag5 = false;
										string[] substitute = (string[])strArrays.Clone();
										foreach (ItemFieldValueFilter itemFieldValueFilterCollection in fieldToManagedMetadataOption.ItemFieldValueFilterCollection)
										{
											for (int j = 0; j < (int)strArrays.Length; j++)
											{
												FilterString filterString = new FilterString(strArrays[j]);
												if (itemFieldValueFilterCollection.Filter.Evaluate(filterString))
												{
													substitute[j] = itemFieldValueFilterCollection.Substitute;
													flag5 = true;
												}
											}
										}
										if (flag5)
										{
											for (int k = 0; k < (int)strArrays.Length; k++)
											{
												strArrays[k] = substitute[k];
											}
										}
									}
									for (int l = 0; l < (int)strArrays.Length; l++)
									{
										strArrays[l] = TransformUtils.SanitiseForTaxonomy(strArrays[l]);
									}
									string[] strArrays1 = strArrays;
									for (int m = 0; m < (int)strArrays1.Length; m++)
									{
										string str12 = strArrays1[m];
										if (!string.IsNullOrEmpty(str12))
										{
											string str13 = TransformUtils.SanitiseForTaxonomy(str12);
											FieldToManagedMetadata.EnsureTermExists(this, anchorTerm, ref flag, ref flag1, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermstore, fieldToManagedMetadataOption.TargetTermSet, str13, str5);
											if (flag)
											{
												break;
											}
											SPTerm term = anchorTerm.GetTerm(str13);
											StringBuilder stringBuilder3 = stringBuilder1;
											CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
											name = new object[] { term.ToNamePipeGuidString(), null };
											name[1] = ((int)strArrays.Length == 1 ? string.Empty : Environment.NewLine);
											stringBuilder3.Append(string.Format(invariantCulture1, "{0}{1}", name));
										}
									}
									if (flag)
									{
										continue;
									}
									string value4 = itemOf.Value;
									if ((int)strArrays.Length <= 1)
									{
										xmlAttribute.Value = stringBuilder1.ToString();
									}
									else
									{
										stringBuilder1.Replace(Environment.NewLine, ";");
										string str14 = stringBuilder1.ToString();
										char[] chrArray = new char[] { ';' };
										xmlAttribute.Value = str14.TrimEnd(chrArray);
									}
									if (flag4)
									{
										xmlDocument.FirstChild.Attributes.Append(xmlAttribute);
									}
									if (action.SharePointOptions.Verbose)
									{
										CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
										string fSVerboseSourceItemTransformation = Resources.FS_VerboseSourceItemTransformation;
										name = new object[] { fieldCollection.Name, value4, str6, xmlAttribute.Value };
										stringBuilder.AppendLine(string.Format(currentCulture1, fSVerboseSourceItemTransformation, name));
									}
									flag2 = true;
								}
								else
								{
									LogItem logItem2 = new LogItem(this.Name, Resources.AccessingTermstore, str5, fieldToManagedMetadataOption.TargetTermstore, ActionOperationStatus.Warning);
									CultureInfo cultureInfo1 = CultureInfo.CurrentCulture;
									string fSTermstoreNotFound = Resources.FS_TermstoreNotFound;
									name = new object[] { fieldToManagedMetadataOption.TargetTermstore, str5 };
									logItem2.Information = string.Format(cultureInfo1, fSTermstoreNotFound, name);
									base.FireOperationStarted(logItem2);
									base.FireOperationFinished(logItem2);
									flag1 = true;
								}
							}
						}
						if (!flag && flag2)
						{
							dataObject.SetFullXML(xmlDocument.FirstChild);
							if (action.SharePointOptions.Verbose && stringBuilder.Length > 0)
							{
								if (logItem == null)
								{
									logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
								}
								logItem.Information = (flag1 ? Resources.PartialSuccessReviewDetails : Resources.SuccessReviewDetails);
								logItem.Details = stringBuilder.ToString();
							}
						}
					}
					else
					{
						logItem = new LogItem(this.Name, "Transform", sources.ParentSPList.Name, string.Empty, ActionOperationStatus.Running)
						{
							Information = Resources.TargetMustBeSharePoint2010,
							Status = ActionOperationStatus.Skipped
						};
						base.FireOperationStarted(logItem);
						sPListItem = dataObject;
						return sPListItem;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					flag = true;
					if (logItem == null)
					{
						logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
					}
					logItem.Exception = exception;
				}
				if (!flag)
				{
					return dataObject;
				}
				return null;
			}
			finally
			{
				if (logItem == null && (flag || flag1))
				{
					logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
				}
				if (logItem != null)
				{
					if (logItem.Status == ActionOperationStatus.Running)
					{
						LogItem logItem3 = logItem;
						if (flag)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (flag1 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem3.Status = actionOperationStatu;
					}
					base.FireOperationFinished(logItem);
				}
			}
			return sPListItem;
		}
	}
}