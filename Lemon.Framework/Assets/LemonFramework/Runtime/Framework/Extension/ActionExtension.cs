using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon.Framework
{
    public static class ActionExtension
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
