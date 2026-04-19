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

    [HideInInspector]
    public float FrequencyQuality = 0.0f; // NOUVEAU : 1 = Frequence parfaite, 0 = Que du bruit

    private List<RadioBroadcastBehaviour> m_broadcasts;
    private float k_minFreq = 0.0f;
    private float k_maxFreq = 2000.0f;

    private float m_maxVolumeMult = 0.9f;
    private float m_freq = 0.0f;

    void Start()
    {
        k_minFreq = MinFreq;
        k_maxFreq = MaxFreq;

        m_broadcasts = new List<RadioBroadcastBehaviour>(Broadcasts);

        RegisterBroadcasts();

        // On l'allume au demarrage que si la case IsOn est cochee (On va l'allumer ce fils de pute)
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
        IsOn = !IsOn; // Inverse l'etat (Mamie branchee, mamie debranchee, mamie vivante, mamie morte.)

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
        // SI LA RADIO EST ETEINTE, ON COUPE LE VOLUME ET ON S'ARRETE LA (FBI OPEN UP)
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
        float maxTuningAccuracy = 0.0f;

        foreach (RadioBroadcastBehaviour be in m_broadcasts)
        {
            // utilisation d'une cloche de gauss (T sur qu'il y a rien qui cloche? Moi je pense que si)
            float delta = Mathf.Abs(freq - be.Freq);


            float rawExp = Mathf.Exp(-Mathf.Pow(delta / (be.Bandwidth / 2), 2));


            maxTuningAccuracy = Mathf.Max(maxTuningAccuracy, rawExp);


            float exp = rawExp * AntennaSignalQuality;

            be.VolumeMultiplicator = exp * GlobalVolume;
            noiseVolume = Mathf.Max(noiseVolume, exp);
        }


        FrequencyQuality = maxTuningAccuracy;

        NoiseOSC.VolumeMultiplicator = (1.0f - noiseVolume) * m_maxVolumeMult * GlobalVolume;
    }
    // --- NOUVEAU : POUR LE SYSTEME DE PIRATAGE ---

    // Permet de lire la frequence depuis un autre script
    public float CurrentFrequency => m_freq;

    
    public RadioBroadcastBehaviour GetTargetedBroadcast()
    {
        if (m_broadcasts == null || m_broadcasts.Count == 0) return null;

        RadioBroadcastBehaviour bestMatch = null;
        float maxExp = 0.1f; // Il faut capter au moins a 10% pour pouvoir pirater

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

        return bestMatch; // Renvoie null si le joueur est sur du bruit blanc
    }
}