using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DialogueTable))] 
    public class DialogueTableEditor : Editor
    {
        private DialogueTable m_table;
        private SerializedProperty m_nameProperty;
        private SerializedProperty m_descProperty;
        private SerializedProperty m_nodesProperty;

        private bool m_nodeFolder = true;

        void OnEnable()
        {
            m_table = serializedObject.targetObject as DialogueTable;
            m_nameProperty = serializedObject.FindProperty("Name");
            m_descProperty = serializedObject.FindProperty("Description");
            m_nodesProperty = serializedObject.FindProperty("m_nodes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawProperties();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawProperties()
        {
            EditorGUILayout.LabelField("Name");
            m_nameProperty.stringValue = EditorGUILayout.TextField(m_nameProperty.stringValue);

            EditorGUILayout.PropertyField(m_descProperty);
 
            if (m_nodeFolder)
            {
                EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(m_nodesProperty);

                if (GUILayout.Button("Add Node"))
                {
                    m_table.AddNode();
                }

                if (GUILayout.Button("Relink"))
                {
                    m_table.TryRelink();
                }

                if (GUILayout.Button("Print Dialogue"))
                {
                    foreach (DialogueCommand command in m_table)
                    {
                        Debug.Log(command.Text);
                    }
                }
            }
        }
    
        // void DrawInspectorNodes()
        // {
        //     m_nodeFolder = EditorGUILayout.Foldout(m_nodeFolder, "Table");
        //     
        //     if (m_nodeFolder)
        //     {
        //         DialogueCommand command = m_table.Root;
        //         
        //         while (command != null)
        //         {
        //             EditorGUILayout.Space();
        //             EditorGUILayout.BeginHorizontal();
        //             command.Text = EditorGUILayout.TextField(command.Text);
        //             EditorGUILayout.TextField(command.GUID);
        //             EditorGUILayout.EndHorizontal();
        //             command = command.Next;
        //         }
        //     } 
        // }
    }
}