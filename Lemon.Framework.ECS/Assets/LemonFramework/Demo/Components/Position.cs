using LemonFramework.ECS;

namespace LemonFramework.ECS.Demo.Components
{
    public struct Position : IComponentData
    {
        public Vector3 Value;
        public Position(Vector3 value) => Value = value;
    }
}