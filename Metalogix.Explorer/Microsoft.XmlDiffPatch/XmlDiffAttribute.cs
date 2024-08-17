using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffAttribute : XmlDiffAttributeOrNamespace
    {
        private string _localName;

        private string _prefix;

        private string _ns;

        private string _value;

        internal override bool CanMerge
        {
            get { return false; }
        }

        internal override string LocalName
        {
            get { return this._localName; }
        }

        internal string Name
        {
            get
            {
                if (this._prefix.Length <= 0)
                {
                    return this._localName;
                }

                return string.Concat(this._prefix, ":", this._localName);
            }
        }

        internal override string NamespaceURI
        {
            get { return this._ns; }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.Attribute; }
        }

        internal override string Prefix
        {
            get { return this._prefix; }
        }

        internal override string Value
        {
            get { return this._value; }
        }

        internal XmlDiffAttribute(string localName, string prefix, string ns, string value)
        {
            this._localName = localName;
            this._prefix = prefix;
            this._ns = ns;
            this._value = value;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashAttribute(this._localName, this._prefix, this._ns, this._value);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            return XmlDiffOperation.Undefined;
        }

        internal override string GetRelativeAddress()
        {
            return string.Concat("@", this.Name);
        }

        internal override bool IsSameAs(XmlDiffNode node, XmlDiff xmlDiff)
        {
            XmlDiffAttribute xmlDiffAttribute = (XmlDiffAttribute)node;
            if (!(this.LocalName == xmlDiffAttribute.LocalName) ||
                !xmlDiff.IgnoreNamespaces && !(this.NamespaceURI == xmlDiffAttribute.NamespaceURI) ||
                !xmlDiff.IgnorePrefixes && !(this.Prefix == xmlDiffAttribute.Prefix))
            {
                return false;
            }

            return this.Value == xmlDiffAttribute.Value;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
            w.WriteString(this.Value);
        }

        internal override void WriteTo(XmlWriter w)
        {
            w.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
            this.WriteContentTo(w);
            w.WriteEndAttribute();
        }
    }
}