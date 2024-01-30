using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Jenkins
{
    public class BuildScript
    {
        [MenuItem("Build/Build for Android")]
        public static void BuildForAndroid()
        {
            var buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = new[]
                {
                    "Assets/LemonFramework/Jenkins/Sample/Sample.unity"
                },
                locationPathName = "Jenkins.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed");
            }
        }
    }
}