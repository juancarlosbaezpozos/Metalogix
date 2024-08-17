using Metalogix.Actions;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.Transformers
{
    public abstract class
        PreconfiguredTransformer<T, A, C1, C2> : Transformer<T, A, C1, C2, Metalogix.Transformers.TransformerOptions>
        where A : Metalogix.Actions.Action
        where C1 : IEnumerable
        where C2 : IEnumerable
    {
        protected PreconfiguredTransformer()
        {
        }

        public override bool Configure(Metalogix.Actions.Action action, IXMLAbleList source, IXMLAbleList target)
        {
            return true;
        }

        public override void FromXML(XmlNode node)
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
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Transformer");
            xmlWriter.WriteAttributeString("TransformerType", base.GetType().AssemblyQualifiedName);
            xmlWriter.WriteAttributeString("ReadOnly", this.ReadOnly.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}