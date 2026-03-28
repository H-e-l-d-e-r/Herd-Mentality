using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public static RadioBehaviour Instance => s_instance;

    public AudioListener Listener;

    public float SelectedFreq;

    public RadioNoiseOSC NoiseOSC;
    public RadioBroadcastBehaviour[] Broadcasts;

    private static RadioBehaviour s_instance;
    private const float k_maxVolumeMult = 0.9f;
    private const float k_minFreq = 0.5f;
    private const float k_maxFreq = 100.0f;

    void Start()
    {
        s_instance = this;

        foreach(RadioBroadcastBehaviour be in Broadcasts)
        {
            be.Play();
        }
    }

    void Update()
    {
        UpdateSwitchFreq();
    }

    void UpdateSwitchFreq()
    {
        SelectedFreq = Mathf.Clamp(SelectedFreq, k_minFreq, k_maxFreq);

        float noiseVolume = 0.0f;

        foreach(RadioBroadcastBehaviour be in Broadcasts)
        {
            // utilisation d'une cloche de gauss :3
            float delta = Mathf.Abs(SelectedFreq - be.Freq);
            float exp = Mathf.Exp(-Mathf.Pow(delta / (be.Bandwidth / 2), 2));

            be.VolumeMultiplicator = exp;
            noiseVolume = Mathf.Max(noiseVolume, exp);
        }

        NoiseOSC.VolumeMultiplicator = 1.0f - noiseVolume;
    }
}
