using System;
using System.Threading;
using System.Windows.Forms;
using Rainmeter;

namespace PluginClipboard
{
    class ClipboardViewer : Form
    {
        private static ClipboardViewer _mInstance;

        public static void Start()
        {
            Thread t = new Thread(RunForm);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        public static void Stop()
        {
            if (_mInstance == null) throw new InvalidOperationException("Notifier not started");
            _mInstance.Invoke(new MethodInvoker(_mInstance.EndForm));
        }

        private static void RunForm()
        {
            Application.Run(new ClipboardViewer());
        }

        private void EndForm()
        {
            this.Close();
        }

        protected override void SetVisibleCore(bool value)
        {
            // Prevent window getting visible
            if (_mInstance == null) CreateHandle();
            _mInstance = this;
            base.SetVisibleCore(false);
        }

        #region Win32 Clipboard handler

        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        private IntPtr _clipboardViewer;



        //handle windows messages
        protected override void WndProc(ref Message m)
        {
            API.Log(API.LogType.Notice, "WndProc");
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    ClipboardHandler.HandleClipboardData();
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

        #endregion
    }
}
