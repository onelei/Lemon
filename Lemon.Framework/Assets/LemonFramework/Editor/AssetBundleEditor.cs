using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AssetBundle打包脚本Editor
/// </summary>
public class AssetBundleEditor
{
    //static string MAIN_ASSET_BUNDLE_NAME = "AssetBundles";
    static string FILE_LIST_NAME = "FileList.txt";
    static string OUT_PATH_WIN64 = "AssetBundles/Win64/AssetBundles/";
    static string OUT_PATH_IOS = "AssetBundles/IOS/AssetBundles/";
    static string OUT_PATH_Android = "AssetBundles/Android/AssetBundles/";

    /// <summary>
    /// BuildWin64
    /// </summary>
    [MenuItem("AssetBundle/BuildWin64")]
    public static void BuildAssetBundle_Win64()
    {
        BuildAssetBundles(OUT_PATH_WIN64, BuildTarget.StandaloneWindows64);
    }

    /// <summary>
    /// BuildIOS
    /// </summary>
    [MenuItem("AssetBundle/BuildIOS")]
    public static void BuildAssetBundle_IOS()
    {
        BuildAssetBundles(OUT_PATH_IOS, BuildTarget.iOS);
    }

    /// <summary>
    /// BuildAndroid
    /// </summary>
    [MenuItem("AssetBundle/BuildAndroid")]
    public static void BuildAssetBundle_Android()
    {
        BuildAssetBundles(OUT_PATH_Android, BuildTarget.Android);
    }

    /// <summary>
    /// CreateFileList
    /// </summary>
    [MenuItem("AssetBundle/CreateFileList")]
    public static void BuildAssetBundle_CreateFileList()
    {
        CreateFileList(OUT_PATH_WIN64);
    }

    /// <summary>
    /// 打包AssetBundle
    /// </summary>
    /// <param name="outPath"></param>
    /// <param name="buildTarget"></param>
    public static void BuildAssetBundles(string outPath, BuildTarget buildTarget)
    {
        if (Directory.Exists(outPath))
        {
            Directory.Delete(outPath, true);
        }
        Directory.CreateDirectory(outPath);
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.UncompressedAssetBundle, buildTarget); 
        AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// Create FileList
    /// </summary>
    static void CreateFileList(string outPath)
    {
        string filePath = outPath + FILE_LIST_NAME;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        StreamWriter streamWriter = new StreamWriter(filePath);

        string[] files = Directory.GetFiles(outPath);
        for (int i = 0; i < files.Length; i++)
        {
            string tmpfilePath = files[i];
            if (tmpfilePath.Equals(filePath) || tmpfilePath.EndsWith(".manifest"))
                continue;
            Debug.Log(tmpfilePath);
            tmpfilePath.Replace("\\", "/");
            streamWriter.WriteLine(tmpfilePath.Substring(outPath.Length) + "|" + GetFileMD5(tmpfilePath));
        }
        streamWriter.Close();
        streamWriter.Dispose();

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取文件的MD5
    /// </summary>
    static System.Security.Cryptography.MD5 MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    static System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
    static string GetFileMD5(string filePath)
    {
        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        byte[] bytes = MD5.ComputeHash(fileStream);
        fileStream.Close();

        for (int i = 0; i < bytes.Length; i++)
        {
            stringBuilder.Append(bytes[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }

}
