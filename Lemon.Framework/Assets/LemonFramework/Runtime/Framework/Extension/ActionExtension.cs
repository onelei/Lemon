using System;

namespace Lemon.Framework.Extension
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
