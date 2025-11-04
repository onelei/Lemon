using System;
using System.Collections.Generic;
using System.IO;

namespace LemonFramework.ECS.Snapshot
{
    /// <summary>
    /// ECS场景快照，用于保存和恢复整个场景状态
    /// </summary>
    [Serializable]
    public class SceneSnapshot
    {
        public int SnapshotId;
        public long Timestamp;
        public int FrameNumber;
        public byte[] EntityData;
        public byte[] ComponentData;
        
        public SceneSnapshot()
        {
        }
        
        public SceneSnapshot(int snapshotId, int frameNumber)
        {
            SnapshotId = snapshotId;
            FrameNumber = frameNumber;
            Timestamp = DateTime.UtcNow.Ticks;
        }
    }
    
    /// <summary>
    /// 快照管理器
    /// </summary>
    public class SnapshotManager
    {
        private readonly Dictionary<int, SceneSnapshot> _snapshots = new Dictionary<int, SceneSnapshot>();
        private int _nextSnapshotId = 0;
        
        /// <summary>
        /// 创建快照
        /// </summary>
        public SceneSnapshot CreateSnapshot(int frameNumber, byte[] entityData, byte[] componentData)
        {
            var snapshot = new SceneSnapshot(_nextSnapshotId++, frameNumber)
            {
                EntityData = entityData,
                ComponentData = componentData
            };
            
            _snapshots[snapshot.SnapshotId] = snapshot;
            return snapshot;
        }
        
        /// <summary>
        /// 获取快照
        /// </summary>
        public SceneSnapshot GetSnapshot(int snapshotId)
        {
            return _snapshots.TryGetValue(snapshotId, out var snapshot) ? snapshot : null;
        }
        
        /// <summary>
        /// 删除快照
        /// </summary>
        public void RemoveSnapshot(int snapshotId)
        {
            _snapshots.Remove(snapshotId);
        }
        
        /// <summary>
        /// 清空所有快照
        /// </summary>
        public void Clear()
        {
            _snapshots.Clear();
            _nextSnapshotId = 0;
        }
        
        /// <summary>
        /// 将快照保存到文件
        /// </summary>
        public void SaveSnapshotToFile(SceneSnapshot snapshot, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(snapshot.SnapshotId);
                writer.Write(snapshot.Timestamp);
                writer.Write(snapshot.FrameNumber);
                
                writer.Write(snapshot.EntityData.Length);
                writer.Write(snapshot.EntityData);
                
                writer.Write(snapshot.ComponentData.Length);
                writer.Write(snapshot.ComponentData);
            }
        }
        
        /// <summary>
        /// 从文件加载快照
        /// </summary>
        public SceneSnapshot LoadSnapshotFromFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var reader = new BinaryReader(fileStream))
            {
                var snapshot = new SceneSnapshot
                {
                    SnapshotId = reader.ReadInt32(),
                    Timestamp = reader.ReadInt64(),
                    FrameNumber = reader.ReadInt32()
                };
                
                int entityDataLength = reader.ReadInt32();
                snapshot.EntityData = reader.ReadBytes(entityDataLength);
                
                int componentDataLength = reader.ReadInt32();
                snapshot.ComponentData = reader.ReadBytes(componentDataLength);
                
                _snapshots[snapshot.SnapshotId] = snapshot;
                return snapshot;
            }
        }
    }
}

