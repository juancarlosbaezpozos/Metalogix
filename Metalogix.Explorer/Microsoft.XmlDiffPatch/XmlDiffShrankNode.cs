using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffShrankNode : XmlDiffNode
    {
        internal XmlDiffNode _firstNode;

        internal XmlDiffNode _lastNode;

        private XmlDiffShrankNode _matchingShrankNode;

        private string _localAddress;

        private ulong _opid;

        internal override bool CanMerge
        {
            get { return false; }
        }

        internal XmlDiffShrankNode MatchingShrankNode
        {
            get { return this._matchingShrankNode; }
            set { this._matchingShrankNode = value; }
        }

        internal ulong MoveOperationId
        {
            get
            {
                if (this._opid == (long)0)
                {
                    this._opid = this.MatchingShrankNode._opid;
                }

                return this._opid;
            }
            set { this._opid = value; }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.ShrankNode; }
        }

        internal XmlDiffShrankNode(XmlDiffNode firstNode, XmlDiffNode lastNode) : base(-1)
        {
            this._firstNode = firstNode;
            this._lastNode = lastNode;
            this._matchingShrankNode = null;
            XmlDiffNode xmlDiffNode = firstNode;
            while (true)
            {
                XmlDiffShrankNode hashValue = this;
                hashValue._hashValue = hashValue._hashValue + (this._hashValue << 7) + xmlDiffNode.HashValue;
                if (xmlDiffNode == lastNode)
                {
                    break;
                }

                xmlDiffNode = xmlDiffNode._nextSibling;
            }

            this._localAddress = DiffgramOperation.GetRelativeAddressOfNodeset(this._firstNode, this._lastNode);
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.ShrankNode)
            {
                return XmlDiffOperation.Undefined;
            }

            if (this._hashValue == ((XmlDiffShrankNode)changedNode)._hashValue)
            {
                return XmlDiffOperation.Match;
            }

            return XmlDiffOperation.Undefined;
        }

        internal override string GetRelativeAddress()
        {
            return this._localAddress;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
            XmlDiffNode xmlDiffNode = this._firstNode;
            while (true)
            {
                xmlDiffNode.WriteTo(w);
                if (xmlDiffNode == this._lastNode)
                {
                    break;
                }

                xmlDiffNode = xmlDiffNode._nextSibling;
            }
        }

        internal override void WriteTo(XmlWriter w)
        {
            this.WriteContentTo(w);
        }
    }
}