using System;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public abstract class ListTransformer<T> : IListTransformer
    {
        public virtual string Name
        {
            get { return string.Concat(this.GetType().Name, " Transformer"); }
        }

        protected ListTransformer()
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

        public abstract string Transform(T item);

        public string Transform(object item)
        {
            string message;
            try
            {
                message = this.Transform((T)item);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public abstract string TransformColumn(T item, string propertyName);

        public string TransformColumn(object item, string propertyName)
        {
            string message;
            try
            {
                message = this.TransformColumn((T)item, propertyName);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }

        public abstract string TransformType(T item);

        public string TransformType(object item)
        {
            string message;
            try
            {
                message = this.TransformType((T)item);
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return message;
        }
    }
}