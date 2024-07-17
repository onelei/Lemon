using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace XLua.Src.Editor.CodeOptimizable
{
    public class GetDelegateByTypeOptimizable : IOptimizable
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
            var bakFilePath = Path.Combine(Application.dataPath, "XLua/Gen/DelegatesGensBridge.bak.cs");
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

            var bakFilePath = Path.Combine(Application.dataPath, "XLua/Gen/DelegatesGensBridge.bak.cs");
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
            return Path.Combine(Application.dataPath, "XLua/Gen/DelegatesGensBridge.cs");
        }

        public string GetFunctionName()
        {
            return "GetDelegateByType";
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
            //查找GetDelegateByType标记
            lines = Generate__GetDelegateByType(lines);
            //结束
            File.WriteAllLines(filePath, lines.ToArray());
            Debug.Log($"[CodeOptimizableHandler] Generate {filePath} {GetFunctionName()} Success");
        }

        private List<string> Generate__GetDelegateByType(List<string> lines)
        {
            var flagIndex = lines.FindIndex(x =>
                x.Contains(
                    "public override Delegate GetDelegateByType(Type type)"));
            if (flagIndex < 0)
            {
                Debug.LogError($"[CodeOptimizableHandler] {GetFunctionName()} not exist");
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
                    // public override Delegate GetDelegateByType(Type type)
                    // {
                    //
                    //     if (type == typeof(UnityEngine.Events.UnityAction))
                    //     {
                    //         return new UnityEngine.Events.UnityAction(__Gen_Delegate_Imp0);
                    //     }
                    //
                    //     if (type == typeof(UnityEngine.AI.NavMesh.OnNavMeshPreUpdate))
                    //     {
                    //         return new UnityEngine.AI.NavMesh.OnNavMeshPreUpdate(__Gen_Delegate_Imp0);
                    //     }
                    var type = line.Split(new string[] { "== " }, StringSplitOptions.None)[1].Split(')')[0];
                    typeActionDict.Add(type + ")", new List<string>()
                    {
                        lines[i + 2],
                    });

                    i += 3;
                }
                else if (line.Contains("return null;"))
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
            var index = flagIndex;
            optLines.Insert(index,
                "\n\t\tprivate System.Collections.Generic.Dictionary<Type, System.Func<Type,Delegate>> typeDelegateDict;\n");
            index = flagIndex + 3;
            optLines.Insert(index, "\t\t\tif (typeDelegateDict == null){");
            optLines.Insert(++index,
                "\t\t\t\ttypeDelegateDict = new System.Collections.Generic.Dictionary<Type, System.Func<Type,Delegate>>{");
            while (typeActionEnumerator.MoveNext())
            {
                var key = typeActionEnumerator.Current.Key;
                optLines.Insert(++index, $"\t\t\t\t{{{key}, (type) => {{");
                var values = typeActionEnumerator.Current.Value;
                for (int i = 0; i < values.Count; i++)
                {
                    var action = values[i];
                    optLines.Insert(++index, action);
                }

                optLines.Insert(++index, "\t\t\t\t}},\n");
            }
            optLines.Insert(++index, "\t\t\t};}\n");
            optLines.Insert(++index, "\t\t\tif (typeDelegateDict.TryGetValue(type, out var action))");
            optLines.Insert(++index, "\t\t\t{");
            optLines.Insert(++index, "\t\t\t\treturn action?.Invoke(type);\n\t\t\t}\n");
            typeActionEnumerator.Dispose();
            return optLines;
        }
    }
}