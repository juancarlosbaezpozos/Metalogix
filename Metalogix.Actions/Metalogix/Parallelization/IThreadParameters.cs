using System;

namespace Metalogix.Parallelization
{
    public interface IThreadParameters
    {
        int ThreadIndex { get; set; }
    }
}