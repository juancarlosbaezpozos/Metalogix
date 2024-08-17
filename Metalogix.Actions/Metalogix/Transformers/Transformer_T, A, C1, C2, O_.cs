using Metalogix;
using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Transformers
{
    [TransformerCardinality(Metalogix.Actions.Cardinality.ZeroOrOne)]
    public abstract class Transformer<T, A, C1, C2, O> : IXmlable, ITransformer<T, A, C1, C2, O>, ITransformer
        where A : Metalogix.Actions.Action
        where C1 : IEnumerable
        where C2 : IEnumerable
        where O : Metalogix.Transformers.TransformerOptions
    {
        private O m_options;

        protected bool m_bReadOnly;

        private Metalogix.Actions.Cardinality? m_cardinality;

        public Type ActionType
        {
            get { return typeof(A); }
        }

        public Metalogix.Actions.Cardinality Cardinality
        {
            get
            {
                if (!this.m_cardinality.HasValue)
                {
                    object[] customAttributes =
                        this.GetType().GetCustomAttributes(typeof(TransformerCardinalityAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_cardinality = new Metalogix.Actions.Cardinality?(Metalogix.Actions.Cardinality.One);
                    }
                    else
                    {
                        this.m_cardinality =
                            new Metalogix.Actions.Cardinality?(((TransformerCardinalityAttribute)customAttributes[0])
                                .Cardinality);
                    }
                }

                return this.m_cardinality.Value;
            }
        }

        public Type DataObjectType
        {
            get { return typeof(T); }
        }

        public abstract string Name { get; }

        public O Options
        {
            get { return JustDecompileGenerated_get_Options(); }
            set { JustDecompileGenerated_set_Options(value); }
        }

        public O JustDecompileGenerated_get_Options()
        {
            if (this.m_options == null)
            {
                this.m_options = Activator.CreateInstance<O>();
            }

            return this.m_options;
        }

        public void JustDecompileGenerated_set_Options(O value)
        {
            this.m_options = value;
        }

        public virtual bool ReadOnly
        {
            get { return this.m_bReadOnly; }
            set { this.m_bReadOnly = value; }
        }

        public Type SourceCollectionType
        {
            get { return typeof(C1); }
        }

        public Type TargetCollectionType
        {
            get { return typeof(C2); }
        }

        public TransformerOptions TransformerOptions
        {
            get { return this.Options; }
            set { this.Options = (value as O); }
        }

        protected Transformer()
        {
        }

        public abstract void BeginTransformation(A action, C1 sources, C2 targets);

        public virtual ITransformer Clone()
        {
            ITransformer transformer = (ITransformer)Activator.CreateInstance(this.GetType());
            transformer.FromXML(this.ToXML());
            return transformer;
        }

        public virtual bool Configure(Metalogix.Actions.Action action, IXMLAbleList source, IXMLAbleList target)
        {
            ITransformerConfig transformerConfig = TransformerConfigurationProvider.Instance.GetTransformerConfig(this);
            if (transformerConfig == null)
            {
                return true;
            }

            return transformerConfig.Configure(new TransformerConfigContext(this, action,
                new ActionContext(source, target)));
        }

        public abstract void EndTransformation(A action, C1 sources, C2 targets);

        public void FireOperationFinished(LogItem operation)
        {
            if (this.OperationFinished != null)
            {
                this.OperationFinished(operation);
            }
        }

        public void FireOperationStarted(LogItem operation)
        {
            if (this.OperationStarted != null)
            {
                this.OperationStarted(operation);
            }
        }

        public void FireOperationUpdated(LogItem operation)
        {
            if (this.OperationUpdated != null)
            {
                this.OperationUpdated(operation);
            }
        }

        public void FromXML(string sXML)
        {
            this.FromXML(XmlUtility.StringToXmlNode(sXML));
        }

        public virtual void FromXML(XmlNode node)
        {
            if (node.Attributes["ReadOnly"] != null)
            {
                string value = node.Attributes["ReadOnly"].Value;
                bool flag = false;
                if (bool.TryParse(value, out flag))
                {
                    this.ReadOnly = flag;
                }
            }

            if (this.Options != null)
            {
                this.Options.FromXML(node);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public virtual void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Transformer");
            xmlWriter.WriteAttributeString("TransformerType", this.GetType().AssemblyQualifiedName);
            xmlWriter.WriteAttributeString("ReadOnly", this.ReadOnly.ToString());
            if (this.Options != null)
            {
                this.Options.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }

        public abstract T Transform(T dataObject, A action, C1 sources, C2 targets);

        public event ActionEventHandler OperationFinished;

        public event ActionEventHandler OperationStarted;

        public event ActionEventHandler OperationUpdated;
    }
}