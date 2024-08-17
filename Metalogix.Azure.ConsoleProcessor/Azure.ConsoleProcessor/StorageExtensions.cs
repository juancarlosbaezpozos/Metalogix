using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Azure.ConsoleProcessor
{
	public static class StorageExtensions
	{
		public static bool DeleteAndCreateContainerIfNotExists(this CloudBlobContainer cloudBlobContainer)
		{
			bool flag = false;
			for (int i = 1; i < 50; i++)
			{
				try
				{
					cloudBlobContainer.DeleteIfExists(null, null, null);
					if (!cloudBlobContainer.Exists(null, null))
					{
						cloudBlobContainer.Create(null, null);
					}
					flag = true;
					break;
				}
				catch (StorageException storageException1)
				{
					StorageException storageException = storageException1;
					if (storageException.RequestInformation.HttpStatusCode != 409 || !storageException.RequestInformation.ExtendedErrorInformation.ErrorCode.Equals("ContainerBeingDeleted"))
					{
						throw;
					}
					else
					{
						Thread.Sleep(1000 * i);
					}
				}
			}
			return flag;
		}
	}
}