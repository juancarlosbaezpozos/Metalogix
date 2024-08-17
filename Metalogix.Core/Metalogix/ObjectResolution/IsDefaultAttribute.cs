using System;

namespace Metalogix.ObjectResolution
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IsDefaultAttribute : Attribute
    {
        public readonly bool IsDefault;

        public IsDefaultAttribute(bool isDefault)
        {
            this.IsDefault = isDefault;
        }
    }
}