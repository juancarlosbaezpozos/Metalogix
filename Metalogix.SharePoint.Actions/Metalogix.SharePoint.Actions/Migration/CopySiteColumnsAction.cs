using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Metalogix.Actions.AllowsSameSourceTarget(false)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.SiteColumn.ico")]
	[MenuText("3:Paste Site Objects {0-Paste} > Site Columns...")]
	[Name("Copy Site Columns")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class CopySiteColumnsAction : PasteAction<PasteSiteColumnsOptions>
	{
		private TransformerDefinition<SPField, CopySiteColumnsAction, SPFieldCollection, SPFieldCollection> siteColumnTransformerDefinition = new TransformerDefinition<SPField, CopySiteColumnsAction, SPFieldCollection, SPFieldCollection>("SharePoint Site Columns", false);

		internal TransformerDefinition<SPField, CopySiteColumnsAction, SPFieldCollection, SPFieldCollection> SiteColumnTransformerDefinition
		{
			get
			{
				return this.siteColumnTransformerDefinition;
			}
		}

		public CopySiteColumnsAction()
		{
		}

		public void CopySiteColumns(SPFieldCollection siteColumns, IFilterExpression SiteFieldsFilter, SPFieldCollection targetSiteColumns, CommonSerializableTable<string, string> termMappingDictionary, bool bCopySilently)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			if (siteColumns == null || siteColumns.Count <= 0)
			{
				return;
			}
			LogItem logItem = null;
			if (!bCopySilently)
			{
				logItem = new LogItem("Copying Site Columns", siteColumns.ParentWeb.Name, siteColumns.ParentWeb.DisplayUrl, targetSiteColumns.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
			}
			try
			{
				if (this.ActionOptions.CopyReferencedManagedMetadata)
				{
					base.EnsureManagedMetadataExistence(siteColumns, siteColumns.ParentWeb, targetSiteColumns.ParentWeb, this.ActionOptions.ResolveManagedMetadataByName);
				}
				this.siteColumnTransformerDefinition.BeginTransformation(this, siteColumns, targetSiteColumns, this.Options.Transformers);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Fields"));
				SPFieldCollection sPFieldCollections = new SPFieldCollection(siteColumns.ParentWeb, xmlDocument.FirstChild);
				List<SPField> sPFields = new List<SPField>();
				foreach (SPField siteColumn in siteColumns)
				{
					SPField sPField = this.siteColumnTransformerDefinition.Transform(siteColumn, this, siteColumns, targetSiteColumns, this.Options.Transformers);
					if (sPField != null)
					{
						sPFieldCollections.AddField(sPField.FieldXML.OuterXml);
					}
					else
					{
						sPFields.Add(siteColumn);
					}
				}
				bool flag = (targetSiteColumns.ParentWeb.PublishingInfrastructureActive ? false : siteColumns.ParentWeb.PublishingInfrastructureActive);
				XmlNode filteredFieldXML = sPFieldCollections.GetFilteredFieldXML(SiteFieldsFilter, flag);
				if (sPFields.Count > 0)
				{
					foreach (SPField sPField1 in sPFields)
					{
						string str = string.Format("//Field[@ID='{0}' or @Name='{1}']", sPField1.FieldXML.GetAttributeValueAsString("ID"), sPField1.FieldXML.GetAttributeValueAsString("Name"));
						XmlNode xmlNodes = filteredFieldXML.SelectSingleNode(str);
						if (xmlNodes == null || xmlNodes.ParentNode == null)
						{
							continue;
						}
						xmlNodes.ParentNode.RemoveChild(xmlNodes);
					}
				}
				List<XmlNode> xmlNodes1 = new List<XmlNode>();
				foreach (XmlNode xmlNodes2 in filteredFieldXML.SelectNodes("./Field[@SourceID and not(starts-with(@SourceID, 'http://'))]"))
				{
					string attributeValue = XmlUtility.GetAttributeValue(xmlNodes2, "DisplayName");
					string attributeValue1 = XmlUtility.GetAttributeValue(xmlNodes2, "Name");
					SPField fieldByNames = targetSiteColumns.GetFieldByNames(attributeValue, attributeValue1);
					if (fieldByNames != null && !string.Equals(fieldByNames.Name, attributeValue1, StringComparison.OrdinalIgnoreCase))
					{
						string str1 = Metalogix.Transformers.TransformationRepository.GenerateKey(typeof(SiteColumnToManagedMetadata).Name, siteColumns.ParentWeb.RootWebGUID);
						Dictionary<string, string> valuesForKey = base.TransformationRepository.GetValuesForKey(str1);
						if (valuesForKey != null && valuesForKey.ContainsValue(string.Format("{0};#{1}", attributeValue1, attributeValue)))
						{
							fieldByNames = targetSiteColumns.GetFieldByName(attributeValue1);
						}
					}
					XmlAttribute itemOf = xmlNodes2.Attributes["ID"];
					if (itemOf != null)
					{
						Guid guid = new Guid(itemOf.Value);
						if (fieldByNames != null)
						{
							XmlAttribute xmlAttribute = fieldByNames.FieldXML.Attributes["ID"];
							if (xmlAttribute != null)
							{
								base.AddGuidMappings(guid, new Guid(xmlAttribute.Value));
							}
							xmlNodes2.Attributes.Remove(itemOf);
						}
						else
						{
							Guid guid1 = Guid.NewGuid();
							itemOf.Value = string.Concat("{", guid1.ToString(), "}");
							base.AddGuidMappings(guid, guid1);
						}
					}
					if (xmlNodes2.Attributes["FieldRef"] != null)
					{
						xmlNodes1.Add(xmlNodes2);
					}
					XmlAttribute value = xmlNodes2.Attributes["SourceID"];
					if (fieldByNames == null)
					{
						if (xmlNodes2.Attributes["Type"] != null && (xmlNodes2.Attributes["Type"].Value == "Lookup" || xmlNodes2.Attributes["Type"].Value == "LookupMulti"))
						{
							continue;
						}
						xmlNodes2.Attributes.Remove(value);
					}
					else
					{
						XmlAttribute itemOf1 = fieldByNames.FieldXML.Attributes["SourceID"];
						if (itemOf1 == null)
						{
							continue;
						}
						value.Value = itemOf1.Value;
					}
				}
				foreach (XmlNode xmlNode in xmlNodes1)
				{
					if (xmlNode.Attributes != null)
					{
						Guid guid2 = new Guid(xmlNode.Attributes["FieldRef"].Value);
						if (base.GuidMappings.ContainsKey(guid2))
						{
							xmlNode.Attributes["FieldRef"].Value = string.Concat("{", base.GuidMappings[guid2], "}");
						}
						else
						{
							string nodeIdentifyerString = CopySiteColumnsAction.GetNodeIdentifyerString(xmlNode);
							Trace.WriteLine(string.Concat("Removing FieldRef attribute in method CopySiteColumns", (!string.IsNullOrEmpty(nodeIdentifyerString) ? string.Concat(" for field ", nodeIdentifyerString) : " for a field with an unknown identity")));
							xmlNode.Attributes.Remove(xmlNode.Attributes["FieldRef"]);
							Trace.WriteLine(string.Concat("FieldRef attribute in method CopySiteColumns", (!string.IsNullOrEmpty(nodeIdentifyerString) ? string.Concat(" for field ", nodeIdentifyerString) : " has been removed.")));
						}
					}
					else
					{
						Trace.WriteLine("Field attributes in method CopySiteColumns is null.");
					}
				}
				this.ProcessingUserColumn(siteColumns, targetSiteColumns, filteredFieldXML);
				Dictionary<Guid, Guid> guidMappings = base.GuidMappings;
				string rootWebGUID = targetSiteColumns.ParentWeb.RootWebGUID;
				string taxonomyListGUID = targetSiteColumns.ParentWeb.TaxonomyListGUID;
				CommonSerializableTable<string, string> termstoreNameMappingTable = this.ActionOptions.TermstoreNameMappingTable;
				Guid guid3 = new Guid();
				SPFieldCollection.MapFieldXmlGuids(filteredFieldXML, guidMappings, rootWebGUID, taxonomyListGUID, termstoreNameMappingTable, guid3, base.TransformationRepository, this.ActionOptions.ResolveManagedMetadataByName, false, null);
				targetSiteColumns.AddFieldCollection(filteredFieldXML.OuterXml);
				if (flag)
				{
					LogItem logItem1 = new LogItem("Publishing Infrastructure Columns Skipped", siteColumns.ParentWeb.Name, siteColumns.ParentWeb.DisplayUrl, targetSiteColumns.ParentWeb.DisplayUrl, ActionOperationStatus.Warning)
					{
						Information = "SharePoint's publishing infrastructure was enabled on the source but not the target. Publishing infrastructure specific site colums were excluded from the copy so that the feature can still be activated on the target."
					};
					base.FireOperationStarted(logItem1);
				}
				if (logItem != null)
				{
					logItem.Status = ActionOperationStatus.Completed;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (logItem != null)
				{
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
				}
			}
			if (logItem != null)
			{
				base.FireOperationFinished(logItem);
			}
		}

		private static string GetNodeIdentifyerString(XmlNode field)
		{
			string empty = string.Empty;
			if (field.Attributes["Name"] != null)
			{
				empty = field.Attributes["Name"].Value;
				if (!string.IsNullOrEmpty(empty))
				{
					empty = string.Concat("Name", empty);
				}
				else if (field.Attributes["ID"] != null)
				{
					empty = string.Concat("ID", field.Attributes["ID"].Value);
				}
			}
			return empty;
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(this.siteColumnTransformerDefinition);
			return supportedDefinitions;
		}

		private void ProcessingUserColumn(SPFieldCollection siteColumns, SPFieldCollection targetSiteColumns, XmlNode xmlSource)
		{
			LogItem logItem = new LogItem("Processing User Column", siteColumns.ParentWeb.Name, siteColumns.ParentWeb.DisplayUrl, targetSiteColumns.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
			try
			{
				string str = MigrationUtils.ProcessUserFieldsForCopy(xmlSource, siteColumns.ParentWeb, targetSiteColumns.ParentWeb);
				if (!string.IsNullOrEmpty(str))
				{
					base.FireOperationStarted(logItem);
					logItem.Information = Resources.PleaseReviewDetails;
					logItem.Details = str;
					logItem.Status = ActionOperationStatus.Warning;
					base.FireOperationFinished(logItem);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.FireOperationStarted(logItem);
				logItem.Status = ActionOperationStatus.Failed;
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			PasteSiteColumnsOptions options = this.Options as PasteSiteColumnsOptions;
			SPFieldCollection siteColumns = (source[0] as SPWeb).SiteColumns;
			IFilterExpression siteFieldsFilterExpression = options.SiteFieldsFilterExpression;
			SPFieldCollection sPFieldCollections = (target[0] as SPWeb).SiteColumns;
			this.CopySiteColumns(siteColumns, siteFieldsFilterExpression, sPFieldCollections, options.TermstoreNameMappingTable, false);
		}

		protected override void RunOperation(object[] oParams)
		{
			SPFieldCollection sPFieldCollections = oParams[0] as SPFieldCollection;
			IFilterExpression filterExpression = oParams[1] as IFilterExpression;
			SPFieldCollection sPFieldCollections1 = oParams[2] as SPFieldCollection;
			CommonSerializableTable<string, string> commonSerializableTable = oParams[3] as CommonSerializableTable<string, string>;
			bool flag = (bool)oParams[4];
			this.CopySiteColumns(sPFieldCollections, filterExpression, sPFieldCollections1, commonSerializableTable, flag);
		}
	}
}