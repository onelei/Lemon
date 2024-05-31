using UnityEngine;

namespace Lemon.Framework.DiskSpace
{
    public static class DiskSpaceUtility
    {
        private static IDiskSpaceHandler _diskSpaceHandler;

        static IDiskSpaceHandler GetDiskSpace()
        {
            if (_diskSpaceHandler == null)
            {
#if UNITY_ANDROID
                _diskSpaceHandler = new AndroidDiskSpaceHandler();
#elif UNITY_IPHONE
                _diskSpaceHandler = new IOSDiskSpaceHandler();
#else
                _diskSpaceHandler = new DefaultDiskSpaceHandler();
#endif
            }

            return _diskSpaceHandler;
        }

        public static long GetAvailableFreeSpace(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                Debug.LogError("DiskSpaceUtility.GetAvailableFreeSpace: fullPath is null or empty");
                return 0;
            }

            return GetDiskSpace().GetAvailableFreeSpace(fullPath);
        }
    }
}