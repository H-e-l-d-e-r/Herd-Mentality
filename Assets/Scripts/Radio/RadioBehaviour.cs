using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RadioBehaviour : MonoBehaviour
{
    public float Frequence
    {
        get => FreqKnob.Value;
        set => FreqKnob.SetValue(value);
    }

    public float Orientation
    {
        get => Antenna.Value;
        set => Antenna.SetValue(value);
    }

    public AudioListener Listener;
    public RadioManager Manager;

    public FrequencyBehaviour FreqKnob;
    public AntennaTunerBehaviour Antenna;
    public RadioDecrypter Decrypter;

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
    public bool IsOn {
        get => m_isEnabled;
        set
        {
            m_isEnabled = value;

            if (IsOn)
            {
                EnableBroadcasts();
            }
            else
            {
                DisableBroadcasts();
            }
        }
    } // NOUVEAU MON KIKI

    // NEW Pour l'Audimat
    [HideInInspector]
    public float FrequencyQuality = 0.0f; // 1 = Frequence parfaite, 0 = Que du bruit

    private List<RadioBroadcastBehaviour> m_broadcasts;
    private float k_minFreq = 0.0f;
    private float k_maxFreq = 30000.0f;

    private float m_maxVolumeMult = 0.9f;
    private float m_freq = 0.0f;
    private float m_orientation = 0.0f;

    private bool m_isEnabled;

    // NEW Pour le Hack 
    public float CurrentFrequency => m_freq;

    void Start()
    {
        k_minFreq = MinFreq;
        k_maxFreq = MaxFreq;

        m_broadcasts = new List<RadioBroadcastBehaviour>(Broadcasts);

        FreqKnob.OnValueChange.AddListener((value) =>
        {
            m_freq = value;
            Decrypter.InteruptRecoding();
            Decrypter.InteruptTranslating();
        });

        Antenna.OnValueChange.AddListener((value) =>
        {
            m_orientation = value;
            Decrypter.InteruptRecoding();
            Decrypter.InteruptTranslating();
        });

        RegisterBroadcasts();

        // On l'allume au démarrage que si la case IsOn est cochée 
        if (IsOn) EnableBroadcasts();
        else DisableBroadcasts();
    }

    private void OnEnable()
    {
        EnableBroadcasts();
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

            foreach (RadioBroadcastBehaviour be in m_broadcasts)
            {
                if (be.enabled)
                {
                    be.RadioUpdate();
                }
            }

            OnRadioTick.Invoke($"{(int)(Manager.GameClock.Now / 60)}m {(int)(Manager.GameClock.Now % 60)}s");
        }
    }

    public void TogglePower()
    {
        IsOn = !IsOn; // Inverse l'état (Mamie branché, mamie débranché, mamie vivante, mamie morte
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

        // orientation entre 0f et 1f
        float ori = Antenna.Value; 

        float noiseVolume = 0.0f;
        float maxTuningAccuracy = 0.0f; // NOUVEAU : Pour l'audimat

        foreach (RadioBroadcastBehaviour be in m_broadcasts)
        {
            // frequence mask
            // utilisation d'une cloche de gauss :3 (T sur qu'il y a rien qui cloche? Moi je pense que si)
            float deltaF = Mathf.Abs(freq - be.Mask.Frequence);
            float rawExpF = Mathf.Exp(-Mathf.Pow(deltaF / (be.Mask.Bandwidth / 2), 2));

            // orientation mask
            float deltaO = Mathf.Abs(ori - be.Mask.Orientation);
            float rawExpO = Mathf.Exp(-Mathf.Pow(deltaO / (be.Mask.OrientationTresh / 2), 2));

            float signalStrength = rawExpF * rawExpO;

            // On garde la meilleure précision pour l'audimat
            maxTuningAccuracy = Mathf.Max(maxTuningAccuracy, signalStrength);

            be.VolumeMultiplicator = Mathf.Clamp01(GlobalVolume * signalStrength);
            noiseVolume = Mathf.Max(noiseVolume, signalStrength);
        }

        // On assigne la qualité finale de la fréquence
        FrequencyQuality = maxTuningAccuracy;

        NoiseOSC.VolumeMultiplicator = (1.0f - noiseVolume) * m_maxVolumeMult * GlobalVolume;
    }

    //  le système de piratage (Hack)

    public RadioBroadcastBehaviour GetCurrentBroadcast()
    {
        if (m_broadcasts == null || m_broadcasts.Count == 0) return null;

        float freq = Mathf.Clamp(m_freq, k_minFreq, k_maxFreq);

        float ori = Antenna.Value; 

        // minimum captable signal
        float maxSignal = 0.1f;
        RadioBroadcastBehaviour radio = null;
        
        foreach (var be in m_broadcasts)
        {
            // if the broadcast cannot be heard, we skip it
            if (!be.IsListenable || be.IsHerd)
            {
                continue;
            }

            float deltaF = Mathf.Abs(freq - be.Mask.Frequence);
            float rawExpF = Mathf.Exp(-Mathf.Pow(deltaF / (be.Mask.Bandwidth / 2), 2));

            // orientation mask
            float deltaO = Mathf.Abs(ori - be.Mask.Orientation);
            float rawExpO = Mathf.Exp(-Mathf.Pow(deltaO / (be.Mask.OrientationTresh / 2), 2));

            float signalStrength = rawExpF * rawExpO;
        
            if (signalStrength > maxSignal)
            {
                maxSignal = signalStrength;
                radio = be;
            }
        }
        
        return radio;
    }
}