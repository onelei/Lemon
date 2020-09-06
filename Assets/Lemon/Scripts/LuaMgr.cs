using System.IO;
using UnityEngine;
using XLua;

namespace Lemon
{
    [CSharpCallLua]
    [LuaCallCSharp]
    public static class LuaMgr
    {
        public static string LuaInitFilePath = "LuaInit";
        public static LuaEnv luaenv = new LuaEnv();
        private static LuaTable scriptEnv = luaenv.NewTable();

        [CSharpCallLua]
        public delegate void lua_func_object(object args);
        [CSharpCallLua]
        public delegate void lua_func_string(string args);
        [CSharpCallLua]
        public delegate void lua_func_float(float args);
        [CSharpCallLua]
        public delegate void lua_func_gameobject(GameObject args);

        public static void Init()
        {
            AddLoader(LuaInitFilePath);
        }

        public static void DoString(string luaText)
        {
            luaenv.DoString(luaText);
        }

        public static void GetInPath(string func_name, object value)
        {
            var func = luaenv.Global.GetInPath<lua_func_object>(func_name);
            if (func != null)
            {
                func(value);
            }
        }

        //public static void GetInPath(string func_name, float value)
        //{
        //    var func = luaenv.Global.GetInPath<lua_func_float>(func_name);
        //    if (func != null)
        //    {
        //        func(value);
        //    }
        //}

        //public static void GetInPath(string func_name, string value)
        //{
        //    var func = luaenv.Global.GetInPath<lua_func_string>(func_name);
        //    if (func != null)
        //    {
        //        func(value);
        //    }
        //}

        //public static void GetInPath(string func_name, GameObject value)
        //{
        //    var func = luaenv.Global.GetInPath<lua_func_gameobject>(func_name);
        //    if (func != null)
        //    {
        //        func(value);
        //    }
        //}

        public static void SetInPath<T>(string func_name, T value)
        {
            luaenv.Global.SetInPath(func_name, value);
        }

        public static void AddLoader(string luaFileName)
        {
            luaenv.AddLoader((ref string filename) =>
            {
                filename = Application.dataPath + "/LuaProject/" + filename.Replace('.', '/') + ".lua";
                if (File.Exists(filename))
                {
                    return File.ReadAllBytes(filename);
                }
                else
                {
                    return null;
                }
            });
            luaenv.DoString(string.Format("require('{0}')", luaFileName));
        }

        public static string PUBLIC_KEY = "BgIAAACkAABSU0ExAAQAAAEAAQBVDDC5QJ+0uSCJA+EysIC9JBzIsd6wcXa+FuTGXcsJuwyUkabwIiT2+QEjP454RwfSQP8s4VZE1m4npeVD2aDnY4W6ZNJe+V+d9Drt9b+9fc/jushj/5vlEksGBIIC/plU4ZaR6/nDdMIs/JLvhN8lDQthwIYnSLVlPmY1Wgyatw==";

       
        public static void SignatureLoader()
        { 
#if UNITY_EDITOR
            luaenv.AddLoader(new SignatureLoader(PUBLIC_KEY, (ref string filepath) =>
            {
                filepath = Application.dataPath + "/LuaProject/" + filepath.Replace('.', '/') + ".lua";
                if (File.Exists(filepath))
                {
                    return File.ReadAllBytes(filepath);
                }
                else
                {
                    return null;
                }
            }));
#else //为了让手机也能测试
        luaenv.AddLoader(new SignatureLoader(PUBLIC_KEY, (ref string filepath) =>
        {
            filepath = filepath.Replace('.', '/') + ".lua";
            TextAsset file = (TextAsset)Resources.Load(filepath);
            if (file != null)
            {
                return file.bytes;
            }
            else
            {
                return null;
            }
        }));
#endif
            luaenv.DoString(@"
            require 'signatured1'
            require 'signatured2'
        ");
            luaenv.Dispose();
        }

    }
}
