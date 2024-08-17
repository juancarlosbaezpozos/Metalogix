using Metalogix;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Core.ConfigVariables
{
    public class DatabaseTableDataResolverOptions : OptionsBase
    {
        public string ConnectionString { get; set; }

        public string Scope { get; set; }

        public DatabaseTableDataResolverOptions()
        {
        }
    }
}