using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lemon.UI
{
    public class DontDestroyOnLoad : BaseMonoClass
    {
        private void Awake()
        {
            CacheGameObject.isStatic = true;
            DontDestroyOnLoad(CacheGameObject);
        }
    }
}
