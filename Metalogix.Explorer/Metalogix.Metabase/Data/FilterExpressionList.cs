using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.Metabase.Data
{
    public class FilterExpressionList : ArrayList
    {
        public FilterExpressionList()
        {
        }

        public FilterExpressionList(IEnumerable<FilterExpression> filters)
        {
            foreach (FilterExpression filter in filters)
            {
                this.Add(filter);
            }
        }

        public void Clear(bool bRemoveBaseFilters)
        {
            this.Clear(true, bRemoveBaseFilters, true);
        }

        public void Clear(bool bRemoveBaseFilters, bool bRemoveActionFilters)
        {
            this.Clear(true, bRemoveBaseFilters, bRemoveActionFilters);
        }

        public void Clear(bool bRemoveNormalFilters, bool bRemoveBaseFilters, bool bRemoveActionFilters)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                PropertyFilterExpression item = (PropertyFilterExpression)this[i];
                if (bRemoveActionFilters && item.IsActionCreatedFilter || bRemoveBaseFilters && item.IsBaseFilter ||
                    bRemoveNormalFilters && !item.IsActionCreatedFilter && !item.IsBaseFilter)
                {
                    this.RemoveAt(i);
                }
            }
        }

        public bool EvaluateAll(object objValue)
        {
            bool flag;
            if (this.Count == 0)
            {
                return true;
            }

            IEnumerator enumerator = this.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (((FilterExpression)enumerator.Current).Evaluate(objValue))
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }

                return true;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        public override string ToString()
        {
            string str = "";
            foreach (PropertyFilterExpression propertyFilterExpression in this)
            {
                str = string.Concat(str, (str.Length == 0 ? "" : " and\n"));
                str = string.Concat(str, propertyFilterExpression.ToString());
            }

            return str;
        }
    }
}