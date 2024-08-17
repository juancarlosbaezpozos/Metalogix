using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptAdd : EditScript
    {
        internal int _startTargetIndex;

        internal int _endTargetIndex;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.Add; }
        }

        internal EditScriptAdd(int startTargetIndex, int endTargetIndex, EditScript next) : base(next)
        {
            this._startTargetIndex = startTargetIndex;
            this._endTargetIndex = endTargetIndex;
        }
    }
}