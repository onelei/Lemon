using UnityEngine;

namespace LemonFramework.Extension
{
    public static class Vector3Extension
    {
        public static bool Approximately(this Vector3 origin, Vector3 target)
        {
            if (Mathf.Approximately(origin.x, target.x) && Mathf.Approximately(origin.y, target.y) &&
                Mathf.Approximately(origin.z, target.z))
            {
                return true;
            }

            return false;
        }
    }
}