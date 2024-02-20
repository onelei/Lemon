//https://github.com/BurntSushi/ripgrep/releases

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lemon.Framework.Search
{
    public class SearchWindow : EditorWindow
    {
        [MenuItem("Assets/Search Reference")]
        public static void OpenWindow()
        {
            (EditorWindow.GetWindow(typeof(SearchWindow)) as SearchWindow).search = Selection.activeObject;
        }

        [SerializeField] private Object search;
        [SerializeField] private List<Object> results = new List<Object>();

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Input:", EditorStyles.boldLabel, GUILayout.Width(50));
            search = EditorGUILayout.ObjectField(search, typeof(Object), true);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Search"))
            {
                if (search != null)
                {
                    results.Clear();
                    string path = AssetDatabase.GetAssetPath(search);
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.FindAssets("t:Script" + nameof(SearchWindow));
                        string[] guids = AssetDatabase.FindAssets("t:Script " + nameof(SearchWindow));
                        var scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        string guid = AssetDatabase.AssetPathToGUID(path);
                        string meta = AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.WorkingDirectory = Application.dataPath;
#if UNITY_EDITOR_WIN
                        p.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Application.dataPath),
                            Path.GetDirectoryName(scriptPath), "ripgrep/windows/rg.exe");
#endif
                        //rg.exe -l guid 搜索
                        p.StartInfo.Arguments = $"-l {guid}";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.CreateNoWindow = true;
                        p.Start();
                        while (!p.StandardOutput.EndOfStream)
                        {
                            string line = $"Assets/{p.StandardOutput.ReadLine().Replace("\\", "/")}";
                            if (line != meta)
                            {
                                var item = AssetDatabase.LoadAssetAtPath(line, typeof(Object));
                                if (item != null)
                                    results.Add(item);
                            }
                        }
                    }
                }
            }

            if (results.Count > 0)
            {
                GUILayout.Label("Out:", EditorStyles.boldLabel);
                foreach (var result in results)
                {
                    EditorGUILayout.ObjectField(result, typeof(Object), true);
                }
            }
        }
    }
}