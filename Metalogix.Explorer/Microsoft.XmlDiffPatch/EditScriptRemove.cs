using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptRemove : EditScript
    {
        internal int _startSourceIndex;

        internal int _endSourceIndex;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.Remove; }
        }

        internal EditScriptRemove(int startSourceIndex, int endSourceIndex, EditScript next) : base(next)
        {
            this._startSourceIndex = startSourceIndex;
            this._endSourceIndex = endSourceIndex;
        }
    }
}