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

        /// <summary>
        /// Start new Form in separate thread
        /// </summary>
        internal static void Start()
        {
            IsStarted = true;
            var t = new Thread(RunForm);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Close current Form
        /// </summary>
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
            // Hide form
            Visible = false;
            ShowInTaskbar = false;

            _mInstance = this;
            // Get pointer to next clipboard viewer in chain
            _clipboardViewer = NativeMethods.SetClipboardViewer(Handle);
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove clipboard viewer from chain
                NativeMethods.ChangeClipboardChain(Handle, _clipboardViewer);
            }

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 

                case WM_DRAWCLIPBOARD:
                    var clipboardData = new ClipboardData(Clipboard.GetDataObject());
                    ClipboardHandler.Current.AddHistoryItem(clipboardData);

                    // Each window that receives the WM_DRAWCLIPBOARD message 
                    // must call the SendMessage function to pass the message 
                    // on to the next window in the clipboard viewer chain.
                    NativeMethods.SendMessage(_clipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                case WM_CHANGECBCHAIN:
                    // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
                    // it should call the SendMessage function to pass the message to the 
                    // next window in the chain, unless the next window is the window 
                    // being removed. In this case, the clipboard viewer should save 
                    // the handle specified by the lParam parameter as the next window in the chain. 

                    // wParam is the Handle to the window being removed from 
                    // the clipboard viewer chain 
                    // lParam is the Handle to the next window in the chain 
                    // following the window being removed. 
                    if (m.WParam == _clipboardViewer)
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain
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
