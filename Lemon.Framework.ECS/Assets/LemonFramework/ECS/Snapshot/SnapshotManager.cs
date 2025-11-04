using System.Collections.Generic;
using System.IO;

namespace LemonFramework.ECS.Snapshot
{
    /// <summary>
    /// 快照管理器
    /// </summary>
    public class SnapshotManager
    {
        private readonly Dictionary<int, WorldSnapshot> _snapshots = new Dictionary<int, WorldSnapshot>();
        private int _nextSnapshotId = 0;

        /// <summary>
        /// 创建快照
        /// </summary>
        public WorldSnapshot CreateSnapshot(int frameNumber, byte[] entityData, byte[] componentData)
        {
            var snapshot = new WorldSnapshot(_nextSnapshotId++, frameNumber)
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
        public WorldSnapshot GetSnapshot(int snapshotId)
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
        public void SaveSnapshotToFile(WorldSnapshot snapshot, string filePath)
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
        public WorldSnapshot LoadSnapshotFromFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var reader = new BinaryReader(fileStream))
            {
                var snapshot = new WorldSnapshot
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