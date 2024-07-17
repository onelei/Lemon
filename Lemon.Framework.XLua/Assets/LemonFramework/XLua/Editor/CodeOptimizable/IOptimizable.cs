namespace XLua.Src.Editor.CodeOptimizable
{
    public interface IOptimizable
    {
        void Backup();
        void Restore();
        string GetFilePath();
        string GetFunctionName();
        void Optimization();
    }
}