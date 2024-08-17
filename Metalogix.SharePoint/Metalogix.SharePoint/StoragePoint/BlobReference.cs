using System;
using System.Text;

namespace Metalogix.SharePoint.StoragePoint
{
	[Serializable]
	public class BlobReference
	{
		public const string PassThroughSignature1 = "137D2254-9895-4ab5-B9B0-EAEFDC186820";

		public const string PassThroughSignature2 = "852C3799-6779-45ab-B585-79814BF72CD0";

		private Guid _profileId;

		private Guid _blobId;

		private short _flags = 0;

		private long _blobSize = (long)-1;

		private EndPointRef _endPointReference = null;

		private Guid _uniqueIdentifier = Guid.Empty;

		public Guid BlobId
		{
			get
			{
				return this._blobId;
			}
			set
			{
				this._blobId = value;
			}
		}

		public long BlobSize
		{
			get
			{
				return this._blobSize;
			}
			set
			{
				this._blobSize = value;
			}
		}

		public EndPointRef EndPointReference
		{
			get
			{
				if (this._endPointReference == null)
				{
					this._endPointReference = new EndPointRef();
				}
				return this._endPointReference;
			}
			set
			{
				this._endPointReference = value;
			}
		}

		public short Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				this._flags = value;
			}
		}

		public Guid ProfileId
		{
			get
			{
				return this._profileId;
			}
			set
			{
				this._profileId = value;
			}
		}

		public Guid UniqueIndentifier
		{
			get
			{
				return this._uniqueIdentifier;
			}
			set
			{
				this._uniqueIdentifier = value;
			}
		}

		public BlobReference()
		{
		}

		public BlobReference(byte[] blobRef)
		{
			byte[] numArray;
			int num;
			int num1;
			string str;
			string[] strArrays;
			char[] chrArray;
			this._endPointReference = new EndPointRef();
			byte[] numArray1 = new byte[16];
			Array.Copy(blobRef, 0, numArray1, 0, 16);
			this._profileId = new Guid(numArray1);
			byte[] numArray2 = new byte[16];
			Array.Copy(blobRef, 16, numArray2, 0, 16);
			this._blobId = new Guid(numArray2);
			this._endPointReference._blobId = this._blobId;
			if ((int)blobRef.Length <= 32)
			{
				this._endPointReference._folderSysGenerated = null;
				this._endPointReference.FilenameOverride = null;
				this._endPointReference.Flags = 0;
				this._endPointReference.EndPointId = this._profileId;
			}
			else
			{
				this._blobSize = (long)-1;
				if (blobRef[32] == 255)
				{
					this._flags = BitConverter.ToInt16(blobRef, 33);
					this._blobSize = BitConverter.ToInt64(blobRef, 35);
					this._endPointReference._folderSysGenerated = Encoding.UTF8.GetString(blobRef, 43, (int)blobRef.Length - 43);
					this._endPointReference.FilenameOverride = null;
					this._endPointReference.Flags = 0;
					this._endPointReference.EndPointId = this._profileId;
				}
				else if (blobRef[32] == 254)
				{
					this._flags = BitConverter.ToInt16(blobRef, 49);
					this._blobSize = BitConverter.ToInt64(blobRef, 51);
					this._endPointReference.Flags = BitConverter.ToInt16(blobRef, 59);
					numArray = new byte[16];
					Array.Copy(blobRef, 33, numArray, 0, 16);
					this._endPointReference.EndPointId = new Guid(numArray);
					num = -1;
					num1 = 61;
					while (num1 < (int)blobRef.Length)
					{
						if (blobRef[num1] != 0)
						{
							num1++;
						}
						else
						{
							num = num1;
							break;
						}
					}
					this._endPointReference._folderSysGenerated = null;
					this._endPointReference._folderUserOverride = null;
					if (num != -1)
					{
						str = Encoding.UTF8.GetString(blobRef, 61, num - 61);
						if (!string.IsNullOrEmpty(str))
						{
							chrArray = new char[] { '*' };
							strArrays = str.Split(chrArray);
							if ((int)strArrays.Length != 1)
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
								this._endPointReference._folderUserOverride = strArrays[1];
							}
							else
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
							}
						}
						this._endPointReference.FilenameOverride = Encoding.UTF8.GetString(blobRef, num + 1, (int)blobRef.Length - num - 1);
					}
					else
					{
						str = Encoding.UTF8.GetString(blobRef, 61, (int)blobRef.Length - 61);
						if (!string.IsNullOrEmpty(str))
						{
							chrArray = new char[] { '*' };
							strArrays = str.Split(chrArray);
							if ((int)strArrays.Length != 1)
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
								this._endPointReference._folderUserOverride = strArrays[1];
							}
							else
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
							}
						}
						this._endPointReference.FilenameOverride = null;
					}
				}
				else if (blobRef[32] != 253)
				{
					this._endPointReference._folderSysGenerated = Encoding.UTF8.GetString(blobRef, 32, (int)blobRef.Length - 32);
					this._endPointReference.FilenameOverride = null;
					this._endPointReference.Flags = 0;
					this._endPointReference.EndPointId = this._profileId;
				}
				else
				{
					this._flags = BitConverter.ToInt16(blobRef, 49);
					this._blobSize = BitConverter.ToInt64(blobRef, 51);
					this._endPointReference.Flags = BitConverter.ToInt16(blobRef, 59);
					numArray = new byte[16];
					Array.Copy(blobRef, 33, numArray, 0, 16);
					this._endPointReference.EndPointId = new Guid(numArray);
					int length = (int)blobRef.Length;
					byte[] numArray3 = new byte[16];
					Array.Copy(blobRef, length - 16, numArray3, 0, 16);
					this.UniqueIndentifier = new Guid(numArray3);
					length -= 16;
					num = -1;
					num1 = 61;
					while (num1 < length)
					{
						if (blobRef[num1] != 0)
						{
							num1++;
						}
						else
						{
							num = num1;
							break;
						}
					}
					this._endPointReference._folderSysGenerated = null;
					this._endPointReference._folderUserOverride = null;
					if (num != -1)
					{
						str = Encoding.UTF8.GetString(blobRef, 61, num - 61);
						if (!string.IsNullOrEmpty(str))
						{
							chrArray = new char[] { '*' };
							strArrays = str.Split(chrArray);
							if ((int)strArrays.Length != 1)
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
								this._endPointReference._folderUserOverride = strArrays[1];
							}
							else
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
							}
						}
						this._endPointReference.FilenameOverride = Encoding.UTF8.GetString(blobRef, num + 1, length - num - 1);
					}
					else
					{
						str = Encoding.UTF8.GetString(blobRef, 61, length - 61);
						if (!string.IsNullOrEmpty(str))
						{
							chrArray = new char[] { '*' };
							strArrays = str.Split(chrArray);
							if ((int)strArrays.Length != 1)
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
								this._endPointReference._folderUserOverride = strArrays[1];
							}
							else
							{
								this._endPointReference._folderSysGenerated = strArrays[0];
							}
						}
						this._endPointReference.FilenameOverride = null;
					}
				}
			}
		}

		public BlobReference(string blobRef)
		{
			this._endPointReference = new EndPointRef();
			char[] chrArray = new char[] { '|' };
			string[] strArrays = blobRef.Split(chrArray);
			this._profileId = new Guid(strArrays[0]);
			this._blobId = new Guid(strArrays[1]);
			if (!string.IsNullOrEmpty(strArrays[2]))
			{
				string str = strArrays[2];
				chrArray = new char[] { '*' };
				string[] strArrays1 = str.Split(chrArray);
				if ((int)strArrays1.Length == 1)
				{
					this._endPointReference._folderSysGenerated = strArrays1[0];
				}
				else if ((int)strArrays1.Length == 2)
				{
					this._endPointReference._folderSysGenerated = strArrays1[0];
					this._endPointReference._folderUserOverride = strArrays1[1];
				}
			}
			this._blobSize = (long)-1;
			this._endPointReference.EndPointId = this._profileId;
			this._endPointReference.Flags = 0;
			this._endPointReference.FilenameOverride = null;
			this._endPointReference._blobId = this._blobId;
			if ((int)strArrays.Length > 3)
			{
				this._blobSize = Convert.ToInt64(strArrays[3]);
				this._flags = Convert.ToInt16(strArrays[4]);
			}
			if ((int)strArrays.Length >= 8)
			{
				this._endPointReference.EndPointId = new Guid(strArrays[5]);
				if (!string.IsNullOrEmpty(strArrays[6]))
				{
					this._endPointReference.FilenameOverride = strArrays[6];
				}
				this._endPointReference.Flags = Convert.ToInt16(strArrays[7]);
			}
			if ((int)strArrays.Length == 9)
			{
				this.UniqueIndentifier = new Guid(strArrays[8]);
			}
		}

		public byte[] GetBytes()
		{
			if (this._endPointReference == null)
			{
				throw new Exception("Blob reference must have an endpoint defined.");
			}
			byte[] bytes = new byte[0];
			if (!string.IsNullOrEmpty(this._endPointReference.Folder))
			{
				string str = this._endPointReference._folderSysGenerated;
				if (!string.IsNullOrEmpty(this._endPointReference._folderUserOverride))
				{
					if (str == null)
					{
						str = string.Concat(str, string.Empty);
					}
					str = string.Concat(str, "*", this._endPointReference._folderUserOverride);
				}
				bytes = Encoding.UTF8.GetBytes(str);
			}
			byte[] numArray = null;
			if (!string.IsNullOrEmpty(this._endPointReference.FilenameOverride))
			{
				numArray = Encoding.UTF8.GetBytes(this._endPointReference._filenameOverride);
			}
			int length = 61;
			if (!string.IsNullOrEmpty(this._endPointReference.Folder))
			{
				length += (int)bytes.Length;
			}
			if (!string.IsNullOrEmpty(this._endPointReference.FilenameOverride))
			{
				length = length + (int)numArray.Length + 1;
			}
			if (this.UniqueIndentifier != Guid.Empty)
			{
				length += 16;
			}
			byte[] numArray1 = new byte[length];
			this._profileId.ToByteArray().CopyTo(numArray1, 0);
			this._blobId.ToByteArray().CopyTo(numArray1, 16);
			numArray1[32] = 254;
			this._endPointReference.EndPointId.ToByteArray().CopyTo(numArray1, 33);
			BitConverter.GetBytes(this._flags).CopyTo(numArray1, 49);
			BitConverter.GetBytes(this._blobSize).CopyTo(numArray1, 51);
			BitConverter.GetBytes(this._endPointReference.Flags).CopyTo(numArray1, 59);
			if (!string.IsNullOrEmpty(this._endPointReference.Folder))
			{
				bytes.CopyTo(numArray1, 61);
			}
			if (!string.IsNullOrEmpty(this._endPointReference.FilenameOverride))
			{
				numArray1[61 + (int)bytes.Length] = 0;
				numArray.CopyTo(numArray1, 61 + (int)bytes.Length + 1);
			}
			if (this.UniqueIndentifier != Guid.Empty)
			{
				numArray1[32] = 253;
				Guid uniqueIndentifier = this.UniqueIndentifier;
				uniqueIndentifier.ToByteArray().CopyTo(numArray1, length - 16);
			}
			return numArray1;
		}

		public byte[] GetBytesPassthrough()
		{
			byte[] numArray = null;
			byte[] bytes = this.GetBytes();
			if (bytes != null)
			{
				numArray = new byte[(int)bytes.Length + 34];
				Guid guid = new Guid("137D2254-9895-4ab5-B9B0-EAEFDC186820");
				guid.ToByteArray().CopyTo(numArray, 0);
				guid = new Guid("852C3799-6779-45ab-B585-79814BF72CD0");
				guid.ToByteArray().CopyTo(numArray, 16);
				short length = (short)((int)bytes.Length);
				BitConverter.GetBytes(length).CopyTo(numArray, 32);
				bytes.CopyTo(numArray, 34);
			}
			return numArray;
		}

		public string GetString()
		{
			string str = null;
			if (!string.IsNullOrEmpty(this._endPointReference.Folder))
			{
				str = this._endPointReference._folderSysGenerated;
				if (!string.IsNullOrEmpty(this._endPointReference._folderUserOverride))
				{
					if (str == null)
					{
						str = string.Concat(str, string.Empty);
					}
					str = string.Concat(str, "*", this._endPointReference._folderUserOverride);
				}
			}
			object[] objArray = new object[] { this._profileId.ToString(), "|", this._blobId.ToString(), "|", null, null, null, null, null };
			objArray[4] = (str == null ? "" : str);
			objArray[5] = "|";
			objArray[6] = this._blobSize;
			objArray[7] = "|";
			objArray[8] = this._flags;
			string str1 = string.Concat(objArray);
			string[] strArrays = new string[] { str1, "|", this._endPointReference.EndPointId.ToString(), "|", null };
			strArrays[4] = (this._endPointReference._filenameOverride == null ? "" : this._endPointReference._filenameOverride);
			str1 = string.Concat(strArrays);
			str1 = string.Concat(str1, "|", this._endPointReference.Flags.ToString());
			if (this.UniqueIndentifier != Guid.Empty)
			{
				Guid uniqueIndentifier = this.UniqueIndentifier;
				str1 = string.Concat(str1, "|", uniqueIndentifier.ToString());
			}
			return str1;
		}

		public static bool IsBlobReference(byte[] blobRef)
		{
			bool flag;
			try
			{
				if ((int)blobRef.Length > 32)
				{
					byte[] numArray = new byte[16];
					Array.Copy(blobRef, 0, numArray, 0, 16);
					Guid guid = new Guid(numArray);
					if (guid == new Guid("137D2254-9895-4ab5-B9B0-EAEFDC186820"))
					{
						Array.Copy(blobRef, 16, numArray, 0, 16);
						guid = new Guid(numArray);
						if (guid == new Guid("852C3799-6779-45ab-B585-79814BF72CD0"))
						{
							flag = true;
							return flag;
						}
					}
				}
			}
			catch (Exception exception)
			{
			}
			flag = false;
			return flag;
		}
	}
}