using UnityEditor;
using UnityEngine;

namespace LemonFramework.CustomAttribute.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGUIState = GUI.enabled;
            // Set GUI enabled to false
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            // Reset GUI enabled
            GUI.enabled = previousGUIState;
        }
    }
}