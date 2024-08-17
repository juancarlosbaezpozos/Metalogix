using System;
using System.IO;

namespace Metalogix.Interfaces
{
    public interface IErrorFormatter
    {
        string FormatException(Exception ex);

        void FormatException(Exception ex, TextWriter output);
    }
}