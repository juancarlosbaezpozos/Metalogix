using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.AddFolder.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Create Folder... {2-Create}")]
	[Name("Create Folder")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPFolder), true)]
	public class CreateFolderAction : SharePointAction<CreateFolderOptions>
	{
		private readonly static ListTemplateType[] NoFolderListTemplates;

		static CreateFolderAction()
		{
			ListTemplateType[] listTemplateTypeArray = new ListTemplateType[] { ListTemplateType.BlogCategories, ListTemplateType.BlogComments, ListTemplateType.BlogPosts, ListTemplateType.Calltrack, ListTemplateType.Circulation, ListTemplateType.Events, ListTemplateType.Facility, ListTemplateType.Holidays, ListTemplateType.ListTemplateCatalog, ListTemplateType.MeetingAgenda, ListTemplateType.MeetingAttendees, ListTemplateType.MeetingDecisions, ListTemplateType.MeetingDirections, ListTemplateType.MeetingObjectives, ListTemplateType.MeetingPages, ListTemplateType.MeetingThingsToBring, ListTemplateType.SolutionCatalog, ListTemplateType.Survey, ListTemplateType.ThemeCatalog, ListTemplateType.UserInformation, ListTemplateType.WebPartCatalog, ListTemplateType.WebTemplateCatalog, ListTemplateType.Whereabouts, ListTemplateType.WorkFlowHistory, ListTemplateType.WorkflowProcess };
			CreateFolderAction.NoFolderListTemplates = listTemplateTypeArray;
		}

		public CreateFolderAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			if (targetSelections.Count < 1)
			{
				return false;
			}
			SPList item = targetSelections[0] as SPList;
			if (item == null)
			{
				return true;
			}
			return Array.IndexOf<ListTemplateType>(CreateFolderAction.NoFolderListTemplates, item.BaseTemplate) == -1;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("Folder");
			xmlTextWriter.WriteAttributeString("FileLeafRef", base.SharePointOptions.FolderName);
			xmlTextWriter.WriteAttributeString("ContentType", base.SharePointOptions.ContentType);
			xmlTextWriter.WriteAttributeString("ContentTypeId", base.SharePointOptions.ContentTypeId);
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			string str = stringBuilder.ToString();
			try
			{
				using (SPFolder item = (SPFolder)target[0])
				{
					AddFolderOptions addFolderOption = new AddFolderOptions()
					{
						Overwrite = base.SharePointOptions.Overwrite
					};
					SPFolder sPFolder = item.SubFolders.AddFolder(str, addFolderOption, AddFolderMode.Comprehensive);
					SPList parentList = sPFolder.ParentList;
					sPFolder.Dispose();
					if (parentList != null)
					{
						parentList.Refresh();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string message = exception.Message;
				if (exception.InnerException == null || !(exception.InnerException is COMException) || ((COMException)exception.InnerException).ErrorCode != -2147352567)
				{
					throw exception;
				}
				throw new ArgumentException("The target list does not support folders");
			}
		}
	}
}