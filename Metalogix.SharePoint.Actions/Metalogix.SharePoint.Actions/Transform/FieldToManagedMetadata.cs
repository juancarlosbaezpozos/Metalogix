using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data.Filters;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class FieldToManagedMetadata : Transformer<SPList, PasteListAction, SPListCollection, SPListCollection, FieldToManagedMetadataOptionCollection>
	{
		private const string C_XPATH_VIEWFIELDS_SELECT = "ViewFields/FieldRef[@Name='{0}']";

		private const string C_XPATH_FIELDREFS_SELECT = "FieldRefs/FieldRef[@Name='{0}']";

		public const string C_XPATH_FIELDS_SELECT = "Fields/Field[@Name='{0}']";

		private const string C_XPATH_FIELD_ONLY = "//Field";

		public override string Name
		{
			get
			{
				return "Field To Managed Metadata";
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

		public FieldToManagedMetadata()
		{
		}

		public override void BeginTransformation(PasteListAction action, SPListCollection sources, SPListCollection targets)
		{
		}

		internal static string ConstructMMDFieldXml(string targetFieldType, SPTermSet termSet, string fieldName, string displayName, string sourceID, SPTermSetItem anchorTerm)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				OmitXmlDeclaration = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
			{
				xmlWriter.WriteStartElement("Field");
				xmlWriter.WriteAttributeString("Name", fieldName);
				xmlWriter.WriteAttributeString("DisplayName", displayName);
				xmlWriter.WriteAttributeString("Type", targetFieldType);
				if (!string.IsNullOrEmpty(sourceID))
				{
					xmlWriter.WriteAttributeString("SourceID", sourceID);
				}
				if (string.Equals(targetFieldType, "TaxonomyFieldType", StringComparison.OrdinalIgnoreCase) || string.Equals(targetFieldType, "TaxonomyFieldTypeMulti", StringComparison.OrdinalIgnoreCase))
				{
					bool flag = string.Equals(targetFieldType, "TaxonomyFieldTypeMulti", StringComparison.OrdinalIgnoreCase);
					string empty = string.Empty;
					string str = string.Empty;
					string empty1 = string.Empty;
					if (termSet != null)
					{
						if (anchorTerm != null && anchorTerm.Id != termSet.Id)
						{
							empty1 = anchorTerm.Id.ToString();
						}
						str = termSet.Id.ToString();
						if (termSet.TermStore != null)
						{
							empty = termSet.TermStore.Id.ToString();
						}
					}
					xmlWriter.WriteAttributeString("Mult", flag.ToString().ToUpper(CultureInfo.InvariantCulture));
					xmlWriter.WriteStartElement("Default");
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("Customization");
					xmlWriter.WriteStartElement("ArrayOfProperty");
					if (!string.IsNullOrEmpty(empty))
					{
						xmlWriter.WriteStartElement("Property");
						xmlWriter.WriteElementString("Name", "SspId");
						xmlWriter.WriteStartElement("Value");
						xmlWriter.WriteAttributeString("xmlns", "q1", null, "http://www.w3.org/2001/XMLSchema");
						xmlWriter.WriteAttributeString("p4", "type", "http://www.w3.org/2001/XMLSchema-instance", "q1:string");
						xmlWriter.WriteString(empty);
						xmlWriter.WriteEndElement();
						xmlWriter.WriteEndElement();
					}
					if (!string.IsNullOrEmpty(str))
					{
						xmlWriter.WriteStartElement("Property");
						xmlWriter.WriteElementString("Name", "TermSetId");
						xmlWriter.WriteStartElement("Value");
						xmlWriter.WriteAttributeString("xmlns", "q2", null, "http://www.w3.org/2001/XMLSchema");
						xmlWriter.WriteAttributeString("p4", "type", "http://www.w3.org/2001/XMLSchema-instance", "q2:string");
						xmlWriter.WriteString(str);
						xmlWriter.WriteEndElement();
						xmlWriter.WriteEndElement();
					}
					if (!string.IsNullOrEmpty(empty1))
					{
						xmlWriter.WriteStartElement("Property");
						xmlWriter.WriteElementString("Name", "AnchorId");
						xmlWriter.WriteStartElement("Value");
						xmlWriter.WriteAttributeString("xmlns", "q3", null, "http://www.w3.org/2001/XMLSchema");
						xmlWriter.WriteAttributeString("p4", "type", "http://www.w3.org/2001/XMLSchema-instance", "q3:string");
						xmlWriter.WriteString(empty1);
						xmlWriter.WriteEndElement();
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			return stringBuilder.ToString();
		}

		internal static string DetermineTargetFieldType(string sourceFieldType, bool includeTaxonomyFieldTypes)
		{
			string empty = string.Empty;
			string str = sourceFieldType;
			string str1 = str;
			if (str != null)
			{
				switch (str1)
				{
					case "Choice":
					case "Lookup":
					case "Number":
					case "DateTime":
					case "Currency":
					case "Boolean":
					case "User":
					{
						empty = "TaxonomyFieldType";
						break;
					}
					case "Text":
					case "Note":
					case "MultiChoice":
					case "LookupMulti":
					case "UserMulti":
					{
						empty = "TaxonomyFieldTypeMulti";
						break;
					}
					case "TaxonomyFieldType":
					case "TaxonomyFieldTypeMulti":
					{
						if (!includeTaxonomyFieldTypes)
						{
							break;
						}
						empty = sourceFieldType;
						break;
					}
				}
			}
			return empty;
		}

		public override void EndTransformation(PasteListAction action, SPListCollection sources, SPListCollection targets)
		{
		}

		internal static bool EnsureTermAnchorExists(ITransformer transformer, SPTermSet termSet, ManagedMetadataOption configOption, ref bool errorsOccured, ref bool warningsOccured, string sourceListAndField)
		{
			ActionOperationStatus actionOperationStatu;
			bool flag = true;
			if (string.IsNullOrEmpty(configOption.TargetAnchor))
			{
				return flag;
			}
			bool flag1 = false;
			long usage = (long)0;
			List<string> targetAnchorTerms = configOption.GetTargetAnchorTerms();
			SPTermSetItem item = termSet;
			StringBuilder stringBuilder = new StringBuilder(1024);
			try
			{
				try
				{
					foreach (string targetAnchorTerm in targetAnchorTerms)
					{
						if (item == null)
						{
							stringBuilder.AppendLine(string.Format(Resources.FS_AnchorParentTermSetItemNull, termSet.Name, configOption.TargetAnchor));
							errorsOccured = true;
						}
						if (errorsOccured)
						{
							break;
						}
						SPTerm sPTerm = item.Terms[targetAnchorTerm];
						if (sPTerm != null)
						{
							item = item.Terms[targetAnchorTerm];
						}
						else
						{
							try
							{
								SPTermCollection terms = item.Terms;
								CultureInfo currentCulture = CultureInfo.CurrentCulture;
								string fSAddedVia = Resources.FS_AddedVia;
								object[] name = new object[] { transformer.Name };
								TaxonomyOperationResult taxonomyOperationResult = terms.Add(targetAnchorTerm, string.Format(currentCulture, fSAddedVia, name));
								usage += taxonomyOperationResult.Statistics.Usage;
								sPTerm = item.Terms[targetAnchorTerm];
								if (sPTerm != null)
								{
									flag1 = true;
									stringBuilder.AppendLine(string.Format(Resources.FS_AddedAnchorTerm, targetAnchorTerm, sPTerm.ToNamePipeGuidString()));
								}
								if (taxonomyOperationResult.ErrorOccured)
								{
									errorsOccured = true;
								}
								if (taxonomyOperationResult.WarningOccured)
								{
									warningsOccured = true;
								}
								if (taxonomyOperationResult.WarningOccured || taxonomyOperationResult.ErrorOccured)
								{
									stringBuilder.AppendLine(string.Format(Resources.FS_IssueEncounteredAddingAnchorTerm, targetAnchorTerm, item.Name));
									stringBuilder.AppendLine(taxonomyOperationResult.AdapterResult);
									stringBuilder.AppendLine();
								}
								item = item.Terms[targetAnchorTerm];
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								errorsOccured = true;
								Utils.GetExceptionMessage(exception, ref stringBuilder);
							}
						}
					}
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					errorsOccured = true;
					Utils.GetExceptionMessage(exception2, ref stringBuilder);
				}
			}
			finally
			{
				flag = !errorsOccured;
				if (warningsOccured || errorsOccured || flag1)
				{
					string ensureTaxonomyTermAnchorsExist = Resources.EnsureTaxonomyTermAnchorsExist;
					string empty = string.Empty;
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					object[] targetTermstore = new object[] { configOption.TargetTermstore, configOption.TargetGroup, configOption.TargetTermSet };
					LogItem logItem = new LogItem(ensureTaxonomyTermAnchorsExist, empty, sourceListAndField, string.Format(cultureInfo, "{0}.{1}.{2}", targetTermstore), ActionOperationStatus.Running);
					transformer.FireOperationStarted(logItem);
					if (warningsOccured || errorsOccured)
					{
						logItem.Information = Resources.GenericExceptionMessage;
					}
					else
					{
						logItem.Information = Resources.SuccessReviewDetails;
					}
					logItem.LicenseDataUsed = usage;
					LogItem logItem1 = logItem;
					if (errorsOccured)
					{
						actionOperationStatu = ActionOperationStatus.Failed;
					}
					else
					{
						actionOperationStatu = (warningsOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
					}
					logItem1.Status = actionOperationStatu;
					logItem.Details = stringBuilder.ToString();
					transformer.FireOperationFinished(logItem);
				}
			}
			return flag;
		}

		internal static bool EnsureTermExists(ITransformer transformer, SPTermSetItem termSetItem, ref bool errorsOccured, ref bool warningsOccured, string targetGroup, string targetTermstore, string targetTermSet, string targetTerm, string sourceListAndField)
		{
			ActionOperationStatus actionOperationStatu;
			bool flag = true;
			string str = TransformUtils.SanitiseForTaxonomy(targetTerm);
			if (termSetItem.GetTerm(str) == null)
			{
				TaxonomyOperationResult taxonomyOperationResult = null;
				string addingTaxonomyTerm = Resources.AddingTaxonomyTerm;
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				object[] objArray = new object[] { targetTermstore, targetGroup, targetTermSet };
				LogItem logItem = new LogItem(addingTaxonomyTerm, str, sourceListAndField, string.Format(currentCulture, "{0}.{1}.{2}", objArray), ActionOperationStatus.Running);
				transformer.FireOperationStarted(logItem);
				try
				{
					try
					{
						SPTermCollection terms = termSetItem.Terms;
						CultureInfo cultureInfo = CultureInfo.CurrentCulture;
						string fSAddedVia = Resources.FS_AddedVia;
						object[] name = new object[] { transformer.Name };
						taxonomyOperationResult = terms.Add(str, string.Format(cultureInfo, fSAddedVia, name));
						logItem.LicenseDataUsed = taxonomyOperationResult.Statistics.Usage;
						if (taxonomyOperationResult.ErrorOccured)
						{
							errorsOccured = true;
						}
						if (taxonomyOperationResult.WarningOccured)
						{
							warningsOccured = true;
						}
						if (taxonomyOperationResult.WarningOccured || taxonomyOperationResult.ErrorOccured)
						{
							logItem.Details = taxonomyOperationResult.AdapterResult;
							logItem.Information = Resources.GenericExceptionMessage;
						}
						LogItem logItem1 = logItem;
						if (taxonomyOperationResult.ErrorOccured)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (taxonomyOperationResult.WarningOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem1.Status = actionOperationStatu;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						errorsOccured = true;
						logItem.Exception = exception;
						if (taxonomyOperationResult != null)
						{
							LogItem logItem2 = logItem;
							logItem2.Details = string.Concat(logItem2.Details, Environment.NewLine, Environment.NewLine, taxonomyOperationResult.AdapterResult);
						}
					}
				}
				finally
				{
					flag = !errorsOccured;
					transformer.FireOperationFinished(logItem);
				}
			}
			return flag;
		}

		internal static bool EnsureTermGroupExists(ITransformer transformer, SPTermStore termStore, ref bool errorsOccured, ref bool warningsOccured, string targetGroup, string targetTermstore, string sourceListAndField)
		{
			ActionOperationStatus actionOperationStatu;
			bool flag = true;
			string str = TransformUtils.SanitiseForTaxonomy(targetGroup);
			if (!termStore.DoesGroupExist(str))
			{
				flag = false;
				TaxonomyOperationResult taxonomyOperationResult = null;
				LogItem logItem = new LogItem(Resources.AddingTaxonomyGroup, str, sourceListAndField, targetTermstore, ActionOperationStatus.Running);
				transformer.FireOperationStarted(logItem);
				try
				{
					try
					{
						SPTermGroupCollection groups = termStore.Groups;
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string fSAddedVia = Resources.FS_AddedVia;
						object[] name = new object[] { transformer.Name };
						taxonomyOperationResult = groups.Add(str, string.Format(currentCulture, fSAddedVia, name), false);
						logItem.LicenseDataUsed = taxonomyOperationResult.Statistics.Usage;
						if (taxonomyOperationResult.ErrorOccured)
						{
							errorsOccured = true;
						}
						if (taxonomyOperationResult.WarningOccured)
						{
							warningsOccured = true;
						}
						if (taxonomyOperationResult.WarningOccured || taxonomyOperationResult.ErrorOccured)
						{
							logItem.Details = taxonomyOperationResult.AdapterResult;
							logItem.Information = Resources.GenericExceptionMessage;
						}
						LogItem logItem1 = logItem;
						if (taxonomyOperationResult.ErrorOccured)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (taxonomyOperationResult.WarningOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem1.Status = actionOperationStatu;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						errorsOccured = true;
						logItem.Exception = exception;
						if (taxonomyOperationResult != null)
						{
							LogItem logItem2 = logItem;
							logItem2.Details = string.Concat(logItem2.Details, Environment.NewLine, Environment.NewLine, taxonomyOperationResult.AdapterResult);
						}
					}
				}
				finally
				{
					flag = !errorsOccured;
					transformer.FireOperationFinished(logItem);
				}
			}
			return flag;
		}

		internal static bool EnsureTermSetExists(ITransformer transformer, SPTermGroup termGroup, ref bool errorsOccured, ref bool warningsOccured, string targetGroup, string targetTermstore, string targetTermSet, string sourceListAndField)
		{
			ActionOperationStatus actionOperationStatu;
			bool flag = true;
			string str = TransformUtils.SanitiseForTaxonomy(targetTermSet);
			if (termGroup.TermSets[str] == null)
			{
				TaxonomyOperationResult taxonomyOperationResult = null;
				string addingTaxonomyTermset = Resources.AddingTaxonomyTermset;
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				object[] objArray = new object[] { targetTermstore, targetGroup };
				LogItem logItem = new LogItem(addingTaxonomyTermset, str, sourceListAndField, string.Format(currentCulture, "{0}.{1}", objArray), ActionOperationStatus.Running);
				transformer.FireOperationStarted(logItem);
				try
				{
					try
					{
						SPTermSetCollection termSets = termGroup.TermSets;
						CultureInfo cultureInfo = CultureInfo.CurrentCulture;
						string fSAddedVia = Resources.FS_AddedVia;
						object[] name = new object[] { transformer.Name };
						taxonomyOperationResult = termSets.Add(str, string.Format(cultureInfo, fSAddedVia, name));
						logItem.LicenseDataUsed = taxonomyOperationResult.Statistics.Usage;
						if (taxonomyOperationResult.ErrorOccured)
						{
							errorsOccured = true;
						}
						if (taxonomyOperationResult.WarningOccured)
						{
							warningsOccured = true;
						}
						if (taxonomyOperationResult.WarningOccured || taxonomyOperationResult.ErrorOccured)
						{
							logItem.Details = taxonomyOperationResult.AdapterResult;
							logItem.Information = Resources.GenericExceptionMessage;
						}
						LogItem logItem1 = logItem;
						if (taxonomyOperationResult.ErrorOccured)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (taxonomyOperationResult.WarningOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem1.Status = actionOperationStatu;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						errorsOccured = true;
						logItem.Exception = exception;
						if (taxonomyOperationResult != null)
						{
							LogItem logItem2 = logItem;
							logItem2.Details = string.Concat(logItem2.Details, Environment.NewLine, Environment.NewLine, taxonomyOperationResult.AdapterResult);
						}
					}
				}
				finally
				{
					flag = !errorsOccured;
					transformer.FireOperationFinished(logItem);
				}
			}
			return flag;
		}

		internal static SPList FindLookupList(SPWeb parentWeb, string lookupListName, string lookupListWebId, bool checkLikelyScenarios)
		{
			if (parentWeb == null || string.IsNullOrEmpty(lookupListName))
			{
				return null;
			}
			if (checkLikelyScenarios && !string.IsNullOrEmpty(lookupListWebId))
			{
				if (string.Equals(parentWeb.ID, lookupListWebId, StringComparison.OrdinalIgnoreCase))
				{
					return parentWeb.Lists[lookupListName];
				}
				if (string.Equals(parentWeb.RootWebGUID, lookupListWebId, StringComparison.OrdinalIgnoreCase))
				{
					return parentWeb.RootSite.Lists[lookupListName];
				}
			}
			if (parentWeb.Lists[lookupListName] != null)
			{
				return parentWeb.Lists[lookupListName];
			}
			return FieldToManagedMetadata.FindLookupList(parentWeb.Parent as SPWeb, lookupListName, lookupListWebId, false);
		}

		internal static SPField FindSiteColumn(SPWeb parentWeb, string displayName, string internalName, bool searchAvailableColumns)
		{
			if (parentWeb == null || string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(internalName))
			{
				return null;
			}
			SPField sPField = (searchAvailableColumns ? parentWeb.AvailableColumns.GetFieldByNames(displayName, internalName) : parentWeb.SiteColumns.GetFieldByNames(displayName, internalName));
			if (sPField != null)
			{
				return sPField;
			}
			return FieldToManagedMetadata.FindSiteColumn(parentWeb.Parent as SPWeb, displayName, internalName, searchAvailableColumns);
		}

		internal static SPTermSetItem GetAnchorTerm(SPTermSet termSet, ManagedMetadataOption configOption)
		{
			if (string.IsNullOrEmpty(configOption.TargetAnchor))
			{
				return termSet;
			}
			SPTermSetItem item = termSet;
			foreach (string targetAnchorTerm in configOption.GetTargetAnchorTerms())
			{
				if (item == null)
				{
					continue;
				}
				item = item.Terms[targetAnchorTerm];
			}
			SPTermSetItem sPTermSetItem = item;
			if (sPTermSetItem == null)
			{
				sPTermSetItem = termSet;
			}
			return sPTermSetItem;
		}

		internal static bool IsChoiceFieldType(string sourceFieldType)
		{
			if (string.Equals("Choice", sourceFieldType))
			{
				return true;
			}
			return string.Equals("MultiChoice", sourceFieldType);
		}

		internal static bool IsFieldTypeMulti(string sourceFieldType)
		{
			if (string.Equals("MultiChoice", sourceFieldType) || string.Equals("LookupMulti", sourceFieldType))
			{
				return true;
			}
			return string.Equals("UserMulti", sourceFieldType);
		}

		internal static bool IsLookupFieldType(string sourceFieldType)
		{
			if (string.Equals("Lookup", sourceFieldType))
			{
				return true;
			}
			return string.Equals("LookupMulti", sourceFieldType);
		}

		internal static bool IsTaxonomyFieldType(string sourceFieldType)
		{
			if (string.Equals("TaxonomyFieldType", sourceFieldType))
			{
				return true;
			}
			return string.Equals("TaxonomyFieldTypeMulti", sourceFieldType);
		}

		internal static bool IsTextOrNoteFieldType(string sourceFieldType)
		{
			if (string.Equals("Note", sourceFieldType))
			{
				return true;
			}
			return string.Equals("Text", sourceFieldType);
		}

		internal static string MultiTypeSplitCharacters(string multiSourceFieldType)
		{
			if (string.Equals("UserMulti", multiSourceFieldType))
			{
				return ",";
			}
			return ";#";
		}

		internal static void RefreshTermstore(ITransformer transformer, SPWeb targetWeb, TransformationRepository transformationRespository)
		{
			if (transformationRespository.GetValueForKey("PropertyBag", "TermStoreRefreshed") == null && targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				transformationRespository.Add("PropertyBag", "TermStoreRefreshed", bool.TrueString);
				LogItem logItem = new LogItem(transformer.Name, Resources.RefreshingTermstores, string.Empty, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				transformer.FireOperationStarted(logItem);
				try
				{
					try
					{
						targetWeb.TermStores.FetchData();
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Warning;
					}
				}
				finally
				{
					transformer.FireOperationFinished(logItem);
				}
			}
		}

		public override SPList Transform(SPList dataObject, PasteListAction action, SPListCollection sources, SPListCollection targets)
		{
			SPList sPList;
			object[] name;
			string str;
			string empty;
			string empty1;
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
			if (targets.ParentWeb == null)
			{
				throw new ArgumentNullException("targets.ParentWeb");
			}
			if (targets.ParentWeb.Adapter == null)
			{
				throw new ArgumentNullException("targets.ParentWeb.Adapter");
			}
			List<FieldToManagedMetadataOption> fieldToManagedMetadataOptions = new List<FieldToManagedMetadataOption>();
			foreach (FieldToManagedMetadataOption item in base.Options.Items)
			{
				if (item.ListFilterExpression != null)
				{
					if (!item.ListFilterExpression.Evaluate(dataObject))
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
			string str1 = TransformationRepository.GenerateKey(typeof(SiteColumnToManagedMetadata).Name, sources.ParentWeb.RootWebGUID);
			string str2 = TransformationRepository.GenerateKey(typeof(FieldToManagedMetadata).Name, dataObject.ID);
			if (action.TransformationRepository.DoesParentKeyExist(str2))
			{
				return dataObject;
			}
			string list = dataObject.Adapter.Reader.GetList(dataObject.ID);
			OperationReportingResult operationReportingResult = new OperationReportingResult(list);
			if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
			{
				OperationReportingException operationReportingException = new OperationReportingException(string.Format("GetList - No ObjectXml [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
				throw operationReportingException;
			}
			action.TransformationRepository.Add(str2, "listXml", operationReportingResult.ObjectXml);
			LogItem logItem = null;
			bool flag = false;
			List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
			bool flag1 = false;
			bool flag2 = false;
			XmlDocument xmlDocument = null;
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
						xmlDocument.LoadXml(dataObject.XML);
						StringBuilder stringBuilder = null;
						bool flag3 = false;
						bool flag4 = false;
						if (action.SharePointOptions.Verbose)
						{
							stringBuilder = new StringBuilder(4096);
						}
						XmlDocument xmlDocument1 = new XmlDocument();
						xmlDocument1.LoadXml(action.TransformationRepository.GetValueForKey(str2, "listXml"));
						foreach (FieldToManagedMetadataOption fieldToManagedMetadataOption in fieldToManagedMetadataOptions)
						{
							if (flag1)
							{
								break;
							}
							foreach (SPField field in dataObject.Fields)
							{
								flag4 = false;
								flag3 = true;
								if (!fieldToManagedMetadataOption.ListFieldFilterExpression.Evaluate(field))
								{
									continue;
								}
								if (fieldToManagedMetadataOption.ListFilterExpression == null)
								{
									XmlElement documentElement = xmlDocument1.DocumentElement;
									CultureInfo invariantCulture = CultureInfo.InvariantCulture;
									name = new object[] { field.Name };
									XmlNode xmlNodes = documentElement.SelectSingleNode(string.Format(invariantCulture, "Fields/Field[@Name='{0}']", name));
									if (xmlNodes != null)
									{
										empty1 = (xmlNodes.Attributes["ID"] != null ? xmlNodes.Attributes["ID"].Value : string.Empty);
									}
									else
									{
										empty1 = string.Empty;
									}
									string str3 = empty1;
									flag4 = (field.FieldXML.Attributes["SourceID"] == null ? false : !string.IsNullOrEmpty(action.TransformationRepository.GetValueForKey(str1, str3)));
									flag3 = flag4;
								}
								if (!flag3)
								{
									continue;
								}
								XmlElement xmlElement = xmlDocument.DocumentElement;
								CultureInfo cultureInfo = CultureInfo.InvariantCulture;
								name = new object[] { field.Name };
								XmlNode xmlNodes1 = xmlElement.SelectSingleNode(string.Format(cultureInfo, "Fields/Field[@Name='{0}']", name));
								if (xmlNodes1 == null)
								{
									continue;
								}
								if (logItem == null)
								{
									logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
								}
								string value = xmlNodes1.Attributes["Type"].Value;
								string str4 = FieldToManagedMetadata.DetermineTargetFieldType(value, false);
								if (fieldToManagedMetadataOption.CreateNewField)
								{
									XmlElement documentElement1 = xmlDocument.DocumentElement;
									CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
									name = new object[] { fieldToManagedMetadataOption.NewFieldName };
									XmlNode xmlNodes2 = documentElement1.SelectSingleNode(string.Format(invariantCulture1, "Fields/Field[@Name='{0}']", name));
									if (xmlNodes2 == null)
									{
										XmlElement xmlElement1 = xmlDocument.DocumentElement;
										CultureInfo cultureInfo1 = CultureInfo.InvariantCulture;
										name = new object[] { fieldToManagedMetadataOption.NewFieldDisplayName };
										xmlNodes2 = xmlElement1.SelectSingleNode(string.Format(cultureInfo1, "Fields/Field[@Name='{0}']", name));
									}
									if (xmlNodes2 != null)
									{
										string name1 = this.Name;
										string transform = Resources.Transform;
										CultureInfo currentCulture = CultureInfo.CurrentCulture;
										name = new object[] { dataObject.Name, field.Name };
										LogItem logItem1 = new LogItem(name1, transform, string.Format(currentCulture, "{0}.{1}", name), fieldToManagedMetadataOption.NewFieldName, ActionOperationStatus.Warning);
										CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
										string fSNewFieldAlreadyExistsOnSource = Resources.FS_NewFieldAlreadyExistsOnSource;
										name = new object[] { fieldToManagedMetadataOption.NewFieldDisplayName, fieldToManagedMetadataOption.NewFieldName };
										logItem1.Information = string.Format(currentCulture1, fSNewFieldAlreadyExistsOnSource, name);
										CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
										string fSUnableToTransformSourceFieldDuplicate = Resources.FS_UnableToTransformSourceFieldDuplicate;
										name = new object[] { field.DisplayName, field.Name, value, fieldToManagedMetadataOption.NewFieldDisplayName, fieldToManagedMetadataOption.NewFieldName, str4, fieldToManagedMetadataOption.TargetTermstore, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermSet };
										logItem1.Details = string.Format(currentCulture2, fSUnableToTransformSourceFieldDuplicate, name);
										base.FireOperationStarted(logItem1);
										flag2 = true;
										continue;
									}
								}
								if (!string.IsNullOrEmpty(str4))
								{
									CultureInfo cultureInfo2 = CultureInfo.CurrentCulture;
									name = new object[] { dataObject.Name, field.Name };
									string str5 = string.Format(cultureInfo2, "{0}.{1}", name);
									SPTermStore sPTermStore = parentWeb.TermStores[fieldToManagedMetadataOption.TargetTermstore];
									if (sPTermStore != null)
									{
										FieldToManagedMetadata.EnsureTermGroupExists(this, sPTermStore, ref flag1, ref flag2, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermstore, str5);
										if (flag1)
										{
											break;
										}
										SPTermGroup sPTermGroup = sPTermStore.Groups[fieldToManagedMetadataOption.TargetGroup];
										FieldToManagedMetadata.EnsureTermSetExists(this, sPTermGroup, ref flag1, ref flag2, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermstore, fieldToManagedMetadataOption.TargetTermSet, str5);
										if (flag1)
										{
											break;
										}
										SPTermSet sPTermSet = sPTermGroup.TermSets[fieldToManagedMetadataOption.TargetTermSet];
										FieldToManagedMetadata.EnsureTermAnchorExists(this, sPTermSet, fieldToManagedMetadataOption, ref flag1, ref flag2, str5);
										if (flag1)
										{
											break;
										}
										SPTermSetItem anchorTerm = FieldToManagedMetadata.GetAnchorTerm(sPTermSet, fieldToManagedMetadataOption);
										IEnumerator enumerator = xmlNodes1.SelectNodes("CHOICES/CHOICE").GetEnumerator();
										try
										{
											do
											{
											Label2:
												if (!enumerator.MoveNext())
												{
													break;
												}
												XmlNode current = (XmlNode)enumerator.Current;
												if (!string.IsNullOrEmpty(current.FirstChild.Value))
												{
													string str6 = TransformUtils.SanitiseForTaxonomy(current.FirstChild.Value);
													FieldToManagedMetadata.EnsureTermExists(this, anchorTerm, ref flag1, ref flag2, fieldToManagedMetadataOption.TargetGroup, fieldToManagedMetadataOption.TargetTermstore, fieldToManagedMetadataOption.TargetTermSet, str6, str5);
												}
												else
												{
													goto Label2;
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
										string str7 = (fieldToManagedMetadataOption.CreateNewField ? fieldToManagedMetadataOption.NewFieldName : field.Name);
										XmlAttribute itemOf = xmlNodes1.Attributes["DisplayName"];
										if (itemOf != null)
										{
											str = (fieldToManagedMetadataOption.CreateNewField ? fieldToManagedMetadataOption.NewFieldDisplayName : itemOf.Value);
										}
										else
										{
											str = str7;
										}
										string str8 = str;
										XmlAttribute xmlAttribute = xmlNodes1.Attributes["SourceID"];
										string str9 = (xmlAttribute == null || !flag4 ? string.Empty : xmlAttribute.Value);
										string str10 = FieldToManagedMetadata.ConstructMMDFieldXml(str4, parentWeb.TermStores[fieldToManagedMetadataOption.TargetTermstore].Groups[fieldToManagedMetadataOption.TargetGroup].TermSets[fieldToManagedMetadataOption.TargetTermSet], str7, str8, str9, anchorTerm);
										if (action.SharePointOptions.Verbose)
										{
											StringBuilder stringBuilder1 = stringBuilder;
											CultureInfo currentCulture3 = CultureInfo.CurrentCulture;
											string fSVerboseSourceFieldTransformation = Resources.FS_VerboseSourceFieldTransformation;
											name = new object[] { field.Name, value, str7, str4, fieldToManagedMetadataOption.TargetTermstore, fieldToManagedMetadataOption.TargetGroup, null };
											object[] objArray = name;
											string targetTermSet = fieldToManagedMetadataOption.TargetTermSet;
											if (flag4)
											{
												CultureInfo cultureInfo3 = CultureInfo.CurrentCulture;
												object[] siteColumn = new object[] { Resources.SiteColumn };
												empty = string.Format(cultureInfo3, ", {0}", siteColumn);
											}
											else
											{
												empty = string.Empty;
											}
											objArray[6] = string.Concat(targetTermSet, empty);
											stringBuilder1.AppendLine(string.Format(currentCulture3, fSVerboseSourceFieldTransformation, name));
										}
										XmlDocument xmlDocument2 = new XmlDocument();
										xmlDocument2.LoadXml(str10);
										XmlNode xmlNodes3 = xmlDocument2.SelectSingleNode("//Field");
										XmlNode innerXml = xmlDocument.CreateNode(XmlNodeType.Element, "Field", string.Empty);
										innerXml.InnerXml = xmlNodes3.InnerXml;
										foreach (XmlAttribute attribute in xmlNodes3.Attributes)
										{
											XmlAttribute value1 = xmlDocument.CreateAttribute(attribute.Name);
											value1.Value = attribute.Value;
											innerXml.Attributes.Append(value1);
										}
										if (!fieldToManagedMetadataOption.CreateNewField)
										{
											XmlNode xmlNodes4 = xmlDocument.DocumentElement.SelectSingleNode("Fields");
											XmlAttribute[] xmlAttributeArray = new XmlAttribute[] { xmlNodes1.Attributes["ID"], xmlNodes1.Attributes["Required"], xmlNodes1.Attributes["FillInChoice"] };
											foreach (XmlAttribute xmlAttribute1 in new List<XmlAttribute>(xmlAttributeArray))
											{
												if (xmlAttribute1 == null || innerXml.Attributes[xmlAttribute1.Name] != null)
												{
													continue;
												}
												XmlAttribute value2 = xmlDocument.CreateAttribute(xmlAttribute1.Name);
												value2.Value = xmlAttribute1.Value;
												innerXml.Attributes.Append(value2);
											}
											xmlNodes4.ReplaceChild(innerXml, xmlNodes1);
										}
										else
										{
											if (innerXml.Attributes["DisplayName"] != null)
											{
												innerXml.Attributes["DisplayName"].Value = str8;
											}
											xmlNodes1.ParentNode.AppendChild(innerXml);
											keyValuePairs.Add(new KeyValuePair<string, string>(field.Name, str7));
										}
										flag = true;
									}
									else
									{
										LogItem logItem2 = new LogItem(this.Name, Resources.AccessingTermstore, str5, fieldToManagedMetadataOption.TargetTermstore, ActionOperationStatus.Warning);
										CultureInfo currentCulture4 = CultureInfo.CurrentCulture;
										string fSTermstoreNotFound = Resources.FS_TermstoreNotFound;
										name = new object[] { fieldToManagedMetadataOption.TargetTermstore, str5 };
										logItem2.Information = string.Format(currentCulture4, fSTermstoreNotFound, name);
										base.FireOperationStarted(logItem2);
										flag2 = true;
									}
								}
								else
								{
									string name2 = this.Name;
									string transform1 = Resources.Transform;
									CultureInfo cultureInfo4 = CultureInfo.CurrentCulture;
									name = new object[] { dataObject.Name, field.Name };
									LogItem logItem3 = new LogItem(name2, transform1, string.Format(cultureInfo4, "{0}.{1}", name), fieldToManagedMetadataOption.NewFieldName, ActionOperationStatus.Warning);
									CultureInfo currentCulture5 = CultureInfo.CurrentCulture;
									string fSSourceFieldTypeNotSupported = Resources.FS_SourceFieldTypeNotSupported;
									name = new object[] { value };
									logItem3.Information = string.Format(currentCulture5, fSSourceFieldTypeNotSupported, name);
									base.FireOperationStarted(logItem3);
								}
							}
						}
						if (!flag1 && xmlDocument != null && flag)
						{
							if (keyValuePairs.Count > 0)
							{
								Dictionary<string, string> valuesForKey = action.TransformationRepository.GetValuesForKey(str1);
								bool flag5 = true;
								foreach (SPContentType contentType in dataObject.ContentTypes)
								{
									bool flag6 = false;
									XmlDocument xmlDocument3 = new XmlDocument();
									xmlDocument3.LoadXml(contentType.XML);
									foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
									{
										flag5 = false;
										XmlElement documentElement2 = xmlDocument3.DocumentElement;
										CultureInfo invariantCulture2 = CultureInfo.InvariantCulture;
										name = new object[] { keyValuePair.Key };
										XmlNode xmlNodes5 = documentElement2.SelectSingleNode(string.Format(invariantCulture2, "FieldRefs/FieldRef[@Name='{0}']", name));
										if (xmlNodes5 == null)
										{
											continue;
										}
										if ((valuesForKey == null || xmlNodes5.Attributes["Id"] == null ? false : valuesForKey.ContainsKey(xmlNodes5.Attributes["Id"].Value)))
										{
											continue;
										}
										XmlElement xmlElement2 = xmlDocument3.DocumentElement;
										CultureInfo invariantCulture3 = CultureInfo.InvariantCulture;
										name = new object[] { keyValuePair.Value };
										if (xmlElement2.SelectSingleNode(string.Format(invariantCulture3, "FieldRefs/FieldRef[@Name='{0}']", name)) != null)
										{
											continue;
										}
										XmlNode value3 = xmlNodes5.Clone();
										if (value3.Attributes["Id"] != null)
										{
											value3.Attributes["Id"].Value = Guid.Empty.ToString();
										}
										value3.Attributes["Name"].Value = keyValuePair.Value;
										XmlNode xmlNodes6 = xmlDocument3.DocumentElement.SelectSingleNode("FieldRefs");
										xmlNodes6.AppendChild(value3);
										flag6 = true;
									}
									if (!flag6)
									{
										continue;
									}
									contentType.FromXML(xmlDocument3.DocumentElement);
								}
								XmlNode xmlNodes7 = xmlDocument.DocumentElement.SelectSingleNode("Views");
								if (xmlNodes7 != null)
								{
									foreach (XmlNode childNode in xmlNodes7.ChildNodes)
									{
										foreach (KeyValuePair<string, string> keyValuePair1 in keyValuePairs)
										{
											CultureInfo invariantCulture4 = CultureInfo.InvariantCulture;
											name = new object[] { keyValuePair1.Key };
											XmlNode xmlNodes8 = childNode.SelectSingleNode(string.Format(invariantCulture4, "ViewFields/FieldRef[@Name='{0}']", name));
											if (xmlNodes8 == null)
											{
												continue;
											}
											CultureInfo invariantCulture5 = CultureInfo.InvariantCulture;
											name = new object[] { keyValuePair1.Value };
											if (childNode.SelectSingleNode(string.Format(invariantCulture5, "ViewFields/FieldRef[@Name='{0}']", name)) != null)
											{
												continue;
											}
											XmlNode value4 = xmlNodes8.Clone();
											value4.Attributes["Name"].Value = keyValuePair1.Value;
											childNode.SelectSingleNode("ViewFields").AppendChild(value4);
										}
									}
								}
							}
							dataObject.UpdateList(xmlDocument.FirstChild.OuterXml, true, true);
							if (action.SharePointOptions.Verbose && stringBuilder.Length > 0)
							{
								if (logItem == null)
								{
									logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
								}
								logItem.Information = (flag2 ? Resources.PartialSuccessReviewDetails : Resources.SuccessReviewDetails);
								StringBuilder stringBuilder2 = stringBuilder;
								CultureInfo cultureInfo5 = CultureInfo.CurrentCulture;
								name = new object[] { Resources.Transform, null, null, null };
								name[1] = (dataObject.IsDocumentLibrary ? Resources.Library : Resources.List);
								name[2] = dataObject.Name;
								name[3] = Environment.NewLine;
								stringBuilder2.Insert(0, string.Format(cultureInfo5, "{0} {1} '{2}':{3}", name));
								logItem.Details = stringBuilder.ToString();
							}
						}
					}
					else
					{
						logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running)
						{
							Information = Resources.TargetMustBeSharePoint2010,
							Status = ActionOperationStatus.Warning
						};
						base.FireOperationStarted(logItem);
						sPList = dataObject;
						return sPList;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					flag1 = true;
					if (logItem == null)
					{
						logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
					}
					logItem.Exception = exception;
				}
				if (!flag1)
				{
					return dataObject;
				}
				return null;
			}
			finally
			{
				if (logItem == null && (flag1 || flag2))
				{
					logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
				}
				if (logItem != null)
				{
					if (logItem.Status == ActionOperationStatus.Running)
					{
						LogItem logItem4 = logItem;
						if (flag1)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (flag2 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem4.Status = actionOperationStatu;
					}
					base.FireOperationFinished(logItem);
				}
			}
			return sPList;
		}
	}
}