using System;
using System.Net;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.SK
{
    internal class HttpWebRequestPathed : HttpWebRequest
    {
        protected HttpWebRequestPathed(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
            serializationInfo, streamingContext)
        {
        }

        public static WebRequest GetWebRequest(string url)
        {
            WebRequest version10 = WebRequest.Create(url);
            ((HttpWebRequest)version10).KeepAlive = false;
            ((HttpWebRequest)version10).ProtocolVersion = HttpVersion.Version10;
            return version10;
        }
    }
}