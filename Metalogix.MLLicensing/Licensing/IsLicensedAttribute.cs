using Metalogix;
using System;
using System.ComponentModel;

namespace Metalogix.Licensing
{
    public abstract class IsLicensedAttribute : SystemPreconditionAttribute
    {
        protected IsLicensedAttribute()
        {
        }

        public abstract bool IsLicensed(License license);

        public override bool IsPreconditionTrue()
        {
            License license = MLLicenseProvider.Instance.GetLicense(new LicenseContext(), base.GetType(), this, false);
            if (license == null)
            {
                return false;
            }

            return this.IsLicensed(license);
        }
    }
}