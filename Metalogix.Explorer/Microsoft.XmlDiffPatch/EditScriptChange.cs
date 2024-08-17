using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptChange : EditScript
    {
        internal int _sourceIndex;

        internal int _targetIndex;

        internal XmlDiffOperation _changeOp;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.ChangeNode; }
        }

        internal EditScriptChange(int sourceIndex, int targetIndex, XmlDiffOperation changeOp, EditScript next) :
            base(next)
        {
            this._sourceIndex = sourceIndex;
            this._targetIndex = targetIndex;
            this._changeOp = changeOp;
        }
    }
}