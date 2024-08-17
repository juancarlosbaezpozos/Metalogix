using System;

namespace Microsoft.XmlDiffPatch
{
    internal enum EditScriptOperation
    {
        None,
        Match,
        Add,
        Remove,
        ChangeNode,
        EditScriptReference,
        EditScriptPostponed,
        OpenedAdd,
        OpenedRemove,
        OpenedMatch
    }
}