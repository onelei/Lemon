using Lemon.Framework.Log;

namespace Lemon.Framework.BT
{
    public class Debug
    {
        public static void Log(Result _result, string msg)
        {
            switch (_result)
            {
                case Result.Fail:
                    LogManager.LogError("Bt_Result: " + _result);
                    break;
                case Result.Runing:
                    LogManager.LogWarning("Bt_Result: " + _result);
                    break;
                case Result.Successful:
                    LogManager.Log("Bt_Result: " + _result);
                    break;
                default:
                    LogManager.Log("Bt_Result: " + _result);
                    break;
            }
        }
    }
}
