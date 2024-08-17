using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using Metalogix.Explorer;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class ScopableLeftNavigableTabsFormBasicView : ScopableLeftNavigableTabsForm
	{
		private int _navigationPageCount;

		private NodeCollection _sourceNodes;

		private IContainer components;

		private NavigationPane navigationPane;

		private SimpleButton btnSwitchToAdvancedView;

		private SimpleButton btnSkipToEnd;

		private SimpleButton btnNext;

		public override bool CanShowBasicModeButton => false;

		public override NodeCollection SourceNodes
		{
			get
			{
				return _sourceNodes;
			}
			set
			{
				_sourceNodes = value;
				foreach (ScopableTabbableControl tab in base.Tabs)
				{
					tab.SourceNodes = value;
				}
			}
		}

		public ScopableLeftNavigableTabsFormBasicView()
		{
			InitializeComponent();
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (_navigationPageCount > 0)
			{
				navigationPane.SelectNextPage();
				if (navigationPane.SelectedPageIndex == _navigationPageCount - 1)
				{
					SetButtonVisibilityForSummaryPage();
				}
			}
		}

		private void btnSkipToEnd_Click(object sender, EventArgs e)
		{
			if (_navigationPageCount > 0)
			{
				navigationPane.SelectedPageIndex = _navigationPageCount - 1;
				SetButtonVisibilityForSummaryPage();
			}
		}

		private void btnSwitchToAdvancedView_Click(object sender, EventArgs e)
		{
			SwitchView(Action, isFromAdvancedView: false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void GetSummaryScreenDetails()
		{
			foreach (TabbableControl tab in base.Tabs)
			{
				if (!(tab is TCSummaryBasicView))
				{
					continue;
				}
				((TCSummaryBasicView)tab).GetSummaryScreenDetails();
				break;
			}
		}

		private void InitializeComponent()
		{
			this.navigationPane = new DevExpress.XtraBars.Navigation.NavigationPane();
			this.btnSwitchToAdvancedView = new DevExpress.XtraEditors.SimpleButton();
			this.btnSkipToEnd = new DevExpress.XtraEditors.SimpleButton();
			this.btnNext = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.navigationPane).BeginInit();
			base.SuspendLayout();
			base._loadActionTemplateButton.Location = new System.Drawing.Point(126, 726);
			base._loadActionTemplateButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base._loadActionTemplateButton.LookAndFeel.UseDefaultLookAndFeel = false;
			base._loadActionTemplateButton.Size = new System.Drawing.Size(199, 23);
			base._loadActionTemplateButton.TabIndex = 2;
			base._loadActionTemplateButton.Text = "&Open Existing Job Configuration";
			base._loadActionTemplateButton.Visible = true;
			base._saveActionTemplateButton.Location = new System.Drawing.Point(331, 726);
			base._saveActionTemplateButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base._saveActionTemplateButton.LookAndFeel.UseDefaultLookAndFeel = false;
			base._saveActionTemplateButton.Size = new System.Drawing.Size(149, 23);
			base._saveActionTemplateButton.TabIndex = 3;
			base._saveActionTemplateButton.Text = "&Save Job Configuration";
			base._saveActionTemplateButton.Visible = true;
			base.w_btnCancel.Location = new System.Drawing.Point(697, 726);
			base.w_btnOK.Location = new System.Drawing.Point(608, 726);
			base.w_btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base.w_btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
			base.w_btnSave.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base.w_btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
			base.tabControl.Appearance.BackColor = System.Drawing.Color.White;
			base.tabControl.Appearance.Options.UseBackColor = true;
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.tabControl.Size = new System.Drawing.Size(760, 702);
			this.navigationPane.AllowResize = false;
			this.navigationPane.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.navigationPane.ItemOrientation = System.Windows.Forms.Orientation.Vertical;
			this.navigationPane.Location = new System.Drawing.Point(0, 0);
			this.navigationPane.Name = "navigationPane";
			this.navigationPane.RegularSize = new System.Drawing.Size(759, 702);
			this.navigationPane.SelectedPage = null;
			this.navigationPane.Size = new System.Drawing.Size(759, 702);
			this.navigationPane.TabIndex = 0;
			this.navigationPane.Text = "Copy List";
			this.navigationPane.StateChanged += new DevExpress.XtraBars.Navigation.StateChangedEventHandler(navigationPane_StateChanged);
			this.navigationPane.SelectedPageChanged += new DevExpress.XtraBars.Navigation.SelectedPageChangedEventHandler(navigationPane_SelectedPageChanged);
			this.btnSwitchToAdvancedView.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.btnSwitchToAdvancedView.Location = new System.Drawing.Point(12, 726);
			this.btnSwitchToAdvancedView.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnSwitchToAdvancedView.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnSwitchToAdvancedView.Name = "btnSwitchToAdvancedView";
			this.btnSwitchToAdvancedView.Size = new System.Drawing.Size(108, 23);
			this.btnSwitchToAdvancedView.TabIndex = 1;
			this.btnSwitchToAdvancedView.Text = "&Advanced Mode";
			this.btnSwitchToAdvancedView.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnSwitchToAdvancedView.Click += new System.EventHandler(btnSwitchToAdvancedView_Click);
			this.btnSkipToEnd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnSkipToEnd.Location = new System.Drawing.Point(608, 726);
			this.btnSkipToEnd.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnSkipToEnd.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnSkipToEnd.Name = "btnSkipToEnd";
			this.btnSkipToEnd.Size = new System.Drawing.Size(82, 23);
			this.btnSkipToEnd.TabIndex = 4;
			this.btnSkipToEnd.Text = "Skip to &End";
			this.btnSkipToEnd.Click += new System.EventHandler(btnSkipToEnd_Click);
			this.btnNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnNext.Location = new System.Drawing.Point(697, 726);
			this.btnNext.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnNext.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(75, 23);
			this.btnNext.TabIndex = 5;
			this.btnNext.Text = "&Next ->";
			this.btnNext.Click += new System.EventHandler(btnNext_Click);
			base.Appearance.BackColor = System.Drawing.Color.White;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(784, 766);
			base.Controls.Add(this.btnNext);
			base.Controls.Add(this.btnSkipToEnd);
			base.Controls.Add(this.btnSwitchToAdvancedView);
			base.Controls.Add(this.navigationPane);
			base.LookAndFeel.SkinName = "Office 2013";
			this.MaximumSize = new System.Drawing.Size(800, 805);
			this.MinimumSize = new System.Drawing.Size(800, 805);
			base.Name = "ScopableLeftNavigableTabsFormBasicView";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "ScopableLeftNavigableTabsFormBasicView";
			base.Controls.SetChildIndex(this.navigationPane, 0);
			base.Controls.SetChildIndex(base.tabControl, 0);
			base.Controls.SetChildIndex(base.w_btnOK, 0);
			base.Controls.SetChildIndex(base.w_btnCancel, 0);
			base.Controls.SetChildIndex(base._loadActionTemplateButton, 0);
			base.Controls.SetChildIndex(base._saveActionTemplateButton, 0);
			base.Controls.SetChildIndex(this.btnSwitchToAdvancedView, 0);
			base.Controls.SetChildIndex(this.btnSkipToEnd, 0);
			base.Controls.SetChildIndex(this.btnNext, 0);
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			((System.ComponentModel.ISupportInitialize)this.navigationPane).EndInit();
			base.ResumeLayout(false);
		}

		protected override void InitializeSizeCalculationValues()
		{
		}

		private void LoadBasicTab(TabbableControl tab)
		{
			NavigationPage navigationPage = new NavigationPage
			{
				Caption = tab.TabName
			};
			navigationPage.Controls.Add(tab);
			navigationPage.Name = tab.Name;
			navigationPage.BackColor = Color.White;
			navigationPage.BorderStyle = BorderStyle.None;
			navigationPage.Properties.ShowCollapseButton = DefaultBoolean.False;
			navigationPage.Properties.ShowExpandButton = DefaultBoolean.False;
			navigationPane.Pages.Add(navigationPage);
			tab.Dock = DockStyle.Fill;
		}

		protected override void LoadTabs()
		{
			foreach (TabbableControl tab in base.Tabs)
			{
				LoadBasicTab(tab);
			}
			base.Controls.Add(navigationPane);
		}

		private void navigationPane_SelectedPageChanged(object sender, SelectedPageChangedEventArgs e)
		{
			if (navigationPane.SelectedPageIndex != _navigationPageCount - 1)
			{
				SetButtonVisibilityForPagesExceptForSummaryPage();
				return;
			}
			SetButtonVisibilityForSummaryPage();
			GetSummaryScreenDetails();
		}

		private void navigationPane_StateChanged(object sender, StateChangedEventArgs e)
		{
			navigationPane.State = NavigationPaneState.Default;
		}

		protected override void On_Load(object sender, EventArgs e)
		{
			_navigationPageCount = navigationPane.Pages.Count;
			UpdateExistingControls();
			base.AcceptButton = btnNext;
			SetLocation();
			base.ActionTemplatesSupported = true;
		}

		protected override void On_Shown(object sender, EventArgs e)
		{
			if (Action == null)
			{
				return;
			}
			try
			{
				Bitmap bitmap = (Bitmap)base.Image;
				if (bitmap != null)
				{
					Icon icon2 = (base.Icon = Icon.FromHandle(bitmap.GetHicon()));
					base.ShowIcon = icon2 != null;
				}
			}
			catch (Exception)
			{
			}
		}

		private void SetBasicViewButtonsVisibility(bool isVisible)
		{
			btnNext.Visible = isVisible;
			btnSkipToEnd.Visible = isVisible;
			btnSwitchToAdvancedView.Visible = isVisible;
			_loadActionTemplateButton.Visible = isVisible;
			_saveActionTemplateButton.Visible = isVisible;
		}

		private void SetButtonVisibilityForPagesExceptForSummaryPage()
		{
			w_btnOK.Visible = false;
			w_btnSave.Visible = false;
			SetBasicViewButtonsVisibility(isVisible: true);
			base.AcceptButton = btnNext;
		}

		private void SetButtonVisibilityForSummaryPage()
		{
			w_btnOK.Visible = true;
			w_btnSave.Visible = true;
			base.AcceptButton = w_btnOK;
			SetBasicViewButtonsVisibility(isVisible: false);
		}

		private void SetLocation()
		{
			_loadActionTemplateButton.Location = new Point(126, 726);
			_saveActionTemplateButton.Location = new Point(331, 726);
			w_btnOK.Location = new Point(558, 726);
			w_btnSave.Location = new Point(647, 726);
		}

		protected override void SetSelectedTab(TabbableControl tab)
		{
			NavigationPage selectedPage = tab.Parent as NavigationPage;
			navigationPane.SelectedPage = selectedPage;
			if (tab is TCMigrationOptionsBasicView)
			{
				((TCMigrationOptionsBasicView)tab).ExpandSelectedElement();
			}
		}

		private void UpdateExistingControls()
		{
			tabControl.Visible = false;
			w_btnCancel.Visible = false;
			w_btnOK.Visible = false;
			w_btnSave.Visible = false;
			w_btnOK.Text = Resources.Run;
		}
	}
}
