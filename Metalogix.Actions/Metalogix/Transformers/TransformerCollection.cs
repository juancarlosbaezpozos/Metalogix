using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.Transformers
{
    public class TransformerCollection : SerializableList<ITransformer>
    {
        private readonly static object s_oLock;

        private static volatile TransformerCollection s_availableTransformers;

        private static volatile List<Type> s_genericTransformerTypes;

        public static List<Type> AvailableGenericTransformers
        {
            get
            {
                if (TransformerCollection.s_genericTransformerTypes == null)
                {
                    TransformerCollection.LoadTransformerTypes();
                }

                return TransformerCollection.s_genericTransformerTypes;
            }
        }

        public static TransformerCollection AvailableTransformers
        {
            get
            {
                if (TransformerCollection.s_availableTransformers == null)
                {
                    TransformerCollection.LoadTransformerTypes();
                }

                return TransformerCollection.s_availableTransformers;
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override ITransformer this[ITransformer itransformer_0]
        {
            get
            {
                ITransformer transformer;
                List<ITransformer>.Enumerator enumerator = this.m_collection.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ITransformer current = enumerator.Current;
                        if (current != itransformer_0)
                        {
                            continue;
                        }

                        transformer = current;
                        return transformer;
                    }

                    return null;
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                return transformer;
            }
        }

        static TransformerCollection()
        {
            TransformerCollection.s_oLock = new object();
            TransformerCollection.s_availableTransformers = null;
            TransformerCollection.s_genericTransformerTypes = null;
            TransformerCollection.LoadTransformerTypes();
            ConfigurationVariables.ConfigurationVariablesChanged +=
                new ConfigurationVariables.ConfigurationVariablesChangedHander(TransformerCollection
                    .EnvironmentVariables_EnvironmentVariablesChanged);
        }

        public TransformerCollection()
        {
        }

        public new TransformerCollection Clone()
        {
            TransformerCollection transformerCollection = new TransformerCollection();
            transformerCollection.FromXML(base.ToXML());
            return transformerCollection;
        }

        private static void EnvironmentVariables_EnvironmentVariablesChanged(object sender,
            ConfigurationVariables.ConfigVarsChangedArgs configVarsChangedArgs_0)
        {
            if (configVarsChangedArgs_0.VariableName == Resources.EnableCustomTransformersKey)
            {
                lock (TransformerCollection.s_oLock)
                {
                    TransformerCollection.s_availableTransformers = null;
                    TransformerCollection.s_genericTransformerTypes = null;
                }
            }
        }

        public ITransformer Find(string transformerName)
        {
            return this.m_collection.Find((ITransformer itransformer_0) =>
                string.Equals(itransformer_0.Name, transformerName));
        }

        public ITransformer Find(Type transformerType)
        {
            return this.m_collection.Find((ITransformer itransformer_0) => itransformer_0.GetType() == transformerType);
        }

        public List<ITransformer> FindAll(string transformerName)
        {
            return this.m_collection.FindAll((ITransformer itransformer_0) =>
                string.Equals(itransformer_0.Name, transformerName));
        }

        public List<ITransformer> FindAll(Type transformerType)
        {
            return this.m_collection.FindAll((ITransformer itransformer_0) =>
                itransformer_0.GetType() == transformerType);
        }

        public override void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = xmlNode.SelectSingleNode("//TransformerCollection");
            if (xmlNode.HasChildNodes && xmlNode.ChildNodes[0].HasChildNodes && xmlNodes != null &&
                !xmlNodes.HasChildNodes)
            {
                xmlNodes = xmlNode.SelectSingleNode("./TransformerCollection");
            }

            foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes("./Transformer"))
            {
                XmlAttribute itemOf = xmlNodes1.Attributes["TransformerType"];
                ITransformer transformer1 = TransformerCollection.AvailableTransformers.FirstOrDefault<ITransformer>(
                    (ITransformer transformer) =>
                        string.Equals(transformer.GetType().AssemblyQualifiedName, itemOf.Value));
                if (transformer1 == null)
                {
                    Type type = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                    if (type == null)
                    {
                        continue;
                    }

                    ITransformer transformer2 = (ITransformer)Activator.CreateInstance(type);
                    transformer2.FromXML(xmlNodes1);
                    this.Add(transformer2);
                }
                else
                {
                    ITransformer transformer3 = transformer1.Clone();
                    transformer3.FromXML(xmlNodes1);
                    this.Add(transformer3);
                }
            }
        }

        public static bool IsTransformerEnabled(ITransformer transformer)
        {
            if (transformer == null)
            {
                return false;
            }

            Type type1 = transformer.GetType();
            if (!type1.IsGenericType)
            {
                return TransformerCollection.AvailableTransformers.Any<ITransformer>((ITransformer itransformer_0) =>
                    itransformer_0.GetType() == transformer.GetType());
            }

            return TransformerCollection.AvailableGenericTransformers.Any<Type>((Type type) =>
                type1.GetGenericTypeDefinition() == type);
        }

        private static void LoadTransformerTypes()
        {
            lock (TransformerCollection.s_oLock)
            {
                if (TransformerCollection.s_availableTransformers == null)
                {
                    TransformerCollection transformerCollection = new TransformerCollection();
                    List<Type> types = new List<Type>();
                    AssemblyTiers assemblyTier = AssemblyTiers.Referenced | AssemblyTiers.Signed;
                    if (ActionConfigurationVariables.EnableCustomTransformers)
                    {
                        assemblyTier |= AssemblyTiers.Unsigned;
                    }

                    Type[] typesByInterface = Catalogs.GetTypesByInterface(typeof(ITransformer), assemblyTier);
                    for (int i = 0; i < (int)typesByInterface.Length; i++)
                    {
                        Type type = typesByInterface[i];
                        try
                        {
                            if (!type.IsGenericType)
                            {
                                ITransformer transformer = (ITransformer)Activator.CreateInstance(type);
                                if (!transformerCollection.Contains(transformer))
                                {
                                    transformerCollection.Add(transformer);
                                }
                            }
                            else
                            {
                                types.Add(type);
                            }
                        }
                        catch (Exception exception)
                        {
                        }
                    }

                    TransformerCollection.s_genericTransformerTypes = types;
                    TransformerCollection.s_availableTransformers = transformerCollection;
                }
            }
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TransformerCollection");
            foreach (ITransformer transformer in this)
            {
                transformer.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }
    }
}