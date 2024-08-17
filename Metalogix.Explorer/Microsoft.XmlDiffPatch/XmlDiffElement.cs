using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffElement : XmlDiffParentNode
    {
        private string _localName;

        private string _prefix;

        private string _ns;

        internal XmlDiffAttributeOrNamespace _attributes = null;

        internal ulong _allAttributesHash = (long)0;

        internal ulong _attributesHashAH = (long)0;

        internal ulong _attributesHashIQ = (long)0;

        internal ulong _attributesHashRZ = (long)0;

        internal string LocalName
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

        internal string NamespaceURI
        {
            get { return this._ns; }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.Element; }
        }

        internal string Prefix
        {
            get { return this._prefix; }
        }

        internal XmlDiffElement(int position, string localName, string prefix, string ns) : base(position)
        {
            this._localName = localName;
            this._prefix = prefix;
            this._ns = ns;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.ComputeHashXmlDiffElement(this);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.Element)
            {
                return XmlDiffOperation.Undefined;
            }

            XmlDiffElement xmlDiffElement = (XmlDiffElement)changedNode;
            bool flag = false;
            if (this.LocalName == xmlDiffElement.LocalName)
            {
                if (xmlDiff.IgnoreNamespaces)
                {
                    flag = true;
                }
                else if (this.NamespaceURI == xmlDiffElement.NamespaceURI &&
                         (xmlDiff.IgnorePrefixes || this.Prefix == xmlDiffElement.Prefix))
                {
                    flag = true;
                }
            }

            if (xmlDiffElement._allAttributesHash == this._allAttributesHash)
            {
                if (!flag)
                {
                    return XmlDiffOperation.ChangeElementName;
                }

                return XmlDiffOperation.Match;
            }

            int num = (xmlDiffElement._attributesHashAH == this._attributesHashAH ? 0 : 1) +
                      (xmlDiffElement._attributesHashIQ == this._attributesHashIQ ? 0 : 1) +
                      (xmlDiffElement._attributesHashRZ == this._attributesHashRZ ? 0 : 1);
            if (flag)
            {
                return (XmlDiffOperation)(3 + num);
            }

            return (XmlDiffOperation)(6 + num);
        }

        internal void InsertAttributeOrNamespace(XmlDiffAttributeOrNamespace newAttrOrNs)
        {
            char localName;
            newAttrOrNs._parent = this;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace = this._attributes;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace1 = null;
            while (xmlDiffAttributeOrNamespace != null &&
                   XmlDiffDocument.OrderAttributesOrNamespaces(xmlDiffAttributeOrNamespace, newAttrOrNs) <= 0)
            {
                xmlDiffAttributeOrNamespace1 = xmlDiffAttributeOrNamespace;
                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
            }

            if (xmlDiffAttributeOrNamespace1 != null)
            {
                newAttrOrNs._nextSibling = xmlDiffAttributeOrNamespace1._nextSibling;
                xmlDiffAttributeOrNamespace1._nextSibling = newAttrOrNs;
            }
            else
            {
                newAttrOrNs._nextSibling = this._attributes;
                this._attributes = newAttrOrNs;
            }

            this._allAttributesHash += newAttrOrNs.HashValue;
            if (newAttrOrNs.NodeType != XmlDiffNodeType.Attribute)
            {
                XmlDiffNamespace xmlDiffNamespace = (XmlDiffNamespace)newAttrOrNs;
                localName = (xmlDiffNamespace.Prefix == string.Empty ? 'A' : xmlDiffNamespace.Prefix[0]);
            }
            else
            {
                localName = ((XmlDiffAttribute)newAttrOrNs).LocalName[0];
            }

            localName = char.ToUpper(localName);
            if (localName >= 'R')
            {
                this._attributesHashRZ += newAttrOrNs.HashValue;
            }
            else if (localName < 'I')
            {
                this._attributesHashAH += newAttrOrNs.HashValue;
            }
            else
            {
                this._attributesHashIQ += newAttrOrNs.HashValue;
            }

            if (newAttrOrNs.NodeType == XmlDiffNodeType.Namespace)
            {
                this._bDefinesNamespaces = true;
            }
        }

        internal override bool IsSameAs(XmlDiffNode node, XmlDiff xmlDiff)
        {
            if (node.NodeType != XmlDiffNodeType.Element)
            {
                return false;
            }

            XmlDiffElement xmlDiffElement = (XmlDiffElement)node;
            if (this.LocalName != xmlDiffElement.LocalName)
            {
                return false;
            }

            if (!xmlDiff.IgnoreNamespaces)
            {
                if (this.NamespaceURI != xmlDiffElement.NamespaceURI)
                {
                    return false;
                }

                if (!xmlDiff.IgnorePrefixes && this.Prefix != xmlDiffElement.Prefix)
                {
                    return false;
                }
            }

            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace = this._attributes;
            while (xmlDiffAttributeOrNamespace != null &&
                   xmlDiffAttributeOrNamespace.NodeType == XmlDiffNodeType.Namespace)
            {
                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
            }

            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace1 = this._attributes;
            while (xmlDiffAttributeOrNamespace1 != null)
            {
                if (xmlDiffAttributeOrNamespace1.NodeType == XmlDiffNodeType.Namespace)
                {
                    xmlDiffAttributeOrNamespace1 =
                        (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace1._nextSibling;
                }
                else
                {
                    break;
                }
            }

            while (xmlDiffAttributeOrNamespace != null && xmlDiffAttributeOrNamespace1 != null)
            {
                if (!xmlDiffAttributeOrNamespace.IsSameAs(xmlDiffAttributeOrNamespace1, xmlDiff))
                {
                    return false;
                }

                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                xmlDiffAttributeOrNamespace1 = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace1._nextSibling;
            }

            if (xmlDiffAttributeOrNamespace != null)
            {
                return false;
            }

            return xmlDiffAttributeOrNamespace1 == null;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
            for (XmlDiffNode i = this._firstChildNode; i != null; i = i._nextSibling)
            {
                i.WriteTo(w);
            }
        }

        internal override void WriteTo(XmlWriter w)
        {
            w.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
            for (XmlDiffAttributeOrNamespace i = this._attributes;
                 i != null;
                 i = (XmlDiffAttributeOrNamespace)i._nextSibling)
            {
                i.WriteTo(w);
            }

            this.WriteContentTo(w);
            w.WriteEndElement();
        }
    }
}