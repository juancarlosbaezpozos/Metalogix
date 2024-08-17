using System;
using System.IO;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class XmlDiffNode
    {
        internal XmlDiffParentNode _parent;

        internal XmlDiffNode _nextSibling;

        internal int _position;

        internal ulong _hashValue = (long)0;

        internal bool _bExpanded;

        internal int _leftmostLeafIndex;

        internal bool _bKeyRoot;

        internal bool _bSomeDescendantMatches = false;

        internal virtual bool CanMerge
        {
            get { return true; }
        }

        internal virtual XmlDiffNode FirstChildNode
        {
            get { return null; }
        }

        internal virtual bool HasChildNodes
        {
            get { return false; }
        }

        internal ulong HashValue
        {
            get { return this._hashValue; }
        }

        internal virtual string InnerXml
        {
            get
            {
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                this.WriteContentTo(xmlTextWriter);
                xmlTextWriter.Close();
                return stringWriter.ToString();
            }
        }

        internal bool IsKeyRoot
        {
            get { return this._bKeyRoot; }
        }

        internal int Left
        {
            get { return this._leftmostLeafIndex; }
            set { this._leftmostLeafIndex = value; }
        }

        internal virtual int NodesCount
        {
            get { return 1; }
            set { }
        }

        internal abstract XmlDiffNodeType NodeType { get; }

        internal virtual string OuterXml
        {
            get
            {
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                this.WriteTo(xmlTextWriter);
                xmlTextWriter.Close();
                return stringWriter.ToString();
            }
        }

        internal int Position
        {
            get { return this._position; }
        }

        internal XmlDiffNode(int position)
        {
            this._parent = null;
            this._nextSibling = null;
            this._position = position;
            this._bExpanded = false;
        }

        internal abstract void ComputeHashValue(XmlHash xmlHash);

        internal string GetAbsoluteAddress()
        {
            string relativeAddress = this.GetRelativeAddress();
            for (XmlDiffNode i = this._parent; i.NodeType != XmlDiffNodeType.Document; i = i._parent)
            {
                relativeAddress = string.Concat(i.GetRelativeAddress(), "/", relativeAddress);
            }

            return string.Concat("/", relativeAddress);
        }

        internal abstract XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff);

        internal virtual string GetRelativeAddress()
        {
            return this.Position.ToString();
        }

        internal static string GetRelativeAddressOfInterval(XmlDiffNode firstNode, XmlDiffNode lastNode)
        {
            if (firstNode == lastNode)
            {
                return firstNode.GetRelativeAddress();
            }

            if (firstNode._parent._firstChildNode == firstNode && lastNode._nextSibling == null)
            {
                return "*";
            }

            string str = firstNode.Position.ToString();
            int position = lastNode.Position;
            return string.Concat(str, "-", position.ToString());
        }

        internal virtual bool IsSameAs(XmlDiffNode node, XmlDiff xmlDiff)
        {
            return this.GetDiffOperation(node, xmlDiff) == XmlDiffOperation.Match;
        }

        internal abstract void WriteContentTo(XmlWriter w);

        internal abstract void WriteTo(XmlWriter w);
    }
}