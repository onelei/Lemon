/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace Lemon.UI
{
    [CustomEditor(typeof(QRawImage), true)]
    public class QRawImageEditor : UnityEditor.UI.RawImageEditor
    {
        [MenuItem("GameObject/UI/QRawImage", false, UtilEditor.Priority_QRawImage)]
        public static QRawImage AddComponent()
        { 
            QRawImage component = UtilEditor.ExtensionComponentWhenCreate<QRawImage>(typeof(QRawImage).Name.ToString());
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        QRawImage component;
        public override void OnInspectorGUI()
        {
            component = (QRawImage)target;
            base.OnInspectorGUI();
            component.key = EditorGUILayout.TextField("KEY", component.key);
            if (!component.bInit)
            {
                component.bInit = true;
                SetDefaultValue(component);
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
