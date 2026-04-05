using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent (typeof(Rigidbody))]
public class VinylRecordPlayer : MonoBehaviour
{
    public RadioBroadcastBehaviour Broadcast;

    private RadioVinyl m_vinyl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NullComponents.ThrowIfNull(Broadcast);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        VinylRecord draggable = other.gameObject.GetComponent<VinylRecord>();

        if(draggable != null && !draggable.IsDragged)
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
    }
}
