using System;
using System.ComponentModel;

namespace Metalogix.UI.WinForms.Widgets.Interface
{
	public interface ITextMonikerEditorPresenter
	{
		int RecordIndex
		{
			get;
		}

		int RecordIndexMax
		{
			get;
		}

		void ChangeProperty(PropertyDescriptor pd);

		void JumpToRecord(int recordStepIncrement);

		void SaveText(bool askFirst);
	}
}