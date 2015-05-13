using System;
using System.Runtime.InteropServices;
using Rainmeter;

// Overview
/*
 * This plugin helps to work with clipboard - saving it's history and restoring items from history.
 * The amount of stored items is unlimited, but they are lost after restart of Rainmeter.
 * Plugin supports two bangs - "Set" and "Delete". First one puts item from history to clipboard,
 * second deletes it.
 * All types of copied items are supported, but not all of them are serialized to measures correctly.
 * This can be fixed by adding correct element to ClipboardData._convertors dictionary.
*/

// Sample skin:
/*
    [Rainmeter]
    Update=1000

    [sLeft]
    Y=18r
    H=18
    W=200
    ClipString=1
    AntiAlias=1

    [line1]
    Measure=Plugin
    Plugin=PluginClipboard.dll

    [line2]
    Measure=Plugin
    Plugin=PluginClipboard.dll

    [line3]
    Measure=Plugin
    Plugin=PluginClipboard.dll

    [Left1]
    Meter=STRING
    MeterStyle=sLeft
    MeasureName=line1
    Text="%1"
    LeftMouseDownAction=!CommandMeasure "line1" "Set"
    RightMouseDownAction=!CommandMeasure "line1" "Delete"

    [Left2]
    Meter=STRING
    MeterStyle=sLeft
    MeasureName=line2
    Text="%1"
    LeftMouseDownAction=!CommandMeasure "line2" "Set"
    RightMouseDownAction=!CommandMeasure "line2" "Delete"

    [Left3]
    Meter=STRING
    MeterStyle=sLeft
    MeasureName=line3
    Text="%1"
    LeftMouseDownAction=!CommandMeasure "line3" "Set"
    RightMouseDownAction=!CommandMeasure "line3" "Delete"
*/

namespace PluginClipboard
{
    internal class Measure
    {
        /// <summary>
        /// Id of clipboard item in history list
        /// </summary>
        private readonly int _id;

        /// <summary>
        /// Total amount of measures
        /// </summary>
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

        internal string GetString()
        {
            return ClipboardHandler.Current.GetHistoryItem(_id);
        }

        internal void ExecuteBang(string args)
        {
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
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
            // Start clipboard viewer if it's not started yet
            if (!ClipboardViewer.IsStarted)
            {
                ClipboardViewer.Start();
            }
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            // Close clipboard viewer form
            if (ClipboardViewer.IsStarted)
            {
                // Reset total amount of measures
                Measure.Count = 0;
                ClipboardViewer.Stop();
            }
            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
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
        [DllExport]
        public static void ExecuteBang(IntPtr data, IntPtr args)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.ExecuteBang(Marshal.PtrToStringUni(args));
        }
    }
}
