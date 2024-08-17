using Metalogix.Metabase;
using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;

namespace Metalogix.Metabase.DataTypes
{
    [Editor("Metalogix.UI.WinForms.Widgets.TextMonikerUITypeEditor, Metalogix.UI.WinForms", typeof(UITypeEditor))]
    [TypeConverter(typeof(TextMonikerTypeConverter))]
    public class TextMoniker : ITextMoniker, IMetabaseDataType, IComparable<TextMoniker>, IComparable
    {
        protected string m_sName;

        protected string m_sShortValue;

        protected Record m_item;

        public string Name
        {
            get { return this.m_sName; }
        }

        public Record ParentItem
        {
            get { return this.m_item; }
        }

        public TextMoniker(string sName, string sShortValue, Record item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.m_item = item;
            this.m_sName = sName;
            this.m_sShortValue = (sShortValue == null ? string.Empty : sShortValue);
        }

        public int CompareTo(TextMoniker other)
        {
            if (other == null)
            {
                return 1;
            }

            string fullText = this.GetFullText();
            string str = other.GetFullText();
            if (fullText == null && str == null)
            {
                return 0;
            }

            if (fullText == null)
            {
                return -1;
            }

            if (str == null)
            {
                return 1;
            }

            return fullText.CompareTo(str);
        }

        public int CompareTo(object other)
        {
            TextMoniker textMoniker = other as TextMoniker;
            if (textMoniker == null)
            {
                return 1;
            }

            return this.CompareTo(textMoniker);
        }

        public virtual string GetFullText()
        {
            string str = this.m_item.Connection.Adapter.LoadTextMoniker(this) ?? this.m_sShortValue;
            return str;
        }

        public virtual void SetFullText(string sFullText)
        {
            this.m_item.SetValue(this.m_sName, sFullText, this.GetType());
            DataRow dataRow = this.m_item.FindPropertyRow(this.m_sName);
            ArrayList arrayLists = new ArrayList(1);
            arrayLists.Add(dataRow);
            this.m_item.Connection.Adapter.CommitItemProperties(arrayLists);
            if (sFullText == null)
            {
                sFullText = string.Empty;
            }

            this.m_item.Connection.Adapter.SaveTextMoniker(this, sFullText);
        }

        public override string ToString()
        {
            return this.m_sShortValue;
        }
    }
}