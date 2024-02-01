using System;
using System.Collections.Generic;
using Lemon.Framework.Extension;
using Lemon.Framework.UI.Widgets;
using UnityEngine;

namespace Lemon.Framework
{
    public class ClockUtility : BaseBehavior
    {
        private static ClockUtility _instance;
        public static ClockUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(ClockUtility).ToString());
                    _instance = go.GetOrAddComponent<ClockUtility>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public class ClockData
        {
            /// <summary>
            /// Wait millisecond;
            /// </summary>
            public float waitTimeMS;
            public Action callBack;
            public bool bIgnoreTime;

            public ClockData(float waitTimeMS, Action callBack,bool bIgnoreTime = false)
            {
                this.waitTimeMS = waitTimeMS;
                this.callBack = callBack;
                this.bIgnoreTime = bIgnoreTime;
            }
        }

        public List<ClockData> clockDatas = new List<ClockData>();

        private void Update()
        {
            for (int i = clockDatas.Count - 1; i >= 0; --i)
            {
                ClockData clockData = clockDatas[i];
                if (clockData != null)
                {
                    if (clockData.waitTimeMS <= 0)
                    {
                        clockData.callBack.InvokeSafely();
                        clockDatas.Remove(clockData);
                    }
                    else
                    {
                        float deltaTime = clockData.bIgnoreTime ? Time.unscaledDeltaTime : Time.deltaTime;
                        clockData.waitTimeMS += deltaTime;
                    }
                }
            }
        }
    }
}
