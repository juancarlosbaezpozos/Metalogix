using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.UI.WinForms.Data.Mapping;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Proxy
{
    internal class EditProxyAddressControl : UserControl
    {
        private CommonSerializableList<string> _options;

        private IContainer components;

        private Metalogix.UI.WinForms.Data.Mapping.ListControl w_listControlAddress;

        private BarManager _barManager;

        private Bar bar1;

        private BarButtonItem _barBtnAdd;

        private BarButtonItem _barBtnRemove;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        public CommonSerializableList<string> Options
        {
            get
		{
			if (_options != null)
			{
				_options.Clear();
				ListPickerItem[] items = w_listControlAddress.GetItems();
				if (items == null)
				{
					return _options;
				}
				ListPickerItem[] listPickerItemArray = items;
				foreach (ListPickerItem listPickerItem in listPickerItemArray)
				{
					if (listPickerItem != null)
					{
						string target = listPickerItem.Target;
						_options.Add(target);
					}
				}
			}
			return _options;
		}
            set
		{
			_options = value;
			if (_options == null)
			{
				return;
			}
			foreach (string _option in _options)
			{
				AddItem(_option);
			}
		}
        }

        public EditProxyAddressControl()
	{
		InitializeComponent();
	}

        private void AddItem(string address)
	{
		ListPickerItem listPickerItem = new ListPickerItem
		{
			Target = address,
			TargetType = "URL",
			Tag = address
		};
		w_listControlAddress.AddItem(listPickerItem, bReadOnly: false);
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.w_listControlAddress = new Metalogix.UI.WinForms.Data.Mapping.ListControl();
		this._barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.bar1 = new DevExpress.XtraBars.Bar();
		this._barBtnAdd = new DevExpress.XtraBars.BarButtonItem();
		this._barBtnRemove = new DevExpress.XtraBars.BarButtonItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this._barManager).BeginInit();
		base.SuspendLayout();
		this.w_listControlAddress.AllowEdit = false;
		this.w_listControlAddress.AllowNewTagCreation = false;
		this.w_listControlAddress.BackColor = System.Drawing.Color.White;
		this.w_listControlAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.w_listControlAddress.CurrentFilter = null;
		this.w_listControlAddress.CurrentView = null;
		this.w_listControlAddress.Dock = System.Windows.Forms.DockStyle.Fill;
		this.w_listControlAddress.Enabled = false;
		this.w_listControlAddress.ForeColor = System.Drawing.SystemColors.ControlText;
		this.w_listControlAddress.Items = null;
		this.w_listControlAddress.Location = new System.Drawing.Point(0, 0);
		this.w_listControlAddress.MultiSelect = false;
		this.w_listControlAddress.Name = "w_listControlAddress";
		this.w_listControlAddress.SelectedSource = null;
		this.w_listControlAddress.ShowSource = false;
		this.w_listControlAddress.Size = new System.Drawing.Size(456, 318);
		this.w_listControlAddress.Sources = null;
		this.w_listControlAddress.TabIndex = 2;
		this._barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[1] { this.bar1 });
		this._barManager.DockControls.Add(this.barDockControlTop);
		this._barManager.DockControls.Add(this.barDockControlBottom);
		this._barManager.DockControls.Add(this.barDockControlLeft);
		this._barManager.DockControls.Add(this.barDockControlRight);
		this._barManager.Form = this;
		DevExpress.XtraBars.BarItems items = this._barManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[2] { this._barBtnRemove, this._barBtnAdd };
		items.AddRange(barItemArray);
		this._barManager.MaxItemId = 3;
		this.bar1.BarName = "Tools";
		this.bar1.DockCol = 0;
		this.bar1.DockRow = 0;
		this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Right;
		this.bar1.FloatLocation = new System.Drawing.Point(1041, 283);
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this.bar1.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._barBtnAdd),
			new DevExpress.XtraBars.LinkPersistInfo(this._barBtnRemove)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this.bar1.Offset = 38;
		this.bar1.OptionsBar.AllowQuickCustomization = false;
		this.bar1.OptionsBar.DisableCustomization = true;
		this.bar1.OptionsBar.DrawDragBorder = false;
		this.bar1.Text = "Tools";
		this._barBtnAdd.Caption = "Add";
		this._barBtnAdd.Glyph = Metalogix.UI.WinForms.Properties.Resources.Plus;
		this._barBtnAdd.Id = 2;
		this._barBtnAdd.Name = "_barBtnAdd";
		this._barBtnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(On_Add_Clicked);
		this._barBtnRemove.Caption = "Remove";
		this._barBtnRemove.Glyph = Metalogix.UI.WinForms.Properties.Resources.Minus;
		this._barBtnRemove.Id = 1;
		this._barBtnRemove.Name = "_barBtnRemove";
		this._barBtnRemove.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(On_Remove_Clicked);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Size = new System.Drawing.Size(487, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 318);
		this.barDockControlBottom.Size = new System.Drawing.Size(487, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 318);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(456, 0);
		this.barDockControlRight.Size = new System.Drawing.Size(31, 318);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_listControlAddress);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "EditProxyAddressControl";
		base.Size = new System.Drawing.Size(487, 318);
		((System.ComponentModel.ISupportInitialize)this._barManager).EndInit();
		base.ResumeLayout(false);
	}

        private void On_Add_Clicked(object sender, ItemClickEventArgs e)
	{
		NewAddressDialog newAddressDialog = new NewAddressDialog();
		if (newAddressDialog.ShowDialog() == DialogResult.OK)
		{
			AddItem(newAddressDialog.ServerAddress);
		}
	}

        private void On_Remove_Clicked(object sender, ItemClickEventArgs e)
	{
		ListPickerItem[] selectedItems = w_listControlAddress.GetSelectedItems();
		if (selectedItems != null && selectedItems.Length != 0)
		{
			Queue<ListPickerItem> listPickerItems = new Queue<ListPickerItem>(selectedItems);
			while (listPickerItems.Count != 0)
			{
				ListPickerItem listPickerItem = listPickerItems.Dequeue();
				w_listControlAddress.DeleteItem(listPickerItem);
			}
		}
	}
    }
}
