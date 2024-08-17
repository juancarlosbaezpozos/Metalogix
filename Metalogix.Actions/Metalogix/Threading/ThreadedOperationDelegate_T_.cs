using System;

namespace Metalogix.Threading
{
    public delegate void ThreadedOperationDelegate<T>(T item, object[] oParams);
}