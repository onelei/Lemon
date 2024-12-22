using LemonFramework.Log;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LemonFramework
{
    public class SceneEditor : UnityEditor.Editor
    {
        [MenuItem("LemonFramework/场景/开始 _F5")]
        private static void OpenScene_Entry()
        {
            OpenScene(Application.dataPath + "/LemonFramework/Scenes/Entry.unity");
        }

        [MenuItem("LemonFramework/场景/技能编辑器 _F6")]
        private static void OpenScene_SkillSystemEditor()
        {
            OpenScene(Application.dataPath + "/LemonFramework/Scenes/SkillSystemEditor.unity");
        }

        private static void OpenScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                LogManager.LogError("无法打开场景，场景名字为空！！！");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                LogManager.LogError("请保存当前场景");
                return;
            }

            EditorSceneManager.OpenScene(sceneName);
        }
    }
}
