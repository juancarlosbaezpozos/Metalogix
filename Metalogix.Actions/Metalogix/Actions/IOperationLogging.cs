using System;

namespace Metalogix.Actions
{
    public interface IOperationLogging
    {
        event ActionEventHandler OperationFinished;

        event ActionEventHandler OperationStarted;

        event ActionEventHandler OperationUpdated;
    }
}