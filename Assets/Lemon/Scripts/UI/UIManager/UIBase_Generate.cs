/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

namespace Lemon.UI
{
    public partial class UIBase : BaseUIBehavior
    {
#if UNITY_EDITOR
        private const string KEY_VARIABLE = "//==自动化变量开始";
        private const string KEY_PATH = "//==自动化路径开始";

        private string UI_TEMPLATE_PATH
        {
            get
            {
                return Application.dataPath + "/Lemon/Scripts/UI/UITemplate/UITemplate_Generate.cs";
            }
        }

        /// <summary>
        /// 自动化组件的前缀和类型
        /// </summary>
        private Dictionary<string, Type> name_type = new Dictionary<string, Type>()
        {
            {"QButton", typeof(QButton)},
            {"QRawImage", typeof(QRawImage)},
            {"QImage", typeof(QImage)},
            {"QText", typeof(QText)},
            {"QToggleButton", typeof(QToggleButton)},
            {"QToggleButtonGroup", typeof(QToggleButtonGroup)}
        };

        private Dictionary<Transform, Type> transformGroup = new Dictionary<Transform, Type>();

        [ContextMenu("GenerateCodeEditor")]
        public void GenerateCodeEditor()
        {
            transformGroup.Clear();
            Transform[] children = CacheTransform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                Transform child = children[i];
                string fullName = child.name;
                string[] tmpName = fullName.Split('_');
                if (tmpName.Length > 1)
                {
                    Type type;
                    if (name_type.TryGetValue(tmpName[0], out type))
                    {
                        transformGroup.Add(child, type);
                    }
                }
            }

            //读取
            StreamReader streamReader = new StreamReader(UI_TEMPLATE_PATH, Encoding.UTF8);
            string ClassText = streamReader.ReadToEnd();
            streamReader.Close();

            //生成
            UIBase uIBase = GetComponent<UIBase>();
            string uiBasePath = StringPool.Format(Application.dataPath + "/Lemon/Scripts/UI/{0}/{1}_Generate.cs", uIBase.name, uIBase.name);
            StreamWriter streamWriter = new StreamWriter(uiBasePath, false, Encoding.UTF8);

            //添加自动化的变量
            StringBuilder stringBuilder = StringPool.GetStringBuilder();
            stringBuilder.Append("\n");
            Dictionary<Transform, Type>.Enumerator enumerator = transformGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string name = enumerator.Current.Key.name;
                Type type = enumerator.Current.Value;
                stringBuilder.Append("        public " + type.Name + " " + name + "; ");
                stringBuilder.Append("\n");
            }

            ClassText = ClassText.Replace("UITemplate", uIBase.name);
            ClassText = ClassText.Replace(KEY_VARIABLE, KEY_VARIABLE + stringBuilder.ToString());

            //添加自动化的变量路径
            stringBuilder = StringPool.GetStringBuilder();
            stringBuilder.Append("\n");
            enumerator = transformGroup.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Transform childTransform = enumerator.Current.Key;
                Type type = enumerator.Current.Value;
                stringBuilder.Append("            " + childTransform.name + " = " + "CacheTransform.Find(\"" + UtilityEditor.GetPath(childTransform, CacheTransform) + "\").GetComponent<" + type.Name + ">();\n");
            }

            ClassText = ClassText.Replace(KEY_PATH, KEY_PATH + stringBuilder.ToString());

            streamWriter.Write(ClassText);
            streamWriter.Close();

            UnityEditor.AssetDatabase.Refresh();
        }

        [ContextMenu("GeneratePathEditor")]
        public virtual void GeneratePathEditor()
        {

        }
#endif

    }
}
