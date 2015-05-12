using System;
using System.Threading;
using System.Windows.Forms;
using Rainmeter;

namespace PluginClipboard
{
    internal class ClipboardViewer : Form
    {
        private static ClipboardViewer _mInstance;

        internal delegate void DeviceNotifyDelegate(ClipboardData data);
        internal static event DeviceNotifyDelegate DeviceNotify = ClipboardHandler.Current.AddHistoryItem;

        private static IntPtr _clipboardViewer;

        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        internal static bool IsStarted;

        internal static void Start()
        {
            API.Log(API.LogType.Notice, "ClipboardViewer.Start");
            IsStarted = true;
            Thread t = new Thread(RunForm);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        internal static void Stop()
        {
            API.Log(API.LogType.Notice, "ClipboardViewer.Stop");
            IsStarted = false;
            _mInstance.Invoke(new MethodInvoker(_mInstance.Close));
        }

        private static void RunForm()
        {
            API.Log(API.LogType.Notice, "ClipboardViewer.RunForm");
            Application.Run(new ClipboardViewer());
        }

        protected override void OnLoad(EventArgs e)
        {
            API.Log(API.LogType.Notice, "ClipboardViewer.OnLoad");
            Visible = false;
            ShowInTaskbar = false;

            _mInstance = this;
            _clipboardViewer = (IntPtr)NativeMethods.SetClipboardViewer((int)Handle);
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            API.Log(API.LogType.Notice, "ClipboardViewer.Dispose");
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
                    object data;
                    if (Clipboard.ContainsFileDropList())
                    {
                        data = Clipboard.GetFileDropList();
                    }
                    else if (Clipboard.ContainsImage())
                    {
                        data = Clipboard.GetImage();
                    }
                    else if (Clipboard.ContainsText())
                    {
                        data = Clipboard.GetText();
                    }
                    else
                    {
                        data = Clipboard.GetDataObject();
                    }

                    DeviceNotify(new ClipboardData(data));
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
