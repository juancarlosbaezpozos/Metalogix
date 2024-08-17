using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class SingleNodeList : XmlPatchNodeList
    {
        private XmlNode _node;

        public override int Count
        {
            get { return 1; }
        }

        internal SingleNodeList()
        {
        }

        internal override void AddNode(XmlNode node)
        {
            if (this._node != null)
            {
                XmlPatchError.Error("Internal Error. XmlDiffPathSingleNodeList can contain one node only.");
            }

            this._node = node;
        }

        public override IEnumerator GetEnumerator()
        {
            return new SingleNodeList.Enumerator(this._node);
        }

        public override XmlNode Item(int index)
        {
            if (index != 0)
            {
                return null;
            }

            return this._node;
        }

        private class Enumerator : IEnumerator
        {
            private XmlNode _node;

            private SingleNodeList.Enumerator.State _state;

            public object Current
            {
                get
                {
                    if (this._state != SingleNodeList.Enumerator.State.OnNode)
                    {
                        return null;
                    }

                    return this._node;
                }
            }

            internal Enumerator(XmlNode node)
            {
                this._node = node;
            }

            public bool MoveNext()
            {
                switch (this._state)
                {
                    case SingleNodeList.Enumerator.State.BeforeNode:
                    {
                        this._state = SingleNodeList.Enumerator.State.OnNode;
                        return true;
                    }
                    case SingleNodeList.Enumerator.State.OnNode:
                    {
                        this._state = SingleNodeList.Enumerator.State.AfterNode;
                        return false;
                    }
                    case SingleNodeList.Enumerator.State.AfterNode:
                    {
                        return false;
                    }
                }

                return false;
            }

            public void Reset()
            {
                this._state = SingleNodeList.Enumerator.State.BeforeNode;
            }

            private enum State
            {
                BeforeNode,
                OnNode,
                AfterNode
            }
        }
    }
}