using UnityEngine;

[CreateAssetMenu(fileName = "Vinyl", menuName = "Herd Mentality/Radio Vinyl")]
public class RadioVinyl : ScriptableObject
{
    public AudioClip Clip;

    [Range(0.0f, 1.0f)]
    public float Volume;
}
