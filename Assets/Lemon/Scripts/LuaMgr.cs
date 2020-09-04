using System.IO;
using UnityEngine;
using XLua;

namespace Lemon
{
    [LuaCallCSharp]
    public static class LuaMgr
    {
        public static string LuaInitFilePath = "LuaInit";
        public static LuaEnv luaenv = new LuaEnv();
        private static LuaTable scriptEnv = luaenv.NewTable();

        [CSharpCallLua]
        public delegate void BehaviorSendToLua(GameObject go);

        public static void Init()
        {
            AddLoader(LuaInitFilePath);
        }

        public static void DoString(string luaText)
        {
            luaenv.DoString(luaText);
        }

        public static void Set(string key, GameObject value)
        {
            BehaviorSendToLua f = luaenv.Global.GetInPath<BehaviorSendToLua>(key);
            f(value);
            //scriptEnv.Set(key, value);
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
                filepath = Application.dataPath + "/TanghuluGames/LuaProject/" + filepath.Replace('.', '/') + ".lua";
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
