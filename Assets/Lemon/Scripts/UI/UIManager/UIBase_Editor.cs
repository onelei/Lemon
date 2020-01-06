using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using Lemon;

namespace Lemon.UI
{
    public partial class UIBase : BaseMonoUIClass
    {
#if UNITY_EDITOR
        private const string KEY_VARIABLE = "//==自动化变量开始";
        private const string KEY_PATH = "//==自动化路径开始";

        private List<Transform> qImages = new List<Transform>();
        private List<Transform> qButtons = new List<Transform>();

        private StreamWriter streamWriter;
        private StreamReader streamReader;

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

            string ClassText = "";
            //读取

            UIBase uIBase = GetComponent<UIBase>();
            string scriptName = uIBase.name;
            string path2 = Application.dataPath + "/Lemon/Scripts/UI/" + scriptName + "/" + scriptName + ".cs";

            streamReader = new StreamReader(path2, Encoding.UTF8);
            ClassText = streamReader.ReadToEnd();
            streamReader.Close();

            //生成
            streamWriter = new StreamWriter(path2, false, Encoding.UTF8);


            //添加自动化的变量
            StringBuilder stringBuilder = StringPool.GetStringBuilder();
            stringBuilder.Append("\n");
            for (int i = 0; i < qButtons.Count; i++)
            {
                stringBuilder.Append("        public QButton " + qButtons[i].name + ";");
                stringBuilder.Append("\n");
            }
            ClassText = ClassText.Replace(KEY_VARIABLE, KEY_VARIABLE + stringBuilder.ToString());

            //添加自动化的变量路径
            stringBuilder = StringPool.GetStringBuilder();
            stringBuilder.Append("\n");
            //stringBuilder.Append("        [ContextMenu(\"GeneratePathEditor\")]\n");
            //stringBuilder.Append("        public void GeneratePathEditor()\n");
            //stringBuilder.Append("        {\n");

            for (int i = 0; i < qButtons.Count; i++)
            {
                stringBuilder.Append("            " + qButtons[i].name + " = " + "CacheTransform.Find(\"" + UtilEditor.GetPath(qButtons[i], CacheTransform) + "\").GetComponent<QButton>();\n");
            }
            //stringBuilder.Append("        }\n");

            ClassText = ClassText.Replace(KEY_PATH, KEY_PATH + stringBuilder.ToString());

            streamWriter.Write(ClassText);
            streamWriter.Close();

            AssetDatabase.Refresh();
        }

        [ContextMenu("GeneratePathEditor")]
        public virtual void GeneratePathEditor()
        { 

        }
#endif

    }
}
