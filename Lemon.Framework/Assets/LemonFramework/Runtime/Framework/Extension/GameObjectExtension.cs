using UnityEngine;
using System;

namespace Lemon.Framework.Extension
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public static Component GetOrAddComponent(this GameObject gameObject, Type type)
        {
            var component = gameObject.GetComponent(type);
            if (component == null)
            {
                component = gameObject.AddComponent(type);
            }

            return component;
        }

        public static void RemoveComponent(this GameObject gameObject, Component component)
        {
            if (component != null)
            {
                UnityEngine.Object.Destroy(component);
            }
        }
    }
}