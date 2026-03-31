using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public static RadioBehaviour Instance => s_instance;

    public AudioListener Listener;

    [Header("Parameters")]
    public float MinFreq;
    public float MaxFreq;

    [Range(0.0f, 1.0f)]
    public float GlobalVolume;

    [Header("Broadcast")]
    public RadioNoiseOSC NoiseOSC;
    public RadioBroadcastBehaviour[] Broadcasts;

    private static RadioBehaviour s_instance;
    
    private List<RadioBroadcastBehaviour> m_broadcasts;
    private float k_minFreq = 0.0f;
    private float k_maxFreq = 2000.0f;

    private float m_maxVolumeMult = 0.9f;
    private float m_freq = 0.0f;

    void Start()
    {
        s_instance = this;

        k_minFreq = MinFreq;
        k_maxFreq = MaxFreq;

        m_broadcasts = new List<RadioBroadcastBehaviour>(Broadcasts);

        FindBroadcast();
        EnableBroadcasts();
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
        UpdateSwitchFreq();
    }

    public void UpdateFrequenceKnobs(float v)
    {
        m_freq = v;
    }

    void EnableBroadcasts()
    {
        if(m_broadcasts == null)
        {
            return;
        }

        foreach (RadioBroadcastBehaviour be in m_broadcasts)
        {
            be.Play();
        }
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
    }

    void FindBroadcast()
    {
        foreach(RadioBroadcastBehaviour be in transform.GetComponentsInChildren<RadioBroadcastBehaviour>())
        {
            if (!m_broadcasts.Contains(be))
            {
                m_broadcasts.Add(be);
            }
        }
    }

    void UpdateSwitchFreq()
    {
        float freq = Mathf.Clamp(m_freq, k_minFreq, k_maxFreq);

        float noiseVolume = 0.0f;

        foreach(RadioBroadcastBehaviour be in m_broadcasts)
        {
            // utilisation d'une cloche de gauss :3
            float delta = Mathf.Abs(freq - be.Freq);
            float exp = Mathf.Exp(-Mathf.Pow(delta / (be.Bandwidth / 2), 2));

            be.VolumeMultiplicator = exp * GlobalVolume;
            noiseVolume = Mathf.Max(noiseVolume, exp);
        }

        NoiseOSC.VolumeMultiplicator = (1.0f - noiseVolume) * m_maxVolumeMult * GlobalVolume;
    }
}
