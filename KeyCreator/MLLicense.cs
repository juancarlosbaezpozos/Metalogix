using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new string[0];
        }

        public virtual string[] GetLicenseInfo()
        {
            return new string[0];
        }

        public abstract void Validate(string sProductCode);
    }
}
