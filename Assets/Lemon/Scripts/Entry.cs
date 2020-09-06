using Lemon.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon
{
    [XLua.CSharpCallLua]
    public class Entry : BaseBehavior
    {
        public Button Button_Main;

        private void Awake()
        {
            CacheGameObject.GetOrAddCompoment<DontDestroyOnLoad>(); 
        }
         

        void OnClickMain()
        {
            EventMgr.Send("LuaBehavior.Test", string.Empty);
        }

        // Start is called before the first frame update
        void Start()
        {

            LuaMgr.Init(); 

            Button_Main.onClick.AddListener(OnClickMain);
        }

        // Update is called once per frame
        void Update()
        {
            EventMgr.Send("TimeMgr.Update", Time.realtimeSinceStartup);
        }
    }

}