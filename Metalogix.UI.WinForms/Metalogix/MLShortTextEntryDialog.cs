using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix
{
    public partial class MLShortTextEntryDialog : XtraForm
    {
        private const int LABEL_BASE_SIZE = 67;

        private const int BASE_HEIGHT = 100;

        private const int BASE_WIDTH = 296;

        private const int TXTBOX_WIDTH = 75;

        private const int TXTBOX_XPOS = 199;

        private const int TXTBOX_X_INTERVAL = 88;

        private const int Y_INTERVAL = 22;

        private int m_iLongestLabel;

        private int m_iTextBoxWidth = 75;

        private int m_iTextBoxXPos = 199;

        private TextBoxSize m_size = TextBoxSize.Medium;

        private List<TextEntryType> m_Types = new List<TextEntryType>();

        private List<LabelControl> m_labels;

        private List<TextEdit> m_textBoxes;

        private Dictionary<string, object> m_dictionary;

        private List<int> m_iMin = new List<int>();

        private List<int> m_iMax = new List<int>();

        private List<float> m_fMin = new List<float>();

        private List<float> m_fMax = new List<float>();

        public List<char[]> m_lstIllegal = new List<char[]>();

        private IContainer components;

        private TextEdit w_tbTextEntry;

        private LabelControl w_label;

        private SimpleButton w_btnCancel;

        private SimpleButton w_btnOK;

        private ImageList w_imageIcons;

        private Panel w_entryPanel;

        private DevExpress.XtraEditors.VScrollBar w_scrollBar;

        public TextBoxSize EntrySize => m_size;

        public float FloatRangeMax => m_fMax[0];

        public List<float> FloatRangeMaximums => m_fMax;

        public float FloatRangeMin => m_fMin[0];

        public List<float> FloatRangeMinimums => m_fMin;

        public List<char[]> IllegalChars => m_lstIllegal;

        public int IntRangeMax => m_iMax[0];

        public List<int> IntRangeMaximums => m_iMax;

        public int IntRangeMin => m_iMin[0];

        public List<int> IntRangeMinimums => m_iMin;

        public int LongestLabel => m_iLongestLabel;

        public Dictionary<string, object> ReturnDictionary => m_dictionary;

        public object ReturnValue => m_dictionary[w_label.Text];

        public TextEntryType Type
        {
            get
            {
                if (m_Types.Count <= 0)
                {
                    return TextEntryType.String;
                }
                return m_Types[0];
            }
        }

        public List<TextEntryType> Types => m_Types;

        public MLShortTextEntryDialog(string title, string label)
        {
            InitializeComponent();
            BuildDialog(title, label, "", TextBoxSize.Medium, TextEntryType.String);
        }

        public MLShortTextEntryDialog(string title, string label, string defaultText)
        {
            InitializeComponent();
            BuildDialog(title, label, defaultText, TextBoxSize.Medium, TextEntryType.String);
        }

        public MLShortTextEntryDialog(string title, string label, TextBoxSize size)
        {
            InitializeComponent();
            BuildDialog(title, label, "", size, TextEntryType.String);
        }

        public MLShortTextEntryDialog(string title, string label, string defaultText, TextBoxSize size)
        {
            InitializeComponent();
            BuildDialog(title, label, defaultText, size, TextEntryType.String);
        }

        public MLShortTextEntryDialog(string title, string label, TextEntryType type)
        {
            InitializeComponent();
            BuildDialog(title, label, "", TextBoxSize.Medium, type);
        }

        public MLShortTextEntryDialog(string title, string label, string defaultText, TextEntryType type)
        {
            InitializeComponent();
            BuildDialog(title, label, defaultText, TextBoxSize.Medium, type);
        }

        public MLShortTextEntryDialog(string title, string label, TextBoxSize size, TextEntryType type)
        {
            InitializeComponent();
            BuildDialog(title, label, "", size, type);
        }

        public MLShortTextEntryDialog(string title, string label, string defaultText, TextBoxSize size, TextEntryType type)
        {
            InitializeComponent();
            BuildDialog(title, label, defaultText, size, type);
        }

        public bool Add(string label)
        {
            return Add(label, null, TextEntryType.String);
        }

        public bool Add(string label, TextEntryType type)
        {
            return Add(label, null, type);
        }

        public bool Add(string label, string defaultText)
        {
            return Add(label, defaultText, TextEntryType.String);
        }

        public bool Add(string label, string defaultText, TextEntryType type)
        {
            if (base.Visible)
            {
                return false;
            }
            int y = w_label.Bounds.Y + m_labels.Count * 22;
            int num = w_tbTextEntry.Bounds.Y + m_labels.Count * 22;
            LabelControl labelControl = new LabelControl
            {
                AutoSize = true,
                Text = label
            };
            int x = w_label.Bounds.X;
            labelControl.SetBounds(x, y, labelControl.PreferredSize.Width, labelControl.Height);
            labelControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            if (labelControl.Width > LongestLabel)
            {
                m_iLongestLabel = labelControl.Width;
            }
            w_entryPanel.Controls.Add(labelControl);
            m_labels.Add(labelControl);
            TextEdit textEdit = new TextEdit
            {
                TabIndex = m_textBoxes.Count + 1
            };
            w_btnOK.TabIndex++;
            w_btnCancel.TabIndex++;
            textEdit.SetBounds(w_tbTextEntry.Bounds.X, num, w_tbTextEntry.Width, w_tbTextEntry.Height);
            textEdit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            if (defaultText != null)
            {
                textEdit.Text = defaultText;
            }
            w_entryPanel.Controls.Add(textEdit);
            m_textBoxes.Add(textEdit);
            m_Types.Add(type);
            m_iMin.Add(int.MinValue);
            m_iMax.Add(int.MaxValue);
            m_fMin.Add(float.MinValue);
            m_fMax.Add(float.MaxValue);
            m_lstIllegal.Add(null);
            UpdateUI();
            return true;
        }

        public bool Add(string label, string defaultText, TextEntryType type, int min, int max)
        {
            bool flag = Add(label, defaultText, type);
            if (flag)
            {
                SetRange(min, max, m_iMin.Count - 1);
            }
            return flag;
        }

        public bool Add(string label, string defaultText, TextEntryType type, float min, float max)
        {
            bool flag = Add(label, defaultText, type);
            if (flag)
            {
                SetRange(min, max, m_fMin.Count - 1);
            }
            return flag;
        }

        public bool Add(string label, string defaultText, TextEntryType type, char[] illegalChars)
        {
            bool flag = Add(label, defaultText, type);
            if (flag)
            {
                SetIllegalChars(illegalChars, m_lstIllegal.Count - 1);
            }
            return flag;
        }

        private void BuildDialog(string title, string label, string defaultText, TextBoxSize size, TextEntryType type)
        {
            FetchIcon();
            m_iMin.Add(int.MinValue);
            m_iMax.Add(int.MaxValue);
            m_fMin.Add(float.MinValue);
            m_fMax.Add(float.MaxValue);
            m_lstIllegal.Add(null);
            Text = title;
            w_label.Text = label;
            Types.Add(type);
            w_tbTextEntry.Text = defaultText;
            m_labels = new List<LabelControl> { w_label };
            m_textBoxes = new List<TextEdit> { w_tbTextEntry };
            m_iLongestLabel = w_label.Width;
            m_size = size;
            m_iTextBoxXPos = 199 - (int)size * 88;
            m_iTextBoxWidth = 75 + (int)size * 88;
            TextEdit wTbTextEntry = w_tbTextEntry;
            int mITextBoxXPos = m_iTextBoxXPos;
            wTbTextEntry.SetBounds(mITextBoxXPos, w_tbTextEntry.Location.Y, m_iTextBoxWidth, w_tbTextEntry.Height);
            UpdateUI();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FetchIcon()
        {
            string product = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            try
            {
                product = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyProductAttribute))).Product;
            }
            catch
            {
                product = "Content Matrix Console";
            }
            if (w_imageIcons.Images[product] != null)
            {
                base.Icon = Icon.FromHandle(((Bitmap)w_imageIcons.Images[product]).GetHicon());
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.MLShortTextEntryDialog));
            this.w_tbTextEntry = new DevExpress.XtraEditors.TextEdit();
            this.w_label = new DevExpress.XtraEditors.LabelControl();
            this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.w_imageIcons = new System.Windows.Forms.ImageList(this.components);
            this.w_entryPanel = new System.Windows.Forms.Panel();
            this.w_scrollBar = new DevExpress.XtraEditors.VScrollBar();
            ((System.ComponentModel.ISupportInitialize)this.w_tbTextEntry.Properties).BeginInit();
            this.w_entryPanel.SuspendLayout();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_tbTextEntry, "w_tbTextEntry");
            this.w_tbTextEntry.Name = "w_tbTextEntry";
            componentResourceManager.ApplyResources(this.w_label, "w_label");
            this.w_label.Name = "w_label";
            componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new System.EventHandler(On_btnOK_Click);
            this.w_imageIcons.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("w_imageIcons.ImageStream");
            this.w_imageIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.w_imageIcons.Images.SetKeyName(0, "Content Matrix Console");
            componentResourceManager.ApplyResources(this.w_entryPanel, "w_entryPanel");
            this.w_entryPanel.Controls.Add(this.w_scrollBar);
            this.w_entryPanel.Controls.Add(this.w_label);
            this.w_entryPanel.Controls.Add(this.w_tbTextEntry);
            this.w_entryPanel.Name = "w_entryPanel";
            this.w_entryPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(On_Panel_Scrolled);
            componentResourceManager.ApplyResources(this.w_scrollBar, "w_scrollBar");
            this.w_scrollBar.Name = "w_scrollBar";
            this.w_scrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(On_Panel_Scrolled);
            base.AcceptButton = this.w_btnOK;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.w_entryPanel);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "MLShortTextEntryDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)this.w_tbTextEntry.Properties).EndInit();
            this.w_entryPanel.ResumeLayout(false);
            this.w_entryPanel.PerformLayout();
            base.ResumeLayout(false);
        }

        private void On_btnOK_Click(object sender, EventArgs e)
        {
            m_dictionary = new Dictionary<string, object>();
            for (int num = 0; num < m_labels.Count; num++)
            {
                try
                {
                    m_dictionary.Add(m_labels[num].Text, ParseReturnValue(num));
                }
                catch (Exception exception)
                {
                    GlobalServices.ErrorHandler.HandleException("Warning", exception.Message, exception, ErrorIcon.Warning);
                    base.DialogResult = DialogResult.None;
                    m_textBoxes[num].Focus();
                    return;
                }
            }
            base.DialogResult = DialogResult.OK;
        }

        private void On_Panel_Scrolled(object sender, ScrollEventArgs e)
        {
            SuspendLayout();
            int oldValue = e.OldValue - e.NewValue;
            foreach (LabelControl mLabel in m_labels)
            {
                int x = mLabel.Location.X;
                mLabel.Location = new Point(x, mLabel.Location.Y + oldValue);
            }
            foreach (TextEdit mTextBox in m_textBoxes)
            {
                int num = mTextBox.Location.X;
                mTextBox.Location = new Point(num, mTextBox.Location.Y + oldValue);
            }
            w_scrollBar.Value = e.NewValue;
            ResumeLayout();
        }

        private object ParseReturnValue(int index)
        {
            object obj = null;
            string text = m_textBoxes[index].Text;
            switch (Types[index])
            {
                case TextEntryType.String:
                {
                    char[] item = m_lstIllegal[index];
                    if (item != null && text.IndexOfAny(item) >= 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder(item.Length * 4);
                        char[] chrArray = item;
                        foreach (char chr in chrArray)
                        {
                            stringBuilder.Append(((stringBuilder.Length > 0) ? "," : "") + "'" + chr + "'");
                        }
                        throw new Exception("Value cannot contain illegal characters " + stringBuilder.ToString());
                    }
                    return text;
                }
                case TextEntryType.Integer:
                {
                    if (!int.TryParse(text, out var num) || num < m_iMin[index] || num > m_iMax[index])
                    {
                        object[] objArray = new object[5]
                        {
                            "Value must be an integer between ",
                            m_iMin[index],
                            " and ",
                            m_iMax[index],
                            "."
                        };
                        throw new Exception(string.Concat(objArray));
                    }
                    return num;
                }
                case TextEntryType.Bool:
                {
                    if (!bool.TryParse(text, out var flag))
                    {
                        throw new Exception("Value must be true or false");
                    }
                    return flag;
                }
                case TextEntryType.Float:
                {
                    if (!float.TryParse(text, out var single) || single < m_fMin[index] || single > m_fMin[index])
                    {
                        object[] item1 = new object[5]
                        {
                            "Value must be a floating point number between ",
                            m_fMin[index],
                            " and ",
                            m_fMin[index],
                            "."
                        };
                        throw new Exception(string.Concat(item1));
                    }
                    return single;
                }
                default:
                    return text;
            }
        }

        public bool SetIllegalChars(char[] chars)
        {
            return SetIllegalChars(chars, 0);
        }

        public bool SetIllegalChars(char[] chars, int index)
        {
            if (Types[index] != 0)
            {
                return false;
            }
            m_lstIllegal[index] = chars;
            return true;
        }

        public bool SetRange(int newMin, int newMax)
        {
            return SetRange(newMin, newMax, 0);
        }

        public bool SetRange(int newMin, int newMax, int index)
        {
            if (Types[index] != TextEntryType.Integer)
            {
                return false;
            }
            if (newMin > newMax)
            {
                return false;
            }
            if (index >= m_iMin.Count)
            {
                return false;
            }
            m_iMin[index] = newMin;
            m_iMax[index] = newMax;
            return true;
        }

        public bool SetRange(float newMin, float newMax)
        {
            return SetRange(newMin, newMax, 0);
        }

        public bool SetRange(float newMin, float newMax, int index)
        {
            if (Types[index] != TextEntryType.Float)
            {
                return false;
            }
            if (newMin > newMax)
            {
                return false;
            }
            if (index >= m_fMin.Count)
            {
                return false;
            }
            m_fMin[index] = newMin;
            m_fMax[index] = newMax;
            return true;
        }

        public static object Show(string title, string label)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextBoxSize size, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextBoxSize size, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextBoxSize size, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextBoxSize size, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextBoxSize size, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextBoxSize size, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, TextBoxSize size, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object Show(string title, string label, string defaultText, TextBoxSize size, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static Dictionary<string, object> Show(string title, string[] labels)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0]);
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i]);
            }
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static Dictionary<string, object> Show(string title, string[] labels, string[] defaultTexts)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0], defaultTexts[0]);
            if (labels.Length != defaultTexts.Length)
            {
                throw new ArgumentException("Multiline argument array lengths do not match");
            }
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i], defaultTexts[i]);
            }
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static Dictionary<string, object> Show(string title, string[] labels, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0], size);
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i]);
            }
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static Dictionary<string, object> Show(string title, string[] labels, string[] defaultTexts, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0], defaultTexts[0], size);
            if (labels.Length != defaultTexts.Length)
            {
                throw new ArgumentException("Multiline argument array lengths do not match");
            }
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i], defaultTexts[i]);
            }
            mLShortTextEntryDialog.Show();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static object ShowDialog(string title, string label)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextBoxSize size, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextBoxSize size, TextEntryType type)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextBoxSize size, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextBoxSize size, TextEntryType type, float min, float max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextBoxSize size, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextBoxSize size, TextEntryType type, int min, int max)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.SetRange(min, max);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, TextBoxSize size, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, size, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static object ShowDialog(string title, string label, string defaultText, TextBoxSize size, TextEntryType type, char[] illegalChars)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, label, defaultText, size, type);
            mLShortTextEntryDialog.SetIllegalChars(illegalChars);
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnValue;
        }

        public static Dictionary<string, object> ShowDialog(string title, string[] labels)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0]);
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i]);
            }
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static Dictionary<string, object> ShowDialog(string title, string[] labels, string[] defaultTexts)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0], defaultTexts[0]);
            if (labels.Length != defaultTexts.Length)
            {
                throw new ArgumentException("Multiline argument array lengths do not match");
            }
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i], defaultTexts[i]);
            }
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static Dictionary<string, object> ShowDialog(string title, string[] labels, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0], size);
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i]);
            }
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public static Dictionary<string, object> ShowDialog(string title, string[] labels, string[] defaultTexts, TextBoxSize size)
        {
            MLShortTextEntryDialog mLShortTextEntryDialog = new MLShortTextEntryDialog(title, labels[0], defaultTexts[0], size);
            if (labels.Length != defaultTexts.Length)
            {
                throw new ArgumentException("Multiline argument array lengths do not match");
            }
            for (int i = 1; i < labels.Length; i++)
            {
                mLShortTextEntryDialog.Add(labels[i], defaultTexts[i]);
            }
            mLShortTextEntryDialog.ShowDialog();
            if (mLShortTextEntryDialog.DialogResult != DialogResult.OK)
            {
                return null;
            }
            return mLShortTextEntryDialog.ReturnDictionary;
        }

        public override string ToString()
        {
            return w_tbTextEntry.Text;
        }

        private void UpdateUI()
        {
            int entrySize = (int)EntrySize;
            int num = ((entrySize > 0) ? (entrySize - 1) : 0);
            int count = m_labels.Count;
            int num1 = 0;
            if (m_labels.Count <= 5)
            {
                w_scrollBar.Enabled = false;
                w_scrollBar.Visible = false;
            }
            else
            {
                count = 5;
                num1 = 15;
                if (m_labels.Count == 6)
                {
                    foreach (TextEdit mTextBox in m_textBoxes)
                    {
                        Point location = mTextBox.Location;
                        Point point = mTextBox.Location;
                        mTextBox.Location = new Point(location.X - num1, point.Y);
                    }
                    SimpleButton wBtnCancel = w_btnCancel;
                    Point location1 = w_btnCancel.Location;
                    Point point1 = w_btnCancel.Location;
                    wBtnCancel.Location = new Point(location1.X - num1, point1.Y);
                    SimpleButton wBtnOK = w_btnOK;
                    Point location2 = w_btnOK.Location;
                    Point point2 = w_btnOK.Location;
                    wBtnOK.Location = new Point(location2.X - num1, point2.Y);
                    w_scrollBar.Enabled = true;
                    w_scrollBar.Visible = true;
                }
            }
            base.Width = 296 + LongestLabel - 67 + num * 88 + num1;
            base.Height = 100 + 22 * count;
            DevExpress.XtraEditors.VScrollBar wScrollBar = w_scrollBar;
            wScrollBar.Maximum = w_entryPanel.PreferredSize.Height - w_entryPanel.Height + 9;
        }
    }
}
