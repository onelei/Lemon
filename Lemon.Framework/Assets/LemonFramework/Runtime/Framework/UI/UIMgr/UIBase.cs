using System;
using System.Collections.Generic;
using Lemon.Framework.Extension;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon.Framework
{
    public partial class UIBase : UIBaseBehavior
    {
        [HideInInspector]
        public string UIName;

        public int UINameType { get; private set; }
        public int Depth { get; private set; }

        public object[] objs { get; private set; }

        private Canvas _CacheCanvas = null;
        public Canvas CacheCanvas
        {
            get
            {
                if (_CacheCanvas == null)
                {
                    _CacheCanvas = CacheGameObject.GetOrAddComponent<Canvas>();
                }
                return _CacheCanvas;
            }
        }

        private GraphicRaycaster _CacheGraphicRaycaster = null;
        public GraphicRaycaster CacheGraphicRaycaster
        {
            get
            {
                if (_CacheGraphicRaycaster == null)
                {
                    _CacheGraphicRaycaster = CacheGameObject.GetOrAddComponent<GraphicRaycaster>();
                }
                return _CacheGraphicRaycaster;
            }
        }

        public virtual void SetData(params object[] objs)
        {
            this.objs = objs;
        }

   
        public void SetDepth(string UIName, int Depth)
        {
            this.UIName = UIName;
            this.Depth = Depth;

            CacheCanvas.sortingOrder = Depth;
        }

        public virtual bool IsCanOpen() { return true; }

        public virtual void OnOpen() { }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual void OnClose() { }

    }
}
