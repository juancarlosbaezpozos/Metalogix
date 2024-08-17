using Metalogix.SharePoint.Adapters;
using System;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client
{
    public class BindingProperties
    {
        public static NetNamedPipeBinding ManagerBindingData;

        public static NetNamedPipeBinding ServiceBindingData;

        public readonly static EndpointAddress ManagerAddress;

        public readonly static EndpointAddress CSOMServiceAddress;

        static BindingProperties()
        {
            NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding()
            {
                CloseTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeCloseTimeout),
                OpenTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeOpenTimeout),
                ReceiveTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeReceiveTimeout),
                SendTimeout = TimeSpan.FromMinutes(AdapterConfigurationVariables.PipeSendTimeout),
                TransactionFlow = false,
                TransferMode = TransferMode.Buffered,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxBufferPoolSize = (long)524288,
                MaxBufferSize = 65536,
                MaxConnections = 10,
                MaxReceivedMessageSize = (long)65536
            };
            BindingProperties.ManagerBindingData = netNamedPipeBinding;
            NetNamedPipeBinding netNamedPipeBinding1 = new NetNamedPipeBinding()
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
            netNamedPipeBinding1.ReaderQuotas = xmlDictionaryReaderQuota;
            BindingProperties.ServiceBindingData = netNamedPipeBinding1;
            string str = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            int hashCode = WindowsIdentity.GetCurrent().User.GetHashCode();
            string str1 = hashCode.ToString();
            string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(str1));
            BindingProperties.ManagerAddress =
                new EndpointAddress(string.Format("net.pipe://localhost/CSOM2013ServiceManager{0}{1}", str,
                    base64String));
            BindingProperties.CSOMServiceAddress =
                new EndpointAddress(string.Format("net.pipe://localhost/CSOM2013Service{0}{1}", str, base64String));
        }

        public BindingProperties()
        {
        }
    }
}