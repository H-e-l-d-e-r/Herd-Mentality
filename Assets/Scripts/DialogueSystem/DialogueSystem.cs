using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    public delegate void OnDialogueCallback(DialogueCommand cmd);
    public delegate void OnDialogueStartCallback();
    public delegate void OnDialogueCloseCallback();

    /// <summary>
    /// This is the main dialogue class.
    /// It class handles the dialogue queuing and event handlers. 
    /// 
    /// This class is a singleton which means that you can access to a static instance.
    /// </summary>
    public class Dialogue : IEnumerator<DialogueCommand>, IEnumerable<DialoguePtr>
    {
        public static Dialogue Instance => s_instance;

        public event OnDialogueCallback OnDialogueEvent;
        public event OnDialogueStartCallback OnDialogueStartEvent;
        public event OnDialogueCloseCallback OnDialogueCloseEvent;

        public DialogueSettings Settings
        {
            get; private set;
        }

        public DialogueCommand Current
        {
            get => m_current;
            private set => m_current = value;
        }

        public bool HasCommand => m_current != null;

        public DialogueCommand[] Commands
        {
            get => m_registerdCommands.ToArray();
        }

        object IEnumerator.Current => Current;

        private static Dialogue s_instance = null;

        private List<DialogueCommand> m_registerdCommands;
        private Queue<DialoguePtr> m_dialogueQueue;
        
        private DialogueCommand m_current; 

        public Dialogue(DialogueSettings settings)
        {
            if (s_instance != null)
            {
                Debug.LogError(new Exception("a Dialogue System instance already exists!"));
            }

            Settings = settings;

            m_registerdCommands = new List<DialogueCommand>();
            m_dialogueQueue = new Queue<DialoguePtr>();
            m_current = null;

            s_instance = this;
        }

        /// <summary>
        /// Dialogue polling. Called on every update.
        /// </summary>
        /// <returns>Returns true if it can pool to the next one</returns>
        public void Poll()
        {
            if (!HasCommand)
            {
                TryDequeueNextCommand();
                return;
            }

            DoCommand();
        }

        /// <summary> 
        /// Validate the dialogue and go to the next one.
        /// </summary>
        public bool MoveNext()
        {
            if (!HasCommand)
            {
                return false;
            }

            m_current.ProcessCommand(DialogueState.Finished);

            return true;
        }

        /// <summary>
        /// Remove all queued dialogues
        /// </summary>
        public void Reset()
        {
            m_dialogueQueue.Clear();
            Current = null;
        }

        /// <summary>
        /// Find a dialogue ptr by using a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DialoguePtr Find(string name)
        {
            int index = m_registerdCommands.FindIndex(cmd => cmd.Name == name);

            // check for invalid index?
            return new DialoguePtr(index);
        }

        private void DoCommand()
        {
            switch (m_current.State)
            {
                case DialogueState.Hidden:
                    OnDialogueStartEvent?.Invoke();

                    m_current.ProcessCommand(DialogueState.Typing);
                    break;
                
                case DialogueState.Typing:
                    OnDialogueEvent?.Invoke(m_current);
                    m_current.ProcessCommand(DialogueState.Visible);
                    break;
                
                case DialogueState.Finished:
                    m_current.ProcessCommand(DialogueState.Hidden);
                    m_current = m_current.Next;

                    if(m_current == null)
                    {
                        OnDialogueCloseEvent?.Invoke();
                        break;
                    }

                    break;

                default: break;
            }
        }

        private bool TryDequeueNextCommand()
        {
            // avoid more dequeues
            if(HasCommand)
            {
                return false;
            }

            // dequeue
            if(m_dialogueQueue.TryDequeue(out DialoguePtr ptr))
            {
                if(ptr == DialoguePtr.k_INVALID || ptr >= m_registerdCommands.Count)
                {
                    return false;
                }

                m_current = m_registerdCommands[ptr];
                m_current.ProcessCommand(DialogueState.Hidden);

                return true;
            }

            return false;
        }

        public IEnumerator<DialoguePtr> GetEnumerator()
        {
            return m_dialogueQueue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            Reset();

            m_registerdCommands.Clear();
            s_instance = null;
        }

        /// <summary>
        /// Register a new command using a dialogue table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DialoguePtr RegisterDialogue(DialogueTable table)
        {
            table.TryRelink();
            return RegisterDialogue(table.ToArray().First());
        }

        /// <summary>
        /// Register a new command linked array.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Returns a pointer to the dialogue</returns>
        public static DialoguePtr RegisterDialogue(DialogueCommand command)
        {
            if(s_instance == null)
            {
                Debug.LogError(new Exception("no instance exists!"));
                return DialoguePtr.k_INVALID;
            }

            if(command == null)
            {
                Debug.LogError(new ArgumentException("invalid dialogue command", nameof(command)));
                return DialoguePtr.k_INVALID;
            }

            // if is already registered
            if (s_instance.m_registerdCommands.Contains(command))
            {
                return new DialoguePtr(s_instance.m_registerdCommands.IndexOf(command));
            }

            s_instance.m_registerdCommands.Add(command);
            return new DialoguePtr(s_instance.m_registerdCommands.Count - 1);
        }

        /// <summary>
        /// Remove and clear a registered dialogue pointer
        /// </summary>
        /// <param name="ptr"></param>
        public static void DestroyDialogue(DialoguePtr ptr)
        {
            if(s_instance == null)
            {
                Debug.LogError(new Exception("no instance exists!"));
                return;
            }

            if (!DialoguePtr.IsValid(ptr))
            {
                Debug.LogError(new ArgumentException("invalid dialogue pointer", nameof(ptr)));
            }

            s_instance.m_registerdCommands[ptr].Clear();
            s_instance.m_registerdCommands.RemoveAt(ptr);
        }

        /// <summary>
        /// Start a dialogue.
        /// </summary>
        /// <param name="ptr">A pointer to an existing registered dialogue</param>
        /// <returns>Returns if the dialogue has been played.</returns>
        public static bool PlayDialogue(DialoguePtr ptr)
        {
            if(s_instance == null)
            {
                Debug.LogError(new Exception("no instance exists!"));
                return false;
            }

            if (!DialoguePtr.IsValid(ptr))
            {
                Debug.LogError(new ArgumentException("invalid dialogue pointer", nameof(ptr)));
            }

            // enqueue the dialogue pointer
            if (!s_instance.m_dialogueQueue.Contains(ptr))
            {
                s_instance.m_dialogueQueue.Enqueue(ptr);
            }

            return true;
        }

        /// <summary>
        /// Play a one shot dialogue.
        /// </summary>
        /// <param name="text">dialogue content</param>
        /// <returns>Returns if the dialogue has been played.</returns>
        public static bool DisplayDialogueString(string text) => DisplayDialogueString(text, null);

        /// <summary>
        /// Play a one shot dialogue.
        /// </summary>
        /// <param name="text">dialogue content</param>
        /// <param name="actor">dialogue speaker's actor</param>
        /// <returns>Returns if the dialogue has been played.</returns>
        public static bool DisplayDialogueString(string text, DialogueActor actor)
        {
            if(s_instance == null)
            {
                Debug.LogError(new Exception("no instance exists!"));
                return false;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            string guid = new GUID().ToString();

            DialogueCommand cmd = new DialogueCommand(guid);
            cmd.Text = text;
            cmd.Actor = actor;

            DialoguePtr ptr = RegisterDialogue(cmd);
            PlayDialogue(ptr);

            // todo: try to find a way to destroy the dialogue pointer after it has been used
            //DestroyDialogue(ptr);

            return true;
        }
    }   
}
