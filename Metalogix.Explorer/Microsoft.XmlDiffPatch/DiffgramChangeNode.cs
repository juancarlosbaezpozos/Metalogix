using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramChangeNode : DiffgramParentOperation
    {
        private XmlDiffNode _sourceNode;

        private XmlDiffNode _targetNode;

        private XmlDiffOperation _op;

        internal DiffgramChangeNode(XmlDiffNode sourceNode, XmlDiffNode targetNode, XmlDiffOperation op,
            ulong operationID) : base(operationID)
        {
            this._sourceNode = sourceNode;
            this._targetNode = targetNode;
            this._op = op;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            xmlWriter.WriteStartElement("xd", "change", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            xmlWriter.WriteAttributeString("match", this._sourceNode.GetRelativeAddress());
            if (this._operationID != (long)0)
            {
                xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            }

            XmlDiffOperation xmlDiffOperation = this._op;
            if (xmlDiffOperation == XmlDiffOperation.ChangeElementName)
            {
                XmlDiffElement xmlDiffElement = (XmlDiffElement)this._sourceNode;
                XmlDiffElement xmlDiffElement1 = (XmlDiffElement)this._targetNode;
                if (xmlDiffElement.LocalName != xmlDiffElement1.LocalName)
                {
                    xmlWriter.WriteAttributeString("name", xmlDiffElement1.LocalName);
                }

                if (xmlDiffElement.Prefix != xmlDiffElement1.Prefix && !xmlDiff.IgnorePrefixes &&
                    !xmlDiff.IgnoreNamespaces)
                {
                    xmlWriter.WriteAttributeString("prefix", xmlDiffElement1.Prefix);
                }

                if (xmlDiffElement.NamespaceURI != xmlDiffElement1.NamespaceURI && !xmlDiff.IgnoreNamespaces)
                {
                    xmlWriter.WriteAttributeString("ns", xmlDiffElement1.NamespaceURI);
                }

                base.WriteChildrenTo(xmlWriter, xmlDiff);
            }
            else
            {
                switch (xmlDiffOperation)
                {
                    case XmlDiffOperation.ChangePI:
                    {
                        XmlDiffPI xmlDiffPI = (XmlDiffPI)this._sourceNode;
                        XmlDiffPI xmlDiffPI1 = (XmlDiffPI)this._targetNode;
                        if (xmlDiffPI.Value != xmlDiffPI1.Value)
                        {
                            xmlWriter.WriteProcessingInstruction(xmlDiffPI1.Name, xmlDiffPI1.Value);
                            break;
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("name", xmlDiffPI1.Name);
                            break;
                        }
                    }
                    case XmlDiffOperation.ChangeER:
                    {
                        xmlWriter.WriteAttributeString("name", ((XmlDiffER)this._targetNode).Name);
                        break;
                    }
                    case XmlDiffOperation.ChangeCharacterData:
                    {
                        XmlDiffCharData xmlDiffCharDatum = (XmlDiffCharData)this._targetNode;
                        XmlDiffNodeType nodeType = this._targetNode.NodeType;
                        switch (nodeType)
                        {
                            case XmlDiffNodeType.Text:
                            {
                                xmlWriter.WriteString(xmlDiffCharDatum.Value);
                                break;
                            }
                            case XmlDiffNodeType.CDATA:
                            {
                                xmlWriter.WriteCData(xmlDiffCharDatum.Value);
                                break;
                            }
                            default:
                            {
                                if (nodeType == XmlDiffNodeType.Comment)
                                {
                                    xmlWriter.WriteComment(xmlDiffCharDatum.Value);
                                }
                                else if (nodeType == XmlDiffNodeType.SignificantWhitespace)
                                {
                                    goto case XmlDiffNodeType.Text;
                                }

                                break;
                            }
                        }

                        break;
                    }
                    case XmlDiffOperation.ChangeXmlDeclaration:
                    {
                        xmlWriter.WriteString(((XmlDiffXmlDeclaration)this._targetNode).Value);
                        break;
                    }
                    case XmlDiffOperation.ChangeDTD:
                    {
                        XmlDiffDocumentType xmlDiffDocumentType = (XmlDiffDocumentType)this._sourceNode;
                        XmlDiffDocumentType xmlDiffDocumentType1 = (XmlDiffDocumentType)this._targetNode;
                        if (xmlDiffDocumentType.Name != xmlDiffDocumentType1.Name)
                        {
                            xmlWriter.WriteAttributeString("name", xmlDiffDocumentType1.Name);
                        }

                        if (xmlDiffDocumentType.SystemId != xmlDiffDocumentType1.SystemId)
                        {
                            xmlWriter.WriteAttributeString("systemId", xmlDiffDocumentType1.SystemId);
                        }

                        if (xmlDiffDocumentType.PublicId != xmlDiffDocumentType1.PublicId)
                        {
                            xmlWriter.WriteAttributeString("publicId", xmlDiffDocumentType1.PublicId);
                        }

                        if (xmlDiffDocumentType.Subset == xmlDiffDocumentType1.Subset)
                        {
                            break;
                        }

                        xmlWriter.WriteCData(xmlDiffDocumentType1.Subset);
                        break;
                    }
                    case XmlDiffOperation.ChangeAttr:
                    {
                        XmlDiffAttribute xmlDiffAttribute = (XmlDiffAttribute)this._sourceNode;
                        XmlDiffAttribute xmlDiffAttribute1 = (XmlDiffAttribute)this._targetNode;
                        if (xmlDiffAttribute.Prefix != xmlDiffAttribute1.Prefix && !xmlDiff.IgnorePrefixes &&
                            !xmlDiff.IgnoreNamespaces)
                        {
                            xmlWriter.WriteAttributeString("prefix", xmlDiffAttribute1.Prefix);
                        }

                        if (xmlDiffAttribute.NamespaceURI != xmlDiffAttribute1.NamespaceURI &&
                            !xmlDiff.IgnoreNamespaces)
                        {
                            xmlWriter.WriteAttributeString("ns", xmlDiffAttribute1.NamespaceURI);
                        }

                        xmlWriter.WriteString(xmlDiffAttribute1.Value);
                        break;
                    }
                }
            }

            xmlWriter.WriteEndElement();
        }
    }
}