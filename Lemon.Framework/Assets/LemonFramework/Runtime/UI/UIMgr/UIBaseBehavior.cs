﻿using LemonFramework.UI.Widgets;
using UnityEngine;

namespace LemonFramework
{ 
    public class UIBaseBehavior : BaseBehavior
    { 
        protected virtual void MonoStart() { }
        protected virtual void MonoUpdate() { }
        protected virtual void MonoDestroy() { }

        private void Start()
        {
            MonoStart();
        }

        private void Update()
        {
            MonoUpdate();
        }

        private void OnDestroy()
        {
            MonoDestroy();
        }

    }
}
