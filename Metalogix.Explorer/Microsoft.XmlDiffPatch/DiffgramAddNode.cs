using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramAddNode : DiffgramParentOperation
    {
        private XmlDiffNode _targetNode;

        internal DiffgramAddNode(XmlDiffNode targetNode, ulong operationID) : base(operationID)
        {
            this._targetNode = targetNode;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            int nodeType;
            xmlWriter.WriteStartElement("xd", "add", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            XmlDiffNodeType xmlDiffNodeType = this._targetNode.NodeType;
            switch (xmlDiffNodeType)
            {
                case XmlDiffNodeType.XmlDeclaration:
                {
                    xmlWriter.WriteAttributeString("type", 17.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteString(((XmlDiffXmlDeclaration)this._targetNode).Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.DocumentType:
                {
                    xmlWriter.WriteAttributeString("type", 10.ToString());
                    XmlDiffDocumentType xmlDiffDocumentType = (XmlDiffDocumentType)this._targetNode;
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteAttributeString("name", xmlDiffDocumentType.Name);
                    if (xmlDiffDocumentType.PublicId != string.Empty)
                    {
                        xmlWriter.WriteAttributeString("publicId", xmlDiffDocumentType.PublicId);
                    }

                    if (xmlDiffDocumentType.SystemId != string.Empty)
                    {
                        xmlWriter.WriteAttributeString("systemId", xmlDiffDocumentType.SystemId);
                    }

                    if (xmlDiffDocumentType.Subset == string.Empty)
                    {
                        xmlWriter.WriteEndElement();
                        return;
                    }

                    xmlWriter.WriteCData(xmlDiffDocumentType.Subset);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.None:
                case XmlDiffNodeType.Attribute | XmlDiffNodeType.CDATA:
                case XmlDiffNodeType.Document:
                case XmlDiffNodeType.Attribute | XmlDiffNodeType.Comment:
                case XmlDiffNodeType.Element | XmlDiffNodeType.Attribute | XmlDiffNodeType.Text |
                     XmlDiffNodeType.Comment | XmlDiffNodeType.Document:
                case XmlDiffNodeType.CDATA | XmlDiffNodeType.Comment:
                case XmlDiffNodeType.Element | XmlDiffNodeType.CDATA | XmlDiffNodeType.Comment |
                     XmlDiffNodeType.Document | XmlDiffNodeType.EntityReference:
                {
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.Element:
                {
                    xmlWriter.WriteAttributeString("type", 1.ToString());
                    XmlDiffElement xmlDiffElement = this._targetNode as XmlDiffElement;
                    xmlWriter.WriteAttributeString("name", xmlDiffElement.LocalName);
                    if (xmlDiffElement.NamespaceURI != string.Empty)
                    {
                        xmlWriter.WriteAttributeString("ns", xmlDiffElement.NamespaceURI);
                    }

                    if (xmlDiffElement.Prefix != string.Empty)
                    {
                        xmlWriter.WriteAttributeString("prefix", xmlDiffElement.Prefix);
                    }

                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    base.WriteChildrenTo(xmlWriter, xmlDiff);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.Attribute:
                {
                    xmlWriter.WriteAttributeString("type", 2.ToString());
                    XmlDiffAttribute xmlDiffAttribute = this._targetNode as XmlDiffAttribute;
                    xmlWriter.WriteAttributeString("name", xmlDiffAttribute.LocalName);
                    if (xmlDiffAttribute.NamespaceURI != string.Empty)
                    {
                        xmlWriter.WriteAttributeString("ns", xmlDiffAttribute.NamespaceURI);
                    }

                    if (xmlDiffAttribute.Prefix != string.Empty)
                    {
                        xmlWriter.WriteAttributeString("prefix", xmlDiffAttribute.Prefix);
                    }

                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteString(xmlDiffAttribute.Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.Text:
                {
                    nodeType = (int)this._targetNode.NodeType;
                    xmlWriter.WriteAttributeString("type", nodeType.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteString((this._targetNode as XmlDiffCharData).Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.CDATA:
                {
                    nodeType = (int)this._targetNode.NodeType;
                    xmlWriter.WriteAttributeString("type", nodeType.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteCData((this._targetNode as XmlDiffCharData).Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.EntityReference:
                {
                    xmlWriter.WriteAttributeString("type", 5.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteAttributeString("name", ((XmlDiffER)this._targetNode).Name);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.ProcessingInstruction:
                {
                    nodeType = (int)this._targetNode.NodeType;
                    xmlWriter.WriteAttributeString("type", nodeType.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    XmlDiffPI xmlDiffPI = this._targetNode as XmlDiffPI;
                    xmlWriter.WriteProcessingInstruction(xmlDiffPI.Name, xmlDiffPI.Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.Comment:
                {
                    nodeType = (int)this._targetNode.NodeType;
                    xmlWriter.WriteAttributeString("type", nodeType.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteComment((this._targetNode as XmlDiffCharData).Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                case XmlDiffNodeType.SignificantWhitespace:
                {
                    nodeType = (int)this._targetNode.NodeType;
                    xmlWriter.WriteAttributeString("type", nodeType.ToString());
                    if (this._operationID != (long)0)
                    {
                        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                    }

                    xmlWriter.WriteString(((XmlDiffCharData)this._targetNode).Value);
                    xmlWriter.WriteEndElement();
                    return;
                }
                default:
                {
                    if (xmlDiffNodeType == XmlDiffNodeType.Namespace)
                    {
                        xmlWriter.WriteAttributeString("type", 2.ToString());
                        XmlDiffNamespace xmlDiffNamespace = this._targetNode as XmlDiffNamespace;
                        if (xmlDiffNamespace.Prefix == string.Empty)
                        {
                            xmlWriter.WriteAttributeString("name", "xmlns");
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("prefix", "xmlns");
                            xmlWriter.WriteAttributeString("name", xmlDiffNamespace.Prefix);
                        }

                        if (this._operationID != (long)0)
                        {
                            xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
                        }

                        xmlWriter.WriteString(xmlDiffNamespace.NamespaceURI);
                        xmlWriter.WriteEndElement();
                        return;
                    }
                    else
                    {
                        xmlWriter.WriteEndElement();
                        return;
                    }
                }
            }
        }
    }
}