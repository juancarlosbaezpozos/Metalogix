using System;

namespace Metalogix.Deployment
{
    public class AnotherProcessInfo
    {
        private string _userName;

        private string _processName;

        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string ProcessName
        {
            get { return this._processName; }
            set { this._processName = value; }
        }

        public string UserName
        {
            get { return this._userName; }
            set { this._userName = value; }
        }

        public AnotherProcessInfo()
        {
        }
    }
}