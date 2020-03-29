using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon
{
    public static partial class Utility
    {
        public static void InvokeSafely(this Action callBack)
        {
            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
