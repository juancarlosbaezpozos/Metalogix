using System;
using System.Collections;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramGenerator
    {
        internal const int MoveHashtableInitialSize = 8;

        private const int LogMultiplier = 4;

        private XmlDiff _xmlDiff;

        private bool _bChildOrderSignificant;

        private ulong _lastOperationID;

        internal Hashtable _moveDescriptors = new Hashtable(8);

        private DiffgramGenerator.PrefixChange _prefixChangeDescr = null;

        private DiffgramGenerator.NamespaceChange _namespaceChangeDescr = null;

        private EditScript _editScript;

        private XmlDiffNode[] _sourceNodes;

        private XmlDiffNode[] _targetNodes;

        private int _curSourceIndex;

        private int _curTargetIndex;

        private DiffgramGenerator.PostponedEditScriptInfo _postponedEditScript;

        private bool _bBuildingAddTree = false;

        private DiffgramPosition _cachedDiffgramPosition = new DiffgramPosition(null);

        internal DiffgramGenerator(XmlDiff xmlDiff)
        {
            this._xmlDiff = xmlDiff;
            this._bChildOrderSignificant = !xmlDiff.IgnoreChildOrder;
            this._lastOperationID = (ulong)0;
        }

        private void AppendDescriptors(Diffgram diffgram)
        {
            IDictionaryEnumerator enumerator = this._moveDescriptors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                diffgram.AddDescriptor(new OperationDescrMove((ulong)((ulong)enumerator.Value)));
            }

            for (DiffgramGenerator.NamespaceChange i = this._namespaceChangeDescr; i != null; i = i._next)
            {
                diffgram.AddDescriptor(new OperationDescrNamespaceChange(i));
            }

            for (DiffgramGenerator.PrefixChange j = this._prefixChangeDescr; j != null; j = j._next)
            {
                diffgram.AddDescriptor(new OperationDescrPrefixChange(j));
            }
        }

        private void GenerateAddDiffgramForAttributes(DiffgramParentOperation diffgramParent,
            XmlDiffElement targetElement)
        {
            for (XmlDiffAttributeOrNamespace i = targetElement._attributes;
                 i != null;
                 i = (XmlDiffAttributeOrNamespace)i._nextSibling)
            {
                diffgramParent.InsertAtBeginning(new DiffgramAddNode(i, (ulong)0));
            }
        }

        // Microsoft.XmlDiffPatch.DiffgramGenerator
        private void GenerateChangeDiffgramForAttributes(DiffgramParentOperation diffgramParent,
            XmlDiffElement sourceElement, XmlDiffElement targetElement)
        {
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace = sourceElement._attributes;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace2 = targetElement._attributes;
            while (xmlDiffAttributeOrNamespace != null)
            {
                if (xmlDiffAttributeOrNamespace2 == null)
                {
                    break;
                }

                ulong operationID = 0uL;
                if (xmlDiffAttributeOrNamespace.NodeType == xmlDiffAttributeOrNamespace2.NodeType)
                {
                    int num;
                    if (xmlDiffAttributeOrNamespace.NodeType == XmlDiffNodeType.Attribute)
                    {
                        if ((num = XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.LocalName,
                                xmlDiffAttributeOrNamespace2.LocalName)) != 0)
                        {
                            goto IL_1CA;
                        }

                        if (this._xmlDiff.IgnoreNamespaces)
                        {
                            if (XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.Value,
                                    xmlDiffAttributeOrNamespace2.Value) == 0)
                            {
                                goto IL_1AD;
                            }
                        }
                        else if (XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.NamespaceURI,
                                     xmlDiffAttributeOrNamespace2.NamespaceURI) == 0 &&
                                 (this._xmlDiff.IgnorePrefixes ||
                                  XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.Prefix,
                                      xmlDiffAttributeOrNamespace2.Prefix) == 0) &&
                                 XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.Value,
                                     xmlDiffAttributeOrNamespace2.Value) == 0)
                        {
                            goto IL_1AD;
                        }

                        diffgramParent.InsertAtBeginning(new DiffgramChangeNode(xmlDiffAttributeOrNamespace,
                            xmlDiffAttributeOrNamespace2, XmlDiffOperation.ChangeAttr, 0uL));
                    }
                    else
                    {
                        if (!this._xmlDiff.IgnorePrefixes)
                        {
                            if (XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.Prefix,
                                    xmlDiffAttributeOrNamespace2.Prefix) == 0)
                            {
                                if (XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.NamespaceURI,
                                        xmlDiffAttributeOrNamespace2.NamespaceURI) == 0)
                                {
                                    goto IL_1AD;
                                }

                                operationID = this.GetNamespaceChangeOpid(xmlDiffAttributeOrNamespace.NamespaceURI,
                                    xmlDiffAttributeOrNamespace.Prefix, xmlDiffAttributeOrNamespace2.NamespaceURI,
                                    xmlDiffAttributeOrNamespace2.Prefix);
                            }
                            else
                            {
                                if ((num = XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.NamespaceURI,
                                        xmlDiffAttributeOrNamespace2.NamespaceURI)) != 0)
                                {
                                    goto IL_1CA;
                                }

                                operationID = this.GetNamespaceChangeOpid(xmlDiffAttributeOrNamespace.NamespaceURI,
                                    xmlDiffAttributeOrNamespace.Prefix, xmlDiffAttributeOrNamespace2.NamespaceURI,
                                    xmlDiffAttributeOrNamespace2.Prefix);
                            }

                            if (!diffgramParent.MergeRemoveAttributeAtBeginning(xmlDiffAttributeOrNamespace))
                            {
                                diffgramParent.InsertAtBeginning(new DiffgramRemoveNode(xmlDiffAttributeOrNamespace,
                                    true, operationID));
                            }

                            xmlDiffAttributeOrNamespace =
                                (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                            diffgramParent.InsertAtBeginning(new DiffgramAddNode(xmlDiffAttributeOrNamespace2,
                                operationID));
                            xmlDiffAttributeOrNamespace2 =
                                (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace2._nextSibling;
                            continue;
                        }

                        if ((num = XmlDiffDocument.OrderStrings(xmlDiffAttributeOrNamespace.NamespaceURI,
                                xmlDiffAttributeOrNamespace2.NamespaceURI)) != 0)
                        {
                            goto IL_1CA;
                        }
                    }

                    IL_1AD:
                    xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                    xmlDiffAttributeOrNamespace2 =
                        (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace2._nextSibling;
                    continue;
                    IL_1CA:
                    if (num != -1)
                    {
                        goto IL_245;
                    }
                }
                else if (xmlDiffAttributeOrNamespace.NodeType != XmlDiffNodeType.Namespace)
                {
                    goto IL_245;
                }

                if (!diffgramParent.MergeRemoveAttributeAtBeginning(xmlDiffAttributeOrNamespace))
                {
                    diffgramParent.InsertAtBeginning(new DiffgramRemoveNode(xmlDiffAttributeOrNamespace, true,
                        operationID));
                }

                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                continue;
                IL_245:
                diffgramParent.InsertAtBeginning(new DiffgramAddNode(xmlDiffAttributeOrNamespace2, operationID));
                xmlDiffAttributeOrNamespace2 = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace2._nextSibling;
            }

            while (xmlDiffAttributeOrNamespace != null)
            {
                if (!diffgramParent.MergeRemoveAttributeAtBeginning(xmlDiffAttributeOrNamespace))
                {
                    diffgramParent.InsertAtBeginning(new DiffgramRemoveNode(xmlDiffAttributeOrNamespace, true, 0uL));
                }

                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
            }

            while (xmlDiffAttributeOrNamespace2 != null)
            {
                diffgramParent.InsertAtBeginning(new DiffgramAddNode(xmlDiffAttributeOrNamespace2, 0uL));
                xmlDiffAttributeOrNamespace2 = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace2._nextSibling;
            }
        }

        private void GenerateDiffgramAdd(DiffgramParentOperation parent, int sourceBorderIndex, int targetBorderIndex)
        {
            while (this._curTargetIndex >= targetBorderIndex)
            {
                switch (this._editScript.Operation)
                {
                    case EditScriptOperation.Match:
                    {
                        this.OnMatch(parent, false);
                        continue;
                    }
                    case EditScriptOperation.Add:
                    {
                        this.OnAdd(parent, sourceBorderIndex, targetBorderIndex);
                        continue;
                    }
                    case EditScriptOperation.Remove:
                    {
                        this.OnRemove(parent);
                        continue;
                    }
                    case EditScriptOperation.ChangeNode:
                    {
                        this.OnChange(parent);
                        continue;
                    }
                    case EditScriptOperation.EditScriptPostponed:
                    {
                        this.OnEditScriptPostponed(parent, targetBorderIndex);
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
            }
        }

        private DiffgramOperation GenerateDiffgramAddWhenDescendantMatches(XmlDiffNode targetParent)
        {
            DiffgramParentOperation diffgramAddNode = new DiffgramAddNode(targetParent, (ulong)0);
            if (targetParent.NodeType == XmlDiffNodeType.Element)
            {
                for (XmlDiffAttributeOrNamespace i = ((XmlDiffElement)targetParent)._attributes;
                     i != null;
                     i = (XmlDiffAttributeOrNamespace)i._nextSibling)
                {
                    diffgramAddNode.InsertAtEnd(new DiffgramAddNode(i, (ulong)0));
                }
            }

            for (XmlDiffNode j = ((XmlDiffParentNode)targetParent)._firstChildNode; j != null; j = j._nextSibling)
            {
                if (j.NodeType == XmlDiffNodeType.ShrankNode)
                {
                    XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode)j;
                    if (xmlDiffShrankNode.MoveOperationId == (long)0)
                    {
                        xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                    }

                    diffgramAddNode.InsertAtEnd(new DiffgramCopy(xmlDiffShrankNode.MatchingShrankNode, true,
                        xmlDiffShrankNode.MoveOperationId));
                }
                else if (j.HasChildNodes && j._bSomeDescendantMatches)
                {
                    diffgramAddNode.InsertAtEnd(this.GenerateDiffgramAddWhenDescendantMatches((XmlDiffParentNode)j));
                }
                else if (!diffgramAddNode.MergeAddSubtreeAtEnd(j))
                {
                    diffgramAddNode.InsertAtEnd(new DiffgramAddSubtrees(j, (ulong)0, !this._xmlDiff.IgnoreChildOrder));
                }
            }

            return diffgramAddNode;
        }

        private void GenerateDiffgramMatch(DiffgramParentOperation parent, int sourceBorderIndex, int targetBorderIndex)
        {
            bool flag = false;
            while (this._curSourceIndex >= sourceBorderIndex || this._curTargetIndex >= targetBorderIndex)
            {
                switch (this._editScript.Operation)
                {
                    case EditScriptOperation.Match:
                    {
                        this.OnMatch(parent, flag);
                        flag = false;
                        continue;
                    }
                    case EditScriptOperation.Add:
                    {
                        flag = this.OnAdd(parent, sourceBorderIndex, targetBorderIndex);
                        continue;
                    }
                    case EditScriptOperation.Remove:
                    {
                        if (this._curSourceIndex < sourceBorderIndex)
                        {
                            return;
                        }

                        this.OnRemove(parent);
                        continue;
                    }
                    case EditScriptOperation.ChangeNode:
                    {
                        if (this._curSourceIndex < sourceBorderIndex)
                        {
                            return;
                        }

                        this.OnChange(parent);
                        continue;
                    }
                    case EditScriptOperation.EditScriptPostponed:
                    {
                        if (this._curSourceIndex < sourceBorderIndex)
                        {
                            return;
                        }

                        this.OnEditScriptPostponed(parent, targetBorderIndex);
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
            }
        }

        private void GenerateDiffgramPostponed(DiffgramParentOperation parent, ref EditScript editScript,
            int sourceBorderIndex, int targetBorderIndex)
        {
            while (this._curSourceIndex >= sourceBorderIndex && editScript != null)
            {
                EditScriptPostponed editScriptPostponed = editScript as EditScriptPostponed;
                if (editScriptPostponed == null)
                {
                    this.GenerateDiffgramMatch(parent, sourceBorderIndex, targetBorderIndex);
                    return;
                }

                int num = editScriptPostponed._startSourceIndex;
                int left = this._sourceNodes[editScriptPostponed._endSourceIndex].Left;
                DiffgramOperation diffgramOperation = editScriptPostponed._diffOperation;
                this._curSourceIndex = editScriptPostponed._startSourceIndex - 1;
                editScript = editScriptPostponed._nextEditScript;
                if (num > left)
                {
                    this.GenerateDiffgramPostponed((DiffgramParentOperation)diffgramOperation, ref editScript, left,
                        targetBorderIndex);
                }

                parent.InsertAtBeginning(diffgramOperation);
            }
        }

        private DiffgramOperation GenerateDiffgramRemoveWhenDescendantMatches(XmlDiffNode sourceParent)
        {
            DiffgramParentOperation diffgramRemoveNode = new DiffgramRemoveNode(sourceParent, false, (ulong)0);
            for (XmlDiffNode i = ((XmlDiffParentNode)sourceParent)._firstChildNode; i != null; i = i._nextSibling)
            {
                if (i.NodeType == XmlDiffNodeType.ShrankNode)
                {
                    XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode)i;
                    if (xmlDiffShrankNode.MoveOperationId == (long)0)
                    {
                        xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                    }

                    diffgramRemoveNode.InsertAtEnd(new DiffgramRemoveSubtrees(i, xmlDiffShrankNode.MoveOperationId,
                        !this._xmlDiff.IgnoreChildOrder));
                }
                else if (i.HasChildNodes && i._bSomeDescendantMatches)
                {
                    diffgramRemoveNode.InsertAtEnd(
                        this.GenerateDiffgramRemoveWhenDescendantMatches((XmlDiffParentNode)i));
                }
                else if (!diffgramRemoveNode.MergeRemoveSubtreeAtEnd(i))
                {
                    diffgramRemoveNode.InsertAtEnd(new DiffgramRemoveSubtrees(i, (ulong)0,
                        !this._xmlDiff.IgnoreChildOrder));
                }
            }

            return diffgramRemoveNode;
        }

        internal Diffgram GenerateEmptyDiffgram()
        {
            return new Diffgram(this._xmlDiff);
        }

        internal Diffgram GenerateFromEditScript(EditScript editScript)
        {
            this._sourceNodes = this._xmlDiff._sourceNodes;
            this._targetNodes = this._xmlDiff._targetNodes;
            Diffgram diffgram = new Diffgram(this._xmlDiff);
            EditScriptMatch editScriptMatch = editScript as EditScriptMatch;
            if (editScript.Operation == EditScriptOperation.Match &&
                editScriptMatch._firstSourceIndex + editScriptMatch._length == (int)this._sourceNodes.Length &&
                editScriptMatch._firstTargetIndex + editScriptMatch._length == (int)this._targetNodes.Length)
            {
                editScriptMatch._length--;
                if (editScriptMatch._length == 0)
                {
                    editScript = editScriptMatch._nextEditScript;
                }
            }

            this._curSourceIndex = (int)this._sourceNodes.Length - 2;
            this._curTargetIndex = (int)this._targetNodes.Length - 2;
            this._editScript = editScript;
            this.GenerateDiffgramMatch(diffgram, 1, 1);
            this.AppendDescriptors(diffgram);
            return diffgram;
        }

        internal Diffgram GenerateFromWalkTree()
        {
            Diffgram diffgram = new Diffgram(this._xmlDiff);
            this.WalkTreeGenerateDiffgramMatch(diffgram, this._xmlDiff._sourceDoc, this._xmlDiff._targetDoc);
            this.AppendDescriptors(diffgram);
            return diffgram;
        }

        private ulong GenerateOperationID(XmlDiffDescriptorType descriptorType)
        {
            DiffgramGenerator diffgramGenerator = this;
            ulong num = diffgramGenerator._lastOperationID + (long)1;
            ulong num1 = num;
            diffgramGenerator._lastOperationID = num;
            ulong num2 = num1;
            if (descriptorType == XmlDiffDescriptorType.Move)
            {
                this._moveDescriptors.Add(num2, num2);
            }

            return num2;
        }

        private ulong GetNamespaceChangeOpid(string oldNamespaceURI, string oldPrefix, string newNamespaceURI,
            string newPrefix)
        {
            ulong num = (ulong)0;
            if (oldNamespaceURI != newNamespaceURI)
            {
                if (oldPrefix != newPrefix)
                {
                    return (ulong)0;
                }

                for (DiffgramGenerator.NamespaceChange i = this._namespaceChangeDescr; i != null; i = i._next)
                {
                    if (i._oldNS == oldNamespaceURI && i._prefix == oldPrefix && i._newNS == newNamespaceURI)
                    {
                        return i._opid;
                    }
                }

                num = this.GenerateOperationID(XmlDiffDescriptorType.NamespaceChange);
                this._namespaceChangeDescr = new DiffgramGenerator.NamespaceChange(oldPrefix, oldNamespaceURI,
                    newNamespaceURI, num, this._namespaceChangeDescr);
            }
            else if (!this._xmlDiff.IgnorePrefixes && oldPrefix != newPrefix)
            {
                for (DiffgramGenerator.PrefixChange j = this._prefixChangeDescr; j != null; j = j._next)
                {
                    if (j._NS == oldNamespaceURI && j._oldPrefix == oldPrefix && j._newPrefix == newPrefix)
                    {
                        return j._opid;
                    }
                }

                num = this.GenerateOperationID(XmlDiffDescriptorType.PrefixChange);
                this._prefixChangeDescr = new DiffgramGenerator.PrefixChange(oldPrefix, newPrefix, oldNamespaceURI, num,
                    this._prefixChangeDescr);
            }

            return num;
        }

        private bool GoForElementChange(XmlDiffElement sourceElement, XmlDiffElement targetElement)
        {
            int num = 0;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            bool localName = sourceElement.LocalName != targetElement.LocalName;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace = sourceElement._attributes;
            XmlDiffAttributeOrNamespace xmlDiffAttributeOrNamespace1 = targetElement._attributes;
            while (xmlDiffAttributeOrNamespace != null)
            {
                if (xmlDiffAttributeOrNamespace1 == null)
                {
                    break;
                }
                else if (xmlDiffAttributeOrNamespace.LocalName == xmlDiffAttributeOrNamespace1.LocalName)
                {
                    if (!this._xmlDiff.IgnorePrefixes && !this._xmlDiff.IgnoreNamespaces &&
                        xmlDiffAttributeOrNamespace.Prefix != xmlDiffAttributeOrNamespace1.Prefix ||
                        !this._xmlDiff.IgnoreNamespaces && xmlDiffAttributeOrNamespace.NamespaceURI !=
                        xmlDiffAttributeOrNamespace1.NamespaceURI)
                    {
                        num3++;
                    }
                    else if (xmlDiffAttributeOrNamespace.Value != xmlDiffAttributeOrNamespace1.Value)
                    {
                        num3++;
                    }
                    else
                    {
                        num++;
                    }

                    xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                    xmlDiffAttributeOrNamespace1 =
                        (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace1._nextSibling;
                }
                else if (XmlDiffDocument.OrderAttributesOrNamespaces(xmlDiffAttributeOrNamespace,
                             xmlDiffAttributeOrNamespace1) >= 0)
                {
                    num1++;
                    xmlDiffAttributeOrNamespace1 =
                        (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace1._nextSibling;
                }
                else
                {
                    num2++;
                    xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
                }
            }

            while (xmlDiffAttributeOrNamespace != null)
            {
                num2++;
                xmlDiffAttributeOrNamespace = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace._nextSibling;
            }

            while (xmlDiffAttributeOrNamespace1 != null)
            {
                num1++;
                xmlDiffAttributeOrNamespace1 = (XmlDiffAttributeOrNamespace)xmlDiffAttributeOrNamespace1._nextSibling;
            }

            if (localName)
            {
                if (num2 + num1 + num3 <= num)
                {
                    return true;
                }

                return false;
            }

            if (num2 + num3 == 0)
            {
                return true;
            }

            if (num1 + num3 == 0)
            {
                return true;
            }

            if (num2 + num1 == 0)
            {
                return true;
            }

            if (num2 + num1 + num3 > num * 3 && sourceElement._nextSibling != null)
            {
                return false;
            }

            return true;
        }

        private bool OnAdd(DiffgramParentOperation parent, int sourceBorderIndex, int targetBorderIndex)
        {
            EditScriptAdd left = this._editScript as EditScriptAdd;
            XmlDiffNode xmlDiffNode = this._targetNodes[left._endTargetIndex];
            if (left._startTargetIndex > xmlDiffNode.Left || xmlDiffNode._bSomeDescendantMatches)
            {
                DiffgramAddNode diffgramAddNode = new DiffgramAddNode(xmlDiffNode, (ulong)0);
                this._curTargetIndex--;
                left._endTargetIndex--;
                if (left._startTargetIndex > left._endTargetIndex)
                {
                    this._editScript = this._editScript._nextEditScript;
                }

                if (!this._bBuildingAddTree)
                {
                    this._postponedEditScript.Reset();
                    this._bBuildingAddTree = true;
                    this.GenerateDiffgramAdd(diffgramAddNode, sourceBorderIndex, xmlDiffNode.Left);
                    this._bBuildingAddTree = false;
                    if (this._postponedEditScript._firstES != null)
                    {
                        this._curSourceIndex = this._postponedEditScript._endSourceIndex;
                        this._postponedEditScript._lastES._nextEditScript = this._editScript;
                        this._editScript = this._postponedEditScript._firstES;
                    }
                }
                else
                {
                    this.GenerateDiffgramAdd(diffgramAddNode, sourceBorderIndex, xmlDiffNode.Left);
                }

                if (xmlDiffNode.NodeType == XmlDiffNodeType.Element)
                {
                    this.GenerateAddDiffgramForAttributes(diffgramAddNode, (XmlDiffElement)xmlDiffNode);
                }

                parent.InsertAtBeginning(diffgramAddNode);
            }
            else
            {
                XmlDiffNodeType nodeType = xmlDiffNode.NodeType;
                switch (nodeType)
                {
                    case XmlDiffNodeType.XmlDeclaration:
                    case XmlDiffNodeType.DocumentType:
                    {
                        parent.InsertAtBeginning(new DiffgramAddNode(xmlDiffNode, (ulong)0));
                        break;
                    }
                    default:
                    {
                        if (nodeType == XmlDiffNodeType.EntityReference)
                        {
                            goto case XmlDiffNodeType.DocumentType;
                        }

                        if (nodeType != XmlDiffNodeType.ShrankNode)
                        {
                            if (parent.MergeAddSubtreeAtBeginning(xmlDiffNode))
                            {
                                break;
                            }

                            parent.InsertAtBeginning(new DiffgramAddSubtrees(xmlDiffNode, (ulong)0,
                                !this._xmlDiff.IgnoreChildOrder));
                            break;
                        }
                        else
                        {
                            XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode)xmlDiffNode;
                            if (xmlDiffShrankNode.MoveOperationId == (long)0)
                            {
                                xmlDiffShrankNode.MoveOperationId =
                                    this.GenerateOperationID(XmlDiffDescriptorType.Move);
                            }

                            parent.InsertAtBeginning(new DiffgramCopy(xmlDiffShrankNode.MatchingShrankNode, true,
                                xmlDiffShrankNode.MoveOperationId));
                            break;
                        }
                    }
                }

                this._curTargetIndex = xmlDiffNode.Left - 1;
                left._endTargetIndex = xmlDiffNode.Left - 1;
                if (left._startTargetIndex > left._endTargetIndex)
                {
                    this._editScript = this._editScript._nextEditScript;
                }
            }

            if (!this._bChildOrderSignificant)
            {
                return false;
            }

            return !this._bBuildingAddTree;
        }

        private void OnChange(DiffgramParentOperation parent)
        {
            EditScriptChange editScriptChange = this._editScript as EditScriptChange;
            XmlDiffNode xmlDiffNode = this._sourceNodes[editScriptChange._sourceIndex];
            XmlDiffNode xmlDiffNode1 = this._targetNodes[editScriptChange._targetIndex];
            this._curSourceIndex--;
            this._curTargetIndex--;
            this._editScript = this._editScript._nextEditScript;
            DiffgramOperation diffgramChangeNode = null;
            if (!this._bBuildingAddTree)
            {
                ulong namespaceChangeOpid = (ulong)0;
                if (!this._xmlDiff.IgnoreNamespaces && xmlDiffNode.NodeType == XmlDiffNodeType.Element)
                {
                    XmlDiffElement xmlDiffElement = (XmlDiffElement)xmlDiffNode;
                    XmlDiffElement xmlDiffElement1 = (XmlDiffElement)xmlDiffNode1;
                    if (xmlDiffElement.LocalName == xmlDiffElement1.LocalName)
                    {
                        namespaceChangeOpid = this.GetNamespaceChangeOpid(xmlDiffElement.NamespaceURI,
                            xmlDiffElement.Prefix, xmlDiffElement1.NamespaceURI, xmlDiffElement1.Prefix);
                    }
                }

                if (xmlDiffNode.NodeType != XmlDiffNodeType.Element)
                {
                    diffgramChangeNode = new DiffgramChangeNode(xmlDiffNode, xmlDiffNode1, editScriptChange._changeOp,
                        namespaceChangeOpid);
                }
                else
                {
                    if (!XmlDiff.IsChangeOperationOnAttributesOnly(editScriptChange._changeOp))
                    {
                        diffgramChangeNode = new DiffgramChangeNode(xmlDiffNode, xmlDiffNode1,
                            XmlDiffOperation.ChangeElementName, namespaceChangeOpid);
                    }
                    else
                    {
                        diffgramChangeNode = new DiffgramPosition(xmlDiffNode);
                    }

                    if (xmlDiffNode.Left < editScriptChange._sourceIndex ||
                        xmlDiffNode1.Left < editScriptChange._targetIndex)
                    {
                        this.GenerateDiffgramMatch((DiffgramParentOperation)diffgramChangeNode, xmlDiffNode.Left,
                            xmlDiffNode1.Left);
                    }

                    this.GenerateChangeDiffgramForAttributes((DiffgramParentOperation)diffgramChangeNode,
                        (XmlDiffElement)xmlDiffNode, (XmlDiffElement)xmlDiffNode1);
                }
            }
            else
            {
                if (xmlDiffNode1.NodeType != XmlDiffNodeType.Element)
                {
                    diffgramChangeNode =
                        new DiffgramAddSubtrees(xmlDiffNode1, (ulong)0, !this._xmlDiff.IgnoreChildOrder);
                }
                else
                {
                    diffgramChangeNode = new DiffgramAddNode(xmlDiffNode1, (ulong)0);
                }

                bool nodeType = xmlDiffNode.NodeType != XmlDiffNodeType.Element;
                this.PostponedRemoveNode(xmlDiffNode, nodeType, (ulong)0, editScriptChange._sourceIndex,
                    editScriptChange._sourceIndex);
                if (xmlDiffNode.Left < editScriptChange._sourceIndex ||
                    xmlDiffNode1.Left < editScriptChange._targetIndex)
                {
                    this.GenerateDiffgramAdd((DiffgramParentOperation)diffgramChangeNode, xmlDiffNode.Left,
                        xmlDiffNode1.Left);
                }

                if (xmlDiffNode1.NodeType == XmlDiffNodeType.Element)
                {
                    this.GenerateAddDiffgramForAttributes((DiffgramParentOperation)diffgramChangeNode,
                        (XmlDiffElement)xmlDiffNode1);
                }
            }

            parent.InsertAtBeginning(diffgramChangeNode);
        }

        private void OnEditScriptPostponed(DiffgramParentOperation parent, int targetBorderIndex)
        {
            EditScriptPostponed editScriptPostponed = (EditScriptPostponed)this._editScript;
            DiffgramOperation diffgramOperation = editScriptPostponed._diffOperation;
            int num = editScriptPostponed._startSourceIndex;
            int left = this._sourceNodes[editScriptPostponed._endSourceIndex].Left;
            this._curSourceIndex = editScriptPostponed._startSourceIndex - 1;
            this._editScript = editScriptPostponed._nextEditScript;
            if (num > left)
            {
                this.GenerateDiffgramPostponed((DiffgramParentOperation)diffgramOperation, ref this._editScript, left,
                    targetBorderIndex);
            }

            parent.InsertAtBeginning(diffgramOperation);
        }

        private void OnMatch(DiffgramParentOperation parent, bool bNeedPosition)
        {
            DiffgramParentOperation diffgramCopy;
            EditScriptMatch editScriptMatch = this._editScript as EditScriptMatch;
            int num = editScriptMatch._firstTargetIndex + editScriptMatch._length - 1;
            int num1 = editScriptMatch._firstSourceIndex + editScriptMatch._length - 1;
            XmlDiffNode xmlDiffNode = this._targetNodes[num];
            XmlDiffNode xmlDiffNode1 = this._sourceNodes[num1];
            if (editScriptMatch._firstTargetIndex > xmlDiffNode.Left ||
                editScriptMatch._firstSourceIndex > xmlDiffNode1.Left)
            {
                this._curSourceIndex--;
                this._curTargetIndex--;
                editScriptMatch._length--;
                if (editScriptMatch._length <= 0)
                {
                    this._editScript = this._editScript._nextEditScript;
                }

                if (this._bBuildingAddTree)
                {
                    ulong num2 = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                    bool nodeType = xmlDiffNode1.NodeType != XmlDiffNodeType.Element;
                    diffgramCopy = new DiffgramCopy(xmlDiffNode1, nodeType, num2);
                    this.PostponedRemoveNode(xmlDiffNode1, nodeType, num2, num1, num1);
                    this.GenerateDiffgramAdd(diffgramCopy, xmlDiffNode1.Left, xmlDiffNode.Left);
                    parent.InsertAtBeginning(diffgramCopy);
                    return;
                }

                diffgramCopy = new DiffgramPosition(xmlDiffNode1);
                this.GenerateDiffgramMatch(diffgramCopy, xmlDiffNode1.Left, xmlDiffNode.Left);
                if (diffgramCopy._firstChildOp != null)
                {
                    parent.InsertAtBeginning(diffgramCopy);
                }
            }
            else
            {
                if (this._bBuildingAddTree)
                {
                    ulong num3 = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                    parent.InsertAtBeginning(new DiffgramCopy(xmlDiffNode1, true, num3));
                    this.PostponedRemoveSubtrees(xmlDiffNode1, num3, xmlDiffNode1.Left, num1);
                }
                else if (xmlDiffNode1.NodeType == XmlDiffNodeType.Element)
                {
                    DiffgramPosition diffgramPosition = this._cachedDiffgramPosition;
                    diffgramPosition._sourceNode = xmlDiffNode1;
                    this.GenerateChangeDiffgramForAttributes(diffgramPosition, (XmlDiffElement)xmlDiffNode1,
                        (XmlDiffElement)xmlDiffNode);
                    if (diffgramPosition._firstChildOp != null || bNeedPosition)
                    {
                        parent.InsertAtBeginning(diffgramPosition);
                        this._cachedDiffgramPosition = new DiffgramPosition(null);
                        bNeedPosition = false;
                    }
                }
                else if (bNeedPosition)
                {
                    parent.InsertAtBeginning(new DiffgramPosition(xmlDiffNode1));
                    bNeedPosition = false;
                }
                else if (!this._bChildOrderSignificant && xmlDiffNode1.NodeType < XmlDiffNodeType.None)
                {
                    DiffgramOperation diffgramOperation = parent._firstChildOp;
                    if (diffgramOperation is DiffgramAddNode || diffgramOperation is DiffgramAddSubtrees ||
                        diffgramOperation is DiffgramCopy)
                    {
                        parent.InsertAtBeginning(new DiffgramPosition(xmlDiffNode1));
                    }
                }

                this._curSourceIndex = xmlDiffNode1.Left - 1;
                this._curTargetIndex = xmlDiffNode.Left - 1;
                EditScriptMatch left = editScriptMatch;
                left._length = left._length - (num - xmlDiffNode.Left + 1);
                if (editScriptMatch._length <= 0)
                {
                    this._editScript = this._editScript._nextEditScript;
                    return;
                }
            }
        }

        private void OnRemove(DiffgramParentOperation parent)
        {
            EditScriptRemove left = this._editScript as EditScriptRemove;
            XmlDiffNode xmlDiffNode = this._sourceNodes[left._endSourceIndex];
            if (left._startSourceIndex > xmlDiffNode.Left)
            {
                this._curSourceIndex--;
                left._endSourceIndex--;
                if (left._startSourceIndex > left._endSourceIndex)
                {
                    this._editScript = this._editScript._nextEditScript;
                }

                bool nodeType = xmlDiffNode.NodeType != XmlDiffNodeType.Element;
                if (this._bBuildingAddTree)
                {
                    this.PostponedRemoveNode(xmlDiffNode, nodeType, (ulong)0, left._endSourceIndex + 1,
                        left._endSourceIndex + 1);
                    this.GenerateDiffgramAdd(parent, xmlDiffNode.Left, this._targetNodes[this._curTargetIndex].Left);
                    return;
                }

                DiffgramRemoveNode diffgramRemoveNode = new DiffgramRemoveNode(xmlDiffNode, nodeType, (ulong)0);
                this.GenerateDiffgramMatch(diffgramRemoveNode, xmlDiffNode.Left,
                    this._targetNodes[this._curTargetIndex].Left);
                parent.InsertAtBeginning(diffgramRemoveNode);
            }
            else
            {
                bool flag = xmlDiffNode is XmlDiffShrankNode;
                if (!xmlDiffNode._bSomeDescendantMatches || flag)
                {
                    ulong moveOperationId = (ulong)0;
                    if (flag)
                    {
                        XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode)xmlDiffNode;
                        if (xmlDiffShrankNode.MoveOperationId == (long)0)
                        {
                            xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                        }

                        moveOperationId = xmlDiffShrankNode.MoveOperationId;
                    }

                    if (this._bBuildingAddTree)
                    {
                        this.PostponedRemoveSubtrees(xmlDiffNode, moveOperationId, xmlDiffNode.Left,
                            left._endSourceIndex);
                    }
                    else if (moveOperationId != (long)0 || !parent.MergeRemoveSubtreeAtBeginning(xmlDiffNode))
                    {
                        parent.InsertAtBeginning(new DiffgramRemoveSubtrees(xmlDiffNode, moveOperationId,
                            !this._xmlDiff.IgnoreChildOrder));
                    }
                }
                else
                {
                    DiffgramOperation diffgramOperation =
                        this.GenerateDiffgramRemoveWhenDescendantMatches((XmlDiffParentNode)xmlDiffNode);
                    if (!this._bBuildingAddTree)
                    {
                        parent.InsertAtBeginning(diffgramOperation);
                    }
                    else
                    {
                        this.PostponedOperation(diffgramOperation, xmlDiffNode.Left, left._endSourceIndex);
                    }
                }

                this._curSourceIndex = xmlDiffNode.Left - 1;
                left._endSourceIndex = xmlDiffNode.Left - 1;
                if (left._startSourceIndex > left._endSourceIndex)
                {
                    this._editScript = this._editScript._nextEditScript;
                    return;
                }
            }
        }

        private void PostponedOperation(DiffgramOperation op, int startSourceIndex, int endSourceIndex)
        {
            EditScriptPostponed editScriptPostponed = new EditScriptPostponed(op, startSourceIndex, endSourceIndex);
            if (this._postponedEditScript._firstES != null)
            {
                this._postponedEditScript._lastES._nextEditScript = editScriptPostponed;
                this._postponedEditScript._lastES = editScriptPostponed;
                this._postponedEditScript._startSourceIndex = startSourceIndex;
                return;
            }

            this._postponedEditScript._firstES = editScriptPostponed;
            this._postponedEditScript._lastES = editScriptPostponed;
            this._postponedEditScript._startSourceIndex = startSourceIndex;
            this._postponedEditScript._endSourceIndex = endSourceIndex;
        }

        private void PostponedRemoveNode(XmlDiffNode sourceNode, bool bSubtree, ulong operationID, int startSourceIndex,
            int endSourceIndex)
        {
            this.PostponedOperation(new DiffgramRemoveNode(sourceNode, bSubtree, operationID), startSourceIndex,
                endSourceIndex);
        }

        private void PostponedRemoveSubtrees(XmlDiffNode sourceNode, ulong operationID, int startSourceIndex,
            int endSourceIndex)
        {
            if (operationID == (long)0 && this._postponedEditScript._firstES != null)
            {
                DiffgramRemoveSubtrees diffgramRemoveSubtree =
                    this._postponedEditScript._lastES._diffOperation as DiffgramRemoveSubtrees;
                if (diffgramRemoveSubtree != null && diffgramRemoveSubtree.SetNewFirstNode(sourceNode))
                {
                    this._postponedEditScript._lastES._startSourceIndex = startSourceIndex;
                    this._postponedEditScript._startSourceIndex = startSourceIndex;
                    return;
                }
            }

            this.PostponedOperation(
                new DiffgramRemoveSubtrees(sourceNode, operationID, !this._xmlDiff.IgnoreChildOrder), startSourceIndex,
                endSourceIndex);
        }

        // Microsoft.XmlDiffPatch.DiffgramGenerator
        private void WalkTreeGenerateDiffgramMatch(DiffgramParentOperation diffParent, XmlDiffParentNode sourceParent,
            XmlDiffParentNode targetParent)
        {
            XmlDiffNode xmlDiffNode = sourceParent.FirstChildNode;
            XmlDiffNode xmlDiffNode2 = targetParent.FirstChildNode;
            XmlDiffNode sourcePositionNode = null;
            while (xmlDiffNode != null || xmlDiffNode2 != null)
            {
                if (xmlDiffNode != null)
                {
                    if (xmlDiffNode2 != null)
                    {
                        XmlDiffOperation xmlDiffOperation = xmlDiffNode.GetDiffOperation(xmlDiffNode2, this._xmlDiff);
                        if (xmlDiffOperation != XmlDiffOperation.Match)
                        {
                            int num = (xmlDiffNode._parent.ChildNodesCount + xmlDiffNode2._parent.ChildNodesCount) / 2;
                            num = (int)(4.0 * Math.Log((double)num) + 1.0);
                            XmlDiffNode xmlDiffNode3 = xmlDiffNode2;
                            XmlDiffNode xmlDiffNode4 = xmlDiffNode;
                            XmlDiffOperation xmlDiffOperation2 = xmlDiffOperation;
                            XmlDiffOperation xmlDiffOperation3 = xmlDiffOperation;
                            int num2 = xmlDiffNode2.NodesCount;
                            int num3 = xmlDiffNode.NodesCount;
                            XmlDiffNode xmlDiffNode5 = xmlDiffNode._nextSibling;
                            XmlDiffNode xmlDiffNode6 = xmlDiffNode2._nextSibling;
                            int num4 = 0;
                            while (num4 < num && (xmlDiffNode6 != null || xmlDiffNode5 != null))
                            {
                                if (xmlDiffNode6 != null && xmlDiffOperation2 != XmlDiffOperation.Match)
                                {
                                    XmlDiffOperation diffOperation =
                                        xmlDiffNode.GetDiffOperation(xmlDiffNode6, this._xmlDiff);
                                    if (MinimalTreeDistanceAlgo.OperationCost[(int)diffOperation] <
                                        MinimalTreeDistanceAlgo.OperationCost[(int)xmlDiffOperation2])
                                    {
                                        xmlDiffOperation2 = diffOperation;
                                        xmlDiffNode3 = xmlDiffNode6;
                                    }
                                    else
                                    {
                                        num2 += xmlDiffNode6.NodesCount;
                                        xmlDiffNode6 = xmlDiffNode6._nextSibling;
                                    }
                                }

                                if (xmlDiffNode5 != null && xmlDiffOperation3 != XmlDiffOperation.Match)
                                {
                                    XmlDiffOperation diffOperation2 =
                                        xmlDiffNode2.GetDiffOperation(xmlDiffNode5, this._xmlDiff);
                                    if (MinimalTreeDistanceAlgo.OperationCost[(int)diffOperation2] <
                                        MinimalTreeDistanceAlgo.OperationCost[(int)xmlDiffOperation3])
                                    {
                                        xmlDiffOperation3 = diffOperation2;
                                        xmlDiffNode4 = xmlDiffNode5;
                                    }
                                    else
                                    {
                                        num3 += xmlDiffNode5.NodesCount;
                                        xmlDiffNode5 = xmlDiffNode5._nextSibling;
                                    }
                                }

                                if (xmlDiffOperation2 == XmlDiffOperation.Match ||
                                    xmlDiffOperation3 == XmlDiffOperation.Match)
                                {
                                    break;
                                }

                                if (this._xmlDiff.IgnoreChildOrder)
                                {
                                    if (xmlDiffNode6 != null &&
                                        XmlDiffDocument.OrderChildren(xmlDiffNode, xmlDiffNode6) < 0)
                                    {
                                        xmlDiffNode6 = null;
                                    }

                                    if (xmlDiffNode5 != null &&
                                        XmlDiffDocument.OrderChildren(xmlDiffNode2, xmlDiffNode5) < 0)
                                    {
                                        xmlDiffNode5 = null;
                                    }
                                }

                                num4++;
                            }

                            if (xmlDiffOperation2 == XmlDiffOperation.Match)
                            {
                                if (xmlDiffOperation3 == XmlDiffOperation.Match)
                                {
                                    if (num2 >= num3)
                                    {
                                        goto IL_2A7;
                                    }
                                }

                                while (xmlDiffNode2 != xmlDiffNode3)
                                {
                                    this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, sourcePositionNode);
                                    sourcePositionNode = null;
                                    xmlDiffNode2 = xmlDiffNode2._nextSibling;
                                }

                                this.WalkTreeOnMatchNode(diffParent, xmlDiffNode, xmlDiffNode2, ref sourcePositionNode);
                                goto IL_367;
                            }

                            if (xmlDiffOperation3 == XmlDiffOperation.Match)
                            {
                                while (xmlDiffNode != xmlDiffNode4)
                                {
                                    this.WalkTreeOnRemoveNode(diffParent, xmlDiffNode);
                                    xmlDiffNode = xmlDiffNode._nextSibling;
                                }

                                sourcePositionNode = null;
                                this.WalkTreeOnMatchNode(diffParent, xmlDiffNode, xmlDiffNode2, ref sourcePositionNode);
                                goto IL_367;
                            }

                            int num5 = MinimalTreeDistanceAlgo.OperationCost[(int)xmlDiffOperation2];
                            int num6 = MinimalTreeDistanceAlgo.OperationCost[(int)xmlDiffOperation3];
                            if (num5 >= num6)
                            {
                                if (num5 != num6 || num2 >= num3)
                                {
                                    while (xmlDiffNode != xmlDiffNode4)
                                    {
                                        this.WalkTreeOnRemoveNode(diffParent, xmlDiffNode);
                                        xmlDiffNode = xmlDiffNode._nextSibling;
                                    }

                                    xmlDiffOperation = xmlDiffOperation3;
                                    goto IL_2A7;
                                }
                            }

                            while (xmlDiffNode2 != xmlDiffNode3)
                            {
                                this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, sourcePositionNode);
                                sourcePositionNode = null;
                                xmlDiffNode2 = xmlDiffNode2._nextSibling;
                            }

                            xmlDiffOperation = xmlDiffOperation2;
                            IL_2A7:
                            switch (xmlDiffOperation)
                            {
                                case XmlDiffOperation.ChangeElementName:
                                    this.WalkTreeOnChangeElement(diffParent, (XmlDiffElement)xmlDiffNode,
                                        (XmlDiffElement)xmlDiffNode2, xmlDiffOperation);
                                    sourcePositionNode = null;
                                    goto IL_367;
                                case XmlDiffOperation.ChangeElementAttr1:
                                case XmlDiffOperation.ChangeElementAttr2:
                                case XmlDiffOperation.ChangeElementAttr3:
                                case XmlDiffOperation.ChangeElementNameAndAttr1:
                                case XmlDiffOperation.ChangeElementNameAndAttr2:
                                case XmlDiffOperation.ChangeElementNameAndAttr3:
                                    if (this.GoForElementChange((XmlDiffElement)xmlDiffNode,
                                            (XmlDiffElement)xmlDiffNode2))
                                    {
                                        this.WalkTreeOnChangeElement(diffParent, (XmlDiffElement)xmlDiffNode,
                                            (XmlDiffElement)xmlDiffNode2, xmlDiffOperation);
                                        sourcePositionNode = null;
                                        goto IL_367;
                                    }

                                    break;
                                case XmlDiffOperation.ChangePI:
                                case XmlDiffOperation.ChangeER:
                                case XmlDiffOperation.ChangeCharacterData:
                                case XmlDiffOperation.ChangeXmlDeclaration:
                                case XmlDiffOperation.ChangeDTD:
                                    diffParent.InsertAtEnd(new DiffgramChangeNode(xmlDiffNode, xmlDiffNode2,
                                        xmlDiffOperation, 0uL));
                                    sourcePositionNode = null;
                                    goto IL_367;
                                case XmlDiffOperation.Undefined:
                                    break;
                                default:
                                    goto IL_367;
                            }

                            this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, sourcePositionNode);
                            sourcePositionNode = null;
                            xmlDiffNode2 = xmlDiffNode2._nextSibling;
                            continue;
                        }

                        this.WalkTreeOnMatchNode(diffParent, xmlDiffNode, xmlDiffNode2, ref sourcePositionNode);
                        IL_367:
                        xmlDiffNode = xmlDiffNode._nextSibling;
                        xmlDiffNode2 = xmlDiffNode2._nextSibling;
                    }
                    else
                    {
                        do
                        {
                            this.WalkTreeOnRemoveNode(diffParent, xmlDiffNode);
                            xmlDiffNode = xmlDiffNode._nextSibling;
                        } while (xmlDiffNode != null);
                    }
                }
                else
                {
                    while (xmlDiffNode2 != null)
                    {
                        this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, sourcePositionNode);
                        sourcePositionNode = null;
                        xmlDiffNode2 = xmlDiffNode2._nextSibling;
                    }
                }
            }
        }

        private void WalkTreeOnAddNode(DiffgramParentOperation diffParent, XmlDiffNode targetNode,
            XmlDiffNode sourcePositionNode)
        {
            bool flag = targetNode is XmlDiffShrankNode;
            if (this._bChildOrderSignificant)
            {
                if (sourcePositionNode != null)
                {
                    diffParent.InsertAtEnd(new DiffgramPosition(sourcePositionNode));
                }
            }
            else if (diffParent._firstChildOp == null && diffParent is Diffgram)
            {
                diffParent.InsertAtEnd(new DiffgramPosition(sourcePositionNode));
            }

            if (targetNode._bSomeDescendantMatches && !flag)
            {
                diffParent.InsertAtEnd(this.GenerateDiffgramAddWhenDescendantMatches((XmlDiffParentNode)targetNode));
                return;
            }

            if (flag)
            {
                ulong moveOperationId = (ulong)0;
                XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode)targetNode;
                if (xmlDiffShrankNode.MoveOperationId == (long)0)
                {
                    xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                }

                moveOperationId = xmlDiffShrankNode.MoveOperationId;
                diffParent.InsertAtEnd(new DiffgramCopy(xmlDiffShrankNode.MatchingShrankNode, true, moveOperationId));
                return;
            }

            XmlDiffNodeType nodeType = targetNode.NodeType;
            switch (nodeType)
            {
                case XmlDiffNodeType.XmlDeclaration:
                case XmlDiffNodeType.DocumentType:
                {
                    diffParent.InsertAtEnd(new DiffgramAddNode(targetNode, (ulong)0));
                    return;
                }
                default:
                {
                    if (nodeType == XmlDiffNodeType.EntityReference)
                    {
                        diffParent.InsertAtEnd(new DiffgramAddNode(targetNode, (ulong)0));
                        return;
                    }

                    if (diffParent.MergeAddSubtreeAtEnd(targetNode))
                    {
                        return;
                    }

                    diffParent.InsertAtEnd(new DiffgramAddSubtrees(targetNode, (ulong)0,
                        !this._xmlDiff.IgnoreChildOrder));
                    return;
                }
            }
        }

        private void WalkTreeOnChangeElement(DiffgramParentOperation diffParent, XmlDiffElement sourceElement,
            XmlDiffElement targetElement, XmlDiffOperation op)
        {
            DiffgramParentOperation diffgramChangeNode;
            if (!XmlDiff.IsChangeOperationOnAttributesOnly(op))
            {
                ulong namespaceChangeOpid = (ulong)0;
                if (!this._xmlDiff.IgnoreNamespaces && sourceElement.LocalName == targetElement.LocalName)
                {
                    namespaceChangeOpid = this.GetNamespaceChangeOpid(sourceElement.NamespaceURI, sourceElement.Prefix,
                        targetElement.NamespaceURI, targetElement.Prefix);
                }

                diffgramChangeNode = new DiffgramChangeNode(sourceElement, targetElement,
                    XmlDiffOperation.ChangeElementName, namespaceChangeOpid);
            }
            else
            {
                diffgramChangeNode = new DiffgramPosition(sourceElement);
            }

            this.GenerateChangeDiffgramForAttributes(diffgramChangeNode, sourceElement, targetElement);
            if (sourceElement.HasChildNodes || targetElement.HasChildNodes)
            {
                this.WalkTreeGenerateDiffgramMatch(diffgramChangeNode, sourceElement, targetElement);
            }

            diffParent.InsertAtEnd(diffgramChangeNode);
        }

        private void WalkTreeOnChangeNode(DiffgramParentOperation diffParent, XmlDiffNode sourceNode,
            XmlDiffNode targetNode, XmlDiffOperation op)
        {
            DiffgramChangeNode diffgramChangeNode = new DiffgramChangeNode(sourceNode, targetNode, op, (ulong)0);
            if (sourceNode.HasChildNodes || targetNode.HasChildNodes)
            {
                this.WalkTreeGenerateDiffgramMatch(diffgramChangeNode, (XmlDiffParentNode)sourceNode,
                    (XmlDiffParentNode)targetNode);
            }

            diffParent.InsertAtEnd(diffgramChangeNode);
        }

        private void WalkTreeOnMatchNode(DiffgramParentOperation diffParent, XmlDiffNode sourceNode,
            XmlDiffNode targetNode, ref XmlDiffNode needPositionSourceNode)
        {
            if (!sourceNode.HasChildNodes && !targetNode.HasChildNodes)
            {
                if (sourceNode.NodeType != XmlDiffNodeType.ShrankNode)
                {
                    needPositionSourceNode = sourceNode;
                    return;
                }

                needPositionSourceNode = ((XmlDiffShrankNode)sourceNode)._lastNode;
                return;
            }

            DiffgramPosition diffgramPosition = new DiffgramPosition(sourceNode);
            this.WalkTreeGenerateDiffgramMatch(diffgramPosition, (XmlDiffParentNode)sourceNode,
                (XmlDiffParentNode)targetNode);
            diffParent.InsertAtEnd(diffgramPosition);
            needPositionSourceNode = null;
        }

        private void WalkTreeOnRemoveNode(DiffgramParentOperation diffParent, XmlDiffNode sourceNode)
        {
            bool flag = sourceNode is XmlDiffShrankNode;
            if (sourceNode._bSomeDescendantMatches && !flag)
            {
                diffParent.InsertAtEnd(this.GenerateDiffgramRemoveWhenDescendantMatches((XmlDiffParentNode)sourceNode));
                return;
            }

            ulong moveOperationId = (ulong)0;
            if (flag)
            {
                XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode)sourceNode;
                if (xmlDiffShrankNode.MoveOperationId == (long)0)
                {
                    xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
                }

                moveOperationId = xmlDiffShrankNode.MoveOperationId;
            }

            if (moveOperationId != (long)0 || !diffParent.MergeRemoveSubtreeAtEnd(sourceNode))
            {
                diffParent.InsertAtEnd(new DiffgramRemoveSubtrees(sourceNode, moveOperationId,
                    !this._xmlDiff.IgnoreChildOrder));
            }
        }

        internal class NamespaceChange
        {
            internal string _prefix;

            internal string _oldNS;

            internal string _newNS;

            internal ulong _opid;

            internal DiffgramGenerator.NamespaceChange _next;

            internal NamespaceChange(string prefix, string oldNamespace, string newNamespace, ulong opid,
                DiffgramGenerator.NamespaceChange next)
            {
                this._prefix = prefix;
                this._oldNS = oldNamespace;
                this._newNS = newNamespace;
                this._opid = opid;
                this._next = next;
            }
        }

        internal struct PostponedEditScriptInfo
        {
            internal EditScriptPostponed _firstES;

            internal EditScriptPostponed _lastES;

            internal int _startSourceIndex;

            internal int _endSourceIndex;

            internal void Reset()
            {
                this._firstES = null;
                this._lastES = null;
                this._startSourceIndex = 0;
                this._endSourceIndex = 0;
            }
        }

        internal class PrefixChange
        {
            internal string _oldPrefix;

            internal string _newPrefix;

            internal string _NS;

            internal ulong _opid;

            internal DiffgramGenerator.PrefixChange _next;

            internal PrefixChange(string oldPrefix, string newPrefix, string ns, ulong opid,
                DiffgramGenerator.PrefixChange next)
            {
                this._oldPrefix = oldPrefix;
                this._newPrefix = newPrefix;
                this._NS = ns;
                this._opid = opid;
                this._next = next;
            }
        }
    }
}