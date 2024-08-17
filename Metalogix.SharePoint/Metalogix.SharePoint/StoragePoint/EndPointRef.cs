using System;

namespace Metalogix.SharePoint.StoragePoint
{
	[Serializable]
	public class EndPointRef
	{
		public const short endpointRefFlagFilenameOverrideAppendBlobId = 1;

		public const short endpointRefFlagFilenameOverrideAppendDotBlobExtension = 2;

		public const short endpointRefFlagLibrarianSourceLink = 4;

		public Guid EndPointId;

		internal string _folderSysGenerated;

		internal string _folderUserOverride;

		internal Guid _blobId;

		internal string _filenameOverride;

		public short Flags;

		public string FileName
		{
			get
			{
				string str;
				string filenameOverride = this.FilenameOverride;
				str = (!string.IsNullOrEmpty(filenameOverride) ? filenameOverride : this._blobId.ToString());
				return str;
			}
		}

		public string FilenameOverride
		{
			get
			{
				string str;
				string str1 = null;
				if (this._filenameOverride != null)
				{
					str1 = ((this.Flags & 1) != 0 ? EndPointRef.formFilenameOverride(this._filenameOverride, this._blobId) : this._filenameOverride);
					if ((this.Flags & 2) != 0)
					{
						str1 = string.Concat(str1, ".blob");
					}
					str = str1;
				}
				else
				{
					str = null;
				}
				return str;
			}
			set
			{
				this._filenameOverride = value;
			}
		}

		public string FilenameOverrideBase
		{
			get
			{
				return this._filenameOverride;
			}
		}

		public string Folder
		{
			get
			{
				string empty = this._folderSysGenerated;
				if (!string.IsNullOrEmpty(this._folderUserOverride))
				{
					if (string.IsNullOrEmpty(empty))
					{
						empty = string.Empty;
					}
					else if ((empty.EndsWith("\\") ? false : !this._folderUserOverride.StartsWith("\\")))
					{
						empty = string.Concat(empty, "\\");
					}
					empty = string.Concat(empty, this._folderUserOverride);
				}
				return empty;
			}
		}

		public string FolderSystem
		{
			get
			{
				return this._folderSysGenerated;
			}
			set
			{
				this._folderSysGenerated = value;
			}
		}

		public string FolderUserOverride
		{
			get
			{
				return this._folderUserOverride;
			}
			set
			{
				this._folderUserOverride = value;
			}
		}

		public EndPointRef()
		{
		}

		internal static string formFilenameOverride(string filenameOverride, Guid blobId)
		{
			string str;
			int num = filenameOverride.LastIndexOf(".");
			if (num >= 0)
			{
				str = (num != 0 ? string.Concat(filenameOverride.Substring(0, num), "__", blobId.ToString(), filenameOverride.Substring(num)) : string.Concat(blobId.ToString(), filenameOverride.Substring(num)));
			}
			else
			{
				str = string.Concat(filenameOverride, blobId.ToString());
			}
			return str;
		}
	}
}