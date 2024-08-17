using System;

namespace Metalogix
{
    public class EncryptedValueParameterAttribute : Attribute
    {
        private bool m_bEncrypt;

        public bool Encrypt
        {
            get { return this.m_bEncrypt; }
            set { this.m_bEncrypt = value; }
        }

        public EncryptedValueParameterAttribute(bool bEncrypt)
        {
            this.m_bEncrypt = bEncrypt;
        }

        public override string ToString()
        {
            if (!this.Encrypt)
            {
                return "Do Not Encrypt Parameter";
            }

            return "Encrypt Parameter";
        }
    }
}