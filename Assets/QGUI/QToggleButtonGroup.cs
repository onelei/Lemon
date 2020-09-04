/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Lemon.UI
{
    [AddComponentMenu("UI/QToggleButtonGroup")]
    public class QToggleButtonGroup : BaseBehavior
    {
        [SerializeField]
        public List<QToggleButton> list = new List<QToggleButton>();

        //内部变量
        private int pre;
        private int cur;
        private Action<int, int> OnGroupAction;

        public void SetData(int index, Action<int, int> OnGroupAction, bool doAction = false)
        {
            this.pre = -1;
            this.cur = -1;

            this.cur = index;
            this.OnGroupAction = OnGroupAction;

            int length = list.Count;
            for (int i = 0; i < length; i++)
            {
                QToggleButton toggleButton = list[i];
                toggleButton.SetGroupData(i, index, OnGroup);
            }

            if (doAction)
            {
                OnGroup(pre, cur);
            }
        }

        public void OnGroup(int pre, int cur)
        {
            QLog.LogEditor(StringPool.Format("点击pre = {0}, cur = {1}.", pre, cur));

            //设置所有组件的显示隐藏
            int length = list.Count;
            for (int i = 0; i < length; i++)
            {
                QToggleButton toggleButton = list[i];
                toggleButton.SetChoose(cur);
            }

            //设置组件的回调函数
            if (OnGroupAction != null)
            {
                OnGroupAction(pre, cur);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("SetToggleGroupEditor")]
        public void SetToggleGroupEditor(int index = 0)
        {
            SetData(index, null);
        }
#endif
    }
}
