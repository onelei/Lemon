/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework.UI.Widgets
{
    [CustomEditor(typeof(LemonToggleButtonGroup))]
    public class LemonToggleButtonGroupInspector : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/LemonToggleButtonGroup", false, UtilityEditor.Priority_LemonToggleButtonGroup)]
        public static LemonToggleButtonGroup AddComponent()
        {
            LemonToggleButtonGroup component = UtilityEditor.ExtensionComponentWhenCreate<LemonToggleButtonGroup>(nameof(LemonToggleButtonGroup));
            component.list.Clear();

            for (int i = 0; i < 2; i++)
            {
                Selection.activeObject = component;
                LemonToggleButton button = LemonToggleButtonInspector.AddComponent();
                component.list.Add(button);
            }
            Selection.activeObject = component;
            return component;
        }

        LemonToggleButtonGroup component;
        int index = 0; 
        public override void OnInspectorGUI()
        { 
            component = (LemonToggleButtonGroup)target;
            GUILayout.BeginHorizontal();
            index = EditorGUILayout.IntField("index", index);
            if (GUILayout.Button("Choose"))
            {
                component.SetToggleGroupEditor(index);
            }
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();

            if(GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
 
