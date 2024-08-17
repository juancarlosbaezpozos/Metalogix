// Metalogix.SharePoint.UI.WinForms, Version=8.3.0.3, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4
// Metalogix.SharePoint.UI.WinForms.Administration.TCSharePointConnectionConfig
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.UI.WinForms.Administration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Utilities;
using Metalogix.Utilities;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.ConnectionOptions.png")]
    [ControlName("Connection Options")]
    [UsesGroupBox(true)]
    public class TCSharePointConnectionConfig : TabbableControl
    {
        private class DataBoundedComboBox<T>
        {
            private readonly Dictionary<string, T> m_index;

            private readonly ComboBoxEdit m_comboBox;

            private readonly T[] m_dataSource;

            private readonly Func<T, string> m_getKey;

            public ComboBoxEdit ComboBox => m_comboBox;

            public T[] DataSource => m_dataSource;

            public Func<T, string> GetKey => m_getKey;

            public DataBoundedComboBox(ComboBoxEdit comboBox, T[] dataSource, Func<T, string> getKey)
            {
                m_index = new Dictionary<string, T>();
                m_comboBox = comboBox;
                m_dataSource = dataSource;
                m_getKey = getKey;
            }

            public void FilterByKey(IEnumerable<string> keys)
            {
                try
                {
                    ComboBox.Properties.BeginUpdate();
                    string text = ComboBox.SelectedItem as string;
                    ComboBox.Properties.Items.Clear();
                    if (keys != null)
                    {
                        foreach (string key in keys)
                        {
                            if (m_index.ContainsKey(key))
                            {
                                ComboBox.Properties.Items.Add(key);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(text) && ComboBox.Properties.Items.Count > 0)
                    {
                        int num = ComboBox.FindString(text);
                        ComboBox.SelectedIndex = ((num != -1) ? num : 0);
                    }
                }
                finally
                {
                    ComboBox.Properties.EndUpdate();
                }
            }

            public void FilterByValue(IEnumerable<T> values)
            {
                try
                {
                    ComboBox.Properties.BeginUpdate();
                    string text = ComboBox.SelectedItem as string;
                    ComboBox.Properties.Items.Clear();
                    if (values != null)
                    {
                        foreach (T value in values)
                        {
                            string text2 = GetKey(value);
                            if (m_index.ContainsKey(text2))
                            {
                                ComboBox.Properties.Items.Add(text2);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(text) && ComboBox.Properties.Items.Count > 0)
                    {
                        int num = ComboBox.FindString(text);
                        ComboBox.SelectedIndex = ((num != -1) ? num : 0);
                    }
                }
                finally
                {
                    ComboBox.Properties.EndUpdate();
                }
            }

            public T GetDefaultValue()
            {
                if (ComboBox.Properties.Items.Count <= 0)
                {
                    return default(T);
                }
                return GetValue(ComboBox.Properties.Items[0].ToString());
            }

            public T GetValue(string key)
            {
                if (!m_index.ContainsKey(key))
                {
                    return default(T);
                }
                return m_index[key];
            }

            public void Initialize()
            {
                Array.ForEach(m_dataSource, delegate (T item)
                {
                    string text = GetKey(item);
                    m_index.Add(text, item);
                    m_comboBox.Properties.Items.Add(text);
                });
                m_comboBox.SelectedIndex = 0;
            }

            public void SelectByKey(string key)
            {
                if (ComboBox.Properties.Items.Contains(key))
                {
                    ComboBox.SelectedItem = key;
                }
            }

            public void SelectByValue(T value)
            {
                SelectByKey(GetKey(value));
            }
        }

        protected class SharePointAdapterTypeWrapper
        {
            private Type _wrappedType;

            private string _displayName;

            private string _shortName;

            private int _menuIndex;

            private AdapterSupportedFlags _flags;

            public string DisplayName => _displayName;

            public int MenuIndex => _menuIndex;

            public string ShortName => _shortName;

            public Type WrappedType => _wrappedType;

            public SharePointAdapterTypeWrapper(Type type)
            {
                _wrappedType = type;
                _displayName = AdapterDisplayNameAttribute.GetAdapterDisplayName(_wrappedType);
                _shortName = AdapterShortNameAttribute.GetAdapterShortName(_wrappedType);
                _flags = AdapterSupportsAttribute.GetAdapterSupportedFlags(_wrappedType);
                _menuIndex = MenuOrderAttribute.GetMenuOrder(_wrappedType);
            }

            public static int CompareForMenuSort(SharePointAdapterTypeWrapper leftType, SharePointAdapterTypeWrapper rightType)
            {
                int num = leftType.MenuIndex.CompareTo(rightType.MenuIndex);
                if (num != 0)
                {
                    return num;
                }
                return leftType.DisplayName.CompareTo(rightType.DisplayName);
            }

            public static implicit operator Type(SharePointAdapterTypeWrapper wrapper)
            {
                return wrapper?.WrappedType;
            }

            public static implicit operator SharePointAdapterTypeWrapper(Type type)
            {
                if (type == null)
                {
                    return null;
                }
                return new SharePointAdapterTypeWrapper(type);
            }

            public bool SupportsState(ConnectionScope scope, CompatibilityLevel licenseLevel)
            {
                bool flag = true;
                switch (licenseLevel)
                {
                    case CompatibilityLevel.Invalid:
                        flag = false;
                        break;
                    case CompatibilityLevel.Legacy:
                        flag = (_flags & AdapterSupportedFlags.LegacyLicense) == AdapterSupportedFlags.LegacyLicense;
                        break;
                    default:
                        flag = (_flags & AdapterSupportedFlags.CurrentLicense) == AdapterSupportedFlags.CurrentLicense;
                        break;
                }
                if (!flag)
                {
                    return false;
                }
                return SharePointAdapter.GetAdapterSupportsScope(_flags, scope);
            }
        }

        private readonly DataBoundedComboBox<SharePointAdapterTypeWrapper> m_adapters;

        private readonly DataBoundedComboBox<AuthenticationInitializer> m_initializers;

        private ISharePointConnectionOptionsContainer m_options;

        private readonly object m_lock = new object();

        private volatile bool m_isRefreshing;

        private CompatibilityLevel m_licenseLevel;

        private bool _isValidSPOLicense;

        private IContainer components;

        private CheckEdit w_radioButtonWebApp;

        private LabelControl w_labelAuthenticationType;

        private LabelControl w_labelConnectionType;

        private LabelControl w_lblTargetType;

        private CheckEdit w_radioButtonSite;

        private CheckEdit w_radioButtonFarm;

        private GroupControl w_groupBox;

        private CheckEdit w_cbRememberMe;

        private TextEdit w_txtPassword;

        private LabelControl w_labelPwd;

        private TextEdit w_txtDifferentUser;

        private CheckEdit w_radioButtonNewUser;

        private CheckEdit w_radioButtonCurrentUser;

        private LabelControl w_lblAddress;

        private HelpTipButton w_helpTargetType;

        private XtraAutoCompleteComboEdit w_comboAddress;

        private ComboBoxEdit w_comboConnectionType;

        private ComboBoxEdit w_comboAuthenticationType;

        private HelpTipButton w_helpConnectionType;

        private HelpTipButton w_helpAuthenticationType;

        private CheckEdit w_radioButtonTenant;

        private GroupControl _gbAzureGraphCredentials;

        private LabelControl _lblAzureAppSecret;

        private LabelControl _lblAzureClientId;

        private TextEdit _txtAzureAppSecret;

        private TextEdit _txtAzureClientId;

        protected bool AuthenticateAsCurrentUser
        {
            get
            {
                return w_radioButtonCurrentUser.Checked;
            }
            set
            {
                if (!value)
                {
                    w_radioButtonCurrentUser.Checked = false;
                    w_radioButtonNewUser.Checked = true;
                }
                else
                {
                    w_radioButtonCurrentUser.Checked = true;
                    w_radioButtonNewUser.Checked = false;
                }
            }
        }

        protected AuthenticationInitializer AuthenticationInitializer
        {
            get
            {
                if (m_initializers == null)
                {
                    return null;
                }
                return m_initializers.GetValue(w_comboAuthenticationType.Text) ?? m_initializers.GetDefaultValue();
            }
            set
            {
                if (value == null)
                {
                    w_comboAuthenticationType.SelectedIndex = 0;
                    return;
                }
                Type type = value.GetType();
                AuthenticationInitializer[] dataSource = m_initializers.DataSource;
                foreach (AuthenticationInitializer authenticationInitializer in dataSource)
                {
                    if (authenticationInitializer.GetType().IsAssignableFrom(type))
                    {
                        m_initializers.SelectByValue(authenticationInitializer);
                        return;
                    }
                }
                w_comboAuthenticationType.SelectedIndex = 0;
            }
        }

        protected AzureAdGraphCredentials AzureAdGraphCredentials { get; set; }

        protected ConnectionScope ConnectionScope
        {
            get
            {
                if (w_radioButtonFarm.Checked)
                {
                    return ConnectionScope.Farm;
                }
                if (w_radioButtonWebApp.Checked)
                {
                    return ConnectionScope.WebApp;
                }
                if (w_radioButtonTenant.Checked)
                {
                    return ConnectionScope.Tenant;
                }
                return ConnectionScope.Site;
            }
            set
            {
                switch (value)
                {
                    case ConnectionScope.Site:
                        w_radioButtonSite.Checked = true;
                        break;
                    case ConnectionScope.WebApp:
                        w_radioButtonWebApp.Checked = true;
                        break;
                    case ConnectionScope.Farm:
                        w_radioButtonFarm.Checked = true;
                        break;
                    case ConnectionScope.Tenant:
                        w_radioButtonTenant.Checked = true;
                        break;
                }
            }
        }

        protected SharePointAdapterTypeWrapper ConnectionType
        {
            get
            {
                if (m_adapters == null)
                {
                    return null;
                }
                return m_adapters.GetValue(w_comboConnectionType.Text) ?? m_adapters.GetDefaultValue();
            }
            set
            {
                if (value == null)
                {
                    m_adapters.ComboBox.SelectedIndex = 0;
                    return;
                }
                SharePointAdapterTypeWrapper[] dataSource = m_adapters.DataSource;
                foreach (SharePointAdapterTypeWrapper sharePointAdapterTypeWrapper in dataSource)
                {
                    if (sharePointAdapterTypeWrapper.WrappedType.IsAssignableFrom(value.WrappedType))
                    {
                        m_adapters.SelectByValue(sharePointAdapterTypeWrapper);
                        return;
                    }
                }
                w_comboConnectionType.SelectedIndex = 0;
            }
        }

        protected Credentials Credentials
        {
            get
            {
                if (AuthenticateAsCurrentUser)
                {
                    return new Credentials();
                }
                return new Credentials(w_txtDifferentUser.Text, w_txtPassword.Text.ToSecureString(), w_cbRememberMe.Checked);
            }
            set
            {
                if (value == null || value.IsDefault)
                {
                    AuthenticateAsCurrentUser = true;
                    return;
                }
                AuthenticateAsCurrentUser = false;
                w_txtDifferentUser.Text = value.UserName;
                w_txtPassword.Text = value.Password.ToInsecureString();
                w_cbRememberMe.Checked = value.SavePassword;
            }
        }

        public bool EnableLocationEditing
        {
            get
            {
                return w_comboAddress.Enabled;
            }
            set
            {
                w_comboAddress.Enabled = value;
                w_radioButtonSite.Enabled = value;
                w_radioButtonFarm.Enabled = value;
                w_radioButtonWebApp.Enabled = value;
                w_radioButtonTenant.Enabled = value;
                w_comboConnectionType.Enabled = value;
                w_comboAuthenticationType.Enabled = value;
            }
        }

        public ISharePointConnectionOptionsContainer Options
        {
            get
            {
                return m_options;
            }
            set
            {
                m_options = value;
                LoadUI();
            }
        }

        protected string Url
        {
            get
            {
                return HttpUtility.UrlDecode(w_comboAddress.Text).TrimEnd();
            }
            set
            {
                w_comboAddress.Text = ((value == null) ? string.Empty : value.TrimEnd());
            }
        }

        public TCSharePointConnectionConfig()
        {
            InitializeComponent();
            Type type = GetType();
            w_helpAuthenticationType.SetResourceString(type.FullName + w_comboAuthenticationType.Name, type);
            w_helpConnectionType.SetResourceString(type.FullName + w_comboConnectionType.Name, type);
            w_helpTargetType.SetResourceString(type.FullName + w_lblTargetType.Name, type);
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                SupportedConnectionScopesAttribute[] array = (SupportedConnectionScopesAttribute[])entryAssembly.GetCustomAttributes(typeof(SupportedConnectionScopesAttribute), inherit: false);
                if (array != null && array.Length != 0)
                {
                    ConfigureConnecitonScopeVisibility(array);
                }
                object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(ProductAttribute), inherit: false);
                if (customAttributes != null && customAttributes.Length != 0)
                {
                    Product product = (customAttributes[0] as ProductAttribute).Product;
                    if (product != Product.CMCSharePoint && product != Product.CMCFileShare && product != Product.CMCGoogle)
                    {
                        w_radioButtonTenant.Visible = false;
                        HelpTipButton helpTipButton = w_helpTargetType;
                        helpTipButton.Location = new Point(320, w_helpTargetType.Location.Y);
                    }
                }
            }
            if (SharePointConfigurationVariables.ResolvePrincipalsMethod.ToEnumValue(ResolvePrincipalsMethod.People) != 0)
            {
                HideControl(_gbAzureGraphCredentials);
            }
            CheckSPOLicense();
            m_licenseLevel = LicensingUtils.GetLevel();
            w_radioButtonCurrentUser.Text = WindowsIdentity.GetCurrent().Name;
            m_adapters = new DataBoundedComboBox<SharePointAdapterTypeWrapper>(w_comboConnectionType, GetSharePointAdapterTypes(), (SharePointAdapterTypeWrapper adapter) => adapter.DisplayName);
            m_adapters.Initialize();
            m_initializers = new DataBoundedComboBox<AuthenticationInitializer>(w_comboAuthenticationType, GetAuthenticationInitializers(), (AuthenticationInitializer initializer) => initializer.ToString());
            m_initializers.Initialize();
        }

        private void CheckSPOLicense()
        {
            LicensedSharePointVersions licensedSharePointVersions = LicensingUtils.GetLicensedSharePointVersions();
            _isValidSPOLicense = licensedSharePointVersions.HasFlag(LicensedSharePointVersions.SPOnline);
            if (!_isValidSPOLicense)
            {
                w_radioButtonTenant.Enabled = false;
                VerifyUserActionDialog.GetUserVerification(AdapterConfigurationVariables.ShowSPOLicenseNotice, Resources.SPOLicensingWarningMessage, Resources.SPOLicensingWarningMessageTitle, null, null, MessageBoxButtons.OK);
            }
        }

        private void ConfigureConnecitonScopeVisibility(SupportedConnectionScopesAttribute[] connecitonScopes)
        {
            w_radioButtonFarm.Enabled = false;
            w_radioButtonSite.Enabled = false;
            w_radioButtonTenant.Enabled = false;
            w_radioButtonWebApp.Enabled = false;
            for (int i = 0; i < connecitonScopes.Length; i++)
            {
                switch (connecitonScopes[i].Value)
                {
                    case ConnectionScope.Site:
                        w_radioButtonSite.Enabled = true;
                        break;
                    case ConnectionScope.WebApp:
                        w_radioButtonWebApp.Enabled = true;
                        break;
                    case ConnectionScope.Farm:
                        w_radioButtonFarm.Enabled = true;
                        break;
                    case ConnectionScope.Tenant:
                        w_radioButtonTenant.Enabled = true;
                        break;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private AuthenticationInitializer[] GetAuthenticationInitializers()
        {
            Type[] availableInitializerTypes = AuthenticationInitializer.AvailableInitializerTypes;
            List<AuthenticationInitializer> list = new List<AuthenticationInitializer>(availableInitializerTypes.Length);
            SupportedAuthenticationMethodsAttribute[] array = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                array = (SupportedAuthenticationMethodsAttribute[])entryAssembly.GetCustomAttributes(typeof(SupportedAuthenticationMethodsAttribute), inherit: false);
            }
            if (array == null || array.Length == 0)
            {
                Type[] array2 = availableInitializerTypes;
                for (int i = 0; i < array2.Length; i++)
                {
                    list.Add(AuthenticationInitializer.Create(array2[i]));
                }
            }
            else
            {
                SupportedAuthenticationMethodsAttribute[] array3 = array;
                foreach (SupportedAuthenticationMethodsAttribute supportedAuthenticationMethodsAttribute in array3)
                {
                    list.Add(AuthenticationInitializer.Create(supportedAuthenticationMethodsAttribute.Value));
                }
            }
            list.Sort(delegate (AuthenticationInitializer leftInit, AuthenticationInitializer rightInit)
            {
                int num = leftInit.MenuIndex.CompareTo(rightInit.MenuIndex);
                return (num != 0) ? num : leftInit.MenuText.CompareTo(rightInit.MenuText);
            });
            return list.ToArray();
        }

        private SharePointAdapterTypeWrapper[] GetSharePointAdapterTypes()
        {
            Type[] availableAdapterTypes = SharePointAdapter.AvailableAdapterTypes;
            List<SharePointAdapterTypeWrapper> list = new List<SharePointAdapterTypeWrapper>(availableAdapterTypes.Length);
            SupportedAdaptersAttribute[] array = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                array = (SupportedAdaptersAttribute[])entryAssembly.GetCustomAttributes(typeof(SupportedAdaptersAttribute), inherit: false);
            }
            if (array == null || array.Length == 0)
            {
                Type[] array2 = availableAdapterTypes;
                foreach (Type type in array2)
                {
                    if (ShowInMenuAttribute.GetAdapterShowInMenu(type) && Is2007or2010OMConnection(type))
                    {
                        list.Add(type);
                    }
                }
            }
            else
            {
                SupportedAdaptersAttribute[] array3 = array;
                for (int j = 0; j < array3.Length; j++)
                {
                    list.Add(array3[j].Value);
                }
            }
            list.Sort(SharePointAdapterTypeWrapper.CompareForMenuSort);
            return list.ToArray();
        }

        private void InitializeComponent()
        {
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.TCSharePointConnectionConfig));
            this.w_radioButtonWebApp = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_labelAuthenticationType = new global::DevExpress.XtraEditors.LabelControl();
            this.w_labelConnectionType = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblTargetType = new global::DevExpress.XtraEditors.LabelControl();
            this.w_radioButtonSite = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_radioButtonFarm = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_groupBox = new global::DevExpress.XtraEditors.GroupControl();
            this.w_cbRememberMe = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_txtPassword = new global::DevExpress.XtraEditors.TextEdit();
            this.w_labelPwd = new global::DevExpress.XtraEditors.LabelControl();
            this.w_txtDifferentUser = new global::DevExpress.XtraEditors.TextEdit();
            this.w_radioButtonNewUser = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_radioButtonCurrentUser = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_lblAddress = new global::DevExpress.XtraEditors.LabelControl();
            this.w_helpTargetType = new global::TooltipsTest.HelpTipButton();
            this.w_comboAddress = new global::Metalogix.UI.WinForms.Components.XtraAutoCompleteComboEdit();
            this.w_comboConnectionType = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_comboAuthenticationType = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_helpConnectionType = new global::TooltipsTest.HelpTipButton();
            this.w_helpAuthenticationType = new global::TooltipsTest.HelpTipButton();
            this.w_radioButtonTenant = new global::DevExpress.XtraEditors.CheckEdit();
            this._gbAzureGraphCredentials = new global::DevExpress.XtraEditors.GroupControl();
            this._txtAzureAppSecret = new global::DevExpress.XtraEditors.TextEdit();
            this._txtAzureClientId = new global::DevExpress.XtraEditors.TextEdit();
            this._lblAzureClientId = new global::DevExpress.XtraEditors.LabelControl();
            this._lblAzureAppSecret = new global::DevExpress.XtraEditors.LabelControl();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonWebApp.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonSite.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonFarm.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_groupBox).BeginInit();
            this.w_groupBox.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbRememberMe.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtPassword.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtDifferentUser.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonNewUser.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonCurrentUser.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_helpTargetType).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboAddress.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboConnectionType.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboAuthenticationType.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_helpConnectionType).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_helpAuthenticationType).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonTenant.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this._gbAzureGraphCredentials).BeginInit();
            this._gbAzureGraphCredentials.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this._txtAzureAppSecret.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this._txtAzureClientId.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_radioButtonWebApp, "w_radioButtonWebApp");
            this.w_radioButtonWebApp.Name = "w_radioButtonWebApp";
            this.w_radioButtonWebApp.Properties.AutoWidth = true;
            this.w_radioButtonWebApp.Properties.Caption = resources.GetString("w_radioButtonWebApp.Properties.Caption");
            this.w_radioButtonWebApp.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioButtonWebApp.Properties.RadioGroupIndex = 1;
            this.w_radioButtonWebApp.TabStop = false;
            this.w_radioButtonWebApp.CheckedChanged += new global::System.EventHandler(w_radioButtonConnectionScope_CheckedChanged);
            resources.ApplyResources(this.w_labelAuthenticationType, "w_labelAuthenticationType");
            this.w_labelAuthenticationType.Name = "w_labelAuthenticationType";
            resources.ApplyResources(this.w_labelConnectionType, "w_labelConnectionType");
            this.w_labelConnectionType.Name = "w_labelConnectionType";
            resources.ApplyResources(this.w_lblTargetType, "w_lblTargetType");
            this.w_lblTargetType.Name = "w_lblTargetType";
            resources.ApplyResources(this.w_radioButtonSite, "w_radioButtonSite");
            this.w_radioButtonSite.Name = "w_radioButtonSite";
            this.w_radioButtonSite.Properties.AutoWidth = true;
            this.w_radioButtonSite.Properties.Caption = resources.GetString("w_radioButtonSite.Properties.Caption");
            this.w_radioButtonSite.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioButtonSite.Properties.RadioGroupIndex = 1;
            this.w_radioButtonSite.CheckedChanged += new global::System.EventHandler(w_radioButtonConnectionScope_CheckedChanged);
            resources.ApplyResources(this.w_radioButtonFarm, "w_radioButtonFarm");
            this.w_radioButtonFarm.Name = "w_radioButtonFarm";
            this.w_radioButtonFarm.Properties.AutoWidth = true;
            this.w_radioButtonFarm.Properties.Caption = resources.GetString("w_radioButtonFarm.Properties.Caption");
            this.w_radioButtonFarm.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioButtonFarm.Properties.RadioGroupIndex = 1;
            this.w_radioButtonFarm.TabStop = false;
            this.w_radioButtonFarm.CheckedChanged += new global::System.EventHandler(w_radioButtonConnectionScope_CheckedChanged);
            resources.ApplyResources(this.w_groupBox, "w_groupBox");
            this.w_groupBox.Controls.Add(this.w_cbRememberMe);
            this.w_groupBox.Controls.Add(this.w_txtPassword);
            this.w_groupBox.Controls.Add(this.w_labelPwd);
            this.w_groupBox.Controls.Add(this.w_txtDifferentUser);
            this.w_groupBox.Controls.Add(this.w_radioButtonNewUser);
            this.w_groupBox.Controls.Add(this.w_radioButtonCurrentUser);
            this.w_groupBox.Name = "w_groupBox";
            resources.ApplyResources(this.w_cbRememberMe, "w_cbRememberMe");
            this.w_cbRememberMe.Name = "w_cbRememberMe";
            this.w_cbRememberMe.Properties.AutoWidth = true;
            this.w_cbRememberMe.Properties.Caption = resources.GetString("w_cbRememberMe.Properties.Caption");
            resources.ApplyResources(this.w_txtPassword, "w_txtPassword");
            this.w_txtPassword.Name = "w_txtPassword";
            this.w_txtPassword.Properties.Appearance.BackColor = (global::System.Drawing.Color)resources.GetObject("w_txtPassword.Properties.Appearance.BackColor");
            this.w_txtPassword.Properties.Appearance.Options.UseBackColor = true;
            this.w_txtPassword.Properties.PasswordChar = '*';
            resources.ApplyResources(this.w_labelPwd, "w_labelPwd");
            this.w_labelPwd.Name = "w_labelPwd";
            resources.ApplyResources(this.w_txtDifferentUser, "w_txtDifferentUser");
            this.w_txtDifferentUser.Name = "w_txtDifferentUser";
            this.w_txtDifferentUser.Properties.Appearance.BackColor = (global::System.Drawing.Color)resources.GetObject("w_txtDifferentUser.Properties.Appearance.BackColor");
            this.w_txtDifferentUser.Properties.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this.w_radioButtonNewUser, "w_radioButtonNewUser");
            this.w_radioButtonNewUser.Name = "w_radioButtonNewUser";
            this.w_radioButtonNewUser.Properties.AutoWidth = true;
            this.w_radioButtonNewUser.Properties.Caption = resources.GetString("w_radioButtonNewUser.Properties.Caption");
            this.w_radioButtonNewUser.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioButtonNewUser.Properties.RadioGroupIndex = 2;
            this.w_radioButtonNewUser.TabStop = false;
            this.w_radioButtonNewUser.CheckedChanged += new global::System.EventHandler(w_radioButtonNewUser_CheckedChanged);
            this.w_radioButtonNewUser.EnabledChanged += new global::System.EventHandler(w_radioButtonNewUser_EnabledChanged);
            resources.ApplyResources(this.w_radioButtonCurrentUser, "w_radioButtonCurrentUser");
            this.w_radioButtonCurrentUser.Name = "w_radioButtonCurrentUser";
            this.w_radioButtonCurrentUser.Properties.AutoWidth = true;
            this.w_radioButtonCurrentUser.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioButtonCurrentUser.Properties.RadioGroupIndex = 2;
            resources.ApplyResources(this.w_lblAddress, "w_lblAddress");
            this.w_lblAddress.Name = "w_lblAddress";
            this.w_helpTargetType.AnchoringControl = this.w_radioButtonFarm;
            this.w_helpTargetType.BackColor = global::System.Drawing.Color.Transparent;
            this.w_helpTargetType.CommonParentControl = null;
            resources.ApplyResources(this.w_helpTargetType, "w_helpTargetType");
            this.w_helpTargetType.Name = "w_helpTargetType";
            this.w_helpTargetType.TabStop = false;
            resources.ApplyResources(this.w_comboAddress, "w_comboAddress");
            this.w_comboAddress.Name = "w_comboAddress";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_comboAddress.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            resources.ApplyResources(this.w_comboConnectionType, "w_comboConnectionType");
            this.w_comboConnectionType.Name = "w_comboConnectionType";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons3 = this.w_comboConnectionType.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons4 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons3.AddRange(buttons4);
            this.w_comboConnectionType.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_comboConnectionType.SelectedIndexChanged += new global::System.EventHandler(w_comboConnectionType_SelectedIndexChanged);
            resources.ApplyResources(this.w_comboAuthenticationType, "w_comboAuthenticationType");
            this.w_comboAuthenticationType.Name = "w_comboAuthenticationType";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons5 = this.w_comboAuthenticationType.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons6 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons5.AddRange(buttons6);
            this.w_comboAuthenticationType.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_helpConnectionType.AnchoringControl = this.w_comboConnectionType;
            this.w_helpConnectionType.BackColor = global::System.Drawing.Color.Transparent;
            this.w_helpConnectionType.CommonParentControl = null;
            resources.ApplyResources(this.w_helpConnectionType, "w_helpConnectionType");
            this.w_helpConnectionType.Name = "w_helpConnectionType";
            this.w_helpConnectionType.TabStop = false;
            this.w_helpAuthenticationType.AnchoringControl = this.w_comboAuthenticationType;
            this.w_helpAuthenticationType.BackColor = global::System.Drawing.Color.Transparent;
            this.w_helpAuthenticationType.CommonParentControl = null;
            resources.ApplyResources(this.w_helpAuthenticationType, "w_helpAuthenticationType");
            this.w_helpAuthenticationType.Name = "w_helpAuthenticationType";
            this.w_helpAuthenticationType.TabStop = false;
            resources.ApplyResources(this.w_radioButtonTenant, "w_radioButtonTenant");
            this.w_radioButtonTenant.Name = "w_radioButtonTenant";
            this.w_radioButtonTenant.Properties.AutoWidth = true;
            this.w_radioButtonTenant.Properties.Caption = resources.GetString("w_radioButtonTenant.Properties.Caption");
            this.w_radioButtonTenant.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioButtonTenant.Properties.RadioGroupIndex = 1;
            this.w_radioButtonTenant.TabStop = false;
            this.w_radioButtonTenant.CheckedChanged += new global::System.EventHandler(w_radioButtonConnectionScope_CheckedChanged);
            resources.ApplyResources(this._gbAzureGraphCredentials, "_gbAzureGraphCredentials");
            this._gbAzureGraphCredentials.Controls.Add(this._lblAzureAppSecret);
            this._gbAzureGraphCredentials.Controls.Add(this._lblAzureClientId);
            this._gbAzureGraphCredentials.Controls.Add(this._txtAzureAppSecret);
            this._gbAzureGraphCredentials.Controls.Add(this._txtAzureClientId);
            this._gbAzureGraphCredentials.Name = "_gbAzureGraphCredentials";
            resources.ApplyResources(this._txtAzureAppSecret, "_txtAzureAppSecret");
            this._txtAzureAppSecret.Name = "_txtAzureAppSecret";
            this._txtAzureAppSecret.Properties.Appearance.BackColor = (global::System.Drawing.Color)resources.GetObject("textEdit1.Properties.Appearance.BackColor");
            this._txtAzureAppSecret.Properties.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this._txtAzureClientId, "_txtAzureClientId");
            this._txtAzureClientId.Name = "_txtAzureClientId";
            this._txtAzureClientId.Properties.Appearance.BackColor = (global::System.Drawing.Color)resources.GetObject("textEdit2.Properties.Appearance.BackColor");
            this._txtAzureClientId.Properties.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this._lblAzureClientId, "_lblAzureClientId");
            this._lblAzureClientId.Name = "_lblAzureClientId";
            resources.ApplyResources(this._lblAzureAppSecret, "_lblAzureAppSecret");
            this._lblAzureAppSecret.Name = "_lblAzureAppSecret";
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this._gbAzureGraphCredentials);
            base.Controls.Add(this.w_radioButtonTenant);
            base.Controls.Add(this.w_helpAuthenticationType);
            base.Controls.Add(this.w_helpConnectionType);
            base.Controls.Add(this.w_comboAuthenticationType);
            base.Controls.Add(this.w_comboConnectionType);
            base.Controls.Add(this.w_comboAddress);
            base.Controls.Add(this.w_helpTargetType);
            base.Controls.Add(this.w_radioButtonWebApp);
            base.Controls.Add(this.w_labelAuthenticationType);
            base.Controls.Add(this.w_labelConnectionType);
            base.Controls.Add(this.w_lblTargetType);
            base.Controls.Add(this.w_radioButtonSite);
            base.Controls.Add(this.w_radioButtonFarm);
            base.Controls.Add(this.w_groupBox);
            base.Controls.Add(this.w_lblAddress);
            this.MaximumSize = new global::System.Drawing.Size(1000, 360);
            this.MinimumSize = new global::System.Drawing.Size(306, 360);
            base.Name = "TCSharePointConnectionConfig";
            base.Load += new global::System.EventHandler(On_LoginTab_Loaded);
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonWebApp.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonSite.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonFarm.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_groupBox).EndInit();
            this.w_groupBox.ResumeLayout(false);
            this.w_groupBox.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbRememberMe.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtPassword.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtDifferentUser.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonNewUser.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonCurrentUser.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_helpTargetType).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboAddress.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboConnectionType.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboAuthenticationType.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_helpConnectionType).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_helpAuthenticationType).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_radioButtonTenant.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this._gbAzureGraphCredentials).EndInit();
            this._gbAzureGraphCredentials.ResumeLayout(false);
            this._gbAzureGraphCredentials.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this._txtAzureAppSecret.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this._txtAzureClientId.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool Is2007or2010OMConnection(Type adapterType)
        {
            if (adapterType.Name.Equals("OMAdapter", StringComparison.OrdinalIgnoreCase) && (SharePointUtils.IsRegistryKeyExists("SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\14.0") || (!AdapterConfigurationVariables.Show2007OMConnection && SharePointUtils.IsRegistryKeyExists("SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\12.0"))))
            {
                return false;
            }
            return true;
        }

        protected override void LoadUI()
        {
            if (Options != null)
            {
                Url = Options.Url;
                Credentials = Options.Credentials;
                AuthenticationInitializer = Options.AuthenticationInitializer;
                ConnectionType = Options.ConnectionType;
                ConnectionScope = Options.ConnectionScope;
            }
        }

        private void On_LoginTab_Loaded(object sender, EventArgs e)
        {
            if (EnableLocationEditing)
            {
                base.ActiveControl = w_comboAddress;
            }
            else
            {
                base.ActiveControl = w_txtPassword;
            }
        }

        private void RefreshAzureAdGraphControls()
        {
            if (ConnectionScope == ConnectionScope.Site || ConnectionScope == ConnectionScope.Tenant)
            {
                _gbAzureGraphCredentials.Enabled = true;
                return;
            }
            _gbAzureGraphCredentials.Enabled = false;
            _txtAzureClientId.Text = string.Empty;
            _txtAzureAppSecret.Text = string.Empty;
        }

        private void RefreshFilters()
        {
            if (m_isRefreshing)
            {
                return;
            }
            lock (m_lock)
            {
                if (m_isRefreshing)
                {
                    return;
                }
                try
                {
                    m_isRefreshing = true;
                    if (m_adapters != null)
                    {
                        List<SharePointAdapterTypeWrapper> list = new List<SharePointAdapterTypeWrapper>();
                        SharePointAdapterTypeWrapper[] dataSource = m_adapters.DataSource;
                        foreach (SharePointAdapterTypeWrapper sharePointAdapterTypeWrapper in dataSource)
                        {
                            if (sharePointAdapterTypeWrapper.SupportsState(ConnectionScope, m_licenseLevel) && (AuthenticationInitializer == null || AuthenticationInitializer.CompatibleWithAdapter(sharePointAdapterTypeWrapper)))
                            {
                                list.Add(sharePointAdapterTypeWrapper);
                            }
                        }
                        m_adapters.FilterByValue(list);
                    }
                    if (m_initializers == null)
                    {
                        return;
                    }
                    List<AuthenticationInitializer> list2 = new List<AuthenticationInitializer>();
                    AuthenticationInitializer[] dataSource2 = m_initializers.DataSource;
                    foreach (AuthenticationInitializer authenticationInitializer in dataSource2)
                    {
                        if (authenticationInitializer.CompatibleWithAdapter(ConnectionType))
                        {
                            list2.Add(authenticationInitializer);
                        }
                    }
                    m_initializers.FilterByValue(list2);
                }
                finally
                {
                    m_isRefreshing = false;
                }
            }
        }

        public override bool SaveUI()
        {
            if (Url == null || Url.Trim() == "")
            {
                FlatXtraMessageBox.Show("The Site Address cannot be empty", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if ((w_radioButtonFarm.Checked || w_radioButtonWebApp.Checked) && (ConnectionType.ShortName == "NW" || ConnectionType.ShortName == "CSOM"))
            {
                string text = $"The {ConnectionType.DisplayName} must have the Target Type as SharePoint Site.";
                FlatXtraMessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                w_radioButtonSite.Checked = true;
                return false;
            }
            if (!AuthenticationInitializer.AutomaticAuthenticationEnabled && !VerifyUserActionDialog.GetUserVerification(SharePointConfigurationVariables.ShowManualLoginInformationDialog, Resources.ManualReconnectionRequiredDialog))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(_txtAzureClientId.Text) && !string.IsNullOrEmpty(_txtAzureAppSecret.Text))
            {
                AzureAdGraphCredentials = new AzureAdGraphCredentials(_txtAzureClientId.Text, _txtAzureAppSecret.Text);
            }
            if (Options != null)
            {
                Options.AuthenticationInitializer = AuthenticationInitializer;
                Options.ConnectionScope = ConnectionScope;
                Options.ConnectionType = ConnectionType;
                Options.Credentials = Credentials;
                Options.AzureAdGraphCredentials = AzureAdGraphCredentials;
                Options.Url = Url;
            }
            return true;
        }

        public void SetVisitedServers(string[] visitedSPServers)
        {
            w_comboAddress.Properties.Items.Clear();
            w_comboAddress.SelectedText = string.Empty;
            if (visitedSPServers != null && visitedSPServers.Length != 0)
            {
                w_comboAddress.Properties.Items.AddRange(visitedSPServers);
                w_comboAddress.SelectedText = visitedSPServers[0];
            }
        }

        private void w_comboAuthenticationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AuthenticationInitializer != null)
            {
                switch (AuthenticationInitializer.CredentialEntryStyle)
                {
                    case EntryStyles.ForceDefault:
                        w_radioButtonCurrentUser.Enabled = true;
                        w_radioButtonCurrentUser.Checked = true;
                        w_radioButtonNewUser.Enabled = false;
                        w_labelPwd.Enabled = false;
                        break;
                    case EntryStyles.ForceEntry:
                        w_radioButtonCurrentUser.Enabled = false;
                        w_radioButtonNewUser.Enabled = true;
                        w_radioButtonNewUser.Checked = true;
                        w_labelPwd.Enabled = true;
                        break;
                    case EntryStyles.Either:
                        w_radioButtonCurrentUser.Enabled = true;
                        w_radioButtonNewUser.Enabled = true;
                        w_labelPwd.Enabled = true;
                        break;
                    case EntryStyles.None:
                        w_radioButtonCurrentUser.Enabled = false;
                        w_radioButtonCurrentUser.Checked = true;
                        w_radioButtonNewUser.Enabled = false;
                        w_labelPwd.Enabled = false;
                        break;
                }
            }
            RefreshFilters();
        }

        private void w_comboConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFilters();
            RefreshAzureAdGraphControls();
        }

        private void w_radioButtonConnectionScope_CheckedChanged(object sender, EventArgs e)
        {
            RefreshFilters();
            if (!w_radioButtonTenant.Checked)
            {
                w_radioButtonCurrentUser.Enabled = true;
                w_radioButtonCurrentUser.Checked = true;
            }
            else
            {
                w_radioButtonCurrentUser.Enabled = false;
                w_radioButtonNewUser.Checked = true;
            }
            RefreshAzureAdGraphControls();
        }

        private void w_radioButtonNewUser_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = w_radioButtonNewUser.Enabled && w_radioButtonNewUser.Checked;
            w_txtDifferentUser.Enabled = enabled;
            w_txtPassword.Enabled = enabled;
            w_cbRememberMe.Enabled = enabled;
        }

        private void w_radioButtonNewUser_EnabledChanged(object sender, EventArgs e)
        {
            w_radioButtonNewUser_CheckedChanged(sender, e);
        }
    }
}