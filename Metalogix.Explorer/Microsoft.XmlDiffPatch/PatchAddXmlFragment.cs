using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class PatchAddXmlFragment : XmlPatchOperation
    {
        private XmlNodeList _nodes;

        internal PatchAddXmlFragment(XmlNodeList nodes)
        {
            this._nodes = nodes;
        }

        internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
        {
            XmlDocument ownerDocument = parent.OwnerDocument;
            IEnumerator enumerator = this._nodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                XmlNode xmlNodes = ownerDocument.ImportNode((XmlNode)enumerator.Current, true);
                parent.InsertAfter(xmlNodes, currentPosition);
                currentPosition = xmlNodes;
            }
        }
    }
}