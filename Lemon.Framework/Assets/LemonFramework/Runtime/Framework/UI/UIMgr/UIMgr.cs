/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Lemon.Framework.Log;

namespace Lemon.Framework
{
    public enum UINameType
    {
        /// <summary>
        /// UI界面
        /// </summary>
        Default = 1000,
        /// <summary>
        /// 弹出窗口
        /// </summary>
        Window  = 10000,
        /// <summary>
        /// 网络界面
        /// </summary>
        Network = 20000,
        /// <summary>
        /// 公共弹出消息界面
        /// </summary>
        Message = 30000,
        /// <summary>
        /// 最上层提示界面
        /// </summary>
        Top = 50000,
    } 

    public sealed partial class UIMgr : Singleton<UIMgr>
    {
        public UGUIGroup UGUI;

        /// <summary>
        /// UI prefab path
        /// </summary>
        private const string PATH_PREFAB_UI = "Assets/Lemon.Framework/Prefabs/UI/";
        /// <summary>
        /// UI depth distance
        /// </summary>
        private const int DEPTH_BETWEEN_UI = 20;

        private List<UIBase> UIBasePool = new List<UIBase>((int)UIName.Max / 2);
        private Dictionary<int, int> DepthPool = new Dictionary<int, int>(4);

        public void Init()
        {
            var asset = Resources.Load<UGUIGroup>("UGUI");
#if UNITY_EDITOR
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UGUIGroup>("Assets/Lemon.Framework/Prefabs/UGUI.prefab");
#endif
            UGUI = GameObject.Instantiate<UGUIGroup>(asset);

            if (UGUI == null)
            {
                LogManager.LogError("UGUI is null");
                return;
            }

        }
         
        public void Open(string UIName, params object[] objs)
        {
            UIBase uIBase;
            if (TryGetUIBase(UIName, out uIBase))
            {
                LogManager.LogWarning(StringUtility.Concat("Already open UI, witch UIName is ", UIName.ToString()));
                return;
            }

            UIBase prefabDatabase = null;
#if UNITY_EDITOR
            prefabDatabase =
                UnityEditor.AssetDatabase.LoadAssetAtPath<UIBase>(StringUtility.Concat(PATH_PREFAB_UI,
                    UIName.ToString(), ".prefab"));
#endif
            if (prefabDatabase == null)
            {
                LogManager.LogError(StringUtility.Concat("Can not find UIBase Script in UIName = ", UIName.ToString()));
                return;
            }

            uIBase = Object.Instantiate<UIBase>(prefabDatabase, UGUI.UGUICanvas.transform, false);
            if (uIBase == null)
            {
                LogManager.LogError(StringUtility.Concat("Instantiate fail, UIName = ", UIName.ToString()));
                return;
            }

            //设置数据，判断是否进入
            uIBase.SetData(objs);
            if (!uIBase.IsCanOpen())
            {
                LogManager.Log(StringUtility.Concat("The UIBase script can not enter, UIName = ", UIName.ToString()));
                return;
            }

            //取出最后的界面，执行OnPause函数
            UIBase lastUIBase;
            if (TryGetLastUIBase(out lastUIBase))
            {
                LogManager.LogEditor(StringUtility.Format("{0} 执行暂停函数", lastUIBase.UIName.ToString()));
                lastUIBase.OnPause();
            }

            uIBase.CacheGameObject.SetActive(true);

            //设置层级
            var UINameDepth = uIBase.UINameType;
            int tmpDepth;
            if (DepthPool.TryGetValue(UINameDepth, out tmpDepth))
            {
                tmpDepth += DEPTH_BETWEEN_UI;
                DepthPool[UINameDepth] = tmpDepth;
            }
            else
            {
                tmpDepth += (int)UINameDepth + DEPTH_BETWEEN_UI;
                DepthPool.Add(UINameDepth, tmpDepth);
            }
            
            uIBase.SetDepth(UIName, tmpDepth);
            LogManager.LogEditor(StringUtility.Format("{0} depth is {1}", uIBase.UIName.ToString(), tmpDepth.ToString()));


            //新界面，执行OnEnter函数
            LogManager.LogEditor(StringUtility.Format("{0} 执行OnEnter函数", uIBase.UIName.ToString()));
            uIBase.OnOpen();
            UIBasePool.Add(uIBase);
        }

        public void Close(string UIName)
        {
            UIBase uIBase;
            if (!TryGetUIBase(UIName, out uIBase))
            {
                return;
            }

            //取出界面，执行OnExit函数
            LogManager.LogEditor(StringUtility.Format("{0} 执行OnExit函数", uIBase.UIName.ToString()));
            uIBase.OnClose();
            UIBasePool.Remove(uIBase);

            GameObject.Destroy(uIBase.CacheGameObject);
            uIBase = null;

            //取出最后的界面，执行OnResume函数
            UIBase lastUIBase;
            if (TryGetLastUIBase(out lastUIBase))
            {
                LogManager.LogEditor(StringUtility.Format("{0} 执行OnResume函数", lastUIBase.UIName.ToString()));
                lastUIBase.OnResume();
            }
        }

        public void Close()
        {
            UIBase lastUIBase;
            if (TryGetLastUIBase(out lastUIBase))
            {
                var UIName = lastUIBase.UIName;
                LogManager.LogEditor(StringUtility.Format("统一关闭 {0} .", UIName.ToString()));
                Close(UIName);
            }
        }

        public bool TryGetUIBase(string UIName, out UIBase iBase)
        {
            for (int i = 0; i < UIBasePool.Count; i++)
            {
                UIBase uIBase = UIBasePool[i];
                if (uIBase.UIName == UIName)
                {
                    iBase = uIBase;
                    return true;
                }
            }
            iBase = null;
            return false;
        }

        public bool TryGetLastUIBase(out UIBase iBase)
        {
            if (UIBasePool.Count > 0)
            {
                iBase = UIBasePool[UIBasePool.Count - 1];
                return true;
            }
            iBase = null;
            return false;
        }
    }
}
