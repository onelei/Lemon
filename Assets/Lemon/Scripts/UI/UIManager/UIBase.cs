using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon.UI
{
    public partial class UIBase : BaseMonoUIClass
    {
        public EUI eUI;

        public EUIDepth eUIDepth { get; private set; }
        public int Depth { get; private set; }

        public object[] objs { get; private set; }

        private Canvas _CacheCanvas = null;
        public Canvas CacheCanvas
        {
            get
            {
                if (_CacheCanvas == null)
                {
                    _CacheCanvas = Util.GetOrAddCompoment<Canvas>(CacheGameObject);
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
                    _CacheGraphicRaycaster = Util.GetOrAddCompoment<GraphicRaycaster>(CacheGameObject);
                }
                return _CacheGraphicRaycaster;
            }
        }

        public virtual void SetData(params object[] objs)
        {
            this.objs = objs;
        }

        public void SetDepth(EUI eUI, EUIDepth eUIDepth, int Depth)
        {
            this.eUI = eUI;
            this.eUIDepth = eUIDepth;
            this.Depth = Depth;

            CacheCanvas.sortingOrder = Depth;
        }

        public virtual bool IsCanEnter() { return true; }

        public virtual void OnEnter() { }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual void OnExit() { }

    }
}
