using System;

namespace Microsoft.XmlDiffPatch
{
    internal class OperationDescrMove : OperationDescriptor
    {
        internal override string Type
        {
            get { return "move"; }
        }

        internal OperationDescrMove(ulong opid) : base(opid)
        {
        }
    }
}