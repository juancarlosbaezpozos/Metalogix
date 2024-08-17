using System;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class EditScript
    {
        internal EditScript _nextEditScript;

        internal EditScript Next
        {
            get { return this._nextEditScript; }
        }

        internal abstract EditScriptOperation Operation { get; }

        internal EditScript(EditScript next)
        {
            this._nextEditScript = next;
        }

        internal virtual EditScript GetClosedScript(int currentSourceIndex, int currentTargetIndex)
        {
            return this;
        }
    }
}