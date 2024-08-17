using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public class GetDocumentResult
    {
        private readonly byte[] _content;

        private readonly Guid _sessionID;

        public bool IsSessionID { get; private set; }

        public Guid SessionID
        {
            get
            {
                if (!this.IsSessionID)
                {
                    throw new Exception("Content is not a session ID.");
                }

                return this._sessionID;
            }
        }

        public GetDocumentResult(byte[] content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this._content = content;
            if (this._content.LongLength == (long)17 && this._content[0] == 1)
            {
                byte[] numArray = new byte[16];
                for (int i = 1; i < (int)this._content.Length; i++)
                {
                    numArray[i - 1] = this._content[i];
                }

                this._sessionID = new Guid(numArray);
                this.IsSessionID = true;
            }
        }

        public GetDocumentResult(Guid sessionID)
        {
            this._sessionID = sessionID;
            this._content = new byte[17];
            this._content[0] = 1;
            sessionID.ToByteArray().CopyTo(this._content, 1);
            this.IsSessionID = true;
        }

        public byte[] GetContent()
        {
            return this._content;
        }
    }
}