using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Data
{
    public class ConditionalMappingCollection : SerializableList<ConditionalMapping>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override ConditionalMapping this[ConditionalMapping conditionalMapping_0]
        {
            get { throw new NotImplementedException(); }
        }

        public ConditionalMappingCollection()
        {
        }

        public ConditionalMappingCollection(XmlNode xmlNode_0)
        {
            this.FromXML(xmlNode_0);
        }

        public ConditionalMappingCollection(ConditionalMapping[] mappings) : base(mappings)
        {
        }

        public string Evaluate(string sourceName, object component)
        {
            return this.Evaluate(sourceName, component, (Comparison<object>)null);
        }

        public string Evaluate(string sourceName, object component, IComparer comparer)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.Evaluate(sourceName, component, comparison);
        }

        public string Evaluate(string sourceName, object component, Comparison<object> comparer)
        {
            ConditionalMappingCollection applicableMappings =
                this.GetMappingsFromSource(sourceName).GetApplicableMappings(component, comparer);
            if (applicableMappings.Count <= 0)
            {
                return null;
            }

            return ((ConditionalMapping)applicableMappings[0]).TargetName;
        }

        public ConditionalMappingCollection GetApplicableMappings(object component)
        {
            return this.GetApplicableMappings(component, (Comparison<object>)null);
        }

        public ConditionalMappingCollection GetApplicableMappings(object component, IComparer comparer)
        {
            Comparison<object> comparison = null;
            if (comparer != null)
            {
                IComparer comparer1 = comparer;
                comparison = new Comparison<object>(comparer1.Compare);
            }

            return this.GetApplicableMappings(component, comparison);
        }

        public ConditionalMappingCollection GetApplicableMappings(object component, Comparison<object> comparer)
        {
            List<ConditionalMapping> conditionalMappings = new List<ConditionalMapping>();
            foreach (ConditionalMapping conditionalMapping in this)
            {
                if (!conditionalMapping.Condition.Evaluate(component, comparer))
                {
                    continue;
                }

                conditionalMappings.Add(conditionalMapping);
            }

            return new ConditionalMappingCollection(conditionalMappings.ToArray());
        }

        public ConditionalMappingCollection GetMappingsFromSource(string sSourceName)
        {
            List<ConditionalMapping> conditionalMappings = new List<ConditionalMapping>();
            foreach (ConditionalMapping conditionalMapping in this)
            {
                if (conditionalMapping.SourceName != sSourceName)
                {
                    continue;
                }

                conditionalMappings.Add(conditionalMapping);
            }

            return new ConditionalMappingCollection(conditionalMappings.ToArray());
        }

        public ConditionalMappingCollection GetMappingsToTarget(string sTargetName)
        {
            List<ConditionalMapping> conditionalMappings = new List<ConditionalMapping>();
            foreach (ConditionalMapping conditionalMapping in this)
            {
                if (conditionalMapping.TargetName != sTargetName)
                {
                    continue;
                }

                conditionalMappings.Add(conditionalMapping);
            }

            return new ConditionalMappingCollection(conditionalMappings.ToArray());
        }
    }
}