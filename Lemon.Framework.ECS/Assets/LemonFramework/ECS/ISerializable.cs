using System.IO;

namespace LemonFramework.ECS
{
    /// <summary>
    /// 序列化接口，用于ECS快照和网络同步
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// 序列化到二进制流
        /// </summary>
        void Serialize(BinaryWriter writer);
        
        /// <summary>
        /// 从二进制流反序列化
        /// </summary>
        void Deserialize(BinaryReader reader);
    }
}

