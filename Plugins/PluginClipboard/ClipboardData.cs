using System.Windows.Forms;

namespace PluginClipboard
{
    internal class ClipboardData
    {

        private readonly Convertor _convertor;

        private readonly object _data;

        private readonly string _text;

        internal ClipboardData(DataObject dataObject)
        {
            foreach (var convertor in Convertor.AllConvertors)
            {
                if (dataObject.GetDataPresent(convertor.Format))
                {
                    _convertor = convertor;
                    _data = convertor.GetObject(dataObject);
                    _text = convertor.GetString(dataObject);
                    break;
                }
            }
        }

        internal void SetToClipboard()
        {
            _convertor.SetObject(_data);
        }

        public override string ToString()
        {
            return _text;
        }
    }
}