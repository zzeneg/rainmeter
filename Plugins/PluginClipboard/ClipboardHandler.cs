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
        private static ClipboardHandler _current;

        private readonly List<ClipboardData> _historyList;

        internal static ClipboardHandler Current
        {
            get { return _current ?? (_current = new ClipboardHandler()); }
        }

        internal static IntPtr RmSkin;

        internal ClipboardHandler()
        {
            _historyList = new List<ClipboardData>();
        }

        internal void AddHistoryItem(ClipboardData clipboardData)
        {
            API.Log(API.LogType.Notice, "HandleClipboardData");

            for (var i = _historyList.Count - 1; i >= 0; i--)
            {
                if (_historyList[i].ToString() == clipboardData.ToString())
                {
                    _historyList.RemoveAt(i);
                }
            }

            _historyList.Insert(0, clipboardData);

            if (_historyList.Count > Measure.Count)
            {
                _historyList.RemoveAt(_historyList.Count - 1);
            }
            ReloadMeasures();
        }

        internal string GetHistoryItem(int id)
        {
            if (id >= _historyList.Count)
            {
                return string.Empty;
            }

            return _historyList[id].ToString();
        }

        internal void DeleteHistoryItem(int id)
        {
            if (id >= _historyList.Count)
            {
                return;
            }

            _historyList.RemoveAt(id);
            ReloadMeasures();
        }

        internal void SetClipboard(int id)
        {
            var data = _historyList[id].Data;

            var text = data as string;
            if (text != null)
            {
                Clipboard.SetText(text);
                return;
            }

            var collection = data as StringCollection;
            if (collection != null)
            {
                Clipboard.SetFileDropList(collection);
                return;
            }

            var image = data as Image;
            if (image != null)
            {
                Clipboard.SetImage(image);
                return;
            }

            var dataObject = data as IDataObject;
            if (dataObject != null)
            {
                Clipboard.SetDataObject(dataObject);
            }
        }

        private void ReloadMeasures()
        {
            API.Execute(RmSkin, "!Update");
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
