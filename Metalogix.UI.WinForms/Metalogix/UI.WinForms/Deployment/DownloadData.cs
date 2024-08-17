using System;
using System.IO;
using System.Net;

namespace Metalogix.UI.WinForms.Deployment
{
	internal class DownloadData
	{
		private WebResponse response;

		private Stream stream;

		private long size;

		private long start;

		private IWebProxy proxy;

		public Stream DownloadStream
		{
			get
			{
				if (this.start == this.size)
				{
					return Stream.Null;
				}
				if (this.stream == null)
				{
					this.stream = this.response.GetResponseStream();
				}
				return this.stream;
			}
		}

		public long FileSize
		{
			get
			{
				return this.size;
			}
		}

		public bool IsProgressKnown
		{
			get
			{
				return this.size > (long)-1;
			}
		}

		public WebResponse Response
		{
			get
			{
				return this.response;
			}
			set
			{
				this.response = value;
			}
		}

		public long StartPoint
		{
			get
			{
				return this.start;
			}
		}

		private DownloadData()
		{
		}

		private DownloadData(WebResponse response, long size, long start)
		{
			this.response = response;
			this.size = size;
			this.start = start;
			this.stream = null;
		}

		public void Close()
		{
			this.response.Close();
		}

		public static DownloadData Create(string url, string destFolder, string destFileName)
		{
			return DownloadData.Create(url, destFolder, null);
		}

		public static DownloadData Create(string url, string destFolder, string destFileName, IWebProxy proxy)
		{
			DownloadData downloadDatum = new DownloadData()
			{
				proxy = proxy
			};
			long fileSize = downloadDatum.GetFileSize(url);
			downloadDatum.size = fileSize;
			WebRequest request = downloadDatum.GetRequest(url);
			try
			{
				downloadDatum.response = request.GetResponse();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new ArgumentException(string.Format("Error downloading \"{0}\": {1}", url, exception.Message), exception);
			}
			DownloadData.ValidateResponse(downloadDatum.response, url);
			if (string.IsNullOrEmpty(destFileName))
			{
				destFileName = Path.GetFileName(downloadDatum.response.ResponseUri.ToString());
			}
			string str = Path.Combine(destFolder, destFileName);
			File.Delete(str);
			if (downloadDatum.IsProgressKnown && File.Exists(str))
			{
				if (downloadDatum.Response is HttpWebResponse)
				{
					downloadDatum.start = (new FileInfo(str)).Length;
					if (downloadDatum.start > fileSize)
					{
						File.Delete(str);
					}
					else if (downloadDatum.start < fileSize)
					{
						downloadDatum.response.Close();
						request = downloadDatum.GetRequest(url);
						((HttpWebRequest)request).AddRange((int)downloadDatum.start);
						downloadDatum.response = request.GetResponse();
						if (((HttpWebResponse)downloadDatum.Response).StatusCode != HttpStatusCode.PartialContent)
						{
							File.Delete(str);
							downloadDatum.start = (long)0;
						}
					}
				}
				else
				{
					File.Delete(str);
				}
			}
			return downloadDatum;
		}

		private long GetFileSize(string url)
		{
			WebResponse response = null;
			long contentLength = (long)-1;
			try
			{
				response = this.GetRequest(url).GetResponse();
				contentLength = response.ContentLength;
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}
			return contentLength;
		}

		private WebRequest GetRequest(string url)
		{
			WebRequest defaultCredentials = WebRequest.Create(url);
			if (defaultCredentials is HttpWebRequest)
			{
				defaultCredentials.Credentials = CredentialCache.DefaultCredentials;
				if (defaultCredentials.Proxy != null)
				{
					defaultCredentials.Proxy.GetProxy(new Uri("http://www.google.com"));
				}
			}
			if (this.proxy != null)
			{
				defaultCredentials.Proxy = this.proxy;
			}
			return defaultCredentials;
		}

		private static void ValidateResponse(WebResponse response, string url)
		{
			if (response is HttpWebResponse)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)response;
				if (httpWebResponse.ContentType.Contains("text/html") || httpWebResponse.StatusCode == HttpStatusCode.NotFound)
				{
					throw new ArgumentException(string.Format("Could not download \"{0}\" - a web page was returned from the web server.", url));
				}
			}
			else if (response is FtpWebResponse && ((FtpWebResponse)response).StatusCode == FtpStatusCode.ConnectionClosed)
			{
				throw new ArgumentException(string.Format("Could not download \"{0}\" - FTP server closed the connection.", url));
			}
		}
	}
}