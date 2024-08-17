using System;

namespace Metalogix
{
    public abstract class SystemPreconditionAttribute : Attribute
    {
        protected SystemPreconditionAttribute()
        {
        }

        public abstract bool IsPreconditionTrue();
    }
}