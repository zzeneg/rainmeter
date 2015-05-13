using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

namespace PluginClipboard
{
    internal class ClipboardData
    {
        internal object Data { get; private set; }

        internal string RenderedString { get; private set; }

        internal ClipboardData(object data)
        {
            Data = data;
            RenderedString = GetRenderString();
        }

        private string GetRenderString()
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