using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class Patch : XmlPatchParentOperation
    {
        internal XmlNode _sourceRootNode;

        internal Patch(XmlNode sourceRootNode)
        {
            this._sourceRootNode = sourceRootNode;
        }

        internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
        {
            XmlDocument ownerDocument = parent.OwnerDocument;
            base.ApplyChildren(parent);
        }
    }
}