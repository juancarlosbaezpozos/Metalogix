using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptRemoveOpened : EditScriptOpened
    {
        internal int _startSourceIndex;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.OpenedRemove; }
        }

        internal EditScriptRemoveOpened(int startSourceIndex, EditScript next) : base(next)
        {
            this._startSourceIndex = startSourceIndex;
        }

        internal override EditScript GetClosedScript(int currentSourceIndex, int currentTargetIndex)
        {
            return new EditScriptRemove(this._startSourceIndex, currentSourceIndex, this._nextEditScript);
        }
    }
}