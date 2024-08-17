using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Metalogix.UI.WinForms.Deployment
{
	public class FileDownloader
	{
		private const int downloadBlockSize = 51200;

		private bool canceled;

		private string downloadingTo;

		private IWebProxy proxy;

		public string DownloadingTo
		{
			get
			{
				return this.downloadingTo;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.proxy = value;
			}
		}

		public FileDownloader()
		{
		}

		public void AsyncDownload(string url)
		{
			WaitCallback waitCallback = new WaitCallback(this.WaitCallbackMethod);
			string[] strArrays = new string[] { url, "" };
			ThreadPool.QueueUserWorkItem(waitCallback, strArrays);
		}

		public void AsyncDownload(string url, string destFolder, string destFileName)
		{
			WaitCallback waitCallback = new WaitCallback(this.WaitCallbackMethod);
			string[] strArrays = new string[] { url, destFolder, destFileName };
			ThreadPool.QueueUserWorkItem(waitCallback, strArrays);
		}

		public void AsyncDownload(List<string> urlList, string destFolder, string destFileName)
		{
			WaitCallback waitCallback = new WaitCallback(this.WaitCallbackMethod);
			object[] objArray = new object[] { urlList, destFolder, destFileName };
			ThreadPool.QueueUserWorkItem(waitCallback, objArray);
		}

		public void AsyncDownload(List<string> urlList)
		{
			WaitCallback waitCallback = new WaitCallback(this.WaitCallbackMethod);
			object[] objArray = new object[] { urlList, "" };
			ThreadPool.QueueUserWorkItem(waitCallback, objArray);
		}

		public void Cancel()
		{
			this.canceled = true;
			if (!string.IsNullOrEmpty(this.downloadingTo) && File.Exists(this.downloadingTo))
			{
				int num = 0;
				while (num < 5)
				{
					try
					{
						File.Delete(this.downloadingTo);
						break;
					}
					catch
					{
						num++;
						Thread.Sleep(1000);
					}
				}
			}
		}

		public void Download(string url)
		{
			this.Download(url, "", "");
		}

		public void Download(string url, string destFolder, string destFileName)
		{
			DownloadData downloadDatum = null;
			this.canceled = false;
			try
			{
				try
				{
					downloadDatum = DownloadData.Create(url, destFolder, destFileName, this.proxy);
					if (string.IsNullOrEmpty(destFileName))
					{
						destFileName = Path.GetFileName(downloadDatum.Response.ResponseUri.ToString());
					}
					destFolder = destFolder.Replace("file:///", "").Replace("file://", "");
					this.downloadingTo = Path.Combine(destFolder, destFileName);
					if (!File.Exists(this.downloadingTo))
					{
						File.Create(this.downloadingTo).Close();
					}
					byte[] numArray = new byte[51200];
					long startPoint = downloadDatum.StartPoint;
					bool flag = false;
					while (true)
					{
						int num = downloadDatum.DownloadStream.Read(numArray, 0, 51200);
						int num1 = num;
						if (num <= 0)
						{
							break;
						}
						if (!this.canceled)
						{
							startPoint += (long)num1;
							this.SaveToFile(numArray, num1, this.DownloadingTo);
							if (downloadDatum.IsProgressKnown)
							{
								this.RaiseProgressChanged(startPoint, downloadDatum.FileSize);
							}
							if (this.canceled)
							{
								flag = true;
								downloadDatum.Close();
								break;
							}
						}
						else
						{
							flag = true;
							downloadDatum.Close();
							break;
						}
					}
					if (!flag)
					{
						this.OnDownloadComplete();
					}
				}
				catch (UriFormatException uriFormatException)
				{
					throw new ArgumentException(string.Format("Could not parse the URL \"{0}\" - it's either malformed or is an unknown protocol.", url), uriFormatException);
				}
			}
			finally
			{
				if (downloadDatum != null)
				{
					downloadDatum.Close();
				}
			}
		}

		public void Download(List<string> urlList)
		{
			this.Download(urlList, "", "");
		}

		public void Download(List<string> urlList, string destFolder, string destFileName)
		{
			if (urlList == null)
			{
				throw new ArgumentException("Url list not specified.");
			}
			if (urlList.Count == 0)
			{
				throw new ArgumentException("Url list empty.");
			}
			Exception exception = null;
			List<string>.Enumerator enumerator = urlList.GetEnumerator();
			try
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					string current = enumerator.Current;
					exception = null;
					try
					{
						this.Download(current, destFolder, destFileName);
					}
					catch (Exception exception1)
					{
						exception = exception1;
					}
				}
				while (exception != null);
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (exception != null)
			{
				throw exception;
			}
		}

		private void OnDownloadComplete()
		{
			if (this.DownloadComplete != null)
			{
				this.DownloadComplete(this, new EventArgs());
			}
		}

		private void RaiseProgressChanged(long current, long target)
		{
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(this, new DownloadEventArgs(target, current));
			}
		}

		private void SaveToFile(byte[] buffer, int count, string fileName)
		{
			FileStream fileStream = null;
			try
			{
				try
				{
					fileStream = File.Open(fileName, FileMode.Append, FileAccess.Write);
					fileStream.Write(buffer, 0, count);
				}
				catch (ArgumentException argumentException1)
				{
					ArgumentException argumentException = argumentException1;
					throw new ArgumentException(string.Format("Error trying to save file \"{0}\": {1}", fileName, argumentException.Message), argumentException);
				}
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}

		private void WaitCallbackMethod(object data)
		{
			if (data is string[])
			{
				string[] strArrays = data as string[];
				this.Download(strArrays[0], strArrays[1], strArrays[2]);
				return;
			}
			object[] objArray = data as object[];
			List<string> strs = objArray[0] as List<string>;
			string str = objArray[1] as string;
			this.Download(strs, str, objArray[2] as string);
		}

		public event EventHandler DownloadComplete;

		public event DownloadProgressHandler ProgressChanged;
	}
}