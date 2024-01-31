using System;
using System.Collections.Generic;
using Lemon.Framework.ECS.Entitys;

namespace Lemon.Framework.ECS
{
    public class World
    {
        private readonly Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        private readonly Dictionary<Type, Dictionary<Entity, object>> componentsByType =
            new Dictionary<Type, Dictionary<Entity, object>>();

        public void AddEntity(Entity entity)
        {
            entities.Add(entity.Id, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity.Id);
        }

        public void AddComponent<T>(Entity entity, T component) where T : class
        {
            var type = typeof(T);
            if (!componentsByType.ContainsKey(type))
            {
                componentsByType[type] = new Dictionary<Entity, object>();
            }

            componentsByType[type][entity] = component;
        }

        public T GetComponent<T>(Entity entity) where T : class
        {
            var type = typeof(T);
            if (componentsByType.TryGetValue(type, out var components) &&
                components.TryGetValue(entity, out var component))
            {
                return component as T;
            }

            return null;
        }
    }
}