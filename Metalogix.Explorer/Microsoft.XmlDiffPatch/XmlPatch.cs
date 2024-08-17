using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    [ComVisible(true)]
    public class XmlPatch
    {
        private XmlNode _sourceRootNode;

        private bool _ignoreChildOrder;

        public XmlPatch()
        {
        }

        private Patch CreatePatch(XmlNode sourceNode, XmlElement diffgramElement)
        {
            Patch patch = new Patch(sourceNode);
            this._sourceRootNode = sourceNode;
            this.CreatePatchForChildren(sourceNode, diffgramElement, patch);
            return patch;
        }

        private void CreatePatchForChildren(XmlNode sourceParent, XmlElement diffgramParent,
            XmlPatchParentOperation patchParent)
        {
            XmlPatchOperation child = null;
            XmlNode xmlNode = diffgramParent.FirstChild;
            while (xmlNode != null)
            {
                if (xmlNode.NodeType != XmlNodeType.Element)
                {
                    xmlNode = xmlNode.NextSibling;
                }
                else
                {
                    XmlElement xmlElement = (XmlElement)xmlNode;
                    XmlNodeList xmlNodeList = null;
                    string attribute = xmlElement.GetAttribute("match");
                    if (attribute != string.Empty)
                    {
                        xmlNodeList = PathDescriptorParser.SelectNodes(this._sourceRootNode, sourceParent, attribute);
                        if (xmlNodeList.Count == 0)
                        {
                            XmlPatchError.Error("Invalid XDL diffgram. No node matches the path descriptor '{0}'.",
                                attribute);
                        }
                    }

                    XmlPatchOperation xmlPatchOperation = null;
                    string text;
                    if ((text = xmlElement.LocalName) != null)
                    {
                        text = string.IsInterned(text);
                        if (text != "node")
                        {
                            if (text != "add")
                            {
                                if (text != "remove")
                                {
                                    if (text != "change")
                                    {
                                        if (text == "descriptor")
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (xmlNodeList.Count != 1)
                                        {
                                            XmlPatchError.Error(
                                                "Invalid XDL diffgram; more than one node matches the '{0}' path descriptor on the xd:node or xd:change element.",
                                                attribute);
                                        }

                                        XmlNode xmlNode2 = xmlNodeList.Item(0);
                                        if (xmlNode2.NodeType != XmlNodeType.DocumentType)
                                        {
                                            xmlPatchOperation = new PatchChange(xmlNode2,
                                                xmlElement.HasAttribute("name")
                                                    ? xmlElement.GetAttribute("name")
                                                    : null,
                                                xmlElement.HasAttribute("ns") ? xmlElement.GetAttribute("ns") : null,
                                                xmlElement.HasAttribute("prefix")
                                                    ? xmlElement.GetAttribute("prefix")
                                                    : null,
                                                (xmlNode2.NodeType == XmlNodeType.Element) ? null : xmlElement);
                                        }
                                        else
                                        {
                                            xmlPatchOperation = new PatchChange(xmlNode2,
                                                xmlElement.HasAttribute("name")
                                                    ? xmlElement.GetAttribute("name")
                                                    : null,
                                                xmlElement.HasAttribute("systemId")
                                                    ? xmlElement.GetAttribute("systemId")
                                                    : null,
                                                xmlElement.HasAttribute("publicId")
                                                    ? xmlElement.GetAttribute("publicId")
                                                    : null, xmlElement.IsEmpty ? null : xmlElement);
                                        }

                                        if (xmlNode2.NodeType == XmlNodeType.Element)
                                        {
                                            this.CreatePatchForChildren(xmlNode2, xmlElement,
                                                (XmlPatchParentOperation)xmlPatchOperation);
                                        }
                                    }
                                }
                                else
                                {
                                    bool flag = xmlElement.GetAttribute("subtree") != "no";
                                    xmlPatchOperation = new PatchRemove(xmlNodeList, flag);
                                    if (!flag)
                                    {
                                        this.CreatePatchForChildren(xmlNodeList.Item(0), xmlElement,
                                            (XmlPatchParentOperation)xmlPatchOperation);
                                    }
                                }
                            }
                            else if (attribute != string.Empty)
                            {
                                bool flag2 = xmlElement.GetAttribute("subtree") != "no";
                                xmlPatchOperation = new PatchCopy(xmlNodeList, flag2);
                                if (!flag2)
                                {
                                    this.CreatePatchForChildren(sourceParent, xmlElement,
                                        (XmlPatchParentOperation)xmlPatchOperation);
                                }
                            }
                            else
                            {
                                string attribute2 = xmlElement.GetAttribute("type");
                                if (attribute2 != string.Empty)
                                {
                                    XmlNodeType xmlNodeType = (XmlNodeType)int.Parse(attribute2);
                                    bool flag3 = xmlNodeType == XmlNodeType.Element;
                                    if (xmlNodeType != XmlNodeType.DocumentType)
                                    {
                                        xmlPatchOperation = new PatchAddNode(xmlNodeType,
                                            xmlElement.GetAttribute("name"), xmlElement.GetAttribute("ns"),
                                            xmlElement.GetAttribute("prefix"),
                                            flag3 ? string.Empty : xmlElement.InnerText, this._ignoreChildOrder);
                                        if (flag3)
                                        {
                                            this.CreatePatchForChildren(sourceParent, xmlElement,
                                                (XmlPatchParentOperation)xmlPatchOperation);
                                        }
                                    }
                                    else
                                    {
                                        xmlPatchOperation = new PatchAddNode(xmlNodeType,
                                            xmlElement.GetAttribute("name"), xmlElement.GetAttribute("systemId"),
                                            xmlElement.GetAttribute("publicId"), xmlElement.InnerText,
                                            this._ignoreChildOrder);
                                    }
                                }
                                else
                                {
                                    xmlPatchOperation = new PatchAddXmlFragment(xmlElement.ChildNodes);
                                }
                            }
                        }
                        else
                        {
                            if (xmlNodeList.Count != 1)
                            {
                                XmlPatchError.Error(
                                    "Invalid XDL diffgram; more than one node matches the '{0}' path descriptor on the xd:node or xd:change element.",
                                    attribute);
                            }

                            XmlNode xmlNode3 = xmlNodeList.Item(0);
                            if (this._sourceRootNode.NodeType != XmlNodeType.Document ||
                                (xmlNode3.NodeType != XmlNodeType.XmlDeclaration &&
                                 xmlNode3.NodeType != XmlNodeType.DocumentType))
                            {
                                xmlPatchOperation = new PatchSetPosition(xmlNode3);
                                this.CreatePatchForChildren(xmlNode3, xmlElement,
                                    (XmlPatchParentOperation)xmlPatchOperation);
                            }
                        }
                    }

                    if (xmlPatchOperation != null)
                    {
                        patchParent.InsertChildAfter(child, xmlPatchOperation);
                        child = xmlPatchOperation;
                    }

                    xmlNode = xmlNode.NextSibling;
                }
            }
        }

        public void Patch(XmlDocument sourceDoc, XmlReader diffgram)
        {
            if (sourceDoc == null)
            {
                throw new ArgumentNullException("sourceDoc");
            }

            if (diffgram == null)
            {
                throw new ArgumentNullException("diffgram");
            }

            XmlNode xmlNodes = sourceDoc;
            this.Patch(ref xmlNodes, diffgram);
        }

        public void Patch(string sourceFile, Stream outputStream, XmlReader diffgram)
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            if (outputStream == null)
            {
                throw new ArgumentNullException("outputStream");
            }

            if (diffgram == null)
            {
                throw new ArgumentException("diffgram");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(diffgram);
            if (xmlDocument.DocumentElement.GetAttribute("fragments") != "yes")
            {
                this.Patch(new XmlTextReader(sourceFile), outputStream, xmlDocument);
                return;
            }

            NameTable nameTable = new NameTable();
            XmlTextReader xmlTextReader = new XmlTextReader(new FileStream(sourceFile, FileMode.Open, FileAccess.Read),
                XmlNodeType.Element,
                new XmlParserContext(nameTable, new XmlNamespaceManager(nameTable), string.Empty, XmlSpace.Default));
            this.Patch(xmlTextReader, outputStream, xmlDocument);
        }

        public void Patch(XmlReader sourceReader, Stream outputStream, XmlReader diffgram)
        {
            if (sourceReader == null)
            {
                throw new ArgumentNullException("sourceReader");
            }

            if (outputStream == null)
            {
                throw new ArgumentNullException("outputStream");
            }

            if (diffgram == null)
            {
                throw new ArgumentException("diffgram");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(diffgram);
            this.Patch(sourceReader, outputStream, xmlDocument);
        }

        private void Patch(XmlReader sourceReader, Stream outputStream, XmlDocument diffDoc)
        {
            bool attribute = diffDoc.DocumentElement.GetAttribute("fragments") == "yes";
            Encoding encoding = null;
            if (!attribute)
            {
                XmlDocument xmlDocument = new XmlDocument()
                {
                    XmlResolver = null
                };
                xmlDocument.Load(sourceReader);
                XmlNode xmlNodes = xmlDocument;
                this.Patch(ref xmlNodes, diffDoc);
                xmlDocument.Save(outputStream);
                return;
            }

            XmlDocument xmlDocument1 = new XmlDocument();
            XmlDocumentFragment outerXml = xmlDocument1.CreateDocumentFragment();
            while (true)
            {
                XmlNode xmlNodes1 = xmlDocument1.ReadNode(sourceReader);
                XmlNode xmlNodes2 = xmlNodes1;
                if (xmlNodes1 == null)
                {
                    break;
                }

                XmlNodeType nodeType = xmlNodes2.NodeType;
                if (nodeType != XmlNodeType.Whitespace)
                {
                    if (nodeType != XmlNodeType.XmlDeclaration)
                    {
                        outerXml.AppendChild(xmlNodes2);
                    }
                    else
                    {
                        outerXml.InnerXml = xmlNodes2.OuterXml;
                    }
                }

                if (encoding == null)
                {
                    if (!(sourceReader is XmlTextReader))
                    {
                        encoding = (!(sourceReader is XmlValidatingReader)
                            ? Encoding.Unicode
                            : ((XmlValidatingReader)sourceReader).Encoding);
                    }
                    else
                    {
                        encoding = ((XmlTextReader)sourceReader).Encoding;
                    }
                }
            }

            XmlNode xmlNodes3 = outerXml;
            this.Patch(ref xmlNodes3, diffDoc);
            if (outerXml.FirstChild != null && outerXml.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                encoding = Encoding.GetEncoding(((XmlDeclaration)xmlNodes3.FirstChild).Encoding);
            }

            XmlTextWriter xmlTextWriter = new XmlTextWriter(outputStream, encoding);
            outerXml.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
        }

        public void Patch(ref XmlNode sourceNode, XmlReader diffgram)
        {
            if (sourceNode == null)
            {
                throw new ArgumentNullException("sourceNode");
            }

            if (diffgram == null)
            {
                throw new ArgumentNullException("diffgram");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(diffgram);
            this.Patch(ref sourceNode, xmlDocument);
        }

        private void Patch(ref XmlNode sourceNode, XmlDocument diffDoc)
        {
            XmlNode i;
            XmlNode nextSibling = null;
            XmlElement documentElement = diffDoc.DocumentElement;
            if (documentElement.LocalName != "xmldiff" ||
                documentElement.NamespaceURI != "http://schemas.microsoft.com/xmltools/2002/xmldiff")
            {
                XmlPatchError.Error(
                    "Invalid XDL diffgram. Expecting xd:xmldiff as a root element with namespace URI 'http://schemas.microsoft.com/xmltools/2002/xmldiff'.");
            }

            XmlNamedNodeMap attributes = documentElement.Attributes;
            XmlAttribute namedItem = (XmlAttribute)attributes.GetNamedItem("srcDocHash");
            if (namedItem == null)
            {
                XmlPatchError.Error("Invalid XDL diffgram. Missing srcDocHash attribute on the xd:xmldiff element.");
            }

            ulong num = (ulong)0;
            try
            {
                num = ulong.Parse(namedItem.Value);
            }
            catch
            {
                XmlPatchError.Error("Invalid XDL diffgram. The srcDocHash attribute has an invalid value.");
            }

            XmlAttribute xmlAttribute = (XmlAttribute)attributes.GetNamedItem("options");
            if (xmlAttribute == null)
            {
                XmlPatchError.Error("Invalid XDL diffgram. Missing options attribute on the xd:xmldiff element.");
            }

            XmlDiffOptions xmlDiffOption = XmlDiffOptions.None;
            try
            {
                xmlDiffOption = XmlDiff.ParseOptions(xmlAttribute.Value);
            }
            catch
            {
                XmlPatchError.Error("Invalid XDL diffgram. The options attribute has an invalid value.");
            }

            this._ignoreChildOrder = (xmlDiffOption & XmlDiffOptions.IgnoreChildOrder) != XmlDiffOptions.None;
            if (!XmlDiff.VerifySource(sourceNode, num, xmlDiffOption))
            {
                XmlPatchError.Error(
                    "The XDL diffgram is not applicable to this XML document; the srcDocHash value does not match.");
            }

            if (sourceNode.NodeType != XmlNodeType.Document)
            {
                if (sourceNode.NodeType == XmlNodeType.DocumentFragment)
                {
                    Patch patch = this.CreatePatch(sourceNode, documentElement);
                    XmlNode xmlNodes = null;
                    patch.Apply(sourceNode, ref xmlNodes);
                    return;
                }

                XmlDocumentFragment outerXml = sourceNode.OwnerDocument.CreateDocumentFragment();
                XmlNode parentNode = sourceNode.ParentNode;
                XmlNode previousSibling = sourceNode.PreviousSibling;
                if (parentNode != null)
                {
                    parentNode.RemoveChild(sourceNode);
                }

                if (sourceNode.NodeType == XmlNodeType.XmlDeclaration)
                {
                    outerXml.InnerXml = sourceNode.OuterXml;
                }
                else
                {
                    outerXml.AppendChild(sourceNode);
                }

                Patch patch1 = this.CreatePatch(outerXml, documentElement);
                XmlNode xmlNodes1 = null;
                patch1.Apply(outerXml, ref xmlNodes1);
                XmlNodeList childNodes = outerXml.ChildNodes;
                if (childNodes.Count != 1)
                {
                    XmlPatchError.Error("Internal Error. {0} nodes left after patch, expecting 1.",
                        childNodes.Count.ToString());
                }

                sourceNode = childNodes.Item(0);
                outerXml.RemoveAll();
                if (parentNode != null)
                {
                    parentNode.InsertAfter(sourceNode, previousSibling);
                }
            }
            else
            {
                Patch patch2 = this.CreatePatch(sourceNode, documentElement);
                XmlDocument xmlDocument = (XmlDocument)sourceNode;
                XmlElement xmlElement = xmlDocument.CreateElement("tempRoot");
                for (i = xmlDocument.FirstChild; i != null; i = nextSibling)
                {
                    nextSibling = i.NextSibling;
                    if (i.NodeType != XmlNodeType.XmlDeclaration && i.NodeType != XmlNodeType.DocumentType)
                    {
                        xmlDocument.RemoveChild(i);
                        xmlElement.AppendChild(i);
                    }
                }

                xmlDocument.AppendChild(xmlElement);
                XmlNode xmlNodes2 = null;
                patch2.Apply(xmlElement, ref xmlNodes2);
                if (sourceNode.NodeType == XmlNodeType.Document)
                {
                    xmlDocument.RemoveChild(xmlElement);
                    while (true)
                    {
                        XmlNode firstChild = xmlElement.FirstChild;
                        i = firstChild;
                        if (firstChild == null)
                        {
                            break;
                        }

                        xmlElement.RemoveChild(i);
                        xmlDocument.AppendChild(i);
                    }

                    return;
                }
            }
        }
    }
}