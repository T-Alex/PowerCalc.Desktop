using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TAlex.PowerCalc.Services
{
    public class ClipboardService : IClipboardService
    {
        #region IClipboardService Members

        public string GetText()
        {
            return Clipboard.GetText();
        }

        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }

        public object GetData(string format)
        {
            return Clipboard.GetData(format);
        }

        public void SetData(string format, object data)
        {
            Clipboard.SetData(format, data);
        }

        public void SetDataObject(params object[] objs)
        {
            var dataObject = new DataObject();
            foreach (object obj in objs)
            {
                dataObject.SetData(obj);
            }
            Clipboard.SetDataObject(dataObject);
        }

        #endregion
    }
}
