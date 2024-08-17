using Metalogix;
using Metalogix.Explorer;
using Metalogix.Licensing;
using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Metalogix.Commands
{
	public class NodeDriveInfo : PSDriveInfo
	{
		private Metalogix.Explorer.Connection m_connection;

		private DirectoryInfo _contextDirectory;

		private bool _licenseInitializerRun;

		public Metalogix.Explorer.Connection Connection
		{
			get
			{
				return this.m_connection;
			}
			set
			{
				this.m_connection = value;
			}
		}

		public NodeDriveInfo(PSDriveInfo driveInfo) : base(driveInfo)
		{
			string location = Assembly.GetExecutingAssembly().Location;
			this._contextDirectory = (new FileInfo(location)).Directory.Parent;
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
			this.InitializeLicense();
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string str = null;
			AssemblyName assemblyName = new AssemblyName(args.Name);
			string str1 = string.Concat(assemblyName.Name, ".dll");
			DirectoryInfo[] directories = this._contextDirectory.GetDirectories();
			int num = 0;
			while (num < (int)directories.Length)
			{
				DirectoryInfo directoryInfo = directories[num];
				string str2 = Path.Combine(directoryInfo.FullName, str1);
				if (!File.Exists(str2))
				{
					num++;
				}
				else
				{
					str = str2;
					break;
				}
			}
			if (str == null)
			{
				return null;
			}
			return Assembly.LoadFrom(str);
		}

		protected void DisposeLicense()
		{
			MLLicenseProvider.Dispose();
			ApplicationData.MainAssembly = null;
			this._licenseInitializerRun = false;
		}

		public Node GetCurrentNode()
		{
			string str = base.CurrentLocation.Replace("\\", "/");
			Node node = this.Connection.Node;
			if (!this.PathIsDrive(str))
			{
				node = node.GetNodeByPath(this.GetDriveRelativePath(str));
			}
			return node;
		}

		private string GetDriveRelativePath(string sPath)
		{
			sPath = sPath.Replace("\\", "/");
			string str = base.Root.Replace("\\", "/");
			string str1 = sPath;
			if (sPath.StartsWith(str))
			{
				str1 = sPath.Substring(str.Length, sPath.Length - str.Length);
			}
			str1 = str1.TrimStart(new char[] { '/' });
			return str1;
		}

		protected void InitializeLicense()
		{
			if (this._licenseInitializerRun)
			{
				return;
			}
			Assembly assembly = base.GetType().Assembly;
			if (MLLicenseProvider.IsInitialized)
			{
				if (ApplicationData.MainAssembly == assembly)
				{
					return;
				}
				MLLicenseProvider.Dispose();
				ApplicationData.MainAssembly = null;
			}
			ApplicationData.MainAssembly = assembly;
			MLLicenseProvider.Initialize(LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(assembly));
			this._licenseInitializerRun = true;
		}

		private bool PathIsDrive(string sPath)
		{
			return this.GetDriveRelativePath(sPath) == string.Empty;
		}
	}
}