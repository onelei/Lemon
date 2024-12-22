using UnityEngine;
using UnityEditor;
using System.Text;

namespace LemonFramework
{
    public static partial class UtilityEditor
    {
        public const int Priority_LemonText = 2000;
        public const int Priority_LemonRawImage = Priority_LemonText - 6;
        public const int Priority_LemonImage = Priority_LemonText - 5;
        public const int Priority_LemonImageBox = Priority_LemonText - 4;
        public const int Priority_QButton = Priority_LemonText - 3;
        public const int Priority_LemonToggleButton = Priority_LemonText - 2;
        public const int Priority_LemonToggleButtonGroup = Priority_LemonText - 1;

        public static T ExtensionComponentWhenCreate<T>(string gameObejctName) where T : Component
        {
            GameObject go = new GameObject(gameObejctName);
            if (Selection.gameObjects.Length != 0)
            {
                GameObject parent = Selection.gameObjects[0];
                //设置父节点
                go.transform.SetParent(parent.transform, false);
                //设置和父节点一样的层级
                go.layer = parent.layer;
            }
            //设置选中
            Selection.activeObject = go;
            return go.AddComponent<T>();
        }


        public static string GetPath(Transform child, Transform parent)
        {
            string path = string.Empty;
            if (child == null || parent == null || child.name == parent.name)
                return path;
            Transform tmp = child;
            while (tmp != null)
            {
                if (tmp != parent)
                {
                    path = StringUtility.Concat(tmp.name, "/", path);
                    tmp = tmp.parent;
                }
                else
                {
                    break;
                }
            }
            return path;
        }

        public static void PropertyField(string label, SerializedProperty serializedProperty)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            EditorGUILayout.PropertyField(serializedProperty, GUIContent.none);
            GUILayout.EndHorizontal();
        }
    }
}
