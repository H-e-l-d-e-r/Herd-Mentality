using System;
using UnityEngine;

public class RadioNoiseOSC : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Volume = 1.0f;

    [Range(0.0f, 1.0f)]
    public float MinimumNoise = 0.05f;

    [Range(0.0f, 1.0f)]
    public float Smoothness = 0.95f;

    [HideInInspector]
    public float VolumeMultiplicator = 1.0f;

    private bool m_canPlay;
    public uint m_seed = 33423204;
    private float m_last = 0.0f;

    void Start()
    {

    }

    private void OnEnable()
    {
        m_canPlay = true;
    }

    private void OnDisable()
    {
        m_canPlay = false;
    }

    private float Next()
    {
        m_seed = 1664525 * m_seed + 1013904223;
        return ((m_seed / (float)uint.MaxValue) * 2f - 1f);
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if(!m_canPlay) return;

        float volume = Mathf.Clamp(VolumeMultiplicator * Volume, MinimumNoise, 0.5f);
        for (int i = 0; i < data.Length; i++)
        {
            float white = Next();

            m_last = m_last * Smoothness + white * (1.0f - Smoothness);
            data[i] = data[i] * (1.0f - volume) + m_last * volume;
        }
    }
}