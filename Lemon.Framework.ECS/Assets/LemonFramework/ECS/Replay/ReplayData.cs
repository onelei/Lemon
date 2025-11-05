using System;
using System.Collections.Generic;
using System.IO;
using LemonFramework.ECS.Snapshot;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 录像数据，包含一系列连续的帧快照
    /// </summary>
    [Serializable]
    public class ReplayData
    {
        /// <summary>
        /// 录像名称
        /// </summary>
        public string ReplayName;
        
        /// <summary>
        /// 录像创建时间
        /// </summary>
        public long CreationTime;
        
        /// <summary>
        /// 开始帧号
        /// </summary>
        public int StartFrame;
        
        /// <summary>
        /// 结束帧号
        /// </summary>
        public int EndFrame;
        
        /// <summary>
        /// 总帧数
        /// </summary>
        public int TotalFrames => EndFrame - StartFrame + 1;
        
        /// <summary>
        /// 录像时的帧率
        /// </summary>
        public int TargetFrameRate;
        
        /// <summary>
        /// 录像时长（秒）
        /// </summary>
        public float Duration => TotalFrames / (float)TargetFrameRate;
        
        /// <summary>
        /// 所有帧的快照数据 (帧号 -> 快照)
        /// </summary>
        private Dictionary<int, WorldSnapshot> _frameSnapshots;
        
        public ReplayData()
        {
            _frameSnapshots = new Dictionary<int, WorldSnapshot>();
            CreationTime = DateTime.UtcNow.Ticks;
        }
        
        public ReplayData(string name, int targetFrameRate)
        {
            ReplayName = name;
            TargetFrameRate = targetFrameRate;
            CreationTime = DateTime.UtcNow.Ticks;
            _frameSnapshots = new Dictionary<int, WorldSnapshot>();
        }
        
        /// <summary>
        /// 添加一帧快照
        /// </summary>
        public void AddFrame(WorldSnapshot snapshot)
        {
            _frameSnapshots[snapshot.FrameNumber] = snapshot;
            
            if (_frameSnapshots.Count == 1)
            {
                StartFrame = snapshot.FrameNumber;
            }
            EndFrame = snapshot.FrameNumber;
        }
        
        /// <summary>
        /// 获取指定帧的快照
        /// </summary>
        public WorldSnapshot GetFrame(int frameNumber)
        {
            return _frameSnapshots.TryGetValue(frameNumber, out var snapshot) ? snapshot : null;
        }
        
        /// <summary>
        /// 检查是否包含指定帧
        /// </summary>
        public bool HasFrame(int frameNumber)
        {
            return _frameSnapshots.ContainsKey(frameNumber);
        }
        
        /// <summary>
        /// 获取所有帧号
        /// </summary>
        public List<int> GetAllFrameNumbers()
        {
            var frames = new List<int>(_frameSnapshots.Keys);
            frames.Sort();
            return frames;
        }
        
        /// <summary>
        /// 清空所有帧数据
        /// </summary>
        public void Clear()
        {
            _frameSnapshots.Clear();
            StartFrame = 0;
            EndFrame = 0;
        }
        
        /// <summary>
        /// 保存录像到文件
        /// </summary>
        public void SaveToFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(fileStream))
            {
                // 写入录像元数据
                writer.Write(ReplayName ?? "");
                writer.Write(CreationTime);
                writer.Write(StartFrame);
                writer.Write(EndFrame);
                writer.Write(TargetFrameRate);
                
                // 写入帧数量
                writer.Write(_frameSnapshots.Count);
                
                // 写入每一帧的快照
                foreach (var kvp in _frameSnapshots)
                {
                    var snapshot = kvp.Value;
                    
                    writer.Write(snapshot.SnapshotId);
                    writer.Write(snapshot.Timestamp);
                    writer.Write(snapshot.FrameNumber);
                    
                    writer.Write(snapshot.EntityData.Length);
                    writer.Write(snapshot.EntityData);
                    
                    writer.Write(snapshot.ComponentData.Length);
                    writer.Write(snapshot.ComponentData);
                }
            }
        }
        
        /// <summary>
        /// 从文件加载录像
        /// </summary>
        public static ReplayData LoadFromFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var reader = new BinaryReader(fileStream))
            {
                var replay = new ReplayData
                {
                    ReplayName = reader.ReadString(),
                    CreationTime = reader.ReadInt64(),
                    StartFrame = reader.ReadInt32(),
                    EndFrame = reader.ReadInt32(),
                    TargetFrameRate = reader.ReadInt32()
                };
                
                // 读取帧数量
                int frameCount = reader.ReadInt32();
                
                // 读取每一帧的快照
                for (int i = 0; i < frameCount; i++)
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
                    
                    replay._frameSnapshots[snapshot.FrameNumber] = snapshot;
                }
                
                return replay;
            }
        }
    }
}

