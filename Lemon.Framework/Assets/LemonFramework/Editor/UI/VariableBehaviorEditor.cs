using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LemonFramework.Editor
{
    [CustomEditor(typeof(VariableBehavior), true)]
    public class VariableBehaviorEditor : UnityEditor.Editor
    {
        private VariableBehavior component;

        public void OnEnable()
        {
            component = (VariableBehavior)target;

        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Refresh"))
            {
                component.CacheAll();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.MinWidth(100));
            GUILayout.Label("Object", GUILayout.MinWidth(100));
            GUILayout.EndHorizontal();

            for (int i = 0; i < component.properties.Count; i++)
            {
                var property = component.properties[i];
                if (property == null)
                {
                    component.properties.RemoveAt(i);
                    break;
                }
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                property.Name = GUILayout.TextField(property.Name, GUILayout.MinWidth(100));
                property.GameObject = (GameObject)EditorGUILayout.ObjectField(property.GameObject, typeof(GameObject), true, GUILayout.MinWidth(100));

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
 