using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class OperationDescrPrefixChange : OperationDescriptor
    {
        private DiffgramGenerator.PrefixChange _prefixChange;

        internal override string Type
        {
            get { return "prefix change"; }
        }

        internal OperationDescrPrefixChange(DiffgramGenerator.PrefixChange prefixChange) : base(prefixChange._opid)
        {
            this._prefixChange = prefixChange;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("xd", "descriptor", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            xmlWriter.WriteAttributeString("type", this.Type);
            xmlWriter.WriteAttributeString("ns", this._prefixChange._NS);
            xmlWriter.WriteAttributeString("oldPrefix", this._prefixChange._oldPrefix);
            xmlWriter.WriteAttributeString("newPrefix", this._prefixChange._newPrefix);
            xmlWriter.WriteEndElement();
        }
    }
}