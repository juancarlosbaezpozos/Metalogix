using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class DiffgramParentOperation : DiffgramOperation
    {
        internal DiffgramOperation _firstChildOp;

        internal DiffgramOperation _lastChildOp;

        internal DiffgramParentOperation(ulong operationID) : base(operationID)
        {
            this._firstChildOp = null;
            this._lastChildOp = null;
        }

        internal void InsertAfter(DiffgramOperation newOp, DiffgramOperation refOp)
        {
            if (refOp == null)
            {
                return;
            }

            newOp._nextSiblingOp = refOp._nextSiblingOp;
            refOp._nextSiblingOp = newOp;
        }

        internal void InsertAtBeginning(DiffgramOperation newOp)
        {
            newOp._nextSiblingOp = this._firstChildOp;
            this._firstChildOp = newOp;
            if (newOp._nextSiblingOp == null)
            {
                this._lastChildOp = newOp;
            }
        }

        internal void InsertAtEnd(DiffgramOperation newOp)
        {
            newOp._nextSiblingOp = null;
            if (this._lastChildOp != null)
            {
                this._lastChildOp._nextSiblingOp = newOp;
                this._lastChildOp = newOp;
                return;
            }

            DiffgramOperation diffgramOperation = newOp;
            DiffgramOperation diffgramOperation1 = diffgramOperation;
            this._lastChildOp = diffgramOperation;
            this._firstChildOp = diffgramOperation1;
        }

        internal void InsertOperationAtBeginning(DiffgramOperation op)
        {
            op._nextSiblingOp = this._firstChildOp;
            this._firstChildOp = op;
        }

        internal bool MergeAddSubtreeAtBeginning(XmlDiffNode subtreeRoot)
        {
            DiffgramAddSubtrees diffgramAddSubtree = this._firstChildOp as DiffgramAddSubtrees;
            if (diffgramAddSubtree == null)
            {
                return false;
            }

            return diffgramAddSubtree.SetNewFirstNode(subtreeRoot);
        }

        internal bool MergeAddSubtreeAtEnd(XmlDiffNode subtreeRoot)
        {
            DiffgramAddSubtrees diffgramAddSubtree = this._lastChildOp as DiffgramAddSubtrees;
            if (diffgramAddSubtree == null)
            {
                return false;
            }

            return diffgramAddSubtree.SetNewLastNode(subtreeRoot);
        }

        internal bool MergeRemoveAttributeAtBeginning(XmlDiffNode subtreeRoot)
        {
            if (subtreeRoot.NodeType != XmlDiffNodeType.Attribute)
            {
                return false;
            }

            DiffgramRemoveAttributes diffgramRemoveAttribute = this._firstChildOp as DiffgramRemoveAttributes;
            if (diffgramRemoveAttribute == null)
            {
                return false;
            }

            return diffgramRemoveAttribute.AddAttribute((XmlDiffAttribute)subtreeRoot);
        }

        internal bool MergeRemoveSubtreeAtBeginning(XmlDiffNode subtreeRoot)
        {
            DiffgramRemoveSubtrees diffgramRemoveSubtree = this._firstChildOp as DiffgramRemoveSubtrees;
            if (diffgramRemoveSubtree == null)
            {
                return false;
            }

            return diffgramRemoveSubtree.SetNewFirstNode(subtreeRoot);
        }

        internal bool MergeRemoveSubtreeAtEnd(XmlDiffNode subtreeRoot)
        {
            DiffgramRemoveSubtrees diffgramRemoveSubtree = this._lastChildOp as DiffgramRemoveSubtrees;
            if (diffgramRemoveSubtree == null)
            {
                return false;
            }

            return diffgramRemoveSubtree.SetNewLastNode(subtreeRoot);
        }

        internal void WriteChildrenTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            for (DiffgramOperation i = this._firstChildOp; i != null; i = i._nextSiblingOp)
            {
                i.WriteTo(xmlWriter, xmlDiff);
            }
        }
    }
}