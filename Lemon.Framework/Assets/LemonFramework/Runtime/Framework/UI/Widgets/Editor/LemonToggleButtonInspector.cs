/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon.Framework.UI.Widgets
{
    [RequireComponent(typeof(LemonImageBox))]
    [CustomEditor(typeof(LemonToggleButton), true)]
    public class LemonToggleButtonInspector : LemonButtonInspector
    {
        [MenuItem("GameObject/UI/LemonToggleButton", false, UtilityEditor.Priority_LemonToggleButton)]
        public static new LemonToggleButton AddComponent()
        {
            LemonImageBox image = LemonImageBoxEditor.AddComponent();
            image.raycastTarget = true;

            LemonToggleButton component = Utility.GetOrAddCompoment<LemonToggleButton>(image.CacheGameObject);
            component.name = typeof(LemonToggleButton).Name.ToString();
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private LemonToggleButton ToggleButtonComponent;
        private SerializedProperty Normal, Choose;
        protected override void OnEnable()
        {
            base.OnEnable();

            ToggleButtonComponent = (LemonToggleButton)target;
            Normal = serializedObject.FindProperty("Normal");
            Choose = serializedObject.FindProperty("Choose");
        }

        public override void OnInspectorGUI()
        {
            UtilityEditor.PropertyField("Normal", Normal);
            UtilityEditor.PropertyField("Choose", Choose);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Normal"))
            {
                ToggleButtonComponent.SetToggleEditor(false);
            }
            if (GUILayout.Button("Choose"))
            {
                ToggleButtonComponent.SetToggleEditor(true);
            }
            GUILayout.EndHorizontal();

            //base.OnInspectorGUI();
            if (!ToggleButtonComponent.bInit)
            {
                ToggleButtonComponent.bInit = true;
                SetDefaultValue(ToggleButtonComponent);
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void SetDefaultValue(LemonToggleButton component)
        {
            if (component == null)
                return;
            if (component.targetGraphic != null)
                component.targetGraphic.raycastTarget = true;
            component.SetToggleEditor(false);
        }
    }
}
