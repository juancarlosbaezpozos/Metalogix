using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class XmlPatchNodeList : XmlNodeList
    {
        protected XmlPatchNodeList()
        {
        }

        internal abstract void AddNode(XmlNode node);
    }
}