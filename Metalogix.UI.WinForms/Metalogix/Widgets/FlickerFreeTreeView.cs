using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Metalogix.Widgets
{
    public class FlickerFreeTreeView : TreeView
    {
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;

            public int fErase;

            public RECT rcPaint;

            public int fRestore;

            public int fIncUpdate;

            public int Reserved1;

            public int Reserved2;

            public int Reserved3;

            public int Reserved4;

            public int Reserved5;

            public int Reserved6;

            public int Reserved7;

            public int Reserved8;
        }

        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        private BufferedGraphics m_buffGraphics;

        private BufferedGraphicsContext m_buffGraphicsContext;

        private bool m_bUseDoubleBuffering = true;

        private bool m_bSmudgeProtection;

        private bool m_bClearBackground;

        private bool ClearBackground
        {
            get
            {
                if (!m_bClearBackground)
                {
                    return false;
                }
                return SmudgeProtection;
            }
            set
            {
                m_bClearBackground = value;
            }
        }

        public bool SmudgeProtection
        {
            get
            {
                return m_bSmudgeProtection;
            }
            set
            {
                m_bSmudgeProtection = value;
            }
        }

        public bool UseDoubleBuffering
        {
            get
            {
                return m_bUseDoubleBuffering;
            }
            set
            {
                if (m_bUseDoubleBuffering != value && value)
                {
                    InitalizeBufferObjects();
                }
                m_bUseDoubleBuffering = value;
            }
        }

        public FlickerFreeTreeView()
        {
            if (!base.DesignMode)
            {
                m_buffGraphicsContext = BufferedGraphicsManager.Current;
                InitalizeBufferObjects();
            }
        }

        [DllImport("User32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT paintStruct);

        protected void BufferedPaint(Graphics g)
        {
            if (m_buffGraphics != null)
            {
                IntPtr hdc = m_buffGraphics.Graphics.GetHdc();
                Message message = Message.Create(base.Handle, 792, hdc, IntPtr.Zero);
                DefWndProc(ref message);
                m_buffGraphics.Graphics.ReleaseHdc(hdc);
                m_buffGraphics.Render(g);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (m_buffGraphics != null)
                {
                    m_buffGraphics.Dispose();
                }
                if (m_buffGraphicsContext != null)
                {
                    m_buffGraphicsContext.Dispose();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DllImport("User32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT paintStruct);

        private void InitalizeBufferObjects()
        {
            m_buffGraphicsContext.MaximumBuffer = new Size(base.Width + 1, base.Height + 1);
            if (m_buffGraphics != null)
            {
                m_buffGraphics.Dispose();
                m_buffGraphics = null;
            }
            if (base.Width > 0 && base.Height > 0)
            {
                m_buffGraphics = m_buffGraphicsContext.Allocate(CreateGraphics(), new Rectangle(0, 0, base.Width, base.Height));
            }
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.ResumeLayout(false);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            ClearBackground = false;
            base.OnMouseCaptureChanged(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            ClearBackground = false;
            base.OnMouseClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ClearBackground = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ClearBackground = false;
            base.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (UseDoubleBuffering && !base.DesignMode)
            {
                InitalizeBufferObjects();
            }
        }

        protected override void WndProc(ref Message messg)
        {
            if (!base.DesignMode)
            {
                if (messg.Msg == 20 && !ClearBackground)
                {
                    return;
                }
                if (messg.Msg == 15 && UseDoubleBuffering && m_buffGraphics != null)
                {
                    PAINTSTRUCT pAINTSTRUCT = default(PAINTSTRUCT);
                    IntPtr intPtr = BeginPaint(messg.HWnd, ref pAINTSTRUCT);
                    using (Graphics graphic = Graphics.FromHdc(intPtr))
                    {
                        BufferedPaint(graphic);
                    }
                    EndPaint(messg.HWnd, ref pAINTSTRUCT);
                    return;
                }
            }
            base.WndProc(ref messg);
        }
    }
}
