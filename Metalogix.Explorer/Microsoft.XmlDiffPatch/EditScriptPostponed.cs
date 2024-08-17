using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptPostponed : EditScript
    {
        internal DiffgramOperation _diffOperation;

        internal int _startSourceIndex;

        internal int _endSourceIndex;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.EditScriptPostponed; }
        }

        internal EditScriptPostponed(DiffgramOperation diffOperation, int startSourceIndex, int endSourceIndex) :
            base(null)
        {
            this._diffOperation = diffOperation;
            this._startSourceIndex = startSourceIndex;
            this._endSourceIndex = endSourceIndex;
        }
    }
}