using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PluginClipboard
{
    internal class Convertor
    {
        internal delegate TResult Func<in T, out TResult>(T arg);

        internal string Format { get; private set; }

        internal Action<object> SetObject { get; private set; }

        internal Func<DataObject, string> GetString { get; private set; }

        internal Func<DataObject, object> GetObject { get; private set; }

        private Convertor(string format, Action<object> setObject, Func<DataObject, object> getObject, Func<DataObject, string> getString)
        {
            Format = format;
            SetObject = setObject;
            GetObject = getObject;
            GetString = getString;
        }

        internal static List<Convertor> AllConvertors = new List<Convertor>
        {
            new Convertor(DataFormats.Bitmap, o => Clipboard.SetImage((Image)o), o => o.GetImage(), o => "[IMG] " + o.GetImage().Size),
            new Convertor(DataFormats.FileDrop, o => Clipboard.SetFileDropList((StringCollection)o), o => o.GetFileDropList(), o => "[FILE] " + Path.GetFileName(o.GetFileDropList()[0])),
            //new Convertor(DataFormats.Rtf, o => Clipboard.SetData(DataFormats.Rtf, o), o => o.GetData(DataFormats.Rtf), o => "[RTF] " + o.GetText().Replace(Environment.NewLine, "/r/n").Trim()),
            new Convertor(DataFormats.Text, o => Clipboard.SetText((string)o), o => o.GetText(), o => o.GetText().Replace(Environment.NewLine, "/r/n").Trim())
        };
    }
}