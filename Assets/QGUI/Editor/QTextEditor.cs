/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace Lemon.UI
{
    [CustomEditor(typeof(QText), true)]
    [CanEditMultipleObjects]
    public class QTextEditor : UnityEditor.UI.TextEditor
    {
        [MenuItem("GameObject/UI/QText", false, UtilityEditor.Priority_QText)]
        public static QText AddComponent()
        {
            QText component = UtilityEditor.ExtensionComponentWhenCreate<QText>(typeof(QText).Name.ToString());
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private QText TextComponent;
        private SerializedProperty key;
        protected override void OnEnable()
        {
            base.OnEnable();

            TextComponent = (QText)target;
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

        private static void SetDefaultValue(QText component)
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
            component.text = "QText";
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
