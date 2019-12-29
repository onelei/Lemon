using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Lemon
{
    public class LemonEditor_Scene : Editor
    {
        [MenuItem("Lemon/场景/开始 _F5")]
        static private void OpenScene_Start()
        {
            OpenScene(Application.dataPath + "/Lemon/Scenes/Start.unity");
        }

        [MenuItem("Lemon/场景/技能编辑器 _F6")]
        static private void OpenScene_SkillSystemEditor()
        {
            OpenScene(Application.dataPath + "/Lemon/Scenes/SkillSystemEditor.unity");
        }

        static private void OpenScene(string sceneName)
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
