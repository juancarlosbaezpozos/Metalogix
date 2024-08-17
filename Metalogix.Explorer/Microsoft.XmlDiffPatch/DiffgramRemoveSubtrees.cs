using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramRemoveSubtrees : DiffgramOperation
    {
        private XmlDiffNode _firstSourceNode;

        private XmlDiffNode _lastSourceNode;

        private bool _bSorted;

        internal DiffgramRemoveSubtrees(XmlDiffNode sourceNode, ulong operationID, bool bSorted) : base(operationID)
        {
            this._firstSourceNode = sourceNode;
            this._lastSourceNode = sourceNode;
            this._bSorted = bSorted;
        }

        internal bool SetNewFirstNode(XmlDiffNode srcNode)
        {
            if (this._operationID != (long)0 || srcNode._nextSibling != this._firstSourceNode || !srcNode.CanMerge ||
                !this._firstSourceNode.CanMerge)
            {
                return false;
            }

            this._firstSourceNode = srcNode;
            return true;
        }

        internal bool SetNewLastNode(XmlDiffNode srcNode)
        {
            if (this._operationID != (long)0 || this._lastSourceNode._nextSibling != srcNode || !srcNode.CanMerge ||
                !this._firstSourceNode.CanMerge)
            {
                return false;
            }

            this._lastSourceNode = srcNode;
            return true;
        }

        private void Sort()
        {
            XmlDiffNode xmlDiffNode = null;
            XmlDiff.SortNodesByPosition(ref this._firstSourceNode, ref this._lastSourceNode, ref xmlDiffNode);
            this._bSorted = true;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            if (!this._bSorted)
            {
                this.Sort();
            }

            xmlWriter.WriteStartElement("xd", "remove", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            if (this._firstSourceNode != this._lastSourceNode)
            {
                xmlWriter.WriteAttributeString("match",
                    DiffgramOperation.GetRelativeAddressOfNodeset(this._firstSourceNode, this._lastSourceNode));
            }
            else
            {
                xmlWriter.WriteAttributeString("match", this._firstSourceNode.GetRelativeAddress());
            }

            if (this._operationID != (long)0)
            {
                xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            }

            xmlWriter.WriteEndElement();
        }
    }
}