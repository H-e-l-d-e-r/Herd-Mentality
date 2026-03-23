using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{
    public AudioListener Listener;

    public float SelectedFreq;

    public RadioBroadcastBehaviour[] Broadcasts;

    void Start()
    {
        foreach(RadioBroadcastBehaviour be in Broadcasts)
        {
            be.Play();
        }
    }
}
