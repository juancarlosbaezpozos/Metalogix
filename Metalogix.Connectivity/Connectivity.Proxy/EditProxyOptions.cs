using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;

namespace Metalogix.Connectivity.Proxy
{
    public class EditProxyOptions : OptionsBase
    {
        private SecureString _password;

        private CommonSerializableList<string> _addressList;

        public CommonSerializableList<string> BypassAddresses
        {
            get
            {
                if (this._addressList == null)
                {
                    this._addressList = new CommonSerializableList<string>();
                }

                return this._addressList;
            }
            set { this._addressList = value; }
        }

        public bool BypassProxyOnAddress { get; set; }

        public bool BypassProxyOnLocal { get; set; }

        [EncryptedValueParameter(true)]
        public SecureString Password
        {
            get
            {
                if (!this._password.IsNullOrEmpty())
                {
                    return this._password;
                }

                SecureString secureString = new SecureString();
                SecureString secureString1 = secureString;
                this._password = secureString;
                return secureString1;
            }
            set { this._password = value; }
        }

        public string ProxyServerAddress { get; set; }

        public bool SavePassword { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public string UserName { get; set; }

        public EditProxyOptions()
        {
        }

        public WebProxy AsWebProxy()
        {
            WebProxy webProxy = new WebProxy(this.ProxyServerAddress)
            {
                UseDefaultCredentials = this.UseDefaultCredentials,
                BypassProxyOnLocal = this.BypassProxyOnLocal
            };
            WebProxy networkCredential = webProxy;
            if (!networkCredential.UseDefaultCredentials)
            {
                networkCredential.Credentials = new NetworkCredential(this.UserName, this.Password.ToInsecureString());
            }

            if (this.BypassProxyOnAddress)
            {
                networkCredential.BypassList = this.BypassAddresses.ToArray();
            }

            return networkCredential;
        }
    }
}