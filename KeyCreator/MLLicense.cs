using System;
using System.ComponentModel;

namespace KeyCreator
{
    public abstract class MLLicense : License
    {
        public const string DATE_FORMAT = "dd-MMM-yyyy";

        public abstract string Email { get; set; }

        public abstract DateTime ExpiryDate { get; set; }

        public abstract MLLicenseType LicenseType { get; set; }

        public abstract string Name { get; set; }

        public abstract string Organization { get; set; }

        protected MLLicense()
        {
        }

        public virtual string[] GetLicenseCustomInfo()
        {
            return Array.Empty<string>();
        }

        public virtual string[] GetLicenseInfo()
        {
            return Array.Empty<string>();
        }

        public abstract void Validate(string sProductCode);
    }
}
