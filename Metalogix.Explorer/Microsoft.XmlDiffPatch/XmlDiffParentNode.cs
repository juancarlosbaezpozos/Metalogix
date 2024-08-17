using System;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class XmlDiffParentNode : XmlDiffNode
    {
        internal XmlDiffNode _firstChildNode;

        private int _nodesCount;

        private int _childNodesCount = -1;

        internal bool _elementChildrenOnly;

        internal bool _bDefinesNamespaces;

        internal int ChildNodesCount
        {
            get
            {
                if (this._childNodesCount == -1)
                {
                    int num = 0;
                    for (XmlDiffNode i = this._firstChildNode; i != null; i = i._nextSibling)
                    {
                        num++;
                    }

                    this._childNodesCount = num;
                }

                return this._childNodesCount;
            }
        }

        internal override XmlDiffNode FirstChildNode
        {
            get { return this._firstChildNode; }
        }

        internal override bool HasChildNodes
        {
            get { return this._firstChildNode != null; }
        }

        internal override int NodesCount
        {
            get { return this._nodesCount; }
            set { this._nodesCount = value; }
        }

        internal XmlDiffParentNode(int position) : base(position)
        {
            this._firstChildNode = null;
            this._nodesCount = 1;
            this._elementChildrenOnly = true;
            this._bDefinesNamespaces = false;
            this._hashValue = (ulong)0;
        }

        internal virtual void InsertChildNodeAfter(XmlDiffNode childNode, XmlDiffNode newChildNode)
        {
            newChildNode._parent = this;
            if (childNode != null)
            {
                newChildNode._nextSibling = childNode._nextSibling;
                childNode._nextSibling = newChildNode;
            }
            else
            {
                newChildNode._nextSibling = this._firstChildNode;
                this._firstChildNode = newChildNode;
            }

            this._nodesCount += newChildNode.NodesCount;
            if (newChildNode.NodeType != XmlDiffNodeType.Element && !(newChildNode is XmlDiffAttributeOrNamespace))
            {
                this._elementChildrenOnly = false;
            }
        }
    }
}