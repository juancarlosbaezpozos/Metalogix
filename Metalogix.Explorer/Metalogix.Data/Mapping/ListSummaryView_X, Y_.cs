using System;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public abstract class ListSummaryView<X, Y> : IListView
    {
        public virtual string Name
        {
            get { return string.Concat(this.GetType().Name, " Summary View"); }
        }

        public abstract ListView<X> SourceView { get; }

        public abstract ListView<Y> TargetView { get; }

        protected ListSummaryView()
        {
        }

        public bool AppliesTo(object item)
        {
            bool flag;
            try
            {
                flag = (item == null || !(item is ListSummaryItem) ? false : this.AppliesTo((ListSummaryItem)item));
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        protected virtual bool AppliesTo(ListSummaryItem item)
        {
            if (item == null || item.Source == null || item.Target == null || this.SourceView == null ||
                !this.SourceView.AppliesTo(item.Source.Tag) || this.TargetView == null)
            {
                return false;
            }

            return this.TargetView.AppliesTo(item.Target.Tag);
        }

        public string Render(object item)
        {
            string message;
            try
            {
                ListSummaryItem listSummaryItem = item as ListSummaryItem;
                string str = "";
                str = string.Concat(str, this.SourceView.Render(listSummaryItem.Source.Tag), "</>");
                str = string.Concat(str, this.TargetView.Render(listSummaryItem.Target.Tag));
                message = str;
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public string RenderColumn(object item, string propertyName)
        {
            string message;
            try
            {
                ListSummaryItem listSummaryItem = item as ListSummaryItem;
                message = this.RenderColumn((X)listSummaryItem.Source.Tag, (Y)listSummaryItem.Target.Tag, propertyName);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public virtual string RenderColumn(X source, Y target, string propertyName)
        {
            return "";
        }

        public string RenderGroup(object item)
        {
            string group;
            try
            {
                group = (item as ListSummaryItem).Group;
            }
            catch (Exception exception)
            {
                group = exception.Message;
            }

            return group;
        }

        public string RenderType(object item)
        {
            string message;
            try
            {
                ListSummaryItem listSummaryItem = item as ListSummaryItem;
                string str = "";
                str = string.Concat(str, this.SourceView.RenderType(listSummaryItem.Source.Tag), "</>");
                str = string.Concat(str, this.TargetView.RenderType(listSummaryItem.Target.Tag));
                message = str;
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }
    }
}