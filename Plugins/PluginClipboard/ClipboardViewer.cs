using System;
using System.Threading;
using System.Windows.Forms;

namespace PluginClipboard
{
    internal class ClipboardViewer : Form
    {
        private static ClipboardViewer _mInstance;

        private static IntPtr _clipboardViewer;

        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        internal static bool IsStarted;

        internal static void Start()
        {
            IsStarted = true;
            Thread t = new Thread(RunForm);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        internal static void Stop()
        {
            IsStarted = false;
            _mInstance.Invoke(new MethodInvoker(_mInstance.Close));
        }

        private static void RunForm()
        {
            Application.Run(new ClipboardViewer());
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            _mInstance = this;
            _clipboardViewer = (IntPtr)NativeMethods.SetClipboardViewer((int)Handle);
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NativeMethods.ChangeClipboardChain(Handle, _clipboardViewer);
            }

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    var clipboardData = new ClipboardData(Clipboard.GetDataObject());
                    ClipboardHandler.Current.AddHistoryItem(clipboardData);
                    NativeMethods.SendMessage(_clipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == _clipboardViewer)
                        _clipboardViewer = m.LParam;
                    else
                        NativeMethods.SendMessage(_clipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
