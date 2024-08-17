using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components.AnchoredControls
{
	public interface IAnchoredControl : ISupportInitialize
	{
		AnchorStyles Anchor
		{
			get;
			set;
		}

		Control AnchoringControl
		{
			get;
			set;
		}

		Control CommonParentControl
		{
			get;
			set;
		}

		Point Location
		{
			get;
			set;
		}

		Control Parent
		{
			get;
			set;
		}

		Coordinates RealOffset
		{
			get;
			set;
		}

		Coordinates RelativeOffset
		{
			get;
			set;
		}

		bool Visible
		{
			get;
			set;
		}

		void OnAnchorContextChanged(object sender, EventArgs e);

		void OnAnchorPointChanged(object sender, EventArgs e);

		void OnAnchorVisibleChanged(object sender, EventArgs e);

		event EventHandler ParentChanged;
	}
}