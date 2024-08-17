using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class XmlDiffDocumentType : XmlDiffNode
    {
        private string _name;

        private string _publicId;

        private string _systemId;

        private string _subset;

        internal string Name
        {
            get { return this._name; }
        }

        internal override XmlDiffNodeType NodeType
        {
            get { return XmlDiffNodeType.DocumentType; }
        }

        internal string PublicId
        {
            get { return this._publicId; }
        }

        internal string Subset
        {
            get { return this._subset; }
        }

        internal string SystemId
        {
            get { return this._systemId; }
        }

        internal XmlDiffDocumentType(int position, string name, string publicId, string systemId, string subset) :
            base(position)
        {
            this._name = name;
            this._publicId = publicId;
            this._systemId = systemId;
            this._subset = subset;
        }

        internal override void ComputeHashValue(XmlHash xmlHash)
        {
            this._hashValue = xmlHash.HashDocumentType(this._name, this._publicId, this._systemId, this._subset);
        }

        internal override XmlDiffOperation GetDiffOperation(XmlDiffNode changedNode, XmlDiff xmlDiff)
        {
            if (changedNode.NodeType != XmlDiffNodeType.DocumentType)
            {
                return XmlDiffOperation.Undefined;
            }

            XmlDiffDocumentType xmlDiffDocumentType = (XmlDiffDocumentType)changedNode;
            if (this.Name == xmlDiffDocumentType.Name && this.PublicId == xmlDiffDocumentType.PublicId &&
                this.SystemId == xmlDiffDocumentType.SystemId && this.Subset == xmlDiffDocumentType.Subset)
            {
                return XmlDiffOperation.Match;
            }

            return XmlDiffOperation.ChangeDTD;
        }

        internal override void WriteContentTo(XmlWriter w)
        {
        }

        internal override void WriteTo(XmlWriter w)
        {
            w.WriteDocType(this._name, string.Empty, string.Empty, this._subset);
        }
    }
}