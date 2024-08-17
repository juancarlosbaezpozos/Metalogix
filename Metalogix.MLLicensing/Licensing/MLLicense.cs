using System;
using System.ComponentModel;

namespace Metalogix.Licensing
{
    public abstract class MLLicense : License
    {
        public const string DATE_FORMAT = "dd-MMM-yyyy";

        public abstract string Email { get; }

        public abstract DateTime ExpiryDate { get; }

        public abstract MLLicenseType LicenseType { get; }

        public abstract string Name { get; }

        public abstract string Organization { get; }

        protected MLLicense()
        {
        }

        public virtual string[] GetLicenseCustomInfo()
        {
            return new string[0];
        }

        public virtual string[] GetLicenseInfo()
        {
            return new string[0];
        }

        public abstract void Validate(string sProductCode);
    }
}