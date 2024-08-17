using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Database
{
    public class Mapping
    {
        public string SourceId { get; set; }

        public string SourceUrl { get; set; }

        public string TargetId { get; set; }

        public string TargetType { get; set; }

        public string TargetUrl { get; set; }

        public Mapping()
        {
        }
    }
}