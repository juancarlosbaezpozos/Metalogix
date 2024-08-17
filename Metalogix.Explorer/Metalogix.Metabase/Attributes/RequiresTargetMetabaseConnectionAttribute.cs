using System;

namespace Metalogix.Metabase.Attributes
{
    public sealed class RequiresTargetMetabaseConnectionAttribute : RequiresMetabaseConnectionAttribute
    {
        public RequiresTargetMetabaseConnectionAttribute(bool isRequired) : base(isRequired)
        {
        }
    }
}