using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
namespace UnityEditor.UI
{
    [CustomEditor(typeof(ModGrid), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the GridLayout Component.
    /// Extend this class to write a custom editor for a component derived from GridLayout.
    /// </summary>
    public class ModGridEditor : Editor
    {
        SerializedProperty m_Padding;
        SerializedProperty m_CellSize;
        SerializedProperty m_Spacing;
        SerializedProperty m_StartCorner;
        SerializedProperty m_StartAxis;
        SerializedProperty m_ChildAlignment;
        SerializedProperty m_Constraint;
        SerializedProperty m_ConstraintCount;
        SerializedProperty m_AutoWidth;
        SerializedProperty m_AutoColumn;

        protected virtual void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_CellSize = serializedObject.FindProperty("m_CellSize");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_Constraint = serializedObject.FindProperty("m_Constraint");
            m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");
            m_AutoWidth = serializedObject.FindProperty("m_AutoWidth");
            m_AutoColumn = serializedObject.FindProperty("m_AutoColumn");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_CellSize, true);
            EditorGUILayout.PropertyField(m_AutoWidth, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            EditorGUILayout.PropertyField(m_Constraint, true);
            if (m_Constraint.enumValueIndex > 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_ConstraintCount, true);
                EditorGUILayout.PropertyField(m_AutoColumn, true);
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
