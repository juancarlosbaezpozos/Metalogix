using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramPosition : DiffgramParentOperation
    {
        internal XmlDiffNode _sourceNode;

        internal DiffgramPosition(XmlDiffNode sourceNode) : base((ulong)0)
        {
            if (sourceNode is XmlDiffShrankNode)
            {
                sourceNode = ((XmlDiffShrankNode)sourceNode)._lastNode;
            }

            this._sourceNode = sourceNode;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            xmlWriter.WriteStartElement("xd", "node", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("match", this._sourceNode.GetRelativeAddress());
            base.WriteChildrenTo(xmlWriter, xmlDiff);
            xmlWriter.WriteEndElement();
        }
    }
}