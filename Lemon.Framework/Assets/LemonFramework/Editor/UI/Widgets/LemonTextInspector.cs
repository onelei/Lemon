/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace LemonFramework.UI.Widgets
{
    [CustomEditor(typeof(LemonText), true)]
    [CanEditMultipleObjects]
    public class LemonTextInspector : UnityEditor.UI.TextEditor
    {
        [MenuItem("GameObject/UI/LemonText", false, UtilityEditor.Priority_LemonText)]
        public static LemonText AddComponent()
        {
            LemonText component = UtilityEditor.ExtensionComponentWhenCreate<LemonText>(nameof(LemonText));
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private LemonText TextComponent;
        private SerializedProperty key;
        protected override void OnEnable()
        {
            base.OnEnable();

            TextComponent = (LemonText)target;
            key = serializedObject.FindProperty("key");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UtilityEditor.PropertyField("KEY", key);

            if (!TextComponent.bInit)
            {
                TextComponent.bInit = true;
                SetDefaultValue(TextComponent);
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void SetDefaultValue(LemonText component)
        {
            if (component == null)
                return;
            component.font = DefaultFont;
            component.supportRichText = false;
            component.raycastTarget = false;
            component.alignment = TextAnchor.MiddleCenter;
            component.horizontalOverflow = HorizontalWrapMode.Overflow;
            component.color = Color.black;
            component.fontSize = 18;
            component.text = "LemonText";
        }

        private static Font font;
        public static Font DefaultFont
        {
            get
            {
                if (font == null)
                {
                    font = Resources.Load<Font>("Default");
                }
                return font;
            }
        }
    }
}
