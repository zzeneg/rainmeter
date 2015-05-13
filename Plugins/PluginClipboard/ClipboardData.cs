using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PluginClipboard
{
    internal class ClipboardData
    {
        private readonly DataObject _dataObject;

        private readonly string _text;

        private delegate TResult Func<in T, out TResult>(T arg);

        private readonly Dictionary<string, Func<DataObject, string>> _convertors = new Dictionary
            <string, Func<DataObject, string>>
        {
            {DataFormats.Bitmap, o => "[IMG] " + o.GetImage().Size},
            {DataFormats.FileDrop, o => "[FILE] " + Path.GetFileName(o.GetFileDropList()[0])},
            {DataFormats.Text, o => o.GetText()}
        };

        internal ClipboardData(IDataObject dataObject)
        {
            _dataObject = new DataObject();
            foreach (var format in dataObject.GetFormats())
            {
                _dataObject.SetData(format, dataObject.GetData(format));
                if (_convertors.ContainsKey(format))
                {
                    _text = _convertors[format](_dataObject);
                }
            }
        }

        internal void SetToClipboard()
        {
            Clipboard.SetDataObject(_dataObject);
        }

        public override string ToString()
        {
            return _text;
        }
    }
}