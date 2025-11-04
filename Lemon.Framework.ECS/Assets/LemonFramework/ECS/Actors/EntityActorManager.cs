using System.Collections.Generic;
using LemonFramework.ECS.Entitys;
using UnityEngine;

namespace LemonFramework.ECS.Actors
{
    /// <summary>
    /// Unity ECS 同步管理器
    /// 管理所有Unity GameObject与ECS实体的同步
    /// </summary>
    public class EntityActorManager : MonoBehaviour
    {
        private World _ecsWorld;
        private readonly List<EntityActor> _entityActors = new List<EntityActor>();
        
        /// <summary>
        /// 设置ECS场景
        /// </summary>
        public void SetWorld(World world)
        {
            _ecsWorld = world;
        }
        
        /// <summary>
        /// 注册一个Unity GameObject与ECS实体的关联
        /// </summary>
        public EntityActor RegisterEntity(GameObject gameObject, Entity entity)
        {
            var entityActor = gameObject.GetComponent<EntityActor>();
            if (entityActor == null)
            {
                entityActor = gameObject.AddComponent<EntityActor>();
            }
            
            entityActor.Initialize(_ecsWorld, entity);
            
            if (!_entityActors.Contains(entityActor))
            {
                _entityActors.Add(entityActor);
            }
            
            return entityActor;
        }
        
        /// <summary>
        /// 注销实体
        /// </summary>
        public void UnregisterEntity(EntityActor actor)
        {
            _entityActors.Remove(actor);
        }
        
        /// <summary>
        /// Unity主线程Update：从ECS同步所有数据
        /// </summary>
        void LateUpdate()
        {
            // 在LateUpdate中同步，确保ECS已经更新完毕
            for (int i = _entityActors.Count - 1; i >= 0; i--)
            {
                if (_entityActors[i] == null)
                {
                    _entityActors.RemoveAt(i);
                    continue;
                }
                
                _entityActors[i].Sync();
            }
        }
    }
}

