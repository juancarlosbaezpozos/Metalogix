using Metalogix.Interfaces;
using System;

namespace Metalogix
{
    public abstract class ErrorHandler : IErrorHandler
    {
        private readonly IErrorFormatter _formatter;

        public IErrorFormatter Formatter
        {
            get { return this._formatter; }
        }

        protected ErrorHandler(IErrorFormatter formatter)
        {
            this._formatter = formatter;
        }

        public abstract void HandleException(Exception exc);

        public abstract void HandleException(string message, Exception exc);

        public abstract void HandleException(string message, ErrorIcon icon);

        public abstract void HandleException(string caption, string message, ErrorIcon icon);

        public abstract void HandleException(string caption, string message, Exception exc);

        public abstract void HandleException(string caption, string message, Exception exc, ErrorIcon icon);
    }
}