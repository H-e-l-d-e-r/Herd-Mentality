using System;
using System.Text;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueTypewritter : MonoBehaviour
    {
        [Header("Components")]
        public CanvasGroup PanelGroup;
        public Image NextCallToAction;

        [Header("Dynamics")]
        public TMP_Text DialogueText; 
        public TMP_Text CharacterNameText;

        public string Text
        {
            get => this.ToString();
            set
            {
                DialogueText.SetText(value);
                m_string = new StringBuilder(value);
            }
        }

        public string CharacterName
        {
            get => CharacterNameText.text;
            set => CharacterNameText.SetText(value);
        }

        public bool IsFinish
        {
            get
            {
                if(m_command == null)
                {
                    return true;
                }

                return m_characterIndex == m_command.Text.Length;
            }
        }

        public bool HasCommand => m_command != null;

        private const double k_MINIMUM_TIME_DELAY = double.Epsilon;

        private StringBuilder m_string;
        private DialogueCommand m_command;

        private int m_characterIndex;
        private char m_lastCharacter;
        private double m_lastTime;

        void Awake()
        {
            // constructor
            m_string = new StringBuilder();
        } 

        void Start()
        {  
            ClearBuffers();
            ClearComponents();
            ClearTypewritterCooldowns();

            Hide();
        }

        void Update()
        {
            if (HasCommand)
            {
                UpdateTypewritter();
            }

            UpdateUI();
        }

        /// <summary>
        /// Show Dialogue box
        /// </summary>
        public void Show()
        {
            PanelGroup.alpha = 1.0f;
        }

        /// <summary>
        /// Hide Dialogue box
        /// </summary>
        public void Hide()
        {
            PanelGroup.alpha = 0.0f;
            
            ClearBuffers();
            ClearComponents();
        }

        /// <summary>
        /// Try to display a new command.
        /// </summary>
        /// <param name="command">command</param>
        /// <returns></returns>
        public bool TryEnqueueCommand(DialogueCommand command)
        {
            if(HasCommand)
            {
                return false;
            }

            m_command = command;
            return true;
        }

        public void Reveal()
        {
            if (!HasCommand)
            {
                return;
            }

            AddStringToBuffers(m_command.Text.Substring(m_characterIndex), m_command.Text.Length - m_characterIndex);
        }

        private void ClearBuffers()
        {   
            m_string.Clear();
            m_command = null;

            m_characterIndex = 0;
        }

        private void ClearComponents()
        {
            DialogueText.SetText(string.Empty);
            CharacterNameText.SetText(string.Empty);
        }

        private void ClearTypewritterCooldowns()
        {
            if (HasCommand && Dialogue.Instance.Settings.DelayHashmap.TryGetValue(m_lastCharacter, out float delay))
            {
                m_lastTime = delay;
            }
            else
            {
                m_lastTime = Dialogue.Instance.Settings.CharacterDelay;
            }
        }

        private void AddStringToBuffers(ReadOnlySpan<char> value, int length)
        {
            if (value.IsEmpty)
            {
                return;
            }

            m_string.Append(value.Slice(0, length));
            m_lastCharacter = value[0]; 
            m_characterIndex += length;

            DialogueText.text = m_string.ToString();
        }

        private void UpdateTypewritter()
        {
            m_lastTime -= Time.deltaTime;
 
            // cooldown has exceed.
            if(m_lastTime <= double.Epsilon)
            {
                CharacterName = m_command.Actor.Name;

                AddStringToBuffers(m_command.Text.Substring(m_characterIndex), 1);
                ClearTypewritterCooldowns();
            }

            if (IsFinish)
            {
                ClearBuffers();
                ClearTypewritterCooldowns();
            }
        }

        private void UpdateUI()
        {
            NextCallToAction.SetTransparency(Convert.ToSingle(IsFinish));
        }

        public override string ToString()
        {
            return m_string.ToString();
        }
    }
}