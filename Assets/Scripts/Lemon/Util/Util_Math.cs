using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon
{
    public static partial class Util
    {
        public static bool Approximately(float delta, float accuracy = 0.001f)
        {
            if ((-accuracy) <= (delta) && (delta <= accuracy))
            {
                return true;
            }
            return false;
        }
    }
}
