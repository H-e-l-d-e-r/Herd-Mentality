using System;
using UnityEngine;

[Serializable]
public struct DecryptionsResults
{
    public DecryptionModes Mode;

    [TextArea]
    public string Content;

    public DecryptionsResults(DecryptionModes mode, string value)
    {
        Mode = mode;
        Content = value;
    }
}

public enum DecryptionModes 
{
    FromAudio,
    FromMorse,
    FromInvertAudio
}