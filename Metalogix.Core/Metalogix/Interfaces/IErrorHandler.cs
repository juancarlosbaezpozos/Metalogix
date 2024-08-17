using Metalogix;
using System;

namespace Metalogix.Interfaces
{
    public interface IErrorHandler
    {
        void HandleException(Exception exc);

        void HandleException(string message, Exception exc);

        void HandleException(string message, ErrorIcon icon);

        void HandleException(string caption, string message, Exception exc);

        void HandleException(string caption, string message, ErrorIcon icon);

        void HandleException(string caption, string message, Exception exc, ErrorIcon icon);
    }
}