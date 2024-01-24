using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Lemon.Framework
{
    public class SceneEditor : UnityEditor.Editor
    {
        [MenuItem("Lemon.Framework/场景/开始 _F5")]
        private static void OpenScene_Entry()
        {
            OpenScene(Application.dataPath + "/Lemon.Framework/Scenes/Entry.unity");
        }

        [MenuItem("Lemon.Framework/场景/技能编辑器 _F6")]
        private static void OpenScene_SkillSystemEditor()
        {
            OpenScene(Application.dataPath + "/Lemon.Framework/Scenes/SkillSystemEditor.unity");
        }

        private static void OpenScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                QLog.LogErrorEditor("无法打开场景，场景名字为空！！！");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                QLog.LogErrorEditor("请保存当前场景");
                return;
            }

            EditorSceneManager.OpenScene(sceneName);
        }
    }
}
