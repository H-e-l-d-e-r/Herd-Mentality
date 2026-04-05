using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Herd Mentality/Global Settings")]
public class GlobalGameSettings : SingletonScriptableObject<GlobalGameSettings>
{
    [Range(0.0f, 0.5f)]
    public float GenericInputCooldown;

    public List<RadioSequenceObject> Sequences;
}
