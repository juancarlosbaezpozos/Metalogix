using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public abstract class AuthenticationInitializer
    {
        private Metalogix.Permissions.Credentials m_credentials;

        private X509CertificateWrapperCollection m_certificates;

        public bool AutomaticAuthenticationEnabled
        {
            get { return AuthenticationInitializer.GetInitializerAllowsAutomaticLogin(this.GetType()); }
        }

        public static Type[] AvailableInitializerTypes
        {
            get { return TypeCatalog.AvailableInitializerTypes; }
        }

        public X509CertificateWrapperCollection Certificates
        {
            get { return this.m_certificates; }
            set { this.m_certificates = value; }
        }

        public EntryStyles CredentialEntryStyle
        {
            get
            {
                object[] customAttributes = this.GetType()
                    .GetCustomAttributes(typeof(Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle),
                        true);
                if (customAttributes == null || (int)customAttributes.Length == 0)
                {
                    return EntryStyles.Either;
                }

                object[] objArray = customAttributes;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle credentialEntryStyle =
                        objArray[i] as Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle;
                    if (credentialEntryStyle != null)
                    {
                        return credentialEntryStyle.Style;
                    }
                }

                return EntryStyles.Either;
            }
        }

        public Metalogix.Permissions.Credentials Credentials
        {
            get { return this.m_credentials; }
            set { this.m_credentials = value; }
        }

        public int MenuIndex
        {
            get { return MenuOrderAttribute.GetMenuOrder(this.GetType()); }
        }

        public string MenuText
        {
            get
            {
                object[] customAttributes = this.GetType().GetCustomAttributes(typeof(MenuTextAttribute), true);
                if (customAttributes == null || (int)customAttributes.Length == 0)
                {
                    return this.ToString();
                }

                object[] objArray = customAttributes;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    MenuTextAttribute menuTextAttribute = objArray[i] as MenuTextAttribute;
                    if (menuTextAttribute != null)
                    {
                        return menuTextAttribute.Text;
                    }
                }

                return this.ToString();
            }
        }

        protected AuthenticationInitializer()
        {
        }

        public bool CompatibleWithAdapter(SharePointAdapter adapter)
        {
            return this.CompatibleWithAdapter(adapter.AdapterShortName);
        }

        public bool CompatibleWithAdapter(Type type)
        {
            string adapterShortName = AdapterShortNameAttribute.GetAdapterShortName(type);
            if (string.IsNullOrEmpty(adapterShortName))
            {
                return false;
            }

            return this.CompatibleWithAdapter(adapterShortName);
        }

        public bool CompatibleWithAdapter(string sAdapterShortName)
        {
            object[] customAttributes = this.GetType().GetCustomAttributes(typeof(DisallowSharePointAdapter), true);
            List<string> strs = new List<string>((customAttributes == null ? 0 : (int)customAttributes.Length));
            if (customAttributes != null)
            {
                object[] objArray = customAttributes;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    DisallowSharePointAdapter disallowSharePointAdapter = objArray[i] as DisallowSharePointAdapter;
                    if (disallowSharePointAdapter != null)
                    {
                        strs.Add(disallowSharePointAdapter.Adapter);
                    }
                }
            }

            return !strs.Contains(sAdapterShortName);
        }

        public static AuthenticationInitializer Create(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (!type.IsSubclassOf(typeof(AuthenticationInitializer)))
            {
                throw new Exception(string.Concat("The given type is not an authentication initializer type: ",
                    type.FullName));
            }

            return Activator.CreateInstance(type) as AuthenticationInitializer;
        }

        public static AuthenticationInitializer Create(string typeFullName)
        {
            Type[] availableInitializerTypes = AuthenticationInitializer.AvailableInitializerTypes;
            for (int i = 0; i < (int)availableInitializerTypes.Length; i++)
            {
                Type type = availableInitializerTypes[i];
                if (type.FullName == typeFullName)
                {
                    return AuthenticationInitializer.Create(type);
                }
            }

            throw new Exception(string.Format("The authentication initializer type '{0}' could not be located.",
                typeFullName));
        }

        public static bool GetInitializerAllowsAutomaticLogin(Type initializerType)
        {
            object[] customAttributes = initializerType.GetCustomAttributes(
                typeof(Metalogix.SharePoint.Adapters.Authentication.AutomaticAuthenticationEnabled), true);
            if (customAttributes == null || (int)customAttributes.Length == 0)
            {
                return true;
            }

            object[] objArray = customAttributes;
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                Metalogix.SharePoint.Adapters.Authentication.AutomaticAuthenticationEnabled
                    automaticAuthenticationEnabled =
                        objArray[i] as Metalogix.SharePoint.Adapters.Authentication.AutomaticAuthenticationEnabled;
                if (automaticAuthenticationEnabled != null)
                {
                    return automaticAuthenticationEnabled.Value;
                }
            }

            return true;
        }

        private void GlobalInitActions(SharePointAdapter adapter)
        {
            adapter.Credentials = this.Credentials;
            adapter.IncludedCertificates = this.Certificates;
            adapter.AuthenticationInitializer = this;
        }

        public void InitializeAuthenticationSettings(SharePointAdapter adapter)
        {
            this.GlobalInitActions(adapter);
            this.SpecializedInitActions(adapter);
        }

        protected abstract void SpecializedInitActions(SharePointAdapter adapter);

        public virtual bool TestAuthenticationSetup(SharePointAdapter adapter)
        {
            return false;
        }

        public override string ToString()
        {
            return this.MenuText;
        }
    }
}