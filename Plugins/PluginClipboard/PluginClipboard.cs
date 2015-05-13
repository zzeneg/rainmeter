using System;
using System.Runtime.InteropServices;
using Rainmeter;

namespace PluginClipboard
{
    internal class Measure
    {
        private readonly int _id;

        internal static int Count;

        internal Measure()
        {
            _id = Measure.Count;
            Measure.Count++;
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
            return ClipboardHandler.Current.GetHistoryItem(_id);
        }
#endif

#if DLLEXPORT_EXECUTEBANG
        internal void ExecuteBang(string args)
        {
            API.Log(API.LogType.Notice, args);
            switch (args)
            {
                case "Set":
                    ClipboardHandler.Current.SetHistoryItem(_id);
                    break;
                case "Delete":
                    ClipboardHandler.Current.DeleteHistoryItem(_id);
                    break;
                default:
                    API.Log(API.LogType.Error, "Unsupported bang: " + args);
                    break;
            }
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
            if (!ClipboardViewer.IsStarted)
            {
                ClipboardViewer.Start();
            }
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            
            if (ClipboardViewer.IsStarted)
            {
                Measure.Count = 0;
                ClipboardViewer.Stop();
            }
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
