namespace LemonFramework.Log
{
    public interface ILogger
    {
        string Name { get; }
        void Log(string msg);
        void LogWarning(string msg);
        void LogError(string msg);
    }
}