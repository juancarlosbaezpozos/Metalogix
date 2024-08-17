using System;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class EditScriptOpened : EditScript
    {
        internal EditScriptOpened(EditScript next) : base(next)
        {
        }
    }
}