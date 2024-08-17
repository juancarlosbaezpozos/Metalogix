using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.Transformers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ContentTypeToManagedMetadata : Transformer<SPContentType, CopyContentTypesAction, SPContentTypeCollection, SPContentTypeCollection, SiteColumnToManagedMetadataOptionCollection>
	{
		private const string C_XPATH_FIELDREFS_SELECT = "FieldRefs/FieldRef[@Id='{0}']";

		public override string Name
		{
			get
			{
				return "Content Type To Managed Metadata";
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

		public ContentTypeToManagedMetadata()
		{
		}

		public override void BeginTransformation(CopyContentTypesAction action, SPContentTypeCollection sources, SPContentTypeCollection targets)
		{
		}

		public override void EndTransformation(CopyContentTypesAction action, SPContentTypeCollection sources, SPContentTypeCollection targets)
		{
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
			string str = TransformationRepository.GenerateKey(typeof(SiteColumnToManagedMetadata).Name, sources.ParentWeb.RootWebGUID);
			if (!action.TransformationRepository.DoesParentKeyExist(str))
			{
				return dataObject;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(dataObject.XML);
			StringBuilder stringBuilder = null;
			if (action.SharePointOptions.Verbose)
			{
				stringBuilder = new StringBuilder(4096);
			}
			Dictionary<string, string> valuesForKey = action.TransformationRepository.GetValuesForKey(str);
			bool flag = false;
			foreach (KeyValuePair<string, string> keyValuePair in valuesForKey)
			{
				XmlElement documentElement = xmlDocument.DocumentElement;
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] objArray = new object[1];
				string key = keyValuePair.Key;
				char[] chrArray = new char[] { '{', '}' };
				objArray[0] = key.Trim(chrArray);
				XmlNode xmlNodes = documentElement.SelectSingleNode(string.Format(invariantCulture, "FieldRefs/FieldRef[@Id='{0}']", objArray));
				if (xmlNodes == null)
				{
					XmlElement xmlElement = xmlDocument.DocumentElement;
					CultureInfo cultureInfo = CultureInfo.InvariantCulture;
					object[] key1 = new object[] { keyValuePair.Key };
					xmlNodes = xmlElement.SelectSingleNode(string.Format(cultureInfo, "FieldRefs/FieldRef[@Id='{0}']", key1));
				}
				if (xmlNodes == null)
				{
					continue;
				}
				XmlAttribute itemOf = xmlNodes.Attributes["Name"];
				if (itemOf == null)
				{
					continue;
				}
				string value = keyValuePair.Value;
				string[] strArrays = new string[] { ";#" };
				string[] strArrays1 = value.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
				if ((int)strArrays1.Length != 2)
				{
					continue;
				}
				string str1 = strArrays1[0];
				string str2 = strArrays1[1];
				if (string.Equals(itemOf.Value, str1, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				SPField sPField = FieldToManagedMetadata.FindSiteColumn(targets.ParentWeb, str2, str1, false) ?? FieldToManagedMetadata.FindSiteColumn(targets.ParentWeb, str2, str1, true);
				if (sPField == null)
				{
					continue;
				}
				XmlNode name = xmlNodes.CloneNode(true);
				name.Attributes["Id"].Value = sPField.ID.ToString();
				name.Attributes["Name"].Value = sPField.Name;
				xmlNodes.ParentNode.AppendChild(name);
				flag = true;
				if (!action.SharePointOptions.Verbose)
				{
					continue;
				}
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string fSVerboseNewFieldAddedToContentType = Resources.FS_VerboseNewFieldAddedToContentType;
				object[] value1 = new object[] { str2, str1, itemOf.Value };
				stringBuilder.AppendLine(string.Format(currentCulture, fSVerboseNewFieldAddedToContentType, value1));
			}
			if (flag)
			{
				dataObject = sources.AddOrUpdateContentType(dataObject.Name, xmlDocument.OuterXml, dataObject.ParentContentType.Name);
			}
			if (action.SharePointOptions.Verbose && stringBuilder.Length > 0)
			{
				LogItem logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Completed)
				{
					Information = Resources.SuccessReviewDetails
				};
				CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
				object[] transform = new object[] { Resources.Transform, Resources.ContentType, dataObject.Name, Environment.NewLine };
				stringBuilder.Insert(0, string.Format(currentCulture1, "{0} {1} '{2}':{3}", transform));
				logItem.Details = stringBuilder.ToString();
				base.FireOperationStarted(logItem);
			}
			return dataObject;
		}
	}
}