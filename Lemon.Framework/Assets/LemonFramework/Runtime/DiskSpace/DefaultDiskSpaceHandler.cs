using System;

namespace LemonFramework.DiskSpace
{
    public class DefaultDiskSpaceHandler : IDiskSpaceHandler
    {
        public long GetAvailableFreeSpace(string fullPath)
        {
            try
            {
                var driveInfo = new System.IO.DriveInfo(fullPath);
                return driveInfo.AvailableFreeSpace;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}