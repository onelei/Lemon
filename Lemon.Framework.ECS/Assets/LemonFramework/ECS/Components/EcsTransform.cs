using System;
using System.IO;
using UnityEngine;

namespace LemonFramework.ECS.Components
{
    /// <summary>
    /// ECS Transform组件（用于ECS内部计算）
    /// Unity会从这个组件读取数据同步到GameObject
    /// </summary>
    [Serializable]
    public struct EcsTransform : IComponentData, ISerializable
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalScale;
        
        public EcsTransform(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            Position = position;
            Rotation = rotation;
            LocalScale = localScale;
        }
        
        public EcsTransform(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
            Rotation = Quaternion.identity;
            LocalScale = Vector3.one;
        }

        public void Serialize(BinaryWriter writer)
        {
            // Position
            writer.Write(Position.x);
            writer.Write(Position.y);
            writer.Write(Position.z);
            
            // Rotation
            writer.Write(Rotation.x);
            writer.Write(Rotation.y);
            writer.Write(Rotation.z);
            writer.Write(Rotation.w);
            
            // Scale
            writer.Write(LocalScale.x);
            writer.Write(LocalScale.y);
            writer.Write(LocalScale.z);
        }

        public void Deserialize(BinaryReader reader)
        {
            // Position
            Position = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            
            // Rotation
            Rotation = new Quaternion(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            
            // Scale
            LocalScale = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
        }
    
}
}

