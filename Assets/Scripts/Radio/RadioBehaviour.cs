using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RadioBehaviour : MonoBehaviour
{
    public AudioListener Listener;
    public RadioManager Manager;

    [Header("Parameters")]
    public float MinFreq;
    public float MaxFreq;

    [Range(0.0f, 1.0f)]
    public float GlobalVolume;

    [Header("Broadcast")]
    public RadioNoiseOSC NoiseOSC;
    public RadioBroadcastBehaviour[] Broadcasts;

    [Header("Events")]
    public UnityEvent OnRadioEnable;
    public UnityEvent<string> OnRadioTick;
    public UnityEvent OnRadioDisable;

    [HideInInspector]
    public bool IsOn = false; // NOUVEAU MON KIKI

    [HideInInspector]
    public float AntennaSignalQuality = 1.0f; // 1 = Son Parfait, 0 = Que du bruit blanc

    // NEW Pour l'Audimat
    [HideInInspector]
    public float FrequencyQuality = 0.0f; // 1 = Frequence parfaite, 0 = Que du bruit

    private List<RadioBroadcastBehaviour> m_broadcasts;
    private float k_minFreq = 0.0f;
    private float k_maxFreq = 2000.0f;

    private float m_maxVolumeMult = 0.9f;
    private float m_freq = 0.0f;

    // NEW Pour le Hack 
    public float CurrentFrequency => m_freq;

    void Start()
    {
        k_minFreq = MinFreq;
        k_maxFreq = MaxFreq;

        m_broadcasts = new List<RadioBroadcastBehaviour>(Broadcasts);

        RegisterBroadcasts();

        // On l'allume au démarrage que si la case IsOn est cochée 
        if (IsOn) EnableBroadcasts();
        else DisableBroadcasts();
    }

    private void OnEnable()
    {
        if (IsOn) EnableBroadcasts();
    }

    private void OnDisable()
    {
        DisableBroadcasts();
    }

    void Update()
    {
        if (IsOn)
        {
            UpdateSwitchFreq();

            OnRadioTick.Invoke($"{(int)(Manager.Timer / 60)}m {(int)(Manager.Timer % 60)}s");
        }
    }

    public void TogglePower()
    {
        IsOn = !IsOn; // Inverse l'état (Mamie branché, mamie débranché, mamie vivante, mamie morte.)

        if (IsOn)
        {
            EnableBroadcasts();
        }
        else
        {
            DisableBroadcasts();
        }
    }

    public void UpdateFrequenceKnobs(float v)
    {
        m_freq = v;
    }

    void EnableBroadcasts()
    {
        if (m_broadcasts == null)
        {
            return;
        }

        foreach (RadioBroadcastBehaviour be in m_broadcasts)
        {
            if (be.gameObject.activeInHierarchy)
            {
                be.Play();
            }
        }

        NoiseOSC.Play();
        OnRadioEnable.Invoke();
    }

    void DisableBroadcasts()
    {
        if (m_broadcasts == null)
        {
            return;
        }

        foreach (RadioBroadcastBehaviour be in m_broadcasts)
        {
            be.Stop();
        }

        NoiseOSC.Stop();
        OnRadioDisable.Invoke();
    }

    void RegisterBroadcasts()
    {
        foreach (RadioBroadcastBehaviour be in transform.GetComponentsInChildren<RadioBroadcastBehaviour>())
        {
            if (!m_broadcasts.Contains(be))
            {
                m_broadcasts.Add(be);
            }
        }
    }

    void UpdateSwitchFreq()
    {
        // SI LA RADIO EST ÉTEINTE, ON COUPE LE VOLUME ET ON S'ARRÊTE LÀ (FBI OPEN UP)
        // if (!IsOn)
        // {
        //     foreach(RadioBroadcastBehaviour be in m_broadcasts)
        //     {
        //         be.VolumeMultiplicator = 0.0f;
        //     }
        //     if (NoiseOSC != null) NoiseOSC.VolumeMultiplicator = 0.0f;
        //     
        //     return; 
        // }

        float freq = Mathf.Clamp(m_freq, k_minFreq, k_maxFreq);

        float noiseVolume = 0.0f;
        float maxTuningAccuracy = 0.0f; // NOUVEAU : Pour l'audimat

        foreach (RadioBroadcastBehaviour be in m_broadcasts)
        {
            // utilisation d'une cloche de gauss :3 (T sur qu'il y a rien qui cloche? Moi je pense que si)
            float delta = Mathf.Abs(freq - be.Freq);
            float rawExp = Mathf.Exp(-Mathf.Pow(delta / (be.Bandwidth / 2), 2));

            // On garde la meilleure précision pour l'audimat
            maxTuningAccuracy = Mathf.Max(maxTuningAccuracy, rawExp);

            // On applique la qualité de l'antenne au volume
            float exp = rawExp * AntennaSignalQuality;

            be.VolumeMultiplicator = exp * GlobalVolume;
            noiseVolume = Mathf.Max(noiseVolume, exp);
        }

        // On assigne la qualité finale de la fréquence
        FrequencyQuality = maxTuningAccuracy;

        NoiseOSC.VolumeMultiplicator = (1.0f - noiseVolume) * m_maxVolumeMult * GlobalVolume;
    }

    //  le système de piratage (Hack)

    public RadioBroadcastBehaviour GetTargetedBroadcast()
    {
        if (m_broadcasts == null || m_broadcasts.Count == 0) return null;

        RadioBroadcastBehaviour bestMatch = null;
        float maxExp = 0.1f; // Faut capter à au moins 10% pour pouvoir pirater

        foreach (var be in m_broadcasts)
        {
            float delta = Mathf.Abs(m_freq - be.Freq);
            float exp = Mathf.Exp(-Mathf.Pow(delta / (be.Bandwidth / 2), 2));

            if (exp > maxExp)
            {
                maxExp = exp;
                bestMatch = be;
            }
        }

        return bestMatch;
    }
}