using System;
using UnityEngine;

namespace LemonFramework.DiskSpace
{
    public class AndroidDiskSpaceHandler : IDiskSpaceHandler
    {
        public long GetAvailableFreeSpace(string fullPath)
        {
            try
            {
                using (var javaObject = new AndroidJavaObject("android.os.StatFs", fullPath))
                {
                    var blockSize = javaObject.Call<long>("getBlockSizeLong");
                    var blockCount = javaObject.Call<long>("getAvailableBlocksLong");
                    return blockSize * blockCount;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}