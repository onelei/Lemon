using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon.Framework
{
    [XLua.CSharpCallLua]
    public class Entry : BaseBehavior
    {
        private void Awake()
        {
            CacheGameObject.isStatic = true;
            DontDestroyOnLoad(CacheGameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            LuaMgr.Init();
        }

        // Update is called once per frame
        void Update()
        {
            EventMgr.Send("TimeMgr.Update", Time.realtimeSinceStartup);
        }
    }
}