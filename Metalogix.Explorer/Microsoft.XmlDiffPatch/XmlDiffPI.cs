using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffPI : XmlDiffCharData
    {
        private string _name;

        internal string Name
        {
            get { return this._name; }
        }

        internal XmlDiffPI(int position, string name, string value) : base(position, value,
            XmlDiffNodeType.ProcessingInstruction)
        {
            this._name = name;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashPI(this.Name, base.Value);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.ProcessingInstruction)
            {
                return XmlDiffOperation.Undefined;
            }

            XmlDiffPI xmlDiffPI = (XmlDiffPI)changedNode;
            if (this.Name == xmlDiffPI.Name)
            {
                if (base.Value == xmlDiffPI.Value)
                {
                    return XmlDiffOperation.Match;
                }

                return XmlDiffOperation.ChangePI;
            }

            if (base.Value == xmlDiffPI.Value)
            {
                return XmlDiffOperation.ChangePI;
            }

            return XmlDiffOperation.Undefined;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }

        internal override void WriteTo(XmlWriter w)
        {
            w.WriteProcessingInstruction(this.Name, base.Value);
        }
    }
}