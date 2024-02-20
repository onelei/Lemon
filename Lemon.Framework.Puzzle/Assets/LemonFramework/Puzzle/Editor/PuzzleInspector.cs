using UnityEditor;
using UnityEngine;

namespace Lemon.Framework.Puzzle
{
    [CustomEditor(typeof(Puzzle))]
    public class PuzzleInspector :Editor
    {
        private Puzzle m_Target;
        private SerializedProperty m_Parent;
        private SerializedProperty m_Children;

        public void OnEnable()
        {
            m_Target = target as Puzzle;
            m_Parent = serializedObject.FindProperty("mParent");
            m_Children = serializedObject.FindProperty("mChildren");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate"))
                {
                    m_Target.GenerateAll();
                }
            }
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Parent, new GUIContent("Preview Texture"));
            EditorGUILayout.PropertyField(m_Children, new GUIContent("Children Textures"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}