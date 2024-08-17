using System;
using System.ComponentModel;

namespace Metalogix.UI.WinForms.Widgets.Interface
{
	public interface ITextMonikerEditorView
	{
		string EditorText
		{
			get;
			set;
		}

		bool NextTransverseEnabled
		{
			set;
		}

		bool PrevTransverseEnabled
		{
			set;
		}

		string Title
		{
			set;
		}

		void AddPropertyDescriptor(PropertyDescriptor pd);

		void SelectPropertyDescriptor(PropertyDescriptor value);

		bool ShowSaveConfirmationBox();
	}
}