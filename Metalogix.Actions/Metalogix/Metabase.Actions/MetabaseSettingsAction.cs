using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.Attributes;
using Metalogix.Metabase.Options;
using System;
using System.IO;

namespace Metalogix.Metabase.Actions
{
    [ActionConfigRequired(true)]
    [Image("Metalogix.Actions.Icons.Metabase.ico")]
    [IsAdvanced(true)]
    [IsConnectivityAction(true)]
    [LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare | ProductFlags.CMCPublicFolder |
                      ProductFlags.UnifiedContentMatrixExpressKey | ProductFlags.CMCOracleAndStellent |
                      ProductFlags.CMCDocumentum | ProductFlags.CMCBlogsAndWikis | ProductFlags.CMCGoogle |
                      ProductFlags.SRM | ProductFlags.CMWebComponents)]
    [MenuText("Change Metabase Storage... {1-Metabase}")]
    [Name("Change Metabase Storage")]
    [RequiresTargetMetabaseConnection(true)]
    [RunAsync(false)]
    [ShowStatusDialog(false)]
    [TargetCardinality(Cardinality.One)]
    [TargetType(typeof(Connection))]
    [UsesStickySettings(false)]
    public class MetabaseSettingsAction : MetabaseAction<MetabaseSettingsOptions>
    {
        public MetabaseSettingsAction()
        {
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            if (!base.AppliesTo(sourceSelections, targetSelections))
            {
                return false;
            }

            Connection item = (Connection)targetSelections[0];
            if (item != null && item.Node.Parent == null &&
                (item.Status == ConnectionStatus.Valid || !item.MetabaseConnection.IsConnected))
            {
                return true;
            }

            return false;
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            Connection item = (Connection)target[0];
            if (this.ActionOptions.UseDefault)
            {
                item.MetabaseConnection = MetabaseFactory.CreateDefaultMetabaseConnection();
            }
            else if (!(this.ActionOptions.MetabaseType == "SqlCe") || File.Exists(this.ActionOptions.MetabaseContext))
            {
                item.MetabaseConnection = MetabaseFactory.ConnectToExistingMetabase(this.ActionOptions.MetabaseType,
                    this.ActionOptions.MetabaseContext);
            }
            else
            {
                item.MetabaseConnection = MetabaseFactory.CreateNewMetabaseConnection(this.ActionOptions.MetabaseType,
                    this.ActionOptions.MetabaseContext);
            }

            Settings.SaveActiveConnections();
        }
    }
}