using UnityEditor;
using UnityEngine;

namespace Lemon.Framework.UI.Widgets.SortOrderGroup
{
    [CustomEditor(typeof(LemonSortOrderGroup), true)]
    [CanEditMultipleObjects]
    public class LemonSortOrderGroupInspector : UnityEditor.Editor
    {
        private LemonSortOrderGroup _component;
        private SerializedProperty _sortOrderProperty;
        private bool _isDisableSortOrder = true;

        protected void OnEnable()
        {
            _component = (LemonSortOrderGroup)target;
            _sortOrderProperty = serializedObject.FindProperty("sortOrder");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Sort Order", GUILayout.Width(100));
                EditorGUI.BeginDisabledGroup(_isDisableSortOrder);
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(_sortOrderProperty, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _component.SetOrder(_sortOrderProperty.intValue);
                    }
                }
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Modify"))
                {
                    _isDisableSortOrder = !_isDisableSortOrder;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Max Sort Order", GUILayout.Width(100));
                EditorGUI.BeginDisabledGroup(true);
                {
                    _component.MaxSortOrder = EditorGUILayout.IntField(_component.MaxSortOrder);
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                _component.Refresh(true);
            }

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_component);
            }
        }
    }
}