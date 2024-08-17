using Metalogix.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Metalogix.Azure.ConsoleProcessor
{
	internal class AzureBlobStorageManager : IAzureBlobStorageManager
	{
		private const int MinQueueMessageRequestCount = 1;

		private const int MaxQueueMessageRequestCount = 32;

		private const int MessageInvisibleTimeInMinutes = 10;

		public bool IsEncryptionUsed
		{
			get;
			set;
		}

		public AzureBlobStorageManager()
		{
		}

		private void DecryptFile(string encFile, byte[] encryptionKey, string iv, string decFile)
		{
			using (SymmetricAlgorithm rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Key = encryptionKey;
				rijndaelManaged.IV = Convert.FromBase64String(iv);
				using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(encFile)))
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read))
					{
						using (FileStream fileStream = File.OpenWrite(decFile))
						{
							cryptoStream.CopyTo(fileStream);
						}
					}
				}
			}
		}

		public DownloadResponse DownloadBlob(string containerUri, string blobName, string localFilePath, byte[] encryptionKey)
		{
			DownloadResponse downloadResponse = new DownloadResponse();
			try
			{
				CloudBlockBlob blockBlobReference = (new CloudBlobContainer(new Uri(containerUri))).GetBlockBlobReference(blobName);
				if (!blockBlobReference.Exists(null, null))
				{
					throw new ArgumentException(string.Format("Blob '{0}' does not exist", blobName));
				}
				if (!this.IsEncryptionUsed)
				{
					blockBlobReference.DownloadToFile(localFilePath, FileMode.Create, null, null, null);
				}
				else
				{
					string tempFileName = Path.GetTempFileName();
					try
					{
						blockBlobReference.DownloadToFile(tempFileName, FileMode.Create, null, null, null);
						this.DecryptFile(tempFileName, encryptionKey, blockBlobReference.Metadata["IV"], localFilePath);
					}
					finally
					{
						if (File.Exists(tempFileName))
						{
							File.Delete(tempFileName);
						}
					}
				}
				downloadResponse.Success = true;
				downloadResponse.Details = string.Empty;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				downloadResponse.Success = false;
				downloadResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return downloadResponse;
		}

		private Stream Encrypt(Stream inputStream, byte[] encryptionKey, out byte[] iv)
		{
			MemoryStream memoryStream;
			Stream stream;
			using (SymmetricAlgorithm rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Key = encryptionKey;
				rijndaelManaged.GenerateIV();
				MemoryStream memoryStream1 = null;
				try
				{
					memoryStream1 = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream1, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write);
					byte[] numArray = new byte[4096];
					while (true)
					{
						int num = inputStream.Read(numArray, 0, (int)numArray.Length);
						int num1 = num;
						if (num <= 0)
						{
							break;
						}
						cryptoStream.Write(numArray, 0, num1);
					}
					if (!cryptoStream.HasFlushedFinalBlock)
					{
						cryptoStream.FlushFinalBlock();
					}
					iv = rijndaelManaged.IV;
					memoryStream1.Position = (long)0;
					memoryStream = memoryStream1;
					memoryStream1 = null;
				}
				finally
				{
					if (memoryStream1 != null)
					{
						memoryStream1.Close();
					}
				}
				stream = memoryStream;
			}
			return stream;
		}

		public QueueMessageResponse GetQueueMessage(string queueUri)
		{
			QueueMessageResponse queueMessageResponse = new QueueMessageResponse();
			try
			{
				CloudQueue cloudQueue = new CloudQueue(new Uri(queueUri));
				CloudQueueMessage message = cloudQueue.GetMessage(null, null, null);
				if (message != null)
				{
					queueMessageResponse.Id = message.Id;
					queueMessageResponse.Message = message.AsString;
					cloudQueue.DeleteMessage(message, null, null);
				}
				queueMessageResponse.Success = true;
				if (message == null)
				{
					queueMessageResponse.Details = "Queue is empty";
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				queueMessageResponse.Success = false;
				queueMessageResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return queueMessageResponse;
		}

		public QueueMessagesResponse GetQueueMessages(string queueUri, int numberOfMessagesToGet)
		{
			QueueMessagesResponse queueMessagesResponse = new QueueMessagesResponse();
			if (numberOfMessagesToGet < 1 || numberOfMessagesToGet > 32)
			{
				queueMessagesResponse.Success = false;
				queueMessagesResponse.Details = string.Format("numberOfMessagesToGet specified was {0}. The value must be in a range from {1} to {2}", numberOfMessagesToGet, 1, 32);
				return queueMessagesResponse;
			}
			try
			{
				CloudQueue cloudQueue = new CloudQueue(new Uri(queueUri));
				IEnumerable<CloudQueueMessage> messages = cloudQueue.GetMessages(numberOfMessagesToGet, new TimeSpan?(TimeSpan.FromMinutes(10)), null, null);
				foreach (CloudQueueMessage message in messages)
				{
					List<QueueMessageResponse> queueMessageResponses = queueMessagesResponse.Messages;
					QueueMessageResponse queueMessageResponse = new QueueMessageResponse()
					{
						Id = message.Id,
						Message = message.AsString,
						Details = string.Empty,
						Success = true
					};
					queueMessageResponses.Add(queueMessageResponse);
					cloudQueue.DeleteMessage(message, null, null);
				}
				queueMessagesResponse.Success = true;
				if (messages.Count<CloudQueueMessage>() == 0)
				{
					queueMessagesResponse.Details = "Queue is empty";
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				queueMessagesResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
				queueMessagesResponse.Success = false;
			}
			return queueMessagesResponse;
		}

		public ListResponse ListContainer(string containerUri)
		{
			ListResponse listResponse = new ListResponse();
			try
			{
				CloudBlobContainer cloudBlobContainer = new CloudBlobContainer(new Uri(containerUri));
				foreach (IListBlobItem listBlobItem in cloudBlobContainer.ListBlobs(null, false, BlobListingDetails.None, null, null))
				{
					CloudBlockBlob cloudBlockBlob = listBlobItem as CloudBlockBlob;
					if (cloudBlockBlob == null)
					{
						continue;
					}
					BlobInfo blobInfo = new BlobInfo()
					{
						BlobName = cloudBlockBlob.Name,
						BlobType = cloudBlockBlob.BlobType.ToString(),
						Filesize = cloudBlockBlob.Properties.Length,
						ContentType = cloudBlockBlob.Properties.ContentType
					};
					listResponse.Blobs.Add(blobInfo);
				}
				listResponse.Success = true;
				listResponse.Details = string.Empty;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				listResponse.Success = false;
				listResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return listResponse;
		}

		public UploadResponse UploadBlob(string containerUri, string blobName, string localBlobFilePath, byte[] encryptionKey)
		{
			byte[] numArray;
			UploadResponse uploadResponse = new UploadResponse();
			try
			{
				CloudBlockBlob blockBlobReference = (new CloudBlobContainer(new Uri(containerUri))).GetBlockBlobReference(blobName);
				using (FileStream fileStream = File.OpenRead(localBlobFilePath))
				{
					if (!this.IsEncryptionUsed)
					{
						blockBlobReference.UploadFromStream(fileStream, null, null, null);
					}
					else
					{
						using (Stream stream = this.Encrypt(fileStream, encryptionKey, out numArray))
						{
							blockBlobReference.Metadata.Add("IV", Convert.ToBase64String(numArray));
							blockBlobReference.UploadFromStream(stream, null, null, null);
						}
					}
				}
				blockBlobReference.CreateSnapshot(null, null, null, null);
				uploadResponse.Success = true;
				uploadResponse.Details = string.Empty;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				uploadResponse.Success = false;
				uploadResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return uploadResponse;
		}
	}
}