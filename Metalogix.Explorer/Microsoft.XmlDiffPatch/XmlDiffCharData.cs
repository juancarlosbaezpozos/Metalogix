using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffCharData : XmlDiffNode
    {
        private string _value;

        private XmlDiffNodeType _nodeType;

        internal override XmlDiffNodeType NodeType
        {
            get { return this._nodeType; }
        }

        internal string Value
        {
            get { return this._value; }
        }

        internal XmlDiffCharData(int position, string value, XmlDiffNodeType nodeType) : base(position)
        {
            this._value = value;
            this._nodeType = nodeType;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashCharacterNode((XmlNodeType)this._nodeType, this._value);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (this.NodeType != changedNode.NodeType)
            {
                return XmlDiffOperation.Undefined;
            }

            XmlDiffCharData xmlDiffCharDatum = changedNode as XmlDiffCharData;
            if (xmlDiffCharDatum == null)
            {
                return XmlDiffOperation.Undefined;
            }

            if (this.Value == xmlDiffCharDatum.Value)
            {
                return XmlDiffOperation.Match;
            }

            return XmlDiffOperation.ChangeCharacterData;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }

        internal override void WriteTo(XmlWriter w)
        {
            XmlDiffNodeType xmlDiffNodeType = this._nodeType;
            switch (xmlDiffNodeType)
            {
                case XmlDiffNodeType.Text:
                {
                    w.WriteString(this.Value);
                    return;
                }
                case XmlDiffNodeType.CDATA:
                {
                    w.WriteCData(this.Value);
                    return;
                }
                default:
                {
                    if (xmlDiffNodeType == XmlDiffNodeType.Comment)
                    {
                        w.WriteComment(this.Value);
                        return;
                    }

                    if (xmlDiffNodeType == XmlDiffNodeType.SignificantWhitespace)
                    {
                        w.WriteString(this.Value);
                        return;
                    }

                    return;
                }
            }
        }
    }
}