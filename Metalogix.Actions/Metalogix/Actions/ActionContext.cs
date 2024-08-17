using Metalogix.Explorer;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions
{
    public class ActionContext : ActionContext<IXMLAbleList, IXMLAbleList>
    {
        public ActionContext(IXMLAbleList sources, IXMLAbleList targets)
        {
            base.Sources = sources;
            base.Targets = targets;
        }

        public ActionContext(Node nodeSource, Node nodeTarget)
        {
            IEnumerable<Node> nodes;
            IEnumerable<Node> nodes1;
            if (nodeSource != null)
            {
                nodes = new Node[] { nodeSource };
            }
            else
            {
                nodes = null;
            }

            base.Sources = new NodeCollection(nodes);
            if (nodeTarget != null)
            {
                nodes1 = new Node[] { nodeTarget };
            }
            else
            {
                nodes1 = null;
            }

            base.Targets = new NodeCollection(nodes1);
        }

        public NodeCollection GetSourcesAsNodeCollection()
        {
            return XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(base.Sources);
        }

        public NodeCollection GetTargetsAsNodeCollection()
        {
            return XMLAbleListConverter.Instance.ConvertTo<NodeCollection>(base.Targets);
        }
    }
}