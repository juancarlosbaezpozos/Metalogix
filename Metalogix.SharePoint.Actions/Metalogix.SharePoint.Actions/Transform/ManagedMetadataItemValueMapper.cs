using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.Taxonomy.Generic;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ManagedMetadataItemValueMapper : PreconfiguredTransformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>
	{
		private const string EnterpriseKeywordFieldId = "23f27201-bee3-471e-b2e7-b64fd8b7ca38";

		public override string Name
		{
			get
			{
				return "Managed Metadata Item Value Mapper";
			}
		}

		public ManagedMetadataItemValueMapper()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (action.ActionOptions.CopyReferencedManagedMetadata && action.ActionOptions.ResolveManagedMetadataByName)
			{
				action.EnsureManagedMetadataExistence(sources.ParentSPList.FieldCollection, sources.ParentSPList.ParentWeb, targets.ParentSPList.ParentWeb, true);
			}
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		private void MapItemValue(XmlAttribute attr, ref StringBuilder sb, ref Dictionary<string, List<string>> guidsNotFound, TransformationRepository transformationRepository, string originalTermsetId, ref bool itemXmlChanged, Dictionary<Guid, Guid> guidMappings, bool sourceSupportsTaxonomy)
		{
			MatchCollection matchCollections = Regex.Matches(attr.Value, "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})");
			if (matchCollections.Count > 0)
			{
				itemXmlChanged = true;
				if (sb == null)
				{
					sb = new StringBuilder(1024);
				}
				sb.Length = 0;
				sb.Append(attr.Value);
				foreach (Match match in matchCollections)
				{
					string str = match.Value.TrimStart(new char[] { '|' });
					string str1 = null;
					if (sourceSupportsTaxonomy)
					{
						str1 = (originalTermsetId == null ? transformationRepository.GetFirstValueForKey(str.ToLower()) : transformationRepository.GetValueForKey(originalTermsetId, str.ToLower()));
					}
					if (str1 == null)
					{
						Guid guid = new Guid(str);
						if (guidMappings.ContainsKey(guid))
						{
							str1 = guidMappings[guid].ToString("D");
						}
					}
					if (str1 == null)
					{
						if (!sourceSupportsTaxonomy || AdapterConfigurationVariables.UseExistingTargetTerms)
						{
							continue;
						}
						if (guidsNotFound == null)
						{
							guidsNotFound = new Dictionary<string, List<string>>();
						}
						if (!guidsNotFound.ContainsKey(attr.Name))
						{
							guidsNotFound.Add(attr.Name, new List<string>());
						}
						if (guidsNotFound[attr.Name].Contains(str))
						{
							continue;
						}
						guidsNotFound[attr.Name].Add(str);
					}
					else
					{
						sb.Replace(str, str1);
					}
				}
				attr.Value = sb.ToString();
			}
		}

		private void RemoveInvalidValues(XmlAttribute attr, IList<string> invalidGuids)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			string value = attr.Value;
			char[] chrArray = new char[] { ';' };
			string[] empty = value.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)empty.Length; i++)
			{
				foreach (string invalidGuid in invalidGuids)
				{
					if (empty[i].IndexOf(invalidGuid, StringComparison.OrdinalIgnoreCase) == -1)
					{
						continue;
					}
					empty[i] = string.Empty;
					break;
				}
				if (!string.IsNullOrEmpty(empty[i]))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(empty[i]);
				}
			}
			attr.Value = stringBuilder.ToString();
		}

		private void ResolveEnterpriseKeywordTerms(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets, Dictionary<string, List<string>> guidsNotFound, XmlNode itemXML, string enterpriseKeyName)
		{
			Guid termstoreId;
			List<string> strs = new List<string>();
			List<string> item = guidsNotFound[enterpriseKeyName];
			SPField fieldByName = sources.ParentSPList.FieldCollection.GetFieldByName(enterpriseKeyName);
			SPField sPField = targets.ParentSPList.FieldCollection.GetFieldByName(enterpriseKeyName);
			SPTermStore sPTermStore = null;
			SPTermStore item1 = null;
			if (fieldByName == null || sPField == null)
			{
				return;
			}
			if (fieldByName.TermstoreId != sPField.TermstoreId)
			{
				termstoreId = fieldByName.TermstoreId;
			}
			else
			{
				TransformationRepository transformationRepository = action.TransformationRepository;
				Guid guid = sPField.TermstoreId;
				string valueForKey = transformationRepository.GetValueForKey("$TSPKR$", guid.ToString());
				termstoreId = new Guid(valueForKey);
			}
			sPTermStore = sources.ParentSPList.ParentWeb.TermStores[termstoreId];
			item1 = targets.ParentSPList.ParentWeb.TermStores[sPField.TermstoreId];
			if (sPTermStore == null || item1 == null)
			{
				return;
			}
			foreach (string str in item)
			{
				SPTerm term = sPTermStore.GetTerm(new Guid(str));
				if (term == null)
				{
					continue;
				}
				IList<string> termHierarchy = sPTermStore.GetTermHierarchy(term.TermSet.Id, term.Id);
				if (termHierarchy.Count <= 0)
				{
					continue;
				}
				bool flag = false;
				bool flag1 = false;
				bool flag2 = FieldToManagedMetadata.EnsureTermGroupExists(this, item1, ref flag, ref flag1, termHierarchy[0], item1.Name, enterpriseKeyName);
				SPTermGroup sPTermGroup = null;
				if (flag2)
				{
					sPTermGroup = item1.Groups[termHierarchy[0]];
					flag2 = FieldToManagedMetadata.EnsureTermSetExists(this, sPTermGroup, ref flag, ref flag1, sPTermGroup.Name, item1.Name, termHierarchy[1], enterpriseKeyName);
				}
				if (!flag2)
				{
					continue;
				}
				SPTermSetItem sPTermSetItem = sPTermGroup.TermSets[termHierarchy[1]];
				for (int i = 2; i < termHierarchy.Count; i++)
				{
					flag2 = FieldToManagedMetadata.EnsureTermExists(this, sPTermSetItem, ref flag, ref flag1, sPTermGroup.Name, item1.Name, sPTermSetItem.Name, termHierarchy[i], enterpriseKeyName);
					if (flag2)
					{
						sPTermSetItem = sPTermSetItem.Terms[termHierarchy[i]];
					}
				}
				XmlAttribute itemOf = itemXML.Attributes[enterpriseKeyName];
				if (itemOf == null || sPTermSetItem == null)
				{
					continue;
				}
				string value = itemOf.Value;
				Guid id = sPTermSetItem.Id;
				itemOf.Value = value.Replace(str, id.ToString());
				strs.Add(str);
				TransformationRepository transformationRepository1 = action.TransformationRepository;
				string str1 = fieldByName.TermSetId.ToString("D");
				Guid id1 = sPTermSetItem.Id;
				transformationRepository1.Add(str1, str, id1.ToString("D"));
			}
			item.RemoveAll(new Predicate<string>(strs.Contains));
			if (item.Count == 0)
			{
				guidsNotFound.Remove(enterpriseKeyName);
			}
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (!action.ActionOptions.ResolveManagedMetadataByName && sources.ParentSPList.Adapter.SupportsTaxonomy && (action.GuidMappings == null || action.GuidMappings.Count == 0))
			{
				return dataObject;
			}
			if (!dataObject.ParentList.FieldCollection.TaxonomyFieldsExist)
			{
				return dataObject;
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
			StringBuilder stringBuilder = null;
			Dictionary<string, List<string>> strs = null;
			string str = null;
			bool flag = false;
			List<XmlAttribute> xmlAttributes = new List<XmlAttribute>();
			foreach (SPField taxonomyField in dataObject.ParentList.FieldCollection.GetTaxonomyFields())
			{
				string valueForKey = null;
				if (sources.ParentSPList.Adapter.SupportsTaxonomy)
				{
					TransformationRepository transformationRepository = action.TransformationRepository;
					Guid termSetId = taxonomyField.TermSetId;
					valueForKey = transformationRepository.GetValueForKey("$TSPKR$", termSetId.ToString("D"));
					if (valueForKey == null)
					{
						TransformationRepository transformationRepository1 = action.TransformationRepository;
						string lower = taxonomyField.TermSetId.ToString("D").ToLower();
						Guid guid = taxonomyField.TermSetId;
						string valueForKey1 = transformationRepository1.GetValueForKey(lower, guid.ToString("D").ToLower());
						if (valueForKey1 != null)
						{
							valueForKey = action.TransformationRepository.GetValueForKey("$TSPKR$", valueForKey1);
						}
					}
				}
				SPField fieldById = dataObject.ParentList.FieldCollection.GetFieldById(taxonomyField.TaxonomyHiddenTextField);
				XmlAttribute itemOf = xmlNode.Attributes[taxonomyField.Name];
				if (itemOf != null)
				{
					this.MapItemValue(itemOf, ref stringBuilder, ref strs, action.TransformationRepository, valueForKey, ref flag, action.GuidMappings, sources.ParentSPList.Adapter.SupportsTaxonomy);
				}
				if (fieldById == null || itemOf == null)
				{
					continue;
				}
				XmlAttribute xmlAttribute = xmlNode.Attributes[fieldById.Name];
				if (xmlAttribute == null)
				{
					continue;
				}
				xmlAttributes.Add(xmlAttribute);
				flag = true;
			}
			if (strs != null && action.ActionOptions.ResolveManagedMetadataByName)
			{
				SPField sPField = sources.ParentSPList.FieldCollection.GetFieldById(new Guid("23f27201-bee3-471e-b2e7-b64fd8b7ca38"));
				if (sPField != null && strs.ContainsKey(sPField.Name))
				{
					this.ResolveEnterpriseKeywordTerms(action, sources, targets, strs, xmlNode, sPField.Name);
				}
			}
			XmlAttribute itemOf1 = xmlNode.Attributes["MetaInfo"];
			if (itemOf1 != null && itemOf1.Value.Contains("|") && Regex.IsMatch(itemOf1.Value, "\\|[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})"))
			{
				this.MapItemValue(itemOf1, ref stringBuilder, ref strs, action.TransformationRepository, null, ref flag, action.GuidMappings, sources.ParentSPList.Adapter.SupportsTaxonomy);
			}
			if (xmlAttributes.Count == 0)
			{
				string str1 = TransformationRepository.GenerateKey("RemovedNoteFields", dataObject.ParentList.ID);
				Dictionary<string, string> valuesForKey = action.TransformationRepository.GetValuesForKey(str1);
				if (valuesForKey != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in valuesForKey)
					{
						if (keyValuePair.Value.Equals("TaxCatchAll", StringComparison.OrdinalIgnoreCase) || keyValuePair.Value.Equals("TaxCatchAllLabel", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						XmlAttribute xmlAttribute1 = xmlNode.Attributes[keyValuePair.Value];
						if (xmlAttribute1 == null || xmlAttributes.Contains(xmlAttribute1))
						{
							continue;
						}
						xmlAttributes.Add(xmlAttribute1);
					}
				}
			}
			foreach (XmlAttribute xmlAttribute2 in xmlAttributes)
			{
				xmlNode.Attributes.Remove(xmlAttribute2);
			}
			if (strs != null && strs.Count > 0)
			{
				str = (string.IsNullOrEmpty(dataObject.VersionString) ? string.Empty : string.Format(" [v {0}]", dataObject.VersionString));
				LogItem logItem = new LogItem(this.Name, Resources.Transform, string.Format("{0}{1}", dataObject.Name, str), string.Empty, ActionOperationStatus.Warning);
				base.FireOperationStarted(logItem);
				logItem.Information = Resources.CertainManagedMetadataTermGuidNotFound;
				StringBuilder stringBuilder1 = new StringBuilder(512);
				stringBuilder1.AppendLine(string.Format(Resources.FS_ManagedMetadataPleaseCheckVersionsOfItems, Environment.NewLine));
				foreach (KeyValuePair<string, List<string>> keyValuePair1 in strs)
				{
					stringBuilder1.AppendLine();
					stringBuilder1.AppendLine(string.Format(Resources.FS_ManagedMetadataItemMapFieldLabel, keyValuePair1.Key));
					stringBuilder1.Append(Resources.ManagedMetadataItemMapTermGuidLabel);
					foreach (string value in keyValuePair1.Value)
					{
						stringBuilder1.Append(string.Format("'{0}',", value));
					}
					StringBuilder length = stringBuilder1;
					length.Length = length.Length - 1;
					stringBuilder1.AppendLine();
					stringBuilder1.AppendLine(string.Format(Resources.FS_ManagedMetadataItemMapValueXmlLabel, xmlNode.Attributes[keyValuePair1.Key].Value));
					this.RemoveInvalidValues(xmlNode.Attributes[keyValuePair1.Key], keyValuePair1.Value);
				}
				logItem.Details = stringBuilder1.ToString();
				base.FireOperationFinished(logItem);
				strs.Clear();
				stringBuilder1.Length = 0;
			}
			if (flag)
			{
				dataObject.SetFullXML(xmlNode);
			}
			return dataObject;
		}
	}
}