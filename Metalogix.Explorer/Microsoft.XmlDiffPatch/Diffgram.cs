using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class Diffgram : DiffgramParentOperation
    {
        private XmlDiff _xmlDiff;

        private OperationDescriptor _descriptors;

        internal Diffgram(XmlDiff xmlDiff) : base((ulong)0)
        {
            this._xmlDiff = xmlDiff;
        }

        internal void AddDescriptor(OperationDescriptor desc)
        {
            desc._nextDescriptor = this._descriptors;
            this._descriptors = desc;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            this._xmlDiff = xmlDiff;
            this.WriteTo(xmlWriter);
        }

        internal void WriteTo(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("xd", "xmldiff", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("version", "1.0");
            ulong hashValue = this._xmlDiff._sourceDoc.HashValue;
            xmlWriter.WriteAttributeString("srcDocHash", hashValue.ToString());
            xmlWriter.WriteAttributeString("options", this._xmlDiff.GetXmlDiffOptionsString());
            xmlWriter.WriteAttributeString("fragments", (this._xmlDiff._fragments == TriStateBool.Yes ? "yes" : "no"));
            base.WriteChildrenTo(xmlWriter, this._xmlDiff);
            for (OperationDescriptor i = this._descriptors; i != null; i = i._nextDescriptor)
            {
                i.WriteTo(xmlWriter);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
        }
    }
}