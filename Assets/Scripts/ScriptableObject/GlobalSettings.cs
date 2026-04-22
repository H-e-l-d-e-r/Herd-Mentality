using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Herd Mentality/Global Settings")]
public class GlobalGameSettings : SingletonScriptableObject<GlobalGameSettings>
{
    [Header("Inputs")]
    [Range(0.0f, 0.5f)]
    public float GenericInputCooldown;

    [Header("Atomization")]
    [Tooltip("En minutes")]
    public float RadioPlayTime = 4.5f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Accroissement de l'appreciation pour la musique par secondes")]
    public float AppreciationIncreaseMusic = 0.5f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Decroissement de l'appreciation pour la musique par secondes")]
    public float AppreciationDecreaseMusic = 0.5f;

    [Tooltip("Accroisseement de l'appreciation lors d'une action")]
    public float AppreciationIncrease = 1.0f;

    [Header("Registries")]
    public RadioSequenceObject[] Sequences;
    public QuestObject[] Quests;
}
