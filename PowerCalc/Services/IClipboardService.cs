namespace TAlex.PowerCalc.Services
{
    public interface IClipboardService
    {
        string GetText();
        void SetText(string text);
        object GetData(string format);
        void SetData(string format, object data);
        void SetDataObject(params object[] objs);
    }
}
