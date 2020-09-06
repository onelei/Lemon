using UnityEngine;
using UnityEngine.EventSystems;

namespace Lemon
{
    public class EventMgr
    {
        public static void Send(string func_name, float value)
        {
            LuaMgr.GetInPath(func_name, value);
        }

        public static void Send(string func_name, string value)
        {
            LuaMgr.GetInPath(func_name, value);
        }

        public static void Send(string func_name, GameObject value)
        {
            LuaMgr.GetInPath(func_name, value);
        }

        public static void Send(string func_name, object value)
        {
            LuaMgr.GetInPath(func_name, value);
        }

    }
}
