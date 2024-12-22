
using System.Collections;
using System.Collections.Generic;
using LemonFramework.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework
{
    public class Entry : BaseBehavior
    {
        private void Awake()
        {
            DontDestroyOnLoad(CacheGameObject);
        }
         
        // Update is called once per frame
        void Update()
        {
            //EventMgr.Send("TimeMgr.Update", Time.realtimeSinceStartup);
        }
    }

}