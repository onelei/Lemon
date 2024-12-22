/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using UnityEngine;
using UnityEngine.UI;
using System;
using LemonFramework.Log;

namespace LemonFramework.UI.Widgets
{
    [AddComponentMenu("UI/LemonToggleButton")]
    public class LemonToggleButton : LemonButton
    {
        [SerializeField]
        public GameObject Normal;
        [SerializeField]
        public GameObject Choose;

        //Group
        private int index = -1;
        private int curIndex = -1;
        private Action<int, int> OnGroupAction = null;
        private bool bGroup = false;
        //Toggle
        private Action<bool> OnToggleAction;

        private bool bChoose = false;

        public void SetData(bool bChoose, Action<bool> OnToggleAction, bool doAction = false)
        {
            this.bGroup = false;
            this.OnToggleAction = OnToggleAction;
            SetChoose(bChoose);
            if (doAction)
            {
                OnToggleAction(bChoose);
            }
            this.onClick.RemoveListener(ClickEvent);
            this.onClick.AddListener(ClickEvent);
        }

        public void SetGroupData(int index, int curIndex, Action<int, int> OnGroupAction)
        {
            this.bGroup = true;
            this.index = index;
            this.curIndex = curIndex;
            this.OnGroupAction = OnGroupAction;

            SetChoose(curIndex == index);
            this.onClick.RemoveListener(ClickEvent);
            this.onClick.AddListener(ClickEvent);
        }

        public void SetChoose(int _index)
        {
            this.curIndex = _index;
            SetChoose(curIndex == index);
        }

        public void SetChoose(bool bChoose)
        {
            this.bChoose = bChoose;

            if (Normal != null)
            {
                Normal.SetActive(!bChoose);
            }
            if (Choose != null)
            {
                Choose.SetActive(bChoose);
            }
        }

        private void ClickEvent()
        {
            if (bGroup)
            {
                if (index == curIndex)
                    return;
                if (OnGroupAction != null)
                {
                    OnGroupAction(curIndex, index);
                }
            }
            else
            {
                SetChoose(!bChoose);
                if (OnToggleAction != null)
                {
                    OnToggleAction(bChoose);
                }
                LogManager.LogEditor("当前bChoose: " + bChoose);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("SetToggleEditor")]
        public void SetToggleEditor(bool bChoose)
        {
            SetChoose(bChoose);
        }
#endif
    }
}
