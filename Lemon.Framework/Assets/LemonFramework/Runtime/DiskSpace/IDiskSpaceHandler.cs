namespace LemonFramework.DiskSpace
{
    public interface IDiskSpaceHandler
    {
        long GetAvailableFreeSpace(string fullPath);
    }
}