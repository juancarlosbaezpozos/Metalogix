using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using Metalogix;
using Metalogix.DataStructures;
using Metalogix.Licensing;
using Metalogix.Licensing.CA;
using Metalogix.Licensing.Common;
using Metalogix.UI.WinForms.Attributes;
using Metalogix.UI.WinForms.Licensing.Common;
using Metalogix.Utilities;
using Metalogix.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public sealed class UIApplication
	{
		private static UIApplication _instance;

		private UIMainForm m_mainForm;

		private string m_productCode;

		private int? m_productId;

		private Type m_formType;

		private bool? m_enableMultiSelect;

		private EnhancedTreeView.AllowSelectionDelegate m_multiSelectLimiterDelegate;

		private bool? m_enableItemDataConverter;

		private IDataConverter<object, string> m_itemDataConverter;

		private Icon m_icon;

		private Image _applicationIcon;

		private Image m_splashImage;

		private Image _expressSplashScreenImage;

		private string m_licenseTrialWarningMessage;

		private bool? _allowsAdvancedModeToggle = null;

		public Icon AppIcon
		{
			get
			{
				if (this.m_icon == null)
				{
					this.m_icon = this.GetApplicationAttribute<FormIconAttribute>().GetIcon();
				}
				return this.m_icon;
			}
		}

		public Image ApplicationIcon
		{
			get
			{
				if (this._applicationIcon == null)
				{
					this._applicationIcon = this.GetApplicationAttribute<ApplicationIconAttribute>().GetApplicationIcon();
				}
				return this._applicationIcon;
			}
		}

		public Image ExpressSplashScreenImage
		{
			get
			{
				if (this._expressSplashScreenImage == null)
				{
					this._expressSplashScreenImage = this.GetApplicationAttribute<SplashScreenAttribute>().GetExpressImage();
				}
				return this._expressSplashScreenImage;
			}
		}

		public Type FormType
		{
			get
			{
				if (this.m_formType == null)
				{
					this.m_formType = this.GetApplicationAttribute<FormTypeAttribute>().FormType;
				}
				return this.m_formType;
			}
		}

		public static UIApplication INSTANCE
		{
			get
			{
				if (UIApplication._instance == null)
				{
					UIApplication._instance = new UIApplication();
				}
				return UIApplication._instance;
			}
		}

		public IDataConverter<object, string> ItemDataConverter
		{
			get
			{
				return this.m_itemDataConverter;
			}
		}

		public bool ItemDataConverterEnabled
		{
			get
			{
				if (!this.m_enableItemDataConverter.HasValue)
				{
					ItemDataConverterAttribute applicationAttribute = this.GetApplicationAttribute<ItemDataConverterAttribute>();
					if (applicationAttribute == null)
					{
						this.m_enableItemDataConverter = new bool?(false);
						return this.m_enableItemDataConverter.Value;
					}
					this.m_enableItemDataConverter = new bool?(applicationAttribute.Enabled);
					this.m_itemDataConverter = applicationAttribute.CreateDataConverter();
				}
				return this.m_enableItemDataConverter.Value;
			}
		}

		public string LicenseTrialWarningMessage
		{
			get
			{
				if (this.m_licenseTrialWarningMessage == null)
				{
					LicenseWarningMessagesAttribute applicationAttribute = this.GetApplicationAttribute<LicenseWarningMessagesAttribute>();
					this.m_licenseTrialWarningMessage = (applicationAttribute != null ? applicationAttribute.TrialWarningMessage : "You are using an evaluation license.");
				}
				return this.m_licenseTrialWarningMessage;
			}
		}

		public UIMainForm MainForm
		{
			get
			{
				if (this.m_mainForm == null)
				{
					this.m_mainForm = this.CreateForm<UIMainForm>(this.FormType);
					this.m_mainForm.Application = this;
					this.m_mainForm.Text = string.Concat(this.ProductName, (UIConfigurationVariables.ShowProductVersionInAppTitle ? string.Concat(" - ", this.ProductVersion) : string.Empty));
					this.m_mainForm.Icon = this.AppIcon;
					Skin skin = RibbonSkins.GetSkin(UserLookAndFeel.Default);
					SkinElement item = skin[RibbonSkins.SkinFormApplicationButton];
					item.Image.SetImage(this.ApplicationIcon, Color.Empty);
					LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
				}
				return this.m_mainForm;
			}
		}

		public bool MultiSelectEnabled
		{
			get
			{
				if (!this.m_enableMultiSelect.HasValue)
				{
					MultiSelectAttribute applicationAttribute = this.GetApplicationAttribute<MultiSelectAttribute>();
					if (applicationAttribute == null)
					{
						this.m_enableMultiSelect = new bool?(false);
						return this.m_enableMultiSelect.Value;
					}
					this.m_enableMultiSelect = new bool?(applicationAttribute.Enabled);
					this.m_multiSelectLimiterDelegate = applicationAttribute.CreateLimiterDelegate();
				}
				return this.m_enableMultiSelect.Value;
			}
		}

		public EnhancedTreeView.AllowSelectionDelegate MultiSelectLimitationMethod
		{
			get
			{
				return this.m_multiSelectLimiterDelegate;
			}
		}

		public string ProductCode
		{
			get
			{
				if (this.m_productCode == null)
				{
					ProductAttribute applicationAttribute = this.GetApplicationAttribute<ProductAttribute>();
					this.m_productCode = applicationAttribute.Product.ToString();
				}
				return this.m_productCode;
			}
		}

	    // Metalogix.UI.WinForms.UIApplication
	    public int ProductId
	    {
	        get
	        {
	            if (!this.m_productId.HasValue)
	            {
	                ProductAttribute applicationAttribute = this.GetApplicationAttribute<ProductAttribute>();
	                if (applicationAttribute == null)
	                {
	                    this.m_productId = new int?(-1);
	                    return this.m_productId.Value;
	                }
	                this.m_productId = new int?((int)applicationAttribute.Product);
	            }
	            return this.m_productId.Value;
	        }
	    }


        public string ProductName
		{
			get
			{
				return Application.ProductName;
			}
		}

		public string ProductVersion
		{
			get
			{
				return Application.ProductVersion;
			}
		}

		public bool ShowAdvancedModeToggleButton
		{
			get
			{
				if (!this._allowsAdvancedModeToggle.HasValue)
				{
					this._allowsAdvancedModeToggle = new bool?((
						from attribute in (IEnumerable<ApplicationSettingAttribute>)ReflectionUtils.GetApplicationAttributesMultiple<ApplicationSettingAttribute>()
						where attribute.SettingType == typeof(ShowAdvancedSetting)
						select attribute).Any<ApplicationSettingAttribute>());
				}
				return this._allowsAdvancedModeToggle.Value;
			}
		}

		public Image SplashScreenImage
		{
			get
			{
				if (this.m_splashImage == null)
				{
					this.m_splashImage = this.GetApplicationAttribute<SplashScreenAttribute>().GetImage();
				}
				return this.m_splashImage;
			}
		}

		static UIApplication()
		{
		}

		private UIApplication()
		{
		}

		public T CreateForm<T>()
		where T : Form
		{
			return this.CreateForm<T>(typeof(T));
		}

		public T CreateForm<T>(Type formType)
		where T : Form
		{
			return (T)this.CreateForm(formType);
		}

		public Form CreateForm(Type formType)
		{
			if (formType == null)
			{
				throw new ArgumentNullException("formType");
			}
			return (Form)Activator.CreateInstance(formType);
		}

		private T GetApplicationAttribute<T>()
		where T : Attribute
		{
			Assembly mainAssembly = ApplicationData.MainAssembly;
			if (mainAssembly == null)
			{
				return default(T);
			}
			object[] customAttributes = mainAssembly.GetCustomAttributes(typeof(T), true);
			if (customAttributes == null || (int)customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)customAttributes[0];
		}

		public MLLicense GetLicense()
		{
			MLLicense mLLicense = null;
			if (LicenseManager.CurrentContext.UsageMode == LicenseUsageMode.Runtime)
			{
				try
				{
					mLLicense = (MLLicense)LicenseManager.Validate(typeof(UIMainForm), this.MainForm);
				}
				catch (Exception exception)
				{
				}
				if (mLLicense == null)
				{
					mLLicense = this.SetLicense(true);
				}
			}
			return mLLicense;
		}

		public UISplashForm NewSplashScreen()
		{
			UISplashForm uISplashForm = new UISplashForm()
			{
				Application = this,
				Text = string.Concat(this.ProductName, " - ", this.ProductVersion),
				Icon = this.AppIcon
			};
			return uISplashForm;
		}

		public MLLicense SetLicense(bool fillLicenseIfExists)
		{
			string licenseKey;
			MLLicense mLLicense = null;
			ILicensingDialogServiceProvider licensingDialogServiceProvider = MigrationDialogServiceProvider.CreateInstance();
			if (licensingDialogServiceProvider == null)
			{
				string str = null;
				string str1 = null;
				string str2 = null;
				MLLicenseProviderCA.GetLicenseInfoFromType(this.GetType(), out str, out str1, out str2);
				LicenseActivation licenseActivation = new LicenseActivation(str, str1);
				if (licenseActivation.ShowDialog() != DialogResult.OK)
				{
					return null;
				}
				MLLicenseCA mLLicenseCA = new MLLicenseCA(licenseActivation.LicenseKey);
				if (!mLLicenseCA.IsWellFormed)
				{
					throw new Exception("Invalid license returned from License activation dialog");
				}
				FileInfo fileInfo = new FileInfo(str2);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				mLLicenseCA.SaveToFile(str2);
				mLLicense = mLLicenseCA;
				MLLicenseProvider.Notify();
			}
			else
			{
				if (fillLicenseIfExists)
				{
					licenseKey = licensingDialogServiceProvider.LicenseKey;
				}
				else
				{
					licenseKey = null;
				}
				LicenseNewKeyWizard licenseNewKeyWizard = new LicenseNewKeyWizard(licensingDialogServiceProvider, licenseKey);
				if (this.m_mainForm == null || !this.m_mainForm.Visible)
				{
					licenseNewKeyWizard.StartPosition = FormStartPosition.CenterScreen;
				}
				licenseNewKeyWizard.ShowDialog();
				try
				{
					mLLicense = (MLLicense)LicenseManager.Validate(typeof(UIMainForm), this);
				}
				catch (Exception exception)
				{
					mLLicense = null;
				}
			}
			return mLLicense;
		}
	}
}