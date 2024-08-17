using System;

namespace Metalogix.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class LicenseProviderAttribute : Attribute
    {
        private readonly LicenseProviderType _type;

        public LicenseProviderType Type
        {
            get { return this._type; }
        }

        public LicenseProviderAttribute(LicenseProviderType type)
        {
            this._type = type;
        }
    }
}