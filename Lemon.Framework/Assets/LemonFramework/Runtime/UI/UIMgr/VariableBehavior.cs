using System;
using System.Collections.Generic;
using LemonFramework.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework
{
    public class VariableBehavior : BaseBehavior
    {
        public class Property
        {
            public string Name;
            public List<Component> Components;
            public GameObject GameObject;
        }

        public List<Type> serializeComponents = new List<Type>()
        {
            typeof(Image),
            typeof(Text),
            typeof(InputField),
            typeof(Button),
            typeof(Transform),
            typeof(RectTransform),

            typeof(LemonButton),
            typeof(LemonImage),
            typeof(LemonImageBox),
            typeof(LemonRawImage),
            typeof(LemonText),
            typeof(LemonToggleButton),
            typeof(LemonToggleButtonGroup),
        };

        [HideInInspector] [SerializeField] public List<Property> properties = new List<Property>();

        public void CacheAll()
        {
            properties.Clear();
            Transform[] children = CacheTransform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                var childTransform = children[i];
                if (childTransform != null)
                {
                    var childName = childTransform.name;
                    if (childName.StartsWith("V_"))
                    {
                        var property = new Property();
                        property.Name = childName;
                        property.GameObject = childTransform.gameObject;
                        var components = new List<Component>();
                        for (int j = 0; j < serializeComponents.Count; j++)
                        {
                            var component = childTransform.GetComponent(serializeComponents[j]);
                            if (component != null)
                            {
                                components.Add(component);
                            }
                        }

                        property.Components = components;
                        properties.Add(property);
                    }
                }
            }
        }

        public bool TryGetProperty(string propertyName, out Property property)
        {
            property = null;
            if (string.IsNullOrEmpty(name))
                return false;

            for (int i = 0; i < properties.Count; i++)
            {
                property = properties[i];
                if (property.Name == propertyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}