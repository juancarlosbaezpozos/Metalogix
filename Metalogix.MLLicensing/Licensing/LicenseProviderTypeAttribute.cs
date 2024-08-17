using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class LicenseProviderTypeAttribute : Attribute
    {
        private readonly Type _type;

        public Type ProviderType
        {
            get { return this._type; }
        }

        public LicenseProviderTypeAttribute(Type type)
        {
            this._type = type;
        }
    }
}