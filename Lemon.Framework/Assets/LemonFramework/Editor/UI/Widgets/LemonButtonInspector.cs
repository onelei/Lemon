/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/

using LemonFramework.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework.UI.Widgets
{
    [RequireComponent(typeof(LemonImageBox))]
    [CustomEditor(typeof(LemonButton), true)]
    public class LemonButtonInspector : UnityEditor.UI.ButtonEditor
    {
        [MenuItem("GameObject/UI/QButton", false, UtilityEditor.Priority_QButton)]
        public static LemonButton AddComponent()
        {
            LemonImageBox image = UtilityEditor.ExtensionComponentWhenCreate<LemonImageBox>(nameof(LemonButton));
            LemonButton component = image.gameObject.GetOrAddComponent<LemonButton>();
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private LemonButton ButtonComponent;
        public override void OnInspectorGUI()
        {
            ButtonComponent = (LemonButton)target;
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

        private static void SetDefaultValue(LemonButton component)
        { 
            if (component.targetGraphic != null)
                component.targetGraphic.raycastTarget = true;
        } 
    }
}
