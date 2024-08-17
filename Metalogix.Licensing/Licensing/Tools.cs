using System;
using System.Reflection;

namespace Metalogix.Licensing
{
    internal static class Tools
    {
        public static Version ClientVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
    }
}