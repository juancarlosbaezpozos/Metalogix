using System;

namespace Metalogix.Deployment
{
    public class CheckForUpdateCompletedEventArgs : EventArgs
    {
        public bool UpdateNeeded;

        public object Tag;

        public CheckForUpdateCompletedEventArgs()
        {
        }
    }
}