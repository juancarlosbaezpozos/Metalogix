using System;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Metalogix.SharePoint.Adapters
{
    internal class CertificateValidator
    {
        public CertificateValidator()
        {
        }

        internal static bool CertificationValidator(object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors sslError)
        {
            return true;
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