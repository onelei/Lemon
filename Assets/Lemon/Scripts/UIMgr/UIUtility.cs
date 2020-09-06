using UnityEngine;
using UnityEngine.UI;

namespace Lemon
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
         
        public static void SetActive(VariableBehavior behavior, string propertyName)
        {

        }
    }
}
