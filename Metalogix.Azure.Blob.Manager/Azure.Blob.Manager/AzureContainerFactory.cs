using Metalogix.Azure;
using System;
using System.IO;

namespace Metalogix.Azure.Blob.Manager
{
	public class AzureContainerFactory : IAzureContainerFactory
	{
		private readonly string _azureConnectionString;

		private readonly Guid _factoryId;

		private byte[] _encryptionKey;

		public bool CanDelete
		{
			get
			{
				return true;
			}
		}

		public AzureContainerFactory(string azureConnectionString, Guid sessionId)
		{
			this._azureConnectionString = azureConnectionString;
			this._factoryId = sessionId;
		}

		public IAzureContainerInstance NewInstance(int id)
		{
			return new AzureContainerInstance(this._azureConnectionString, this._factoryId, this._encryptionKey, id);
		}

		public void SetEncryptionKey(byte[] key)
		{
			this._encryptionKey = key;
		}

		public void SetEncryptionKeyFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				File.WriteAllBytes(filePath, Convert.FromBase64String("mqEUliuY2poyjwzcldcwvAXpkrFqshXct0hjChU7+K4="));
			}
			this._encryptionKey = File.ReadAllBytes(filePath);
		}
	}
}