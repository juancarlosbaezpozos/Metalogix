using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptMatchOpened : EditScriptOpened
    {
        internal int _startSourceIndex;

        internal int _startTargetIndex;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.OpenedMatch; }
        }

        internal EditScriptMatchOpened(int startSourceIndex, int startTargetIndex, EditScript next) : base(next)
        {
            this._startSourceIndex = startSourceIndex;
            this._startTargetIndex = startTargetIndex;
        }

        internal override EditScript GetClosedScript(int currentSourceIndex, int currentTargetIndex)
        {
            return new EditScriptMatch(this._startSourceIndex, this._startTargetIndex,
                currentSourceIndex - this._startSourceIndex + 1, this._nextEditScript);
        }
    }
}