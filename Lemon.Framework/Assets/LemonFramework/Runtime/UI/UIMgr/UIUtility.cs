using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework
{
    public class UIUtility
    {
        public static Object GetComponent(VariableBehavior behavior, string propertyName, string typeName)
        {
            if (behavior == null)
                return null;

            VariableBehavior.Property property;
            if (behavior.TryGetProperty(propertyName, out property))
            {
                if (string.IsNullOrEmpty(typeName) || typeName == "GameObject")
                    return property.GameObject;

                for (int i = 0; i < property.Components.Count; i++)
                {
                    var component = property.Components[i];
                    if (component.name == typeName)
                    {
                        return component;
                    }
                }
                //By GetComponent
                if (property.GameObject == null)
                    return null;
                var newComponent = property.GameObject.GetComponent(typeName);
                if (newComponent != null)
                {
                    property.Components.Add(newComponent);
                }
            }
            return null;
        }

        public static GameObject GetGameObject(VariableBehavior behavior, string propertyName)
        {
            if (behavior == null)
                return null;

            VariableBehavior.Property property;
            if (behavior.TryGetProperty(propertyName, out property))
            {
                return property.GameObject;
            }
            return null;
        }

        public static void SetActive(VariableBehavior behavior, string propertyName, bool bActive)
        {
            if (behavior == null)
            {
                return;
            }

            var Obj = GetGameObject(behavior, propertyName);
            SetActive(Obj, bActive);
        }

        public static void SetActive(GameObject go, bool bActive)
        {
            if (go == null)
                return;
            if (go.activeSelf == bActive)
                return;
            go.SetActive(bActive);
        }
    }
}
