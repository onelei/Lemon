using UnityEngine;
using System.Collections.Generic;

namespace Lemon.Framework
{
    public class AssetBundleManager
    {
        static AssetBundle assetbundle = null;

        static Dictionary<string, AssetBundle> DicAssetBundle = new Dictionary<string, AssetBundle>();

        public static T LoadResource<T>(string assetBundleName, string assetBundleGroupName) where T : Object
        {
            if (string.IsNullOrEmpty(assetBundleGroupName))
            {
                return default(T);
            }

            if (!DicAssetBundle.TryGetValue(assetBundleGroupName, out assetbundle))
            {
                assetbundle = AssetBundle.LoadFromFile(GetApplicationdataPath() + assetBundleGroupName);//+ ".assetbundle"
                DicAssetBundle.Add(assetBundleGroupName, assetbundle);
            }
            object obj = assetbundle.LoadAsset(assetBundleName, typeof(T));
            var one = obj as T;
            return one;
        }

        public static void UnLoadResource(string assetBundleGroupName)
        {
            if (DicAssetBundle.TryGetValue(assetBundleGroupName, out assetbundle))
            {
                assetbundle.Unload(false);
                DicAssetBundle.Remove(assetBundleGroupName);
                Resources.UnloadUnusedAssets();
            }
        }

        public static string GetApplicationdataPath()
        {
            string StreamingAssetsPath =
#if UNITY_EDITOR
        Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
		"jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
		Application.dataPath + "/Raw/";
#else
		string.Empty;
#endif
            return StreamingAssetsPath;
        }
    }
}



