using System;

namespace Metalogix.Threading
{
    public delegate bool QueuingDelegate<T>(T item, ref int iCurrentCount);
}