using System;
using System.Collections.Generic;

namespace LemonFramework.ECS
{
    // 帧数据结构，存储每帧的输入和状态信息
    [Serializable]
    public struct FrameData
    {
        public int frameNumber;           // 帧号
        public float deltaTime;           // 帧间隔时间
        public Dictionary<string, object> inputs;  // 输入数据
        public Dictionary<int, Dictionary<Type, IComponentData>> entityStates; // 实体状态
        
        public FrameData(int frame, float dt)
        {
            frameNumber = frame;
            deltaTime = dt;
            inputs = new Dictionary<string, object>();
            entityStates = new Dictionary<int, Dictionary<Type, IComponentData>>();
        }
        
        // 添加输入数据
        public void AddInput(string key, object value)
        {
            if (inputs == null)
                inputs = new Dictionary<string, object>();
            inputs[key] = value;
        }
        
        // 获取输入数据
        public T GetInput<T>(string key)
        {
            if (inputs != null && inputs.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default(T);
        }
        
        // 添加实体状态
        public void AddEntityState(int entityId, Type componentType, IComponentData component)
        {
            if (entityStates == null)
                entityStates = new Dictionary<int, Dictionary<Type, IComponentData>>();
                
            if (!entityStates.TryGetValue(entityId, out var components))
            {
                components = new Dictionary<Type, IComponentData>();
                entityStates[entityId] = components;
            }
            
            components[componentType] = component;
        }
        
        // 获取实体状态
        public T GetEntityComponent<T>(int entityId) where T : struct, IComponentData
        {
            if (entityStates != null && 
                entityStates.TryGetValue(entityId, out var components) &&
                components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }
            return default(T);
        }
    }
}