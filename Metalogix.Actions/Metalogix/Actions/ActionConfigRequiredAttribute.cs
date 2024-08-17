using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionConfigRequiredAttribute : Attribute
    {
        private readonly bool _isRequired;

        public bool IsRequired
        {
            get { return this._isRequired; }
        }

        public ActionConfigRequiredAttribute() : this(true)
        {
        }

        public ActionConfigRequiredAttribute(bool isRequired)
        {
            this._isRequired = isRequired;
        }
    }
}