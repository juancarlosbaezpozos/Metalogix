using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using Metalogix.Actions;
using Metalogix.Transformers;
using Metalogix.UI.WinForms.Interfaces;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Transformers;

namespace Metalogix.UI.WinForms.Components
{
    public class LeftNavigableTabsForm : XtraForm
    {
        private delegate void SetEnableStateDelegate(bool enable);

        protected const int THREE_STATE_WIDTH_OFFSET = 90;

        protected const int BASE_BUTTON_WIDTH = 219;

        private const float DESIRED_SIZE_ASPECT_RATIO = 1.5f;

        private TCTransformation m_transformerTab;

        private Metalogix.Actions.Action _action;

        private bool? _threeStateConfiguration;

        private Image _image;

        private List<TabbableControl> _tabs;

        private TabbableControl.TabMessageDelegate m_tabMessageListener;

        private TabbableControl.AsyncLoadStartedDelegate m_asyncLoadStartedListener;

        private TabbableControl.AsyncLoadCompleteDelegate m_asyncLoadCompletedListener;

        private volatile int waitingOnCount;

        private object waitingOnCountLock = new object();

        private int _clientPadding_Top;

        private int _clientPadding_Bottom;

        private int _clientPadding_Left;

        private int _clientPadding_Right;

        private int _sizePadding_Vertical;

        private int _sizePadding_Horizontal;

        private IContainer components;

        protected SimpleButton w_btnCancel;

        protected SimpleButton w_btnOK;

        protected SimpleButton w_btnSave;

        protected DevExpress.XtraTab.XtraTabControl tabControl;

        protected virtual int AbsoluteMinimumWidth
        {
            get
		{
			int num = 219;
			if (ThreeStateConfiguration)
			{
				num += 90;
			}
			return num;
		}
        }

        public virtual Metalogix.Actions.Action Action
        {
            get
		{
			if (_action == null)
			{
				Metalogix.Actions.Action action;
				try
				{
					if (!(ActionType == null) && !ActionType.IsAbstract)
					{
						_action = (Metalogix.Actions.Action)Activator.CreateInstance(ActionType);
						return _action;
					}
					action = _action;
				}
				catch (Exception)
				{
					return _action;
				}
				return action;
			}
			return _action;
		}
            set
		{
			_action = value;
			ActionType = _action.GetType();
		}
        }

        public Type ActionType { get; private set; }

        private TabbableControl.AsyncLoadCompleteDelegate AsyncLoadCompletedListener
        {
            get
		{
			if (m_asyncLoadCompletedListener == null)
			{
				m_asyncLoadCompletedListener = On_AsyncLoadCompleted;
			}
			return m_asyncLoadCompletedListener;
		}
        }

        private TabbableControl.AsyncLoadStartedDelegate AsyncLoadStartedListener
        {
            get
		{
			if (m_asyncLoadStartedListener == null)
			{
				m_asyncLoadStartedListener = On_AsyncLoadStarted;
			}
			return m_asyncLoadStartedListener;
		}
        }

        public ConfigurationResult ConfigurationResult { get; set; }

        public virtual ActionContext Context { get; set; }

        public Image Image
        {
            get
		{
			if (_image == null && Action != null)
			{
				_image = Action.GetImage(Context) as Bitmap;
			}
			return _image;
		}
            set
		{
			_image = value;
		}
        }

        public bool IsModeSwitched { get; set; }

        private TabbableControl.TabMessageDelegate TabMessageListener
        {
            get
		{
			if (m_tabMessageListener == null)
			{
				m_tabMessageListener = On_TabMessageSent;
			}
			return m_tabMessageListener;
		}
        }

        public List<TabbableControl> Tabs
        {
            get
		{
			return _tabs;
		}
            set
		{
			RemoveTabEventListeners();
			if (m_transformerTab != null && !value.Contains(m_transformerTab))
			{
				value.Add(m_transformerTab);
			}
			_tabs = value;
			LoadTabs();
		}
        }

        public bool ThreeStateConfiguration
        {
            get
		{
			if (!_threeStateConfiguration.HasValue && ActionType != null)
			{
				object[] customAttributes = ActionType.GetCustomAttributes(typeof(SupportsThreeStateConfigurationAttribute), inherit: true);
				if (customAttributes.Length != 0)
				{
					_threeStateConfiguration = ((SupportsThreeStateConfigurationAttribute)customAttributes[0]).SupportsThreeStateConfiguration;
				}
			}
			if (!_threeStateConfiguration.HasValue)
			{
				_threeStateConfiguration = false;
			}
			return _threeStateConfiguration.Value;
		}
            set
		{
			_threeStateConfiguration = value;
		}
        }

        protected TCTransformation TransformerTab => m_transformerTab;

        public LeftNavigableTabsForm()
	{
		InitializeComponent();
		InitializeSizeCalculationValues();
		ConfigurationResult = ConfigurationResult.Cancel;
		DoubleBuffered = true;
		StackFrame[] frames = new StackTrace().GetFrames();
		TypeFilter typeFilter = Metalogix.Actions.Action.InterfaceFilter;
		for (int i = 0; i < frames.Length; i++)
		{
			Type reflectedType = frames[i].GetMethod().ReflectedType;
			if (reflectedType != null && reflectedType.IsSubclassOf(typeof(Metalogix.Actions.Action)))
			{
				ActionType = reflectedType;
				break;
			}
			if (reflectedType.FindInterfaces(typeFilter, typeof(IActionConfig)).Length == 0)
			{
				continue;
			}
			object[] customAttributes = reflectedType.GetCustomAttributes(typeof(ActionConfigAttribute), inherit: true);
			if (customAttributes.Length != 0)
			{
				ActionConfigAttribute actionConfigAttribute = (ActionConfigAttribute)customAttributes[0];
				if (actionConfigAttribute.ActionTypes.Length != 0)
				{
					ActionType = actionConfigAttribute.ActionTypes[0];
					break;
				}
			}
		}
	}

        protected bool AttemptSaveTabs()
	{
		foreach (TabbableControl current in _tabs)
		{
			current.IsModeSwitched = IsModeSwitched;
			if (current.SaveUI())
			{
				continue;
			}
			if (current.Parent != null)
			{
				if (current.Parent is XtraTabPage)
				{
					tabControl.SelectedTabPage = current.Parent as XtraTabPage;
				}
				else if (current.Parent is NavigationPage)
				{
					SetSelectedTab(current);
				}
				return false;
			}
			return false;
		}
		return true;
	}

        private int CalculateIntendedHeightByWidth(int width)
	{
		return (int)((float)width / 1.5f + 1f);
	}

        private int CalculateIntendedWidthByHeight(int height)
	{
		return (int)((float)height * 1.5f + 1f);
	}

        private int CalculateMinHeightByNumberOfTabs()
	{
		return tabControl.TabPages.Count * 44 + 4 + _clientPadding_Top + _clientPadding_Bottom + _sizePadding_Vertical;
	}

        private Size CalculateMinimumSizeByPages()
	{
		int _clientPaddingLeft = 0;
		int _clientPaddingTop = 0;
		foreach (XtraTabPage tabPage in tabControl.TabPages)
		{
			Size size = tabControl.CalcSizeByPageClient(tabPage.ClientSize);
			_clientPaddingLeft = Math.Max(_clientPaddingLeft, size.Width);
			_clientPaddingTop = Math.Max(_clientPaddingTop, size.Height);
		}
		_clientPaddingLeft = _clientPaddingLeft + _clientPadding_Left + _clientPadding_Right + _sizePadding_Horizontal;
		_clientPaddingTop = _clientPaddingTop + _clientPadding_Top + _clientPadding_Bottom + _sizePadding_Vertical;
		return new Size(_clientPaddingLeft, _clientPaddingTop);
	}

        private Rectangle CalculateTabControlArea()
	{
		int _clientPaddingLeft = _clientPadding_Left;
		int _clientPaddingTop = _clientPadding_Top;
		int width = base.ClientSize.Width - _clientPaddingLeft - _clientPadding_Right;
		int height = base.ClientSize.Height - _clientPaddingTop - _clientPadding_Bottom;
		return new Rectangle(_clientPaddingLeft, _clientPaddingTop, width, height);
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        public void EnableTransformerConfiguration(Metalogix.Actions.Action action, ActionContext context, TransformerCollection collection)
	{
		if (tabControl.TabPages.Count == 1)
		{
			tabControl.ShowTabHeader = DefaultBoolean.True;
		}
		if (m_transformerTab == null)
		{
			m_transformerTab = new TCTransformation(action, context);
		}
		m_transformerTab.PersistentTransformerCollection = collection;
		if (_tabs != null && !_tabs.Contains(m_transformerTab))
		{
			_tabs.Add(m_transformerTab);
			LoadTab(m_transformerTab);
			UpdateMinimumSize();
		}
		foreach (TabbableControl _tab in _tabs)
		{
			if (_tab is ITransformerTabConfig currentTransformerCollection)
			{
				currentTransformerCollection.Action = action;
				currentTransformerCollection.Context = context;
				currentTransformerCollection.Transformers = m_transformerTab.CurrentTransformerCollection;
			}
		}
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Components.LeftNavigableTabsForm));
		this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_btnSave = new DevExpress.XtraEditors.SimpleButton();
		this.tabControl = new DevExpress.XtraTab.XtraTabControl();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
		this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.w_btnCancel.Name = "w_btnCancel";
		componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
		this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.w_btnOK.Name = "w_btnOK";
		this.w_btnOK.Click += new System.EventHandler(On_btnOK_Clicked);
		componentResourceManager.ApplyResources(this.w_btnSave, "w_btnSave");
		this.w_btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.w_btnSave.Name = "w_btnSave";
		this.w_btnSave.Click += new System.EventHandler(On_Save_Clicked);
		componentResourceManager.ApplyResources(this.tabControl, "tabControl");
		this.tabControl.LookAndFeel.SkinName = "Office 2013";
		this.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.tabControl.Name = "tabControl";
		base.AcceptButton = this.w_btnOK;
		base.Appearance.BackColor = (System.Drawing.Color)componentResourceManager.GetObject("LeftNavigableTabsForm.Appearance.BackColor");
		base.Appearance.Options.UseBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.w_btnCancel;
		base.Controls.Add(this.tabControl);
		base.Controls.Add(this.w_btnCancel);
		base.Controls.Add(this.w_btnOK);
		base.Controls.Add(this.w_btnSave);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "LeftNavigableTabsForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(LeftNavigableTabsForm_FormClosing);
		base.Load += new System.EventHandler(On_Load);
		base.Shown += new System.EventHandler(On_Shown);
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		base.ResumeLayout(false);
	}

        protected virtual void InitializeSizeCalculationValues()
	{
		Size clientSize = base.ClientSize;
		Size size = tabControl.ClientSize;
		Size size1 = base.Size;
		_clientPadding_Top = tabControl.Location.Y;
		_clientPadding_Left = tabControl.Location.X;
		_clientPadding_Bottom = clientSize.Height - _clientPadding_Top - size.Height;
		_clientPadding_Right = clientSize.Width - _clientPadding_Left - size.Width;
		_sizePadding_Vertical = size1.Height - clientSize.Height;
		_sizePadding_Horizontal = size1.Width - clientSize.Width;
	}

        private void LeftNavigableTabsForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		foreach (TabbableControl _tab in _tabs)
		{
			try
			{
				_tab.CancelOperation();
			}
			catch
			{
			}
		}
	}

        private void LoadTab(TabbableControl tab)
	{
		if (tab.GetType().CheckPreconditions())
		{
			XtraTabPage padding = tabControl.TabPages.Add();
			Size size = tab.Size;
			Size size1 = tab.Size;
			Size size2 = new Size(size.Width + 30, size1.Height + 20);
			padding.MinimumSize = size2;
			padding.Controls.Add(tab);
			padding.Padding = new Padding(15, 10, 15, 10);
			padding.ImagePadding = new Padding(0, -5, 5, -5);
			tab.TabMessageSent += TabMessageListener;
			tab.AsyncLoadStarted += AsyncLoadStartedListener;
			tab.AsyncLoadCompleted += AsyncLoadCompletedListener;
			if (tab.Image != null)
			{
				padding.Text = tab.ControlName;
				padding.Image = tab.Image;
			}
			tab.Dock = DockStyle.Fill;
		}
	}

        protected virtual void LoadTabs()
	{
		tabControl.TabPages.Clear();
		foreach (TabbableControl _tab in _tabs)
		{
			LoadTab(_tab);
		}
		if (tabControl.TabPages.Count > 1)
		{
			tabControl.SelectedTabPageIndex = 0;
		}
		if (tabControl.TabPages.Count > 1)
		{
			tabControl.ShowTabHeader = DefaultBoolean.True;
		}
		else
		{
			tabControl.ShowTabHeader = DefaultBoolean.False;
		}
		UpdateMinimumSize();
	}

        private void On_AsyncLoadCompleted(TabbableControl sender)
	{
		bool flag = false;
		lock (waitingOnCountLock)
		{
			waitingOnCount--;
			flag = waitingOnCount <= 0;
		}
		if (flag)
		{
			SetEnableState(enable: true);
		}
	}

        private void On_AsyncLoadStarted(TabbableControl sender)
	{
		lock (waitingOnCountLock)
		{
			waitingOnCount++;
		}
		SetEnableState(enable: false);
	}

        private void On_btnOK_Clicked(object sender, EventArgs e)
	{
		IsModeSwitched = false;
		if (!ValidateOptions())
		{
			base.DialogResult = DialogResult.None;
		}
		if (!SaveUI())
		{
			base.DialogResult = DialogResult.None;
		}
		else
		{
			ConfigurationResult = ConfigurationResult.Run;
		}
	}

        protected virtual void On_Load(object sender, EventArgs e)
	{
		SuspendLayout();
		if (!ThreeStateConfiguration)
		{
			w_btnOK.Text = Resources.OK;
			SimpleButton wBtnOK = w_btnOK;
			Point location = w_btnCancel.Location;
			Point point = w_btnCancel.Location;
			wBtnOK.Location = new Point(location.X - 90, point.Y);
			base.Controls.Remove(w_btnSave);
		}
		else
		{
			w_btnOK.Text = Resources.Run;
			SimpleButton wBtnSave = w_btnSave;
			Point location1 = w_btnCancel.Location;
			Point point1 = w_btnCancel.Location;
			wBtnSave.Location = new Point(location1.X - 90, point1.Y);
			SimpleButton simpleButton = w_btnOK;
			Point location2 = w_btnCancel.Location;
			Point point2 = w_btnCancel.Location;
			simpleButton.Location = new Point(location2.X - 180, point2.Y);
		}
		ResumeLayout(performLayout: true);
	}

        private void On_Save_Clicked(object sender, EventArgs e)
	{
		IsModeSwitched = false;
		if (!ValidateOptions())
		{
			base.DialogResult = DialogResult.None;
		}
		if (!SaveUI())
		{
			base.DialogResult = DialogResult.None;
		}
		else
		{
			ConfigurationResult = ConfigurationResult.Save;
		}
	}

        protected virtual void On_Shown(object sender, EventArgs e)
	{
		Rectangle rectangle = CalculateTabControlArea();
		tabControl.Location = rectangle.Location;
		tabControl.Size = rectangle.Size;
		tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		if (Action == null)
		{
			return;
		}
		try
		{
			Bitmap image = (Bitmap)Image;
			if (image != null)
			{
				Icon icon = (base.Icon = Icon.FromHandle(image.GetHicon()));
				base.ShowIcon = icon != null;
			}
		}
		catch (Exception)
		{
		}
	}

        private void On_TabMessageSent(TabbableControl sender, string sMessage, object oValue)
	{
		foreach (TabbableControl tab in Tabs)
		{
			if (tab != sender)
			{
				tab.HandleMessage(sender, sMessage, oValue);
			}
		}
	}

        private void RemoveTabEventListeners()
	{
		if (_tabs == null)
		{
			return;
		}
		foreach (TabbableControl _tab in _tabs)
		{
			_tab.TabMessageSent -= TabMessageListener;
		}
	}

        protected virtual bool SaveUI()
	{
		if (!AttemptSaveTabs())
		{
			return false;
		}
		if (TransformerTab != null)
		{
			TransformerTab.PersistentTransformerCollection.Clear();
			TransformerTab.PersistentTransformerCollection.AddRange(TransformerTab.OutputTransformerCollection.ToArray());
		}
		return true;
	}

        private void SetEnableState(bool enable)
	{
		if (!base.InvokeRequired)
		{
			w_btnOK.Enabled = enable;
			w_btnSave.Enabled = enable;
			return;
		}
		SetEnableStateDelegate setEnableStateDelegate = SetEnableState;
		object[] objArray = new object[1] { enable };
		BeginInvoke(setEnableStateDelegate, objArray);
	}

        protected virtual void SetSelectedTab(TabbableControl tab)
	{
	}

        private void UpdateMinimumSize()
	{
		Size size = new Size(0, 0);
		Size height = new Size(0, 0);
		Size size1 = CalculateMinimumSizeByPages();
		int num = CalculateMinHeightByNumberOfTabs();
		size.Height = Math.Max(size1.Height, num);
		size.Width = Math.Max(size1.Width, AbsoluteMinimumWidth);
		int num1 = CalculateIntendedWidthByHeight(size.Height);
		int num2 = CalculateIntendedHeightByWidth(size.Width);
		if (num1 > size.Width)
		{
			height.Height = size.Height;
			height.Width = num1;
		}
		else if (num2 > size.Height)
		{
			height.Height = num2;
			height.Width = size.Width;
		}
		MinimumSize = size;
		base.Size = height;
	}

        protected virtual bool Validate(out string message)
	{
		message = string.Empty;
		return true;
	}

        private bool ValidateOptions()
	{
		if (Validate(out var str))
		{
			return true;
		}
		FlatXtraMessageBox.Show(this, str);
		return false;
	}
    }
}
