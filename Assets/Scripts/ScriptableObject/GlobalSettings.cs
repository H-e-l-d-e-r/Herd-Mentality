using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Herd Mentality/Global Settings")]
public class GlobalGameSettings : SingletonScriptableObject<GlobalGameSettings>
{
    [Header("Inputs")]
    [Range(0.0f, 0.5f)]
    public float GenericInputCooldown;

    [Header("Time")]
    [Tooltip("En minutes.")]
    public float RadioPlayTime = 4.5f;

    [Header("Registries")]
    public List<RadioSequenceObject> Sequences;
}
