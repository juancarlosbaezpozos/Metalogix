using Metalogix;
using Metalogix.Connectivity.Proxy;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.Interfaces;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.RestoreWebService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.SharePoint.ExternalConnections
{
	public class StoragePointExternalConnection : ExternalConnection
	{
		public X509CertificateWrapperCollection IncludedCertificates
		{
			get;
			set;
		}

		public MLProxy Proxy
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public StoragePointExternalConnection()
		{
		}

		public override void CheckConnection()
		{
			if (this.IsAlive())
			{
				this.Status = ConnectionStatus.Valid;
			}
			else
			{
				this.Status = ConnectionStatus.Invalid;
			}
		}

		public override void FromXml(XmlNode ndExternalConnection)
		{
			base.FromXml(ndExternalConnection);
			this.Url = ndExternalConnection.Attributes["Url"].Value;
			this.Proxy = new MLProxy(ndExternalConnection.SelectSingleNode(".//Proxy"));
			this.IncludedCertificates = X509CertificateWrapperCollection.BuildCollectionFromXml(ndExternalConnection.SelectSingleNode(".//IncludedCertificates"));
			this.Status = ConnectionStatus.NotChecked;
		}

		public byte[] GetBLOB(byte[] blobReference)
		{
			byte[] bLOB;
			StoragePointRestoreBLOBWebService storagePointRestoreBLOBWebService = new StoragePointRestoreBLOBWebService();
			try
			{
				storagePointRestoreBLOBWebService.Url = this.Url;
				storagePointRestoreBLOBWebService.Credentials = base.Credentials.NetworkCredentials;
				if ((this.Proxy == null ? false : this.Proxy.Enabled))
				{
					storagePointRestoreBLOBWebService.Proxy = this.Proxy.GetWebProxy();
				}
				if ((this.IncludedCertificates == null ? false : this.IncludedCertificates.Count > 0))
				{
					this.IncludedCertificates.CopyCertificatesToCollection(storagePointRestoreBLOBWebService.ClientCertificates);
				}
				bLOB = storagePointRestoreBLOBWebService.GetBLOB(blobReference);
			}
			finally
			{
				if (storagePointRestoreBLOBWebService != null)
				{
					((IDisposable)storagePointRestoreBLOBWebService).Dispose();
				}
			}
			return bLOB;
		}

		public bool IsAlive()
		{
			bool flag;
			CookieAwareWebClient cookieAwareWebClient = new CookieAwareWebClient();
			try
			{
				try
				{
					cookieAwareWebClient.Credentials = base.Credentials.NetworkCredentials;
					if ((this.Proxy == null ? false : this.Proxy.Enabled))
					{
						cookieAwareWebClient.Proxy = this.Proxy.GetWebProxy();
					}
					if ((this.IncludedCertificates == null ? false : this.IncludedCertificates.Count > 0))
					{
						cookieAwareWebClient.ClientCertificates = new X509CertificateCollection();
						this.IncludedCertificates.CopyCertificatesToCollection(cookieAwareWebClient.ClientCertificates);
					}
					ServiceDescription serviceDescription = ServiceDescription.Read(new StreamReader(cookieAwareWebClient.OpenRead(string.Concat(this.Url, "?WSDL"))));
					if (serviceDescription.Messages.Cast<Message>().Any<Message>((Message m) => m.Name.StartsWith("GetBLOB")))
					{
						flag = true;
					}
					else
					{
						GlobalServices.ErrorHandler.HandleException("Unrecognized StoragePoint Web Service", "Error", ErrorIcon.Error);
						flag = false;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					GlobalServices.ErrorHandler.HandleException("Unable to connect to StoragePoint Web Service", exception.Message, exception, ErrorIcon.Error);
					flag = false;
				}
			}
			finally
			{
				if (cookieAwareWebClient != null)
				{
					((IDisposable)cookieAwareWebClient).Dispose();
				}
			}
			return flag;
		}

		protected override void WriteConnectionXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString("Url", this.Url);
			this.Proxy.ToXML(xmlWriter);
			this.IncludedCertificates.ToXml(xmlWriter);
		}
	}
}