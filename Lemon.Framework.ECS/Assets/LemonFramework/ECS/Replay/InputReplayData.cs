using System;
using System.Collections.Generic;
using System.IO;
using LemonFramework.ECS.Snapshot;
using UnityEngine;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 基于输入的录像数据（文件更小）
    /// 只记录玩家输入，播放时通过System计算
    /// </summary>
    [Serializable]
    public class InputReplayData
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
        /// 初始状态快照（录制开始时的状态）
        /// </summary>
        public WorldSnapshot InitialSnapshot;
        
        /// <summary>
        /// 所有帧的输入数据（只记录有输入的帧）
        /// </summary>
        private Dictionary<int, FrameInput> _frameInputs;
        
        public InputReplayData()
        {
            _frameInputs = new Dictionary<int, FrameInput>();
            CreationTime = DateTime.UtcNow.Ticks;
        }
        
        public InputReplayData(string name, int targetFrameRate, WorldSnapshot initialSnapshot)
        {
            ReplayName = name;
            TargetFrameRate = targetFrameRate;
            InitialSnapshot = initialSnapshot;
            CreationTime = DateTime.UtcNow.Ticks;
            _frameInputs = new Dictionary<int, FrameInput>();
        }
        
        /// <summary>
        /// 添加一帧的输入（只有输入不为空时才记录）
        /// </summary>
        public void AddInput(FrameInput input)
        {
            // 只记录有实际输入的帧，节省空间
            if (input.Movement.sqrMagnitude > 0.001f || input.ButtonStates != 0)
            {
                _frameInputs[input.FrameNumber] = input;
            }
            
            if (_frameInputs.Count == 0 || input.FrameNumber < StartFrame)
            {
                StartFrame = input.FrameNumber;
            }
            
            if (input.FrameNumber > EndFrame)
            {
                EndFrame = input.FrameNumber;
            }
        }
        
        /// <summary>
        /// 获取指定帧的输入
        /// </summary>
        public FrameInput GetInput(int frameNumber)
        {
            if (_frameInputs.TryGetValue(frameNumber, out var input))
            {
                return input;
            }
            // 如果该帧没有输入，返回空输入
            return new FrameInput(frameNumber, Vector2.zero, 0);
        }
        
        /// <summary>
        /// 检查指定帧是否有输入
        /// </summary>
        public bool HasInput(int frameNumber)
        {
            return _frameInputs.ContainsKey(frameNumber);
        }
        
        /// <summary>
        /// 获取所有有输入的帧号
        /// </summary>
        public List<int> GetInputFrameNumbers()
        {
            var frames = new List<int>(_frameInputs.Keys);
            frames.Sort();
            return frames;
        }
        
        /// <summary>
        /// 获取输入帧数量
        /// </summary>
        public int InputFrameCount => _frameInputs.Count;
        
        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void Clear()
        {
            _frameInputs.Clear();
            StartFrame = 0;
            EndFrame = 0;
            InitialSnapshot = null;
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
                
                // 写入初始快照
                if (InitialSnapshot != null)
                {
                    writer.Write(true);
                    writer.Write(InitialSnapshot.SnapshotId);
                    writer.Write(InitialSnapshot.Timestamp);
                    writer.Write(InitialSnapshot.FrameNumber);
                    
                    writer.Write(InitialSnapshot.EntityData.Length);
                    writer.Write(InitialSnapshot.EntityData);
                    
                    writer.Write(InitialSnapshot.ComponentData.Length);
                    writer.Write(InitialSnapshot.ComponentData);
                }
                else
                {
                    writer.Write(false);
                }
                
                // 写入输入数据数量
                writer.Write(_frameInputs.Count);
                
                // 写入每一帧的输入
                foreach (var kvp in _frameInputs)
                {
                    kvp.Value.Serialize(writer);
                }
            }
        }
        
        /// <summary>
        /// 从文件加载录像
        /// </summary>
        public static InputReplayData LoadFromFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var reader = new BinaryReader(fileStream))
            {
                var replay = new InputReplayData
                {
                    ReplayName = reader.ReadString(),
                    CreationTime = reader.ReadInt64(),
                    StartFrame = reader.ReadInt32(),
                    EndFrame = reader.ReadInt32(),
                    TargetFrameRate = reader.ReadInt32()
                };
                
                // 读取初始快照
                bool hasInitialSnapshot = reader.ReadBoolean();
                if (hasInitialSnapshot)
                {
                    replay.InitialSnapshot = new WorldSnapshot
                    {
                        SnapshotId = reader.ReadInt32(),
                        Timestamp = reader.ReadInt64(),
                        FrameNumber = reader.ReadInt32()
                    };
                    
                    int entityDataLength = reader.ReadInt32();
                    replay.InitialSnapshot.EntityData = reader.ReadBytes(entityDataLength);
                    
                    int componentDataLength = reader.ReadInt32();
                    replay.InitialSnapshot.ComponentData = reader.ReadBytes(componentDataLength);
                }
                
                // 读取输入数据数量
                int inputCount = reader.ReadInt32();
                
                // 读取每一帧的输入
                for (int i = 0; i < inputCount; i++)
                {
                    var input = FrameInput.Deserialize(reader);
                    replay._frameInputs[input.FrameNumber] = input;
                }
                
                return replay;
            }
        }
    }
}

