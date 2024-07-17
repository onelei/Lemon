using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace XLua.Src.Editor.CodeOptimizable
{
    public class TryArraySetOptimizable : IOptimizable
    {
        public void Backup()
        {
            var filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[CodeOptimizable] {filePath} not exist");
                return;
            }

            var lines = File.ReadAllLines(filePath).ToList();
            //备份
            var bakLines = new List<string>(lines);
            var bakFilePath = Path.Combine(Application.dataPath, "XLua/Gen/WrapPusher.bak.cs");
            if (File.Exists(bakFilePath))
            {
                File.Delete(bakFilePath);
            }

            bakLines.Insert(0, "/*");
            bakLines.Insert(bakLines.Count, "*/");
            File.WriteAllLines(bakFilePath, bakLines.ToArray());
            Debug.Log($"[CodeOptimizableHandler] Backup {filePath} Success");
        }

        public void Restore()
        {
            var filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[CodeOptimizable] {filePath} not exist");
                return;
            }

            var bakFilePath = Path.Combine(Application.dataPath, "XLua/Gen/WrapPusher.bak.cs");
            if (!File.Exists(bakFilePath))
            {
                Debug.LogError($"[CodeOptimizable] {bakFilePath} not exist");
                return;
            }

            var lines = File.ReadAllLines(bakFilePath).ToList();
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log($"[CodeOptimizableHandler] Restore {filePath} Success");
        }

        public string GetFilePath()
        {
            return Path.Combine(Application.dataPath, "XLua/Gen/WrapPusher.cs");
        }

        public string GetFunctionName()
        {
            return "__tryArraySet";
        }

        public void Optimization()
        {
            var filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[CodeOptimizable] {filePath} not exist");
                return;
            }

            var lines = File.ReadAllLines(filePath).ToList();
            lines = Generate(lines);
            //结束
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log($"[CodeOptimizableHandler] Generate {filePath} {GetFunctionName()} Success");
        }

        private List<string> Generate(List<string> lines)
        {
            var flagIndex = lines.FindIndex(x =>
                x.Contains(
                    "internal static bool __tryArraySet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int array_idx, int obj_idx)"));
            if (flagIndex < 0)
            {
                Debug.LogError("[CodeOptimizableHandler] {GetFunctionName()} not exist");
                return lines;
            }

            //开始
            var functionIndex = flagIndex + 3;
            var flagEndIndex = 0;
            Dictionary<string, List<string>> typeActionDict = new Dictionary<string, List<string>>(2500);
            for (int i = functionIndex; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Contains("if (type == typeof("))
                {
                    // if (type == typeof(UnityEngine.Vector2[]))
                    // {
                    //     UnityEngine.Vector2[] array = obj as UnityEngine.Vector2[];
                    //     translator.PushUnityEngineVector2(L, array[index]);
                    //     return true;
                    // }
                    var type = line.Split(new string[] { "== " }, StringSplitOptions.None)[1].Split(')')[0];
                    typeActionDict.Add(type + ")", new List<string>()
                    {
                        lines[i + 2],
                        lines[i + 3],
                    });

                    i += 5;
                }
                else if (line.Contains("return false;"))
                {
                    flagEndIndex = i - 1;
                    break;
                }
            }

            if (flagEndIndex <= 0)
            {
                Debug.LogError($"[CodeOptimizableHandler] {GetFunctionName()} flagEndIndex <= 0");
                return lines;
            }

            var optLines = new List<string>(lines);

            //移除空函数
            for (int i = flagEndIndex; i > flagIndex + 1; i--)
            {
                optLines.RemoveAt(i);
            }

            //开始替换
            var typeActionEnumerator = typeActionDict.GetEnumerator();
            var index = flagIndex + 1;
            optLines.Insert(++index, "\t\t\tif (__ArraySetDict.TryGetValue(type, out var action))");
            optLines.Insert(++index, "\t\t\t{");
            optLines.Insert(++index, "\t\t\t\taction?.Invoke(L,translator,obj,array_idx,obj_idx);");
            optLines.Insert(++index, "\t\t\t\treturn true;\n\t\t\t}\n");

            index = flagIndex;
            optLines.Insert(index,
                "\n\t\tprivate static System.Collections.Generic.Dictionary<Type, System.Action<RealStatePtr,ObjectTranslator,object,int,int>> __ArraySetDict = new System.Collections.Generic.Dictionary<Type, System.Action<RealStatePtr,ObjectTranslator,object,int,int>>{\n");
            while (typeActionEnumerator.MoveNext())
            {
                var key = typeActionEnumerator.Current.Key;
                optLines.Insert(++index, $"\t\t\t{{{key}, (L,translator,obj,array_idx,obj_idx) => {{");
                var values = typeActionEnumerator.Current.Value;
                for (int i = 0; i < values.Count; i++)
                {
                    var action = values[i];
                    optLines.Insert(++index, action);
                }

                optLines.Insert(++index, "\t\t\t}},\n");
            }

            optLines.Insert(++index, "\t\t};\n");
            typeActionEnumerator.Dispose();
            return optLines;
        }
    }
}