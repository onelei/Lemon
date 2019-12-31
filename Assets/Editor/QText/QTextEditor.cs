using UnityEditor;
using UnityEngine;

namespace Lemon.UI
{
    [CustomEditor(typeof(QText), true)]
    [CanEditMultipleObjects]
    public class QTextEditor : UnityEditor.UI.TextEditor
    {
        [MenuItem("GameObject/UI/QText", false, 2000)]
        public static void AddComponent()
        { 
            QText component = UtilEditor.ExtensionComponentWhenCreate<QText>(typeof(QText).ToString());
            //设置默认值
            SetDefaultValue(component);
        }

        QText component;
        static bool bInit = false;
        public override void OnInspectorGUI()
        {
            component = (QText)target;
            base.OnInspectorGUI();
            component.key = EditorGUILayout.TextField("KEY", component.key);
            if (!component.bInit)
            {
                component.bInit = true;
                SetDefaultValue(component);
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
