using Metalogix.Actions;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Metabase.Options
{
    public class MetabaseSettingsOptions : ActionOptions
    {
        public string MetabaseContext { get; set; }

        public string MetabaseType { get; set; }

        public bool UseDefault { get; set; }

        public MetabaseSettingsOptions()
        {
        }
    }
}