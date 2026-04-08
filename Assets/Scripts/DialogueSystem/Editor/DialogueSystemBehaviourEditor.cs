using UnityEngine;
using UnityEditor;

using DialogueSystem;

namespace DialogueSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogueSystemBehaviour))]
    public class DialogueSystemBehaviourEditor : Editor
    {
        public DialogueSystemBehaviour Self => (DialogueSystemBehaviour)serializedObject.targetObject;

        private SerializedProperty m_settings;
        private SerializedProperty m_typewritter;
        private SerializedProperty m_registeredTables;

        private bool m_commandOpened = true;
        private bool m_tablesOpened = true;
        
        void OnEnable()
        {
            m_settings = serializedObject.FindProperty("Settings");
            m_typewritter = serializedObject.FindProperty("Typewritter");
            m_registeredTables = serializedObject.FindProperty("RegisteredTables");
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_settings);
            EditorGUILayout.PropertyField(m_typewritter);
            EditorGUILayout.PropertyField(m_registeredTables);

            if (Application.isPlaying)
            {
                DrawInspectorCommandArray();
                DrawInspectorTablesArray();
            }

            serializedObject.ApplyModifiedProperties();
        }

        override public bool RequiresConstantRepaint() { return true; }

        void DrawInspectorCommandArray()
        {
            m_commandOpened = EditorGUILayout.Foldout(m_commandOpened, "Dialogue Queue");

            if (m_commandOpened)
            {
                DialogueCommand command = Self.GetCurrentCommand();
                
                while (command != null)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label($"{command.Text} - {command.State}");
                    command = command.Next;
                }
            } 
        }

        void DrawInspectorTablesArray()
        {
            m_tablesOpened = EditorGUILayout.Foldout(m_tablesOpened, "Dialogue Queue");

            if (m_tablesOpened)
            {
                foreach (DialogueCommand cmd in Self.System.Commands)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label($"{cmd.Text}");
                }
            } 
        }
    }
}

