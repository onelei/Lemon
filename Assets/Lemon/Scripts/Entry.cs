using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lemon
{

    public class Entry : MonoBehaviour
    {
        public Button Button_Main;

        private void Awake()
        {
            Button_Main.onClick.AddListener(OnClickMain);
            LuaMgr.Init();
        }

        void OnClickMain()
        {
            EventMgr.CallLuaFunc("LuaBehavior.Test",gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}