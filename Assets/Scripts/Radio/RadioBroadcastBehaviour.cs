using System;
using UnityEngine;
using UnityEngine.Audio;

public class RadioBroadcastBehaviour : MonoBehaviour
{
    public bool IsPlaying
    {
        get => m_source != null ? m_source.isPlaying : false;
    }

    public bool IsListenable
    {
        get => VolumeMultiplicator * Volume > c_listenTreshold;
    }

    public double LoopOffset => m_loopOffset;

    public bool HasNext
    {
        get => m_current != null ? m_current.Next != null : false;
    }

    public SequenceObject Current => m_current != null ? m_current.Object : null;
    public BroadcastMessageObject CurrentMessage => m_current;

    [Header("Audio Parameters")]
    public AudioMixerGroup Group;

    [Range(0f, 1f)]
    public float Volume;
    public bool Loop;
    public bool IsHerd;

    public BroadcastMask Mask;

    [Header("Messages")]
    public BroadcastMessageObject[] Messages;

    [HideInInspector]
    public float VolumeMultiplicator = 1.0f;

    private const float c_listenTreshold = 0.01f;

    private AudioSource m_source;
    private BroadcastMessageObject m_current;
    private double m_loopOffset = 0;
    
    private void Start()
    {
        // 
        if (Messages.Length > 0)
        {
            m_current = Messages[0];
            m_current.Next = null;
            m_current.Prev = null;

            for (int index = 1; index < Messages.Length; index++)
            {
                m_current.Next = Messages[index];
                m_current.Next.Prev = m_current;

                m_current = m_current.Next;
            }

            m_current = null;
        }
    }

    public void RadioUpdate()
    {
        if (m_source != null && m_source.isPlaying)
        {
            m_source.volume = Mathf.Clamp01(Volume * VolumeMultiplicator);
        }

        if (m_current != null)
        {
            double now = RadioManager.Instance.GameClock.Now - m_loopOffset;

            //  toujours attendre la fin du clip actuel, peu importe le StartTime du suivant
            if (now > m_current.EndTime)
            {
                if (Loop && m_current.Next == null)
                {
                    m_loopOffset = RadioManager.Instance.GameClock.Now;
                    m_current = Messages[0];
                }
                else
                {
                    m_current = m_current.Next;
                }

                Stop();
                Play(m_current);
                return;
            }
        }
        else if (Messages.Length > 0)
        {
            m_loopOffset = RadioManager.Instance.GameClock.Now;
            m_current = Messages[0];
            Play(m_current);
        }
    }

    public void Play()
    {

    }

    public void Play(BroadcastMessageObject @object)
    {
        if (m_source != null || @object == null) return;
 
        @object.Time = (float)(RadioManager.Instance.GameClock.Now - m_loopOffset);
        Play(@object.Object.Clip);
    }

    public void Play(AudioClip clip)
    {
        if (m_source != null)
        {
            return;
        }

        m_source = gameObject.AddComponent<AudioSource>();
        m_source.clip = clip;
        m_source.volume = Volume;
        m_source.loop = false;
        m_source.outputAudioMixerGroup = Group;

        m_source.Play();
    }

    public void Stop()
    {
        if (m_source == null)
        {
            return;
        }

        m_source.Stop();
        Destroy(m_source);
        m_source = null;
    }


    public double GetTimeToNext()
    {
        if (!HasNext)
        {
            return m_current.EndTime - RadioManager.Instance.GameClock.Now;
        }

        return m_current.EndTime - RadioManager.Instance.GameClock.Now;
    }

    [Serializable]
    public class BroadcastMessageObject
    {
        public float Time;
        public SequenceObject Object;

        // linked list
        public BroadcastMessageObject Prev;
        public BroadcastMessageObject Next;

        public float StartTime
        {
            get
            {
                float offset = Time;
                BroadcastMessageObject self = this;

                while (self.Prev != null)
                {
                    offset += self.Prev.StartTime;
                    self = self.Prev;
                }

                return offset;
            }
        }

        public float EndTime => StartTime + Object.Duration;

        private float m_initialOffset;

        public BroadcastMessageObject()
        {
            m_initialOffset = Time;
            
        }
    }
}