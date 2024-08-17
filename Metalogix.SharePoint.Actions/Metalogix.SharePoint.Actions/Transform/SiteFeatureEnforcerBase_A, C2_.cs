using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public abstract class SiteFeatureEnforcerBase<A, C2> : PreconfiguredTransformer<SPWeb, A, SPWebCollection, C2>
	where A : Metalogix.Actions.Action
	where C2 : NodeCollection
	{
		private const string COLLECT_SIGNATURE_FEATURE_ID = "6c09612b-46af-4b2f-8dfc-59185c962a29";

		private const string ROUTING_WORKFLOW_FEATURE_ID = "02464c6a-9d07-4f30-ba04-e9035cf54392";

		private const string SHAREPOINT_2007_WORKFLOWS_FEATURE_ID = "c845ed8d-9ce5-448c-bd3e-ea71350ce45b";

		private readonly static string[] s_2007SharePointServerStandardSiteFeatures;

		private readonly static string[] s_2007SharePointServerEnterpriseSitefeatures;

		private readonly static string[] s_2007TeamColaborationListsFeatures;

		private readonly static string[] s_2010SharePointServerStandardSiteFeatures;

		private readonly static string[] s_2013SharePointServerStandardSiteFeatures;

		private readonly static string[] s_O365SharePointServerStandardSiteFeatures;

		static SiteFeatureEnforcerBase()
		{
			string[] strArrays = new string[] { "e8734bb6-be8e-48a1-b036-5a40ff0b8a81", "56dd7fe7-a155-4283-b5e6-6147560601ee", "0be49fe9-9bc9-409d-abf9-702753bd878d", "99fe402e-89a0-45aa-9163-85342e865dc8" };
			SiteFeatureEnforcerBase<A, C2>.s_2007SharePointServerStandardSiteFeatures = strArrays;
			string[] strArrays1 = new string[] { "065c78be-5231-477e-a972-14177cc5b3c7", "0806d127-06e6-447a-980e-2e90b03101b8", "2510d73f-7109-4ccc-8a1c-314894deeb3a", "00bfea71-dbd7-4f72-b8cb-da7ac0440130" };
			SiteFeatureEnforcerBase<A, C2>.s_2007SharePointServerEnterpriseSitefeatures = strArrays1;
			string[] strArrays2 = new string[] { "00bfea71-c796-4402-9f2f-0eb9a6e71b18", "00bfea71-5932-4f9c-ad71-1557e5751100", "00bfea71-4ea5-48d4-a4ad-305cf7030140", "00bfea71-f600-43f6-a895-40c0de7b0117", "00bfea71-eb8a-40b1-80c7-506be7590102", "00bfea71-3a1d-41d3-a0ee-651d11570120", "00bfea71-513d-4ca0-96c2-6a47775c0119", "00bfea71-2062-426c-90bf-714c59600103", "00bfea71-2d77-4a75-9fca-76516689e21a", "00bfea71-a83e-497e-9ba0-7a5c597d0107", "00bfea71-4ea5-48d4-a4ad-7ea5c011abe5", "00bfea71-d1ce-42de-9c63-a44004ce0104", "00bfea71-52d4-45b3-b544-b1c71b620109", "00bfea71-7e6d-4186-9ba8-c047ac750105", "00bfea71-de22-43b2-a848-c05709900100", "00bfea71-e717-4e80-aa17-d0c71b360101", "00bfea71-6a49-43fa-b535-d15c05500108", "00bfea71-f381-423d-b9d1-da7a54c50110", "00bfea71-ec85-4903-972d-ebe475780106", "00bfea71-1e1d-4562-b56a-f05371bb0115" };
			SiteFeatureEnforcerBase<A, C2>.s_2007TeamColaborationListsFeatures = strArrays2;
			SiteFeatureEnforcerBase<A, C2>.s_2010SharePointServerStandardSiteFeatures = new string[] { "e8734bb6-be8e-48a1-b036-5a40ff0b8a81", "0be49fe9-9bc9-409d-abf9-702753bd878d", "99fe402e-89a0-45aa-9163-85342e865dc8" };
			SiteFeatureEnforcerBase<A, C2>.s_2013SharePointServerStandardSiteFeatures = new string[] { "99fe402e-89a0-45aa-9163-85342e865dc8", "ff13819a-a9ac-46fb-8163-9d53357ef98d" };
			SiteFeatureEnforcerBase<A, C2>.s_O365SharePointServerStandardSiteFeatures = new string[] { "99fe402e-89a0-45aa-9163-85342e865dc8" };
		}

		protected SiteFeatureEnforcerBase()
		{
		}

		private string EnsureFeaturesExist(string featureString, IEnumerable<string> siteFeaturesToEnsure)
		{
			List<string> strs = new List<string>(this.GetFeaturesFromString(featureString));
			foreach (string str in siteFeaturesToEnsure)
			{
				if (strs.Contains<string>(str, new SiteFeatureEnforcerBase<A, C2>.CaseInsensitiveStringComparer()))
				{
					continue;
				}
				strs.Add(str);
			}
			StringBuilder stringBuilder = new StringBuilder(1000);
			bool flag = false;
			foreach (string str1 in strs)
			{
				if (flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(str1);
				flag = true;
			}
			return stringBuilder.ToString();
		}

		private string[] GetFeaturesFromString(string featureString)
		{
			if (string.IsNullOrEmpty(featureString))
			{
				return new string[0];
			}
			return featureString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}

		private SPWeb Map2007WorkflowFeature(SPWeb dataObject, SharePointVersion targetVersion)
		{
			if (dataObject.Adapter.SharePointVersion.IsSharePoint2007 && targetVersion.IsSharePoint2010OrLater)
			{
				XmlNode xML = dataObject.ToXML();
				string attributeValueAsString = xML.GetAttributeValueAsString("SiteCollFeatures");
				if (!string.IsNullOrEmpty(attributeValueAsString))
				{
					bool flag = false;
					if (attributeValueAsString.Contains("6c09612b-46af-4b2f-8dfc-59185c962a29"))
					{
						attributeValueAsString = attributeValueAsString.Replace("6c09612b-46af-4b2f-8dfc-59185c962a29", "c845ed8d-9ce5-448c-bd3e-ea71350ce45b");
						flag = true;
					}
					else if (attributeValueAsString.Contains("02464c6a-9d07-4f30-ba04-e9035cf54392"))
					{
						attributeValueAsString = attributeValueAsString.Replace("02464c6a-9d07-4f30-ba04-e9035cf54392", "c845ed8d-9ce5-448c-bd3e-ea71350ce45b");
						flag = true;
					}
					if (flag)
					{
						xML.Attributes["SiteCollFeatures"].Value = attributeValueAsString;
						UpdateWebOptions updateWebOption = new UpdateWebOptions()
						{
							CopyFeatures = true,
							MergeFeatures = false
						};
						dataObject.Update(xML.OuterXml, updateWebOption);
					}
				}
			}
			return dataObject;
		}

		protected SPWeb UpdateWeb(SPWeb dataObject, SharePointVersion targetVersion)
		{
			if (!dataObject.Adapter.SharePointVersion.IsSharePoint2003)
			{
				return this.Map2007WorkflowFeature(dataObject, targetVersion);
			}
			XmlNode xML = dataObject.ToXML();
			if (xML == null)
			{
				return dataObject;
			}
			xML = xML.Clone();
			XmlAttribute itemOf = xML.Attributes["SiteFeatures"];
			if (itemOf == null)
			{
				itemOf = xML.OwnerDocument.CreateAttribute("SiteFeatures");
				xML.Attributes.Append(itemOf);
			}
			SharePointVersion sharePointVersion = dataObject.Adapter.SharePointVersion;
			if (targetVersion.IsSharePoint2007)
			{
				itemOf.Value = this.EnsureFeaturesExist(itemOf.Value, SiteFeatureEnforcerBase<A, C2>.s_2007SharePointServerStandardSiteFeatures);
				itemOf.Value = this.EnsureFeaturesExist(itemOf.Value, SiteFeatureEnforcerBase<A, C2>.s_2007SharePointServerEnterpriseSitefeatures);
				itemOf.Value = this.EnsureFeaturesExist(itemOf.Value, SiteFeatureEnforcerBase<A, C2>.s_2007TeamColaborationListsFeatures);
			}
			else if (targetVersion.IsSharePoint2010)
			{
				itemOf.Value = this.EnsureFeaturesExist(itemOf.Value, SiteFeatureEnforcerBase<A, C2>.s_2010SharePointServerStandardSiteFeatures);
			}
			else if (!targetVersion.IsSharePoint2013)
			{
				if (!targetVersion.IsSharePoint2016OrLater)
				{
					throw new Exception("Unable to enforce standard site feature existence. Unrecognized target SharePoint version.");
				}
				itemOf.Value = this.EnsureFeaturesExist(itemOf.Value, SiteFeatureEnforcerBase<A, C2>.s_O365SharePointServerStandardSiteFeatures);
			}
			else
			{
				itemOf.Value = this.EnsureFeaturesExist(itemOf.Value, SiteFeatureEnforcerBase<A, C2>.s_2013SharePointServerStandardSiteFeatures);
			}
			UpdateWebOptions updateWebOption = new UpdateWebOptions()
			{
				CopyFeatures = true,
				MergeFeatures = false
			};
			dataObject.Update(xML.OuterXml, updateWebOption);
			return dataObject;
		}

		private class CaseInsensitiveStringComparer : IEqualityComparer<string>
		{
			public CaseInsensitiveStringComparer()
			{
			}

			public bool Equals(string x, string y)
			{
				return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
			}

			public int GetHashCode(string obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}