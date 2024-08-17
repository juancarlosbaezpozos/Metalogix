using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramAddSubtrees : DiffgramOperation
    {
        internal XmlDiffNode _firstTargetNode;

        internal XmlDiffNode _lastTargetNode;

        private bool _bSorted;

        private bool _bNeedNamespaces;

        internal DiffgramAddSubtrees(XmlDiffNode subtreeRoot, ulong operationID, bool bSorted) : base(operationID)
        {
            this._firstTargetNode = subtreeRoot;
            this._lastTargetNode = subtreeRoot;
            this._bSorted = bSorted;
            this._bNeedNamespaces = subtreeRoot.NodeType == XmlDiffNodeType.Element;
        }

        internal bool SetNewFirstNode(XmlDiffNode targetNode)
        {
            if (this._operationID != (long)0 || targetNode._nextSibling != this._firstTargetNode ||
                targetNode.NodeType == XmlDiffNodeType.Text && this._firstTargetNode.NodeType == XmlDiffNodeType.Text ||
                !targetNode.CanMerge || !this._firstTargetNode.CanMerge)
            {
                return false;
            }

            this._firstTargetNode = targetNode;
            if (targetNode.NodeType == XmlDiffNodeType.Element)
            {
                this._bNeedNamespaces = true;
            }

            return true;
        }

        internal bool SetNewLastNode(XmlDiffNode targetNode)
        {
            if (this._operationID != (long)0 || this._lastTargetNode._nextSibling != targetNode ||
                targetNode.NodeType == XmlDiffNodeType.Text && this._lastTargetNode.NodeType == XmlDiffNodeType.Text ||
                !targetNode.CanMerge || !this._lastTargetNode.CanMerge)
            {
                return false;
            }

            this._lastTargetNode = targetNode;
            if (targetNode.NodeType == XmlDiffNodeType.Element)
            {
                this._bNeedNamespaces = true;
            }

            return true;
        }

        private void Sort()
        {
            XmlDiffNode xmlDiffNode = null;
            XmlDiff.SortNodesByPosition(ref this._firstTargetNode, ref this._lastTargetNode, ref xmlDiffNode);
            this._bSorted = true;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            if (!this._bSorted)
            {
                this.Sort();
            }

            xmlWriter.WriteStartElement("xd", "add", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            if (this._operationID != (long)0)
            {
                xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
            }

            if (this._bNeedNamespaces)
            {
                Hashtable hashtables = new Hashtable();
                for (XmlDiffParentNode i = this._firstTargetNode._parent; i != null; i = i._parent)
                {
                    if (i._bDefinesNamespaces)
                    {
                        for (XmlDiffAttributeOrNamespace j = ((XmlDiffElement)i)._attributes;
                             j != null && j.NodeType == XmlDiffNodeType.Namespace;
                             j = (XmlDiffAttributeOrNamespace)j._nextSibling)
                        {
                            if (hashtables[j.Prefix] == null)
                            {
                                if (j.Prefix != string.Empty)
                                {
                                    xmlWriter.WriteAttributeString("xmlns", j.Prefix, "http://www.w3.org/2000/xmlns/",
                                        j.NamespaceURI);
                                }
                                else
                                {
                                    xmlWriter.WriteAttributeString("xmlns", "http://www.w3.org/2000/xmlns/",
                                        j.NamespaceURI);
                                }

                                hashtables[j.Prefix] = j.Prefix;
                            }
                        }
                    }
                }
            }

            XmlDiffNode xmlDiffNode = this._firstTargetNode;
            while (true)
            {
                xmlDiffNode.WriteTo(xmlWriter);
                if (xmlDiffNode == this._lastTargetNode)
                {
                    break;
                }

                xmlDiffNode = xmlDiffNode._nextSibling;
            }

            xmlWriter.WriteEndElement();
        }
    }
}