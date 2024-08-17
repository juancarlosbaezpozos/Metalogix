using System;
using System.ComponentModel;

namespace Metalogix.Licensing
{
    public class RequiresCompatibilityLevelAttribute : IsLicensedAttribute
    {
        private CompatibilityLevel _level;

        public RequiresCompatibilityLevelAttribute(CompatibilityLevel requiredCompatibilityLevel)
        {
            this._level = requiredCompatibilityLevel;
        }

        public override bool IsLicensed(License license)
        {
            return LicensingUtils.GetLevel(license) >= this._level;
        }
    }
}