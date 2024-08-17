using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    [ComVisible(true)]
    public class XmlDiff
    {
        private const int MininumNodesForQuicksort = 5;

        private const int MaxTotalNodesCountForTreeDistance = 256;

        public const string NamespaceUri = "http://schemas.microsoft.com/xmltools/2002/xmldiff";

        internal const string Prefix = "xd";

        internal const string XmlnsNamespaceUri = "http://www.w3.org/2000/xmlns/";

        private bool _bIgnoreChildOrder = false;

        private bool _bIgnoreComments = false;

        private bool _bIgnorePI = false;

        private bool _bIgnoreWhitespace = false;

        private bool _bIgnoreNamespaces = false;

        private bool _bIgnorePrefixes = false;

        private bool _bIgnoreXmlDecl = false;

        private bool _bIgnoreDtd = false;

        private XmlDiffAlgorithm _algorithm = XmlDiffAlgorithm.Auto;

        internal XmlDiffDocument _sourceDoc = null;

        internal XmlDiffDocument _targetDoc = null;

        internal XmlDiffNode[] _sourceNodes = null;

        internal XmlDiffNode[] _targetNodes = null;

        internal TriStateBool _fragments = TriStateBool.DontKnown;

        public XmlDiffPerf _xmlDiffPerf = new XmlDiffPerf();

        public XmlDiffAlgorithm Algorithm
        {
            get { return this._algorithm; }
            set { this._algorithm = value; }
        }

        public bool IgnoreChildOrder
        {
            get { return this._bIgnoreChildOrder; }
            set { this._bIgnoreChildOrder = value; }
        }

        public bool IgnoreComments
        {
            get { return this._bIgnoreComments; }
            set { this._bIgnoreComments = value; }
        }

        public bool IgnoreDtd
        {
            get { return this._bIgnoreDtd; }
            set { this._bIgnoreDtd = value; }
        }

        public bool IgnoreNamespaces
        {
            get { return this._bIgnoreNamespaces; }
            set { this._bIgnoreNamespaces = value; }
        }

        public bool IgnorePI
        {
            get { return this._bIgnorePI; }
            set { this._bIgnorePI = value; }
        }

        public bool IgnorePrefixes
        {
            get { return this._bIgnorePrefixes; }
            set { this._bIgnorePrefixes = value; }
        }

        public bool IgnoreWhitespace
        {
            get { return this._bIgnoreWhitespace; }
            set { this._bIgnoreWhitespace = value; }
        }

        public bool IgnoreXmlDecl
        {
            get { return this._bIgnoreXmlDecl; }
            set { this._bIgnoreXmlDecl = value; }
        }

        public XmlDiffOptions Options
        {
            set
            {
                this.IgnoreChildOrder = (value & XmlDiffOptions.IgnoreChildOrder) > XmlDiffOptions.None;
                this.IgnoreComments = (value & XmlDiffOptions.IgnoreComments) > XmlDiffOptions.None;
                this.IgnorePI = (value & XmlDiffOptions.IgnorePI) > XmlDiffOptions.None;
                this.IgnoreWhitespace = (value & XmlDiffOptions.IgnoreWhitespace) > XmlDiffOptions.None;
                this.IgnoreNamespaces = (value & XmlDiffOptions.IgnoreNamespaces) > XmlDiffOptions.None;
                this.IgnorePrefixes = (value & XmlDiffOptions.IgnorePrefixes) > XmlDiffOptions.None;
                this.IgnoreXmlDecl = (value & XmlDiffOptions.IgnoreXmlDecl) > XmlDiffOptions.None;
                this.IgnoreDtd = (value & XmlDiffOptions.IgnoreDtd) > XmlDiffOptions.None;
            }
        }

        public XmlDiff()
        {
        }

        public XmlDiff(XmlDiffOptions options) : this()
        {
            this.Options = options;
        }

        private void AddNodeToHashTable(Hashtable hashtable, XmlDiffNode node)
        {
            ulong hashValue = node.HashValue;
            XmlDiff.XmlDiffNodeListHead item = (XmlDiff.XmlDiffNodeListHead)hashtable[hashValue];
            if (item == null)
            {
                hashtable[hashValue] = new XmlDiff.XmlDiffNodeListHead(new XmlDiff.XmlDiffNodeListMember(node, null));
                return;
            }

            XmlDiff.XmlDiffNodeListMember xmlDiffNodeListMember = new XmlDiff.XmlDiffNodeListMember(node, null);
            item._last._next = xmlDiffNodeListMember;
            item._last = xmlDiffNodeListMember;
        }

        public bool Compare(string sourceFile, string changedFile, bool bFragments)
        {
            return this.Compare(sourceFile, changedFile, bFragments, null);
        }

        public bool Compare(string sourceFile, string changedFile, bool bFragments, XmlWriter diffgramWriter)
        {
            bool flag;
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            if (changedFile == null)
            {
                throw new ArgumentNullException("changedFile");
            }

            XmlReader xmlReader = null;
            XmlReader xmlReader1 = null;
            try
            {
                this._fragments = (bFragments ? TriStateBool.Yes : TriStateBool.No);
                if (!bFragments)
                {
                    this.OpenDocuments(sourceFile, changedFile, ref xmlReader, ref xmlReader1);
                }
                else
                {
                    this.OpenFragments(sourceFile, changedFile, ref xmlReader, ref xmlReader1);
                }

                flag = this.Compare(xmlReader, xmlReader1, diffgramWriter);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                    xmlReader = null;
                }

                if (xmlReader1 != null)
                {
                    xmlReader1.Close();
                    xmlReader1 = null;
                }
            }

            return flag;
        }

        public bool Compare(XmlReader sourceReader, XmlReader changedReader)
        {
            return this.Compare(sourceReader, changedReader, null);
        }

        public bool Compare(XmlReader sourceReader, XmlReader changedReader, XmlWriter diffgramWriter)
        {
            bool flag;
            if (sourceReader == null)
            {
                throw new ArgumentNullException("sourceReader");
            }

            if (changedReader == null)
            {
                throw new ArgumentNullException("changedReader");
            }

            try
            {
                XmlHash xmlHash = new XmlHash(this);
                this._xmlDiffPerf.Clean();
                int tickCount = Environment.TickCount;
                this._sourceDoc = new XmlDiffDocument(this);
                this._sourceDoc.Load(sourceReader, xmlHash);
                this._targetDoc = new XmlDiffDocument(this);
                this._targetDoc.Load(changedReader, xmlHash);
                if (this._fragments == TriStateBool.DontKnown)
                {
                    this._fragments = (this._sourceDoc.IsFragment || this._targetDoc.IsFragment
                        ? TriStateBool.Yes
                        : TriStateBool.No);
                }

                this._xmlDiffPerf._loadTime = Environment.TickCount - tickCount;
                flag = this.Diff(diffgramWriter);
            }
            finally
            {
                this._sourceDoc = null;
                this._targetDoc = null;
            }

            return flag;
        }

        public bool Compare(XmlNode sourceNode, XmlNode changedNode)
        {
            return this.Compare(sourceNode, changedNode, null);
        }

        public bool Compare(XmlNode sourceNode, XmlNode changedNode, XmlWriter diffgramWriter)
        {
            bool flag;
            if (sourceNode == null)
            {
                throw new ArgumentNullException("sourceNode");
            }

            if (changedNode == null)
            {
                throw new ArgumentNullException("changedNode");
            }

            try
            {
                XmlHash xmlHash = new XmlHash(this);
                this._xmlDiffPerf.Clean();
                int tickCount = Environment.TickCount;
                this._sourceDoc = new XmlDiffDocument(this);
                this._sourceDoc.Load(sourceNode, xmlHash);
                this._targetDoc = new XmlDiffDocument(this);
                this._targetDoc.Load(changedNode, xmlHash);
                this._fragments =
                    (sourceNode.NodeType != XmlNodeType.Document || changedNode.NodeType != XmlNodeType.Document
                        ? TriStateBool.Yes
                        : TriStateBool.No);
                this._xmlDiffPerf._loadTime = Environment.TickCount - tickCount;
                flag = this.Diff(diffgramWriter);
            }
            finally
            {
                this._sourceDoc = null;
                this._targetDoc = null;
            }

            return flag;
        }

        private bool CompareSubtrees(XmlDiffNode node1, XmlDiffNode node2)
        {
            XmlDiffNode i;
            if (!node1.IsSameAs(node2, this))
            {
                return false;
            }

            if (!node1.HasChildNodes)
            {
                return true;
            }

            XmlDiffNode firstChildNode = ((XmlDiffParentNode)node1).FirstChildNode;
            for (i = ((XmlDiffParentNode)node2).FirstChildNode; firstChildNode != null && i != null; i = i._nextSibling)
            {
                if (!this.CompareSubtrees(firstChildNode, i))
                {
                    return false;
                }

                firstChildNode = firstChildNode._nextSibling;
            }

            return firstChildNode == i;
        }

        private bool Diff(XmlWriter diffgramWriter)
        {
            bool flag;
            try
            {
                int tickCount = Environment.TickCount;
                if (this.IdenticalSubtrees(this._sourceDoc, this._targetDoc))
                {
                    if (diffgramWriter != null)
                    {
                        (new DiffgramGenerator(this)).GenerateEmptyDiffgram().WriteTo(diffgramWriter);
                        diffgramWriter.Flush();
                    }

                    this._xmlDiffPerf._identicalOrNoDiffWriterTime = Environment.TickCount - tickCount;
                    flag = true;
                }
                else if (diffgramWriter != null)
                {
                    this._xmlDiffPerf._identicalOrNoDiffWriterTime = Environment.TickCount - tickCount;
                    tickCount = Environment.TickCount;
                    this.MatchIdenticalSubtrees();
                    this._xmlDiffPerf._matchTime = Environment.TickCount - tickCount;
                    Diffgram diffgram = null;
                    switch (this._algorithm)
                    {
                        case XmlDiffAlgorithm.Auto:
                        {
                            if (this._sourceDoc.NodesCount + this._targetDoc.NodesCount > 256)
                            {
                                diffgram = this.WalkTreeAlgorithm();
                                break;
                            }
                            else
                            {
                                diffgram = this.ZhangShashaAlgorithm();
                                break;
                            }
                        }
                        case XmlDiffAlgorithm.Fast:
                        {
                            diffgram = this.WalkTreeAlgorithm();
                            break;
                        }
                        case XmlDiffAlgorithm.Precise:
                        {
                            diffgram = this.ZhangShashaAlgorithm();
                            break;
                        }
                    }

                    tickCount = Environment.TickCount;
                    diffgram.WriteTo(diffgramWriter);
                    diffgramWriter.Flush();
                    this._xmlDiffPerf._diffgramSaveTime = Environment.TickCount - tickCount;
                    return false;
                }
                else
                {
                    this._xmlDiffPerf._identicalOrNoDiffWriterTime = Environment.TickCount - tickCount;
                    flag = false;
                }
            }
            finally
            {
                this._sourceNodes = null;
                this._targetNodes = null;
            }

            return flag;
        }

        internal string GetXmlDiffOptionsString()
        {
            string empty = string.Empty;
            if (this._bIgnoreChildOrder)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnoreChildOrder.ToString(), " ");
            }

            if (this._bIgnoreComments)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnoreComments.ToString(), " ");
            }

            if (this._bIgnoreNamespaces)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnoreNamespaces.ToString(), " ");
            }

            if (this._bIgnorePI)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnorePI.ToString(), " ");
            }

            if (this._bIgnorePrefixes)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnorePrefixes.ToString(), " ");
            }

            if (this._bIgnoreWhitespace)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnoreWhitespace.ToString(), " ");
            }

            if (this._bIgnoreXmlDecl)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnoreXmlDecl.ToString(), " ");
            }

            if (this._bIgnoreDtd)
            {
                empty = string.Concat(empty, XmlDiffOptions.IgnoreDtd.ToString(), " ");
            }

            if (empty == string.Empty)
            {
                empty = XmlDiffOptions.None.ToString();
            }

            empty.Trim();
            return empty;
        }

        private XmlDiffNode HTFindAndRemoveMatchingNode(Hashtable hashtable, XmlDiff.XmlDiffNodeListHead nodeListHead,
            XmlDiffNode nodeToMatch)
        {
            XmlDiff.XmlDiffNodeListMember xmlDiffNodeListMember = nodeListHead._first;
            XmlDiffNode xmlDiffNode = xmlDiffNodeListMember._node;
            if (this.IdenticalSubtrees(xmlDiffNode, nodeToMatch))
            {
                if (xmlDiffNodeListMember._next != null)
                {
                    nodeListHead._first = xmlDiffNodeListMember._next;
                }
                else
                {
                    hashtable.Remove(xmlDiffNode.HashValue);
                }

                return xmlDiffNode;
            }

            while (xmlDiffNodeListMember._next != null)
            {
                if (!this.IdenticalSubtrees(xmlDiffNodeListMember._node, nodeToMatch))
                {
                    continue;
                }

                xmlDiffNodeListMember._next = xmlDiffNodeListMember._next._next;
                if (xmlDiffNodeListMember._next == null)
                {
                    nodeListHead._last = xmlDiffNodeListMember;
                }

                return xmlDiffNode;
            }

            return null;
        }

        private void HTRemoveAncestors(Hashtable hashtable, XmlDiffNode node)
        {
            for (XmlDiffNode i = node._parent; i != null; i = i._parent)
            {
                if (!this.HTRemoveNode(hashtable, i))
                {
                    return;
                }

                i._bSomeDescendantMatches = true;
            }
        }

        private void HTRemoveDescendants(Hashtable hashtable, XmlDiffNode parent)
        {
            if (!parent._bExpanded || !parent.HasChildNodes)
            {
                return;
            }

            XmlDiffNode firstChildNode = parent.FirstChildNode;
            while (true)
            {
                if (!firstChildNode._bExpanded || !firstChildNode.HasChildNodes)
                {
                    this.HTRemoveNode(hashtable, firstChildNode);
                    while (firstChildNode._nextSibling == null)
                    {
                        if (firstChildNode._parent == parent)
                        {
                            return;
                        }

                        firstChildNode = firstChildNode._parent;
                    }

                    firstChildNode = firstChildNode._nextSibling;
                }
                else
                {
                    firstChildNode = ((XmlDiffParentNode)firstChildNode)._firstChildNode;
                }
            }
        }

        private bool HTRemoveNode(Hashtable hashtable, XmlDiffNode node)
        {
            XmlDiff.XmlDiffNodeListHead item = (XmlDiff.XmlDiffNodeListHead)hashtable[node.HashValue];
            if (item == null)
            {
                return false;
            }

            XmlDiff.XmlDiffNodeListMember xmlDiffNodeListMember = item._first;
            if (xmlDiffNodeListMember._node != node)
            {
                if (xmlDiffNodeListMember._next == null)
                {
                    return false;
                }

                while (xmlDiffNodeListMember._next._node != node)
                {
                    xmlDiffNodeListMember = xmlDiffNodeListMember._next;
                    if (xmlDiffNodeListMember._next != null)
                    {
                        continue;
                    }

                    return false;
                }

                xmlDiffNodeListMember._next = xmlDiffNodeListMember._next._next;
                if (xmlDiffNodeListMember._next == null)
                {
                    item._last = xmlDiffNodeListMember;
                }
            }
            else if (xmlDiffNodeListMember._next != null)
            {
                item._first = xmlDiffNodeListMember._next;
            }
            else
            {
                hashtable.Remove(node.HashValue);
            }

            return true;
        }

        private bool IdenticalSubtrees(XmlDiffNode node1, XmlDiffNode node2)
        {
            if (node1.HashValue != node2.HashValue)
            {
                return false;
            }

            return this.CompareSubtrees(node1, node2);
        }

        internal static bool IsChangeOperation(XmlDiffOperation op)
        {
            if (op < XmlDiffOperation.ChangeElementName)
            {
                return false;
            }

            return op <= XmlDiffOperation.ChangeDTD;
        }

        internal static bool IsChangeOperationOnAttributesOnly(XmlDiffOperation op)
        {
            if (op < XmlDiffOperation.ChangeElementAttr1)
            {
                return false;
            }

            return op <= XmlDiffOperation.ChangeElementAttr3;
        }

        internal static bool IsWhitespace(char c)
        {
            if (c == ' ' || c == '\t' || c == '\n')
            {
                return true;
            }

            return c == '\r';
        }

        private void MatchIdenticalSubtrees()
        {
            Hashtable hashtables = new Hashtable(16);
            Hashtable hashtables1 = new Hashtable(16);
            Queue queues = new Queue(16);
            Queue queues1 = new Queue(16);
            queues.Enqueue(this._sourceDoc);
            queues1.Enqueue(this._targetDoc);
            while (queues.Count > 0 || queues1.Count > 0)
            {
                IEnumerator enumerator = queues.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    XmlDiffParentNode current = (XmlDiffParentNode)enumerator.Current;
                    current._bExpanded = true;
                    if (!current.HasChildNodes)
                    {
                        continue;
                    }

                    for (XmlDiffNode i = current._firstChildNode; i != null; i = i._nextSibling)
                    {
                        this.AddNodeToHashTable(hashtables, i);
                    }
                }

                int count = queues1.Count;
                for (int j = 0; j < count; j++)
                {
                    XmlDiffParentNode xmlDiffParentNode = (XmlDiffParentNode)queues1.Dequeue();
                    xmlDiffParentNode._bExpanded = true;
                    if (xmlDiffParentNode.HasChildNodes)
                    {
                        XmlDiffNode xmlDiffNode = xmlDiffParentNode._firstChildNode;
                        while (xmlDiffNode != null)
                        {
                            XmlDiffNode xmlDiffNode1 = null;
                            XmlDiff.XmlDiffNodeListHead item =
                                (XmlDiff.XmlDiffNodeListHead)hashtables[xmlDiffNode.HashValue];
                            if (item != null)
                            {
                                xmlDiffNode1 = this.HTFindAndRemoveMatchingNode(hashtables, item, xmlDiffNode);
                            }

                            if (xmlDiffNode1 == null || xmlDiffNode.NodeType < XmlDiffNodeType.None)
                            {
                                if (!xmlDiffNode.HasChildNodes)
                                {
                                    xmlDiffNode._bExpanded = true;
                                }
                                else
                                {
                                    queues1.Enqueue(xmlDiffNode);
                                }

                                this.AddNodeToHashTable(hashtables1, xmlDiffNode);
                                xmlDiffNode = xmlDiffNode._nextSibling;
                            }
                            else
                            {
                                this.HTRemoveAncestors(hashtables, xmlDiffNode1);
                                this.HTRemoveDescendants(hashtables, xmlDiffNode1);
                                this.HTRemoveAncestors(hashtables1, xmlDiffNode);
                                XmlDiffNode xmlDiffNode2 = xmlDiffNode;
                                XmlDiffNode xmlDiffNode3 = xmlDiffNode1;
                                XmlDiffNode xmlDiffNode4 = xmlDiffNode2;
                                xmlDiffNode = xmlDiffNode._nextSibling;
                                XmlDiffNode xmlDiffNode5 = xmlDiffNode1._nextSibling;
                                while (xmlDiffNode != null && xmlDiffNode5 != null &&
                                       xmlDiffNode5.NodeType != XmlDiffNodeType.ShrankNode &&
                                       this.IdenticalSubtrees(xmlDiffNode5, xmlDiffNode) &&
                                       hashtables.Contains(xmlDiffNode5.HashValue))
                                {
                                    this.HTRemoveNode(hashtables, xmlDiffNode5);
                                    this.HTRemoveDescendants(hashtables, xmlDiffNode5);
                                    xmlDiffNode3 = xmlDiffNode5;
                                    xmlDiffNode5 = xmlDiffNode5._nextSibling;
                                    xmlDiffNode4 = xmlDiffNode;
                                    xmlDiffNode = xmlDiffNode._nextSibling;
                                }

                                if (xmlDiffNode1 != xmlDiffNode3 || xmlDiffNode1.NodeType != XmlDiffNodeType.Element)
                                {
                                    this.ShrinkNodeInterval(xmlDiffNode1, xmlDiffNode3, xmlDiffNode2, xmlDiffNode4);
                                }
                                else
                                {
                                    XmlDiffElement xmlDiffElement = (XmlDiffElement)xmlDiffNode1;
                                    if (xmlDiffElement.FirstChildNode == null && xmlDiffElement._attributes == null)
                                    {
                                        continue;
                                    }

                                    this.ShrinkNodeInterval(xmlDiffNode1, xmlDiffNode3, xmlDiffNode2, xmlDiffNode4);
                                }
                            }
                        }
                    }
                }

                count = queues.Count;
                for (int k = 0; k < count; k++)
                {
                    XmlDiffParentNode xmlDiffParentNode1 = (XmlDiffParentNode)queues.Dequeue();
                    if (xmlDiffParentNode1.HasChildNodes)
                    {
                        XmlDiffNode xmlDiffNode6 = xmlDiffParentNode1._firstChildNode;
                        while (xmlDiffNode6 != null)
                        {
                            if (xmlDiffNode6 is XmlDiffShrankNode || !this.NodeInHashTable(hashtables, xmlDiffNode6))
                            {
                                xmlDiffNode6 = xmlDiffNode6._nextSibling;
                            }
                            else
                            {
                                XmlDiffNode xmlDiffNode7 = null;
                                XmlDiff.XmlDiffNodeListHead xmlDiffNodeListHead =
                                    (XmlDiff.XmlDiffNodeListHead)hashtables1[xmlDiffNode6.HashValue];
                                if (xmlDiffNodeListHead != null)
                                {
                                    xmlDiffNode7 = this.HTFindAndRemoveMatchingNode(hashtables1, xmlDiffNodeListHead,
                                        xmlDiffNode6);
                                }

                                if (xmlDiffNode7 == null || xmlDiffNode6.NodeType < XmlDiffNodeType.None)
                                {
                                    if (!xmlDiffNode6.HasChildNodes)
                                    {
                                        xmlDiffNode6._bExpanded = true;
                                    }
                                    else
                                    {
                                        queues.Enqueue(xmlDiffNode6);
                                    }

                                    xmlDiffNode6 = xmlDiffNode6._nextSibling;
                                }
                                else
                                {
                                    this.HTRemoveAncestors(hashtables1, xmlDiffNode7);
                                    this.HTRemoveDescendants(hashtables1, xmlDiffNode7);
                                    this.HTRemoveNode(hashtables, xmlDiffNode6);
                                    this.HTRemoveAncestors(hashtables, xmlDiffNode6);
                                    XmlDiffNode xmlDiffNode8 = xmlDiffNode6;
                                    XmlDiffNode xmlDiffNode9 = xmlDiffNode8;
                                    XmlDiffNode xmlDiffNode10 = xmlDiffNode7;
                                    xmlDiffNode6 = xmlDiffNode6._nextSibling;
                                    for (XmlDiffNode l = xmlDiffNode7._nextSibling;
                                         xmlDiffNode6 != null && l != null &&
                                         l.NodeType != XmlDiffNodeType.ShrankNode &&
                                         this.IdenticalSubtrees(xmlDiffNode6, l) &&
                                         hashtables.Contains(xmlDiffNode6.HashValue) &&
                                         hashtables1.Contains(l.HashValue);
                                         l = l._nextSibling)
                                    {
                                        this.HTRemoveNode(hashtables, xmlDiffNode6);
                                        this.HTRemoveDescendants(hashtables, xmlDiffNode6);
                                        this.HTRemoveNode(hashtables1, l);
                                        this.HTRemoveDescendants(hashtables1, l);
                                        xmlDiffNode9 = xmlDiffNode6;
                                        xmlDiffNode6 = xmlDiffNode6._nextSibling;
                                        xmlDiffNode10 = l;
                                    }

                                    if (xmlDiffNode8 != xmlDiffNode9 ||
                                        xmlDiffNode8.NodeType != XmlDiffNodeType.Element)
                                    {
                                        this.ShrinkNodeInterval(xmlDiffNode8, xmlDiffNode9, xmlDiffNode7,
                                            xmlDiffNode10);
                                    }
                                    else
                                    {
                                        XmlDiffElement xmlDiffElement1 = (XmlDiffElement)xmlDiffNode8;
                                        if (xmlDiffElement1.FirstChildNode == null &&
                                            xmlDiffElement1._attributes == null)
                                        {
                                            continue;
                                        }

                                        this.ShrinkNodeInterval(xmlDiffNode8, xmlDiffNode9, xmlDiffNode7,
                                            xmlDiffNode10);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool NodeInHashTable(Hashtable hashtable, XmlDiffNode node)
        {
            XmlDiff.XmlDiffNodeListHead item = (XmlDiff.XmlDiffNodeListHead)hashtable[node.HashValue];
            if (item == null)
            {
                return false;
            }

            for (XmlDiff.XmlDiffNodeListMember i = item._first; i != null; i = i._next)
            {
                if (i._node == node)
                {
                    return true;
                }
            }

            return false;
        }

        internal static string NormalizeText(string text)
        {
            char[] charArray = text.ToCharArray();
            int num = 0;
            int num1 = 0;
            while (true)
            {
                if (num1 < (int)charArray.Length)
                {
                    if (XmlDiff.IsWhitespace(text[num1]))
                    {
                        num1++;
                        continue;
                    }
                }

                while (num1 < (int)charArray.Length && !XmlDiff.IsWhitespace(text[num1]))
                {
                    int num2 = num;
                    num = num2 + 1;
                    int num3 = num1;
                    num1 = num3 + 1;
                    charArray[num2] = charArray[num3];
                }

                if (num1 >= (int)charArray.Length)
                {
                    break;
                }

                int num4 = num;
                num = num4 + 1;
                charArray[num4] = ' ';
                num1++;
            }

            if (num1 == 0)
            {
                return string.Empty;
            }

            if (XmlDiff.IsWhitespace(charArray[num1 - 1]))
            {
                num--;
            }

            return new string(charArray, 0, num);
        }

        internal static string NormalizeXmlDeclaration(string value)
        {
            value = value.Replace('\'', '\"');
            return XmlDiff.NormalizeText(value);
        }

        private void OpenDocuments(string sourceFile, string changedFile, ref XmlReader sourceReader,
            ref XmlReader changedReader)
        {
            XmlTextReader xmlTextReader = new XmlTextReader(sourceFile)
            {
                XmlResolver = null
            };
            sourceReader = xmlTextReader;
            xmlTextReader = new XmlTextReader(changedFile)
            {
                XmlResolver = null
            };
            changedReader = xmlTextReader;
        }

        private void OpenFragments(string sourceFile, string changedFile, ref XmlReader sourceReader,
            ref XmlReader changedReader)
        {
            FileStream fileStream = null;
            FileStream fileStream1 = null;
            try
            {
                XmlNameTable nameTable = new NameTable();
                XmlParserContext xmlParserContext = new XmlParserContext(nameTable, new XmlNamespaceManager(nameTable),
                    string.Empty, XmlSpace.Default);
                XmlParserContext xmlParserContext1 = new XmlParserContext(nameTable, new XmlNamespaceManager(nameTable),
                    string.Empty, XmlSpace.Default);
                fileStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
                fileStream1 = new FileStream(changedFile, FileMode.Open, FileAccess.Read);
                XmlTextReader xmlTextReader = new XmlTextReader(fileStream, XmlNodeType.Element, xmlParserContext)
                {
                    XmlResolver = null
                };
                sourceReader = xmlTextReader;
                xmlTextReader = new XmlTextReader(fileStream1, XmlNodeType.Element, xmlParserContext1)
                {
                    XmlResolver = null
                };
                changedReader = xmlTextReader;
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                if (fileStream1 != null)
                {
                    fileStream1.Close();
                }

                throw;
            }
        }

        public static XmlDiffOptions ParseOptions(string options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options == XmlDiffOptions.None.ToString())
            {
                return XmlDiffOptions.None;
            }

            XmlDiffOptions xmlDiffOptions = XmlDiffOptions.None;
            int num = 0;
            int i = 0;
            while (i < options.Length)
            {
                num = options.IndexOf(' ', i);
                if (num == -1)
                {
                    num = options.Length;
                }

                string text = options.Substring(i, num - i);
                string text2;
                if ((text2 = text) != null)
                {
                    text2 = string.IsInterned(text2);
                    if (text2 != "IgnoreChildOrder")
                    {
                        if (text2 != "IgnoreComments")
                        {
                            if (text2 != "IgnoreNamespaces")
                            {
                                if (text2 != "IgnorePI")
                                {
                                    if (text2 != "IgnorePrefixes")
                                    {
                                        if (text2 != "IgnoreWhitespace")
                                        {
                                            if (text2 != "IgnoreXmlDecl")
                                            {
                                                if (text2 != "IgnoreDtd")
                                                {
                                                    goto IL_14A;
                                                    //throw new ArgumentException("options");
                                                }

                                                xmlDiffOptions |= XmlDiffOptions.IgnoreDtd;
                                            }
                                            else
                                            {
                                                xmlDiffOptions |= XmlDiffOptions.IgnoreXmlDecl;
                                            }
                                        }
                                        else
                                        {
                                            xmlDiffOptions |= XmlDiffOptions.IgnoreWhitespace;
                                        }
                                    }
                                    else
                                    {
                                        xmlDiffOptions |= XmlDiffOptions.IgnorePrefixes;
                                    }
                                }
                                else
                                {
                                    xmlDiffOptions |= XmlDiffOptions.IgnorePI;
                                }
                            }
                            else
                            {
                                xmlDiffOptions |= XmlDiffOptions.IgnoreNamespaces;
                            }
                        }
                        else
                        {
                            xmlDiffOptions |= XmlDiffOptions.IgnoreComments;
                        }
                    }
                    else
                    {
                        xmlDiffOptions |= XmlDiffOptions.IgnoreChildOrder;
                    }

                    i = num + 1;
                    continue;
                }

                IL_14A:
                throw new ArgumentException("options");
            }

            return xmlDiffOptions;
        }


        private void PreprocessNode(XmlDiffNode node, ref XmlDiffNode[] postOrderArray, ref int currentIndex)
        {
            if (!node.HasChildNodes)
            {
                node.Left = currentIndex;
                node.NodesCount = 1;
            }
            else
            {
                XmlDiffNode firstChildNode = node.FirstChildNode;
                firstChildNode._bKeyRoot = false;
                while (true)
                {
                    this.PreprocessNode(firstChildNode, ref postOrderArray, ref currentIndex);
                    firstChildNode = firstChildNode._nextSibling;
                    if (firstChildNode == null)
                    {
                        break;
                    }

                    firstChildNode._bKeyRoot = true;
                }

                node.Left = node.FirstChildNode.Left;
            }

            int num = currentIndex;
            int num1 = num;
            currentIndex = num + 1;
            postOrderArray[num1] = node;
        }

        private void PreprocessTree(XmlDiffDocument doc, ref XmlDiffNode[] postOrderArray)
        {
            postOrderArray = new XmlDiffNode[checked((uint)(doc.NodesCount + 1))];
            postOrderArray[0] = null;
            int num = 1;
            this.PreprocessNode(doc, ref postOrderArray, ref num);
            doc._bKeyRoot = true;
        }

        private static void QuickSortNodes(ref XmlDiffNode firstNode, ref XmlDiffNode lastNode, int count,
            XmlDiffNode firstPreviousSibbling, XmlDiffNode lastNextSibling)
        {
            XmlDiffNode[] xmlDiffNodeArray = new XmlDiffNode[checked((uint)count)];
            XmlDiffNode xmlDiffNode = firstNode;
            int num = 0;
            while (num < count)
            {
                xmlDiffNodeArray[num] = xmlDiffNode;
                num++;
                xmlDiffNode = xmlDiffNode._nextSibling;
            }

            XmlDiff.QuickSortNodesRecursion(ref xmlDiffNodeArray, 0, count - 1);
            for (int i = 0; i < count - 1; i++)
            {
                xmlDiffNodeArray[i]._nextSibling = xmlDiffNodeArray[i + 1];
            }

            if (firstPreviousSibbling != null)
            {
                firstPreviousSibbling._nextSibling = xmlDiffNodeArray[0];
            }
            else
            {
                firstNode._parent._firstChildNode = xmlDiffNodeArray[0];
            }

            xmlDiffNodeArray[count - 1]._nextSibling = lastNextSibling;
            firstNode = xmlDiffNodeArray[0];
            lastNode = xmlDiffNodeArray[count - 1];
        }

        private static void QuickSortNodesRecursion(ref XmlDiffNode[] sortArray, int firstIndex, int lastIndex)
        {
            int position = sortArray[(firstIndex + lastIndex) / 2].Position;
            int num = firstIndex;
            int num1 = lastIndex;
            while (num < num1)
            {
                while (sortArray[num].Position < position)
                {
                    num++;
                }

                while (sortArray[num1].Position > position)
                {
                    num1--;
                }

                if (num >= num1)
                {
                    if (num != num1)
                    {
                        continue;
                    }

                    num++;
                    num1--;
                }
                else
                {
                    XmlDiffNode xmlDiffNode = sortArray[num];
                    sortArray[num] = sortArray[num1];
                    sortArray[num1] = xmlDiffNode;
                    num++;
                    num1--;
                }
            }

            if (firstIndex < num1)
            {
                XmlDiff.QuickSortNodesRecursion(ref sortArray, firstIndex, num1);
            }

            if (num < lastIndex)
            {
                XmlDiff.QuickSortNodesRecursion(ref sortArray, num, lastIndex);
            }
        }

        private void RemoveDescendantsFromHashTable(Hashtable hashtable, XmlDiffNode parentNode)
        {
        }

        private XmlDiffShrankNode ReplaceNodeIntervalWithShrankNode(XmlDiffNode firstNode, XmlDiffNode lastNode,
            XmlDiffNode previousSibling)
        {
            XmlDiffNode xmlDiffNode;
            XmlDiffShrankNode xmlDiffShrankNode = new XmlDiffShrankNode(firstNode, lastNode);
            XmlDiffParentNode xmlDiffParentNode = firstNode._parent;
            if (previousSibling == null && firstNode != xmlDiffParentNode._firstChildNode)
            {
                previousSibling = xmlDiffParentNode._firstChildNode;
                while (previousSibling._nextSibling != firstNode)
                {
                    previousSibling = previousSibling._nextSibling;
                }
            }

            if (previousSibling != null)
            {
                xmlDiffShrankNode._nextSibling = previousSibling._nextSibling;
                previousSibling._nextSibling = xmlDiffShrankNode;
            }
            else
            {
                xmlDiffShrankNode._nextSibling = xmlDiffParentNode._firstChildNode;
                xmlDiffParentNode._firstChildNode = xmlDiffShrankNode;
            }

            xmlDiffShrankNode._parent = xmlDiffParentNode;
            int nodesCount = 0;
            do
            {
                xmlDiffNode = xmlDiffShrankNode._nextSibling;
                nodesCount += xmlDiffNode.NodesCount;
                xmlDiffShrankNode._nextSibling = xmlDiffShrankNode._nextSibling._nextSibling;
            } while (xmlDiffNode != lastNode);

            if (nodesCount > 1)
            {
                nodesCount--;
                while (xmlDiffParentNode != null)
                {
                    XmlDiffParentNode nodesCount1 = xmlDiffParentNode;
                    nodesCount1.NodesCount = nodesCount1.NodesCount - nodesCount;
                    xmlDiffParentNode = xmlDiffParentNode._parent;
                }
            }

            return xmlDiffShrankNode;
        }

        private void ShrinkNodeInterval(XmlDiffNode firstSourceNode, XmlDiffNode lastSourceNode,
            XmlDiffNode firstTargetNode, XmlDiffNode lastTargetNode)
        {
            XmlDiffNode xmlDiffNode = null;
            XmlDiffNode xmlDiffNode1 = null;
            if (this.IgnoreChildOrder && firstSourceNode != lastSourceNode)
            {
                XmlDiff.SortNodesByPosition(ref firstSourceNode, ref lastSourceNode, ref xmlDiffNode);
                XmlDiff.SortNodesByPosition(ref firstTargetNode, ref lastTargetNode, ref xmlDiffNode1);
            }

            XmlDiffShrankNode xmlDiffShrankNode =
                this.ReplaceNodeIntervalWithShrankNode(firstSourceNode, lastSourceNode, xmlDiffNode);
            XmlDiffShrankNode xmlDiffShrankNode1 =
                this.ReplaceNodeIntervalWithShrankNode(firstTargetNode, lastTargetNode, xmlDiffNode1);
            xmlDiffShrankNode.MatchingShrankNode = xmlDiffShrankNode1;
            xmlDiffShrankNode1.MatchingShrankNode = xmlDiffShrankNode;
        }

        private static void SlowSortNodes(ref XmlDiffNode firstNode, ref XmlDiffNode lastNode,
            XmlDiffNode firstPreviousSibbling, XmlDiffNode lastNextSibling)
        {
            XmlDiffNode xmlDiffNode = firstNode;
            XmlDiffNode xmlDiffNode1 = firstNode;
            XmlDiffNode xmlDiffNode2 = firstNode._nextSibling;
            xmlDiffNode1._nextSibling = null;
            while (xmlDiffNode2 != null)
            {
                XmlDiffNode xmlDiffNode3 = xmlDiffNode;
                if (xmlDiffNode2.Position >= xmlDiffNode.Position)
                {
                    while (xmlDiffNode3._nextSibling != null &&
                           xmlDiffNode2.Position > xmlDiffNode3._nextSibling.Position)
                    {
                        xmlDiffNode3 = xmlDiffNode3._nextSibling;
                    }

                    XmlDiffNode xmlDiffNode4 = xmlDiffNode2._nextSibling;
                    if (xmlDiffNode3._nextSibling == null)
                    {
                        xmlDiffNode1 = xmlDiffNode2;
                    }

                    xmlDiffNode2._nextSibling = xmlDiffNode3._nextSibling;
                    xmlDiffNode3._nextSibling = xmlDiffNode2;
                    xmlDiffNode2 = xmlDiffNode4;
                }
                else
                {
                    XmlDiffNode xmlDiffNode5 = xmlDiffNode2._nextSibling;
                    xmlDiffNode2._nextSibling = xmlDiffNode;
                    xmlDiffNode = xmlDiffNode2;
                    xmlDiffNode2 = xmlDiffNode5;
                }
            }

            if (firstPreviousSibbling != null)
            {
                firstPreviousSibbling._nextSibling = xmlDiffNode;
            }
            else
            {
                firstNode._parent._firstChildNode = xmlDiffNode;
            }

            xmlDiffNode1._nextSibling = lastNextSibling;
            firstNode = xmlDiffNode;
            lastNode = xmlDiffNode1;
        }

        internal static void SortNodesByPosition(ref XmlDiffNode firstNode, ref XmlDiffNode lastNode,
            ref XmlDiffNode firstPreviousSibbling)
        {
            XmlDiffParentNode xmlDiffParentNode = firstNode._parent;
            if (firstPreviousSibbling == null && firstNode != xmlDiffParentNode._firstChildNode)
            {
                firstPreviousSibbling = xmlDiffParentNode._firstChildNode;
                while (firstPreviousSibbling._nextSibling != firstNode)
                {
                    firstPreviousSibbling = firstPreviousSibbling._nextSibling;
                }
            }

            XmlDiffNode xmlDiffNode = lastNode._nextSibling;
            lastNode._nextSibling = null;
            int num = 0;
            for (XmlDiffNode i = firstNode; i != null; i = i._nextSibling)
            {
                num++;
            }

            if (num < 5)
            {
                XmlDiff.SlowSortNodes(ref firstNode, ref lastNode, firstPreviousSibbling, xmlDiffNode);
                return;
            }

            XmlDiff.QuickSortNodes(ref firstNode, ref lastNode, num, firstPreviousSibbling, xmlDiffNode);
        }

        public static bool VerifySource(XmlNode node, ulong hashValue, XmlDiffOptions options)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return hashValue == (new XmlHash()).ComputeHash(node, options);
        }

        private Diffgram WalkTreeAlgorithm()
        {
            return (new DiffgramGenerator(this)).GenerateFromWalkTree();
        }

        private Diffgram ZhangShashaAlgorithm()
        {
            int tickCount = Environment.TickCount;
            this.PreprocessTree(this._sourceDoc, ref this._sourceNodes);
            this.PreprocessTree(this._targetDoc, ref this._targetNodes);
            this._xmlDiffPerf._preprocessTime = Environment.TickCount - tickCount;
            tickCount = Environment.TickCount;
            EditScript editScript = (new MinimalTreeDistanceAlgo(this)).FindMinimalDistance();
            this._xmlDiffPerf._treeDistanceTime = Environment.TickCount - tickCount;
            tickCount = Environment.TickCount;
            Diffgram diffgram = (new DiffgramGenerator(this)).GenerateFromEditScript(editScript);
            this._xmlDiffPerf._diffgramGenerationTime = Environment.TickCount - tickCount;
            return diffgram;
        }

        private class XmlDiffNodeListHead
        {
            internal XmlDiff.XmlDiffNodeListMember _first;

            internal XmlDiff.XmlDiffNodeListMember _last;

            internal XmlDiffNodeListHead(XmlDiff.XmlDiffNodeListMember firstMember)
            {
                this._first = firstMember;
                this._last = firstMember;
            }
        }

        private class XmlDiffNodeListMember
        {
            internal XmlDiffNode _node;

            internal XmlDiff.XmlDiffNodeListMember _next;

            internal XmlDiffNodeListMember(XmlDiffNode node, XmlDiff.XmlDiffNodeListMember next)
            {
                this._node = node;
                this._next = next;
            }
        }
    }
}