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
        }

        internal string GetHistoryItem(int id)
        {
            if (id < _historyList.Count)
            {
                return _historyList[id].ToString();
            }

            return string.Empty;
        }

        internal void DeleteHistoryItem(int id)
        {
            if (id < _historyList.Count)
            {
                _historyList.RemoveAt(id);
            }
        }

        internal void SetHistoryItem(int id)
        {
            if (id < _historyList.Count)
            {
                _historyList[id].SetToClipboard();
            }
        }
    }
}
