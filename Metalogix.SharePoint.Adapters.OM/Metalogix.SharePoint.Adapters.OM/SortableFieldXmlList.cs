using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class SortableFieldXmlList : List<XmlNode>
    {
        public SortableFieldXmlList()
        {
        }

        public SortableFieldXmlList(IEnumerable<XmlNode> xmlNodes) : base(xmlNodes)
        {
        }

        private bool AreSameFieldIdOrName(XmlNode firstField, XmlNode secondField)
        {
            if (string.Equals(this.GetFieldIdFrom(firstField), this.GetFieldIdFrom(secondField),
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return string.Equals(this.GetFieldNameFrom(firstField), this.GetFieldNameFrom(secondField),
                StringComparison.OrdinalIgnoreCase);
        }

        private List<XmlNode> GetFieldDependenciesFrom(XmlNode fieldXml)
        {
            List<XmlNode> xmlNodes = new List<XmlNode>();
            XmlNodeList xmlNodeLists = fieldXml.SelectNodes(".//FieldRef");
            if (xmlNodeLists == null || xmlNodeLists.Count == 0)
            {
                return xmlNodes;
            }

            xmlNodes.AddRange(xmlNodeLists.Cast<XmlNode>());
            return xmlNodes;
        }

        private string GetFieldIdFrom(XmlNode fieldXml)
        {
            XmlAttribute itemOf = fieldXml.Attributes["ID"];
            if (itemOf != null)
            {
                return itemOf.Value;
            }

            return Guid.NewGuid().ToString("B");
        }

        private string GetFieldNameFrom(XmlNode fieldXml)
        {
            return fieldXml.Attributes["Name"].Value;
        }

        private bool HasCircularDependency(int numItemsToSort, int numItemsRemaining)
        {
            return numItemsToSort == numItemsRemaining;
        }

        public bool HasFields(IList<XmlNode> fieldDependencies, ref SortableFieldXmlList sortedFields)
        {
            bool flag;
            using (IEnumerator<XmlNode> enumerator = fieldDependencies.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = enumerator.Current;
                    if (sortedFields.Any<XmlNode>((XmlNode field) => this.AreSameFieldIdOrName(field, current)))
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }

                return true;
            }

            return flag;
        }

        public void SortByDependencies()
        {
            if (base.Count == 0)
            {
                return;
            }

            SortableFieldXmlList sortableFieldXmlList = new SortableFieldXmlList();
            SortableFieldXmlList sortableFieldXmlList1 = new SortableFieldXmlList();
            Dictionary<string, List<XmlNode>> strs = new Dictionary<string, List<XmlNode>>();
            foreach (XmlNode xmlNodes in this)
            {
                string fieldNameFrom = this.GetFieldNameFrom(xmlNodes);
                List<XmlNode> fieldDependenciesFrom = this.GetFieldDependenciesFrom(xmlNodes);
                strs.Add(fieldNameFrom, fieldDependenciesFrom);
                if (fieldDependenciesFrom.Count != 0)
                {
                    sortableFieldXmlList1.Add(xmlNodes);
                }
                else
                {
                    sortableFieldXmlList.Add(xmlNodes);
                }
            }

            this.SortFieldsWithDependencies(sortableFieldXmlList1, strs, ref sortableFieldXmlList);
            base.Clear();
            base.AddRange(sortableFieldXmlList);
        }

        private void SortFieldsWithDependencies(SortableFieldXmlList unsortedFields,
            Dictionary<string, List<XmlNode>> dependencyTable, ref SortableFieldXmlList sortedFields)
        {
            if (unsortedFields.Count == 0)
            {
                return;
            }

            SortableFieldXmlList sortableFieldXmlList = new SortableFieldXmlList();
            foreach (XmlNode unsortedField in unsortedFields)
            {
                if (sortedFields.HasFields(dependencyTable[this.GetFieldNameFrom(unsortedField)], ref sortedFields))
                {
                    sortedFields.Add(unsortedField);
                }
                else
                {
                    sortableFieldXmlList.Add(unsortedField);
                }
            }

            if (this.HasCircularDependency(unsortedFields.Count, sortableFieldXmlList.Count))
            {
                sortedFields.AddRange(sortableFieldXmlList);
                return;
            }

            this.SortFieldsWithDependencies(sortableFieldXmlList, dependencyTable, ref sortedFields);
        }
    }
}