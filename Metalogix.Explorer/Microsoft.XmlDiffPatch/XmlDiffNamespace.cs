using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffNamespace : XmlDiffAttributeOrNamespace
    {
        private string _prefix;

        private string _namespaceURI;

        internal override string LocalName
        {
            get { return string.Empty; }
        }

        internal string Name
        {
            get
            {
                if (this._prefix.Length <= 0)
                {
                    return "xmlns";
                }

                return string.Concat("xmlns:", this._prefix);
            }
        }

        internal override string NamespaceURI
        {
            get { return this._namespaceURI; }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.Namespace; }
        }

        internal override string Prefix
        {
            get { return this._prefix; }
        }

        internal override string Value
        {
            get { return string.Empty; }
        }

        internal XmlDiffNamespace(string prefix, string namespaceURI)
        {
            this._prefix = prefix;
            this._namespaceURI = namespaceURI;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashNamespace(this._prefix, this._namespaceURI);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            return XmlDiffOperation.Undefined;
        }

        internal override string GetRelativeAddress()
        {
            return string.Concat("@", this.Name);
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }

        internal override void WriteTo(XmlWriter w)
        {
            if (this.Prefix == string.Empty)
            {
                w.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", this.NamespaceURI);
                return;
            }

            w.WriteAttributeString("xmlns", this.Prefix, "http://www.w3.org/2000/xmlns/", this.NamespaceURI);
        }
    }
}