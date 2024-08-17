using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Metalogix.Licensing.LicenseServer
{
    public class Server : IDisposable
    {
        public const string PUBLIC_KEY =
            "<RSAKeyValue><Modulus>zBQqYBd1a2ck3QAtTRBls4xAaDuhU+QLrJ6wLYlg3KN+tQhLKrTtG6cItOWxQwcRmnFoV7bCuZ5ICMgc+ProYPUW9xEe5SFKsWX7wOai9aAQwpjM3Un1dSCfmLjAADmB32DHnOxZhXB4c6U8XJPK+SANWQtTn3De4aZQ702uxQ8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private readonly static string[] _trustedCertificateIssuers;

        private readonly string _userName;

        private readonly string _password;

        private readonly LicenseService _service;

        private readonly RSACryptoServiceProvider _csp;

        public static System.Version DefaultClientVersion
        {
            get { return Tools.ClientVersion; }
        }

        public string Url
        {
            get { return this._service.Url; }
        }

        static Server()
        {
            Server._trustedCertificateIssuers = new string[] { "\"GoDaddy.com", "\"VeriSign" };
        }

        public Server(string url, string userName, string password, ServerProxySettings proxy)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            this._csp = new RSACryptoServiceProvider();
            this._csp.FromXmlString(
                "<RSAKeyValue><Modulus>zBQqYBd1a2ck3QAtTRBls4xAaDuhU+QLrJ6wLYlg3KN+tQhLKrTtG6cItOWxQwcRmnFoV7bCuZ5ICMgc+ProYPUW9xEe5SFKsWX7wOai9aAQwpjM3Un1dSCfmLjAADmB32DHnOxZhXB4c6U8XJPK+SANWQtTn3De4aZQ702uxQ8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            if (string.Compare((new Uri(url)).Scheme, "https", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new Exception("Only HTTPS connections are allowed.");
            }

            ILogMethods debug = Logger.Debug;
            object[] clientVersion = new object[] { Tools.ClientVersion, userName, url, proxy };
            debug.WriteFormat("License client: version={0}; user={1}; url={2}; proxy=", clientVersion);
            this._userName = userName;
            this._password = password;
            this._service = new LicenseService()
            {
                Url = url,
                Proxy = proxy.GetWebProxy()
            };
            this.SetClientVersion(Tools.ClientVersion);
            ServicePointManager.ServerCertificateValidationCallback =
                (RemoteCertificateValidationCallback)Delegate.Combine(
                    ServicePointManager.ServerCertificateValidationCallback,
                    new RemoteCertificateValidationCallback(this.CertificationValidator));
        }

        public int AddAdvancedOptionForLicense(AdvancedOptionRequest advancedOption)
        {
            Logger.Debug.Write("Server >> AdvancedOptionRequest: Entered");
            if (advancedOption == null)
            {
                throw new ArgumentNullException("advancedOption");
            }

            Logger.Debug.WriteFormat("Server >> AdvancedOptionRequest: info={0}", new object[] { advancedOption });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { advancedOption };
            return (int)this.ExecuteMethod(name, objArray);
        }

        public string AddProductRelease(AddProductReleaseRequest info)
        {
            Logger.Debug.Write("Server >> AddProductRelease: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> AddProductRelease: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public bool CertificationValidator(object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors sslError)
        {
            Logger.Debug.Write("Server >> CertificationValidator: Entered.");
            StackFrame[] frames = (new StackTrace()).GetFrames();
            bool flag = false;
            int length = (int)frames.Length - 1;
            while (length > 0)
            {
                MethodBase method = frames[length].GetMethod();
                if (method.DeclaringType != typeof(Server))
                {
                    if (method.DeclaringType == typeof(HttpWebRequest))
                    {
                        break;
                    }

                    length--;
                }
                else
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                return true;
            }

            if (sslError != SslPolicyErrors.None)
            {
                Logger.Debug.Write(string.Concat("Server >> CertificationValidator: Unknown SSL error got.", sslError));
                return false;
            }

            Server.CertificateData certificateDatum = new Server.CertificateData(cert.Subject);
            if (string.Compare((new Uri(this.Url)).Host, certificateDatum.CommonName,
                    StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                Logger.Warning.Write(string.Concat(
                    "Server >> CertificationValidator: The certificate was not registered for the queried URL.",
                    this.Url));
                return false;
            }

            Logger.Warning.Write("Server >> CertificationValidator: The certificate URL is OK.");
            Logger.Warning.Write("Server >> CertificationValidator: Checking the certificate's issuer.");
            X509ChainElementEnumerator enumerator = chain.ChainElements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Server.CertificateData certificateDatum1 =
                    new Server.CertificateData(enumerator.Current.Certificate.Subject);
                string[] strArrays = Server._trustedCertificateIssuers;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    if (string.Compare(strArrays[i], certificateDatum1.Organization,
                            StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        Logger.Warning.Write("Server >> CertificationValidator: The certificate's issuer is OK.");
                        return true;
                    }
                }
            }

            Logger.Debug.Write("Server >> CertificationValidator: The servers certificate is not valid.");
            return false;
        }

        private void ClearLogin()
        {
            this._service.TokenAuthenticationHeaderValue = null;
            Logger.Debug.Write("Server >> ClearLogin: Session token cleared.");
        }

        public string ConvertOldKey(string oldLicenseKey)
        {
            Logger.Debug.Write("Server >> ConvertOldKey: Entered");
            if (string.IsNullOrEmpty(oldLicenseKey))
            {
                throw new ArgumentNullException("oldLicenseKey");
            }

            Logger.Debug.WriteFormat("Server >> ConvertOldKey: info={0}", new object[] { oldLicenseKey });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { oldLicenseKey };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public LicenseInfo CreateLicense(CreateLicenseRequest info)
        {
            Logger.Debug.Write("Server >> CreateLicense: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> CreateLicense: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (LicenseInfo)this.ExecuteMethod(name, objArray);
        }

        public LicenseInfo CreateTrialLicense(CreateTrialLicenseRequest info)
        {
            Logger.Debug.Write("Server >> CreateTrialLicense: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> CreateTrialLicense: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (LicenseInfo)this.ExecuteMethod(name, objArray);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (RemoteCertificateValidationCallback)Delegate.Remove(
                        ServicePointManager.ServerCertificateValidationCallback,
                        new RemoteCertificateValidationCallback(this.CertificationValidator));
                this._service.Dispose();
                this._csp.Clear();
                ((IDisposable)this._csp).Dispose();
            }
        }

        public void Echo()
        {
            Logger.Debug.Write("Server >> Echo: Entered");
            try
            {
                this._service.Echo();
                Logger.Debug.Write("Server >> Echo: Finished successfully.");
            }
            catch (Exception exception)
            {
                Logger.Warning.Write("Server >> Echo: Failed.", exception);
                throw;
            }
        }

        private object ExecuteMethod(string sMethodName, object[] parameters)
        {
            object obj;
            Logger.Debug.Write("Server >> ExecuteMethod: Entered");
            if (sMethodName == null)
            {
                throw new ArgumentNullException("sMethodName");
            }

            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { sMethodName, null };
            objArray[1] = (parameters != null ? (int)parameters.Length : 0);
            debug.WriteFormat("Server >> ExecuteMethod: MethodName={0}; ParamsCount={1}", objArray);
            MethodInfo method = this._service.GetType().GetMethod(sMethodName);
            if (method == null)
            {
                Logger.Warning.WriteFormat("Server >> ExecuteMethod: Calling method '{0}' was not found.",
                    new object[] { sMethodName });
                throw new Exception(string.Format("Cannot find method '{0}' in web service wrapper.", sMethodName));
            }

            Logger.Debug.Write("Server >> ExecuteMethod: Method found on the interface");
            int num = 0;
            do
            {
                try
                {
                    try
                    {
                        this.ValidateLogin();
                        Logger.Debug.WriteFormat("Server >> ExecuteMethod: Invoking method '{0}'",
                            new object[] { sMethodName });
                        object obj1 = method.Invoke(this._service, parameters);
                        ILogMethods logMethod = Logger.Debug;
                        object[] objArray1 = new object[] { sMethodName, null };
                        objArray1[1] = (obj1 != null ? obj1.GetType().ToString() : "NULL");
                        logMethod.WriteFormat(
                            "Server >> ExecuteMethod: Method '{0}' successfully invoked, data '{1}' got", objArray1);
                        SignedResponse signedResponse = obj1 as SignedResponse;
                        if (this._csp != null && signedResponse != null)
                        {
                            Logger.Debug.Write("Server >> ExecuteMethod: Result is SignedData, validating signature");
                            if (!Metalogix.Licensing.LicenseServer.Signature.Validate(signedResponse, this._csp))
                            {
                                Logger.Warning.Write("Server >> ExecuteMethod: Invalid signature");
                                throw new InvalidSignatureException();
                            }

                            Logger.Warning.Write("Server >> ExecuteMethod: Signature OK");
                        }

                        obj = obj1;
                        return obj;
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        Logger.Warning.Write("Server >> ExecuteMethod: Failed to execute method");
                        Exception executedMethodException = Server.GetExecutedMethodException(exception);
                        Logger.Warning.Write("Server >> ExecuteMethod: Execution exception got",
                            executedMethodException);
                        if (!ServerTools.IsSoapExceptionOfType(executedMethodException, "UnauthorizedAccess"))
                        {
                            if (!ServerTools.IsSoapExceptionOfType(executedMethodException, "InvalidCredentials"))
                            {
                                if (!ServerTools.IsSoapExceptionOfType(executedMethodException, "LicenseNotFound"))
                                {
                                    if (!ServerTools.IsSoapExceptionOfType(executedMethodException, "DatabaseFailure"))
                                    {
                                        if (ServerTools.IsSoapExceptionOfType(executedMethodException, "InvalidData") ||
                                            ServerTools.IsSoapExceptionOfType(executedMethodException,
                                                "PropertyNotFound"))
                                        {
                                            Logger.Debug.Write("Server >> ExecuteMethod: Invalid data exception got.");
                                            string soapMessageDetail =
                                                ServerTools.GetSoapMessageDetail(executedMethodException);
                                            throw new ServerException(string.Format(
                                                "Invalid data was sent to the license server.{0}",
                                                (string.IsNullOrEmpty(soapMessageDetail)
                                                    ? string.Empty
                                                    : string.Concat(" ", soapMessageDetail))));
                                        }

                                        if (!ServerTools.IsSoapExceptionOfType(executedMethodException,
                                                "InvalidLicense"))
                                        {
                                            throw executedMethodException;
                                        }

                                        Logger.Debug.Write("Server >> ExecuteMethod: Invalid license exception got.");
                                        throw new InvalidLicenseException(
                                            ServerTools.GetSoapMessageDetail(executedMethodException));
                                    }

                                    Logger.Debug.Write("Server >> ExecuteMethod: Database exception got.");
                                    throw new ServerException("Database error occured on the license server.");
                                }

                                Logger.Debug.Write("Server >> ExecuteMethod: License not found exception got.");
                                throw new InvalidLicenseException("License key not found in the license database.");
                            }

                            Logger.Debug.Write("Server >> ExecuteMethod: Invalid credentials exception got.");
                            throw new InvalidCredentialsException();
                        }

                        Logger.Debug.Write("Server >> ExecuteMethod: The login session was timed out or not logged in");
                        if (num >= 1)
                        {
                            Logger.Debug.Write("Server >> ExecuteMethod: Re-login failed throwing exception.");
                            throw new UnauthorizedException();
                        }

                        Logger.Debug.Write("Server >> ExecuteMethod: Trying to re-login.");
                        this.ClearLogin();
                    }

                    continue;
                }
                finally
                {
                    num++;
                }

                return obj;
            } while (num <= 1);

            throw new Exception("Total network call attempts exceeded without an error or a return value.");
        }

        ~Server()
        {
            this.Dispose(false);
        }

        public AdvancedOptionsResponse GetAdvancedOptionForLicense(string licenseKey)
        {
            Logger.Debug.Write("Server >> GetAdvancedOptionForLicense: Entered");
            if (string.IsNullOrEmpty(licenseKey))
            {
                throw new ArgumentNullException("licenseKey");
            }

            Logger.Debug.WriteFormat("Server >> GetAdvancedOptionForLicense: info={0}", new object[] { licenseKey });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { licenseKey };
            return (AdvancedOptionsResponse)this.ExecuteMethod(name, objArray);
        }

        private static Exception GetExecutedMethodException(Exception ex)
        {
            if (!(ex is TargetInvocationException) || ex.InnerException == null)
            {
                return ex;
            }

            return ex.InnerException;
        }

        public LatestProductReleaseInfo GetLatestProductRelease(GetProductReleaseRequest request)
        {
            Logger.Debug.Write("Server >> GetLatestProductRelease : Entered");
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Logger.Debug.WriteFormat("Server >> GetLatestProductRelease(: info={0}", new object[] { request });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { request };
            return (LatestProductReleaseInfo)this.ExecuteMethod(name, objArray);
        }

        public LicenseDataResponse GetLicenseData(string licenseKey)
        {
            Logger.Debug.Write("Server >> GetLicenseData: Entered");
            if (string.IsNullOrEmpty(licenseKey))
            {
                throw new ArgumentNullException("licenseKey");
            }

            Logger.Debug.WriteFormat("Server >> GetLicenseData: Key={0};", new object[] { licenseKey });
            return this.GetLicenseData(licenseKey, LicenseDataDetails.LicenseInfo);
        }

        public LicenseDataResponse GetLicenseData(string licenseKey, LicenseDataDetails details)
        {
            Logger.Debug.Write("Server >> GetLicenseData: Entered");
            if (string.IsNullOrEmpty(licenseKey))
            {
                throw new ArgumentNullException("licenseKey");
            }

            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { licenseKey, details };
            debug.WriteFormat("Server >> GetLicenseData: Key={0}; Details={1}", objArray);
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray1 = new object[] { licenseKey, details };
            return (LicenseDataResponse)this.ExecuteMethod(name, objArray1);
        }

        public LicenseInfo ModifyLicense(UpdateLicenseRequest info)
        {
            Logger.Debug.Write("Server >> ModifyLicense: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> ModifyLicense: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (LicenseInfo)this.ExecuteMethod(name, objArray);
        }

        public string ModifyProductRelease(ModifyProductReleaseRequest info)
        {
            Logger.Debug.Write("Server >> ModifyProductRelease: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> ModifyProductRelease: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public void SetClientVersion(System.Version version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            LicenseService licenseService = this._service;
            ClientVersionHeader clientVersionHeader = new ClientVersionHeader()
            {
                Version = version.ToString()
            };
            licenseService.ClientVersionHeaderValue = clientVersionHeader;
        }

        public int UpdateAdvancedOptionForLicense(AdvancedOptionRequest advancedOption)
        {
            Logger.Debug.Write("Server >> AdvancedOptionRequest: Entered");
            if (advancedOption == null)
            {
                throw new ArgumentNullException("advancedOption");
            }

            Logger.Debug.WriteFormat("Server >> AdvancedOptionRequest: info={0}", new object[] { advancedOption });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { advancedOption };
            return (int)this.ExecuteMethod(name, objArray);
        }

        public LicenseInfoResponse UpdateLicense(LicenseInfoRequest info)
        {
            Logger.Debug.Write("Server >> UpdateLicense: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            ILogMethods debug = Logger.Debug;
            object[] key = new object[]
            {
                info.Key, info.UsedData, info.Admin, info.Server, info.ProductCode, info.ProductVersion, info.Message
            };
            debug.WriteFormat(
                "Server >> UpdateLicense: Key={0}; UsedData={1}; Admin={2}; Server={3}; Product={4}, Version={5}, Message={6} ",
                key);
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (LicenseInfoResponse)this.ExecuteMethod(name, objArray);
        }

        public string UpdateServerProductSystemInfo(ServerProductSystemInfo info)
        {
            Logger.Debug.Write("Server >> UpdateServerProductSystemInfo: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> UpdateServerProductSystemInfo: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateServerSystemInfo(ServerSystemInfo info)
        {
            Logger.Debug.Write("Server >> UpdateServerSystemInfo: Entered");
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug.WriteFormat("Server >> UpdateServerSystemInfo: info={0}", new object[] { info });
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { info };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private void ValidateLogin()
        {
            if (this._service.TokenAuthenticationHeaderValue != null)
            {
                Logger.Debug.Write("Server >> ValidateLogin: Already logged in.");
                return;
            }

            Logger.Debug.WriteFormat("Server >> ValidateLogin: Not logged in, trying to login with UserName={0}",
                new object[] { this._userName });
            string str = this._service.Login(this._userName, this._password);
            Logger.Debug.WriteFormat("Server >> ValidateLogin: Login OK, SessionToken={0}", new object[] { str });
            TokenAuthenticationHeader tokenAuthenticationHeader = new TokenAuthenticationHeader()
            {
                Token = str
            };
            this._service.TokenAuthenticationHeaderValue = tokenAuthenticationHeader;
        }

        private class CertificateData
        {
            public string CommonName { get; private set; }

            public string Country { get; private set; }

            public string Locality { get; private set; }

            public string Organization { get; private set; }

            public string OrganizationUnit { get; private set; }

            public string State { get; private set; }

            public CertificateData(string locString)
            {
                string[] strArrays = locString.Split(new char[] { ',' });
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i].Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        string[] strArrays1 = str.Split(new char[] { '=' });
                        string upper = strArrays1[0].ToUpper();
                        string str1 = upper;
                        if (upper != null)
                        {
                            if (str1 == "CN")
                            {
                                this.CommonName = strArrays1[1].Trim();
                            }
                            else if (str1 == "OU")
                            {
                                this.OrganizationUnit = strArrays1[1].Trim();
                            }
                            else if (str1 == "O")
                            {
                                this.Organization = strArrays1[1].Trim();
                            }
                            else if (str1 == "L")
                            {
                                this.Locality = strArrays1[1].Trim();
                            }
                            else if (str1 == "S")
                            {
                                this.State = strArrays1[1].Trim();
                            }
                            else if (str1 == "C")
                            {
                                this.Country = strArrays1[1].Trim();
                            }
                        }
                    }
                }
            }

            public override string ToString()
            {
                object[] commonName = new object[]
                {
                    this.CommonName, this.OrganizationUnit, this.Organization, this.Locality, this.State, this.Country
                };
                return string.Format(
                    "CommonName: {0}, OrganizationUnit: {1}, Organization: {2}, Locality: {3}, State: {4}, Country: {5}",
                    commonName);
            }
        }
    }
}