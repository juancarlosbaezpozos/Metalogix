using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using Metalogix.UI.WinForms.Widgets.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Widgets
{
	public class TextMonikerEditorPresenter : ITextMonikerEditorPresenter
	{
		private readonly ITextMonikerEditorView _view;

		private readonly IList<Record> _recordList = new List<Record>();

		private PropertyDescriptor _propertyDescriptor;

		private readonly IList<PropertyDescriptor> _propertyDescriptorList = new List<PropertyDescriptor>();

		private bool IsDirty
		{
			get
			{
				TextMoniker value = (TextMoniker)this._propertyDescriptor.GetValue(this._recordList[this.RecordIndex]);
				if (value == null)
				{
					return false;
				}
				return value.GetFullText() != this._view.EditorText;
			}
		}

		public int RecordIndex
		{
			get
			{
				return JustDecompileGenerated_get_RecordIndex();
			}
			set
			{
				JustDecompileGenerated_set_RecordIndex(value);
			}
		}

		private int JustDecompileGenerated_RecordIndex_k__BackingField;

		public int JustDecompileGenerated_get_RecordIndex()
		{
			return this.JustDecompileGenerated_RecordIndex_k__BackingField;
		}

		public void JustDecompileGenerated_set_RecordIndex(int value)
		{
			this.JustDecompileGenerated_RecordIndex_k__BackingField = value;
		}

		public int RecordIndexMax
		{
			get
			{
				return this._recordList.Count - 1;
			}
		}

		private PropertyDescriptor SelectedPropertyDescriptor
		{
			set
			{
				this._propertyDescriptor = value;
				this._view.SelectPropertyDescriptor(value);
			}
		}

		private IList<TextMoniker> TextMonikerList
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "TextMoniker List cannot be null");
				}
				if (value.Count == 0)
				{
					throw new ArgumentOutOfRangeException("value", "TextMoniker List cannot be empty");
				}
				foreach (TextMoniker textMoniker in value)
				{
					if (textMoniker.ParentItem == null || textMoniker.ParentItem.ParentList == null)
					{
						throw new ArgumentOutOfRangeException("value", "TextMoniker Parent Item cannot be null");
					}
					this._recordList.Add(textMoniker.ParentItem);
				}
				this._view.EditorText = value[0].GetFullText();
				this._view.Title = string.Concat("Edit Text - ", value[0].ParentItem.SourceURL);
				this.UpdateButtonsEnables();
				foreach (PropertyDescriptor property in value[0].ParentItem.GetProperties())
				{
					if (!typeof(TextMoniker).IsAssignableFrom(property.PropertyType))
					{
						continue;
					}
					this.AddPropertyDescriptor(property);
					if (property.Name != value[0].Name)
					{
						continue;
					}
					this.SelectedPropertyDescriptor = property;
				}
			}
		}

		public TextMonikerEditorPresenter(ITextMonikerEditorView view, IList<TextMoniker> textMonikers)
		{
			this._view = view;
			this.TextMonikerList = textMonikers;
		}

		private void AddPropertyDescriptor(PropertyDescriptor pd)
		{
			this._propertyDescriptorList.Add(pd);
			this._view.AddPropertyDescriptor(pd);
		}

		public void ChangeProperty(PropertyDescriptor pd)
		{
			this.SaveText(true);
			if (pd != null)
			{
				this._propertyDescriptor = pd;
				this.UpdateTextboxWithCurrentRecordPropertyText();
			}
		}

		public void JumpToRecord(int index)
		{
			if (index >= 0 && index <= this.RecordIndexMax)
			{
				this.SaveText(true);
				this.RecordIndex = index;
				this.UpdateButtonsEnables();
				this.UpdateTextboxWithCurrentRecordPropertyText();
			}
		}

		public void SaveText(bool askFirst)
		{
			if (this.IsDirty && (!askFirst || this._view.ShowSaveConfirmationBox()))
			{
				foreach (PropertyDescriptor property in this._recordList[this.RecordIndex].GetProperties())
				{
					if (property.Name != this._propertyDescriptor.Name)
					{
						continue;
					}
					property.SetValue(this._recordList[this.RecordIndex], this._view.EditorText);
					break;
				}
			}
		}

		private void UpdateButtonsEnables()
		{
			this._view.PrevTransverseEnabled = this.RecordIndex != 0;
			this._view.NextTransverseEnabled = this.RecordIndex != this._recordList.Count - 1;
		}

		private void UpdateTextboxWithCurrentRecordPropertyText()
		{
			this._view.Title = string.Concat("Edit Text - ", this._recordList[this.RecordIndex].SourceURL);
			TextMoniker value = (TextMoniker)this._propertyDescriptor.GetValue(this._recordList[this.RecordIndex]);
			this._view.EditorText = (value != null ? value.GetFullText() : string.Empty);
		}
	}
}