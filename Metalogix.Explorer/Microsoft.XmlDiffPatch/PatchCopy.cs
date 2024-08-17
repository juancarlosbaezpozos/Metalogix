using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class PatchCopy : XmlPatchParentOperation
    {
        private XmlNodeList _matchNodes;

        private bool _bSubtree;

        internal PatchCopy(XmlNodeList matchNodes, bool bSubtree)
        {
            this._matchNodes = matchNodes;
            this._bSubtree = bSubtree;
        }

        internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
        {
            XmlNode xmlNodes;
            IEnumerator enumerator = this._matchNodes.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                XmlNode current = (XmlNode)enumerator.Current;
                if (!this._bSubtree)
                {
                    xmlNodes = current.CloneNode(false);
                    base.ApplyChildren(xmlNodes);
                }
                else
                {
                    xmlNodes = current.Clone();
                }

                parent.InsertAfter(xmlNodes, currentPosition);
                currentPosition = xmlNodes;
            }
        }
    }
}