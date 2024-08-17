using Metalogix.Licensing.Common;
using System;
using System.ComponentModel;

namespace Metalogix.Licensing
{
    public class RequiresFullEditionAttribute : IsLicensedAttribute
    {
        private bool _requiresFullEdition;

        public RequiresFullEditionAttribute(bool requiresFullEdition = true)
        {
            this._requiresFullEdition = requiresFullEdition;
        }

        public override bool IsLicensed(License license)
        {
            MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
            if (mLLicenseCommon == null)
            {
                return true;
            }

            if (!this._requiresFullEdition)
            {
                return true;
            }

            return !mLLicenseCommon.IsContentMatrixExpress;
        }
    }
}