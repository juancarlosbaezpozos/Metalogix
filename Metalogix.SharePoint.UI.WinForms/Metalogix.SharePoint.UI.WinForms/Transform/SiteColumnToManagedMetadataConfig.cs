using Metalogix;
using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Transform
{
    [TransformerConfig(new Type[] { typeof(SiteColumnToManagedMetadata) })]
    public class SiteColumnToManagedMetadataConfig : ITransformerConfig
    {
        public SiteColumnToManagedMetadataConfig()
        {
        }

        public bool Configure(TransformerConfigContext context)
        {
            SiteColumnToManagedMetadataConfigDialog siteColumnToManagedMetadataConfigDialog = new SiteColumnToManagedMetadataConfigDialog(context.ActionContext.GetSourcesAsNodeCollection());
            SiteColumnToManagedMetadataOptionCollection transformerOptions = context.GetTransformerOptions<SiteColumnToManagedMetadataOptionCollection>();
            SiteColumnToManagedMetadataOptionCollection siteColumnToManagedMetadataOptionCollection = new SiteColumnToManagedMetadataOptionCollection();
            siteColumnToManagedMetadataOptionCollection.FromXML(transformerOptions.ToXML());
            siteColumnToManagedMetadataConfigDialog.Context = context;
            siteColumnToManagedMetadataConfigDialog.OriginalOptions = transformerOptions;
            siteColumnToManagedMetadataConfigDialog.WorkingOptionsSet = siteColumnToManagedMetadataOptionCollection;
            return siteColumnToManagedMetadataConfigDialog.ShowDialog() == DialogResult.OK;
        }
    }
}