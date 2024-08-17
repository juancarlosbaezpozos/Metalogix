using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlHash
    {
        private const string Delimiter = "\0x01";

        private bool _bIgnoreChildOrder = false;

        private bool _bIgnoreComments = false;

        private bool _bIgnorePI = false;

        private bool _bIgnoreWhitespace = false;

        private bool _bIgnoreNamespaces = false;

        private bool _bIgnorePrefixes = false;

        private bool _bIgnoreXmlDecl = false;

        private bool _bIgnoreDtd = false;

        internal XmlHash(XmlDiff xmlDiff)
        {
            this._bIgnoreChildOrder = xmlDiff.IgnoreChildOrder;
            this._bIgnoreComments = xmlDiff.IgnoreComments;
            this._bIgnorePI = xmlDiff.IgnorePI;
            this._bIgnoreWhitespace = xmlDiff.IgnoreWhitespace;
            this._bIgnoreNamespaces = xmlDiff.IgnoreNamespaces;
            this._bIgnorePrefixes = xmlDiff.IgnorePrefixes;
            this._bIgnoreXmlDecl = xmlDiff.IgnoreXmlDecl;
            this._bIgnoreDtd = xmlDiff.IgnoreDtd;
        }

        internal XmlHash()
        {
        }

        private void ClearFlags()
        {
            this._bIgnoreChildOrder = false;
            this._bIgnoreComments = false;
            this._bIgnorePI = false;
            this._bIgnoreWhitespace = false;
            this._bIgnoreNamespaces = false;
            this._bIgnorePrefixes = false;
            this._bIgnoreXmlDecl = false;
            this._bIgnoreDtd = false;
        }

        internal ulong ComputeHash(XmlNode node, XmlDiffOptions options)
        {
            this._bIgnoreChildOrder = (options & XmlDiffOptions.IgnoreChildOrder) > XmlDiffOptions.None;
            this._bIgnoreComments = (options & XmlDiffOptions.IgnoreComments) > XmlDiffOptions.None;
            this._bIgnorePI = (options & XmlDiffOptions.IgnorePI) > XmlDiffOptions.None;
            this._bIgnoreWhitespace = (options & XmlDiffOptions.IgnoreWhitespace) > XmlDiffOptions.None;
            this._bIgnoreNamespaces = (options & XmlDiffOptions.IgnoreNamespaces) > XmlDiffOptions.None;
            this._bIgnorePrefixes = (options & XmlDiffOptions.IgnorePrefixes) > XmlDiffOptions.None;
            this._bIgnoreXmlDecl = (options & XmlDiffOptions.IgnoreXmlDecl) > XmlDiffOptions.None;
            this._bIgnoreDtd = (options & XmlDiffOptions.IgnoreDtd) > XmlDiffOptions.None;
            return this.ComputeHash(node);
        }

        internal ulong ComputeHash(XmlNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Document:
                {
                    return this.ComputeHashXmlDocument((XmlDocument)node);
                }
                case XmlNodeType.DocumentType:
                {
                    return this.ComputeHashXmlNode(node);
                }
                case XmlNodeType.DocumentFragment:
                {
                    return this.ComputeHashXmlFragment((XmlDocumentFragment)node);
                }
                default:
                {
                    return this.ComputeHashXmlNode(node);
                }
            }
        }

        private void ComputeHashXmlChildren(HashAlgorithm ha, XmlNode parent)
        {
            XmlElement xmlElement = parent as XmlElement;
            if (xmlElement != null)
            {
                ulong num = 0uL;
                int num2 = 0;
                XmlAttributeCollection attributes = ((XmlElement)parent).Attributes;
                int i = 0;
                while (i < attributes.Count)
                {
                    XmlAttribute xmlAttribute = (XmlAttribute)attributes.Item(i);
                    ulong num3;
                    if (xmlAttribute.LocalName == "xmlns" && xmlAttribute.Prefix == string.Empty)
                    {
                        if (!this._bIgnoreNamespaces)
                        {
                            num3 = this.HashNamespace(string.Empty, xmlAttribute.Value);
                            goto IL_122;
                        }
                    }
                    else if (xmlAttribute.Prefix == "xmlns")
                    {
                        if (!this._bIgnoreNamespaces)
                        {
                            num3 = this.HashNamespace(xmlAttribute.LocalName, xmlAttribute.Value);
                            goto IL_122;
                        }
                    }
                    else
                    {
                        if (this._bIgnoreWhitespace)
                        {
                            num3 = this.HashAttribute(xmlAttribute.LocalName, xmlAttribute.Prefix,
                                xmlAttribute.NamespaceURI, XmlDiff.NormalizeText(xmlAttribute.Value));
                            goto IL_122;
                        }

                        num3 = this.HashAttribute(xmlAttribute.LocalName, xmlAttribute.Prefix,
                            xmlAttribute.NamespaceURI, xmlAttribute.Value);
                        goto IL_122;
                    }

                    IL_12B:
                    i++;
                    continue;
                    IL_122:
                    num2++;
                    num += num3;
                    goto IL_12B;
                }

                if (num2 != 0)
                {
                    ha.AddULong(num);
                    ha.AddInt(num2);
                }
            }

            int num4 = 0;
            if (this._bIgnoreChildOrder)
            {
                ulong num5 = 0uL;
                for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
                {
                    ulong num6 = this.ComputeHashXmlNode(xmlNode);
                    if (num6 != 0uL)
                    {
                        num5 += num6;
                        num4++;
                    }
                }

                ha.AddULong(num5);
            }
            else
            {
                for (XmlNode xmlNode2 = parent.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                {
                    ulong num7 = this.ComputeHashXmlNode(xmlNode2);
                    if (num7 != 0uL)
                    {
                        ha.AddULong(num7);
                        num4++;
                    }
                }
            }

            if (num4 != 0)
            {
                ha.AddInt(num4);
            }
        }

        private void ComputeHashXmlDiffAttributes(HashAlgorithm ha, XmlDiffElement el)
        {
            int num = 0;
            ulong hashValue = (ulong)0;
            for (XmlDiffAttributeOrNamespace i = el._attributes;
                 i != null;
                 i = (XmlDiffAttributeOrNamespace)i._nextSibling)
            {
                hashValue += i.HashValue;
                num++;
            }

            if (num > 0)
            {
                ha.AddULong(hashValue);
                ha.AddInt(num);
            }
        }

        private void ComputeHashXmlDiffChildren(HashAlgorithm ha, XmlDiffParentNode parent)
        {
            int num = 0;
            if (!this._bIgnoreChildOrder)
            {
                for (XmlDiffNode i = parent.FirstChildNode; i != null; i = i._nextSibling)
                {
                    ha.AddULong(i.HashValue);
                    num++;
                }
            }
            else
            {
                ulong hashValue = (ulong)0;
                for (XmlDiffNode j = parent.FirstChildNode; j != null; j = j._nextSibling)
                {
                    hashValue += j.HashValue;
                    num++;
                }

                ha.AddULong(hashValue);
            }

            if (num != 0)
            {
                ha.AddInt(num);
            }
        }

        internal ulong ComputeHashXmlDiffDocument(XmlDiffDocument doc)
        {
            HashAlgorithm hashAlgorithm = new HashAlgorithm();
            this.HashDocument(hashAlgorithm);
            this.ComputeHashXmlDiffChildren(hashAlgorithm, doc);
            return hashAlgorithm.Hash;
        }

        internal ulong ComputeHashXmlDiffElement(XmlDiffElement el)
        {
            HashAlgorithm hashAlgorithm = new HashAlgorithm();
            this.HashElement(hashAlgorithm, el.LocalName, el.Prefix, el.NamespaceURI);
            this.ComputeHashXmlDiffAttributes(hashAlgorithm, el);
            this.ComputeHashXmlDiffChildren(hashAlgorithm, el);
            return hashAlgorithm.Hash;
        }

        private ulong ComputeHashXmlDocument(XmlDocument doc)
        {
            HashAlgorithm hashAlgorithm = new HashAlgorithm();
            this.HashDocument(hashAlgorithm);
            this.ComputeHashXmlChildren(hashAlgorithm, doc);
            return hashAlgorithm.Hash;
        }

        private ulong ComputeHashXmlFragment(XmlDocumentFragment frag)
        {
            HashAlgorithm hashAlgorithm = new HashAlgorithm();
            this.ComputeHashXmlChildren(hashAlgorithm, frag);
            return hashAlgorithm.Hash;
        }

        private ulong ComputeHashXmlNode(XmlNode node)
        {
            XmlCharacterData xmlCharacterDatum;
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                {
                    XmlElement xmlElement = (XmlElement)node;
                    HashAlgorithm hashAlgorithm = new HashAlgorithm();
                    this.HashElement(hashAlgorithm, xmlElement.LocalName, xmlElement.Prefix, xmlElement.NamespaceURI);
                    this.ComputeHashXmlChildren(hashAlgorithm, xmlElement);
                    return hashAlgorithm.Hash;
                }
                case XmlNodeType.Attribute:
                {
                    return (ulong)0;
                }
                case XmlNodeType.Text:
                {
                    xmlCharacterDatum = (XmlCharacterData)node;
                    if (!this._bIgnoreWhitespace)
                    {
                        return this.HashCharacterNode(xmlCharacterDatum.NodeType, xmlCharacterDatum.Value);
                    }

                    return this.HashCharacterNode(xmlCharacterDatum.NodeType,
                        XmlDiff.NormalizeText(xmlCharacterDatum.Value));
                }
                case XmlNodeType.CDATA:
                {
                    XmlCharacterData xmlCharacterDatum1 = (XmlCharacterData)node;
                    return this.HashCharacterNode(xmlCharacterDatum1.NodeType, xmlCharacterDatum1.Value);
                }
                case XmlNodeType.EntityReference:
                {
                    return this.HashER(((XmlEntityReference)node).Name);
                }
                case XmlNodeType.Entity:
                case XmlNodeType.Document:
                case XmlNodeType.Notation:
                case XmlNodeType.EndElement:
                case XmlNodeType.EndEntity:
                {
                    return (ulong)0;
                }
                case XmlNodeType.ProcessingInstruction:
                {
                    if (this._bIgnorePI)
                    {
                        return (ulong)0;
                    }

                    XmlProcessingInstruction xmlProcessingInstruction = (XmlProcessingInstruction)node;
                    return this.HashPI(xmlProcessingInstruction.Target, xmlProcessingInstruction.Value);
                }
                case XmlNodeType.Comment:
                {
                    if (this._bIgnoreComments)
                    {
                        return (ulong)0;
                    }

                    return this.HashCharacterNode(XmlNodeType.Comment, ((XmlCharacterData)node).Value);
                }
                case XmlNodeType.DocumentType:
                {
                    if (this._bIgnoreDtd)
                    {
                        return (ulong)0;
                    }

                    XmlDocumentType xmlDocumentType = (XmlDocumentType)node;
                    return this.HashDocumentType(xmlDocumentType.Name, xmlDocumentType.PublicId,
                        xmlDocumentType.SystemId, xmlDocumentType.InternalSubset);
                }
                case XmlNodeType.DocumentFragment:
                {
                    return (ulong)0;
                }
                case XmlNodeType.Whitespace:
                {
                    return (ulong)0;
                }
                case XmlNodeType.SignificantWhitespace:
                {
                    if (!this._bIgnoreWhitespace)
                    {
                        xmlCharacterDatum = (XmlCharacterData)node;
                        if (!this._bIgnoreWhitespace)
                        {
                            return this.HashCharacterNode(xmlCharacterDatum.NodeType, xmlCharacterDatum.Value);
                        }

                        return this.HashCharacterNode(xmlCharacterDatum.NodeType,
                            XmlDiff.NormalizeText(xmlCharacterDatum.Value));
                    }

                    return (ulong)0;
                }
                case XmlNodeType.XmlDeclaration:
                {
                    if (this._bIgnoreXmlDecl)
                    {
                        return (ulong)0;
                    }

                    XmlDeclaration xmlDeclaration = (XmlDeclaration)node;
                    return this.HashXmlDeclaration(XmlDiff.NormalizeXmlDeclaration(xmlDeclaration.Value));
                }
                default:
                {
                    return (ulong)0;
                }
            }
        }

        internal ulong HashAttribute(string localName, string prefix, string ns, string value)
        {
            object[] objArray = new object[] { 2, "\0x01", null, null, null, null, null, null, null };
            objArray[2] = (this._bIgnoreNamespaces || this._bIgnorePrefixes ? string.Empty : prefix);
            objArray[3] = "\0x01";
            objArray[4] = (this._bIgnoreNamespaces ? string.Empty : ns);
            objArray[5] = "\0x01";
            objArray[6] = localName;
            objArray[7] = "\0x01";
            objArray[8] = value;
            return HashAlgorithm.GetHash(string.Concat(objArray));
        }

        internal ulong HashCharacterNode(XmlNodeType nodeType, string value)
        {
            int num = (int)nodeType;
            return HashAlgorithm.GetHash(string.Concat(num.ToString(), "\0x01", value));
        }

        private void HashDocument(HashAlgorithm ha)
        {
        }

        internal ulong HashDocumentType(string name, string publicId, string systemId, string subset)
        {
            string[] str = new string[]
                { 10.ToString(), "\0x01", name, "\0x01", publicId, "\0x01", systemId, "\0x01", subset };
            return HashAlgorithm.GetHash(string.Concat(str));
        }

        internal void HashElement(HashAlgorithm ha, string localName, string prefix, string ns)
        {
            HashAlgorithm hashAlgorithm = ha;
            object[] objArray = new object[] { 1, "\0x01", null, null, null, null, null };
            objArray[2] = (this._bIgnoreNamespaces || this._bIgnorePrefixes ? string.Empty : prefix);
            objArray[3] = "\0x01";
            objArray[4] = (this._bIgnoreNamespaces ? string.Empty : ns);
            objArray[5] = "\0x01";
            objArray[6] = localName;
            hashAlgorithm.AddString(string.Concat(objArray));
        }

        internal ulong HashER(string name)
        {
            int num = 5;
            return HashAlgorithm.GetHash(string.Concat(num.ToString(), "\0x01", name));
        }

        internal ulong HashNamespace(string prefix, string ns)
        {
            object[] objArray = new object[] { 100, "\0x01", null, null, null };
            objArray[2] = (this._bIgnorePrefixes ? string.Empty : prefix);
            objArray[3] = "\0x01";
            objArray[4] = ns;
            return HashAlgorithm.GetHash(string.Concat(objArray));
        }

        internal ulong HashPI(string target, string value)
        {
            string[] str = new string[] { 7.ToString(), "\0x01", target, "\0x01", value };
            return HashAlgorithm.GetHash(string.Concat(str));
        }

        internal ulong HashXmlDeclaration(string value)
        {
            int num = 17;
            return HashAlgorithm.GetHash(string.Concat(num.ToString(), "\0x01", value));
        }
    }
}