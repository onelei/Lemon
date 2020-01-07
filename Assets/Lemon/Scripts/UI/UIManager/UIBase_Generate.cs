using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Lemon.UI
{
    public partial class UIBase : BaseMonoUIClass
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

        private List<Transform> qImages = new List<Transform>();
        private List<Transform> qButtons = new List<Transform>();

        [ContextMenu("GenerateCodeEditor")]
        public void GenerateCodeEditor()
        {
            qButtons.Clear();
            qImages.Clear();
            Transform[] children = CacheTransform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                Transform child = children[i];
                string name = child.name;
                if (name.StartsWith("Button_"))
                {
                    qButtons.Add(child);
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
            for (int i = 0; i < qButtons.Count; i++)
            {
                stringBuilder.Append("        public QButton " + qButtons[i].name + ";");
                stringBuilder.Append("\n");
            }
            ClassText = ClassText.Replace("UITemplate", uIBase.name);
            ClassText = ClassText.Replace(KEY_VARIABLE, KEY_VARIABLE + stringBuilder.ToString());
            
            //添加自动化的变量路径
            stringBuilder = StringPool.GetStringBuilder();
            stringBuilder.Append("\n");

            for (int i = 0; i < qButtons.Count; i++)
            {
                stringBuilder.Append("            " + qButtons[i].name + " = " + "CacheTransform.Find(\"" + UtilEditor.GetPath(qButtons[i], CacheTransform) + "\").GetComponent<QButton>();\n");
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
