using System;

namespace Metalogix.Licensing.LicenseServer
{
    internal static class SoapExceptions
    {
        public const string UnauthorizedAccess = "UnauthorizedAccess";

        public const string InvalidCredentials = "InvalidCredentials";

        public const string DatabaseFailure = "DatabaseFailure";

        public const string InvalidData = "InvalidData";

        public const string LicenseNotFound = "LicenseNotFound";

        public const string PropertyNotFound = "PropertyNotFound";

        public const string InvalidLicense = "InvalidLicense";
    }
}