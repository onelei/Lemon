namespace Lemon.Framework.BT
{
    public class Debug
    {
        public static void Log(Result _result, string msg)
        {
            switch (_result)
            {
                case Result.Fail:
                    QLog.LogError("Bt_Result: " + _result);
                    break;
                case Result.Runing:
                    QLog.LogWarning("Bt_Result: " + _result);
                    break;
                case Result.Successful:
                    QLog.Log("Bt_Result: " + _result);
                    break;
                default:
                    QLog.Log("Bt_Result: " + _result);
                    break;
            }
        }
    }
}
