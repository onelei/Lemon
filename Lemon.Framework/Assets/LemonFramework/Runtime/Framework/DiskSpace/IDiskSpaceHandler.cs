namespace Lemon.Framework.DiskSpace
{
    public interface IDiskSpaceHandler
    {
        long GetAvailableFreeSpace(string fullPath);
    }
}