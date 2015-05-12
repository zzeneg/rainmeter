using System;
using System.Runtime.InteropServices;
using Rainmeter;

namespace PluginClipboard
{
    internal class Measure
    {
        private readonly int _id;

        internal Measure()
        {
            _id = ClipboardHandler.MeasureCount;
            ClipboardHandler.MeasureCount++;
        }

        internal void Reload(API api, ref double maxValue)
        {

        }

        internal double Update()
        {
            return 0.0;
        }

#if DLLEXPORT_GETSTRING
        internal string GetString()
        {
            if (_id >= ClipboardHandler.ClipboardDataList.Count)
            {
                return string.Empty;
            }

            return ClipboardHandler.ClipboardDataList[_id].ToString();
        }
#endif

#if DLLEXPORT_EXECUTEBANG
        internal void ExecuteBang(string args)
        {
        }
#endif
    }

    public static class Plugin
    {
#if DLLEXPORT_GETSTRING
        static IntPtr StringBuffer = IntPtr.Zero;
#endif

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
            if (!ClipboardViewer.IsStarted) ClipboardViewer.Start();
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            if (ClipboardViewer.IsStarted) ClipboardViewer.Stop();
            GCHandle.FromIntPtr(data).Free();

#if DLLEXPORT_GETSTRING
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
#endif
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure) GCHandle.FromIntPtr(data).Target;
            measure.Reload(new API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure) GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

#if DLLEXPORT_GETSTRING
        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }
#endif

#if DLLEXPORT_EXECUTEBANG
        [DllExport]
        public static void ExecuteBang(IntPtr data, IntPtr args)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.ExecuteBang(Marshal.PtrToStringUni(args));
        }
#endif
    }
}
