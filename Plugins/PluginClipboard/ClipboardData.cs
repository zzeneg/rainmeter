using System;
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

        /// <summary>
        /// Serializers for different DataFormats
        /// </summary>
        private readonly Dictionary<string, Func<DataObject, string>> _convertors = new Dictionary
            <string, Func<DataObject, string>>
        {
            {DataFormats.Bitmap, o => "[IMG] " + o.GetImage().Size},
            {DataFormats.FileDrop, o => "[FILE] " + Path.GetFileName(o.GetFileDropList()[0])},
            {DataFormats.Text, o => o.GetText()}
        };

        /// <summary>
        /// </summary>
        /// <see cref="https://social.msdn.microsoft.com/Forums/windows/en-US/ea562ff1-9558-425d-a30a-6a1809224edc/how-best-to-avoid-the-land-mines-planted-in-the-idataobject-returned-from-clipboardgetdataobject"/>
        private readonly string[] _buggyFormats = {DataFormats.EnhancedMetafile, DataFormats.MetafilePict, DataFormats.Palette};

        internal ClipboardData(IDataObject dataObject)
        {
            _dataObject = new DataObject();
            foreach (var format in dataObject.GetFormats())
            {
                if (Array.Exists(_buggyFormats, s => s == format)) continue;
                _dataObject.SetData(format, dataObject.GetData(format));
                if (_convertors.ContainsKey(format))
                {
                    _text = _convertors[format](_dataObject);
                }
            }

            if (_text == null) _text = "[DATA] " + DateTime.Now.ToString("T");
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