using System;

namespace Metalogix.Licensing.Logging
{
    public abstract class BaseLog : ILogMethods
    {
        private readonly LogTypes _type;

        protected string TypeString
        {
            get { return this._type.ToString(); }
        }

        protected BaseLog(LogTypes type)
        {
            this._type = type;
        }

        public abstract void Write(string message);

        public abstract void Write(string message, Exception ex);

        public void WriteFormat(string message, params object[] param)
        {
            this.Write(string.Format(message, param));
        }

        public void WriteFormat(Exception ex, string message, params object[] param)
        {
            this.Write(string.Format(message, param), ex);
        }
    }
}