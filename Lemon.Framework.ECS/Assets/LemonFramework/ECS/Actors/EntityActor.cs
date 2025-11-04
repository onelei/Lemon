using LemonFramework.ECS.Components;
using LemonFramework.ECS.Entitys;
using UnityEngine;

namespace LemonFramework.ECS.Actors
{
    /// <summary>
    /// Unity GameObject 桥接类
    /// 在Unity主线程中，从多线程的ECS读取数据并同步到GameObject
    /// </summary>
    public class EntityActor : MonoBehaviour
    {
        private World _ecsWorld;
        private Entity _ecsEntity;
        private Transform _cachedTransform;
        
        public Entity ECSEntity => _ecsEntity;
        
        /// <summary>
        /// 初始化桥接
        /// </summary>
        public void Initialize(World world, Entity entity)
        {
            _ecsWorld = world;
            _ecsEntity = entity;
            _cachedTransform = transform;
        }
        
        /// <summary>
        /// 从ECS同步Transform数据到Unity GameObject
        /// 在Unity主线程的Update中调用
        /// </summary>
        public void Sync()
        {
            if (_ecsWorld == null || _cachedTransform == null)
                return;
            
            // 线程安全地从ECS读取Transform数据
            if (_ecsWorld.ComponentManager.TryGetComponent(_ecsEntity, out EcsTransform ecsTransform))
            {
                _cachedTransform.position = ecsTransform.Position;
                _cachedTransform.rotation = ecsTransform.Rotation;
                _cachedTransform.localScale = ecsTransform.LocalScale;
            }
        }
    }
}