using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.AddList.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Create List... {2-Create}")]
	[Name("Create List")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb), true)]
	public class CreateListAction : SharePointAction<CreateListOptions>
	{
		public CreateListAction()
		{
		}

		public SPList CreateList(SPWeb targetWeb)
		{
			if (targetWeb.Lists[base.SharePointOptions.Name] != null)
			{
				throw new Exception("The list could not be created because a list with the same name already exists at this level.");
			}
			if (targetWeb.Lists.GetListByTitle(base.SharePointOptions.Title) != null)
			{
				throw new Exception("The list could not be created because a list with the same title already exists at this level.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("List");
			xmlTextWriter.WriteAttributeString("Name", base.SharePointOptions.Name);
			xmlTextWriter.WriteAttributeString("Title", base.SharePointOptions.Title);
			xmlTextWriter.WriteAttributeString("Description", base.SharePointOptions.Description);
			xmlTextWriter.WriteAttributeString("BaseTemplate", base.SharePointOptions.Template.ToString());
			xmlTextWriter.WriteAttributeString("FeatureId", base.SharePointOptions.FeatureId);
			xmlTextWriter.WriteAttributeString("OnQuickLaunch", base.SharePointOptions.IsOnQuickLaunch.ToString());
			xmlTextWriter.WriteAttributeString("EnableModeration", base.SharePointOptions.RequiresContentApproval.ToString());
			xmlTextWriter.WriteAttributeString("EnableVersioning", base.SharePointOptions.HasVersions.ToString());
			xmlTextWriter.WriteAttributeString("EnableMinorVersions", base.SharePointOptions.HasMinorVersions.ToString());
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			string str = stringBuilder.ToString();
			return targetWeb.Lists.AddList(str, new AddListOptions());
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = (SPWeb)target[0])
			{
				this.CreateList(item).Dispose();
			}
		}
	}
}