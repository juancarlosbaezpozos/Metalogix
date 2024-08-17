using Metalogix.Core;
using System;
using System.Reflection;

namespace Metalogix.SharePointEdition
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationData.MainAssembly = Assembly.GetExecutingAssembly();
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.ResolveAssembly;
            ApplicationHost.Run();
        }
    }
}