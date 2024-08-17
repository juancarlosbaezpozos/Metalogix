using System;

namespace Microsoft.XmlDiffPatch
{
    internal class EditScriptEmpty : EditScript
    {
        internal override EditScriptOperation Operation
        {
            get { return EditScriptOperation.None; }
        }

        internal EditScriptEmpty() : base(null)
        {
        }
    }
}