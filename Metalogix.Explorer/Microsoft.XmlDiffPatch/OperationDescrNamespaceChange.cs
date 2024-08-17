using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class OperationDescrNamespaceChange : OperationDescriptor
    {
        private DiffgramGenerator.NamespaceChange _nsChange;

        internal override string Type
        {
            get { return "namespace change"; }
        }

        internal OperationDescrNamespaceChange(DiffgramGenerator.NamespaceChange nsChange) : base(nsChange._opid)
        {
            this._nsChange = nsChange;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("xd", "descriptor", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            xmlWriter.WriteAttributeString("type", this.Type);
            xmlWriter.WriteAttributeString("prefix", this._nsChange._prefix);
            xmlWriter.WriteAttributeString("oldNs", this._nsChange._oldNS);
            xmlWriter.WriteAttributeString("newNs", this._nsChange._newNS);
            xmlWriter.WriteEndElement();
        }
    }
}