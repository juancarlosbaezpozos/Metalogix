using System;
using System.Diagnostics;
using System.IO;

namespace Metalogix.SharePoint.Adapters
{
    public static class ChunkedContentCopyHandler
    {
        private readonly static CopySessionStore _openSessions;

        static ChunkedContentCopyHandler()
        {
            ChunkedContentCopyHandler._openSessions = new CopySessionStore();
        }

        public static string CloseFileCopySession(Guid sessionId)
        {
            long length = ChunkedContentCopyHandler._openSessions.GetStream(sessionId).Length;
            ChunkedFile chunkedFile = ChunkedContentCopyHandler._openSessions.Close(sessionId);
            if (chunkedFile.RemoveAfterTransfer)
            {
                Trace.WriteLine(string.Format(
                    "ChunkedContentCopyHandler >> Session closed: ID={0}; Removing the temporary file: {1}", sessionId,
                    chunkedFile.FileName));
                return null;
            }

            Trace.WriteLine(string.Format(
                "ChunkedContentCopyHandler >> Session closed: ID={0}; Temporary file: {1}; Size:{2}", sessionId,
                chunkedFile.FileName, length));
            return sessionId.ToString();
        }

        public static byte[] GetTransferedContent(Guid sessionId, bool removeStream)
        {
            Stream stream = ChunkedContentCopyHandler._openSessions.GetStream(sessionId);
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, Convert.ToInt32(stream.Length));
            if (removeStream)
            {
                ChunkedContentCopyHandler._openSessions.Close(sessionId, true);
            }

            return array;
        }

        public static Guid OpenFileCopySession(StreamType streamType, int retentionTime)
        {
            return ChunkedContentCopyHandler.OpenFileCopySession(null, null, false, streamType, retentionTime);
        }

        public static Guid OpenFileCopySession(byte[] content, StreamType streamType, int retentionTime)
        {
            return ChunkedContentCopyHandler.OpenFileCopySession(null, content, true, streamType, retentionTime);
        }

        private static Guid OpenFileCopySession(string dstPath, byte[] content, bool remoteFileAfterTransfer,
            StreamType type, int retentionTime)
        {
            Guid guid;
            Stream fileStream;
            FileMode fileMode = FileMode.CreateNew;
            if (type == StreamType.File)
            {
                if (string.IsNullOrEmpty(dstPath))
                {
                    dstPath = Path.GetTempFileName();
                    fileMode = FileMode.Create;
                }

                string directoryName = Path.GetDirectoryName(dstPath);
                if (directoryName == null)
                {
                    throw new Exception(string.Concat(
                        "ChunkedContentCopyHandler >> Failed to get the directory name for '", dstPath, "'"));
                }

                try
                {
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(string.Format(
                        "ChunkedContentCopyHandler >> Failed to create folder \"{0}\", error: \r\n{1}", directoryName,
                        exception));
                    throw;
                }
            }

            try
            {
                if (type == StreamType.File)
                {
                    fileStream = new FileStream(dstPath, fileMode, FileAccess.ReadWrite,
                        FileShare.Read | FileShare.Delete, 512, FileOptions.DeleteOnClose);
                }
                else
                {
                    fileStream = new MemoryStream();
                }

                Stream stream = fileStream;
                ChunkedFile chunkedFile = new ChunkedFile()
                {
                    Stream = stream,
                    RemoveAfterTransfer = remoteFileAfterTransfer,
                    RetentionTimeMinutes = retentionTime
                };
                ChunkedFile chunkedFile1 = chunkedFile;
                if (content != null && (int)content.Length > 0)
                {
                    chunkedFile1.Stream.Write(content, 0, (int)content.Length);
                    chunkedFile1.Stream.Seek((long)0, SeekOrigin.Begin);
                }

                Guid guid1 = ChunkedContentCopyHandler._openSessions.Add(chunkedFile1);
                object[] fileName = new object[]
                {
                    (content != null ? "READ" : "WRITE"), guid1, chunkedFile1.FileName,
                    (content != null ? content.LongLength : (long)0), chunkedFile1.Stream
                };
                Trace.WriteLine(string.Format(
                    "ChunkedContentCopyHandler >> New {0} session opened: ID={1}; TempFile={2}; ContentLength={3}; StreamType={4}",
                    fileName));
                guid = guid1;
            }
            catch (Exception exception1)
            {
                Trace.WriteLine(string.Format(
                    "ChunkedContentCopyHandler >> Failed to create file \"{0}\", error: \r\n{1}", dstPath, exception1));
                throw;
            }

            return guid;
        }

        public static byte[] ReadChunk(Guid sessionId, long bytesToRead)
        {
            Stream stream = ChunkedContentCopyHandler._openSessions.GetStream(sessionId);
            byte[] result;
            try
            {
                byte[] array = new byte[Math.Min(stream.Length - stream.Position, bytesToRead)];
                stream.Read(array, 0, array.Length);
                result = array;
            }
            catch (Exception arg)
            {
                Trace.WriteLine(string.Format(
                    "ChunkedContentCopyHandler >> Failed to read the file \"{0}\", error:\r\n{1}",
                    ChunkedContentCopyHandler._openSessions[sessionId].FileName, arg));
                throw;
            }

            return result;
        }

        public static void RemoveTransferedContent(Guid sessionId)
        {
            ChunkedContentCopyHandler._openSessions.Close(sessionId, true);
        }

        public static void WriteChunk(Guid sessionId, byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Stream stream = ChunkedContentCopyHandler._openSessions.GetStream(sessionId);
            try
            {
                stream.Write(data, 0, (int)data.Length);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Format(
                    "ChunkedContentCopyHandler >> Failed to write to the file \"{0}\", error:\r\n{1}",
                    ChunkedContentCopyHandler._openSessions[sessionId].FileName, exception));
                throw;
            }
        }
    }
}