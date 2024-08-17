using Metalogix;
using Metalogix.Data;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace Metalogix.Permissions
{
    public class Credentials : IXmlable, IDisposable
    {
        private bool m_bIsDefault = true;

        private Credentials.CredentialStatus m_status;

        private string m_sUserName;

        private SecureString m_sPassword;

        private bool m_bSavePassword;

        private IntPtr m_userHandle = new IntPtr(0);

        private bool bDisposed;

        public static Credentials DefaultCredentials
        {
            get { return new Credentials(); }
        }

        public bool IsDefault
        {
            get { return this.m_bIsDefault; }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                if (this.IsDefault)
                {
                    return CredentialCache.DefaultCredentials;
                }

                string str = null;
                string userName = this.UserName;
                int num = this.UserName.IndexOf("\\", StringComparison.Ordinal);
                if (num > 0)
                {
                    str = this.UserName.Substring(0, num);
                    userName = this.UserName.Substring(num + 1);
                }

                if (str == null)
                {
                    return new NetworkCredential(userName, this.Password.ToInsecureString());
                }

                return new NetworkCredential(userName, this.Password.ToInsecureString(), str);
            }
        }

        public SecureString Password
        {
            get { return this.m_sPassword; }
        }

        public bool SavePassword
        {
            get { return this.m_bSavePassword; }
        }

        public Credentials.CredentialStatus Status
        {
            get { return this.m_status; }
        }

        public string UserName
        {
            get { return this.m_sUserName; }
        }

        public Credentials()
        {
            this.m_bIsDefault = true;
            this.m_sUserName = WindowsIdentity.GetCurrent().Name;
            this.m_sPassword = null;
        }

        public Credentials(string sUserName, SecureString sPassword, bool bSavePassword)
        {
            this.m_bIsDefault = false;
            this.m_sUserName = sUserName;
            this.m_sPassword = sPassword;
            this.m_bSavePassword = bSavePassword;
        }

        public Credentials(XmlNode xmlCredentials)
        {
            if (xmlCredentials.Attributes["UserName"] == null)
            {
                this.m_bIsDefault = true;
                this.m_sUserName = WindowsIdentity.GetCurrent().Name;
                this.m_sPassword = null;
            }
            else
            {
                this.m_bIsDefault = false;
                this.m_sUserName = xmlCredentials.Attributes["UserName"].Value;
                bool.TryParse(
                    (xmlCredentials.Attributes["SavePassword"] != null
                        ? xmlCredentials.Attributes["SavePassword"].Value
                        : "false"), out this.m_bSavePassword);
                if (xmlCredentials.Attributes["Password"] != null)
                {
                    string value = xmlCredentials.Attributes["Password"].Value;
                    this.m_sPassword =
                        (this.m_bSavePassword ? Cryptography.DecryptText(value) : value.ToSecureString());
                    this.m_bSavePassword = true;
                    return;
                }
            }
        }

        public void CheckValidity()
        {
            try
            {
                if (!this.IsDefault)
                {
                    if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.UserName.Trim()))
                    {
                        throw new Exception("Invalid username. Username cannot be empty.");
                    }

                    if (this.UserName.IndexOf("\\", StringComparison.Ordinal) < 0)
                    {
                        throw new Exception("Invalid username. Must be of the form: Domain\\username");
                    }

                    if (this.Password == null || this.Password.Length == 0)
                    {
                        throw new Exception("Password cannot be empty.");
                    }

                    int num = this.UserName.IndexOf("\\", StringComparison.Ordinal);
                    string str = null;
                    string userName = this.UserName;
                    if (num >= 0)
                    {
                        str = this.UserName.Substring(0, num);
                        userName = this.UserName.Substring(num + 1);
                    }

                    if (Metalogix.Permissions.NativeMethods.LogonUser(userName, str, this.Password.ToInsecureString(),
                            3, 0, ref this.m_userHandle) == 0)
                    {
                        throw new Exception("Invalid user name or password.");
                    }

                    this.m_status = Credentials.CredentialStatus.Valid;
                }
                else
                {
                    this.m_status = Credentials.CredentialStatus.Valid;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.m_status = Credentials.CredentialStatus.Invalid;
                throw new Exception(exception.Message);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (!this.bDisposed)
            {
                this.m_userHandle = IntPtr.Zero;
            }
        }

        ~Credentials()
        {
            this.Dispose(false);
        }

        public ImpersonationContext GetImpersonationContext()
        {
            if (this.Status == Credentials.CredentialStatus.Invalid)
            {
                throw new Exception("Invalid credentials. Cannot impersonate user");
            }

            if (this.Status == Credentials.CredentialStatus.NotChecked)
            {
                this.CheckValidity();
            }

            if (this.IsDefault)
            {
                return new ImpersonationContext();
            }

            int num = this.UserName.IndexOf("\\", StringComparison.Ordinal);
            string str = null;
            string userName = this.UserName;
            if (num >= 0)
            {
                str = this.UserName.Substring(0, num);
                userName = this.UserName.Substring(num + 1);
            }

            return new ImpersonationContext(userName, str, this.Password.ToInsecureString());
        }

        public WindowsImpersonationContext Impersonate()
        {
            if (this.Status == Credentials.CredentialStatus.Invalid)
            {
                throw new Exception("Invalid credentials. Cannot impersonate user");
            }

            if (this.Status == Credentials.CredentialStatus.NotChecked)
            {
                this.CheckValidity();
            }

            if (this.IsDefault)
            {
                return null;
            }

            return WindowsIdentity.Impersonate(this.m_userHandle);
        }

        public override string ToString()
        {
            if (this.IsDefault)
            {
                return string.Concat("Default Credentials: ", this.UserName);
            }

            return string.Concat("Credentials: ", this.UserName);
        }

        public string ToXML()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Credentials");
            this.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            if (!this.IsDefault)
            {
                xmlWriter.WriteAttributeString("UserName", this.UserName);
                if (this.m_bSavePassword)
                {
                    xmlWriter.WriteAttributeString("SavePassword", this.m_bSavePassword.ToString());
                    xmlWriter.WriteAttributeString("Password",
                        Cryptography.EncryptText(this.Password, Cryptography.ProtectionScope.CurrentUser, null));
                }
            }
        }

        public enum CredentialStatus
        {
            NotChecked,
            Valid,
            Invalid
        }
    }
}