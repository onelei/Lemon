using UnityEngine;
using XLua;
using System;
using System.IO;

namespace Lemon.Framework
{
    [LuaCallCSharp]
    public class LuaLoader : BaseBehavior
    {
        internal static LuaEnv luaEnv = new LuaEnv();
        // 用于处理gc
        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        private LuaTable scriptEnv;

        
        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        private void Awake()
        {
            scriptEnv = luaEnv.NewTable();
            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            // 初始化本地表
            LuaTable meta = luaEnv.NewTable();
            // 把lua化境交给本地表
            meta.Set("__index", luaEnv.Global);
            // 将本地表作为元表，给全局表
            scriptEnv.SetMetaTable(meta);
            //释放本地表
            meta.Dispose();

            //------------------------运行脚本

            //luaEnv.DoString("print(888)");
            //绑定lua的生命周期
            var luaAwake = scriptEnv.Get<Action>("awake");
            scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("ondestroy", out luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake();
            }

        }

        // Use this for initialization
        void Start()
        {
            //虚拟机启动后触发
            if (luaStart != null)
            {
                luaStart();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //lua脚本跟随mono每帧执行
            if (luaUpdate != null)
            {
                luaUpdate();
            }
            //gc相关.清除Lua的未手动释放的LuaBase对象（比如：LuaTable， LuaFunction），以及其它一些事情。
            if (Time.time - LuaLoader.lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                LuaLoader.lastGCTime = Time.time;
            }
        }

        void OnDestroy()
        {
            // 触发销毁相关动作
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
            // 释放lua相关内存
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            scriptEnv.Dispose();
        }
         
    }
}
