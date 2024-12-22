namespace LemonFramework.Extension
{
    public static class ObjectExtension
    {
        public static bool IsNull(this UnityEngine.Object o)
        {
            return o == null;
        }
    }
}