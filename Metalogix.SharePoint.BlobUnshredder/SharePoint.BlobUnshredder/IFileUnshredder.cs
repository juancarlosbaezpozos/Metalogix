using System;

namespace Metalogix.SharePoint.BlobUnshredder
{
	public interface IFileUnshredder
	{
		void GetBlobUsingCobaltStream(Guid docId, int uiVersion, byte level, string filePath);
	}
}