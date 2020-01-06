using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

namespace Lemon.UI
{
    public partial class UIBase : BaseMonoUIClass
    {
        public List<string> qButtons = new List<string>();
        public List<string> qImages = new List<string>();

        private StreamWriter streamWriter;
        private StreamReader streamReader;

        [ContextMenu("GenerateEditor")]
        public void GenerateEditor()
        {
            qButtons.Clear();
            qImages.Clear();
            for (int i = 0; i < CacheTransform.childCount; i++)
            {
                Transform transform = CacheTransform.GetChild(i);
                string name = transform.name;
                if (name.StartsWith("Button_"))
                {
                    qButtons.Add(name);
                }
            }

            string text = "";
            //读取
            //if (streamReader == null)
            {
                UIBase uIBase = GetComponent<UIBase>();
                string scriptName = uIBase.name;
                string path2 = Application.dataPath + "/Lemon/Scripts/UI/" + scriptName +"/"+ scriptName + ".cs";
                streamReader = new StreamReader(path2,  System.Text.Encoding.UTF8);
                text = streamReader.ReadToEnd();
            }
            //生成
            if (streamWriter == null)
            {
                UIBase uIBase = GetComponent<UIBase>();
                string scriptName = uIBase.name;
                string path2 = Application.dataPath + "/Lemon/Scripts/UI/" + scriptName + "/" + scriptName + ".cs";
                streamWriter = new StreamWriter(path2, true,System.Text.Encoding.UTF8);
            }
            for (int i = 0; i < qButtons.Count; i++)
            {
                streamWriter.Write("public QButton " + qButtons[i]);
            }
            streamWriter.Close();
        }
    }
}
