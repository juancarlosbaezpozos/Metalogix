using DevExpress.XtraEditors;
using Metalogix;
using Metalogix.Actions;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
    public partial class TabbableControl : CollapsableControl
    {
        private string m_controlName;

        private string m_sImageName;

        private Image m_img;

        private bool? m_bUseGroupBox = null;

        private string m_groupName;

        private bool? m_bUseTab = null;

        private string m_tabName;

        private IContainer components;

        public virtual ActionContext Context
        {
            get;
            set;
        }

        public virtual string ControlName
        {
            get
            {
                if (this.m_controlName == null)
                {
                    object[] customAttributes = base.GetType().GetCustomAttributes(typeof(ControlNameAttribute), true);
                    if ((int)customAttributes.Length == 1)
                    {
                        this.m_controlName = ((ControlNameAttribute)customAttributes[0]).Name;
                    }
                }
                return this.m_controlName;
            }
            set
            {
                this.m_controlName = value;
            }
        }

        public virtual string GroupName
        {
            get
            {
                if (this.m_groupName == null)
                {
                    this.m_groupName = this.ControlName;
                }
                return this.m_groupName;
            }
        }

        public virtual Image Image
        {
            get
            {
                if (this.m_img != null)
                {
                    return this.m_img;
                }
                return ImageCache.GetImage(this.ImageName, base.GetType().Assembly);
            }
            set
            {
                this.m_img = value;
            }
        }

        public virtual string ImageName
        {
            get
            {
                if (this.m_sImageName == null)
                {
                    object[] customAttributes = base.GetType().GetCustomAttributes(typeof(ControlImageAttribute), true);
                    if ((int)customAttributes.Length == 1)
                    {
                        this.m_sImageName = ((ControlImageAttribute)customAttributes[0]).ImageName;
                    }
                }
                return this.m_sImageName;
            }
            set
            {
                this.m_sImageName = value;
            }
        }

        public bool IsModeSwitched
        {
            get;
            set;
        }

        public TabbableControl ParentTabbableControl
        {
            get;
            set;
        }

        public virtual string TabName
        {
            get
            {
                if (this.m_tabName == null)
                {
                    this.m_tabName = this.ControlName;
                }
                return this.m_tabName;
            }
        }

        public virtual bool UseGroupBox
        {
            get
            {
                if (!this.m_bUseGroupBox.HasValue)
                {
                    object[] customAttributes = base.GetType().GetCustomAttributes(typeof(UsesGroupBoxAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bUseGroupBox = new bool?(false);
                    }
                    else
                    {
                        this.m_bUseGroupBox = new bool?(((UsesGroupBoxAttribute)customAttributes[0]).Enabled);
                        this.m_groupName = ((UsesGroupBoxAttribute)customAttributes[0]).GroupName;
                    }
                }
                return this.m_bUseGroupBox.Value;
            }
        }

        public virtual bool UseTab
        {
            get
            {
                if (!this.m_bUseTab.HasValue)
                {
                    object[] customAttributes = base.GetType().GetCustomAttributes(typeof(UsesTabControlAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bUseTab = new bool?(false);
                    }
                    else
                    {
                        this.m_bUseTab = new bool?(((UsesTabControlAttribute)customAttributes[0]).Enabled);
                        this.m_tabName = ((UsesTabControlAttribute)customAttributes[0]).TabName;
                    }
                }
                return this.m_bUseTab.Value;
            }
            set
            {
                this.m_bUseTab = new bool?(value);
            }
        }

        public TabbableControl()
        {
        }

        public virtual void CancelOperation()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public virtual ArrayList GetSummaryScreenDetails()
        {
            return new ArrayList();
        }

        public virtual void HandleMessage(TabbableControl sender, string sMessage, object oValue)
        {
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.Name = "TabbableControl";
            base.ResumeLayout(false);
        }

        protected virtual void LoadUI()
        {
        }

        public virtual bool SaveUI()
        {
            return true;
        }

        public void SendMessage(string sMessage, object oValue)
        {
            if (this.TabMessageSent != null)
            {
                this.TabMessageSent(this, sMessage, oValue);
            }
        }

        protected void TriggerAsyncCompleted()
        {
            if (this.AsyncLoadCompleted != null)
            {
                this.AsyncLoadCompleted(this);
            }
        }

        protected void TriggerAsyncStarted()
        {
            if (this.AsyncLoadStarted != null)
            {
                this.AsyncLoadStarted(this);
            }
        }

        internal event TabbableControl.AsyncLoadCompleteDelegate AsyncLoadCompleted;

        internal event TabbableControl.AsyncLoadStartedDelegate AsyncLoadStarted;

        internal event TabbableControl.TabMessageDelegate TabMessageSent;

        internal delegate void AsyncLoadCompleteDelegate(TabbableControl sender);

        internal delegate void AsyncLoadStartedDelegate(TabbableControl sender);

        internal delegate void TabMessageDelegate(TabbableControl sender, string sMessage, object oValue);
    }
}