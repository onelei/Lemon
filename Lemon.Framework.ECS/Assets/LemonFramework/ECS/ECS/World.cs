using System.Collections.Generic;
 
namespace LemonFramework.ECS
{
    // ECS世界：管理所有实体和系统
    public class World
    {
        private readonly ComponentManager _componentManager;
        private readonly EntityManager _entityManager;
        private readonly List<ComponentSystem> _systems = new List<ComponentSystem>();
        
        // 公共属性用于访问管理器
        public ComponentManager ComponentManager => _componentManager;
        public EntityManager EntityManager => _entityManager;

        public World()
        {
            _componentManager = new ComponentManager();
            _entityManager = new EntityManager(_componentManager);
        }

        // 创建实体
        public Entity CreateEntity()
        {
            var entity = _entityManager.CreateEntity();
            return entity;
        }

        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _entityManager.AddComponent<T>(entity, component);
        }

        // 销毁实体
        public void DestroyEntity(Entity entity)
        {
            _entityManager.DestroyEntity(entity);
        }

        // 添加系统
        public void AddSystem(ComponentSystem system)
        {
            system.Initialize(_componentManager, _entityManager);
            _systems.Add(system);
        }

        // 执行所有系统的更新
        public void Update(TimeData deltaTime)
        {
            foreach (var system in _systems)
            {
                system.Update(deltaTime);
            }
        }
    }
}