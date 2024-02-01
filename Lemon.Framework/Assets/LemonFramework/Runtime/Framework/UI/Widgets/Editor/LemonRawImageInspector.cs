/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace Lemon.Framework.UI.Widgets
{
    [CustomEditor(typeof(LemonRawImage), true)]
    public class LemonRawImageInspector : UnityEditor.UI.RawImageEditor
    {
        [MenuItem("GameObject/UI/LemonRawImage", false, UtilityEditor.Priority_LemonRawImage)]
        public static LemonRawImage AddComponent()
        { 
            LemonRawImage component = UtilityEditor.ExtensionComponentWhenCreate<LemonRawImage>(typeof(LemonRawImage).Name.ToString());
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private LemonRawImage RawImageComponent;
        private SerializedProperty key;
        protected override void OnEnable()
        {
            base.OnEnable();
            RawImageComponent = (LemonRawImage)target;
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

        private static void SetDefaultValue(LemonRawImage component)
        {
            if (component == null)
                return;
            component.raycastTarget = false;
        } 
    }
}
