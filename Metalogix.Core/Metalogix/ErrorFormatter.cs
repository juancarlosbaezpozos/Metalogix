using Metalogix.Interfaces;
using Metalogix.Utilities;
using System;
using System.IO;

namespace Metalogix
{
    public class ErrorFormatter : IErrorFormatter
    {
        public ErrorFormatter()
        {
        }

        public string FormatException(Exception exception)
        {
            return ExceptionUtils.GetExceptionMessageAndDetail(exception).Detail;
        }

        public void FormatException(Exception exception, TextWriter output)
        {
            output.WriteLine(ExceptionUtils.GetExceptionMessageAndDetail(exception).Detail);
        }
    }
}