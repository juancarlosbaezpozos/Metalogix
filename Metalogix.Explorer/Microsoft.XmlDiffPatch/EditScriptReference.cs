using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptReference : EditScript
    {
        internal EditScript _editScriptReference;

        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.EditScriptReference; }
        }

        internal EditScriptReference(EditScript editScriptReference, EditScript next) : base(next)
        {
            this._editScriptReference = editScriptReference;
        }
    }
}