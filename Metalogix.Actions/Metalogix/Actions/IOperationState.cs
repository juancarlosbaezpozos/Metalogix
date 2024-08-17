using System;

namespace Metalogix.Actions
{
    public interface IOperationState
    {
        bool IsOperationCancelled { get; }
    }
}