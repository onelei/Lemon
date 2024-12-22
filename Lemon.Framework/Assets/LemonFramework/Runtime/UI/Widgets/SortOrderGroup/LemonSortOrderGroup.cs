using System;
using System.Collections.Generic;
using UnityEngine;

namespace LemonFramework.UI.Widgets.SortOrderGroup
{
    public class LemonSortOrderGroup : BaseBehavior
    {
        //[Header("Start Sort Order")] 
        [SerializeField] [HideInInspector] private int sortOrder = 0;

        /// <summary>
        /// 会被自动修改的最大排序值
        /// </summary>
        [NonSerialized] public int MaxSortOrder = 0;

        //序列化GameObjects
        public List<GameObject> items = new List<GameObject>();

        readonly int _kSortOrderOffset = 1;

        [Serializable]
        internal class SortOrderItem
        {
            [Serializable]
            internal class Component
            {
                public MeshRenderer meshRenderer;
                public Canvas canvas;
                public ParticleSystemRenderer particleSystemRenderer;

                public Component(MeshRenderer meshRenderer)
                {
                    this.meshRenderer = meshRenderer;
                }

                public Component(Canvas canvas)
                {
                    this.canvas = canvas;
                }

                public Component(ParticleSystemRenderer particleSystemRenderer)
                {
                    this.particleSystemRenderer = particleSystemRenderer;
                }

                public void SetOrder(int order)
                {
                    if (meshRenderer != null)
                    {
                        meshRenderer.sortingOrder = order;
                    }

                    if (canvas != null)
                    {
                        canvas.sortingOrder = order;
                    }

                    if (particleSystemRenderer != null)
                    {
                        particleSystemRenderer.sortingOrder = order;
                    }
                }

                public int GetOrder()
                {
                    if (meshRenderer != null)
                    {
                        return meshRenderer.sortingOrder;
                    }

                    if (canvas != null)
                    {
                        return canvas.sortingOrder;
                    }

                    if (particleSystemRenderer != null)
                    {
                        return particleSystemRenderer.sortingOrder;
                    }

                    return 0;
                }
            }

            public GameObject gameObject;
            public List<Component> components = new List<Component>();
            public List<LemonSortOrderGroup> sortOrderGroups = new List<LemonSortOrderGroup>();

            [NonSerialized] public int MaxSortOrder = 0;
            private SortedDictionary<int, List<Component>> _sortOrderDic = new SortedDictionary<int, List<Component>>();

            public int SetOrder(int sortOrder)
            {
                MaxSortOrder = sortOrder;
                if (gameObject == null)
                    return MaxSortOrder;

                foreach (var component in components)
                {
                    if (!_sortOrderDic.TryGetValue(component.GetOrder(), out var results))
                    {
                        results = new List<Component>();
                    }

                    results.Add(component);
                    _sortOrderDic[component.GetOrder()] = results;
                }

                foreach (var pair in _sortOrderDic)
                {
                    ++MaxSortOrder;
                    foreach (var component in pair.Value)
                    {
                        component.SetOrder(MaxSortOrder);
                    }
                }

                foreach (var sortOrderGroup in sortOrderGroups)
                {
                    MaxSortOrder = sortOrderGroup.SetOrder(MaxSortOrder + sortOrderGroup._kSortOrderOffset);
                }

                return MaxSortOrder;
            }

            public void Cache(GameObject obj)
            {
                if (obj == null)
                {
#if !UNITY_EDITOR
                    Debug.LogError("[SortOrderComponent]: obj is null");
#endif
                    return;
                }

                this.gameObject = obj;
                components.Clear();
                //获取所有的MeshRenderer
                var meshRenderers = ListPool<MeshRenderer>.Get();
                this.gameObject.GetComponentsInChildren(meshRenderers);
                foreach (var meshRenderer in meshRenderers)
                {
                    components.Add(new Component(meshRenderer));
                }

                ListPool<MeshRenderer>.Release(meshRenderers);
                //获取所有的Canvas
                var canvas = ListPool<Canvas>.Get();
                this.gameObject.GetComponentsInChildren(canvas);
                foreach (var canva in canvas)
                {
                    components.Add(new Component(canva));
                }

                ListPool<Canvas>.Release(canvas);
                //获取所有的ParticleSystemRenderer
                var particleSystemRenderers = ListPool<ParticleSystemRenderer>.Get();
                this.gameObject.GetComponentsInChildren(particleSystemRenderers);
                foreach (var particleSystemRenderer in particleSystemRenderers)
                {
                    components.Add(new Component(particleSystemRenderer));
                }

                ListPool<ParticleSystemRenderer>.Release(particleSystemRenderers);
                //获取所有的SortOrderComponent
                this.gameObject.GetComponentsInChildren(sortOrderGroups);
#if UNITY_EDITOR
                if (components.Count == 0 && sortOrderGroups.Count == 0)
                {
                    Debug.LogError(
                        $"[SortOrderComponent]: There are no Canvas or SortOrderGroup Component in the [{gameObject.name}] GameObject");
                }
#endif
            }
        }

        private readonly Dictionary<GameObject, SortOrderItem> _cacheComponents =
            new Dictionary<GameObject, SortOrderItem>();

        public int SetOrder(int order)
        {
            sortOrder = order;
            MaxSortOrder = order;
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                if (!_cacheComponents.TryGetValue(item, out var component))
                {
                    component = new SortOrderItem();
                    component.Cache(item);
                }

                _cacheComponents[item] = component;
                MaxSortOrder = component.SetOrder(MaxSortOrder);
            }

            return MaxSortOrder;
        }

        public void Refresh(bool force = false)
        {
            if (force)
            {
                _cacheComponents.Clear();
            }

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

        private void OnDestroy()
        {
            _cacheComponents.Clear();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            Refresh(true);
        }
#endif
    }
}