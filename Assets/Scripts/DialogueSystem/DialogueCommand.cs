using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class DialogueCommand : IEnumerable<DialogueCommand>, IEquatable<DialogueCommand>
    {
        public bool IsRoot => Parent == this;
        public DialogueState State => m_state;
        public string GUID => GetHashCode().ToString();

        [HideInInspector]
        public string Name;

        [TextArea]
        public string Text;
        public DialogueActor Actor;
        public uint ActorSprite;

        public DialogueBehavior Behavior; 

        public DialogueCommand Parent;
        public DialogueCommand Next;

        private string m_guid;
        private DialogueState m_state;

        private DialogueCommand(DialogueCommand parent, DialogueCommand next)
        {
            Name = string.Empty;
            Text = string.Empty;
            Behavior = DialogueBehavior.None;

            Actor = null;
            ActorSprite = 0;

            Parent = parent;
            Next = next;

            m_state = DialogueState.Hidden;
            m_guid = new GUID().ToString();
        }

        public DialogueCommand(string name) : this(null, null)
        {
            Name = name;
            Parent = this;
        }

        internal void ProcessCommand(DialogueState newState)
        {
            m_state = newState;
        }

        /// <summary>
        /// Enqueue a new child
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public DialogueCommand AddChild(string name)
        {
            if(Next != null)
            {
                DialogueCommand prev = Next;
                Next = new DialogueCommand(this, null);
                Next.Name = name;
                Next.Next = prev;

                return Next;       
            }

            Next = new DialogueCommand(this, null);
            Next.Name = name;
            return Next;
        }

        /// <summary>
        /// Remove this node.
        /// </summary>
        public void Remove()
        {
            if(IsRoot)
            {
                Next.Parent = Next;
                return;
            }

            if(Parent != null)
            {
                Parent.Next = Next;            
            }
        }

        /// <summary>
        /// Get the root node of the linked list
        /// </summary>
        /// <returns></returns>
        public DialogueCommand GetRoot()
        {
            DialogueCommand root = this;
            while (!root.IsRoot || root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }

        /// <summary>
        /// Clear all children
        /// </summary>
        public void Clear()
        {
            if(Next != null)
            {
                Next.Clear();
                Next = null;  
            }
        }

        public IEnumerator<DialogueCommand> GetEnumerator()
        {
            List<DialogueCommand> childs = new();
            DialogueCommand cmd = this;
            while(cmd != null)
            {
                childs.Add(cmd);
                cmd = cmd.Next;
            }

            return childs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(DialogueCommand other)
        {
            return m_guid == other.m_guid;
        }
    }
}