using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class MultiNodeList : XmlPatchNodeList
    {
        private int _count = 0;

        internal MultiNodeList.ListChunk _chunks = null;

        private MultiNodeList.ListChunk _lastChunk = null;

        public override int Count
        {
            get { return this._count; }
        }

        internal MultiNodeList()
        {
        }

        internal override void AddNode(XmlNode node)
        {
            if (this._lastChunk == null)
            {
                this._chunks = new MultiNodeList.ListChunk();
                this._lastChunk = this._chunks;
            }
            else if (this._lastChunk._count == 10)
            {
                this._lastChunk._next = new MultiNodeList.ListChunk();
                this._lastChunk = this._lastChunk._next;
            }

            this._lastChunk.AddNode(node);
            this._count++;
        }

        public override IEnumerator GetEnumerator()
        {
            return new MultiNodeList.Enumerator(this);
        }

        public override XmlNode Item(int index)
        {
            if (this._chunks == null)
            {
                return null;
            }

            if (index < 10)
            {
                return this._chunks[index];
            }

            int num = index / 10;
            MultiNodeList.ListChunk listChunk = this._chunks;
            while (num > 0)
            {
                listChunk = listChunk._next;
                num--;
            }

            return listChunk[index % 10];
        }

        private class Enumerator : IEnumerator
        {
            private MultiNodeList _nodeList;

            private MultiNodeList.ListChunk _currentChunk;

            private int _currentChunkIndex;

            public object Current
            {
                get
                {
                    if (this._currentChunk == null)
                    {
                        return null;
                    }

                    return this._currentChunk[this._currentChunkIndex];
                }
            }

            internal Enumerator(MultiNodeList nodeList)
            {
                this._nodeList = nodeList;
                this._currentChunk = nodeList._chunks;
            }

            public bool MoveNext()
            {
                if (this._currentChunk == null)
                {
                    return false;
                }

                if (this._currentChunkIndex < this._currentChunk._count - 1)
                {
                    this._currentChunkIndex++;
                    return true;
                }

                if (this._currentChunk._next == null)
                {
                    return false;
                }

                this._currentChunk = this._currentChunk._next;
                this._currentChunkIndex = 0;
                return true;
            }

            public void Reset()
            {
                this._currentChunk = this._nodeList._chunks;
                this._currentChunkIndex = -1;
            }
        }

        internal class ListChunk
        {
            internal const int ChunkSize = 10;

            internal XmlNode[] _nodes;

            internal int _count;

            internal MultiNodeList.ListChunk _next;

            internal XmlNode this[int i]
            {
                get { return this._nodes[i]; }
            }

            public ListChunk()
            {
            }

            internal void AddNode(XmlNode node)
            {
                XmlNode[] xmlNodeArrays = this._nodes;
                MultiNodeList.ListChunk listChunk = this;
                int num = listChunk._count;
                int num1 = num;
                listChunk._count = num + 1;
                xmlNodeArrays[num1] = node;
            }
        }
    }
}