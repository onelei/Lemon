using LemonFramework.ECS.Components;
using UnityEngine;

namespace LemonFramework.ECS.Systems
{
    /// <summary>
    /// 圆周运动系统
    /// 让实体围绕中心点旋转
    /// </summary>
    public class CircleMovementSystem : ComponentSystem
    {
        protected override void OnUpdate(TimeData timeData)
        {
            var entities = ComponentManager.GetEntitiesWithComponents(typeof(EcsTransform));

            foreach (var entity in entities)
            {
                if (ComponentManager.TryGetComponent(entity, out EcsTransform transform))
                {
                    // 获取原始位置（初始半径）
                    float radius = Mathf.Sqrt(transform.Position.x * transform.Position.x +
                                              transform.Position.z * transform.Position.z);

                    if (radius < 0.1f) radius = 1f;

                    // 计算旋转角度
                    float angle = timeData.Frame * 0.02f;

                    // 更新位置
                    transform.Position = new UnityEngine.Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(timeData.Frame * 0.05f) * 0.5f, // Y轴上下波动
                        Mathf.Sin(angle) * radius
                    );

                    // 更新旋转（朝向运动方向）
                    transform.Rotation = UnityEngine.Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);

                    ComponentManager.SetComponent(entity, transform);
                }
            }
        }
    }
}