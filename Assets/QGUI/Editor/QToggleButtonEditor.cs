/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon.UI
{
    [RequireComponent(typeof(QImageBox))]
    [CustomEditor(typeof(QToggleButton), true)]
    public class QToggleButtonEditor : QButtonEditor
    {
        [MenuItem("GameObject/UI/QToggleButton", false, UtilityEditor.Priority_QToggleButton)]
        public static new QToggleButton AddComponent()
        {
            QImageBox image = QImageBoxEditor.AddComponent();
            image.raycastTarget = true;

            QToggleButton component = Utility.GetOrAddCompoment<QToggleButton>(image.CacheGameObject);
            component.name = typeof(QToggleButton).Name.ToString();
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private QToggleButton ToggleButtonComponent;
        private SerializedProperty Normal, Choose;
        protected override void OnEnable()
        {
            base.OnEnable();

            ToggleButtonComponent = (QToggleButton)target;
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

        private static void SetDefaultValue(QToggleButton component)
        {
            if (component == null)
                return;
            if (component.targetGraphic != null)
                component.targetGraphic.raycastTarget = true;
            component.SetToggleEditor(false);
        }
    }
}
