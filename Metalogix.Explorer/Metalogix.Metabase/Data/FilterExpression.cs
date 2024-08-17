using System;

namespace Metalogix.Metabase.Data
{
    public abstract class FilterExpression
    {
        protected bool m_bIsBaseFilter;

        protected bool m_bActionCreatedFilter;

        public bool IsActionCreatedFilter
        {
            get { return this.m_bActionCreatedFilter; }
            set { this.m_bActionCreatedFilter = value; }
        }

        public bool IsBaseFilter
        {
            get { return this.m_bIsBaseFilter; }
            set { this.m_bIsBaseFilter = value; }
        }

        protected FilterExpression()
        {
        }

        public abstract bool Evaluate(object objValue);
    }
}