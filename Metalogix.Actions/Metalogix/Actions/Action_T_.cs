using System;

namespace Metalogix.Actions
{
    public abstract class Action<T> : Action
    {
        private T m_options;

        public override ActionOptions Options
        {
            get
            {
                if (this.ActionOptions != null)
                {
                    return this.ActionOptions as ActionOptions;
                }

                return null;
            }
            set
            {
                if (value != null && value is T)
                {
                    this.m_options = (T)((object)value);
                    base.FireOptionsChanged();
                }
            }
        }

        public virtual T ActionOptions
        {
            get
            {
                if (object.Equals(this.m_options, default(T)))
                {
                    try
                    {
                        this.m_options = Activator.CreateInstance<T>();
                    }
                    catch
                    {
                        string.Format(
                            "Unable to create default action options {0} because it is missing constructor with 0 parameters.",
                            typeof(T).Name);
                    }
                }

                return this.m_options;
            }
            set { this.Options = (value as ActionOptions); }
        }
    }
}