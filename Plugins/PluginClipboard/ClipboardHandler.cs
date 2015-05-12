using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rainmeter;

namespace PluginClipboard
{
    internal class ClipboardHandler
    {
        internal static List<ClipboardData> ClipboardDataList = new List<ClipboardData>();

        internal static int MeasureCount = 0;

        internal static void HandleClipboardData(object data)
        {
            API.Log(API.LogType.Notice, "HandleClipboardData");

            var clipboardData = new ClipboardData(data);

            for (var i = ClipboardDataList.Count - 1; i >= 0; i--)
            {
                if (ClipboardDataList[i].ToString() == clipboardData.ToString())
                {
                    ClipboardDataList.RemoveAt(i);
                }
            }

            ClipboardDataList.Insert(0, clipboardData);

            if (ClipboardDataList.Count > MeasureCount)
            {
                ClipboardDataList.RemoveAt(ClipboardDataList.Count - 1);
            }
        }
    }

    internal class ClipboardData
    {
        internal object Data { get; set; }

        internal ClipboardData(object data)
        {
            Data = data;
        }

        public override string ToString()
        {
            if (Data == null)
            {
                return string.Empty;
            }

            var text = Data as string;
            if (text != null)
            {
                return text.Replace(Environment.NewLine, "/r/n").Trim();
            }

            var collection = Data as StringCollection;
            if (collection != null)
            {
                return "[FILE]" + collection[0];
            }

            var image = Data as Image;
            if (image != null)
            {
                return "[IMG]" + image.Size.Height + image.Size.Width;
            }

            var dataObject = Data as IDataObject;
            if (dataObject != null)
            {
                return "[DATA] " + dataObject.GetData(dataObject.GetFormats()[0]);
            }

            return "Conversion error";
        }
    }
}
