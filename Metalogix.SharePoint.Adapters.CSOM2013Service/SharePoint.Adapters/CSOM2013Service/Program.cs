using Metalogix;
using Metalogix.Core;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.CSOM2013Service
{
	internal static class Program
	{
		private static bool PublishMetadata;

		static Program()
		{
		}

		[STAThread]
		private static void Main(string[] args)
		{
			int hashCode = WindowsIdentity.GetCurrent().User.GetHashCode();
			string str = hashCode.ToString();
			string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
			string str1 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Uri uri = new Uri(string.Format("net.pipe://localhost/CSOM2013Service{0}{1}", str1, base64String));
			string str2 = Path.Combine(ApplicationData.CommonDataPath, string.Concat(ConfigurationVariables.EnvironmentVariables.Name, ".xml"));
			if (File.Exists(str2))
			{
				AdapterConfigurationVariables.LoadConfigurationVariables(File.ReadAllText(str2));
			}
			NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding()
			{
				CloseTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeCloseTimeout),
				OpenTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeOpenTimeout),
				ReceiveTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeReceiveTimeout),
				SendTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeSendTimeout),
				TransactionFlow = false,
				TransferMode = TransferMode.Buffered,
				HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
				MaxBufferPoolSize = (long)2147483647,
				MaxBufferSize = 2147483647,
				MaxConnections = 1024,
				MaxReceivedMessageSize = (long)2147483647
			};
			XmlDictionaryReaderQuotas xmlDictionaryReaderQuota = new XmlDictionaryReaderQuotas()
			{
				MaxArrayLength = 2147483647,
				MaxStringContentLength = 2147483647,
				MaxBytesPerRead = 2147483647,
				MaxNameTableCharCount = 1024,
				MaxDepth = 1024
			};
			netNamedPipeBinding.ReaderQuotas = xmlDictionaryReaderQuota;
			Uri uri1 = new Uri(string.Format("net.pipe://localhost/CSOM2013ServiceManager{0}{1}", str1, base64String));
			NetNamedPipeBinding netNamedPipeBinding1 = new NetNamedPipeBinding()
			{
				CloseTimeout = TimeSpan.FromMinutes(5),
				OpenTimeout = TimeSpan.FromMinutes(5),
				ReceiveTimeout = TimeSpan.FromMinutes(10),
				SendTimeout = TimeSpan.FromMinutes(5),
				TransactionFlow = false,
				TransferMode = TransferMode.Buffered,
				HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
				MaxBufferPoolSize = (long)524288,
				MaxBufferSize = 65536,
				MaxConnections = 10,
				MaxReceivedMessageSize = (long)65536
			};
			try
			{
				using (ServiceHost serviceHost = new ServiceHost(typeof(Metalogix.SharePoint.Adapters.CSOM2013Service.CSOM2013Service), new Uri[] { uri }))
				{
					serviceHost.Description.Behaviors.Add(new DispatchBehaviour());
					ServiceEndpoint serviceEndpoint = serviceHost.AddServiceEndpoint(typeof(ISharePointAdapterService), netNamedPipeBinding, uri);
					serviceEndpoint.Behaviors.Add(new DispatchBehaviour());
					if (Program.PublishMetadata)
					{
						ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior()
						{
							HttpGetEnabled = true,
							HttpGetUrl = new Uri("http://localhost:8016/CSOM2013Service")
						};
						serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);
					}
					serviceHost.Open();
					using (ServiceHost serviceHost1 = new ServiceHost(typeof(ServiceManager), new Uri[] { uri1 }))
					{
						if (Program.PublishMetadata)
						{
							ServiceMetadataBehavior serviceMetadataBehavior1 = new ServiceMetadataBehavior()
							{
								HttpGetEnabled = true,
								HttpGetUrl = new Uri("http://localhost:8016/CSOM2013ServiceManager")
							};
							serviceHost1.Description.Behaviors.Add(serviceMetadataBehavior1);
						}
						serviceHost1.Open();
						int num = 0;
						bool flag = false;
						if ((int)args.Length > 0)
						{
							flag = int.TryParse(args[0], out num);
						}
						if (!flag)
						{
							TickingClock.StartClock();
						}
						else
						{
							TickingClock.StartClock(num * 1000);
						}
						Application.Run();
						serviceHost1.Close();
					}
					serviceHost.Close();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Utils.LogExceptionDetails(exception, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name, null);
			}
		}
	}
}