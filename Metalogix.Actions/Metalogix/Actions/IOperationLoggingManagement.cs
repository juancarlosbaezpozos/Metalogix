using System;

namespace Metalogix.Actions
{
    public interface IOperationLoggingManagement
    {
        void ConnectOperationLogging(IOperationLogging operationLogging);

        void DisconnectOperationLogging(IOperationLogging operationLogging);
    }
}