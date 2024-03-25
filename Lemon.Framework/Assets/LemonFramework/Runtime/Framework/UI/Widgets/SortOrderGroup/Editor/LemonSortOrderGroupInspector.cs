using UnityEditor;
using UnityEngine;

namespace Lemon.Framework.UI.Widgets.SortOrderGroup
{
    [CustomEditor(typeof(LemonSortOrderGroup), true)]
    [CanEditMultipleObjects]
    public class LemonSortOrderGroupInspector : UnityEditor.Editor
    {
        private LemonSortOrderGroup _component;
        protected void OnEnable()
        {
            _component = (LemonSortOrderGroup)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Refresh"))
            {
                _component.RefreshEditor();
            }
            
            base.OnInspectorGUI();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
