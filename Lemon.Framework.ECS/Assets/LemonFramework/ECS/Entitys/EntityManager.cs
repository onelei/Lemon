using System.Collections.Generic;
using System.IO;
using System.Threading;
using LemonFramework.ECS.Components;

namespace LemonFramework.ECS.Entitys
{
    // 实体管理器（支持序列化和线程安全）
    public class EntityManager
    {
        private readonly ComponentManager _componentManager;
        private int _nextEntityId = 0;
        private readonly HashSet<int> _activeEntities = new HashSet<int>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public EntityManager(ComponentManager componentManager)
        {
            _componentManager = componentManager;
        }

        public Entity CreateEntity()
        {
            _lock.EnterWriteLock();
            try
            {
                int entityId = ++_nextEntityId;
                _activeEntities.Add(entityId);
                return new Entity(entityId);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void DestroyEntity(Entity entity)
        {
            _lock.EnterWriteLock();
            try
            {
                _componentManager.RemoveComponentsForEntity(entity);
                _activeEntities.Remove(entity.Id);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _componentManager.AddComponent(entity, component);
        }

        /// <summary>
        /// 获取所有活动实体
        /// </summary>
        public IEnumerable<Entity> GetAllEntities()
        {
            _lock.EnterReadLock();
            try
            {
                foreach (var entityId in _activeEntities)
                {
                    yield return new Entity(entityId);
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 清空所有实体
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _activeEntities.Clear();
                _nextEntityId = 0;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 序列化实体数据
        /// </summary>
        public byte[] Serialize()
        {
            _lock.EnterReadLock();
            try
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new BinaryWriter(memoryStream))
                {
                    // 写入下一个实体ID
                    writer.Write(_nextEntityId);

                    // 写入活动实体数量
                    writer.Write(_activeEntities.Count);

                    // 写入每个活动实体的ID
                    foreach (var entityId in _activeEntities)
                    {
                        writer.Write(entityId);
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
        /// 反序列化实体数据
        /// </summary>
        public void Deserialize(byte[] data)
        {
            _lock.EnterWriteLock();
            try
            {
                _activeEntities.Clear();

                using (var memoryStream = new MemoryStream(data))
                using (var reader = new BinaryReader(memoryStream))
                {
                    // 读取下一个实体ID
                    _nextEntityId = reader.ReadInt32();

                    // 读取活动实体数量
                    int entityCount = reader.ReadInt32();

                    // 读取每个活动实体的ID
                    for (int i = 0; i < entityCount; i++)
                    {
                        int entityId = reader.ReadInt32();
                        _activeEntities.Add(entityId);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public delegate void ForEachAction<T1, T2>(ref T1 component1, ref T2 component2)
        where T1 : struct, IComponentData
        where T2 : struct, IComponentData;

    // 实体查询器
    public class EntitiesQuery
    {
        private readonly ComponentManager _componentManager;

        public EntitiesQuery(ComponentManager componentManager)
        {
            _componentManager = componentManager;
        }

        public void ForEach<T1, T2>(ForEachAction<T1, T2> action)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            var entities = _componentManager.GetEntitiesWithComponents(typeof(T1), typeof(T2));
            foreach (var entity in entities)
            {
                if (_componentManager.TryGetComponent(entity, out T1 comp1) &&
                    _componentManager.TryGetComponent(entity, out T2 comp2))
                {
                    T1 temp1 = comp1;
                    T2 temp2 = comp2;
                    action(ref temp1, ref temp2);
                    _componentManager.SetComponent(entity, temp1);
                    _componentManager.SetComponent(entity, temp2);
                }
            }
        }
    }
}