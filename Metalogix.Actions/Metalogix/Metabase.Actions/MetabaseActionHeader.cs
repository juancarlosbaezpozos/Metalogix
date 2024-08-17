using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.Metabase.Attributes;
using System;

namespace Metalogix.Metabase.Actions
{
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [RequiresTargetMetabaseConnection(true)]
    public abstract class MetabaseActionHeader : MetabaseAction<Metalogix.Actions.ActionOptions>
    {
        protected MetabaseActionHeader()
        {
        }

        public override ConfigurationResult Configure(ref IXMLAbleList source, ref IXMLAbleList target)
        {
            return ConfigurationResult.Cancel;
        }

        public override void Run(IXMLAbleList source, IXMLAbleList target)
        {
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
        }
    }
}