using Lemon.BT;

public class BT_Lemon
{
    public static void Log(BT_Result _result,string msg)
    {
        switch (_result)
        {
            case BT_Result.FAIL:
                QLog.LogError("Bt_Result: " + _result);
                break;
            case BT_Result.RUNING:
                QLog.LogWarning("Bt_Result: " + _result);
                break;
            case BT_Result.SUCCESSFUL:
                QLog.Log("Bt_Result: " + _result);
                break;
            default:
                QLog.Log("Bt_Result: " + _result);
                break;
        } 
    }
}

