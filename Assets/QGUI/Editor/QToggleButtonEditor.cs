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
        [MenuItem("GameObject/UI/QToggleButton", false, UtilEditor.Priority_QToggleButton)]
        public static new QToggleButton AddComponent()
        {
            QImageBox image = QImageBoxEditor.AddComponent();
            image.raycastTarget = true;

            QToggleButton component = Util.GetOrAddCompoment<QToggleButton>(image.CacheGameObject);
            component.name = typeof(QToggleButton).Name.ToString();
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        QToggleButton component;
        public override void OnInspectorGUI()
        {
            component = (QToggleButton)target;
            component.Normal = (GameObject)EditorGUILayout.ObjectField("Normal", component.Normal, typeof(GameObject), true);
            component.Choose = (GameObject)EditorGUILayout.ObjectField("Choose", component.Choose, typeof(GameObject), true);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Normal"))
            {
                component.SetToggleEditor();
            }
            if (GUILayout.Button("Choose"))
            {
                component.SetToggleEditor(true);
            }
            GUILayout.EndHorizontal();

            //base.OnInspectorGUI();
            if (!component.bInit)
            {
                component.bInit = true;
                SetDefaultValue(component);
            }
        }

        private static void SetDefaultValue(QToggleButton component)
        {
            if (component == null)
                return;
            if (component.targetGraphic != null)
                component.targetGraphic.raycastTarget = true;
            component.SetToggleEditor();
        }
    }
}
