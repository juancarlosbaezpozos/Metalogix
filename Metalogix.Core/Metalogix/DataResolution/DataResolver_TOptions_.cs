using Metalogix;
using System;
using System.Reflection;

namespace Metalogix.DataResolution
{
    public abstract class DataResolver<TOptions> : DataResolver
        where TOptions : OptionsBase
    {
        private TOptions _options;

        public override OptionsBase Options
        {
            get { return this.ResolverOptions; }
            set
            {
                TOptions tOption = (TOptions)(value as TOptions);
                if (tOption != null)
                {
                    this._options = tOption;
                    base.FireOptionsChanged();
                }
            }
        }

        public TOptions ResolverOptions
        {
            get
            {
                if (object.Equals(this._options, default(TOptions)))
                {
                    try
                    {
                        this._options = Activator.CreateInstance<TOptions>();
                    }
                    catch
                    {
                        string.Format(
                            "Unable to create default resolver options {0} because it is missing constructor with 0 parameters.",
                            typeof(TOptions).Name);
                    }
                }

                return this._options;
            }
            set { this.Options = value; }
        }

        protected DataResolver()
        {
        }
    }
}