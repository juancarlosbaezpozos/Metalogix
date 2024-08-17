using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public class BinaryTransfer
    {
        private const long _MAX_CHUNK_SIZE = 4194304L;

        private readonly IBinaryTransferHandler _handler;

        private readonly StreamType _streamType;

        private readonly int _retentionTime;

        private long _fullBytesCopied;

        public long FullBytesCopied
        {
            get { return this._fullBytesCopied; }
        }

        public BinaryTransfer(IBinaryTransferHandler handler, StreamType streamType, int retentionTime)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (retentionTime <= 0)
            {
                throw new ArgumentException("Retention time for chunked transfer must be bigger than zero.",
                    "retentionTime");
            }

            this._handler = handler;
            this._streamType = streamType;
            this._retentionTime = retentionTime;
        }

        public static bool[] AnalyzeContents(byte[][] contents)
        {
            if (contents == null)
            {
                return new bool[0];
            }

            if ((int)contents.Length == 1)
            {
                bool[] longLength = new bool[] { contents[0].LongLength > (long)4194304 };
                return longLength;
            }

            BinaryTransfer.Contents[] content = new BinaryTransfer.Contents[(int)contents.Length];
            for (int i = 0; i < (int)contents.Length; i++)
            {
                content[i] = new BinaryTransfer.Contents(i, (contents[i] != null ? contents[i].LongLength : (long)0));
            }

            Array.Sort<BinaryTransfer.Contents>(content,
                new Comparison<BinaryTransfer.Contents>(BinaryTransfer.Contents.SizeComparer));
            long size = (long)0;
            for (int j = 0; j < (int)contents.Length; j++)
            {
                size += content[j].Size;
                content[j].MoveByChunks = size > (long)4194304;
            }

            Array.Sort<BinaryTransfer.Contents>(content,
                new Comparison<BinaryTransfer.Contents>(BinaryTransfer.Contents.PositionComparer));
            bool[] moveByChunks = new bool[(int)contents.Length];
            for (int k = 0; k < (int)contents.Length; k++)
            {
                moveByChunks[k] = content[k].MoveByChunks;
            }

            return moveByChunks;
        }

        private void CopyFileContentToServer(byte[] content, Guid sessionId)
        {
            long num = 0L;
            long longLength = content.LongLength;
            while (num < longLength)
            {
                long num2;
                if (num + 4194304L > longLength)
                {
                    num2 = longLength - num;
                }
                else
                {
                    num2 = 4194304L;
                }

                byte[] array = new byte[num2];
                Array.Copy(content, num, array, 0L, array.LongLength);
                this._handler.WriteChunk(sessionId, array);
                if (num + num2 >= longLength)
                {
                    return;
                }

                num += num2;
                this._fullBytesCopied += num2;
            }
        }

        private byte[] ReadFileContentFromServer(Guid sessionId)
        {
            byte[] numArray;
            byte[] array;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                do
                {
                    numArray = this._handler.ReadChunk(sessionId, (long)4194304);
                    memoryStream.Write(numArray, 0, (int)numArray.Length);
                    this._fullBytesCopied += numArray.LongLength;
                } while (numArray.LongLength >= (long)4194304);

                array = memoryStream.ToArray();
            }

            return array;
        }

        public byte[] StartRead(Guid sessionId)
        {
            Trace.WriteLine(string.Format("BinaryTransfer >> Starting chunked read transfer: SessionID={0}",
                sessionId));
            byte[] numArray = this.ReadFileContentFromServer(sessionId);
            this._handler.CloseFileCopySession(sessionId);
            Trace.WriteLine(string.Format(
                "BinaryTransfer >> Chunked read finished: SessionID={0}; Content Length Read={1}", sessionId,
                numArray.LongLength));
            return numArray;
        }

        public string StartWrite(byte[] content)
        {
            Trace.WriteLine(string.Format(
                "BinaryTransfer >> Starting chunked write transfer: Content Length To Write={0}", content.LongLength));
            Guid guid = this._handler.OpenFileCopySession(this._streamType, this._retentionTime);
            this.CopyFileContentToServer(content, guid);
            string str = this._handler.CloseFileCopySession(guid);
            Trace.WriteLine(string.Format("BinaryTransfer >> Chunked write finished: SessionID={0}; RemotePath={1}",
                guid, str));
            return str;
        }

        private class Contents
        {
            public bool MoveByChunks { get; set; }

            private int Position { get; set; }

            public long Size { get; private set; }

            public Contents(int position, long size)
            {
                this.Position = position;
                this.Size = size;
            }

            public static int PositionComparer(BinaryTransfer.Contents x, BinaryTransfer.Contents y)
            {
                return x.Position.CompareTo(y.Position);
            }

            public static int SizeComparer(BinaryTransfer.Contents x, BinaryTransfer.Contents y)
            {
                return x.Size.CompareTo(y.Size);
            }
        }
    }
}