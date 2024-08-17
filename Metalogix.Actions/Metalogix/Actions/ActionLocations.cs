using System;

namespace Metalogix.Actions
{
    public class ActionLocations
    {
        public IXMLAbleList Source;

        public IXMLAbleList Target;

        public ActionLocations(IXMLAbleList source, IXMLAbleList target)
        {
            this.Source = source;
            this.Target = target;
        }
    }
}