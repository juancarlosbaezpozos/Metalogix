using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Options.Transform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.UI.WinForms.Transform
{
    public class BindableSubstitutionValues
    {
        private IFilterExpression _expression;

        private string _substitute;

        public IFilterExpression Filter
        {
            get
            {
                return this._expression;
            }
            set
            {
                this._expression = value;
            }
        }

        public string Substitute
        {
            get
            {
                return this._substitute;
            }
            set
            {
                this._substitute = TransformUtils.SanitiseForTaxonomy(value);
            }
        }

        public string ValueFilter
        {
            get;
            set;
        }

        public BindableSubstitutionValues()
        {
        }

        public static BindingList<BindableSubstitutionValues> CopyFromItemFieldValueFilterList(CommonSerializableList<ItemFieldValueFilter> list)
        {
            IEnumerable<BindableSubstitutionValues> bindableSubstitutionValues =
                from i in list
                select new BindableSubstitutionValues()
                {
                    Filter = i.Filter,
                    Substitute = i.Substitute,
                    ValueFilter = i.Filter.GetLogicString()
                };
            return new BindingList<BindableSubstitutionValues>(bindableSubstitutionValues.ToList<BindableSubstitutionValues>());
        }

        public static CommonSerializableList<ItemFieldValueFilter> CopyToItemFieldValueFilterList(BindingList<BindableSubstitutionValues> list)
        {
            IEnumerable<ItemFieldValueFilter> itemFieldValueFilters =
                from i in list
                where i.Filter != null
                select new ItemFieldValueFilter()
                {
                    Filter = i.Filter,
                    Substitute = i.Substitute
                };
            return new CommonSerializableList<ItemFieldValueFilter>(itemFieldValueFilters.ToArray<ItemFieldValueFilter>());
        }
    }
}