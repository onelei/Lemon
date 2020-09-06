using UnityEngine;
using UnityEditor;

/// <summary>
/// 根据名字前缀自动化设置图片的格式以及TAG等设置
/// </summary>
public class ImageImporter : AssetPostprocessor
{
    /// <summary>
    /// 图片导入之前调用，可设置图片的格式、spritePackingTag、assetBundleName等信息
    /// </summary>
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        string path = importer.assetPath;
        string[] pathArray = importer.assetPath.Split('/');
        if (pathArray.Length <= 2)
        {
            Debug.LogError("获取路径名失败");
            return;
        }
        string imageName = pathArray[pathArray.Length - 1];
        string packTag = pathArray[pathArray.Length - 2];

        if (imageName.StartsWith("UI_"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            //设置spritePackingTag
            importer.spritePackingTag = packTag;
            //设置assetBundleName
            importer.assetBundleName = packTag; 
        } 
    } 
}
