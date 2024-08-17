using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class PatchRemove : XmlPatchParentOperation
    {
        private XmlNodeList _sourceNodes;

        private bool _bSubtree;

        internal PatchRemove(XmlNodeList sourceNodes, bool bSubtree)
        {
            this._sourceNodes = sourceNodes;
            this._bSubtree = bSubtree;
        }

        internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
        {
            if (!this._bSubtree)
            {
                XmlNode xmlNodes = this._sourceNodes.Item(0);
                base.ApplyChildren(xmlNodes);
                currentPosition = xmlNodes.PreviousSibling;
            }

            IEnumerator enumerator = this._sourceNodes.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                XmlNode current = (XmlNode)enumerator.Current;
                if (current.NodeType != XmlNodeType.Attribute)
                {
                    if (!this._bSubtree)
                    {
                        while (current.FirstChild != null)
                        {
                            XmlNode firstChild = current.FirstChild;
                            current.RemoveChild(firstChild);
                            parent.InsertAfter(firstChild, currentPosition);
                            currentPosition = firstChild;
                        }
                    }

                    current.ParentNode.RemoveChild(current);
                }
                else
                {
                    ((XmlElement)parent).RemoveAttributeNode((XmlAttribute)current);
                }
            }
        }
    }
}