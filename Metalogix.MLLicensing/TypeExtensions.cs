using Metalogix.Licensing;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix
{
    public static class TypeExtensions
    {
        public static bool CheckLicensing(this Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(IsLicensedAttribute), true);
            if ((int)customAttributes.Length == 0)
            {
                return true;
            }

            License license = MLLicenseProvider.Instance.GetLicense(new LicenseContext(), typeof(Type), type, false);
            if (license == null)
            {
                return false;
            }

            object[] objArray = customAttributes;
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                if (!((IsLicensedAttribute)objArray[i]).IsLicensed(license))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckPreconditions(this Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(SystemPreconditionAttribute), true);
            for (int i = 0; i < (int)customAttributes.Length; i++)
            {
                if (!((SystemPreconditionAttribute)customAttributes[i]).IsPreconditionTrue())
                {
                    return false;
                }
            }

            return true;
        }
    }
}