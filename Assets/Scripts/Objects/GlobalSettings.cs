using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Settings", menuName = "Herd Mentality/Global Settings")]
public class GlobalGameSettings : SingletonScriptableObject<GlobalGameSettings>
{
    [Header("Inputs")]
    [Range(0.0f, 0.5f)]
    public float GenericInputCooldown;
    public InputActionReference NextCameraInputAction;
    public InputActionReference InteractInputAction;
    public InputActionReference MousePositionInputAction;

    [Header("Quests")]
    public QuestObject[] QuestObjects;

    [Header("Atomization")]
    [Tooltip("En minutes")]
    public float RadioPlayTime = 4.5f;
    public float LerpStrengh = 0.5f;
    public float FrequenceTreshold = 10.0f;
    public float OrientationTreshold = 10.0f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Accroissement de l'appreciation pour la musique par secondes")]
    public float AppreciationIncreaseMusic = 0.5f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Decroissement de l'appreciation pour la musique par secondes")]
    public float AppreciationDecreaseMusic = 0.5f;

    [Tooltip("Accroisseement de l'appreciation lors d'une action")]
    public float AppreciationIncrease = 1.0f;

    [Header("Registries")]
    public SequenceObject[] Sequences;
}
