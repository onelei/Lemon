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
        [MenuItem("GameObject/UI/QButton", false, UtilEditor.Priority_QButton)]
        public static QButton AddComponent()
        {
            QImageBox image = UtilEditor.ExtensionComponentWhenCreate<QImageBox>(typeof(QButton).Name.ToString());
            QButton component = Util.GetOrAddCompoment<QButton>(image.gameObject);
            //设置默认值
            SetDefaultValue(component);
            return component;
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
            if (component.targetGraphic != null)
                component.targetGraphic.raycastTarget = true;
        } 
    }
}
