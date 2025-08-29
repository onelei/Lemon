using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LemonFramework.ECS
{
    // 组件管理器
    public class ComponentManager
    {
        // 组件存储：组件类型 -> (实体ID -> 组件实例)
        private readonly Dictionary<Type, Dictionary<int, IComponentData>> _componentEntityComponents =
            new Dictionary<Type, Dictionary<int, IComponentData>>();

        // 实体的组件类型缓存：实体ID -> 组件类型集合
        private readonly Dictionary<int, HashSet<Type>> _entityComponentTypes = new Dictionary<int, HashSet<Type>>();

        // 查询结果缓存：组件类型组合 -> 实体列表（预分配避免重复创建）
        private readonly Dictionary<Type[], List<Entity>> _queryCache = new Dictionary<Type[], List<Entity>>();
        private readonly object _queryCacheLock = new object();

        // 获取组件类型的字典，如果不存在则创建
        private Dictionary<int, IComponentData> GetOrCreateComponentDictionary(Type type)
        {
            if (!_componentEntityComponents.TryGetValue(type, out var dict))
            {
                dict = new Dictionary<int, IComponentData>();
                _componentEntityComponents[type] = dict;
            }

            return dict;
        }

        // 添加组件
        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            var type = typeof(T);
            var dict = GetOrCreateComponentDictionary(type);
            dict[entity.Id] = component;

            // 更新实体的组件类型缓存
            if (!_entityComponentTypes.TryGetValue(entity.Id, out var types))
            {
                types = new HashSet<Type>();
                _entityComponentTypes[entity.Id] = types;
            }

            types.Add(type);
        }

        // 获取组件（直接返回引用避免装箱）
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetComponent<T>(Entity entity, out T component) where T : struct, IComponentData
        {
            var type = typeof(T);
            if (_componentEntityComponents.TryGetValue(type, out var dict) && dict.TryGetValue(entity.Id, out var comp))
            {
                component = (T) comp;
                return true;
            }

            component = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetComponent<T>(Entity entity) where T : struct, IComponentData
        {
            TryGetComponent(entity, out T component);
            return component;
        }

        // 更新组件（直接修改避免装箱）
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            var type = typeof(T);
            GetOrCreateComponentDictionary(type)[entity.Id] = component;
        }

        // 移除指定实体的所有组件
        public void RemoveComponentsForEntity(Entity entity)
        {
            int entityId = entity.Id;
            if (!_entityComponentTypes.TryGetValue(entityId, out var types))
                return;

            foreach (var type in types)
            {
                if (_componentEntityComponents.TryGetValue(type, out var dict))
                {
                    dict.Remove(entityId);
                    if (dict.Count == 0)
                        _componentEntityComponents.Remove(type);
                }
            }

            _entityComponentTypes.Remove(entityId);
        }

        public Dictionary<Type, Dictionary<int, IComponentData>> GetAllComponentData()
        {
            return _componentEntityComponents;
        }
 
        // 获取符合组件组合的实体（核心优化：复用列表）
        public List<Entity> GetEntitiesWithComponents(params Type[] componentTypes)
        {
            // 使用数组作为键，需要自定义比较器
            lock (_queryCacheLock)
            {
                // 查找缓存
                foreach (var cacheEntry in _queryCache)
                {
                    if (ArraysEqual(cacheEntry.Key, componentTypes))
                    {
                        // 清空并重用列表，避免创建新列表
                        cacheEntry.Value.Clear();
                        return FindEntitiesMatching(cacheEntry.Value, componentTypes);
                    }
                }

                // 没有缓存则创建新列表
                var newList = new List<Entity>();
                _queryCache[componentTypes] = newList;
                return FindEntitiesMatching(newList, componentTypes);
            }
        }

        // 实际查找符合条件的实体
        private List<Entity> FindEntitiesMatching(List<Entity> resultList, Type[] componentTypes)
        {
            if (componentTypes.Length == 0)
                return resultList;

            // 从第一个组件类型获取候选实体
            if (!_componentEntityComponents.TryGetValue(componentTypes[0], out var firstComponentDict))
                return resultList;

            foreach (var entityId in firstComponentDict.Keys)
            {
                // 检查实体是否拥有所有需要的组件类型
                if (_entityComponentTypes.TryGetValue(entityId, out var entityTypes))
                {
                    bool hasAllComponents = true;
                    for (int i = 1; i < componentTypes.Length; i++)
                    {
                        if (!entityTypes.Contains(componentTypes[i]))
                        {
                            hasAllComponents = false;
                            break;
                        }
                    }

                    if (hasAllComponents)
                    {
                        resultList.Add(new Entity(entityId));
                    }
                }
            }

            return resultList;
        }

        // 比较两个数组是否相等
        private bool ArraysEqual(Type[] a, Type[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }
}