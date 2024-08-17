using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class PatchAddNode : XmlPatchParentOperation
    {
        private XmlNodeType _nodeType;

        private string _name;

        private string _ns;

        private string _prefix;

        private string _value;

        private bool _ignoreChildOrder;

        internal PatchAddNode(XmlNodeType nodeType, string name, string ns, string prefix, string value,
            bool ignoreChildOrder)
        {
            this._nodeType = nodeType;
            this._name = name;
            this._ns = ns;
            this._prefix = prefix;
            this._value = value;
            this._ignoreChildOrder = ignoreChildOrder;
        }

        internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
        {
            XmlNode xmlNodes;
            XmlNode xmlNodes1;
            XmlNode xmlNodes2 = null;
            if (this._nodeType == XmlNodeType.Attribute)
            {
                if (this._prefix != "xmlns")
                {
                    xmlNodes2 = (!(this._prefix == "") || !(this._name == "xmlns")
                        ? parent.OwnerDocument.CreateAttribute(this._prefix, this._name, this._ns)
                        : parent.OwnerDocument.CreateAttribute(this._name));
                }
                else
                {
                    xmlNodes2 = parent.OwnerDocument.CreateAttribute(string.Concat(this._prefix, ":", this._name));
                }

                ((XmlAttribute)xmlNodes2).Value = this._value;
                parent.Attributes.Append((XmlAttribute)xmlNodes2);
                return;
            }

            XmlNodeType xmlNodeType = this._nodeType;
            switch (xmlNodeType)
            {
                case XmlNodeType.Element:
                {
                    xmlNodes2 = parent.OwnerDocument.CreateElement(this._prefix, this._name, this._ns);
                    base.ApplyChildren(xmlNodes2);
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.Attribute:
                case XmlNodeType.Entity:
                case XmlNodeType.Document:
                {
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.Text:
                {
                    xmlNodes2 = parent.OwnerDocument.CreateTextNode(this._value);
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.CDATA:
                {
                    xmlNodes2 = parent.OwnerDocument.CreateCDataSection(this._value);
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.EntityReference:
                {
                    xmlNodes2 = parent.OwnerDocument.CreateEntityReference(this._name);
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.ProcessingInstruction:
                {
                    xmlNodes2 = parent.OwnerDocument.CreateProcessingInstruction(this._name, this._value);
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.Comment:
                {
                    xmlNodes2 = parent.OwnerDocument.CreateComment(this._value);
                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
                case XmlNodeType.DocumentType:
                {
                    XmlDocument ownerDocument = parent.OwnerDocument;
                    if (this._prefix == string.Empty)
                    {
                        this._prefix = null;
                    }

                    if (this._ns == string.Empty)
                    {
                        this._ns = null;
                    }

                    XmlDocumentType xmlDocumentType =
                        ownerDocument.CreateDocumentType(this._name, this._prefix, this._ns, this._value);
                    if (ownerDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        ownerDocument.InsertAfter(xmlDocumentType, ownerDocument.FirstChild);
                        return;
                    }

                    ownerDocument.InsertBefore(xmlDocumentType, ownerDocument.FirstChild);
                    return;
                }
                default:
                {
                    if (xmlNodeType == XmlNodeType.XmlDeclaration)
                    {
                        XmlDocument xmlDocument = parent.OwnerDocument;
                        XmlDeclaration xmlDeclaration =
                            xmlDocument.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
                        xmlDeclaration.Value = this._value;
                        xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.FirstChild);
                        return;
                    }

                    if (!this._ignoreChildOrder)
                    {
                        xmlNodes1 = parent.InsertAfter(xmlNodes2, currentPosition);
                    }
                    else
                    {
                        xmlNodes = parent.AppendChild(xmlNodes2);
                    }

                    currentPosition = xmlNodes2;
                    return;
                }
            }
        }
    }
}