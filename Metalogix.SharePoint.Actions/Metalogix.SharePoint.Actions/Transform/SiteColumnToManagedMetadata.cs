using Metalogix.Actions;
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
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class SiteColumnToManagedMetadata : Transformer<SPField, CopySiteColumnsAction, SPFieldCollection, SPFieldCollection, SiteColumnToManagedMetadataOptionCollection>
	{
		private const string C_XPATH_VIEWFIELDS_SELECT = "ViewFields/FieldRef[@Name='{0}']";

		private const string C_XPATH_FIELDREFS_SELECT = "FieldRefs/FieldRef[@Name='{0}']";

		public const string C_XPATH_FIELDS_SELECT = "Fields/Field[@Name='{0}']";

		private const string C_XPATH_FIELD_ONLY = "//Field";

		public override string Name
		{
			get
			{
				return "Site Column To Managed Metadata";
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

		public SiteColumnToManagedMetadata()
		{
		}

		public override void BeginTransformation(CopySiteColumnsAction action, SPFieldCollection sources, SPFieldCollection targets)
		{
			object[] name;
			string str;
			SPTermGroup item;
			SPTermSet sPTermSet;
			ActionOperationStatus actionOperationStatu;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			if (sources.ParentWeb == null)
			{
				throw new ArgumentNullException("sources.ParentWeb");
			}
			if (targets == null)
			{
				throw new ArgumentNullException("targets");
			}
			if (targets.ParentWeb == null)
			{
				throw new ArgumentNullException("targets.ParentWeb");
			}
			if (targets.ParentWeb.Adapter == null)
			{
				throw new ArgumentNullException("targets.ParentWeb.Adapter");
			}
			List<SiteColumnToManagedMetadataOption> siteColumnToManagedMetadataOptions = new List<SiteColumnToManagedMetadataOption>();
			foreach (SiteColumnToManagedMetadataOption siteColumnToManagedMetadataOption in base.Options.Items)
			{
				if (!siteColumnToManagedMetadataOption.SiteFilterExpression.Evaluate(sources.ParentWeb))
				{
					continue;
				}
				siteColumnToManagedMetadataOptions.Add(siteColumnToManagedMetadataOption);
			}
			if (siteColumnToManagedMetadataOptions.Count == 0)
			{
				return;
			}
			string str1 = TransformationRepository.GenerateKey(typeof(SiteColumnToManagedMetadata).Name, sources.ParentWeb.RootWebGUID);
			LogItem logItem = new LogItem(this.Name, Resources.Transform, sources.ParentWeb.Name, string.Empty, ActionOperationStatus.Running);
			bool flag = false;
			List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
			bool flag1 = false;
			bool flag2 = false;
			XmlDocument xmlDocument = null;
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					if (targets.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
					{
						SPWeb parentWeb = targets.ParentWeb;
						if (action.ActionOptions.ForceRefresh && parentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && action.TransformationRepository.GetValueForKey("PropertyBag", "TermStoreRefreshed") == null)
						{
							FieldToManagedMetadata.RefreshTermstore(this, parentWeb, action.TransformationRepository);
						}
						xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(sources.XML);
						StringBuilder stringBuilder = null;
						if (action.SharePointOptions.Verbose)
						{
							stringBuilder = new StringBuilder(4096);
						}
						foreach (SiteColumnToManagedMetadataOption siteColumnToManagedMetadataOption1 in siteColumnToManagedMetadataOptions)
						{
							if (flag1)
							{
								break;
							}
							foreach (SPField source in sources)
							{
								if (!siteColumnToManagedMetadataOption1.SiteColumnFilterExpression.Evaluate(source))
								{
									continue;
								}
								CultureInfo invariantCulture = CultureInfo.InvariantCulture;
								name = new object[] { source.Name };
								XmlNode xmlNodes = xmlDocument.SelectSingleNode(string.Format(invariantCulture, "Fields/Field[@Name='{0}']", name));
								if (xmlNodes == null)
								{
									continue;
								}
								string value = xmlNodes.Attributes["Type"].Value;
								string str2 = FieldToManagedMetadata.DetermineTargetFieldType(value, false);
								if (siteColumnToManagedMetadataOption1.CreateNewField)
								{
									CultureInfo cultureInfo = CultureInfo.InvariantCulture;
									object[] newFieldName = new object[] { siteColumnToManagedMetadataOption1.NewFieldName };
									XmlNode xmlNodes1 = xmlDocument.SelectSingleNode(string.Format(cultureInfo, "Fields/Field[@Name='{0}']", newFieldName));
									if (xmlNodes1 == null)
									{
										CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
										object[] newFieldDisplayName = new object[] { siteColumnToManagedMetadataOption1.NewFieldDisplayName };
										xmlNodes1 = xmlDocument.SelectSingleNode(string.Format(invariantCulture1, "Fields/Field[@Name='{0}']", newFieldDisplayName));
									}
									if (xmlNodes1 != null)
									{
										string name1 = this.Name;
										string transform = Resources.Transform;
										CultureInfo currentCulture = CultureInfo.CurrentCulture;
										object[] objArray = new object[] { sources.ParentWeb.Name, source.Name };
										LogItem logItem1 = new LogItem(name1, transform, string.Format(currentCulture, "{0}.{1}", objArray), siteColumnToManagedMetadataOption1.NewFieldName, ActionOperationStatus.Warning);
										CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
										string fSNewFieldAlreadyExistsOnSource = Resources.FS_NewFieldAlreadyExistsOnSource;
										object[] newFieldDisplayName1 = new object[] { siteColumnToManagedMetadataOption1.NewFieldDisplayName, siteColumnToManagedMetadataOption1.NewFieldName };
										logItem1.Information = string.Format(currentCulture1, fSNewFieldAlreadyExistsOnSource, newFieldDisplayName1);
										CultureInfo cultureInfo1 = CultureInfo.CurrentCulture;
										string fSUnableToTransformSourceFieldDuplicate = Resources.FS_UnableToTransformSourceFieldDuplicate;
										object[] displayName = new object[] { source.DisplayName, source.Name, value, siteColumnToManagedMetadataOption1.NewFieldDisplayName, siteColumnToManagedMetadataOption1.NewFieldName, str2, siteColumnToManagedMetadataOption1.TargetTermstore, siteColumnToManagedMetadataOption1.TargetGroup, siteColumnToManagedMetadataOption1.TargetTermSet };
										logItem1.Details = string.Format(cultureInfo1, fSUnableToTransformSourceFieldDuplicate, displayName);
										base.FireOperationStarted(logItem1);
										flag2 = true;
										continue;
									}
								}
								if (!string.IsNullOrEmpty(str2))
								{
									SPField sPField = FieldToManagedMetadata.FindSiteColumn(parentWeb, (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldDisplayName : source.DisplayName), (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldName : source.Name), false) ?? FieldToManagedMetadata.FindSiteColumn(parentWeb, (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldDisplayName : source.DisplayName), (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldName : source.Name), true);
									if (sPField != null)
									{
										bool flag3 = true;
										string unableToObtainTaxonomyBindingInfo = null;
										if (FieldToManagedMetadata.IsTaxonomyFieldType(sPField.Type))
										{
											SPTermStore sPTermStore = parentWeb.TermStores[siteColumnToManagedMetadataOption1.TargetTermstore];
											if (sPTermStore != null)
											{
												item = sPTermStore.Groups[siteColumnToManagedMetadataOption1.TargetGroup];
											}
											else
											{
												item = null;
											}
											SPTermGroup sPTermGroup = item;
											if (sPTermGroup != null)
											{
												sPTermSet = sPTermGroup.TermSets[siteColumnToManagedMetadataOption1.TargetTermSet];
											}
											else
											{
												sPTermSet = null;
											}
											SPTermSet sPTermSet1 = sPTermSet;
											if (sPTermSet1 != null)
											{
												XmlNode xmlNodes2 = sPField.FieldXML.SelectSingleNode("Customization/ArrayOfProperty/Property[Name='SspId']/Value");
												XmlNode xmlNodes3 = sPField.FieldXML.SelectSingleNode("Customization/ArrayOfProperty/Property[Name='TermSetId']/Value");
												if (xmlNodes2 == null || xmlNodes3 == null)
												{
													flag3 = false;
													unableToObtainTaxonomyBindingInfo = Resources.UnableToObtainTaxonomyBindingInfo;
												}
												else if (!sPTermStore.Id.ToString().Contains(xmlNodes2.InnerText) || !sPTermSet1.Id.ToString().Contains(xmlNodes3.InnerText))
												{
													flag3 = false;
													CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
													string taxonomyBindingInfoDifferent = Resources.TaxonomyBindingInfoDifferent;
													name = new object[] { xmlNodes2.InnerText, xmlNodes3.InnerText };
													unableToObtainTaxonomyBindingInfo = string.Format(currentCulture2, taxonomyBindingInfoDifferent, name);
												}
											}
										}
										if (flag3)
										{
											flag3 = string.Equals(sPField.Type, str2, StringComparison.OrdinalIgnoreCase);
											if (!flag3)
											{
												CultureInfo cultureInfo2 = CultureInfo.CurrentCulture;
												string taxonomyTypeMismatch = Resources.TaxonomyTypeMismatch;
												name = new object[] { str2 };
												unableToObtainTaxonomyBindingInfo = string.Format(cultureInfo2, taxonomyTypeMismatch, name);
											}
										}
										if (!flag3)
										{
											string name2 = this.Name;
											string transform1 = Resources.Transform;
											CultureInfo currentCulture3 = CultureInfo.CurrentCulture;
											name = new object[] { sources.ParentWeb.Name, source.Name };
											LogItem logItem2 = new LogItem(name2, transform1, string.Format(currentCulture3, "{0}.{1}", name), siteColumnToManagedMetadataOption1.NewFieldName, ActionOperationStatus.Warning);
											LogItem logItem3 = logItem2;
											CultureInfo cultureInfo3 = CultureInfo.CurrentCulture;
											string fSFieldAlreadyExistsOnTarget = Resources.FS_FieldAlreadyExistsOnTarget;
											name = new object[] { (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldDisplayName : source.DisplayName), (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldName : source.Name) };
											logItem3.Information = string.Format(cultureInfo3, fSFieldAlreadyExistsOnTarget, name);
											LogItem logItem4 = logItem2;
											CultureInfo currentCulture4 = CultureInfo.CurrentCulture;
											string fSUnableToTransformTargetFieldTypeMismatch = Resources.FS_UnableToTransformTargetFieldTypeMismatch;
											name = new object[] { source.DisplayName, source.Name, value, sPField.DisplayName, sPField.Name, sPField.Type, null };
											name[6] = unableToObtainTaxonomyBindingInfo ?? string.Empty;
											logItem4.Details = string.Format(currentCulture4, fSUnableToTransformTargetFieldTypeMismatch, name);
											base.FireOperationStarted(logItem2);
											flag2 = true;
											continue;
										}
									}
									CultureInfo cultureInfo4 = CultureInfo.CurrentCulture;
									name = new object[] { sources.ParentWeb.Name, source.Name };
									string str3 = string.Format(cultureInfo4, "{0}.{1}", name);
									SPTermStore item1 = parentWeb.TermStores[siteColumnToManagedMetadataOption1.TargetTermstore];
									if (item1 != null)
									{
										FieldToManagedMetadata.EnsureTermGroupExists(this, item1, ref flag1, ref flag2, siteColumnToManagedMetadataOption1.TargetGroup, siteColumnToManagedMetadataOption1.TargetTermstore, str3);
										if (flag1)
										{
											break;
										}
										SPTermGroup sPTermGroup1 = item1.Groups[siteColumnToManagedMetadataOption1.TargetGroup];
										FieldToManagedMetadata.EnsureTermSetExists(this, sPTermGroup1, ref flag1, ref flag2, siteColumnToManagedMetadataOption1.TargetGroup, siteColumnToManagedMetadataOption1.TargetTermstore, siteColumnToManagedMetadataOption1.TargetTermSet, str3);
										if (flag1)
										{
											break;
										}
										SPTermSet item2 = sPTermGroup1.TermSets[siteColumnToManagedMetadataOption1.TargetTermSet];
										FieldToManagedMetadata.EnsureTermAnchorExists(this, item2, siteColumnToManagedMetadataOption1, ref flag1, ref flag2, str3);
										if (flag1)
										{
											break;
										}
										SPTermSetItem anchorTerm = FieldToManagedMetadata.GetAnchorTerm(item2, siteColumnToManagedMetadataOption1);
										IEnumerator enumerator = xmlNodes.SelectNodes("CHOICES/CHOICE").GetEnumerator();
										try
										{
											do
											{
											Label1:
												if (!enumerator.MoveNext())
												{
													break;
												}
												XmlNode current = (XmlNode)enumerator.Current;
												if (!string.IsNullOrEmpty(current.FirstChild.Value))
												{
													string str4 = TransformUtils.SanitiseForTaxonomy(current.FirstChild.Value);
													FieldToManagedMetadata.EnsureTermExists(this, anchorTerm, ref flag1, ref flag2, siteColumnToManagedMetadataOption1.TargetGroup, siteColumnToManagedMetadataOption1.TargetTermstore, siteColumnToManagedMetadataOption1.TargetTermSet, str4, str3);
												}
												else
												{
													goto Label1;
												}
											}
											while (!flag1);
										}
										finally
										{
											IDisposable disposable = enumerator as IDisposable;
											if (disposable != null)
											{
												disposable.Dispose();
											}
										}
										if (flag1)
										{
											continue;
										}
										string str5 = (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldName : source.Name);
										XmlAttribute itemOf = xmlNodes.Attributes["DisplayName"];
										if (itemOf != null)
										{
											str = (siteColumnToManagedMetadataOption1.CreateNewField ? siteColumnToManagedMetadataOption1.NewFieldDisplayName : itemOf.Value);
										}
										else
										{
											str = str5;
										}
										string str6 = str;
										string str7 = FieldToManagedMetadata.ConstructMMDFieldXml(str2, parentWeb.TermStores[siteColumnToManagedMetadataOption1.TargetTermstore].Groups[siteColumnToManagedMetadataOption1.TargetGroup].TermSets[siteColumnToManagedMetadataOption1.TargetTermSet], str5, str6, string.Empty, anchorTerm);
										if (action.SharePointOptions.Verbose)
										{
											CultureInfo currentCulture5 = CultureInfo.CurrentCulture;
											string fSVerboseSourceFieldTransformation = Resources.FS_VerboseSourceFieldTransformation;
											name = new object[] { source.Name, value, str5, str2, siteColumnToManagedMetadataOption1.TargetTermstore, siteColumnToManagedMetadataOption1.TargetGroup, siteColumnToManagedMetadataOption1.TargetTermSet };
											stringBuilder.AppendLine(string.Format(currentCulture5, fSVerboseSourceFieldTransformation, name));
										}
										XmlDocument xmlDocument1 = new XmlDocument();
										xmlDocument1.LoadXml(str7);
										XmlNode xmlNodes4 = xmlDocument1.SelectSingleNode("//Field");
										XmlNode innerXml = xmlDocument.CreateNode(XmlNodeType.Element, "Field", string.Empty);
										innerXml.InnerXml = xmlNodes4.InnerXml;
										foreach (XmlAttribute attribute in xmlNodes4.Attributes)
										{
											XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(attribute.Name);
											xmlAttribute.Value = attribute.Value;
											innerXml.Attributes.Append(xmlAttribute);
										}
										if (!siteColumnToManagedMetadataOption1.CreateNewField)
										{
											XmlNode xmlNodes5 = xmlDocument.SelectSingleNode("Fields");
											XmlAttribute[] xmlAttributeArray = new XmlAttribute[] { xmlNodes.Attributes["ID"], xmlNodes.Attributes["Required"], xmlNodes.Attributes["FillInChoice"], xmlNodes.Attributes["Group"], xmlNodes.Attributes["StaticName"] };
											foreach (XmlAttribute xmlAttribute1 in new List<XmlAttribute>(xmlAttributeArray))
											{
												if (xmlAttribute1 == null || innerXml.Attributes[xmlAttribute1.Name] != null)
												{
													continue;
												}
												XmlAttribute value1 = xmlDocument.CreateAttribute(xmlAttribute1.Name);
												value1.Value = xmlAttribute1.Value;
												innerXml.Attributes.Append(value1);
											}
											xmlNodes5.ReplaceChild(innerXml, xmlNodes);
										}
										else
										{
											if (innerXml.Attributes["DisplayName"] != null)
											{
												innerXml.Attributes["DisplayName"].Value = str6;
											}
											xmlNodes.ParentNode.AppendChild(innerXml);
											keyValuePairs.Add(new KeyValuePair<string, string>(source.Name, str5));
										}
										flag = true;
										TransformationRepository transformationRepository = action.TransformationRepository;
										string str8 = source.ID.ToString("B");
										CultureInfo invariantCulture2 = CultureInfo.InvariantCulture;
										name = new object[] { str5, str6 };
										transformationRepository.Add(str1, str8, string.Format(invariantCulture2, "{0};#{1}", name));
									}
									else
									{
										LogItem logItem5 = new LogItem(this.Name, Resources.AccessingTermstore, str3, siteColumnToManagedMetadataOption1.TargetTermstore, ActionOperationStatus.Warning);
										CultureInfo cultureInfo5 = CultureInfo.CurrentCulture;
										string fSTermstoreNotFound = Resources.FS_TermstoreNotFound;
										name = new object[] { siteColumnToManagedMetadataOption1.TargetTermstore, str3 };
										logItem5.Information = string.Format(cultureInfo5, fSTermstoreNotFound, name);
										base.FireOperationStarted(logItem5);
										flag2 = true;
									}
								}
								else
								{
									string name3 = this.Name;
									string transform2 = Resources.Transform;
									CultureInfo currentCulture6 = CultureInfo.CurrentCulture;
									object[] objArray1 = new object[] { sources.ParentWeb.Name, source.Name };
									LogItem logItem6 = new LogItem(name3, transform2, string.Format(currentCulture6, "{0}.{1}", objArray1), siteColumnToManagedMetadataOption1.NewFieldName, ActionOperationStatus.Warning);
									CultureInfo cultureInfo6 = CultureInfo.CurrentCulture;
									string fSSourceFieldTypeNotSupported = Resources.FS_SourceFieldTypeNotSupported;
									name = new object[] { value };
									logItem6.Information = string.Format(cultureInfo6, fSSourceFieldTypeNotSupported, name);
									base.FireOperationStarted(logItem6);
								}
							}
						}
						if (!flag1 && xmlDocument != null && flag)
						{
							sources.AddFieldCollection(xmlDocument.OuterXml);
							if (action.SharePointOptions.Verbose && stringBuilder.Length > 0)
							{
								logItem.Information = (flag2 ? Resources.PartialSuccessReviewDetails : Resources.SuccessReviewDetails);
								CultureInfo currentCulture7 = CultureInfo.CurrentCulture;
								name = new object[] { Resources.Transform, Resources.Site, sources.ParentWeb.Name, Environment.NewLine };
								stringBuilder.Insert(0, string.Format(currentCulture7, "{0} {1} '{2}':{3}", name));
								logItem.Details = stringBuilder.ToString();
							}
						}
					}
					else
					{
						logItem.Information = Resources.TargetMustBeSharePoint2010;
						logItem.Status = ActionOperationStatus.Warning;
						return;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					flag1 = true;
					logItem.Exception = exception;
				}
			}
			finally
			{
				if (logItem.Status == ActionOperationStatus.Running)
				{
					LogItem logItem7 = logItem;
					if (flag1)
					{
						actionOperationStatu = ActionOperationStatus.Failed;
					}
					else
					{
						actionOperationStatu = (flag2 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
					}
					logItem7.Status = actionOperationStatu;
				}
				base.FireOperationFinished(logItem);
			}
		}

		public override void EndTransformation(CopySiteColumnsAction action, SPFieldCollection sources, SPFieldCollection targets)
		{
		}

		public override SPField Transform(SPField dataObject, CopySiteColumnsAction action, SPFieldCollection sources, SPFieldCollection targets)
		{
			return dataObject;
		}
	}
}