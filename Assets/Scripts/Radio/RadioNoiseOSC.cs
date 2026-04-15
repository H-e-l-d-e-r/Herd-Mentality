using System;
using UnityEngine;

public class RadioNoiseOSC : MonoBehaviour
{
    [Header("Noise OSC")]
    [Range(0.0f, 1.0f)]
    public float Volume = 1.0f;

    [Range(0.0f, 1.0f)]
    public float MinimumNoise = 0.05f;

    [Range(0.0f, 1.0f)]
    public float Smoothness = 0.95f;

    [Header("Mixer")]
    [Range(0.0f, 1.0f)]
    public float WhiteNoiseVolume = 0.6f;

    [Range(0.0f, 1.0f)]
    public float CrakleNoiseVolume = 0.4f;

    [HideInInspector]
    public float VolumeMultiplicator = 1.0f;

    private const float k_CRACKLE_PROBABILITY = 800f / 44100;

    private bool m_canPlay;
    private uint m_seed = 33423204;
    private float m_last = 0.0f;

    public void Play()
    {
        m_canPlay = true;
    }

    public void Stop()
    {
        m_canPlay = false;
    }

    private void OnEnable()
    {
        m_canPlay = true;
    }

    private void OnDisable()
    {
        m_canPlay = false;
    }

    private float WhiteNoiseNextSample()
    {
        m_seed = 1664525 * m_seed + 1013904223;
        return ((m_seed / (float)uint.MaxValue) * 2f - 1f);
    }

    private float PinkNoiseNextSample()
    {
        return WhiteNoiseNextSample();
    }

    private float Crakle()
    {
        float u = WhiteNoiseNextSample() * 0.5f + 0.5f;

        if (u < k_CRACKLE_PROBABILITY)
        {
            float sign = WhiteNoiseNextSample() >= 0f ? 1f : -1f;
            float amplitude = 0.4f + (WhiteNoiseNextSample() * 0.5f + 0.5f) * 0.6f;
            return sign * amplitude * 2.0f;
        }

        return 0f;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if(!m_canPlay)
        {
            // c'est au cas ou, comment je ne sais pas vraiment
            // comment la thread sonore fonctionne, on securise
            // en copiant toute les donnees.
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i];
            }

            return;
        }

        // NOUVEAU Demande si il peut jouer le bruitblanc
        float volume = Mathf.Clamp(VolumeMultiplicator * Volume, MinimumNoise, 0.5f);
        for (int i = 0; i < data.Length; i++)
        {
            float white = Crakle() * CrakleNoiseVolume + PinkNoiseNextSample() * WhiteNoiseVolume;

            m_last = m_last * Smoothness + white * (1.0f - Smoothness);
            data[i] = data[i] * (1.0f - volume) + m_last * volume;
        }
    }
}