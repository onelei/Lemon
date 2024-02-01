using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;

namespace Lemon.Framework.Test
{
    public class AssetBundleSample : MonoBehaviour
    {
        private string AssetBundlePath = "http://127.0.0.1:8080/";
        //根目录的文件夹产生的AssetBundles文件地址
        private string MainAssetBundleName = "AssetBundles";
        private string FileList = "FileList.txt";

        public Button Button_Load;
        public Button Button_Load2;
        public Button Button_WebLoad;

        public Image Image_BackGround;

        private void Awake()
        {
            Button_Load.onClick.AddListener(OnClickLoad);
            Button_Load2.onClick.AddListener(OnClickLoad2);
            Button_WebLoad.onClick.AddListener(OnClickWebLoad);
        }

        void OnClickLoad()
        {
            Image_BackGround.overrideSprite = AssetBundleManager.LoadResource<Sprite>("UI_1002", "uibackground");
        }

        void OnClickLoad2()
        {
            Image_BackGround.overrideSprite = AssetBundleManager.LoadResource<Sprite>("UI_1003", "uibackground");
        }

        void OnClickWebLoad()
        {
            StartCoroutine(DownloadAssetBundles());
        }

        /// <summary>
        /// 下载根目录AssetBundle文件
        /// </summary>
        /// <returns></returns>
        IEnumerator DownloadAssetBundles()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(AssetBundlePath + MainAssetBundleName))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    yield break;
                }

                byte[] datas = www.downloadHandler.data;
                SaveAssetBundle(MainAssetBundleName, datas);
                string localPath = AssetBundleManager.GetApplicationdataPath() + MainAssetBundleName;
                AssetBundle mainAssetBundle = AssetBundle.LoadFromFile(localPath);
                if (mainAssetBundle == null)
                    yield break;
                //获取AssetBundleManifest文件
                AssetBundleManifest manifest = mainAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                //获取AssetBundleManifest中的所有AssetBundle的名称信息
                string[] assets = manifest.GetAllAssetBundles();
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(AssetBundlePath + assets[i]);
                    //开启协程下载所有的
                    StartCoroutine(DownloadAssetBundleAndSave(AssetBundlePath, assets[i], () =>
                    {
                    //下载完成，按照之前的方法，从本地加载AssetBundle并设置。
                    Image_BackGround.overrideSprite = AssetBundleManager.LoadResource<Sprite>("UI_1003", "uibackground");
                    }));
                }
            }
        }

        IEnumerator DownloadAssetBundleAndSave(string url, string name, Action saveLocalComplate = null)
        {
            WWW www = new WWW(url + name);
            yield return www;
            if (www.isDone)
            {
                SaveAssetBundle(name, www.bytes, saveLocalComplate);
            }
        }

        void SaveAssetBundle(string fileName, byte[] bytes, Action saveLocalComplate = null)
        {
            string path = AssetBundleManager.GetApplicationdataPath() + fileName;
            FileInfo fileInfo = new FileInfo(path);
            FileStream fs = fileInfo.Create();

            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();

            if (saveLocalComplate != null)
            {
                saveLocalComplate();
            }
        }
    }
}