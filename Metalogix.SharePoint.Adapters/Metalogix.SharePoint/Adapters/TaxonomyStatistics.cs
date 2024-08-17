using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class TaxonomyStatistics
    {
        private const uint C_GroupCost = 150;

        private const uint C_TermSetUpdateCost = 220;

        private const uint C_TermSetAddOnlyCost = 290;

        private const uint C_TermUpdateCost = 165;

        private const uint C_TermAddOnlyCost = 220;

        private const uint C_ReassignedCost = 25;

        private BindingFlags m_propertySearchScope =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public int GroupsAdded { get; set; }

        public int GroupsUpdated { get; set; }

        public int TermsAdded { get; set; }

        public int TermSetsAdded { get; set; }

        public int TermSetsUpdated { get; set; }

        public int TermsReassigned { get; set; }

        public int TermsReassignedFromOrphanedTerms { get; set; }

        public int TermsUpdated { get; set; }

        public long Usage
        {
            get
            {
                long groupsAdded = (long)(this.GroupsAdded + this.GroupsUpdated) * (long)150;
                long termSetsAdded = (long)this.TermSetsAdded * (long)290 + (long)this.TermSetsUpdated * (long)220;
                long termsAdded = (long)this.TermsAdded * (long)220 + (long)this.TermsUpdated * (long)165;
                long termsReassigned = (long)this.TermsReassigned * (long)25;
                long termsReassignedFromOrphanedTerms = (long)this.TermsReassignedFromOrphanedTerms * (long)25;
                return groupsAdded + termSetsAdded + termsAdded + termsReassigned + termsReassignedFromOrphanedTerms;
            }
        }

        public TaxonomyStatistics()
        {
            this.Reset();
        }

        public TaxonomyStatistics(XmlNode statNode) : this()
        {
            this.Populate(statNode);
        }

        public void Add(TaxonomyStatistics addStat)
        {
            TaxonomyStatistics groupsAdded = this;
            groupsAdded.GroupsAdded = groupsAdded.GroupsAdded + addStat.GroupsAdded;
            TaxonomyStatistics groupsUpdated = this;
            groupsUpdated.GroupsUpdated = groupsUpdated.GroupsUpdated + addStat.GroupsUpdated;
            TaxonomyStatistics termSetsAdded = this;
            termSetsAdded.TermSetsAdded = termSetsAdded.TermSetsAdded + addStat.TermSetsAdded;
            TaxonomyStatistics termSetsUpdated = this;
            termSetsUpdated.TermSetsUpdated = termSetsUpdated.TermSetsUpdated + addStat.TermSetsUpdated;
            TaxonomyStatistics termsAdded = this;
            termsAdded.TermsAdded = termsAdded.TermsAdded + addStat.TermsAdded;
            TaxonomyStatistics termsUpdated = this;
            termsUpdated.TermsUpdated = termsUpdated.TermsUpdated + addStat.TermsUpdated;
            TaxonomyStatistics termsReassigned = this;
            termsReassigned.TermsReassigned = termsReassigned.TermsReassigned + addStat.TermsReassigned;
            TaxonomyStatistics termsReassignedFromOrphanedTerms = this;
            termsReassignedFromOrphanedTerms.TermsReassignedFromOrphanedTerms =
                termsReassignedFromOrphanedTerms.TermsReassignedFromOrphanedTerms +
                addStat.TermsReassignedFromOrphanedTerms;
        }

        public void Add(XmlNode statNode)
        {
            if (statNode != null)
            {
                foreach (XmlAttribute attribute in statNode.Attributes)
                {
                    PropertyInfo property = this.GetType().GetProperty(attribute.Name, this.m_propertySearchScope);
                    if (property == null || !property.CanWrite || !property.CanRead)
                    {
                        continue;
                    }

                    int num = 0;
                    int.TryParse(attribute.Value, out num);
                    int num1 = 0;
                    int.TryParse(property.GetValue(this, null).ToString(), out num1);
                    num1 += num;
                    property.SetValue(this, num1, null);
                }
            }
        }

        public void Populate(XmlNode statNode)
        {
            if (statNode != null)
            {
                foreach (XmlAttribute attribute in statNode.Attributes)
                {
                    PropertyInfo property = this.GetType().GetProperty(attribute.Name, this.m_propertySearchScope);
                    if (property == null || !property.CanWrite || !property.CanRead)
                    {
                        continue;
                    }

                    int num = 0;
                    int.TryParse(attribute.Value, out num);
                    int num1 = 0;
                    int.TryParse(property.GetValue(this, null).ToString(), out num1);
                    num1 += num;
                    property.SetValue(this, num1, null);
                }
            }
        }

        public void Reset()
        {
            this.GroupsAdded = 0;
            this.GroupsUpdated = 0;
            this.TermSetsAdded = 0;
            this.TermSetsUpdated = 0;
            this.TermsAdded = 0;
            this.TermsUpdated = 0;
            this.TermsReassigned = 0;
            this.TermsReassignedFromOrphanedTerms = 0;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            PropertyInfo[] properties = this.GetType().GetProperties(this.m_propertySearchScope);
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.CanRead)
                {
                    int num = 0;
                    int.TryParse(propertyInfo.GetValue(this, null).ToString(), out num);
                    stringBuilder.AppendLine(string.Format("{0} = {1}", propertyInfo.Name, num.ToString()));
                }
            }

            return stringBuilder.ToString();
        }
    }
}