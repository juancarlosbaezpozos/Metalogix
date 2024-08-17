using System;

namespace Metalogix.Metabase.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class RequiresMetabaseConnectionAttribute : Attribute
    {
        private readonly bool _isRequired;

        public bool IsRequired
        {
            get { return this._isRequired; }
        }

        protected RequiresMetabaseConnectionAttribute(bool isRequired)
        {
            this._isRequired = isRequired;
        }
    }
}