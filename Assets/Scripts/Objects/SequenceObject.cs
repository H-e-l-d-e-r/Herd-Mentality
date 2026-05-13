using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Sequence", menuName = "Herd Mentality/Sequence")]
public class SequenceObject : CollectibleObject
{
    [Header("Message")]
    [Tooltip("Which audio will be played")]
    public AudioClip Clip;

    [Tooltip("The total length of the message")]
    public float Duration;

    public DecryptionsResults[] Translations = new DecryptionsResults[]
    {
        new DecryptionsResults(DecryptionModes.FromAudio, ""),
        new DecryptionsResults(DecryptionModes.FromMorse, ""),
        new DecryptionsResults(DecryptionModes.FromInvertAudio, ""),
    };
}