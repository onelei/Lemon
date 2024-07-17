using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XLua.Src.Editor.CodeOptimizable
{
    public static class XLuaWrapOptimizeHandler
    {
        private static readonly List<IOptimizable> Optimizables;

        static XLuaWrapOptimizeHandler()
        {
            Optimizables = new List<IOptimizable>()
            {
                new TryArrayGetOptimizable(),
                new TryArraySetOptimizable(),
                new GetDelegateByTypeOptimizable(),
            };
        }

        public static void Optimization()
        {
            var start = DateTime.Now;
            foreach (var codeOptimizable in Optimizables)
            {
                codeOptimizable.Optimization();
            }
            Debug.Log("[XLuaWrapOptimizeHandler] Optimization Success! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
        }

        #region UnitTest

        //[MenuItem("XLua/UnitTest Backup")]
        private static void UnitTest_Optimization()
        {
            new GetDelegateByTypeOptimizable().Backup();
            Refresh();
        }

        //[MenuItem("XLua/UnitTest Restore")]
        private static void UnitTest_Optimization2()
        {
            new GetDelegateByTypeOptimizable().Restore();
            Refresh();
        }

        //[MenuItem("XLua/UnitTest Optimization")]
        private static void UnitTest_Optimization3()
        {
            new GetDelegateByTypeOptimizable().Optimization();
            Refresh();
        }
        
        private static void Refresh()
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
        }

        #endregion
    }
}