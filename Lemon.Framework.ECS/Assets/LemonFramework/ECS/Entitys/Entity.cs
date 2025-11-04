using System;

namespace LemonFramework.ECS.Entitys
{
    // 实体ID
    public struct Entity : IEquatable<Entity>
    {
        public readonly int Id;
        public Entity(int id) => Id = id;
        public bool Equals(Entity other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Entity other && Equals(other);
        public override int GetHashCode() => Id;
    }
}