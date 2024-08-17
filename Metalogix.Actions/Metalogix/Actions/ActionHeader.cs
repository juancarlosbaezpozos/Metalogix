using Metalogix.Licensing;
using System;

namespace Metalogix.Actions
{
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    public abstract class ActionHeader : Metalogix.Actions.Action
    {
        protected ActionHeader()
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