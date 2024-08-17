using Metalogix;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Metalogix.Interfaces
{
    public static class AsyncExtensionsIErrorHandler
    {
        public static void AsyncHandleException(this IErrorHandler handler, Exception exc)
        {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork +=
                    new DoWorkEventHandler((object o, DoWorkEventArgs s) => handler.HandleException(exc));
                backgroundWorker.RunWorkerAsync();
            }
        }

        public static void AsyncHandleException(this IErrorHandler handler, string message, Exception exc)
        {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork +=
                    new DoWorkEventHandler((object o, DoWorkEventArgs s) => handler.HandleException(message, exc));
                backgroundWorker.RunWorkerAsync();
            }
        }

        public static void AsyncHandleException(this IErrorHandler handler, string message, ErrorIcon icon)
        {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs s) =>
                    handler.HandleException(message, icon));
                backgroundWorker.RunWorkerAsync();
            }
        }

        public static void AsyncHandleException(this IErrorHandler handler, string caption, string message,
            Exception exception)
        {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs s) =>
                    handler.HandleException(caption, message, exception));
                backgroundWorker.RunWorkerAsync();
            }
        }

        public static void AsyncHandleException(this IErrorHandler handler, string caption, string message,
            ErrorIcon icon)
        {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs s) =>
                    handler.HandleException(caption, message, icon));
                backgroundWorker.RunWorkerAsync();
            }
        }

        public static void AsyncHandleException(this IErrorHandler handler, string caption, string message,
            Exception exc, ErrorIcon icon)
        {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs s) =>
                    handler.HandleException(caption, message, exc, icon));
                backgroundWorker.RunWorkerAsync();
            }
        }
    }
}