/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEditor;
using Lemon;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(QImageBox), true)]
    public class QImageBoxEditor : UnityEditor.UI.GraphicEditor
    {
        [MenuItem("GameObject/UI/QImageBox", false, UtilEditor.Priority_QImageBox)]
        public static QImageBox AddComponent()
        {
            QImageBox component = UtilEditor.ExtensionComponentWhenCreate<QImageBox>(typeof(QImageBox).Name.ToString());
            return component;
        }
    }
}
