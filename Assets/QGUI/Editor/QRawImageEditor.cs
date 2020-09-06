/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace Lemon
{
    [CustomEditor(typeof(QRawImage), true)]
    public class QRawImageEditor : UnityEditor.UI.RawImageEditor
    {
        [MenuItem("GameObject/UI/QRawImage", false, UtilityEditor.Priority_QRawImage)]
        public static QRawImage AddComponent()
        { 
            QRawImage component = UtilityEditor.ExtensionComponentWhenCreate<QRawImage>(typeof(QRawImage).Name.ToString());
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private QRawImage RawImageComponent;
        private SerializedProperty key;
        protected override void OnEnable()
        {
            base.OnEnable();
            RawImageComponent = (QRawImage)target;
            key = serializedObject.FindProperty("key");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UtilityEditor.PropertyField("KEY", key);
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
            if (!RawImageComponent.bInit)
            {
                RawImageComponent.bInit = true;
                SetDefaultValue(RawImageComponent);
            }
        }

        private static void SetDefaultValue(QRawImage component)
        {
            if (component == null)
                return;
            component.raycastTarget = false;
        } 
    }
}
