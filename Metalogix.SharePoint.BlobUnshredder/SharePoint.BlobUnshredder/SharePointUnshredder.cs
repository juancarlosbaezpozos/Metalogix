using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.SharePoint.BlobUnshredder
{
	public class SharePointUnshredder : IFileUnshredder
	{
		public const int UNSHREDDED_STREAM_SCHEMA = 0;

		public int ChunkSize
		{
			get;
			set;
		}

		public string ConnectionString
		{
			get;
			set;
		}

		public SharePointUnshredder(string connectionString, int chunkSize)
		{
			this.ConnectionString = connectionString;
			this.ChunkSize = chunkSize;
		}

		private Assembly EnsureCobaltIsAvailable()
		{
			Assembly assembly = Assembly.Load("Microsoft.CobaltCore, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
			if (assembly == null)
			{
				throw new Exception("Unable to load Microsoft.CobaltCore assembly.");
			}
			return assembly;
		}

		private Assembly EnsureSharePointIsAvailable()
		{
			Assembly assembly = Assembly.Load("Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
			if (assembly == null)
			{
				throw new Exception("Unable to load Microsoft.SharePoint assembly.");
			}
			return assembly;
		}

		public void GetBlobUsingCobaltStream(Guid docId, int uiVersion, byte level, string filePath)
		{
			if (uiVersion == 0)
			{
				this.GetDocumentBinary(docId, level, filePath);
				return;
			}
			this.GetVersionBinary(docId, uiVersion, filePath);
		}

		private Guid GetDatabaseId()
		{
			Guid guid;
			using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
			{
				sqlConnection.Open();
				string str = (new StringBuilder()).Append("SELECT Value ").Append("FROM DatabaseInformation ").Append("WHERE Name='Id';").ToString();
				using (SqlCommand sqlCommand = new SqlCommand(str, sqlConnection))
				{
					guid = new Guid((string)sqlCommand.ExecuteScalar());
				}
			}
			return guid;
		}

		private bool GetDocumentBinary(Guid docId, byte level, string filePath)
		{
			bool unshrededBinary;
			Guid empty = Guid.Empty;
			long num = (long)0;
			byte num1 = 0;
			int num2 = 0;
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
				{
					sqlConnection.Open();
					string str = (new StringBuilder()).Append("select SiteId,Size,Level,NextBSN,StreamSchema,ContentVersion ").Append("FROM AllDocs ").Append("WHERE Id = @DocId AND [Level] = @DocLevel;").ToString();
					using (SqlCommand sqlCommand = new SqlCommand(str, sqlConnection))
					{
						sqlCommand.Parameters.AddWithValue("@DocId", docId);
						sqlCommand.Parameters.AddWithValue("@DocLevel", level);
						using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
						{
							if (!sqlDataReader.Read())
							{
								throw new Exception(string.Concat("Cannot find document with Id ", docId, " in database."));
							}
							empty = sqlDataReader.GetGuid(0);
							num = sqlDataReader.GetInt64(3);
							num1 = sqlDataReader.GetByte(4);
							if (num1 != 0)
							{
								int num3 = sqlDataReader.GetInt32(1);
								num2 = sqlDataReader.GetInt32(5);
								unshrededBinary = this.GetUnshrededBinary(empty, docId, (long)num3, 0, level, new int?(num2), num, num1, filePath);
							}
							else
							{
								unshrededBinary = this.IsUnshreddedDocumentBinaryRetrieved(empty, docId, num, filePath, level);
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("GetDocumentBinary >> Error Occurred: {0}", exception));
				object[] objArray = new object[] { empty, docId, num, filePath, level, num1, num2 };
				Trace.WriteLine(string.Format("GetDocumentBinary >> siteId: {0} , docId: {1} , nextBsn: {2}, filePath: {3}, Level: {4}, streamSchema: {5}, contentVersion: {6}", objArray));
				unshrededBinary = this.IsUnshreddedDocumentBinaryRetrieved(empty, docId, num, filePath, level);
			}
			return unshrededBinary;
		}

		private bool GetUnshreddedDocumentBinary(Guid siteId, Guid docId, string filePath, string columnName, object columnValue)
		{
			bool flag;
			string str = string.Format((columnName.Equals("BSN", StringComparison.Ordinal) ? "AND ds.BSN = @{0}" : "AND dts.Level = @{0}"), columnName);
			string str1 = (new StringBuilder()).Append("SELECT ds.[Content] ").Append("FROM DocStreams ds WITH (NOLOCK) ").Append("INNER JOIN DocsToStreams dts WITH (NOLOCK) ON dts.SiteId = ds.SiteId ").Append("AND dts.DocId = ds.DocId ").Append("AND dts.Partition = ds.Partition ").Append("AND dts.BSN = ds.BSN ").Append("INNER JOIN AllDocs ad WITH (NOLOCK) ON ad.SiteId = dts.SiteId ").Append("AND ad.Id = dts.DocId ").Append("AND ad.[Level] = dts.[Level] ").Append("WHERE ad.SiteId=@SiteId ").Append("AND ad.Id = @DocId ").Append("AND ds.RbsId IS NULL ").Append("AND dts.HistVersion = 0 ").Append(str).ToString();
			using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
			{
				sqlConnection.Open();
				using (SqlCommand sqlCommand = new SqlCommand(str1, sqlConnection))
				{
					sqlCommand.Parameters.AddWithValue("@SiteId", siteId);
					sqlCommand.Parameters.AddWithValue("@DocId", docId);
					sqlCommand.Parameters.AddWithValue(string.Format("@{0}", columnName), columnValue);
					byte[] numArray = (byte[])sqlCommand.ExecuteScalar();
					if (numArray != null)
					{
						using (Stream memoryStream = new MemoryStream(numArray))
						{
							this.WriteStreamInChunks(memoryStream, filePath);
						}
						flag = true;
						return flag;
					}
				}
				return false;
			}
			return flag;
		}

		private bool GetUnshreddedVersionBinary(Guid siteId, Guid docId, int uiVersion, string filePath)
		{
			bool flag;
			string str = (new StringBuilder()).Append("Select ds.Content ").Append("FROM DocStreams ds WITH (NOLOCK) ").Append("INNER JOIN DocsToStreams dts WITH (NOLOCK) ON dts.SiteId = ds.SiteId AND ").Append("dts.DocId = ds.DocId AND dts.Partition = ds.Partition AND dts.BSN = ds.BSN ").Append("INNER JOIN AllDocVersions adv WITH (NOLOCK) ON adv.SiteId = dts.SiteId AND ").Append("adv.Id = dts.DocId AND adv.[Level]  = dts.[Level] AND adv.UIVersion = dts.HistVersion ").Append("WHERE adv.SiteId=@SiteId ").Append("AND adv.Id=@DocId ").Append("AND adv.UIVersion=@DocVersion ").Append("AND ds.RbsId IS NULL;").ToString();
			using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
			{
				sqlConnection.Open();
				using (SqlCommand sqlCommand = new SqlCommand(str, sqlConnection))
				{
					sqlCommand.Parameters.AddWithValue("@SiteId", siteId);
					sqlCommand.Parameters.AddWithValue("@DocId", docId);
					sqlCommand.Parameters.AddWithValue("@DocVersion", uiVersion);
					byte[] numArray = (byte[])sqlCommand.ExecuteScalar();
					if (numArray != null)
					{
						using (Stream memoryStream = new MemoryStream(numArray))
						{
							this.WriteStreamInChunks(memoryStream, filePath);
						}
						flag = true;
						return flag;
					}
				}
				return false;
			}
			return flag;
		}

		private bool GetUnshrededBinary(Guid siteId, Guid docId, long fileLength, int uiVersion, byte level, int? contentVersion, long nextBsn, byte streamSchema, string filePath)
		{
			bool flag;
			Assembly assembly = this.EnsureSharePointIsAvailable();
			Assembly assembly1 = this.EnsureCobaltIsAvailable();
			object obj = null;
			object obj1 = null;
			obj = Activator.CreateInstance(assembly1.GetType("Cobalt.DisposalEscrow"));
			Type type = assembly.GetType("Microsoft.SharePoint.Utilities.SqlSession");
			object[] connectionString = new object[] { this.ConnectionString, null };
			object obj2 = Activator.CreateInstance(type, connectionString);
			Type type1 = assembly.GetType("Microsoft.SharePoint.SPFileStreamStore");
			ConstructorInfo constructors = type1.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[3];
			object[] databaseId = new object[] { this.GetDatabaseId(), siteId, Guid.Empty, docId, null, fileLength, (byte)0, uiVersion, level, contentVersion, true, nextBsn, streamSchema, null, null, (ulong)((long)65541), obj2, false };
			object obj3 = constructors.Invoke(databaseId);
			Type type2 = assembly.GetType("Microsoft.SharePoint.SPFileStreamHostBlobStore");
			object[] objArray = new object[] { obj3, obj, null };
			object obj4 = Activator.CreateInstance(type2, objArray);
			Type type3 = assembly.GetType("Microsoft.SharePoint.CobaltStream");
			object[] objArray1 = new object[] { obj4, null };
			obj1 = Activator.CreateInstance(type3, objArray1);
			using (Stream stream = (Stream)type3.InvokeMember("GetStream", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj1, null))
			{
				this.WriteStreamInChunks(stream, filePath);
				flag = true;
			}
			return flag;
		}

		private bool GetVersionBinary(Guid docId, int uiVersion, string filePath)
		{
			bool unshrededBinary;
			Guid empty = Guid.Empty;
			byte num = 0;
			byte num1 = 0;
			try
			{
				string str = (new StringBuilder()).Append("SELECT SiteId,Size,Level,StreamSchema ").Append("FROM AllDocVersions ").Append("WHERE Id = @DocId AND UIVersion = @DocVersion").ToString();
				using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
				{
					sqlConnection.Open();
					using (SqlCommand sqlCommand = new SqlCommand(str, sqlConnection))
					{
						sqlCommand.Parameters.AddWithValue("@DocId", docId);
						sqlCommand.Parameters.AddWithValue("@DocVersion", uiVersion);
						using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
						{
							if (!sqlDataReader.Read())
							{
								object[] objArray = new object[] { "Cannot find version ", uiVersion, " of document id ", docId, " in database." };
								throw new Exception(string.Concat(objArray));
							}
							empty = sqlDataReader.GetGuid(0);
							num1 = sqlDataReader.GetByte(3);
							if (num1 != 0)
							{
								int num2 = sqlDataReader.GetInt32(1);
								num = sqlDataReader.GetByte(2);
								int? nullable = null;
								unshrededBinary = this.GetUnshrededBinary(empty, docId, (long)num2, uiVersion, num, nullable, (long)10000, num1, filePath);
							}
							else
							{
								unshrededBinary = this.GetUnshreddedVersionBinary(empty, docId, uiVersion, filePath);
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("GetVersionBinary >> Error Occurred: {0}", exception));
				object[] objArray1 = new object[] { empty, docId, uiVersion, filePath, num, num1 };
				Trace.WriteLine(string.Format("GetVersionBinary >> siteId: {0} , docId: {1} , uiVersion: {2}, filePath: {3}, level: {4}, streamSchema: {5}", objArray1));
				unshrededBinary = this.GetUnshreddedVersionBinary(empty, docId, uiVersion, filePath);
			}
			return unshrededBinary;
		}

		private bool IsUnshreddedDocumentBinaryRetrieved(Guid siteId, Guid docId, long nextBsn, string filePath, byte level)
		{
			if (this.GetUnshreddedDocumentBinary(siteId, docId, filePath, "BSN", nextBsn))
			{
				return true;
			}
			Trace.WriteLine("GetDocumentBinary >> GetUnshreddedDocumentBinary : Couldn't retrieve file contents using NextBsn. So retrieving using Level.");
			return this.GetUnshreddedDocumentBinary(siteId, docId, filePath, "Level", level);
		}

		private void WriteStreamInChunks(Stream fsstream, string filePath)
		{
			int num = (this.ChunkSize <= 0 ? 524288 : this.ChunkSize);
			long length = (long)0;
			byte[] numArray = null;
			using (BinaryReader binaryReader = new BinaryReader(fsstream))
			{
				using (FileStream fileStream = File.Create(filePath))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
					{
						do
						{
							binaryReader.BaseStream.Seek((long)0, SeekOrigin.Current);
							numArray = binaryReader.ReadBytes(num);
							binaryWriter.BaseStream.Seek((long)0, SeekOrigin.Current);
							binaryWriter.Write(numArray);
							length += (long)((int)numArray.Length);
						}
						while (length < fsstream.Length);
					}
				}
			}
		}
	}
}