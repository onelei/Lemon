using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using LemonFramework.ECS.Entitys;

namespace LemonFramework.ECS.Components
{
    // 组件管理器（支持序列化和线程安全）
    public class ComponentManager
    {
        // 组件存储：组件类型 -> (实体ID -> 组件实例)
        private readonly Dictionary<Type, Dictionary<int, IComponentData>> _componentEntityComponents =
            new Dictionary<Type, Dictionary<int, IComponentData>>();

        // 实体的组件类型缓存：实体ID -> 组件类型集合
        private readonly Dictionary<int, HashSet<Type>> _entityComponentTypes = new Dictionary<int, HashSet<Type>>();
        
        // 线程安全锁
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        
        // 组件类型注册表（用于序列化）
        private static readonly Dictionary<Type, int> TypeToId = new Dictionary<Type, int>();
        private static readonly Dictionary<int, Type> IDToType = new Dictionary<int, Type>();
        private static int _nextTypeId = 0;
        private static readonly object _typeRegistrationLock = new object(); // 类型注册锁

        /// <summary>
        /// 注册组件类型（用于序列化，线程安全）
        /// </summary>
        public static void RegisterComponentType<T>() where T : struct, IComponentData
        {
            lock (_typeRegistrationLock)
            {
                var type = typeof(T);
                if (!TypeToId.ContainsKey(type))
                {
                    int typeId = _nextTypeId++;
                    TypeToId[type] = typeId;
                    IDToType[typeId] = type;
                }
            }
        }
        
        /// <summary>
        /// 获取组件类型ID（线程安全）
        /// </summary>
        private static int GetTypeId(Type type)
        {
            lock (_typeRegistrationLock)
            {
                return TypeToId.TryGetValue(type, out var id) ? id : -1;
            }
        }
        
        /// <summary>
        /// 从ID获取组件类型（线程安全）
        /// </summary>
        private static Type GetTypeFromId(int typeId)
        {
            lock (_typeRegistrationLock)
            {
                return IDToType.TryGetValue(typeId, out var type) ? type : null;
            }
        }

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

        // 添加组件（线程安全）
        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _lock.EnterWriteLock();
            try
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
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // 获取组件（直接返回引用避免装箱，线程安全）
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetComponent<T>(Entity entity, out T component) where T : struct, IComponentData
        {
            _lock.EnterReadLock();
            try
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
            finally
            {
                _lock.ExitReadLock();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetComponent<T>(Entity entity) where T : struct, IComponentData
        {
            TryGetComponent(entity, out T component);
            return component;
        }

        // 更新组件（直接修改避免装箱，线程安全）
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _lock.EnterWriteLock();
            try
            {
                var type = typeof(T);
                GetOrCreateComponentDictionary(type)[entity.Id] = component;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // 移除指定实体的所有组件（线程安全）
        public void RemoveComponentsForEntity(Entity entity)
        {
            _lock.EnterWriteLock();
            try
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
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取所有组件数据（返回快照副本，线程安全）
        /// 注意：返回的是副本，对其修改不会影响原始数据
        /// </summary>
        public Dictionary<Type, Dictionary<int, IComponentData>> GetAllComponentData()
        {
            _lock.EnterReadLock();
            try
            {
                // 创建深拷贝以避免外部修改
                var snapshot = new Dictionary<Type, Dictionary<int, IComponentData>>();
                foreach (var kvp in _componentEntityComponents)
                {
                    snapshot[kvp.Key] = new Dictionary<int, IComponentData>(kvp.Value);
                }
                return snapshot;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
 
        /// <summary>
        /// 获取符合组件组合的实体（线程安全，返回快照副本）
        /// 注意：返回的是快照，遍历过程中不会被其他线程修改
        /// </summary>
        public List<Entity> GetEntitiesWithComponents(params Type[] componentTypes)
        {
            _lock.EnterReadLock();
            try
            {
                var resultList = new List<Entity>();
                
                if (componentTypes.Length == 0)
                    return resultList;

                // 从第一个组件类型获取候选实体
                if (!_componentEntityComponents.TryGetValue(componentTypes[0], out var firstComponentDict))
                    return resultList;

                // 遍历候选实体（使用 ToArray 避免迭代期间集合被修改）
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
            finally
            {
                _lock.ExitReadLock();
            }
        }
        
        /// <summary>
        /// 清空所有组件数据
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _componentEntityComponents.Clear();
                _entityComponentTypes.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// 序列化组件数据
        /// </summary>
        public byte[] Serialize()
        {
            _lock.EnterReadLock();
            try
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new BinaryWriter(memoryStream))
                {
                    // 写入组件类型数量
                    writer.Write(_componentEntityComponents.Count);
                    
                    // 遍历每种组件类型
                    foreach (var typeEntry in _componentEntityComponents)
                    {
                        var componentType = typeEntry.Key;
                        var entityComponents = typeEntry.Value;
                        
                        // 写入组件类型ID
                        int typeId = GetTypeId(componentType);
                        writer.Write(typeId);
                        
                        // 写入该类型的组件数量
                        writer.Write(entityComponents.Count);
                        
                        // 遍历每个实体的组件
                        foreach (var entityComponent in entityComponents)
                        {
                            // 写入实体ID
                            writer.Write(entityComponent.Key);
                            
                            // 序列化组件数据（如果实现了ISerializable接口）
                            if (entityComponent.Value is ISerializable serializable)
                            {
                                serializable.Serialize(writer);
                            }
                        }
                    }
                    
                    return memoryStream.ToArray();
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        
        /// <summary>
        /// 反序列化组件数据
        /// </summary>
        public void Deserialize(byte[] data)
        {
            _lock.EnterWriteLock();
            try
            {
                // 清空数据
                _componentEntityComponents.Clear();
                _entityComponentTypes.Clear();
                
                using (var memoryStream = new MemoryStream(data))
                using (var reader = new BinaryReader(memoryStream))
                {
                    // 读取组件类型数量
                    int typeCount = reader.ReadInt32();
                    
                    // 遍历每种组件类型
                    for (int i = 0; i < typeCount; i++)
                    {
                        // 读取组件类型ID
                        int typeId = reader.ReadInt32();
                        Type componentType = GetTypeFromId(typeId);
                        
                        if (componentType == null)
                            continue;
                        
                        // 读取该类型的组件数量
                        int componentCount = reader.ReadInt32();
                        
                        // 创建组件字典
                        var componentDict = GetOrCreateComponentDictionary(componentType);
                        
                        // 遍历每个实体的组件
                        for (int j = 0; j < componentCount; j++)
                        {
                            // 读取实体ID
                            int entityId = reader.ReadInt32();
                            
                            // 创建组件实例并反序列化
                            var component = Activator.CreateInstance(componentType) as IComponentData;
                            if (component is ISerializable serializable)
                            {
                                serializable.Deserialize(reader);
                            }
                            
                            // 存储组件
                            componentDict[entityId] = component;
                            
                            // 更新实体的组件类型缓存
                            if (!_entityComponentTypes.TryGetValue(entityId, out var types))
                            {
                                types = new HashSet<Type>();
                                _entityComponentTypes[entityId] = types;
                            }
                            types.Add(componentType);
                        }
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}