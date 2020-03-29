/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace Lemon.UI
{
    [CustomEditor(typeof(QImage), true)]
    public class QImageEditor : UnityEditor.UI.ImageEditor
    {
        [MenuItem("GameObject/UI/QImage", false, UtilityEditor.Priority_QImage)]
        public static QImage AddComponent()
        {
            QImage component = UtilityEditor.ExtensionComponentWhenCreate<QImage>(typeof(QImage).Name.ToString());
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private QImage ImageComponent;
        private SerializedProperty key;
        protected override void OnEnable()
        {
            base.OnEnable();
            ImageComponent = (QImage)target;
            key = serializedObject.FindProperty("key");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UtilityEditor.PropertyField("KEY", key);

            if (!ImageComponent.bInit)
            {
                ImageComponent.bInit = true;
                SetDefaultValue(ImageComponent);
            }
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void SetDefaultValue(QImage component)
        {
            if (component == null)
                return;
            component.raycastTarget = false;
        }
    }
}
