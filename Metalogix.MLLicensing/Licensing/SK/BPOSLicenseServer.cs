using Metalogix;
using Metalogix.Licensing.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.Licensing.SK
{
    internal class BPOSLicenseServer
    {
        private static BPOSLicenseServer _this;

        private BPOSLicenseService _licenseService;

        internal static BPOSLicenseServer Instance
        {
            get
            {
                if (BPOSLicenseServer._this == null)
                {
                    BPOSLicenseServer._this = new BPOSLicenseServer();
                }

                return BPOSLicenseServer._this;
            }
        }

        private BPOSLicenseServer()
        {
            this._licenseService = new BPOSLicenseService(SKLP.Get.InitData.LicenseServerUrl)
            {
                AuthenticationHeaderValue = new AuthenticationHeader()
            };
        }

        public BPOSLicenseInfo CheckLicense(string licenseKey)
        {
            licenseKey = licenseKey.Replace("-", "");
            this._licenseService.AuthenticationHeaderValue.AuthenticationString = Crypter.Encrypt(licenseKey);
            BPOSLicenseInfo bPOSLicenseInfo = null;
            try
            {
                using (BPOSLicenseServer.ServerCertificateChecker serverCertificateChecker =
                       BPOSLicenseServer.ServerCertificateChecker.Create())
                {
                    bPOSLicenseInfo = this._licenseService.BPOSCheckLicense();
                }
            }
            catch (Exception exception)
            {
                BPOSLicenseServer.ThrowSpecificException(exception);
            }

            return bPOSLicenseInfo;
        }

        private static string GetIPAddress()
        {
            List<string> strs = new List<string>();
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            for (int i = 0; i < (int)addressList.Length; i++)
            {
                strs.Add(addressList[i].ToString());
            }

            strs.Sort();
            string str = "";
            foreach (string str1 in strs)
            {
                str = string.Concat(str, str1, ",");
            }

            return str.TrimEnd(new char[] { ',' });
        }

        public BPOSPartnerInfo GetPartnerInfo(string licenseKey)
        {
            BPOSPartnerInfo bPOSPartnerInfo;
            BPOSPartnerInfo bPOSPartnerInfo1;
            licenseKey = licenseKey.Replace("-", "");
            this._licenseService.AuthenticationHeaderValue.AuthenticationString = Crypter.Encrypt(licenseKey);
            try
            {
                PartnerInfo partner = null;
                using (BPOSLicenseServer.ServerCertificateChecker serverCertificateChecker =
                       BPOSLicenseServer.ServerCertificateChecker.Create())
                {
                    partner = this._licenseService.GetPartner();
                }

                if (partner != null)
                {
                    bPOSPartnerInfo1 = new BPOSPartnerInfo(partner.PartnerId, partner.CompanyName,
                        partner.TrialQuoteUrl, partner.PaidQuoteUrl);
                }
                else
                {
                    bPOSPartnerInfo1 = null;
                }

                bPOSPartnerInfo = bPOSPartnerInfo1;
            }
            catch (Exception exception)
            {
                BPOSLicenseServer.ThrowSpecificException(exception);
                return null;
            }

            return bPOSPartnerInfo;
        }

        public LicenseStatus ReadLicenseInfo(string key, LicenseProxy proxy)
        {
            WebResponse response;
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            string licenseServerUrl = SKLP.Get.InitData.LicenseServerUrl;
            int num = licenseServerUrl.IndexOf("licenseservice.asmx", StringComparison.OrdinalIgnoreCase);
            if (num < 0)
            {
                throw new Exception("Incorrect license server url.");
            }

            string str = string.Concat(licenseServerUrl.Substring(0, num), "license.aspx");
            object[] objArray = new object[] { str, key, 0, BPOSLicenseServer.GetIPAddress() };
            string str1 = string.Format("{0}?KEYVALUE={1}&MBX={2}&IP={3}", objArray);
            Logger.Debug.Write(string.Concat("BPOSLicenseServer >> ReadLicenseKeyOnLine: start ", str1));
            WebRequest webRequest = HttpWebRequestPathed.GetWebRequest(str1);
            if (!proxy.Enabled)
            {
                WebRequest.DefaultWebProxy = null;
                Logger.Debug.Write("BPOSLicenseServer >> CheckLicenseKeyOnLine>> no proxy server.");
            }
            else
            {
                ILogMethods debug = Logger.Debug;
                string[] server = new string[]
                {
                    "BPOSLicenseServer >> CheckLicenseKeyOnLine >> Trying proxy server = ", proxy.Server, "; Port = ",
                    proxy.Port, "; User = ", proxy.User
                };
                debug.Write(string.Concat(server));
                WebProxy webProxy = new WebProxy(string.Concat("http://", proxy.Server, ":", proxy.Port), true)
                {
                    Credentials = new NetworkCredential(proxy.User, proxy.Pass)
                };
                WebRequest.DefaultWebProxy = webProxy;
                ILogMethods logMethod = Logger.Debug;
                object[] address = new object[]
                {
                    "BPOSLicenseServer >> CheckLicenseKeyOnLine >> Using proxy >> Server = ", webProxy.Address,
                    "; User = ", proxy.User
                };
                logMethod.Write(string.Concat(address));
            }

            webRequest.Credentials = CredentialCache.DefaultCredentials;
            string end = null;
            using (BPOSLicenseServer.ServerCertificateChecker serverCertificateChecker =
                   BPOSLicenseServer.ServerCertificateChecker.Create())
            {
                response = webRequest.GetResponse();
            }

            using (TextReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                end = streamReader.ReadToEnd();
            }

            if (end == null)
            {
                Logger.Debug.Write(
                    "BPOSLicenseServer >> CheckLicenseKeyOnLine >> Error: Can not download data from license server ");
                throw new Exception("Can not download data from license server.");
            }

            Logger.Debug.Write(string.Concat("BPOSLicenseServer >> CheckLicenseKeyOnLine >> ", end));
            DateTime now = DateTime.Now;
            end = string.Concat(end, "|", now.ToString("yyyyMMdd"));
            return new LicenseStatus(end);
        }

        public IEnumerable<Association> SetAssociation(IEnumerable<Association> associations, string licenseKey)
        {
            this._licenseService.AuthenticationHeaderValue.AuthenticationString = Crypter.Encrypt(licenseKey);
            List<Association> associations1 = new List<Association>();
            List<string> strs = new List<string>();
            foreach (Association association in associations)
            {
                associations1.Add(association);
                strs.Add(association.PartnerId);
            }

            if (associations1.Count == 0)
            {
                throw new Exception("The associations array is empty.");
            }

            Association[] associationArray = null;
            try
            {
                using (BPOSLicenseServer.ServerCertificateChecker serverCertificateChecker =
                       BPOSLicenseServer.ServerCertificateChecker.Create())
                {
                    associationArray = this._licenseService.SetAssociations(associations1.ToArray(), strs.ToArray());
                }
            }
            catch (Exception exception)
            {
                BPOSLicenseServer.ThrowSpecificException(exception);
            }

            return associationArray;
        }

        private static void ThrowSpecificException(Exception ex)
        {
            Logger.Error.Write("An error occured on the license server.", ex);
            if (ex is SoapException)
            {
                SoapException soapException = (SoapException)ex;
                if (soapException.Code != null)
                {
                    if (string.Compare(soapException.Code.Name, "UnauthorizedAccess",
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        throw new InvalidLicenseKeyException();
                    }

                    if (string.Compare(soapException.Code.Name, "InvalidLicense", StringComparison.OrdinalIgnoreCase) ==
                        0)
                    {
                        throw new InvalidLicenseKeyException();
                    }

                    if (string.Compare(soapException.Code.Name, "LicenseDisabled",
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        throw new DisabledLicenseKeyException();
                    }

                    if (string.Compare(soapException.Code.Name, "SeatsMismatch", StringComparison.OrdinalIgnoreCase) ==
                        0)
                    {
                        throw new InvalidLicenseKeyException();
                    }

                    if (string.Compare(soapException.Code.Name, "AlreadyAssociated",
                            StringComparison.OrdinalIgnoreCase) == 0 ||
                        string.Compare(soapException.Code.Name, "TooManyChanges", StringComparison.OrdinalIgnoreCase) ==
                        0 || soapException.Message.IndexOf("Violation of UNIQUE KEY constraint 'UQ_ASSOCIATION",
                            StringComparison.Ordinal) > -1)
                    {
                        throw new AlreadyAssociatedException();
                    }

                    if (string.Compare(soapException.Code.Name, "QuotaExceeded", StringComparison.OrdinalIgnoreCase) ==
                        0)
                    {
                        throw new QuotaExceededException();
                    }
                }
            }

            throw new Exception("Unexpected error occured on the license server.", ex);
        }

        private class ServerCertificateChecker : IDisposable
        {
            public ServerCertificateChecker()
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (RemoteCertificateValidationCallback)Delegate.Combine(
                        ServicePointManager.ServerCertificateValidationCallback,
                        new RemoteCertificateValidationCallback(BPOSLicenseServer.ServerCertificateChecker
                            .CertificateValidationCallBack));
            }

            private static bool CertificateValidationCallBack(object sender, X509Certificate certificate,
                X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            }

            public static BPOSLicenseServer.ServerCertificateChecker Create()
            {
                return new BPOSLicenseServer.ServerCertificateChecker();
            }

            public void Dispose()
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (RemoteCertificateValidationCallback)Delegate.Remove(
                        ServicePointManager.ServerCertificateValidationCallback,
                        new RemoteCertificateValidationCallback(BPOSLicenseServer.ServerCertificateChecker
                            .CertificateValidationCallBack));
                GC.SuppressFinalize(this);
            }
        }
    }
}