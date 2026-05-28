using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class VinylRecordPlayer : MonoBehaviour
{
    public bool IsPlaying { get; private set; }
    public VinylObject CurrentVinyl => m_vinyl;

    public RadioKnobComponent KnobComponent;
    public AntennaTunerBehaviour Slider;
    public VinylStorage Storage;
    public RadioBroadcastBehaviour Broadcast;

    [Header("Visual")]
    public VinylRecord Record;
    public float RecordRotation;

    [Header("Events")]
    public UnityEvent<SequenceObject> OnSequenceValidated; // Quand une s�quence est finie
    public UnityEvent<VinylObject> OnPlayMusic;
    public UnityEvent<VinylObject> OnStopMusic;

    private VinylObject m_vinyl;
    private float m_musicRemaining;

    void Start()
    {
        NullComponents.ThrowIfNull(Broadcast);
        NullComponents.ThrowIfNull(KnobComponent);
        
        Broadcast.Mask.Frequence = KnobComponent.Value;
        Broadcast.Mask.Orientation = Slider.Value;

        KnobComponent.OnValueChange.AddListener((float freq) =>
        {
            Broadcast.Mask.Frequence = freq;
        });

        Slider.OnValueChange.AddListener((float orientation) =>
        {
            Debug.Log(orientation);
            Broadcast.Mask.Orientation = orientation; 
        });

        // au cas ou
        Stop();
    }

    void Update()
    {
        if (IsPlaying)
        {
            // when the music will end
            if(m_musicRemaining < Mathf.Epsilon)
            {
                Stop();
            }
            else
            {
                // decrement cooldown
                m_musicRemaining -= Time.deltaTime;
            }

            float increment = RecordRotation * Time.deltaTime;
            Record.transform.eulerAngles = new Vector3(
                Record.transform.eulerAngles.x,
                Record.transform.eulerAngles.y + increment,
                Record.transform.eulerAngles.z
            );
        }
    }

    private void OnTriggerStay(Collider other)
    {
        VinylRecord draggable = other.gameObject.GetComponent<VinylRecord>();
        
        // do not destroy our friend :/
        if(draggable == Record)
        {
            return;
        }

        if (draggable != null && !draggable.IsDragged)
        {
            Stop();

            m_vinyl = draggable.Vinyl;
            draggable.Destroy();

            Play();
        }
    }

    void Play()
    {
        Broadcast.Play(m_vinyl.Clip);
        Broadcast.Volume = m_vinyl.Volume;

        OnPlayMusic.Invoke(m_vinyl);

        Record.gameObject.SetActive(true);
        Record.Vinyl = m_vinyl;

        m_musicRemaining = m_vinyl.Clip.length;
        IsPlaying = true;

        RadioManager.Instance.EnqueueVinyl(m_vinyl, Broadcast.Mask.Frequence, Broadcast.Mask.Orientation);
    }

    void Stop()
    {
        if(m_vinyl == null)
        {
            return;
        }

        Broadcast.Stop();
        OnStopMusic.Invoke(m_vinyl);
        
        Record.gameObject.SetActive(false);

        Storage.Add(m_vinyl);

        IsPlaying = false;
    }
}