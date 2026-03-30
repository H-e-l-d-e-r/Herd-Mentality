using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogueSystem
{
    [AddComponentMenu("UI (Canvas)/Dialogue System Behaviour")]
    public class DialogueSystemBehaviour : MonoBehaviour
    {
        [Header("Behaviour")]
        public DialogueSettings Settings;

        [Header("Components")]
        public DialogueTypewritter Typewritter;

        [Header("Tables")]
        public DialogueTable[] RegisteredTables;

        public Dialogue System => m_system;

        private Dialogue m_system;

        private InputAction m_inputNext;
        private InputAction m_inputSkip;

        private float k_NEXT_COOLDOWN;
        private float k_SKIP_COOLDOWN;

        private float m_nextCooldown = 0.0f;
        private float m_skipCooldown = 0.0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if(Settings == null) Debug.LogError(new ArgumentNullException(nameof(Settings)));
            if(Typewritter == null) Debug.LogError(new ArgumentNullException(nameof(Typewritter)));

            m_system = new Dialogue(Settings);

            k_NEXT_COOLDOWN = Settings.InputNextCooldown;
            k_SKIP_COOLDOWN = Settings.InputSkipCooldown;
            m_nextCooldown = k_NEXT_COOLDOWN;
            m_skipCooldown = k_SKIP_COOLDOWN;

            // register event
            m_system.OnDialogueEvent += DoDialogue;
            m_system.OnDialogueStartEvent += Typewritter.Show;
            m_system.OnDialogueCloseEvent += Typewritter.Hide;

            // register tables
            foreach (DialogueTable table in RegisteredTables)
            {
                Dialogue.RegisterDialogue(table);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Typewritter.IsFinish)
            {
                m_system.Poll();

                UpdateHandleNext(m_system.Current);
            } 

            UpdateHandleSkipping(m_system.Current);
        }

        void OnEnable()
        {
            // create inputs
            m_inputNext = InputActionReference.Create(Settings.InputActionNext);
            m_inputSkip = InputActionReference.Create(Settings.InputActionSkip);

            m_inputNext.Enable();
            m_inputSkip.Enable();
        }

        void OnDisable()
        {
            m_inputNext.Disable();
            m_inputSkip.Disable();
        }

        public void PlayDialogue(DialoguePtr ptr) => Dialogue.PlayDialogue(ptr);
        public void PlayDialogue(int index) => Dialogue.PlayDialogue(new DialoguePtr(index));
        public void PlayDialogue(string str) => Dialogue.PlayDialogue(PlayDialogueString(str));

        public DialoguePtr PlayDialogueCommand(DialogueCommand cmd)
        {
            DialoguePtr ptr = Dialogue.RegisterDialogue(cmd);
            Dialogue.PlayDialogue(ptr);
            return ptr;
        }

        public DialoguePtr PlayDialogueString(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return DialoguePtr.k_INVALID;
            }

            DialoguePtr ptr = m_system.Find(name);

            if (!DialoguePtr.IsValid(ptr))
            {
                Debug.Log($"invalid dialogue name '{name}'");
                return ptr;
            }

            Dialogue.PlayDialogue(ptr);
            return ptr;    
        }

        public DialogueCommand GetCurrentCommand()
        {
            return m_system.Current;
        }

        private void DoDialogue(DialogueCommand cmd)
        {
            if(cmd.Behavior.HasFlag(DialogueBehavior.EndOfDialogue))
            {
                Typewritter.Hide();
 
                return;
            }

            Typewritter.Show();

            // write
            Typewritter.TryEnqueueCommand(cmd);
        }

        private void UpdateHandleNext(DialogueCommand cmd)
        {
            // input cooldown
            if(m_nextCooldown > Mathf.Epsilon)
            {
                m_nextCooldown -= Time.deltaTime;
                return;
            }
            
            if(cmd == null)
            {
                return;
            }

            if(cmd.State != DialogueState.Visible)
            {
                return;
            }

            if (cmd.Behavior.HasFlag(DialogueBehavior.AutoSkip))
            {
                //m_system.Next();
                //m_system.MoveNext();
            }

            if (cmd.Behavior.HasFlag(DialogueBehavior.WaitForInput))
            {
                // when input pressed
                if (m_inputNext.ReadValue<float>() > 0.5f)
                {
                    m_system.MoveNext();
                    m_nextCooldown = k_NEXT_COOLDOWN;

                    //m_system.Next();
                } 
            }
        }

        private void UpdateHandleSkipping(DialogueCommand cmd)
        {
            if(m_skipCooldown > Mathf.Epsilon)
            {
                m_skipCooldown -= Time.deltaTime;
                return;
            }

            if(cmd == null)
            {
                return;
            }

            if(m_inputSkip.ReadValue<float>() > 0.5f)
            {
                Typewritter.Reveal();
                m_skipCooldown = k_SKIP_COOLDOWN;
            }
        }
    }

}
