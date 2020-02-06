# UGUI自动化--QImage、QRawImage

今天接着介绍UGUI自动化中的Image和RawImage控件，为什么要放在一起介绍呢？主要是封装的功能和代码都差不多。QImage继承自UGUI的Image组件，QRawImage继承自UGUI的RawImage组件。两个Image组件都是增加了一个多语言KEY的显示。图片也需要多语言，因此通过一个string类型的KEY，根据不同语言动态设置即可。

## QImage

![QImage](C:\Users\ahlei\Desktop\博客\QImage&QRawImage\Images\QImage.png)

## QRawImage

![QRawImage](C:\Users\ahlei\Desktop\博客\QImage&QRawImage\Images\QRawImage.png)

图QImage组件和QRawImage组件都使用一个KEY用来统一处理多语言图片的显示。将Image进行封装也是为了后面为Image的默认值的修改可以统一设置，便于管理，下面看如何实现。

## QImage组件

新建一个QImage.cs代码如下

```
/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.UI
{
    [AddComponentMenu("UI/QImage")]
    public class QImage : Image
    {
        [HideInInspector]
        public bool bInit = false;
        /// <summary>
        /// 多语言key
        /// </summary>
        public string key = string.Empty;

        private GameObject _CacheGameObject = null;
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }

        private Transform _CacheTransform = null;
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }
    }
}
```

QImage组件继承自Unity的Image组件，这点应该都知道。主要是多了如下两个变量

```
        [HideInInspector]
        public bool bInit = false;
        /// <summary>
        /// 多语言key
        /// </summary>
        public string key = string.Empty;
```

第一个bInit是用来初始化变量用的，第二个key就是用于多语言处理的KEY。

接着我们新建一个QImageEditor.cs的脚本，将其放入Editor文件夹下。代码如下

```
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
        [MenuItem("GameObject/UI/QImage", false, UtilEditor.Priority_QImage)]
        public static QImage AddComponent()
        {
            QImage component = UtilEditor.ExtensionComponentWhenCreate<QImage>(typeof(QImage).Name.ToString());
            //设置默认值
            SetDefaultValue(component);
            return component;
        }

        QImage component;
        public override void OnInspectorGUI()
        {
            component = (QImage)target;
            base.OnInspectorGUI();
            component.key = EditorGUILayout.TextField("KEY", component.key);
            if (!component.bInit)
            {
                component.bInit = true;
                SetDefaultValue(component);
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
```

QImage通过重写OnInspectorGUI函数，来重新绘制组件的显示。先调用

```
base.OnInspectorGUI();
```

优先绘制Image组件，然后绘制一个ImageField用来显示多语言所需要的KEY。

然后我们通过SetDefaultValue函数来重写Image组件里面的参数。同样的道理用来封装RawImage。

## QRawImge组件

新建一个QRawImage.cs代码如下

```
 /**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.UI
{
    [AddComponentMenu("UI/QRawImage")]
    public class QRawImage : RawImage
    {
        [HideInInspector]
        public bool bInit = false;
        /// <summary>
        /// 多语言key
        /// </summary>
        public string key = string.Empty;

        private GameObject _CacheGameObject = null;
        public GameObject CacheGameObject { get { if (_CacheGameObject == null) { _CacheGameObject = gameObject; } return _CacheGameObject; } }

        private Transform _CacheTransform = null;
        public Transform CacheTransform { get { if (_CacheTransform == null) { _CacheTransform = transform; } return _CacheTransform; } }
    }
}
```

接着我们新建一个QRawImageEditor.cs的脚本，将其放入Editor文件夹下。代码如下

```
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
```

详细代码参见：https://github.com/onelei/Lemon/tree/master/Assets/QGUI，欢迎Star。如果本文对你有所帮助，欢迎赞赏~~~

