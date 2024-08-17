using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffDocument : XmlDiffParentNode
    {
        protected XmlDiff _XmlDiff;

        private bool _bLoaded;

        private XmlDiffNode _curLastChild;

        private XmlHash _xmlHash;

        internal bool IsFragment
        {
            get
            {
                XmlDiffNode xmlDiffNode = this._firstChildNode;
                while (xmlDiffNode != null && xmlDiffNode.NodeType != XmlDiffNodeType.Element)
                {
                    xmlDiffNode = xmlDiffNode._nextSibling;
                }

                if (xmlDiffNode == null)
                {
                    return true;
                }

                xmlDiffNode = xmlDiffNode._nextSibling;
                while (xmlDiffNode != null && xmlDiffNode.NodeType != XmlDiffNodeType.Element)
                {
                    xmlDiffNode = xmlDiffNode._nextSibling;
                }

                return xmlDiffNode != null;
            }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.Document; }
        }

        internal XmlDiffDocument(XmlDiff xmlDiff) : base(0)
        {
            this._bLoaded = false;
            this._XmlDiff = xmlDiff;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.ComputeHashXmlDiffDocument(this);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.Document)
            {
                return XmlDiffOperation.Undefined;
            }

            return XmlDiffOperation.Match;
        }

        private void InsertAttributeOrNamespace(XmlDiffElement element, XmlDiffAttributeOrNamespace newAttrOrNs)
        {
            element.InsertAttributeOrNamespace(newAttrOrNs);
        }

        private void InsertChild(XmlDiffParentNode parent, XmlDiffNode newChild)
        {
            if (!this._XmlDiff.IgnoreChildOrder)
            {
                parent.InsertChildNodeAfter(this._curLastChild, newChild);
                this._curLastChild = newChild;
                return;
            }

            XmlDiffNode firstChildNode = parent.FirstChildNode;
            XmlDiffNode xmlDiffNode = null;
            while (firstChildNode != null && XmlDiffDocument.OrderChildren(firstChildNode, newChild) <= 0)
            {
                xmlDiffNode = firstChildNode;
                firstChildNode = firstChildNode._nextSibling;
            }

            parent.InsertChildNodeAfter(xmlDiffNode, newChild);
        }

        internal virtual void Load(XmlReader reader, XmlHash xmlHash)
        {
            if (this._bLoaded)
            {
                throw new InvalidOperationException("The document already contains data and should not be used again.");
            }

            try
            {
                this._curLastChild = null;
                this._xmlHash = xmlHash;
                this.LoadChildNodes(this, reader, false);
                this.ComputeHashValue(this._xmlHash);
                this._bLoaded = true;
            }
            finally
            {
                this._xmlHash = null;
            }
        }

        internal virtual void Load(XmlNode node, XmlHash xmlHash)
        {
            if (this._bLoaded)
            {
                throw new InvalidOperationException("The document already contains data and should not be used again.");
            }

            if (node.NodeType == XmlNodeType.Attribute || node.NodeType == XmlNodeType.Entity ||
                node.NodeType == XmlNodeType.Notation || node.NodeType == XmlNodeType.Whitespace)
            {
                throw new ArgumentException("Invalid node type.");
            }

            try
            {
                this._curLastChild = null;
                this._xmlHash = xmlHash;
                if (node.NodeType == XmlNodeType.Document || node.NodeType == XmlNodeType.DocumentFragment)
                {
                    this.LoadChildNodes(this, node);
                    this.ComputeHashValue(this._xmlHash);
                }
                else
                {
                    int num = 0;
                    XmlDiffNode xmlDiffNode = this.LoadNode(node, ref num);
                    if (xmlDiffNode != null)
                    {
                        this.InsertChildNodeAfter(null, xmlDiffNode);
                        this._hashValue = xmlDiffNode.HashValue;
                    }
                }

                this._bLoaded = true;
            }
            finally
            {
                this._xmlHash = null;
            }
        }

        internal void LoadChildNodes(XmlDiffParentNode parent, XmlReader reader, bool bEmptyElement)
        {
            XmlDiffNode xmlDiffNode = this._curLastChild;
            this._curLastChild = null;
            while (reader.MoveToNextAttribute())
            {
                if (reader.Prefix == "xmlns")
                {
                    if (this._XmlDiff.IgnoreNamespaces)
                    {
                        continue;
                    }

                    XmlDiffNamespace xmlDiffNamespace = new XmlDiffNamespace(reader.LocalName, reader.Value);
                    xmlDiffNamespace.ComputeHashValue(this._xmlHash);
                    this.InsertAttributeOrNamespace((XmlDiffElement)parent, xmlDiffNamespace);
                }
                else if (!(reader.Prefix == string.Empty) || !(reader.LocalName == "xmlns"))
                {
                    string str = (this._XmlDiff.IgnoreWhitespace ? XmlDiff.NormalizeText(reader.Value) : reader.Value);
                    XmlDiffAttribute xmlDiffAttribute =
                        new XmlDiffAttribute(reader.LocalName, reader.Prefix, reader.NamespaceURI, str);
                    xmlDiffAttribute.ComputeHashValue(this._xmlHash);
                    this.InsertAttributeOrNamespace((XmlDiffElement)parent, xmlDiffAttribute);
                }
                else
                {
                    if (this._XmlDiff.IgnoreNamespaces)
                    {
                        continue;
                    }

                    XmlDiffNamespace xmlDiffNamespace1 = new XmlDiffNamespace(string.Empty, reader.Value);
                    xmlDiffNamespace1.ComputeHashValue(this._xmlHash);
                    this.InsertAttributeOrNamespace((XmlDiffElement)parent, xmlDiffNamespace1);
                }
            }

            if (!bEmptyElement)
            {
                int num = 0;
                if (reader.Read())
                {
                    do
                    {
                        if (reader.NodeType == XmlNodeType.Whitespace)
                        {
                            continue;
                        }

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                            {
                                bool isEmptyElement = reader.IsEmptyElement;
                                int num1 = num + 1;
                                num = num1;
                                XmlDiffElement xmlDiffElement = new XmlDiffElement(num1, reader.LocalName,
                                    reader.Prefix, reader.NamespaceURI);
                                this.LoadChildNodes(xmlDiffElement, reader, isEmptyElement);
                                xmlDiffElement.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffElement);
                                continue;
                            }
                            case XmlNodeType.Text:
                            {
                                string str1 = (this._XmlDiff.IgnoreWhitespace
                                    ? XmlDiff.NormalizeText(reader.Value)
                                    : reader.Value);
                                int num2 = num + 1;
                                num = num2;
                                XmlDiffCharData xmlDiffCharDatum =
                                    new XmlDiffCharData(num2, str1, XmlDiffNodeType.Text);
                                xmlDiffCharDatum.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffCharDatum);
                                continue;
                            }
                            case XmlNodeType.CDATA:
                            {
                                int num3 = num + 1;
                                num = num3;
                                XmlDiffCharData xmlDiffCharDatum1 =
                                    new XmlDiffCharData(num3, reader.Value, XmlDiffNodeType.CDATA);
                                xmlDiffCharDatum1.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffCharDatum1);
                                continue;
                            }
                            case XmlNodeType.EntityReference:
                            {
                                int num4 = num + 1;
                                num = num4;
                                XmlDiffER xmlDiffER = new XmlDiffER(num4, reader.Name);
                                xmlDiffER.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffER);
                                continue;
                            }
                            case XmlNodeType.ProcessingInstruction:
                            {
                                num++;
                                if (this._XmlDiff.IgnorePI)
                                {
                                    continue;
                                }

                                XmlDiffPI xmlDiffPI = new XmlDiffPI(num, reader.Name, reader.Value);
                                xmlDiffPI.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffPI);
                                continue;
                            }
                            case XmlNodeType.Comment:
                            {
                                num++;
                                if (this._XmlDiff.IgnoreComments)
                                {
                                    continue;
                                }

                                XmlDiffCharData xmlDiffCharDatum2 =
                                    new XmlDiffCharData(num, reader.Value, XmlDiffNodeType.Comment);
                                xmlDiffCharDatum2.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffCharDatum2);
                                continue;
                            }
                            case XmlNodeType.DocumentType:
                            {
                                num++;
                                if (this._XmlDiff.IgnoreDtd)
                                {
                                    continue;
                                }

                                XmlDiffDocumentType xmlDiffDocumentType = new XmlDiffDocumentType(num, reader.Name,
                                    reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                                xmlDiffDocumentType.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffDocumentType);
                                continue;
                            }
                            case XmlNodeType.SignificantWhitespace:
                            {
                                if (reader.XmlSpace != XmlSpace.Preserve)
                                {
                                    continue;
                                }

                                num++;
                                if (this._XmlDiff.IgnoreWhitespace)
                                {
                                    continue;
                                }

                                XmlDiffCharData xmlDiffCharDatum3 = new XmlDiffCharData(num, reader.Value,
                                    XmlDiffNodeType.SignificantWhitespace);
                                xmlDiffCharDatum3.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffCharDatum3);
                                continue;
                            }
                            case XmlNodeType.EndElement:
                            {
                                break;
                            }
                            case XmlNodeType.XmlDeclaration:
                            {
                                num++;
                                if (this._XmlDiff.IgnoreXmlDecl)
                                {
                                    continue;
                                }

                                XmlDiffXmlDeclaration xmlDiffXmlDeclaration =
                                    new XmlDiffXmlDeclaration(num, XmlDiff.NormalizeXmlDeclaration(reader.Value));
                                xmlDiffXmlDeclaration.ComputeHashValue(this._xmlHash);
                                this.InsertChild(parent, xmlDiffXmlDeclaration);
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    } while (reader.Read());
                }
            }

            this._curLastChild = xmlDiffNode;
        }

        internal void LoadChildNodes(XmlDiffParentNode parent, XmlNode parentDomNode)
        {
            XmlDiffNode xmlDiffNode = this._curLastChild;
            this._curLastChild = null;
            XmlNamedNodeMap attributes = parentDomNode.Attributes;
            if (attributes != null && attributes.Count > 0)
            {
                IEnumerator enumerator = attributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    XmlAttribute current = (XmlAttribute)enumerator.Current;
                    if (current.Prefix == "xmlns")
                    {
                        if (this._XmlDiff.IgnoreNamespaces)
                        {
                            continue;
                        }

                        XmlDiffNamespace xmlDiffNamespace = new XmlDiffNamespace(current.LocalName, current.Value);
                        xmlDiffNamespace.ComputeHashValue(this._xmlHash);
                        this.InsertAttributeOrNamespace((XmlDiffElement)parent, xmlDiffNamespace);
                    }
                    else if (!(current.Prefix == string.Empty) || !(current.LocalName == "xmlns"))
                    {
                        string str = (this._XmlDiff.IgnoreWhitespace
                            ? XmlDiff.NormalizeText(current.Value)
                            : current.Value);
                        XmlDiffAttribute xmlDiffAttribute = new XmlDiffAttribute(current.LocalName, current.Prefix,
                            current.NamespaceURI, str);
                        xmlDiffAttribute.ComputeHashValue(this._xmlHash);
                        this.InsertAttributeOrNamespace((XmlDiffElement)parent, xmlDiffAttribute);
                    }
                    else
                    {
                        if (this._XmlDiff.IgnoreNamespaces)
                        {
                            continue;
                        }

                        XmlDiffNamespace xmlDiffNamespace1 = new XmlDiffNamespace(string.Empty, current.Value);
                        xmlDiffNamespace1.ComputeHashValue(this._xmlHash);
                        this.InsertAttributeOrNamespace((XmlDiffElement)parent, xmlDiffNamespace1);
                    }
                }
            }

            XmlNodeList childNodes = parentDomNode.ChildNodes;
            if (childNodes.Count != 0)
            {
                int num = 0;
                IEnumerator enumerator1 = childNodes.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    if (((XmlNode)enumerator1.Current).NodeType == XmlNodeType.Whitespace)
                    {
                        continue;
                    }

                    XmlDiffNode xmlDiffNode1 = this.LoadNode((XmlNode)enumerator1.Current, ref num);
                    if (xmlDiffNode1 == null)
                    {
                        continue;
                    }

                    this.InsertChild(parent, xmlDiffNode1);
                }
            }

            this._curLastChild = xmlDiffNode;
        }

        internal XmlDiffNode LoadNode(XmlNode node, ref int childPosition)
        {
            int num;
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                {
                    int num1 = childPosition + 1;
                    num = num1;
                    childPosition = num1;
                    XmlDiffElement xmlDiffElement =
                        new XmlDiffElement(num, node.LocalName, node.Prefix, node.NamespaceURI);
                    this.LoadChildNodes(xmlDiffElement, node);
                    xmlDiffElement.ComputeHashValue(this._xmlHash);
                    return xmlDiffElement;
                }
                case XmlNodeType.Attribute:
                {
                    return null;
                }
                case XmlNodeType.Text:
                {
                    string str = (this._XmlDiff.IgnoreWhitespace ? XmlDiff.NormalizeText(node.Value) : node.Value);
                    int num2 = childPosition + 1;
                    num = num2;
                    childPosition = num2;
                    XmlDiffCharData xmlDiffCharDatum = new XmlDiffCharData(num, str, XmlDiffNodeType.Text);
                    xmlDiffCharDatum.ComputeHashValue(this._xmlHash);
                    return xmlDiffCharDatum;
                }
                case XmlNodeType.CDATA:
                {
                    int num3 = childPosition + 1;
                    num = num3;
                    childPosition = num3;
                    XmlDiffCharData xmlDiffCharDatum1 = new XmlDiffCharData(num, node.Value, XmlDiffNodeType.CDATA);
                    xmlDiffCharDatum1.ComputeHashValue(this._xmlHash);
                    return xmlDiffCharDatum1;
                }
                case XmlNodeType.EntityReference:
                {
                    int num4 = childPosition + 1;
                    num = num4;
                    childPosition = num4;
                    XmlDiffER xmlDiffER = new XmlDiffER(num, node.Name);
                    xmlDiffER.ComputeHashValue(this._xmlHash);
                    return xmlDiffER;
                }
                case XmlNodeType.Entity:
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.Notation:
                case XmlNodeType.Whitespace:
                case XmlNodeType.EndEntity:
                {
                    return null;
                }
                case XmlNodeType.ProcessingInstruction:
                {
                    childPosition++;
                    if (this._XmlDiff.IgnorePI)
                    {
                        return null;
                    }

                    XmlDiffPI xmlDiffPI = new XmlDiffPI(childPosition, node.Name, node.Value);
                    xmlDiffPI.ComputeHashValue(this._xmlHash);
                    return xmlDiffPI;
                }
                case XmlNodeType.Comment:
                {
                    childPosition++;
                    if (this._XmlDiff.IgnoreComments)
                    {
                        return null;
                    }

                    XmlDiffCharData xmlDiffCharDatum2 =
                        new XmlDiffCharData(childPosition, node.Value, XmlDiffNodeType.Comment);
                    xmlDiffCharDatum2.ComputeHashValue(this._xmlHash);
                    return xmlDiffCharDatum2;
                }
                case XmlNodeType.DocumentType:
                {
                    childPosition++;
                    if (this._XmlDiff.IgnoreDtd)
                    {
                        return null;
                    }

                    XmlDocumentType xmlDocumentType = (XmlDocumentType)node;
                    XmlDiffDocumentType xmlDiffDocumentType = new XmlDiffDocumentType(childPosition,
                        xmlDocumentType.Name, xmlDocumentType.PublicId, xmlDocumentType.SystemId,
                        xmlDocumentType.InternalSubset);
                    xmlDiffDocumentType.ComputeHashValue(this._xmlHash);
                    return xmlDiffDocumentType;
                }
                case XmlNodeType.SignificantWhitespace:
                {
                    childPosition++;
                    if (this._XmlDiff.IgnoreWhitespace)
                    {
                        return null;
                    }

                    XmlDiffCharData xmlDiffCharDatum3 = new XmlDiffCharData(childPosition, node.Value,
                        XmlDiffNodeType.SignificantWhitespace);
                    xmlDiffCharDatum3.ComputeHashValue(this._xmlHash);
                    return xmlDiffCharDatum3;
                }
                case XmlNodeType.EndElement:
                {
                    return null;
                }
                case XmlNodeType.XmlDeclaration:
                {
                    childPosition++;
                    if (this._XmlDiff.IgnoreXmlDecl)
                    {
                        return null;
                    }

                    XmlDiffXmlDeclaration xmlDiffXmlDeclaration =
                        new XmlDiffXmlDeclaration(childPosition, XmlDiff.NormalizeXmlDeclaration(node.Value));
                    xmlDiffXmlDeclaration.ComputeHashValue(this._xmlHash);
                    return xmlDiffXmlDeclaration;
                }
                default:
                {
                    return null;
                }
            }
        }

        internal static int OrderAttributesOrNamespaces(XmlDiffAttributeOrNamespace node1,
            XmlDiffAttributeOrNamespace node2)
        {
            if (node1.NodeType != node2.NodeType)
            {
                if (node1.NodeType == XmlDiffNodeType.Namespace)
                {
                    return -1;
                }

                return 1;
            }

            int num = XmlDiffDocument.OrderStrings(node1.LocalName, node2.LocalName);
            int num1 = num;
            if (num == 0)
            {
                int num2 = XmlDiffDocument.OrderStrings(node1.Prefix, node2.Prefix);
                num1 = num2;
                if (num2 == 0)
                {
                    int num3 = XmlDiffDocument.OrderStrings(node1.NamespaceURI, node2.NamespaceURI);
                    num1 = num3;
                    if (num3 == 0)
                    {
                        int num4 = XmlDiffDocument.OrderStrings(node1.Value, node2.Value);
                        num1 = num4;
                        if (num4 == 0)
                        {
                            return 0;
                        }
                    }
                }
            }

            return num1;
        }

        internal static int OrderCharacterData(XmlDiffCharData t1, XmlDiffCharData t2)
        {
            return XmlDiffDocument.OrderStrings(t1.Value, t2.Value);
        }

        internal static int OrderChildren(XmlDiffNode node1, XmlDiffNode node2)
        {
            int nodeType = (int)node1.NodeType;
            int num = (int)node2.NodeType;
            if (nodeType < num)
            {
                return -1;
            }

            if (num < nodeType)
            {
                return 1;
            }

            int num1 = nodeType;
            switch (num1)
            {
                case 1:
                {
                    return XmlDiffDocument.OrderElements(node1 as XmlDiffElement, node2 as XmlDiffElement);
                }
                case 2:
                {
                    return 0;
                }
                case 3:
                case 4:
                case 6:
                {
                    return XmlDiffDocument.OrderCharacterData(node1 as XmlDiffCharData, node2 as XmlDiffCharData);
                }
                case 5:
                {
                    return XmlDiffDocument.OrderERs(node1 as XmlDiffER, node2 as XmlDiffER);
                }
                case 7:
                {
                    return XmlDiffDocument.OrderPIs(node1 as XmlDiffPI, node2 as XmlDiffPI);
                }
                default:
                {
                    switch (num1)
                    {
                        case 100:
                        {
                            return 0;
                        }
                        case 101:
                        {
                            if (((XmlDiffShrankNode)node1).MatchingShrankNode ==
                                ((XmlDiffShrankNode)node2).MatchingShrankNode)
                            {
                                return 0;
                            }

                            if (((XmlDiffShrankNode)node1).HashValue >= ((XmlDiffShrankNode)node2).HashValue)
                            {
                                return 1;
                            }

                            return -1;
                        }
                        default:
                        {
                            return XmlDiffDocument.OrderCharacterData(node1 as XmlDiffCharData,
                                node2 as XmlDiffCharData);
                        }
                    }

                    break;
                }
            }
        }

        internal static int OrderElements(XmlDiffElement elem1, XmlDiffElement elem2)
        {
            int num = XmlDiffDocument.OrderStrings(elem1.LocalName, elem2.LocalName);
            int num1 = num;
            if (num == 0)
            {
                int num2 = XmlDiffDocument.OrderStrings(elem1.NamespaceURI, elem2.NamespaceURI);
                num1 = num2;
                if (num2 == 0)
                {
                    return XmlDiffDocument.OrderSubTrees(elem1, elem2);
                }
            }

            return num1;
        }

        internal static int OrderERs(XmlDiffER er1, XmlDiffER er2)
        {
            return XmlDiffDocument.OrderStrings(er1.Name, er2.Name);
        }

        internal static int OrderPIs(XmlDiffPI pi1, XmlDiffPI pi2)
        {
            int num = 0;
            int num1 = XmlDiffDocument.OrderStrings(pi1.Name, pi2.Name);
            num = num1;
            if (num1 == 0)
            {
                int num2 = XmlDiffDocument.OrderStrings(pi1.Value, pi2.Value);
                num = num2;
                if (num2 == 0)
                {
                    return 0;
                }
            }

            return num;
        }

        internal static int OrderStrings(string s1, string s2)
        {
            int num = (s1.Length < s2.Length ? s1.Length : s2.Length);
            int num1 = 0;
            while (num1 < num && s1[num1] == s2[num1])
            {
                num1++;
            }

            if (num1 < num)
            {
                if (s1[num1] >= s2[num1])
                {
                    return 1;
                }

                return -1;
            }

            if (s1.Length == s2.Length)
            {
                return 0;
            }

            if (s2.Length <= s1.Length)
            {
                return 1;
            }

            return -1;
        }

        internal static int OrderSubTrees(XmlDiffElement elem1, XmlDiffElement elem2)
        {
            XmlDiffNode i;
            int num = 0;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace = elem1._attributes;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace1 = elem2._attributes;
            while (xmlDiffAttributeOrNamespace != null)
            {
                if (xmlDiffAttributeOrNamespace.NodeType == XmlDiffNodeType.Namespace)
                {
                    xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                }
                else
                {
                    break;
                }
            }

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
                int num1 = XmlDiffDocument.OrderAttributesOrNamespaces(xmlDiffAttributeOrNamespace,
                    xmlDiffAttributeOrNamespace1);
                num = num1;
                if (num1 != 0)
                {
                    return num;
                }

                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                xmlDiffAttributeOrNamespace1 = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace1._nextSibling;
            }

            if (xmlDiffAttributeOrNamespace != xmlDiffAttributeOrNamespace1)
            {
                if (xmlDiffAttributeOrNamespace == null)
                {
                    return 1;
                }

                return -1;
            }

            XmlDiffNode firstChildNode = elem1.FirstChildNode;
            for (i = elem2.FirstChildNode; firstChildNode != null && i != null; i = i._nextSibling)
            {
                int num2 = XmlDiffDocument.OrderChildren(firstChildNode, i);
                num = num2;
                if (num2 != 0)
                {
                    return num;
                }

                firstChildNode = firstChildNode._nextSibling;
            }

            if (firstChildNode == i)
            {
                return 0;
            }

            if (firstChildNode == null)
            {
                return 1;
            }

            return -1;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
            for (XmlDiffNode i = this.FirstChildNode; i != null; i = i._nextSibling)
            {
                i.WriteTo(w);
            }
        }

        internal override void WriteTo(XmlWriter w)
        {
            this.WriteContentTo(w);
        }
    }
}