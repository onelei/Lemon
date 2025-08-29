using LemonFramework.ECS;
using LemonFramework.ECS.Demo.Components;
using UnityEngine;

namespace LemonFramework.ECS.Demo.Systems
{
    public class MovementSystem : ComponentSystem
    {
        protected override void OnInitialize()
        {
        }

        protected override void OnUpdate(TimeData timeData)
        {
            Entities.ForEach((ref Translation translation, ref Velocity velocity) =>
            {
                translation.Value += velocity.Value * timeData.DeltaTime;
                //Debug.Log($"{translation.Value.x} {timeData.Frame}");
            });
        }
    }
}