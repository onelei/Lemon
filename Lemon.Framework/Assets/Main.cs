using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateAddressable());

        InstantiateCube();
    }

    private IEnumerator UpdateAddressable()
    {
        var handle = Addressables.InitializeAsync();
        yield return handle;

        var checkHandle = Addressables.CheckForCatalogUpdates(true);
        yield return checkHandle;

        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"checkHandle error: {checkHandle.OperationException}");
            yield break;
        }

        if (checkHandle.Result.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, true);
            yield return updateHandle;

            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"updateHandle error: {updateHandle.OperationException}");
                yield break;
            }

            List<IResourceLocator> locators = updateHandle.Result;
            foreach (var locator in locators)
            {
                List<object> keys = new List<object>();
                keys.AddRange(locator.Keys);

                //获取待下载的文件总大小
                var sizeHandle = Addressables.GetDownloadSizeAsync(keys.GetEnumerator());
                yield return sizeHandle;

                if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"updateHandle error: {sizeHandle.OperationException}");
                    yield break;
                }

                Debug.Log($"download size: {sizeHandle.Result}");

                if (sizeHandle.Result > 0)
                {
                    //下载
                    var downloadHandle = Addressables.DownloadDependenciesAsync(keys, true);
                    while (!downloadHandle.IsDone)
                    {
                        if (downloadHandle.Status == AsyncOperationStatus.Failed)
                        {
                            Debug.LogError($"download error: {downloadHandle.OperationException}");
                            yield break;
                        }

                        Debug.Log($"already download percent: {downloadHandle.PercentComplete}");

                        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            Debug.Log("download complete.");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("no need update.");
        }
    }

    private async void InstantiateCube()
    {
        GameObject cube = await Addressables.InstantiateAsync("Cube").Task;
        cube.transform.localPosition = Vector3.zero;


        var tex = await Addressables.LoadAssetAsync<Texture2D>("Assets/Textures/background.png").Task;
        rawImage.texture = tex;
    }


    // Update is called once per frame
    void Update()
    {
    }
}