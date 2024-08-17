using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class DiffgramRemoveAttributes : DiffgramOperation
    {
        private AttributeInterval _attributes;

        internal DiffgramRemoveAttributes(XmlDiffAttribute sourceAttr) : base((ulong)0)
        {
            this._attributes = new AttributeInterval(sourceAttr, null);
        }

        internal bool AddAttribute(XmlDiffAttribute srcAttr)
        {
            if (this._operationID != (long)0 || srcAttr._parent != this._attributes._firstAttr._parent)
            {
                return false;
            }

            if (srcAttr._nextSibling != this._attributes._firstAttr)
            {
                this._attributes = new AttributeInterval(srcAttr, this._attributes);
            }
            else
            {
                this._attributes._firstAttr = srcAttr;
            }

            return true;
        }

        internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            xmlWriter.WriteStartElement("xd", "remove", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
            DiffgramOperation.GetAddressOfAttributeInterval(this._attributes, xmlWriter);
            xmlWriter.WriteEndElement();
        }
    }
}