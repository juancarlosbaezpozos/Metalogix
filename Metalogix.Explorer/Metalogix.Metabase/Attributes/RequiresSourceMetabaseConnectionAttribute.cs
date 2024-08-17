using System;

namespace Metalogix.Metabase.Attributes
{
    public sealed class RequiresSourceMetabaseConnectionAttribute : RequiresMetabaseConnectionAttribute
    {
        public RequiresSourceMetabaseConnectionAttribute(bool isRequired) : base(isRequired)
        {
        }
    }
}