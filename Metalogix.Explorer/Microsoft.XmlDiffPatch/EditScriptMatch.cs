using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptMatch : EditScript
    {
        internal int _firstSourceIndex;

        internal int _firstTargetIndex;

        internal int _length;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.Match; }
        }

        internal EditScriptMatch(int startSourceIndex, int startTargetIndex, int length, EditScript next) : base(next)
        {
            this._firstSourceIndex = startSourceIndex;
            this._firstTargetIndex = startTargetIndex;
            this._length = length;
        }
    }
}