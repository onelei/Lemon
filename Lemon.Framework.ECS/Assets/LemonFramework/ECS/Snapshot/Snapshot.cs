using System;

namespace LemonFramework.ECS.Snapshot
{
    /// <summary>
    /// ECS场景快照，用于保存和恢复整个场景状态
    /// </summary>
    [Serializable]
    public class WorldSnapshot
    {
        public int SnapshotId;
        public long Timestamp;
        public int FrameNumber;
        public byte[] EntityData;
        public byte[] ComponentData;

        public WorldSnapshot()
        {
        }

        public WorldSnapshot(int snapshotId, int frameNumber)
        {
            SnapshotId = snapshotId;
            FrameNumber = frameNumber;
            Timestamp = DateTime.UtcNow.Ticks;
        }
    }
}