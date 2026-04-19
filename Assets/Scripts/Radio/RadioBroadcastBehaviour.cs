using UnityEngine;
using UnityEngine.Audio;

public class RadioBroadcastBehaviour : MonoBehaviour
{
    public bool IsPlaying
    {
        get => m_source != null ? m_source.isPlaying : false;
    }

    [Header("Audio Parameters")]
    public AudioClip Audio;
    public AudioMixerGroup Group;

    [Range(0f, 1f)]
    public float Volume;
    public bool Loop;

    [HideInInspector]
    public float VolumeMultiplicator = 1.0f;

    [Header("Radio Parameters")]
    public float Freq;
    public float Bandwidth;

    private AudioSource m_source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAudio();
    }

    void UpdateAudio()
    {
        if (m_source != null && m_source.isPlaying)
        {
            m_source.volume = Volume * VolumeMultiplicator;
        }
    }

    public void Play() => Play(Audio);
    public void Play(AudioClip clip)
    {
        if(m_source != null)
        {
            return;
        }

        m_source = gameObject.AddComponent<AudioSource>();
        m_source.clip = clip;
        m_source.volume = Volume;
        m_source.loop = Loop;
        m_source.outputAudioMixerGroup = Group;
        
        m_source.Play();
    }

    public void Stop()
    {
        if(m_source == null)
        {
            return;
        }

        m_source.Stop();
        Destroy(m_source);
        m_source = null;
    } 
}
