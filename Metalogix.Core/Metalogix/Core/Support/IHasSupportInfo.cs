using System;
using System.IO;

namespace Metalogix.Core.Support
{
    public interface IHasSupportInfo
    {
        void WriteSupportInfo(TextWriter output);
    }
}