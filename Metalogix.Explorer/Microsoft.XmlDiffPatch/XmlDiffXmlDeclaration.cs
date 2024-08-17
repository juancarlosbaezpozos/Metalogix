using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffXmlDeclaration : XmlDiffNode
    {
        private string _value;

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.XmlDeclaration; }
        }

        internal string Value
        {
            get { return this._value; }
        }

        internal XmlDiffXmlDeclaration(int position, string value) : base(position)
        {
            this._value = value;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashXmlDeclaration(this._value);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.XmlDeclaration)
            {
                return XmlDiffOperation.Undefined;
            }

            if (this.Value == ((XmlDiffXmlDeclaration)changedNode).Value)
            {
                return XmlDiffOperation.Match;
            }

            return XmlDiffOperation.ChangeXmlDeclaration;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }

        internal override void WriteTo(XmlWriter w)
        {
            w.WriteProcessingInstruction("xml", this._value);
        }
    }
}