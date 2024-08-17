using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Utilities
{
    public class ExceptionDetail
    {
        public string Detail { get; set; }

        public int ErrorCode { get; set; }

        public int HResult { get; set; }

        public string Message { get; set; }

        public ExceptionDetail()
        {
            this.Message = string.Empty;
            this.Detail = string.Empty;
            this.HResult = 0;
            this.ErrorCode = 0;
        }

        public ExceptionDetail(string exceptionMessage, string exceptionDetail, int hResult, int errorCode)
        {
            this.Message = exceptionMessage;
            this.Detail = exceptionDetail;
            this.HResult = hResult;
            this.ErrorCode = errorCode;
        }
    }
}