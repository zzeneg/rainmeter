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
        internal static List<MyMeasure> Measures = new List<MyMeasure>();

        internal static void HandleClipboardData()
        {
            API.Log(API.LogType.Notice, "HandleClipboardData");
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

            Measures.Insert(0, new MyMeasure(data));
            Measures.RemoveAt(Measures.Count - 1);
        }
    }

    internal class MyMeasure
    {
        public object Data { get; set; }

        public MyMeasure(object data)
        {
            Data = data;
        }

        public override string ToString()
        {
            if (Data == null)
            {
                return string.Empty;
            }

            var collection = Data as StringCollection;
            if (collection != null)
            {
                return "[FILE] " + collection[0];
            }

            var image = Data as Image;
            if (image != null)
            {
                return "[IMG] " + image.Size.Height + image.Size.Width;
            }

            var text = Data as string;
            if (text != null)
            {
                return "[TEXT] " + Data;
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
