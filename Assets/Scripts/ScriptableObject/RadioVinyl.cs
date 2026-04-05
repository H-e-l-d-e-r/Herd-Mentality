using UnityEngine;

[CreateAssetMenu(fileName = "Vinyl", menuName = "Herd Mentality/Radio Vinyl")]
public class RadioVinyl : ScriptableObject
{
    [Header("Meta")]
    public string Title;
    public string Author;
    public Color Color;

    [Header("Audio Clip")]
    public AudioClip Clip;

    [Range(0.0f, 1.0f)]
    public float Volume;

    public override string ToString()
    {
        return $"{Author} - {Title}";
    }
}
