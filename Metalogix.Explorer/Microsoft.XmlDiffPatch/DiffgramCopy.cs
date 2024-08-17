using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramCopy : DiffgramParentOperation
    {
        private XmlDiffNode _sourceNode;

        private bool _bSubtree;

        internal DiffgramCopy(XmlDiffNode sourceNode, bool bSubtree, ulong operationID) : base(operationID)
        {
            this._sourceNode = sourceNode;
            this._bSubtree = bSubtree;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            xmlWriter.WriteStartElement("xd", "add", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            DiffgramOperation.WriteAbsoluteMatchAttribute(this._sourceNode, xmlWriter);
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