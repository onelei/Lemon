/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using Lemon.Framework;

namespace Lemon.Framework.UI.Widgets
{
    [CustomEditor(typeof(LemonImageBox), true)]
    public class LemonImageBoxEditor : UnityEditor.UI.GraphicEditor
    {
        [MenuItem("GameObject/UI/LemonImageBox", false, UtilityEditor.Priority_LemonImageBox)]
        public static LemonImageBox AddComponent()
        {
            LemonImageBox component = UtilityEditor.ExtensionComponentWhenCreate<LemonImageBox>(typeof(LemonImageBox).Name.ToString());
            return component;
        }
    }
}
