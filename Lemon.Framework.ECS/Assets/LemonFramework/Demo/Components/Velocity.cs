using LemonFramework.ECS;

namespace LemonFramework.ECS.Demo.Components
{
    public struct Velocity : IComponentData
    {
        public Vector3 Value;
        public Velocity(Vector3 value) => Value = value;
    }
}