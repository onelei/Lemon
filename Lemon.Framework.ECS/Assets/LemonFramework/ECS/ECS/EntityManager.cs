namespace LemonFramework.ECS
{
    // 实体管理器
    public class EntityManager
    {
        private readonly ComponentManager _componentManager;
        private int _nextEntityId = 0;

        public EntityManager(ComponentManager componentManager)
        {
            _componentManager = componentManager;
        }

        public Entity CreateEntity()
        {
            return new Entity(++_nextEntityId);
        }

        public void DestroyEntity(Entity entity)
        {
            _componentManager.RemoveComponentsForEntity(entity);
        }

        public void AddComponent<T>(Entity entity, T component) where T : struct, IComponentData
        {
            _componentManager.AddComponent(entity, component);
        }
    }

    public delegate void ForEachAction<T1, T2>(ref T1 component1, ref T2 component2)
        where T1 : struct, IComponentData
        where T2 : struct, IComponentData;

    // 实体查询器
    public class EntitiesQuery
    {
        private readonly ComponentManager _componentManager;

        public EntitiesQuery(ComponentManager componentManager)
        {
            _componentManager = componentManager;
        }

        public void ForEach<T1, T2>(ForEachAction<T1, T2> action)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            var entities = _componentManager.GetEntitiesWithComponents(typeof(T1), typeof(T2));
            foreach (var entity in entities)
            {
                if (_componentManager.TryGetComponent(entity, out T1 comp1) &&
                    _componentManager.TryGetComponent(entity, out T2 comp2))
                {
                    T1 temp1 = comp1;
                    T2 temp2 = comp2;
                    action(ref temp1, ref temp2);
                    _componentManager.SetComponent(entity, temp1);
                    _componentManager.SetComponent(entity, temp2);
                }
            }
        }
    }
}