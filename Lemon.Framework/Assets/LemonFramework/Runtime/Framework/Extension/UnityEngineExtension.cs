using UnityEngine;
using System;

namespace Lemon.Framework.Extension
{
    public static class UnityEngineExtension
    {
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