using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase.Attributes;
using System;

namespace Metalogix.Metabase.Actions
{
    [Image("Metalogix.Actions.Icons.csv.ico")]
    [LaunchAsJob(false)]
    [LicensedProducts(ProductFlags.CMCPublicFolder | ProductFlags.CMCWebsite | ProductFlags.CMCeRoom |
                      ProductFlags.CMCOracleAndStellent | ProductFlags.CMCDocumentum | ProductFlags.CMCBlogsAndWikis |
                      ProductFlags.CMCGoogle | ProductFlags.SRM | ProductFlags.CMWebComponents)]
    [MenuText("Metadata Modifications {1-Transform} > Load MetaData From CSV {4 - CSV}")]
    [Name("Load CSV")]
    [RequiresTargetMetabaseConnection(true)]
    [RequiresWriteAccess(true)]
    [RunAsync(true)]
    [SourceCardinality(Cardinality.ZeroOrMore)]
    [TargetCardinality(Cardinality.One)]
    [TargetType(typeof(Node))]
    public class LoadFromCSVAction : MetabaseAction<LoadFromCSVOptions>
    {
        public LoadFromCSVAction()
        {
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            if (!base.AppliesTo(sourceSelections, targetSelections))
            {
                return false;
            }

            Node item = targetSelections[0] as Node;
            if (item.Status == ConnectionStatus.Invalid)
            {
                return false;
            }

            return item is IMetadataLoadableObject;
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            throw new NotSupportedException("This action is not runnable.  All work is done in the UI.");
        }
    }
}