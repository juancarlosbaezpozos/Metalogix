using Metalogix;
using Metalogix.Client;
using Metalogix.Licensing;
using System;
using System.Reflection;

namespace Metalogix.Deployment
{
    public class AutomaticUpdaterSettings
    {
        private string m_productName = string.Empty;

        private int? m_productId = null;

        private IUICalls m_uicalls;

        public AutomaticUpdaterSettings.AutoUpdateSettingType AutoUpdateSettings
        {
            get { return ClientConfigurationVariables.AutoUpdateSettings; }
            set { ClientConfigurationVariables.AutoUpdateSettings = value; }
        }

        public Version AutoUpdateSkipVersion
        {
            get { return ClientConfigurationVariables.AutoUpdateSkipVersion; }
            set { ClientConfigurationVariables.AutoUpdateSkipVersion = value; }
        }

        public Version InstalledVersion
        {
            get { return ApplicationData.MainAssembly.GetName().Version; }
        }

        public MLLicense License
        {
            get { return this.m_uicalls.GetLicense(); }
        }

        public int ProductId
        {
            get
            {
                if (!this.m_productId.HasValue)
                {
                    return 0;
                }

                return this.m_productId.Value;
            }
            set { this.m_productId = new int?(value); }
        }

        public string ProductName
        {
            get { return this.m_productName; }
            set { this.m_productName = value; }
        }

        public IUICalls UICalls
        {
            get { return this.m_uicalls; }
        }

        public AutomaticUpdaterSettings()
        {
        }

        public AutomaticUpdaterSettings(IUICalls uicalls)
        {
            if (uicalls == null)
            {
                throw new ArgumentNullException("uicalls");
            }

            this.m_uicalls = uicalls;
            this.m_productName = uicalls.GetProductName();
            this.m_productId = new int?(uicalls.GetProductId());
        }

        public enum AutoUpdateSettingType
        {
            AutoUpdate,
            RemindMeLater,
            TurnOffCompletely
        }
    }
}