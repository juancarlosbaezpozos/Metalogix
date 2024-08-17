using Metalogix;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components.AnchoredControls;
using Metalogix.UI.WinForms.Tooltips;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TooltipsTest
{
	public class HelpTipButton : AnchoredPictureBox
	{
		public const int buttonWidth = 20;

		public const int buttonHeight = 20;

		public bool IsBasicViewHelpIcon;

		protected HelpTipDialog helpTipDialog;

		protected System.Drawing.Image helpImage;

		protected System.Drawing.Image helpImageDisabled;

		protected Assembly resourceAssembly;

		protected string resourceString;

		public HelpTipButton()
		{
			base.SetStyle(ControlStyles.FixedWidth | ControlStyles.FixedHeight, true);
			this.BackColor = Color.Transparent;
			base.SizeMode = PictureBoxSizeMode.CenterImage;
			base.Size = new System.Drawing.Size(20, 20);
			this.MaximumSize = base.Size;
			this.MinimumSize = base.Size;
			this.helpImage = ImageCache.GetImage("Metalogix.UI.WinForms.Icons.Help16.png", base.GetType().Assembly);
			this.helpImageDisabled = ImageCache.GetImage("Metalogix.UI.WinForms.Icons.Help16Disabled.png", base.GetType().Assembly);
			base.Image = this.helpImage;
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(0, 0, base.Width - 1, base.Height - 1);
			base.Region = new System.Drawing.Region(graphicsPath);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (string.IsNullOrEmpty(this.resourceString))
			{
				FlatXtraMessageBox.Show(string.Format("resourceString is null or empty.{0}Ensure SetResourceString() has been called for the help tip button.{0}Ensure there is the appropriate entry in Tooltips.resx", Environment.NewLine), "HelpTipButton Warning", MessageBoxButtons.OK);
				return;
			}
			if (this.helpTipDialog == null || this.helpTipDialog.IsDisposed)
			{
				this.helpTipDialog = new HelpTipDialog()
				{
					StartPosition = FormStartPosition.Manual
				};
			}
			string resourceName = TooltipManager.TypeNameToResourceName(this.resourceString);
			string str = TooltipManager.TypeNameToResourceName(string.Concat(this.resourceString, "LinkText"));
			this.helpTipDialog.HelpTipText = TooltipManager.GetToolTip(resourceName, this.resourceAssembly);
			this.helpTipDialog.HelpTipLinkText = TooltipManager.GetToolTip(str, this.resourceAssembly);
			string str1 = this.resourceString;
			char[] chrArray = new char[] { '.' };
			StringAccumulator.Message.Send("HelpTipButtonsClicked", str1.Split(chrArray).Last<string>(), false, null);
			Point screen = base.PointToScreen(new Point(0, 0));
			screen.X = screen.X + 25;
			screen.Y = screen.Y - 10;
			Rectangle workingArea = Screen.FromControl(this).WorkingArea;
			if (screen.X + 292 > workingArea.Right)
			{
				screen = base.PointToScreen(new Point(0, 0));
				screen.X = screen.X - 297;
				screen.Y = screen.Y - 10;
				this.helpTipDialog.PlacedOnLeft = true;
			}
			this.helpTipDialog.Location = screen;
			if (this.IsBasicViewHelpIcon)
			{
				this.helpTipDialog.DisableHelpLink();
			}
			this.helpTipDialog.Show();
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			base.Image = (base.Enabled ? this.helpImage : this.helpImageDisabled);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			this.BackColor = SystemColors.ControlDark;
			base.OnMouseHover(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.BackColor = Color.Transparent;
			base.OnMouseLeave(e);
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if ((specified & BoundsSpecified.Height) == BoundsSpecified.None || height == 20)
			{
				base.SetBoundsCore(x, y, width, 20, specified);
			}
		}

		public void SetResourceString(string resourceStringValue, Type assemblyType)
		{
			this.resourceString = resourceStringValue;
			this.resourceAssembly = assemblyType.Assembly;
		}
	}
}