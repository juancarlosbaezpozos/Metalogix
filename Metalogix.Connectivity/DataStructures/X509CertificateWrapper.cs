using Metalogix;
using Metalogix.Data;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace Metalogix.DataStructures
{
    public class X509CertificateWrapper : IXmlable
    {
        public const string CERTIFICATE = "Certificate";

        private const string STORE_NAME = "Store";

        private const string SERIAL_NUMBER = "SerialNumber";

        private const string FILE_PATH = "File";

        private const string PASSWORD = "Password";

        private X509Certificate m_certificate;

        private string m_sFriendlyName;

        private bool m_bIsFromStore;

        private string m_sStoreName;

        private string m_sFilePath;

        private SecureString m_sPassword;

        public X509Certificate Certificate
        {
            get { return this.m_certificate; }
        }

        public string ExpirationDateString
        {
            get
            {
                if (this.Certificate == null)
                {
                    return null;
                }

                return this.Certificate.GetExpirationDateString();
            }
        }

        public string FilePath
        {
            get { return this.m_sFilePath; }
        }

        public string FriendlyName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.m_sFriendlyName))
                {
                    return this.m_sFriendlyName;
                }

                this.m_sFriendlyName = X509CertificateWrapper.GetFriendlyName(this.m_certificate);
                return this.m_sFriendlyName;
            }
        }

        public bool IsFromStore
        {
            get { return this.m_bIsFromStore; }
        }

        public string IssuedBy
        {
            get
            {
                if (this.Certificate == null)
                {
                    return null;
                }

                return this.GetNamePropFromCertString(this.Certificate.Issuer);
            }
        }

        public string IssuedTo
        {
            get
            {
                if (this.Certificate == null)
                {
                    return null;
                }

                return this.GetNamePropFromCertString(this.Certificate.Subject);
            }
        }

        public SecureString Password
        {
            get { return this.m_sPassword; }
        }

        public string StoreName
        {
            get { return this.m_sStoreName; }
        }

        public X509CertificateWrapper(X509Certificate certificate) : this(certificate, null)
        {
        }

        public X509CertificateWrapper(X509Certificate certificate, string sStoreName)
        {
            if (certificate == null)
            {
                throw new Exception("Cannot create a X509CertificateWrapper around a null certificate.");
            }

            this.m_certificate = certificate;
            this.m_sStoreName = sStoreName;
            this.m_bIsFromStore = true;
            this.m_sFilePath = null;
            this.m_sPassword = null;
        }

        public X509CertificateWrapper(X509Certificate certificate, string sFilePath, SecureString sPassword)
        {
            if (certificate == null)
            {
                throw new Exception("Cannot create a X509CertificateWrapper around a null certificate.");
            }

            this.m_certificate = certificate;
            this.m_sFilePath = sFilePath;
            this.m_sPassword = sPassword;
            this.m_bIsFromStore = false;
            this.m_sStoreName = null;
        }

        public X509CertificateWrapper(string sXml)
        {
            this.FromXml(XmlUtility.StringToXmlNode(sXml));
        }

        public X509CertificateWrapper(XmlNode node)
        {
            this.FromXml(node);
        }

        public override bool Equals(object obj)
        {
            X509CertificateWrapper x509CertificateWrapper = obj as X509CertificateWrapper;
            if (x509CertificateWrapper == null)
            {
                return false;
            }

            return this.Certificate.GetRawCertDataString()
                .Equals(x509CertificateWrapper.Certificate.GetRawCertDataString());
        }

        private void FromXml(XmlNode node)
        {
            X509Store x509Store;
            this.m_bIsFromStore = false;
            this.m_sStoreName = null;
            this.m_sFilePath = null;
            this.m_sPassword = null;
            if (node.Attributes["SerialNumber"] == null)
            {
                if (node.Attributes["File"] == null)
                {
                    throw new Exception("Failed to load certificate. Invalid XML format.");
                }

                this.m_sFilePath = node.Attributes["File"].Value;
                XmlAttribute itemOf = node.Attributes["Password"];
                if (itemOf != null)
                {
                    try
                    {
                        this.m_sPassword = Cryptography.DecryptText(itemOf.Value);
                    }
                    catch
                    {
                        this.m_sPassword = itemOf.Value.ToSecureString();
                    }
                }

                if (!File.Exists(this.m_sFilePath))
                {
                    throw new Exception(string.Concat("Failed to load certificate from file. File does not exist: ",
                        this.m_sFilePath));
                }

                if (this.m_sPassword.IsNullOrEmpty())
                {
                    this.m_certificate = new X509Certificate(this.m_sFilePath);
                    return;
                }

                this.m_certificate = new X509Certificate(this.m_sFilePath, this.m_sPassword);
            }
            else
            {
                this.m_bIsFromStore = true;
                string value = node.Attributes["SerialNumber"].Value;
                XmlAttribute xmlAttribute = node.Attributes["Store"];
                if (xmlAttribute != null)
                {
                    this.m_sStoreName = xmlAttribute.Value;
                    x509Store = new X509Store(this.m_sStoreName);
                }
                else
                {
                    x509Store = new X509Store();
                    this.m_sStoreName = x509Store.Name;
                }

                x509Store.Open(OpenFlags.ReadOnly);
                try
                {
                    X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        X509Certificate current = enumerator.Current;
                        if (current.GetSerialNumberString() != value)
                        {
                            continue;
                        }

                        this.m_certificate = current;
                        return;
                    }

                    throw new Exception(string.Concat("Failed to located certificate in store. Store Name: ",
                        x509Store.Name, ". Certificate Serial Number: ", value));
                }
                finally
                {
                    x509Store.Close();
                }
            }
        }

        public static string GetFriendlyName(X509Certificate certificate)
        {
            if (certificate == null)
            {
                return string.Empty;
            }

            try
            {
                X509Certificate2 x509Certificate2 = new X509Certificate2(certificate);
                return (string.IsNullOrEmpty(x509Certificate2.FriendlyName) ? "n/a" : x509Certificate2.FriendlyName);
            }
            catch (ArgumentNullException argumentNullException)
            {
            }
            catch (CryptographicException cryptographicException)
            {
            }
            catch (Exception exception)
            {
            }

            return string.Empty;
        }

        public override int GetHashCode()
        {
            return this.Certificate.GetRawCertDataString().GetHashCode();
        }

        private string GetNamePropFromCertString(string value)
        {
            int num = value.IndexOf("CN=", StringComparison.InvariantCultureIgnoreCase);
            if (num < 0)
            {
                return value;
            }

            num += 3;
            int num1 = value.IndexOf(',', num);
            if (num1 < 0)
            {
                return value.Substring(num);
            }

            return value.Substring(num, num1 - num);
        }

        public static implicit operator X509Certificate(X509CertificateWrapper wrapper)
        {
            if (wrapper == null)
            {
                return null;
            }

            return wrapper.Certificate;
        }

        public static implicit operator X509CertificateWrapper(X509Certificate cert)
        {
            return new X509CertificateWrapper(cert);
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            this.ToXML(xmlTextWriter);
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter writer)
        {
            writer.WriteStartElement("Certificate");
            if (!string.IsNullOrEmpty(this.FilePath))
            {
                writer.WriteAttributeString("File", this.FilePath);
                if (!this.Password.IsNullOrEmpty())
                {
                    writer.WriteAttributeString("Password",
                        Cryptography.EncryptText(this.Password, Cryptography.ProtectionScope.CurrentUser, null));
                }
            }
            else
            {
                writer.WriteAttributeString("SerialNumber", this.Certificate.GetSerialNumberString());
                if (!string.IsNullOrEmpty(this.StoreName))
                {
                    writer.WriteAttributeString("Store", this.StoreName);
                }
            }

            writer.WriteEndElement();
        }
    }
}