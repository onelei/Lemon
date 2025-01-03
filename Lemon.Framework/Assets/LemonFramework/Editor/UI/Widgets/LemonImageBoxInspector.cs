﻿/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using LemonFramework;

namespace LemonFramework.UI.Widgets
{
    [CustomEditor(typeof(LemonImageBox), true)]
    public class LemonImageBoxEditor : UnityEditor.UI.GraphicEditor
    {
        [MenuItem("GameObject/UI/LemonImageBox", false, UtilityEditor.Priority_LemonImageBox)]
        public static LemonImageBox AddComponent()
        {
            LemonImageBox component = UtilityEditor.ExtensionComponentWhenCreate<LemonImageBox>(nameof(LemonImageBox));
            return component;
        }
    }
}
