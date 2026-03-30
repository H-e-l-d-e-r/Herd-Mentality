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
        public float CharacterDelay;
        public SerializableDictionary<char, float> DelayHashmap;

        [Header("Behaviour")]
        public DialogueBehavior EndDialogueBehaviour;

        [Header("Inputs")]
        public InputActionReference InputActionNext;
        public float InputNextCooldown = 0.5f; 

        public InputActionReference InputActionSkip;
        public float InputSkipCooldown = 0.5f; 
    }
}