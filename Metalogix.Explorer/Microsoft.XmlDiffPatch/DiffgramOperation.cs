using System;
using System.Text;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class DiffgramOperation
    {
        internal DiffgramOperation _nextSiblingOp;

        protected ulong _operationID;

        internal DiffgramOperation(ulong operationID)
        {
            this._nextSiblingOp = null;
            this._operationID = operationID;
        }

        internal static void GetAddressOfAttributeInterval(AttributeInterval interval, XmlWriter xmlWriter)
        {
            if (interval._next == null)
            {
                if (interval._firstAttr == interval._lastAttr)
                {
                    xmlWriter.WriteAttributeString("match", interval._firstAttr.GetRelativeAddress());
                    return;
                }

                if (interval._firstAttr._parent._firstChildNode == interval._firstAttr &&
                    (interval._lastAttr._nextSibling == null ||
                     interval._lastAttr._nextSibling.NodeType != XmlDiffNodeType.Attribute))
                {
                    xmlWriter.WriteAttributeString("match", "@*");
                    return;
                }
            }

            string empty = string.Empty;
            while (true)
            {
                XmlDiffAttribute xmlDiffAttribute = interval._firstAttr;
                while (true)
                {
                    empty = string.Concat(empty, xmlDiffAttribute.GetRelativeAddress());
                    if (xmlDiffAttribute == interval._lastAttr)
                    {
                        break;
                    }

                    empty = string.Concat(empty, "|");
                    xmlDiffAttribute = (XmlDiffAttribute)xmlDiffAttribute._nextSibling;
                }

                interval = interval._next;
                if (interval == null)
                {
                    break;
                }

                empty = string.Concat(empty, "|");
            }

            xmlWriter.WriteAttributeString("match", empty);
        }

        internal static string GetRelativeAddressOfNodeset(XmlDiffNode firstNode, XmlDiffNode lastNode)
        {
            int position = -1;
            bool flag = false;
            StringBuilder stringBuilder = new StringBuilder();
            XmlDiffNode xmlDiffNode = firstNode;
            while (true)
            {
                if (xmlDiffNode.Position != position + 1)
                {
                    if (flag)
                    {
                        stringBuilder.Append(position);
                        flag = false;
                        stringBuilder.Append('|');
                    }

                    stringBuilder.Append(xmlDiffNode.Position);
                    if (xmlDiffNode != lastNode)
                    {
                        if (xmlDiffNode._nextSibling.Position != xmlDiffNode.Position + 1)
                        {
                            stringBuilder.Append('|');
                        }
                        else
                        {
                            stringBuilder.Append("-");
                            flag = true;
                        }
                    }
                }

                if (xmlDiffNode == lastNode)
                {
                    break;
                }

                position = xmlDiffNode.Position;
                xmlDiffNode = xmlDiffNode._nextSibling;
            }

            if (flag)
            {
                stringBuilder.Append(lastNode.Position);
            }

            return stringBuilder.ToString();
        }

        internal static void WriteAbsoluteMatchAttribute(XmlDiffNode node, XmlWriter xmlWriter)
        {
            XmlDiffAttribute xmlDiffAttribute = node as XmlDiffAttribute;
            if (xmlDiffAttribute != null && xmlDiffAttribute.NamespaceURI != string.Empty)
            {
                DiffgramOperation.WriteNamespaceDefinition(xmlDiffAttribute, xmlWriter);
            }

            xmlWriter.WriteAttributeString("match", node.GetAbsoluteAddress());
        }

        private static void WriteNamespaceDefinition(XmlDiffAttribute attr, XmlWriter xmlWriter)
        {
            if (attr.Prefix == string.Empty)
            {
                xmlWriter.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/",
                    attr.NamespaceURI);
                return;
            }

            xmlWriter.WriteAttributeString("xmlns", attr.Prefix, "http://www.w3.org/2000/xmlns/", attr.NamespaceURI);
        }

        internal abstract void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff);
    }
}