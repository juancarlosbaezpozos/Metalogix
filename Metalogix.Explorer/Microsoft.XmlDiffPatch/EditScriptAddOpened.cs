using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptAddOpened : EditScriptOpened
    {
        internal int _startTargetIndex;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.OpenedAdd; }
        }

        internal EditScriptAddOpened(int startTargetIndex, EditScript next) : base(next)
        {
            this._startTargetIndex = startTargetIndex;
        }

        internal override EditScript GetClosedScript(int currentSourceIndex, int currentTargetIndex)
        {
            return new EditScriptAdd(this._startTargetIndex, currentTargetIndex, this._nextEditScript);
        }
    }
}