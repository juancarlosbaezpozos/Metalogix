using Metalogix.LicenseServerOld;
using Metalogix.Licensing.Cryptography;
using System;
using System.Net;
using System.Web.Services.Protocols;

namespace Metalogix.Licensing
{
    public class LicenseServerAdapter
    {
        public LicenseServerAdapter()
        {
        }

        public static void LogLicenseActivation(string sName, string sLicenseGuid)
        {
            try
            {
                using (LicenseService licenseService = new LicenseService())
                {
                    licenseService.Url = "https://licensing.metalogix.com/LicenseService.asmx";
                    licenseService.PreAuthenticate = true;
                    licenseService.Credentials =
                        new NetworkCredential("licensing", Crypter.Decrypt("gp3xWBtcOriKNbK0nMRVmQ=="));
                    licenseService.LogLicenseActivationAsync(sName, sLicenseGuid);
                }
            }
            catch
            {
            }
        }
    }
}