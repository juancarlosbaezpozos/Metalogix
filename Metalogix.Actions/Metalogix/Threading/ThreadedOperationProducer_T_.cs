using System;

namespace Metalogix.Threading
{
    public delegate T ThreadedOperationProducer<T>(object[] oParams);
}