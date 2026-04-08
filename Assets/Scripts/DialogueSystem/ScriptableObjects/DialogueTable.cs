using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;

using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Table", menuName = "Dialogue/Dialogue Table")]
    public class DialogueTable : ScriptableObject, IEnumerable<DialogueCommand>
    {
        public DialogueCommand Root => m_root;

        public DialogueCommand Last
        {
            get
            {
                DialogueCommand cmd = m_root;
                while(cmd.Next != null)
                {
                    cmd = cmd.Next;
                }

                return cmd;
            }
        }
        
        [Header("Name")]
        public string Name;
        
        [TextArea]
        [Tooltip("A reminder. Not used in the dialogue system.")]
        public string Description;

        [Space]
        [SerializeField]
        private List<DialogueCommand> m_nodes = new();
        private DialogueCommand m_root
        {
            get => m_nodes.First();
            set
            {
                if(m_nodes.Count >= 1)
                {
                    m_nodes[0] = value;
                }
                else
                {
                    m_nodes.Add(value);
                }
            }
        }

        public DialogueCommand AddNode() => AddNode(Name);
        public DialogueCommand AddNode(string name)
        {
            if(m_nodes.Count == 0)
            {
                m_root = new DialogueCommand(name);
                m_nodes.Add(m_root);
                return m_root;
            }

            DialogueCommand cmd = Last.AddChild(name);
            m_nodes.Add(cmd);
            return cmd;
        }

        public DialogueCommand AddNode(DialogueCommand parent, string name)
        {
            DialogueCommand cmd = parent.AddChild(name);
            if (!m_nodes.Contains(cmd))
            {
                m_nodes.Add(cmd);            
            }
            
            return cmd;
        }

        public void RemoveNode(DialogueCommand command)
        {
            m_nodes.Remove(command);
            command.Remove();
        }

        /// <summary>
        /// PLEASE DO NOT USE IT WITHOUT KNOWING WHAT THIS SHIT DOES
        /// Try to relink lonely nodes.
        /// </summary>
        public void TryRelink()
        {
            for(int i = 0; i < m_nodes.Count; i++)
            {
                if(i == 0 && !m_nodes[i].IsRoot)
                {
                    m_root = m_nodes[i];
                    m_nodes[i].Parent = m_nodes[i];    
                }

                if(m_nodes[i].Next == null && i < m_nodes.Count - 1)
                {
                    m_nodes[i].Next = m_nodes[i + 1];
                    m_nodes[i + 1].Parent = m_nodes[i];
                } 

                m_nodes[i].Name = Name;
            }
        }

        [OnOpenAsset]
        public static bool OnOpenEditor(int instance, int line)
        {
            DialogueTable table = EditorUtility.EntityIdToObject(instance) as DialogueTable;
            
            if(table != null)
            {
                
                return true;
            } 

            return false;
        }

        public IEnumerator<DialogueCommand> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DialogueCommand[] ToArray()
        {
            List<DialogueCommand> nodes = new();

            DialogueCommand cmd = m_root;
            while(cmd != null)
            {
                nodes.Add(cmd);
                cmd = cmd.Next;
            }

            return nodes.ToArray();
        }

        public List<DialogueCommand> ToList()
        {
            List<DialogueCommand> nodes = new();

            DialogueCommand cmd = m_root;
            while(cmd != null)
            {
                nodes.Add(cmd);
                cmd = cmd.Next;
            }

            return nodes;
        }
    }
}