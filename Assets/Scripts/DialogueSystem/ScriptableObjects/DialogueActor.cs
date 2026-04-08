using System;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Actor", menuName = "Dialogue/Dialogue Actor")]
    public class DialogueActor : ScriptableObject
    {
        public bool HasSprites => Sprites.Length > 0;

        [Header("Informations")]
        public string Name;
        
        [TextArea]
        [Tooltip("A reminder. Not used in the dialogue system.")]
        public string Description;

        [Header("Sprites")]
        public Sprite[] Sprites;
        public DialogueActorPosition SpriteSide;

        [Header("Sounds")]
        public AudioClip CharacterAudio;

        [Range(0, 1)]
        public float AudioProbability;

        public Vector2 PitchRandomness = Vector2.one;
        public Vector2 VolumeRandomness = Vector2.one;

        public enum DialogueActorPosition
        {
            Left, Right
        }
    }
}