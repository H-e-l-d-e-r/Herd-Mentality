using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public static RadioBehaviour Instance => s_instance;

    public AudioListener Listener;

    [Header("Frequencies")]
    public float MinFreq;
    public float MaxFreq;

    [Header("Broadcast")]
    public RadioNoiseOSC NoiseOSC;
    public RadioBroadcastBehaviour[] Broadcasts;

    private static RadioBehaviour s_instance;
    private float k_minFreq = 0.0f;
    private float k_maxFreq = 2000.0f;

    private float m_maxVolumeMult = 0.9f;
    private float m_freq = 0.0f;

    void Start()
    {
        s_instance = this;

        k_minFreq = MinFreq;
        k_maxFreq = MaxFreq;

        foreach(RadioBroadcastBehaviour be in Broadcasts)
        {
            be.Play();
        }
    }

    void Update()
    {
        UpdateSwitchFreq();
    }

    public void UpdateFrequenceKnobs(float v)
    {
        m_freq = v;
    }

    void UpdateSwitchFreq()
    {
        float freq = Mathf.Clamp(m_freq, k_minFreq, k_maxFreq);

        float noiseVolume = 0.0f;

        foreach(RadioBroadcastBehaviour be in Broadcasts)
        {
            // utilisation d'une cloche de gauss :3
            float delta = Mathf.Abs(freq - be.Freq);
            float exp = Mathf.Exp(-Mathf.Pow(delta / (be.Bandwidth / 2), 2));

            be.VolumeMultiplicator = exp;
            noiseVolume = Mathf.Max(noiseVolume, exp);
        }

        NoiseOSC.VolumeMultiplicator = (1.0f - noiseVolume) * m_maxVolumeMult;
    }
}
