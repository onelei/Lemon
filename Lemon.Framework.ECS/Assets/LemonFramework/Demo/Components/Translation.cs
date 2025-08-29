using LemonFramework.ECS;

namespace LemonFramework.ECS.Demo.Components
{
    public struct Translation : IComponentData
    {
        public Vector3 Value;
        public Translation(Vector3 value) => Value = value;
    }
}