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

        internal ClipboardHandler()
        {
            _historyList = new List<ClipboardData>();
        }

        internal void AddHistoryItem(ClipboardData clipboardData)
        {
            API.Log(API.LogType.Notice, "HandleClipboardData");

            for (var i = _historyList.Count - 1; i >= 0; i--)
            {
                if (_historyList[i].RenderedString == clipboardData.RenderedString)
                {
                    _historyList.RemoveAt(i);
                }
            }

            _historyList.Insert(0, clipboardData);

            if (_historyList.Count > Measure.Count)
            {
                _historyList.RemoveAt(_historyList.Count - 1);
            }
        }

        internal string GetHistoryItem(int id)
        {
            if (id >= _historyList.Count)
            {
                return string.Empty;
            }

            return _historyList[id].RenderedString;
        }

        internal void DeleteHistoryItem(int id)
        {
            _historyList.RemoveAt(id);
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
    }
}
