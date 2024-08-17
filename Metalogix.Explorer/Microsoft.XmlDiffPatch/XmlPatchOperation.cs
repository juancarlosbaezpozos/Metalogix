using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class XmlPatchOperation
    {
        internal XmlPatchOperation _nextOp;

        protected XmlPatchOperation()
        {
        }

        internal abstract void Apply(XmlNode parent, ref XmlNode currentPosition);
    }
}