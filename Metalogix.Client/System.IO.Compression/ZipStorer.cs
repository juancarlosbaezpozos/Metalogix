using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression
{
    public class ZipStorer : IDisposable
    {
        public enum Compression : ushort
        {
            Store,
            Deflate = 8
        }

        public struct ZipFileEntry
        {
            public ZipStorer.Compression Method;

            public string FilenameInZip;

            public uint FileSize;

            public uint CompressedSize;

            public uint HeaderOffset;

            public uint FileOffset;

            public uint HeaderSize;

            public uint Crc32;

            public DateTime ModifyTime;

            public string Comment;

            public bool EncodeUTF8;

            public override string ToString()
            {
                return this.FilenameInZip;
            }
        }

        public bool EncodeUTF8;

        public bool ForceDeflating;

        private List<ZipStorer.ZipFileEntry> Files = new List<ZipStorer.ZipFileEntry>();

        private string FileName;

        private Stream ZipFileStream;

        private string Comment = "";

        private byte[] CentralDirImage;

        private ushort ExistingFiles;

        private FileAccess Access;

        private static uint[] CrcTable;

        private static Encoding DefaultEncoding;

        static ZipStorer()
        {
            ZipStorer.CrcTable = null;
            ZipStorer.DefaultEncoding = Encoding.GetEncoding(437);
            ZipStorer.CrcTable = new uint[256];
            for (int i = 0; i < ZipStorer.CrcTable.Length; i++)
            {
                uint num = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((num & 1u) != 0u)
                    {
                        num = (3988292384u ^ num >> 1);
                    }
                    else
                    {
                        num >>= 1;
                    }
                }

                ZipStorer.CrcTable[i] = num;
            }
        }

        public static ZipStorer Create(string _filename, string _comment)
        {
            Stream stream = null;
            ZipStorer zipStorer;
            try
            {
                stream = new FileStream(_filename, FileMode.Create, FileAccess.ReadWrite);
                zipStorer = ZipStorer.Create(stream, _comment);
                zipStorer.Comment = _comment;
                zipStorer.FileName = _filename;
                zipStorer.ZipFileStream = stream;
                stream = null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return zipStorer;
        }

        public static ZipStorer Create(Stream _stream, string _comment)
        {
            return new ZipStorer
            {
                Comment = _comment,
                ZipFileStream = _stream,
                Access = FileAccess.Write
            };
        }

        public static ZipStorer Open(string _filename, FileAccess _access)
        {
            Stream stream = null;
            ZipStorer zipStorer;
            try
            {
                stream = new FileStream(_filename, FileMode.Open,
                    (_access == FileAccess.Read) ? FileAccess.Read : FileAccess.ReadWrite);
                zipStorer = ZipStorer.Open(stream, _access);
                zipStorer.FileName = _filename;
                zipStorer.ZipFileStream = stream;
                stream = null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return zipStorer;
        }

        public static ZipStorer Open(Stream _stream, FileAccess _access)
        {
            if (!_stream.CanSeek && _access != FileAccess.Read)
            {
                throw new InvalidOperationException("Stream cannot seek");
            }

            ZipStorer zipStorer = null;
            ZipStorer result;
            try
            {
                zipStorer = new ZipStorer();
                zipStorer.ZipFileStream = _stream;
                zipStorer.Access = _access;
                if (!zipStorer.ReadFileInfo())
                {
                    throw new InvalidDataException();
                }

                result = zipStorer;
                zipStorer = null;
            }
            finally
            {
                if (zipStorer != null)
                {
                    zipStorer.Close();
                }
            }

            return result;
        }

        public void AddFile(ZipStorer.Compression _method, string _pathname, string _filenameInZip, string _comment)
        {
            if (this.Access == FileAccess.Read)
            {
                throw new InvalidOperationException("Writing is not alowed");
            }

            using (FileStream fileStream = new FileStream(_pathname, FileMode.Open, FileAccess.Read))
            {
                this.AddStream(_method, _filenameInZip, fileStream, File.GetLastWriteTime(_pathname), _comment);
                fileStream.Close();
            }
        }

        public void AddStream(ZipStorer.Compression _method, string _filenameInZip, Stream _source, DateTime _modTime,
            string _comment)
        {
            if (this.Access == FileAccess.Read)
            {
                throw new InvalidOperationException("Writing is not alowed");
            }

            if (this.Files.Count != 0)
            {
                ZipStorer.ZipFileEntry zipFileEntry = this.Files[this.Files.Count - 1];
            }

            ZipStorer.ZipFileEntry item = default(ZipStorer.ZipFileEntry);
            item.Method = _method;
            item.EncodeUTF8 = this.EncodeUTF8;
            item.FilenameInZip = this.NormalizedFilename(_filenameInZip);
            item.Comment = ((_comment == null) ? "" : _comment);
            item.Crc32 = 0u;
            item.HeaderOffset = (uint)this.ZipFileStream.Position;
            item.ModifyTime = _modTime;
            this.WriteLocalHeader(ref item);
            item.FileOffset = (uint)this.ZipFileStream.Position;
            this.Store(ref item, _source);
            _source.Close();
            this.UpdateCrcAndSizes(ref item);
            this.Files.Add(item);
        }

        public void Close()
        {
            if (this.Access != FileAccess.Read)
            {
                uint offset = (uint)this.ZipFileStream.Position;
                uint num = 0u;
                if (this.CentralDirImage != null)
                {
                    this.ZipFileStream.Write(this.CentralDirImage, 0, this.CentralDirImage.Length);
                }

                for (int i = 0; i < this.Files.Count; i++)
                {
                    long position = this.ZipFileStream.Position;
                    this.WriteCentralDirRecord(this.Files[i]);
                    num += (uint)(this.ZipFileStream.Position - position);
                }

                if (this.CentralDirImage != null)
                {
                    this.WriteEndRecord(num + (uint)this.CentralDirImage.Length, offset);
                }
                else
                {
                    this.WriteEndRecord(num, offset);
                }
            }

            if (this.ZipFileStream != null)
            {
                this.ZipFileStream.Flush();
                this.ZipFileStream.Dispose();
                this.ZipFileStream = null;
            }
        }

        public List<ZipStorer.ZipFileEntry> ReadCentralDir()
        {
            if (this.CentralDirImage == null)
            {
                throw new InvalidOperationException("Central directory currently does not exist");
            }

            List<ZipStorer.ZipFileEntry> list = new List<ZipStorer.ZipFileEntry>();
            ushort num2;
            ushort num3;
            ushort num4;
            for (int i = 0; i < this.CentralDirImage.Length; i += (int)(46 + num2 + num3 + num4))
            {
                uint num = BitConverter.ToUInt32(this.CentralDirImage, i);
                if (num != 33639248u)
                {
                    break;
                }

                bool flag = (BitConverter.ToUInt16(this.CentralDirImage, i + 8) & 2048) != 0;
                ushort method = BitConverter.ToUInt16(this.CentralDirImage, i + 10);
                uint dt = BitConverter.ToUInt32(this.CentralDirImage, i + 12);
                uint crc = BitConverter.ToUInt32(this.CentralDirImage, i + 16);
                uint compressedSize = BitConverter.ToUInt32(this.CentralDirImage, i + 20);
                uint fileSize = BitConverter.ToUInt32(this.CentralDirImage, i + 24);
                num2 = BitConverter.ToUInt16(this.CentralDirImage, i + 28);
                num3 = BitConverter.ToUInt16(this.CentralDirImage, i + 30);
                num4 = BitConverter.ToUInt16(this.CentralDirImage, i + 32);
                uint headerOffset = BitConverter.ToUInt32(this.CentralDirImage, i + 42);
                uint headerSize = (uint)(46 + num2 + num3 + num4);
                Encoding encoding = flag ? Encoding.UTF8 : ZipStorer.DefaultEncoding;
                ZipStorer.ZipFileEntry item = default(ZipStorer.ZipFileEntry);
                item.Method = (ZipStorer.Compression)method;
                item.FilenameInZip = encoding.GetString(this.CentralDirImage, i + 46, (int)num2);
                item.FileOffset = this.GetFileOffset(headerOffset);
                item.FileSize = fileSize;
                item.CompressedSize = compressedSize;
                item.HeaderOffset = headerOffset;
                item.HeaderSize = headerSize;
                item.Crc32 = crc;
                item.ModifyTime = this.DosTimeToDateTime(dt);
                if (num4 > 0)
                {
                    item.Comment = encoding.GetString(this.CentralDirImage, i + 46 + (int)num2 + (int)num3, (int)num4);
                }

                list.Add(item);
            }

            return list;
        }

        public bool ExtractFile(ZipStorer.ZipFileEntry _zfe, string _filename)
        {
            string directoryName = Path.GetDirectoryName(_filename);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (Directory.Exists(_filename))
            {
                return true;
            }

            bool result;
            using (FileStream fileStream = new FileStream(_filename, FileMode.Create, FileAccess.Write))
            {
                bool flag = this.ExtractFile(_zfe, fileStream);
                if (flag)
                {
                    fileStream.Close();
                }

                File.SetCreationTime(_filename, _zfe.ModifyTime);
                File.SetLastWriteTime(_filename, _zfe.ModifyTime);
                result = flag;
            }

            return result;
        }

        public bool ExtractFile(ZipStorer.ZipFileEntry _zfe, Stream _stream)
        {
            if (!_stream.CanWrite)
            {
                throw new InvalidOperationException("Stream cannot be written");
            }

            byte[] array = new byte[4];
            this.ZipFileStream.Seek((long)((ulong)_zfe.HeaderOffset), SeekOrigin.Begin);
            this.ZipFileStream.Read(array, 0, 4);
            if (BitConverter.ToUInt32(array, 0) != 67324752u)
            {
                return false;
            }

            Stream stream;
            if (_zfe.Method == ZipStorer.Compression.Store)
            {
                stream = this.ZipFileStream;
            }
            else
            {
                if (_zfe.Method != ZipStorer.Compression.Deflate)
                {
                    return false;
                }

                stream = new DeflateStream(this.ZipFileStream, CompressionMode.Decompress, true);
            }

            byte[] array2 = new byte[16384];
            this.ZipFileStream.Seek((long)((ulong)_zfe.FileOffset), SeekOrigin.Begin);
            int num2;
            for (uint num = _zfe.FileSize; num > 0u; num -= (uint)num2)
            {
                num2 = stream.Read(array2, 0, (int)Math.Min((long)((ulong)num), (long)array2.Length));
                _stream.Write(array2, 0, num2);
            }

            _stream.Flush();
            if (_zfe.Method == ZipStorer.Compression.Deflate)
            {
                stream.Dispose();
            }

            return true;
        }

        public static bool RemoveEntries(ref ZipStorer _zip, List<ZipStorer.ZipFileEntry> _zfes)
        {
            if (!(_zip.ZipFileStream is FileStream))
            {
                throw new InvalidOperationException("RemoveEntries is allowed just over streams of type FileStream");
            }

            List<ZipStorer.ZipFileEntry> list = _zip.ReadCentralDir();
            string tempFileName = Path.GetTempFileName();
            string tempFileName2 = Path.GetTempFileName();
            try
            {
                ZipStorer zipStorer = ZipStorer.Create(tempFileName, string.Empty);
                foreach (ZipStorer.ZipFileEntry current in list)
                {
                    if (!_zfes.Contains(current) && _zip.ExtractFile(current, tempFileName2))
                    {
                        zipStorer.AddFile(current.Method, tempFileName2, current.FilenameInZip, current.Comment);
                    }
                }

                _zip.Close();
                zipStorer.Close();
                File.Delete(_zip.FileName);
                File.Move(tempFileName, _zip.FileName);
                _zip = ZipStorer.Open(_zip.FileName, _zip.Access);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }

                if (File.Exists(tempFileName2))
                {
                    File.Delete(tempFileName2);
                }
            }

            return true;
        }

        private uint GetFileOffset(uint _headerOffset)
        {
            byte[] array = new byte[2];
            this.ZipFileStream.Seek((long)((ulong)(_headerOffset + 26u)), SeekOrigin.Begin);
            this.ZipFileStream.Read(array, 0, 2);
            ushort num = BitConverter.ToUInt16(array, 0);
            this.ZipFileStream.Read(array, 0, 2);
            ushort num2 = BitConverter.ToUInt16(array, 0);
            return (uint)((long)(30 + num + num2) + (long)((ulong)_headerOffset));
        }

        private void WriteLocalHeader(ref ZipStorer.ZipFileEntry _zfe)
        {
            long position = this.ZipFileStream.Position;
            Encoding encoding = _zfe.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding;
            byte[] bytes = encoding.GetBytes(_zfe.FilenameInZip);
            this.ZipFileStream.Write(new byte[]
            {
                80,
                75,
                3,
                4,
                20,
                0
            }, 0, 6);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? 2048 : 0), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(this.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
            Stream arg_B7_0 = this.ZipFileStream;
            byte[] buffer = new byte[12];
            arg_B7_0.Write(buffer, 0, 12);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
            this.ZipFileStream.Write(bytes, 0, bytes.Length);
            _zfe.HeaderSize = (uint)(this.ZipFileStream.Position - position);
        }

        private void WriteCentralDirRecord(ZipStorer.ZipFileEntry _zfe)
        {
            Encoding encoding = _zfe.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding;
            byte[] bytes = encoding.GetBytes(_zfe.FilenameInZip);
            byte[] bytes2 = encoding.GetBytes(_zfe.Comment);
            this.ZipFileStream.Write(new byte[]
            {
                80,
                75,
                1,
                2,
                23,
                11,
                20,
                0
            }, 0, 8);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? 2048 : 0), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(this.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes2.Length), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(33024), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);
            this.ZipFileStream.Write(bytes, 0, bytes.Length);
            this.ZipFileStream.Write(bytes2, 0, bytes2.Length);
        }

        private void WriteEndRecord(uint _size, uint _offset)
        {
            Encoding encoding = this.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding;
            byte[] bytes = encoding.GetBytes(this.Comment);
            this.ZipFileStream.Write(new byte[]
            {
                80,
                75,
                5,
                6,
                0,
                0,
                0,
                0
            }, 0, 8);
            this.ZipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.Files.Count + this.ExistingFiles)), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.Files.Count + this.ExistingFiles)), 0, 2);
            this.ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
            this.ZipFileStream.Write(bytes, 0, bytes.Length);
        }

        private void Store(ref ZipStorer.ZipFileEntry _zfe, Stream _source)
        {
            byte[] array = new byte[16384];
            uint num = 0u;
            long position = this.ZipFileStream.Position;
            long position2 = _source.Position;
            Stream stream;
            if (_zfe.Method == ZipStorer.Compression.Store)
            {
                stream = this.ZipFileStream;
            }
            else
            {
                stream = new DeflateStream(this.ZipFileStream, CompressionMode.Compress, true);
            }

            _zfe.Crc32 = 4294967295u;
            int num2;
            do
            {
                num2 = _source.Read(array, 0, array.Length);
                num += (uint)num2;
                if (num2 > 0)
                {
                    stream.Write(array, 0, num2);
                    uint num3 = 0u;
                    while ((ulong)num3 < (ulong)((long)num2))
                    {
                        _zfe.Crc32 =
                            (ZipStorer.CrcTable[
                                 (int)((UIntPtr)((_zfe.Crc32 ^ (uint)array[(int)((UIntPtr)num3)]) & 255u))] ^
                             _zfe.Crc32 >> 8);
                        num3 += 1u;
                    }
                }
            } while (num2 == array.Length);

            stream.Flush();
            if (_zfe.Method == ZipStorer.Compression.Deflate)
            {
                stream.Dispose();
            }

            _zfe.Crc32 ^= 4294967295u;
            _zfe.FileSize = num;
            _zfe.CompressedSize = (uint)(this.ZipFileStream.Position - position);
            if (_zfe.Method == ZipStorer.Compression.Deflate && !this.ForceDeflating && _source.CanSeek &&
                _zfe.CompressedSize > _zfe.FileSize)
            {
                _zfe.Method = ZipStorer.Compression.Store;
                this.ZipFileStream.Position = position;
                this.ZipFileStream.SetLength(position);
                _source.Position = position2;
                this.Store(ref _zfe, _source);
            }
        }

        private uint DateTimeToDosTime(DateTime _dt)
        {
            return (uint)(_dt.Second / 2 | _dt.Minute << 5 | _dt.Hour << 11 | _dt.Day << 16 | _dt.Month << 21 |
                          _dt.Year - 1980 << 25);
        }

        private DateTime DosTimeToDateTime(uint _dt)
        {
            return new DateTime((int)((_dt >> 25) + 1980u), (int)(_dt >> 21 & 15u), (int)(_dt >> 16 & 31u),
                (int)(_dt >> 11 & 31u), (int)(_dt >> 5 & 63u), (int)((_dt & 31u) * 2u));
        }

        private void UpdateCrcAndSizes(ref ZipStorer.ZipFileEntry _zfe)
        {
            long position = this.ZipFileStream.Position;
            this.ZipFileStream.Position = (long)((ulong)(_zfe.HeaderOffset + 8u));
            this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
            this.ZipFileStream.Position = (long)((ulong)(_zfe.HeaderOffset + 14u));
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
            this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
            this.ZipFileStream.Position = position;
        }

        private string NormalizedFilename(string _filename)
        {
            string text = _filename.Replace('\\', '/');
            int num = text.IndexOf(':');
            if (num >= 0)
            {
                text = text.Remove(0, num + 1);
            }

            return text.Trim(new char[]
            {
                '/'
            });
        }

        private bool ReadFileInfo()
        {
            if (this.ZipFileStream.Length < 22L)
            {
                return false;
            }

            try
            {
                this.ZipFileStream.Seek(-17L, SeekOrigin.End);
                BinaryReader binaryReader = new BinaryReader(this.ZipFileStream);
                while (true)
                {
                    this.ZipFileStream.Seek(-5L, SeekOrigin.Current);
                    uint num = binaryReader.ReadUInt32();
                    if (num == 101010256u)
                    {
                        break;
                    }

                    if (this.ZipFileStream.Position <= 0L)
                    {
                        goto Block_5;
                    }
                }

                this.ZipFileStream.Seek(6L, SeekOrigin.Current);
                ushort existingFiles = binaryReader.ReadUInt16();
                int num2 = binaryReader.ReadInt32();
                uint num3 = binaryReader.ReadUInt32();
                ushort num4 = binaryReader.ReadUInt16();
                bool result;
                if (this.ZipFileStream.Position + (long)((ulong)num4) != this.ZipFileStream.Length)
                {
                    result = false;
                    return result;
                }

                this.ExistingFiles = existingFiles;
                this.CentralDirImage = new byte[num2];
                this.ZipFileStream.Seek((long)((ulong)num3), SeekOrigin.Begin);
                this.ZipFileStream.Read(this.CentralDirImage, 0, num2);
                this.ZipFileStream.Seek((long)((ulong)num3), SeekOrigin.Begin);
                result = true;
                return result;
                Block_5: ;
            }
            catch
            {
            }

            return false;
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}