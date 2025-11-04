using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LemonFramework.ECS.Components;
using LemonFramework.ECS.Entitys;
using LemonFramework.ECS.Snapshot;
using LemonFramework.ECS.Systems;

namespace LemonFramework.ECS
{
    /// <summary>
    /// ECS场景：管理所有实体和系统，支持序列化和多线程
    /// </summary>
    public class World
    {
        private readonly ComponentManager _componentManager;
        private readonly EntityManager _entityManager;
        private readonly List<ComponentSystem> _systems = new List<ComponentSystem>();
        private readonly SnapshotManager _snapshotManager;
        
        // 线程同步
        private readonly ReaderWriterLockSlim _systemsLock = new ReaderWriterLockSlim();
        private readonly object _updateLock = new object();
        
        // 公共属性用于访问管理器
        public ComponentManager ComponentManager => _componentManager;
        public EntityManager EntityManager => _entityManager;
        public SnapshotManager SnapshotManager => _snapshotManager;
        
        public string WorldName { get; set; }
        public int CurrentFrame { get; private set; }

        public World(string worldName = "DefaultWorld")
        {
            WorldName = worldName;
            _componentManager = new ComponentManager();
            _entityManager = new EntityManager(_componentManager);
            _snapshotManager = new SnapshotManager();
            CurrentFrame = 0;
        }

        #region Entity Management

        /// <summary>
        /// 创建实体
        /// </summary>
        public Entity CreateEntity()
        {
            var entity = _entityManager.CreateEntity();
            return entity;
        }

        /// <summary>
        /// 添加组件到实体
        /// </summary>
        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _entityManager.AddComponent<T>(entity, component);
        }

        /// <summary>
        /// 销毁实体
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            _entityManager.DestroyEntity(entity);
        }

        #endregion

        #region System Management

        /// <summary>
        /// 添加系统
        /// </summary>
        public void AddSystem(ComponentSystem system)
        {
            _systemsLock.EnterWriteLock();
            try
            {
                system.Initialize(_componentManager, _entityManager);
                _systems.Add(system);
            }
            finally
            {
                _systemsLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 移除系统
        /// </summary>
        public void RemoveSystem(ComponentSystem system)
        {
            _systemsLock.EnterWriteLock();
            try
            {
                _systems.Remove(system);
            }
            finally
            {
                _systemsLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 执行所有系统的更新（单线程）
        /// </summary>
        public void Update(TimeData timeData)
        {
            lock (_updateLock)
            {
                _systemsLock.EnterReadLock();
                try
                {
                    foreach (var system in _systems)
                    {
                        system.Update(timeData);
                    }
                    CurrentFrame = timeData.Frame;
                }
                finally
                {
                    _systemsLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 执行所有系统的更新（多线程并行）
        /// </summary>
        public void UpdateParallel(TimeData timeData)
        {
            lock (_updateLock)
            {
                _systemsLock.EnterReadLock();
                try
                {
                    Parallel.ForEach(_systems, system =>
                    {
                        system.Update(timeData);
                    });
                    CurrentFrame = timeData.Frame;
                }
                finally
                {
                    _systemsLock.ExitReadLock();
                }
            }
        }

        #endregion

        #region Snapshot/Serialization

        /// <summary>
        /// 创建场景快照
        /// </summary>
        public WorldSnapshot CreateSnapshot()
        {
            lock (_updateLock)
            {
                var entityData = _entityManager.Serialize();
                var componentData = _componentManager.Serialize();
                
                return _snapshotManager.CreateSnapshot(CurrentFrame, entityData, componentData);
            }
        }

        /// <summary>
        /// 从快照恢复场景
        /// </summary>
        public void RestoreFromSnapshot(WorldSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            lock (_updateLock)
            {
                _entityManager.Deserialize(snapshot.EntityData);
                _componentManager.Deserialize(snapshot.ComponentData);
                CurrentFrame = snapshot.FrameNumber;
            }
        }

        /// <summary>
        /// 保存快照到文件
        /// </summary>
        public void SaveSnapshotToFile(int snapshotId, string filePath)
        {
            var snapshot = _snapshotManager.GetSnapshot(snapshotId);
            if (snapshot != null)
            {
                _snapshotManager.SaveSnapshotToFile(snapshot, filePath);
            }
        }

        /// <summary>
        /// 从文件加载快照
        /// </summary>
        public WorldSnapshot LoadSnapshotFromFile(string filePath)
        {
            return _snapshotManager.LoadSnapshotFromFile(filePath);
        }

        /// <summary>
        /// 序列化整个场景
        /// </summary>
        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                // 写入名称
                writer.Write(WorldName);
                
                // 写入当前帧
                writer.Write(CurrentFrame);
                
                // 序列化实体管理器
                var entityData = _entityManager.Serialize();
                writer.Write(entityData.Length);
                writer.Write(entityData);
                
                // 序列化组件管理器
                var componentData = _componentManager.Serialize();
                writer.Write(componentData.Length);
                writer.Write(componentData);
                
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化整个场景
        /// </summary>
        public void Deserialize(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            using (var reader = new BinaryReader(memoryStream))
            {
                lock (_updateLock)
                {
                    // 读取名称
                    WorldName = reader.ReadString();
                    
                    // 读取当前帧
                    CurrentFrame = reader.ReadInt32();
                    
                    // 反序列化实体管理器
                    int entityDataLength = reader.ReadInt32();
                    var entityData = reader.ReadBytes(entityDataLength);
                    _entityManager.Deserialize(entityData);
                    
                    // 反序列化组件管理器
                    int componentDataLength = reader.ReadInt32();
                    var componentData = reader.ReadBytes(componentDataLength);
                    _componentManager.Deserialize(componentData);
                }
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// 清理场景
        /// </summary>
        public void Clear()
        {
            lock (_updateLock)
            {
                _systemsLock.EnterWriteLock();
                try
                {
                    _systems.Clear();
                    _entityManager.Clear();
                    _componentManager.Clear();
                    _snapshotManager.Clear();
                    CurrentFrame = 0;
                }
                finally
                {
                    _systemsLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Clear();
            _systemsLock?.Dispose();
        }

        #endregion
    }
}

