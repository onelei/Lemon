
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.UI
{
    [CustomEditor(typeof(UIBase), true)]
    public class UIBaseEditor : Editor
    {
        UIBase component;
        public override void OnInspectorGUI()
        {
            component = (UIBase)target;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1.生成变量"))
            {
                component.GenerateCodeEditor();
            }
            if (GUILayout.Button("2.变量绑定"))
            {
                component.GeneratePathEditor();
            }
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        } 
    }
}
