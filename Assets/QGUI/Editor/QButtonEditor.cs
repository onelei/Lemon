/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.UI
{
    [RequireComponent(typeof(QImageBox))]
    [CustomEditor(typeof(QButton), true)]
    public class QButtonEditor : UnityEditor.UI.ButtonEditor
    {
        [MenuItem("GameObject/UI/QButton", false, UtilityEditor.Priority_QButton)]
        public static QButton AddComponent()
        {
            QImageBox image = UtilityEditor.ExtensionComponentWhenCreate<QImageBox>(typeof(QButton).Name.ToString());
            QButton component = Utility.GetOrAddCompoment<QButton>(image.gameObject);
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private QButton ButtonComponent;
        public override void OnInspectorGUI()
        {
            ButtonComponent = (QButton)target;
            base.OnInspectorGUI();
            if (!ButtonComponent.bInit)
            {
                ButtonComponent.bInit = true;
                SetDefaultValue(ButtonComponent);
            }
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void SetDefaultValue(QButton component)
        { 
            if (component.targetGraphic != null)
                component.targetGraphic.raycastTarget = true;
        } 
    }
}
