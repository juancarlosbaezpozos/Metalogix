using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class OperationDescriptor
    {
        protected ulong _operationID;

        internal OperationDescriptor _nextDescriptor;

        internal abstract string Type { get; }

        internal OperationDescriptor(ulong opid)
        {
            this._operationID = opid;
        }

        internal virtual void WriteTo(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("xd", "descriptor", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            xmlWriter.WriteAttributeString("type", this.Type);
            xmlWriter.WriteEndElement();
        }
    }
}