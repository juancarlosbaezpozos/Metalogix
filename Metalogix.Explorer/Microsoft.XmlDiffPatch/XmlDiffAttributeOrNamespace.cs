using System;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class XmlDiffAttributeOrNamespace : XmlDiffNode
    {
        internal abstract string LocalName { get; }

        internal abstract string NamespaceURI { get; }

        internal abstract string Prefix { get; }

        internal abstract string Value { get; }

        internal XmlDiffAttributeOrNamespace() : base(0)
        {
        }
    }
}