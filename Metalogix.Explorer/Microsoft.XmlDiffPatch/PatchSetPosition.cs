using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class PatchSetPosition : XmlPatchParentOperation
    {
        private XmlNode _matchNode;

        internal PatchSetPosition(XmlNode matchNode)
        {
            this._matchNode = matchNode;
        }

        internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
        {
            if (this._matchNode.NodeType == XmlNodeType.Element)
            {
                base.ApplyChildren((XmlElement)this._matchNode);
            }

            currentPosition = this._matchNode;
        }
    }
}