using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Settings", menuName = "Dialogue/Dialogue Settings")]
    public class DialogueSettings : ScriptableObject
    {
        [Header("Typewritter")]
        [Range(0.0f, 1.0f)]
        public float CharacterDelay = 0.0f;
        
        [Range(1, 5)]
        public int CharacterIncrement = 1;
        
        public SerializableDictionary<char, float> DelayHashmap = new();

        [Tooltip("Skip rich text tags. When activate you cannot use less and greater than signs (< and >)")]
        public bool EnableRichText = true;

        [Header("Audio")]
        public bool EnableCharacterSounds = true;

        [Range(0.0f, 1.0f)]
        public float Volume = 1.0f;

        [Header("Behaviour")]
        public DialogueBehavior EndDialogueBehaviour;

        [Header("Inputs")]
        public InputActionReference InputActionNext;
        public float InputNextCooldown = 0.5f; 

        public InputActionReference InputActionSkip;
        public float InputSkipCooldown = 0.5f; 
    }
}