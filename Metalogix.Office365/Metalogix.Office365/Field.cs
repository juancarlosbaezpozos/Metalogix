using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public class Field
    {
        public string Access { get; set; }

        public string FieldId { get; set; }

        public bool IsReadOnly { get; set; }

        public string ListItemOverrideValue { get; set; }

        public string Name { get; set; }

        public string TaxonomyHiddenTextFieldId { get; set; }

        public string TaxonomyHiddenTextFieldName { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public Field()
        {
        }
    }
}