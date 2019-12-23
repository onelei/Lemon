using UnityEngine;

namespace Lemon
{
    public static partial class Util
    {
        public static bool Approximately(this Vector3 origin, Vector3 target, float accuracy = 0.001f)
        {
            Vector3 delta = origin - target;
            if (Util.Approximately(delta.x, accuracy) && Util.Approximately(delta.y, accuracy) && Util.Approximately(delta.z, accuracy))
            {
                return true;
            }
            return false;
        }
    }
}
