using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.AddSite.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Create Site... {2-Create}")]
	[Name("Create Site")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class CreateSiteAction : SharePointAction<CreateSiteOptions>
	{
		public CreateSiteAction()
		{
		}

		public SPWeb CreateSite(SPWeb targetWeb)
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("Web");
			xmlTextWriter.WriteAttributeString("Name", base.SharePointOptions.URL);
			xmlTextWriter.WriteAttributeString("Title", base.SharePointOptions.Title);
			xmlTextWriter.WriteAttributeString("Description", base.SharePointOptions.Description);
			SPWebTemplate template = base.SharePointOptions.Template;
			xmlTextWriter.WriteAttributeString("WebTemplateID", template.ID.ToString());
			xmlTextWriter.WriteAttributeString("WebTemplateConfig", template.Config.ToString());
			xmlTextWriter.WriteAttributeString("WebTemplateName", template.Name);
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			string str = stringBuilder.ToString();
			AddWebOptions addWebOption = new AddWebOptions()
			{
				Overwrite = base.SharePointOptions.Overwrite,
				CopyAssociatedGroupSettings = false
			};
			SPWeb sPWeb = targetWeb.SubWebs.AddWeb(str, addWebOption, null);
			Thread thread = new Thread(new ParameterizedThreadStart(this.FetchChildren));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start(sPWeb);
			return sPWeb;
		}

		private void FetchChildren(object newWeb)
		{
			((SPWeb)newWeb).FetchChildren();
		}

		private void IsValidSiteURL()
		{
			if (!Utils.IsValidSharePointURL(base.SharePointOptions.URL, false))
			{
				throw new Exception(string.Concat(Resources.ValidSiteURL, " ", Utils.IllegalCharactersForSiteUrl));
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = (SPWeb)target[0])
			{
				this.IsValidSiteURL();
				this.CreateSite(item).Dispose();
			}
		}
	}
}