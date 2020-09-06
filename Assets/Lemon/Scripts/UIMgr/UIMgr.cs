/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Lemon
{
    public enum EUIDepth
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

    public struct IEqualityCompare_EUI : IEqualityComparer<EUI>
    {
        public bool Equals(EUI x, EUI y)
        {
            return (int)x == (int)y;
        }

        public int GetHashCode(EUI obj)
        {
            return (int)obj;
        }
    }

    public struct IEqualityCompare_EUIDepth : IEqualityComparer<EUIDepth>
    {
        public bool Equals(EUIDepth x, EUIDepth y)
        {
            return (int)x == (int)y;
        }

        public int GetHashCode(EUIDepth obj)
        {
            return (int)obj;
        }
    }

    public sealed partial class UIMgr : Singleton<UIMgr>
    {
        public UGUIGroup UGUI;

        /// <summary>
        /// UI prefab path
        /// </summary>
        private const string PATH_PREFAB_UI = "Assets/Lemon/Prefabs/UI/";
        /// <summary>
        /// UI depth distance
        /// </summary>
        private const int DEPTH_BETWEEN_UI = 20;

        private List<UIBase> UIBasePool = new List<UIBase>((int)EUI.Max / 2);
        private Dictionary<EUIDepth, int> DepthPool = new Dictionary<EUIDepth, int>(4, new IEqualityCompare_EUIDepth());

        public void Initial()
        {
            UGUI = GameObject.FindObjectOfType<UGUIGroup>();
            Open(EUI.UISample);
        }

        public void Open(EUI eUI, EUIDepth eUIDepth = EUIDepth.Default, params object[] objs)
        {
            UIBase uIBase;
            if (TryGetUIBase(eUI, out uIBase))
            {
                QLog.LogWarning(StringUtility.Concat("Already open UI, witch EUI is ", eUI.ToString()));
                return;
            }

            UIBase prefabDatabase = UnityEditor.AssetDatabase.LoadAssetAtPath<UIBase>(StringUtility.Concat(PATH_PREFAB_UI, eUI.ToString(), ".prefab"));
            if (prefabDatabase == null)
            {
                QLog.LogError(StringUtility.Concat("Can not find UIBase Script in EUI = ", eUI.ToString()));
                return;
            }

            uIBase = Object.Instantiate<UIBase>(prefabDatabase, UGUI.UGUICanvas.transform, false);
            if (uIBase == null)
            {
                QLog.LogError(StringUtility.Concat("Instantiate fail, EUI = ", eUI.ToString()));
                return;
            }

            //设置数据，判断是否进入
            uIBase.SetData(objs);
            if (!uIBase.IsCanOpen())
            {
                QLog.Log(StringUtility.Concat("The UIBase script can not enter, EUI = ", eUI.ToString()));
                return;
            }

            //取出最后的界面，执行OnPause函数
            UIBase lastUIBase;
            if (TryGetLastUIBase(out lastUIBase))
            {
                QLog.LogEditor(StringUtility.Format("{0} 执行暂停函数", lastUIBase.eUI.ToString()));
                lastUIBase.OnPause();
            }

            uIBase.CacheGameObject.SetActive(true);

            //设置层级
            int tmpDepth;
            if (DepthPool.TryGetValue(eUIDepth, out tmpDepth))
            {
                tmpDepth += DEPTH_BETWEEN_UI;
                DepthPool[eUIDepth] = tmpDepth;
            }
            else
            {
                tmpDepth += (int)eUIDepth + DEPTH_BETWEEN_UI;
                DepthPool.Add(eUIDepth, tmpDepth);
            }
            
            uIBase.SetDepth(eUI,eUIDepth, tmpDepth);
            QLog.LogEditor(StringUtility.Format("{0} depth is {1}", uIBase.eUI.ToString(), tmpDepth.ToString()));


            //新界面，执行OnEnter函数
            QLog.LogEditor(StringUtility.Format("{0} 执行OnEnter函数", uIBase.eUI.ToString()));
            uIBase.OnOpen();
            UIBasePool.Add(uIBase);
        }

        public void Close(EUI eUI)
        {
            UIBase uIBase;
            if (!TryGetUIBase(eUI, out uIBase))
            {
                return;
            }

            //取出界面，执行OnExit函数
            QLog.LogEditor(StringUtility.Format("{0} 执行OnExit函数", uIBase.eUI.ToString()));
            uIBase.OnClose();
            UIBasePool.Remove(uIBase);

            GameObject.Destroy(uIBase.CacheGameObject);
            uIBase = null;

            //取出最后的界面，执行OnResume函数
            UIBase lastUIBase;
            if (TryGetLastUIBase(out lastUIBase))
            {
                QLog.LogEditor(StringUtility.Format("{0} 执行OnResume函数", lastUIBase.eUI.ToString()));
                lastUIBase.OnResume();
            }
        }

        public void Close()
        {
            UIBase lastUIBase;
            if (TryGetLastUIBase(out lastUIBase))
            {
                EUI eUI = lastUIBase.eUI;
                QLog.LogEditor(StringUtility.Format("统一关闭 {0} .", eUI.ToString()));
                Close(eUI);
            }
        }

        public bool TryGetUIBase(EUI eUI, out UIBase iBase)
        {
            for (int i = 0; i < UIBasePool.Count; i++)
            {
                UIBase uIBase = UIBasePool[i];
                if (uIBase.eUI == eUI)
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
