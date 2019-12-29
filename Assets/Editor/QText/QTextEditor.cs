using UnityEditor;
using UnityEngine;

namespace Lemon.UI
{
    [CustomEditor(typeof(QText), true)]
    [CanEditMultipleObjects]
    public class QTextEditor : UnityEditor.UI.TextEditor
    {
        [MenuItem("GameObject/UI/QText", false, 2000)]
        static public void AddText(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("QText");
            if (Selection.gameObjects.Length != 0)
            {
                GameObject parent = Selection.gameObjects[0];
                //设置父节点
                go.transform.SetParent(parent.transform, false);
                //设置和父节点一样的层级
                go.layer = parent.layer;
            }
            //设置选中
            Selection.activeObject = go;
            QText qText = go.AddComponent<QText>();
            //设置默认值
            SetDefaultValue(qText);
        }

        QText qText;
        static bool bInit = false;
        public override void OnInspectorGUI()
        {
            qText = (QText)target;
            base.OnInspectorGUI();
            qText.key = EditorGUILayout.TextField("KEY", qText.key);
            if (!qText.bInit)
            {
                qText.bInit = true;
                SetDefaultValue(qText);
            }
        }

        private static void SetDefaultValue(QText qText)
        {
            if (qText == null)
                return;
            qText.font = DefaultFont;
            qText.supportRichText = false;
            qText.raycastTarget = false;
            qText.alignment = TextAnchor.MiddleCenter;
            qText.horizontalOverflow = HorizontalWrapMode.Overflow;
            qText.color = Color.black;
            qText.fontSize = 18;
            qText.text = "QText";
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
