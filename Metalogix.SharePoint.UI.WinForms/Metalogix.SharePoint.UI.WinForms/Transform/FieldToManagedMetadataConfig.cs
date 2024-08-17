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
    [TransformerConfig(new Type[] { typeof(FieldToManagedMetadata) })]
    public class FieldToManagedMetadataConfig : ITransformerConfig
    {
        public FieldToManagedMetadataConfig()
        {
        }

        public bool Configure(TransformerConfigContext context)
        {
            FieldToManagedMetadataConfigDialog fieldToManagedMetadataConfigDialog = new FieldToManagedMetadataConfigDialog(context.ActionContext.GetSourcesAsNodeCollection());
            FieldToManagedMetadataOptionCollection transformerOptions = context.GetTransformerOptions<FieldToManagedMetadataOptionCollection>();
            FieldToManagedMetadataOptionCollection fieldToManagedMetadataOptionCollection = new FieldToManagedMetadataOptionCollection();
            fieldToManagedMetadataOptionCollection.FromXML(transformerOptions.ToXML());
            fieldToManagedMetadataConfigDialog.Context = context;
            fieldToManagedMetadataConfigDialog.OriginalOptions = transformerOptions;
            fieldToManagedMetadataConfigDialog.WorkingOptionsSet = fieldToManagedMetadataOptionCollection;
            return fieldToManagedMetadataConfigDialog.ShowDialog() == DialogResult.OK;
        }
    }
}