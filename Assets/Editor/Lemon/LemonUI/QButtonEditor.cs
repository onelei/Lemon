using UnityEditor;
using UnityEngine;

namespace Lemon.UI
{
    [CustomEditor(typeof(QButton), true)]
    public class QButtonEditor : UnityEditor.UI.ButtonEditor
    {
        [MenuItem("GameObject/UI/QButton", false, 1998)]
        public static void AddComponent()
        {
            QButton component = UtilEditor.ExtensionComponentWhenCreate<QButton>(typeof(QButton).ToString());
            //设置默认值
            SetDefaultValue(component);
        }

        QButton component;
        public override void OnInspectorGUI()
        {
            component = (QButton)target;
            base.OnInspectorGUI();
            if (!component.bInit)
            {
                component.bInit = true;
                SetDefaultValue(component);
            }
        }

        private static void SetDefaultValue(QButton component)
        {
            if (component == null)
                return;
        } 
    }
}
