namespace LemonFramework.Log
{
    public interface IAppender
    {
        string GetFilePath();
        void Append(string msg);
        void Dispose();
    }
}