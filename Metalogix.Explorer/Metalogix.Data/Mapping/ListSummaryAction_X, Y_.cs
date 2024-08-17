using Metalogix;
using Metalogix.Actions;
using System;
using System.Drawing;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public abstract class ListSummaryAction<X, Y> : IListSummaryAction
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
            if (item == null || item.Source == null || item.Source.Tag == null || !(item.Source.Tag is X) ||
                item.Target == null || item.Target.Tag == null)
            {
                return false;
            }

            return item.Target.Tag is Y;
        }

        public virtual void RunAction(ListSummaryItem item)
        {
            try
            {
                this.RunSourceAction((X)item.Source.Tag);
                this.RunTargetAction((Y)item.Target.Tag);
            }
            catch (Exception exception)
            {
            }
        }

        public virtual void RunSourceAction(X item)
        {
        }

        public virtual void RunTargetAction(Y item)
        {
        }
    }
}