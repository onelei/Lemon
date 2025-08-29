namespace LemonFramework.ECS
{
    // 系统基类
    public abstract class ComponentSystem
    {
        protected ComponentManager ComponentManager { get; private set; }
        protected EntityManager EntityManager { get; private set; }
        protected EntitiesQuery Entities { get; private set; }

        public void Initialize(ComponentManager componentManager, EntityManager entityManager)
        {
            ComponentManager = componentManager;
            EntityManager = entityManager;
            Entities = new EntitiesQuery(componentManager);
            OnInitialize();
        }

        public void Update(TimeData timeData) => OnUpdate(timeData);

        protected virtual void OnInitialize()
        {
        }

        protected abstract void OnUpdate(TimeData timeData);
    }
}