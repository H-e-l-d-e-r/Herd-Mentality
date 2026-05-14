using System;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class RadioWaveform : MonoBehaviour
{
    public Material Material;
    public Renderer Renderer;

    public FFTWindow SpectrumMode;

    private float[] m_buffer;
    private ComputeBuffer m_compute;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NullComponents.ThrowIfNull(Renderer);

        m_buffer = new float[256];
        m_compute = new ComputeBuffer(256, sizeof(float));

        if (Material)
        {
            Renderer.material = Material;
        }
    }

    public void UpdateWaveSignal()
    {
        AudioListener.GetOutputData(m_buffer, 0);

        m_compute.SetData(m_buffer);
        Material.SetBuffer("_Buffer", m_compute);
    }

    void OnDestroy()
    {
        m_compute.Release();
    }
}
