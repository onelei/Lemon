using System;
using System.IO;
using UnityEngine;

namespace LemonFramework.ECS.Replay
{
    /// <summary>
    /// 单帧的输入数据
    /// </summary>
    [Serializable]
    public struct FrameInput
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public int FrameNumber;
        
        /// <summary>
        /// 移动输入 (WASD)
        /// </summary>
        public Vector2 Movement;
        
        /// <summary>
        /// 其他按键状态（可扩展）
        /// </summary>
        public int ButtonStates;
        
        public FrameInput(int frameNumber, Vector2 movement, int buttonStates = 0)
        {
            FrameNumber = frameNumber;
            Movement = movement;
            ButtonStates = buttonStates;
        }
        
        /// <summary>
        /// 序列化
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FrameNumber);
            writer.Write(Movement.x);
            writer.Write(Movement.y);
            writer.Write(ButtonStates);
        }
        
        /// <summary>
        /// 反序列化
        /// </summary>
        public static FrameInput Deserialize(BinaryReader reader)
        {
            return new FrameInput
            {
                FrameNumber = reader.ReadInt32(),
                Movement = new Vector2(reader.ReadSingle(), reader.ReadSingle()),
                ButtonStates = reader.ReadInt32()
            };
        }
    }
}

