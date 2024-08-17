using System;

namespace Microsoft.XmlDiffPatch
{
    internal class AttributeInterval
    {
        internal XmlDiffAttribute _firstAttr;

        internal XmlDiffAttribute _lastAttr;

        internal AttributeInterval _next;

        internal AttributeInterval(XmlDiffAttribute attr, AttributeInterval next)
        {
            this._firstAttr = attr;
            this._lastAttr = attr;
            this._next = next;
        }
    }
}