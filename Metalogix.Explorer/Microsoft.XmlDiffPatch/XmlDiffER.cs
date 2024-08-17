using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffER : XmlDiffNode
    {
        private string _name;

        internal override bool CanMerge
        {
            get { return false; }
        }

        internal string Name
        {
            get { return this._name; }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.EntityReference; }
        }

        internal XmlDiffER(int position, string name) : base(position)
        {
            this._name = name;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashER(this._name);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.EntityReference)
            {
                return XmlDiffOperation.Undefined;
            }

            if (this.Name == ((XmlDiffER)changedNode).Name)
            {
                return XmlDiffOperation.Match;
            }

            return XmlDiffOperation.ChangeER;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }

        internal override void WriteTo(XmlWriter w)
        {
            w.WriteEntityRef(this._name);
        }
    }
}