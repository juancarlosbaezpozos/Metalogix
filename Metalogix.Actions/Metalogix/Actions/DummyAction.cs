using Metalogix.Licensing;
using System;

namespace Metalogix.Actions
{
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [ShowInMenus(false)]
    public class DummyAction : Metalogix.Actions.Action
    {
        public DummyAction()
        {
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            return false;
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            throw new NotSupportedException("This class is not runnable.");
        }
    }
}