using LemonFramework.Log;

namespace LemonFramework.Runtime.Behavior
{
    public class Debug
    {
        public static void Log(Result result, string msg)
        {
            switch (result)
            {
                case Result.Fail:
                    LogManager.LogError("Behavior Result: " + result);
                    break;
                case Result.Runing:
                    LogManager.LogWarning("Behavior Result: " + result);
                    break;
                case Result.Successful:
                    LogManager.Log("Behavior Result: " + result);
                    break;
                default:
                    LogManager.Log("Behavior Result: " + result);
                    break;
            }
        }
    }
}
