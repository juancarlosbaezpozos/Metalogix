using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class MasterPageDataUpdater : Transformer<SPListItem, CopyMasterPagesAction, SPListItemCollection, SPListItemCollection, MasterPageUpdaterOptions>
	{
		private const string RegexQueryToGetFieldGuids = "(?<=FieldName=[\"])[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12}(?=[\"])";

		public override string Name
		{
			get
			{
				return "Master Page Data Updater";
			}
		}

		protected string UIVersionToUpdatePageFor
		{
			get;
			set;
		}

		protected bool UpdateMasterPageForUseBySpecificUIVersion
		{
			get;
			set;
		}

		public MasterPageDataUpdater()
		{
		}

		public override void BeginTransformation(CopyMasterPagesAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			this.UpdateMasterPageForUseBySpecificUIVersion = action.ActionOptions.UpdateMasterPagesForUseBySpecificUIVersion;
			this.UIVersionToUpdatePageFor = action.ActionOptions.UIVersionToCopyPagesFor;
		}

		public override void EndTransformation(CopyMasterPagesAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		private void GetMasterPagePlaceHolderData(SharePointVersion targetVersion, string newPageName, Set<string> targetUIVersions, out string resourceXML, out byte[] resourceHTML)
		{
			Stream manifestResourceStream;
			Stream stream;
			if (targetVersion.IsSharePoint2007 || targetVersion.IsSharePoint2010 && !targetUIVersions.Contains("4"))
			{
				if (targetVersion.IsSharePoint2007)
				{
					targetUIVersions = null;
				}
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2007MasterPage.xml");
				stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2007MasterPageHTML.txt");
			}
			else if (targetVersion.IsSharePoint2010 || targetVersion.IsSharePoint2013 && !targetUIVersions.Contains("15") || targetVersion.IsSharePoint2016 && !targetUIVersions.Contains("15") && !targetUIVersions.Contains("16"))
			{
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2010MasterPage.xml");
				stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2010MasterPageHTML.txt");
			}
			else if (targetVersion.IsSharePoint2013 || targetVersion.IsSharePoint2016 && !targetUIVersions.Contains("16"))
			{
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2013MasterPage.xml");
				stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2013MasterPageHTML.txt");
			}
			else
			{
				if (!targetVersion.IsSharePoint2016)
				{
					throw new Exception(Resources.NoPlaceHoderMasterPageDataForVersionError);
				}
				manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2016MasterPage.xml");
				stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Metalogix.SharePoint.Actions.Migration.MasterPage.2016MasterPageHTML.txt");
			}
			using (StreamReader streamReader = new StreamReader(manifestResourceStream))
			{
				resourceXML = streamReader.ReadToEnd();
			}
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				resourceHTML = binaryReader.ReadBytes((int)stream.Length);
			}
			if (targetUIVersions != null)
			{
				TransformationTask transformationTask = new TransformationTask();
				transformationTask.ChangeOperations.Add("UIVersion", this.GetUIVersionStringFromSet(targetUIVersions));
				resourceXML = transformationTask.PerformTransformation(resourceXML);
			}
			resourceXML = resourceXML.Replace("v4.", string.Concat(newPageName, "."));
		}

		private string GetUIVersionStringFromSet(Set<string> uiVersions)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			bool flag = true;
			foreach (string uiVersion in uiVersions)
			{
				if (!flag)
				{
					stringBuilder.Append(";#");
				}
				stringBuilder.Append(uiVersion);
				flag = false;
			}
			return stringBuilder.ToString();
		}

		public override SPListItem Transform(SPListItem dataObject, CopyMasterPagesAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			bool flag;
			string str;
			if (!sources.ParentSPList.GetType().IsAssignableFrom(typeof(SPMasterPageGallery)) || !dataObject.FileLeafRef.EndsWith(".master"))
			{
				return this.UpdatePageLayout(dataObject, action);
			}
			this.UpdateMasterPageData(sources.ParentSPList.ParentWeb, targets.ParentSPList.ParentWeb, dataObject, out flag, out str);
			if (!flag)
			{
				return dataObject;
			}
			LogItem logItem = new LogItem("Master Page Skipped", dataObject.Name, dataObject.DisplayUrl, targets.ParentSPList.ParentWeb.DisplayUrl, ActionOperationStatus.Skipped)
			{
				Information = str
			};
			base.FireOperationStarted(logItem);
			base.FireOperationFinished(logItem);
			return null;
		}

		private string UpdateFieldGuid(string pageData, Dictionary<Guid, Guid> guidMappings)
		{
			string str1 = Regex.Replace(pageData, "(?<=FieldName=[\"])[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12}(?=[\"])", (Match match) => {
				string str = match.ToString();
				Guid guid = new Guid(str);
				if (!guidMappings.ContainsKey(guid))
				{
					return str;
				}
				return guidMappings[guid].ToString();
			});
			return str1;
		}

		private void UpdateMasterPageData(SPWeb sourceWeb, SPWeb targetWeb, SPListItem sourceItem, out bool skipMasterPage, out string skipReason)
		{
			byte[] numArray;
			skipMasterPage = false;
			skipReason = string.Empty;
			SharePointVersion sharePointVersion = sourceWeb.Adapter.SharePointVersion;
			SharePointVersion sharePointVersion1 = targetWeb.Adapter.SharePointVersion;
			if (sharePointVersion.VersionNumber.Major > sharePointVersion1.VersionNumber.Major)
			{
				throw new Exception(Resources.MasterPageDownGradeError);
			}
			if (sharePointVersion.IsSharePoint2003)
			{
				throw new Exception(Resources.No2003MasterPagesError);
			}
			string targetPageName = sourceItem.FileLeafRef.Replace(".master", "");
			Set<string> masterPageUIVersion = MasterPageMigrationUtils.GetMasterPageUIVersion(sourceItem);
			if (masterPageUIVersion.Count == 0)
			{
				masterPageUIVersion = MasterPageMigrationUtils.GetMasterPageUIVersion(sourceItem.ParentCollection.GetItemByID(sourceItem.ID));
			}
			if (!this.UpdateMasterPageForUseBySpecificUIVersion && !masterPageUIVersion.IntersectsWith(MasterPageMigrationUtils.GetSupportedUIVersions(sharePointVersion1)))
			{
				skipMasterPage = true;
				skipReason = Resources.NotSupportedUIVersionError;
				return;
			}
			string str = (this.UpdateMasterPageForUseBySpecificUIVersion ? this.UIVersionToUpdatePageFor : MasterPageMigrationUtils.GetWebUIVersion(targetWeb));
			Set<string> set = (this.UpdateMasterPageForUseBySpecificUIVersion ? new Set<string>(new string[] { str }) : masterPageUIVersion);
			set = Set<string>.Intersection(set, MasterPageMigrationUtils.GetSupportedUIVersions(sharePointVersion1));
			if (sharePointVersion.MajorVersion == sharePointVersion1.MajorVersion && masterPageUIVersion.IntersectsWith(set))
			{
				return;
			}
			targetPageName = MasterPageMigrationUtils.GetTargetPageName(targetPageName, masterPageUIVersion, set, sharePointVersion, sharePointVersion1);
			string xML = sourceItem.XML;
			if (masterPageUIVersion.IntersectsWith(set))
			{
				this.UpdateMasterPageXmlNewData(sourceItem.FileLeafRef, string.Concat(targetPageName, ".master"), set, sharePointVersion, sharePointVersion1, ref xML);
			}
			else
			{
				string[] strArrays = new string[] { str };
				this.GetMasterPagePlaceHolderData(sharePointVersion1, targetPageName, new Set<string>(strArrays), out xML, out numArray);
				sourceItem.Binary = numArray;
			}
			sourceItem.SetFullXML(XmlUtility.StringToXmlNode(xML));
		}

		private void UpdateMasterPageXmlNewData(string originalFileName, string newFileName, Set<string> targetUIVersions, SharePointVersion sourceVersion, SharePointVersion targetVersion, ref string masterPageXML)
		{
			TransformationTask transformationTask = new TransformationTask();
			if (targetUIVersions != null)
			{
				transformationTask.ChangeOperations.Add("UIVersion", this.GetUIVersionStringFromSet(targetUIVersions));
			}
			if (originalFileName != newFileName)
			{
				transformationTask.ChangeOperations.Add("FileLeafRef", newFileName);
				XmlNode xmlNode = XmlUtility.StringToXmlNode(masterPageXML);
				XmlAttribute itemOf = xmlNode.Attributes["FileRef"];
				if (itemOf != null && itemOf.Value.ToLower().EndsWith(string.Concat("/", originalFileName)))
				{
					itemOf.Value = string.Concat(itemOf.Value.Substring(0, itemOf.Value.Length - originalFileName.Length), newFileName);
					masterPageXML = xmlNode.OuterXml;
				}
			}
			if (sourceVersion.IsSharePoint2007 && targetVersion.IsSharePoint2010OrLater)
			{
				transformationTask.ChangeOperations.Add("HTML_x0020_File_x0020_Type", "SharePoint.WebPartPage.Document");
			}
			masterPageXML = transformationTask.PerformTransformation(masterPageXML);
		}

		private SPListItem UpdatePageLayout(SPListItem dataObject, CopyMasterPagesAction action)
		{
			if (dataObject.FileLeafRef.EndsWith(".aspx") && action.SharePointOptions.CopyPageLayouts)
			{
				string str = (new UTF8Encoding()).GetString(dataObject.Binary);
				dataObject.Binary = Encoding.UTF8.GetBytes(this.UpdateFieldGuid(str, action.GuidMappings));
			}
			return dataObject;
		}
	}
}