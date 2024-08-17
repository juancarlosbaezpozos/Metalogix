using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class XmlPatchParentOperation : XmlPatchOperation
    {
        internal XmlPatchOperation _firstChild;

        protected XmlPatchParentOperation()
        {
        }

        protected void ApplyChildren(XmlNode parent)
        {
            XmlNode xmlNodes = null;
            for (XmlPatchOperation i = this._firstChild; i != null; i = i._nextOp)
            {
                i.Apply(parent, ref xmlNodes);
            }
        }

        internal void InsertChildAfter(XmlPatchOperation child, XmlPatchOperation newChild)
        {
            if (child == null)
            {
                newChild._nextOp = this._firstChild;
                this._firstChild = newChild;
                return;
            }

            newChild._nextOp = child._nextOp;
            child._nextOp = newChild;
        }
    }
}