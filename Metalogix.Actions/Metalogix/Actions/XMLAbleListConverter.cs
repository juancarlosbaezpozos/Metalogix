using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections;
using System.Reflection;

namespace Metalogix.Actions
{
    public sealed class XMLAbleListConverter : IXMLAbleListConverter
    {
        private readonly static object _LOCK;

        private static volatile XMLAbleListConverter _instance;

        public static XMLAbleListConverter Instance
        {
            get
            {
                if (XMLAbleListConverter._instance != null)
                {
                    return XMLAbleListConverter._instance;
                }

                lock (XMLAbleListConverter._LOCK)
                {
                    if (XMLAbleListConverter._instance == null)
                    {
                        XMLAbleListConverter._instance = new XMLAbleListConverter();
                    }
                }

                return XMLAbleListConverter._instance;
            }
        }

        static XMLAbleListConverter()
        {
            XMLAbleListConverter._LOCK = new object();
        }

        private XMLAbleListConverter()
        {
        }

        public T ConvertTo<T>(IXMLAbleList list)
            where T : class
        {
            return (T)this.ConvertTo(list, typeof(T));
        }

        public object ConvertTo(IXMLAbleList list, Type targetType)
        {
            if (list == null || targetType == null)
            {
                return null;
            }

            if (list.GetType() == targetType || list.GetType().IsSubclassOf(targetType))
            {
                return list;
            }

            MethodInfo methodInfo = this.FindMethodInfo(targetType);
            if (methodInfo == null)
            {
                throw new NotSupportedException(string.Concat("'", targetType.FullName, "' is not supported."));
            }

            return methodInfo.Invoke(this, new object[] { list });
        }

        private NodeCollection ConvertToNodeCollection(IXMLAbleList list)
        {
            NodeCollection nodeCollection = new NodeCollection();
            foreach (object obj in list)
            {
                if (!(obj is Node))
                {
                    continue;
                }

                nodeCollection.Add((Node)obj);
            }

            return nodeCollection;
        }

        private MethodInfo FindMethodInfo(Type targetType)
        {
            MethodInfo method = this.GetType().GetMethod(string.Concat("ConvertTo", targetType.Name));
            return method;
        }

        public bool IsSupported(Type targetType)
        {
            return this.FindMethodInfo(targetType) != null;
        }
    }
}