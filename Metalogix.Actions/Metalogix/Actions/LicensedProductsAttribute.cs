using Metalogix.Licensing;
using Metalogix.Licensing.Common;
using System;
using System.ComponentModel;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class LicensedProductsAttribute : IsLicensedAttribute
    {
        private readonly ProductFlags _requiredProductCode;

        public ProductFlags RequiredProductCode
        {
            get { return this._requiredProductCode; }
        }

        public LicensedProductsAttribute(ProductFlags requiredProductCode = 0)
        {
            this._requiredProductCode = requiredProductCode;
        }

        public override bool IsLicensed(License license)
        {
            MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
            if (mLLicenseCommon != null)
            {
                ProductFlags specificProductFlags = mLLicenseCommon.SpecificProductFlags;
                return (this._requiredProductCode & specificProductFlags) > ProductFlags.Unknown;
            }

            if ((this._requiredProductCode & ProductFlags.CMWebComponents) == ProductFlags.CMWebComponents)
            {
                return true;
            }

            return this._requiredProductCode == ProductFlags.UnifiedContentMatrixKey;
        }
    }
}