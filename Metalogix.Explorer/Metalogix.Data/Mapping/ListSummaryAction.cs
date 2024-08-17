using Metalogix;
using Metalogix.Actions;
using System;
using System.Drawing;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public abstract class ListSummaryAction : IListSummaryAction
    {
        public virtual System.Drawing.Image Image
        {
            get { return ImageCache.GetImage(this.ImageName, this.GetType().Assembly); }
        }

        public virtual string ImageName
        {
            get
            {
                object[] customAttributes = this.GetType().GetCustomAttributes(typeof(ImageAttribute), true);
                if ((int)customAttributes.Length != 1)
                {
                    return "";
                }

                return ((ImageAttribute)customAttributes[0]).ImageName;
            }
        }

        public abstract string Name { get; }

        protected ListSummaryAction()
        {
        }

        public virtual bool AppliesTo(ListSummaryItem item)
        {
            if (item == null || item.Source == null || item.Source.Tag == null || item.Target == null)
            {
                return false;
            }

            return item.Target.Tag != null;
        }

        public abstract void RunAction(ListSummaryItem item);
    }
}