using System;
using System.Collections;

namespace Metalogix.Explorer
{
    public interface FieldCollection
    {
        int Count { get; }

        string XML { get; }

        IEnumerator GetEnumerator();
    }
}