using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class FaultMessage : PreEmptive.SoS.Client.Messages.Message
    {
        private ExceptionInformation[] exceptionsField;

        private AppComponent[] componentsField;

        private FaultEventType faultEventField;

        private string contactField;

        private string commentField;

        public string Comment
        {
            get { return this.commentField; }
            set { this.commentField = value; }
        }

        public AppComponent[] Components
        {
            get { return this.componentsField; }
            set { this.componentsField = value; }
        }

        public string Contact
        {
            get { return this.contactField; }
            set { this.contactField = value; }
        }

        public ExceptionInformation[] Exceptions
        {
            get { return this.exceptionsField; }
            set { this.exceptionsField = value; }
        }

        public FaultEventType FaultEvent
        {
            get { return this.faultEventField; }
            set { this.faultEventField = value; }
        }

        public FaultMessage(Exception exception_0) : this(exception_0, null, 0)
        {
        }

        public FaultMessage(Exception exception_0, StackTrace currentStack, int skipFrames)
        {
            if (exception_0 == null)
            {
                throw new ArgumentException("Argument cannot be null", "e");
            }

            ArrayList arrayLists = new ArrayList();
            Exception exception0 = exception_0;
            int num = 0;
            while (exception0 != null)
            {
                ExceptionInformation exceptionInformation = new ExceptionInformation()
                {
                    Message = exception0.Message
                };
                int num1 = num;
                num = num1 + 1;
                exceptionInformation.Sequence = num1;
                exceptionInformation.Type = exception0.GetType().ToString();
                StackTrace stackTrace = (num != 1 || exception0.StackTrace != null || currentStack == null
                    ? new StackTrace(exception0)
                    : currentStack);
                int frameCount = stackTrace.FrameCount - skipFrames;
                if (frameCount > 0)
                {
                    exceptionInformation.StackTrace = new StackEntry[frameCount];
                    for (int i = 0; i < frameCount; i++)
                    {
                        StackFrame frame = stackTrace.GetFrame(i + skipFrames);
                        StackEntry stackEntry = new StackEntry();
                        MethodBase method = frame.GetMethod();
                        stackEntry.File = frame.GetFileName();
                        stackEntry.Line = Convert.ToInt64(frame.GetFileLineNumber());
                        stackEntry.Method = method.Name;
                        stackEntry.Sequence = i;
                        stackEntry.Signature = this.GetSignature(method);
                        if (method.DeclaringType != null)
                        {
                            stackEntry.Type = method.DeclaringType.ToString();
                        }

                        exceptionInformation.StackTrace[i] = stackEntry;
                    }
                }

                arrayLists.Add(exceptionInformation);
                exception0 = exception0.InnerException;
            }

            if (arrayLists.Count > 0)
            {
                object[] array = arrayLists.ToArray();
                this.exceptionsField = new ExceptionInformation[arrayLists.Count];
                Array.Copy(array, this.exceptionsField, arrayLists.Count);
            }

            if (exception_0.Data["AppComponentReporting"] != null && !(bool)exception_0.Data["AppComponentReporting"])
            {
                return;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            ArrayList arrayLists1 = new ArrayList();
            Assembly[] assemblyArray = assemblies;
            for (int j = 0; j < (int)assemblyArray.Length; j++)
            {
                AssemblyName name = assemblyArray[j].GetName();
                AppComponent appComponent = new AppComponent()
                {
                    FullName = name.FullName,
                    Name = name.Name,
                    Version = name.Version.ToString()
                };
                arrayLists1.Add(appComponent);
            }

            if (arrayLists1.Count > 0)
            {
                object[] objArray = arrayLists1.ToArray();
                this.componentsField = new AppComponent[arrayLists1.Count];
                Array.Copy(objArray, this.componentsField, arrayLists1.Count);
            }
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.FaultMessage();
        }

        internal override void FillInProxy()
        {
            ((PreEmptive.SoS.Client.MessageProxies.FaultMessage)base.Proxy).Comment = this.Comment;
            ((PreEmptive.SoS.Client.MessageProxies.FaultMessage)base.Proxy).Contact = this.Contact;
            ((PreEmptive.SoS.Client.MessageProxies.FaultMessage)base.Proxy).Components = this.Components;
            ((PreEmptive.SoS.Client.MessageProxies.FaultMessage)base.Proxy).FaultEvent = this.FaultEvent;
            ((PreEmptive.SoS.Client.MessageProxies.FaultMessage)base.Proxy).Exceptions = this.Exceptions;
            base.FillInProxy();
        }

        private string GetSignature(MethodBase method)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (method is MethodInfo)
            {
                MethodInfo methodInfo = (MethodInfo)method;
                stringBuilder.Append(methodInfo.ReturnType.ToString());
            }

            stringBuilder.Append("(");
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < (int)parameters.Length; i++)
            {
                stringBuilder.Append(parameters[i].ToString());
                if (i < (int)parameters.Length - 1)
                {
                    stringBuilder.Append(",");
                }
            }

            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}