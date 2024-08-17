using System;

namespace Metalogix
{
    public interface ILogMethods
    {
        void Write(string message);

        void Write(string message, Exception ex);

        void WriteFormat(string message, params object[] param);

        void WriteFormat(Exception ex, string message, params object[] param);
    }
}