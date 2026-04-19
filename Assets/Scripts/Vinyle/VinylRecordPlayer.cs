using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class VinylRecordPlayer : MonoBehaviour
{
    public bool IsPlaying => Broadcast.IsPlaying;
    public VinylObject CurrentVinyl => m_vinyl;

    public RadioBroadcastBehaviour Broadcast;

    [Header("Événements")]
    public UnityEvent<VinylObject> OnPlayMusic;

    private VinylObject m_vinyl;
    private bool m_wasPlaying;

    void Start()
    {
        NullComponents.ThrowIfNull(Broadcast);
    }

    void Update()
    {
        // La musique vient de s'arrêter naturellement
        if (m_wasPlaying && !IsPlaying)
        {
            if (m_vinyl != null && RadioManager.Instance != null)
            {
                // On envoie le vinyle terminé au boss (RadioManager)
                RadioManager.Instance.ProcessVinylForSequences(m_vinyl);
            }
        }
        m_wasPlaying = IsPlaying;
    }

    private void OnTriggerStay(Collider other)
    {
        VinylRecord draggable = other.gameObject.GetComponent<VinylRecord>();

        if (draggable != null && !draggable.IsDragged)
        {
            m_vinyl = draggable.Vinyl;
            draggable.DestroyObject();

            Broadcast.Stop();
            Play();
        }
    }

    void Play()
    {
        Broadcast.Play(m_vinyl.Clip);
        Broadcast.Volume = m_vinyl.Volume;
        m_wasPlaying = true;

        OnPlayMusic.Invoke(m_vinyl);
    }
}