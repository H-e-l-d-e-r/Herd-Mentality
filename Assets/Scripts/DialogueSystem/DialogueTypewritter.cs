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
        public AudioSource AudioSource;

        [Header("Dynamics")]
        public TMP_Text DialogueText; 
        public TMP_Text CharacterNameText;

        [Space]
        public Image NextCallToAction;
        public Image ActorLeft;
        public Image ActorRight;

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

            ActorLeft.sprite = null;
            ActorRight.sprite = null;
            ActorLeft.color = new Color(0, 0, 0, 0);
            ActorRight.color = new Color(0, 0, 0, 0);

            AudioSource.pitch = 1.0f;
            AudioSource.volume = 1.0f;
            AudioSource.clip = null;
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

            if (Dialogue.Instance.Settings.EnableRichText)
            {
                int richTextTagStart = value.IndexOfAny("<"); 
                int richTextTagEnd = value.IndexOfAny(">"); 

                // when there is a rich text tag
                // and it is inside the insert length
                // and there is a text tag end
                if(richTextTagStart > 0 && richTextTagEnd > 0)
                {
                }

                if(richTextTagStart > 0 && richTextTagStart <= length && richTextTagEnd > richTextTagStart)
                {
                    Debug.Log($"{richTextTagStart} {richTextTagEnd}");
                    length += richTextTagEnd - richTextTagStart + 1;
                }
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
                AddStringToBuffers(
                    m_command.Text.Substring(m_characterIndex), 
                    Dialogue.Instance.Settings.CharacterIncrement
                );

                ClearTypewritterCooldowns();

                // update dialogue informations
                if (m_command.Actor)
                {
                    UpdateCharacterUI();
                    PlayCharacterSound();
                }
            }

            if (IsFinish)
            {
                ClearBuffers();
                ClearTypewritterCooldowns();
            }
        }

        void UpdateUI()
        {
            NextCallToAction.SetTransparency(Convert.ToSingle(IsFinish));
        }

        void UpdateCharacterUI()
        {
            CharacterName = m_command.Actor.Name;
            if (m_command.Actor.HasSprites)
            {
                uint sprite = (uint)Mathf.Min(m_command.ActorSprite, m_command.Actor.Sprites.Length - 1);

                if (m_command.Actor.SpriteSide == DialogueActor.DialogueActorPosition.Left)
                {
                    ActorLeft.sprite = m_command.Actor.Sprites[sprite];
                    ActorLeft.color = Color.white;
                }
                else
                {
                    ActorRight.sprite = m_command.Actor.Sprites[sprite];
                    ActorRight.color = Color.white;
                }
            }
        }

        void PlayCharacterSound()
        {
            if (!Dialogue.Instance.Settings.EnableCharacterSounds)
            {
                return;
            }

            if(m_command.Actor.CharacterAudio == null)
            {
                return;
            }
 
            if(UnityEngine.Random.Range(0.0f, 1.0f) <= m_command.Actor.AudioProbability)
            {
                float volume = (float)UnityEngine.Random.Range(m_command.Actor.VolumeRandomness.x, m_command.Actor.VolumeRandomness.y);
                float pitch = (float)UnityEngine.Random.Range(m_command.Actor.PitchRandomness.x, m_command.Actor.PitchRandomness.y);
                
                AudioSource.pitch = pitch;
                AudioSource.volume = volume * Dialogue.Instance.Settings.Volume;
                AudioSource.PlayOneShot(m_command.Actor.CharacterAudio);
            }
            
        }

        public override string ToString()
        {
            return m_string.ToString();
        }
    }
}