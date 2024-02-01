/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using UnityEngine;

namespace Lemon.Framework.UI.Widgets
{
    [CustomEditor(typeof(LemonImage), true)]
    public class LemonImageInspector : UnityEditor.UI.ImageEditor
    {
        [MenuItem("GameObject/UI/LemonImage", false, UtilityEditor.Priority_LemonImage)]
        public static LemonImage AddComponent()
        {
            LemonImage component = UtilityEditor.ExtensionComponentWhenCreate<LemonImage>(nameof(LemonImage));
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        private LemonImage ImageComponent;
        private SerializedProperty key;
        protected override void OnEnable()
        {
            base.OnEnable();
            ImageComponent = (LemonImage)target;
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

        private static void SetDefaultValue(LemonImage component)
        {
            if (component == null)
                return;
            component.raycastTarget = false;
        }
    }
}
