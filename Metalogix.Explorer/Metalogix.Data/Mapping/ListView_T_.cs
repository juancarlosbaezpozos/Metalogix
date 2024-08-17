using System;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public abstract class ListView<T> : IListView
    {
        public virtual string Name
        {
            get { return string.Concat(this.GetType().Name, " View"); }
        }

        protected ListView()
        {
        }

        public virtual bool AppliesTo(object item)
        {
            if (item == null)
            {
                return false;
            }

            return item is T;
        }

        public abstract string Render(T item);

        public string Render(object item)
        {
            string message;
            try
            {
                message = this.Render((T)item);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public virtual string RenderColumn(T item, string propertyName)
        {
            PropertyInfo property = item.GetType().GetProperty(propertyName);
            if (property != null && property.CanRead)
            {
                object value = property.GetValue(item, null);
                if (value != null)
                {
                    return value.ToString();
                }
            }

            return "";
        }

        public string RenderColumn(object item, string propertyName)
        {
            string message;
            try
            {
                message = this.RenderColumn((T)item, propertyName);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public virtual string RenderGroup(T item)
        {
            return "default";
        }

        public string RenderGroup(object item)
        {
            string message;
            try
            {
                message = this.RenderGroup((T)item);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public virtual string RenderType(T item)
        {
            return item.GetType().Name;
        }

        public string RenderType(object item)
        {
            string message;
            try
            {
                message = this.RenderType((T)item);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }
    }
}