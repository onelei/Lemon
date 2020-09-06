using UnityEngine;
using XLua;

namespace Lemon
{
    [LuaCallCSharp]
    public sealed class LuaBehaviour : BaseBehavior
    {
        internal static readonly string EVENT_CALL_LUA = "LuaBehavior.OnEnable";
        internal static readonly string ENABLE = "LuaBehavior.OnEnable";
        internal static readonly string UPDATE = "LuaBehavior.Update";
        internal static readonly string LATEUPDATE = "LuaBehavior.LateUpdate";
         
        private void OnEnable()
        {
            EventMgr.Send(ENABLE, CacheGameObject);
        }

        private void Update()
        {
            EventMgr.Send(UPDATE, CacheGameObject);
        }

        private void LateUpdate()
        {
            EventMgr.Send(LATEUPDATE, CacheGameObject);
        }
    }
}
