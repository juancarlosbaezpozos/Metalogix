using Metalogix;
using Metalogix.Data.Filters;
using System;
using System.Xml;

namespace Metalogix.Actions
{
    public class ActionOptionsBase : OptionsBase
    {
        public ActionOptionsBase()
        {
        }

        public new ActionOptionsBase Clone()
        {
            ActionOptionsBase actionOptionsBase = (ActionOptionsBase)Activator.CreateInstance(base.GetType());
            actionOptionsBase.FromXML(base.ToXML());
            return actionOptionsBase;
        }

        protected override object DeserializeOption(XmlNode optionNode, Type optionType, bool isEncrypted,
            out bool isDeserialized)
        {
            if (!typeof(IFilterExpression).IsAssignableFrom(optionType))
            {
                return base.DeserializeOption(optionNode, optionType, isEncrypted, out isDeserialized);
            }

            if (optionNode == null)
            {
                isDeserialized = false;
                return null;
            }

            isDeserialized = true;
            return FilterExpression.ParseExpression(optionNode.InnerXml);
        }

        public virtual void MakeOptionsIncremental(DateTime? incrementFromTime)
        {
        }
    }
}