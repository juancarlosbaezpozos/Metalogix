using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class AssemblyFrameworkAttribute : Attribute
    {
        private readonly bool _isFramework;

        public bool IsFramework
        {
            get { return this._isFramework; }
        }

        public AssemblyFrameworkAttribute() : this(true)
        {
        }

        public AssemblyFrameworkAttribute(bool isFramework)
        {
            this._isFramework = isFramework;
        }
    }
}