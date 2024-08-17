using Metalogix.SharePoint.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.RestoreWebService
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("System.Web.Services", "4.0.30319.17929")]
	[WebServiceBinding(Name="StoragePointRestoreBLOBWebServiceSoap", Namespace="http://www.metalogix.net/")]
	public class StoragePointRestoreBLOBWebService : SoapHttpClientProtocol
	{
		private SendOrPostCallback GetBLOBOperationCompleted;

		private bool useDefaultCredentialsSetExplicitly;

		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				if ((!this.IsLocalFileSystemWebService(base.Url) || this.useDefaultCredentialsSetExplicitly ? false : !this.IsLocalFileSystemWebService(value)))
				{
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}

		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		public StoragePointRestoreBLOBWebService()
		{
			this.Url = Metalogix.SharePoint.Properties.Settings.Default.Metalogix_SharePoint_RestoreWebService_StoragePointRestoreBLOBWebService;
			if (!this.IsLocalFileSystemWebService(this.Url))
			{
				this.useDefaultCredentialsSetExplicitly = true;
			}
			else
			{
				this.UseDefaultCredentials = true;
				this.useDefaultCredentialsSetExplicitly = false;
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		[SoapDocumentMethod("http://www.metalogix.net/GetBLOB", RequestNamespace="http://www.metalogix.net/", ResponseNamespace="http://www.metalogix.net/", Use=SoapBindingUse.Literal, ParameterStyle=SoapParameterStyle.Wrapped)]
		[return: XmlElement(DataType="base64Binary")]
		public byte[] GetBLOB([XmlElement(DataType="base64Binary")] byte[] BlobReference)
		{
			object[] blobReference = new object[] { BlobReference };
			return (byte[])base.Invoke("GetBLOB", blobReference)[0];
		}

		public void GetBLOBAsync(byte[] BlobReference)
		{
			this.GetBLOBAsync(BlobReference, null);
		}

		public void GetBLOBAsync(byte[] BlobReference, object userState)
		{
			if (this.GetBLOBOperationCompleted == null)
			{
				this.GetBLOBOperationCompleted = new SendOrPostCallback(this.OnGetBLOBOperationCompleted);
			}
			object[] blobReference = new object[] { BlobReference };
			base.InvokeAsync("GetBLOB", blobReference, this.GetBLOBOperationCompleted, userState);
		}

		private bool IsLocalFileSystemWebService(string url)
		{
			bool flag;
			if ((url == null ? false : !(url == string.Empty)))
			{
				System.Uri uri = new System.Uri(url);
				flag = ((uri.Port < 1024 ? true : string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) != 0) ? false : true);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private void OnGetBLOBOperationCompleted(object arg)
		{
			if (this.GetBLOBCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
				this.GetBLOBCompleted(this, new GetBLOBCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
			}
		}

		public event GetBLOBCompletedEventHandler GetBLOBCompleted;
	}
}