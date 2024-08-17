using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramRemoveNode : DiffgramParentOperation
    {
        private XmlDiffNode _sourceNode;

        private bool _bSubtree;

        internal DiffgramRemoveNode(XmlDiffNode sourceNode, bool bSubtree, ulong operationID) : base(operationID)
        {
            this._sourceNode = sourceNode;
            this._bSubtree = bSubtree;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            xmlWriter.WriteStartElement("xd", "remove", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("match", this._sourceNode.GetRelativeAddress());
            if (!this._bSubtree)
            {
                xmlWriter.WriteAttributeString("subtree", "no");
            }

            if (this._operationID != (long)0)
            {
                xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            }

            base.WriteChildrenTo(xmlWriter, xmlDiff);
            xmlWriter.WriteEndElement();
        }
    }
}