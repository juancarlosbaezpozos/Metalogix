using Metalogix;
using Metalogix.Core;
using Metalogix.Licensing;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Metalogix.Commands
{
	public abstract class AssemblyBindingCmdlet : PSCmdlet
	{
		private bool _licenseInitializerRun;

		private Assembly _previousLicensingAssembly;

		private Assembly _currentLicensingAssembly;

		static AssemblyBindingCmdlet()
		{
			AssemblyBindingCmdlet.RegisterAssemblyResolve();
		}

		protected AssemblyBindingCmdlet() : this(true)
		{
		}

		protected AssemblyBindingCmdlet(bool initializeLicense)
		{
			this.ConfigureGlobalConfigVarSettings();
			if (!initializeLicense)
			{
				return;
			}
			this.InitializeMainAssembly();
			this.InitializeLicense();
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
		}

		private void ConfigureGlobalConfigVarSettings()
		{
			GlobalConfigurationVariableSettings.ClearApplicationVariablesOnMainAssemblyChanged = false;
		}

		protected override void EndProcessing()
		{
			base.EndProcessing();
			this.RevertLicense();
		}

		protected void InitializeLicense()
		{
			if (this._licenseInitializerRun)
			{
				return;
			}
			Assembly assembly = this.ResolveLicensingAssembly();
			if (MLLicenseProvider.IsInitialized)
			{
				if (object.Equals(this._currentLicensingAssembly, assembly))
				{
					return;
				}
				MLLicenseProvider.Dispose();
				this._previousLicensingAssembly = this._currentLicensingAssembly;
			}
			if (!object.Equals(ApplicationData.MainAssembly, assembly))
			{
				ApplicationData.MainAssembly = assembly;
			}
			MLLicenseProvider.Initialize(LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(assembly));
			this._currentLicensingAssembly = assembly;
			this._licenseInitializerRun = true;
		}

		protected void InitializeMainAssembly()
		{
			DirectoryInfo parent = (new DirectoryInfo(base.GetType().Assembly.Location)).Parent;
			FileInfo[] files = parent.GetFiles("*.exe");
			for (int i = 0; i < (int)files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				try
				{
					Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
					if (ApplicationData.GetSignature(assembly.FullName) == ApplicationData.GetSignature(Assembly.GetExecutingAssembly().FullName))
					{
						ApplicationData.MainAssembly = assembly;
						return;
					}
				}
				catch
				{
				}
			}
			throw new FileNotFoundException("Application executable was not found.");
		}

		private static void RegisterAssemblyResolve()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolver.ResolveAssembly);
		}

		private Assembly ResolveLicensingAssembly()
		{
			Assembly assembly = base.GetType().Assembly;
			DirectoryInfo parent = (new DirectoryInfo(assembly.Location)).Parent;
			FileInfo[] files = parent.GetFiles("*.exe");
			for (int i = 0; i < (int)files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				try
				{
					Assembly assembly1 = Assembly.LoadFrom(fileInfo.FullName);
					if (ReflectionUtils.GetSignature(assembly1.FullName) == ReflectionUtils.GetSignature(Assembly.GetExecutingAssembly().FullName))
					{
						return assembly1;
					}
				}
				catch (Exception exception)
				{
				}
			}
			return assembly;
		}

		protected void RevertLicense()
		{
			if (this._previousLicensingAssembly != null && !object.Equals(this._currentLicensingAssembly, this._previousLicensingAssembly))
			{
				MLLicenseProvider.Dispose();
				MLLicenseProvider.Initialize(LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(this._previousLicensingAssembly));
				this._licenseInitializerRun = false;
			}
		}
	}
}