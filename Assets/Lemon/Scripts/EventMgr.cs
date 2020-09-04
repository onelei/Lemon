using UnityEngine;
using UnityEngine.EventSystems;

namespace Lemon
{
    public interface ICustomMessageTarget : IEventSystemHandler
    {
        // 可通过消息系统调用的函数
        void Message1();
        void Message2();
    }

    public class EventMgr
    {

        //private static Dictionary<string,>
        public static void AddLisiner(string eventName, object objs)
        {

        }

        public static void RemoveLisiner(string eventName)
        {

        }


        public static void CallLuaFunc(string funcName, GameObject go)
        { 
            LuaMgr.Set(funcName, go);
        }

        public static void Send(string eventName,params object[] objs)
        {
            //ExecuteEvents.Execute<ICustomMessageTarget>(target, null, (x, y) => x.Message1());
        }
    }
}
