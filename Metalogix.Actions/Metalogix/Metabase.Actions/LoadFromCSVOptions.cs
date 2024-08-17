using Metalogix.Actions;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Metabase.Actions
{
    public class LoadFromCSVOptions : ActionOptions
    {
        public string CsvFile { get; set; }

        public int IdIndex { get; set; }

        public SeparatorTypes Separator { get; set; }

        public LoadFromCSVOptions()
        {
        }
    }
}