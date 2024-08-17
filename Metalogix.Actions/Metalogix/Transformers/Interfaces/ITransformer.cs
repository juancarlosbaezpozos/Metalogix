using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Transformers;
using System;
using System.Xml;

namespace Metalogix.Transformers.Interfaces
{
    public interface ITransformer : IXmlable
    {
        Type ActionType { get; }

        Metalogix.Actions.Cardinality Cardinality { get; }

        Type DataObjectType { get; }

        string Name { get; }

        bool ReadOnly { get; set; }

        Type SourceCollectionType { get; }

        Type TargetCollectionType { get; }

        Metalogix.Transformers.TransformerOptions TransformerOptions { get; set; }

        ITransformer Clone();

        bool Configure(Metalogix.Actions.Action action, IXMLAbleList source, IXMLAbleList target);

        void FireOperationFinished(LogItem operation);

        void FireOperationStarted(LogItem operation);

        void FireOperationUpdated(LogItem operation);

        void FromXML(XmlNode node);

        void FromXML(string sXML);

        event ActionEventHandler OperationFinished;

        event ActionEventHandler OperationStarted;

        event ActionEventHandler OperationUpdated;
    }
}