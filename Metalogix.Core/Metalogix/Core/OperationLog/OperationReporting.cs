using Metalogix.Utilities;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.Core.OperationLog
{
    public class OperationReporting
    {
        private readonly StringBuilder _buffer;

        private XmlWriter _xmlWriter;

        private volatile bool _started;

        private volatile bool _ended;

        public bool HasErrors { get; private set; }

        public string ResultXml
        {
            get
            {
                if (!this._ended)
                {
                    return string.Empty;
                }

                return this._buffer.ToString();
            }
        }

        public OperationReporting()
        {
            this._started = false;
            this._ended = false;
            this._buffer = new StringBuilder(1024);
            this._xmlWriter = null;
        }

        public void End()
        {
            if (this._started)
            {
                this._xmlWriter.WriteEndElement();
                this._xmlWriter.Flush();
                this._xmlWriter.Close();
                this._started = false;
                this._ended = true;
            }
        }

        public void LogError(Exception exception, string detail)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ExceptionUtils.GetExceptionMessage(exception, stringBuilder);
            OperationReporting.LogError(this._xmlWriter, exception.Message, detail, stringBuilder.ToString(), 0, 0);
            this.HasErrors = true;
        }

        public void LogError(string message, string detail, string stack, int errorCode = 0, int hResult = 0)
        {
            OperationReporting.LogError(this._xmlWriter, message, detail, stack, errorCode, hResult);
        }

        public static void LogError(XmlWriter xmlWriter, string message, string detail, string stack, int errorCode,
            int hResult = 0)
        {
            xmlWriter.WriteStartElement(OperationReportingElements.Error.ToString());
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Message.ToString(), message ?? string.Empty);
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Detail.ToString(), detail ?? string.Empty);
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Stack.ToString(), stack ?? string.Empty);
            xmlWriter.WriteAttributeString(OperationReportingAttributes.ErrorCode.ToString(), errorCode.ToString());
            xmlWriter.WriteAttributeString(OperationReportingAttributes.HResult.ToString(), hResult.ToString());
            xmlWriter.WriteEndElement();
        }

        public void LogInformation(string message, string detail)
        {
            OperationReporting.LogInformation(this._xmlWriter, message, detail);
        }

        public static void LogInformation(XmlWriter xmlWriter, string message, string detail)
        {
            xmlWriter.WriteStartElement(OperationReportingElements.Information.ToString());
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Message.ToString(), message ?? string.Empty);
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Detail.ToString(), detail ?? string.Empty);
            xmlWriter.WriteEndElement();
        }

        public void LogObjectXml(string objectXmlData)
        {
            OperationReporting.LogObjectXml(this._xmlWriter, objectXmlData);
        }

        public static void LogObjectXml(XmlWriter xmlWriter, string objectXmlData)
        {
            xmlWriter.WriteStartElement(OperationReportingElements.ObjectXml.ToString());
            xmlWriter.WriteRaw(objectXmlData ?? string.Empty);
            xmlWriter.WriteEndElement();
        }

        public void LogWarning(string message, string detail)
        {
            OperationReporting.LogWarning(this._xmlWriter, message, detail);
        }

        public static void LogWarning(XmlWriter xmlWriter, string message, string detail)
        {
            xmlWriter.WriteStartElement(OperationReportingElements.Warning.ToString());
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Message.ToString(), message ?? string.Empty);
            xmlWriter.WriteAttributeString(OperationReportingAttributes.Detail.ToString(), detail ?? string.Empty);
            xmlWriter.WriteEndElement();
        }

        public void Start()
        {
            if (!this._started)
            {
                this._ended = false;
                this._started = true;
                this._buffer.Length = 0;
                this._xmlWriter = XmlWriter.Create(this._buffer, XmlUtility.WriterSettings);
                this._xmlWriter.WriteStartElement(OperationReportingElements.Result.ToString());
            }
        }
    }
}