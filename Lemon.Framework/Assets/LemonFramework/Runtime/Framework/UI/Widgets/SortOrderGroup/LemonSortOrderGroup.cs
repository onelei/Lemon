using System;
using System.Collections.Generic;
using Lemon.Framework.CustomAttribute;
using UnityEngine;

namespace Lemon.Framework.UI.Widgets.SortOrderGroup
{
    public class LemonSortOrderGroup : BaseBehavior
    {
        [Header("Start Sort Order")] public int sortOrder = 0;

        /// <summary>
        /// 会被自动修改的最大排序值
        /// </summary>
        [ReadOnly] [Header("[ReadOnly] Max Sort Order")]
        public int maxSortOrder = 0;

        //序列化GameObjects
        public List<GameObject> items = new List<GameObject>();

        readonly int k_SortOrderOffset = 10;

        [Serializable]
        public class SortOrderComponent
        {
            public int sortOrder;
            public GameObject gameObject;
            public List<LemonSortOrderGroup> sortOrderGroupList = new List<LemonSortOrderGroup>();
            public List<MeshRenderer> meshRendererList = new List<MeshRenderer>();
            public List<Canvas> canvasList = new List<Canvas>();
            public List<ParticleSystemRenderer> particleSystemRendererList = new List<ParticleSystemRenderer>();
            [NonSerialized] public int maxSortOrder = 0;

            public int SetOrder(int sortOrder)
            {
                this.sortOrder = sortOrder;
                maxSortOrder = sortOrder;
                if (gameObject == null)
                    return maxSortOrder;

                if (sortOrderGroupList != null)
                {
                    foreach (var sortOrderGroup in sortOrderGroupList)
                    {
                        maxSortOrder = sortOrderGroup.SetOrder(maxSortOrder + sortOrderGroup.k_SortOrderOffset);
                    }
                }

                if (meshRendererList != null)
                {
                    foreach (var component in meshRendererList)
                    {
                        component.sortingOrder = ++maxSortOrder;
                    }
                }

                if (canvasList != null)
                {
                    foreach (var component in canvasList)
                    {
                        component.sortingOrder = ++maxSortOrder;
                    }
                }

                if (particleSystemRendererList != null)
                {
                    foreach (var component in particleSystemRendererList)
                    {
                        component.sortingOrder = ++maxSortOrder;
                    }
                }

                return maxSortOrder;
            }

            public void Cache(GameObject gameObject)
            {
                if (gameObject == null)
                {
#if !UNITY_EDITOR
                    Debug.LogError("[SortOrderComponent]: gameObject is null");
#endif
                    return;
                }

                this.gameObject = gameObject;
                this.gameObject.GetComponentsInChildren(sortOrderGroupList);
                this.gameObject.GetComponentsInChildren(meshRendererList);
                this.gameObject.GetComponentsInChildren(canvasList);
                this.gameObject.GetComponentsInChildren(particleSystemRendererList);
            }
        }

        private Dictionary<GameObject, SortOrderComponent> cacheComponents =
            new Dictionary<GameObject, SortOrderComponent>();

        public int SetOrder(int order)
        {
            sortOrder = order;
            maxSortOrder = order;
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                if (!cacheComponents.TryGetValue(item, out var component))
                {
                    component = new SortOrderComponent();
                    component.Cache(item);
                }

                maxSortOrder = component.SetOrder(maxSortOrder);
            }

            return maxSortOrder;
        }

        public void Refresh()
        {
            SetOrder(sortOrder);
        }

        public void AddItem(GameObject item)
        {
            if (items.Contains(item))
                return;
            items.Add(item);
            SetOrder(sortOrder);
        }

        public void AddItem(int index, GameObject item)
        {
            if (items.Contains(item))
                return;
            items.Insert(index, item);
            SetOrder(sortOrder);
        }

        public void RemoveItem(GameObject item)
        {
            if (!items.Contains(item))
                return;
            items.Remove(item);
            SetOrder(sortOrder);
        }

#if UNITY_EDITOR
        public void RefreshEditor()
        {
            cacheComponents.Clear();
            SetOrder(sortOrder);
        }

        private void OnValidate()
        {
            RefreshEditor();
        }
#endif
    }
}