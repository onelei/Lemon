#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
#endif

namespace Lemon.Framework.DiskSpace
{
    public class IOSDiskSpaceHandler : IDiskSpaceHandler
    {
#if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern long _GetAvailableFreeSpace(string fullPath);
#endif

        public long GetAvailableFreeSpace(string fullPath)
        {
#if UNITY_IPHONE
            try
            {
                return _GetAvailableFreeSpace(fullPath);
            }
            catch (Exception e)
            {
                return 0;
            }
#endif
            return 0;
        }
    }
}