using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class VinylRecordPlayer : MonoBehaviour
{
    public bool IsPlaying => Broadcast.IsPlaying;
    public VinylObject CurrentVinyl => m_vinyl;

    public RadioBroadcastBehaviour Broadcast;

    [Header("Événements")]
    public UnityEvent<RadioSequenceObject> OnSequenceValidated; // Quand une séquence est finie
    public UnityEvent<VinylObject> OnPlayMusic;

    private VinylObject m_vinyl;
    private bool m_wasPlaying;

    void Start()
    {
        NullComponents.ThrowIfNull(Broadcast);
    }

    void Update()
    {
        // Détection de fin de morceau (Quand ça jouait, mais que ça ne joue plus)
        // if (m_wasPlaying && !IsPlaying)
        // {
        //     CheckSequencesProgress();
        // }
        // m_wasPlaying = IsPlaying;
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

    // --- LE VÉRIFICATEUR DE SÉQUENCES ---
    /*private void CheckSequencesProgress()
    {
        if (m_vinyl == null) return;

        var sequences = m_sequencesProgress.Keys.ToList();

        foreach (var seq in sequences)
        {
            int currentIndex = m_sequencesProgress[seq];

            // Est-ce le bon vinyle pour cette étape ?
            if (m_vinyl == seq.Blocs[currentIndex])
            {
                m_sequencesProgress[seq]++;
                Debug.Log($" Progression : {seq.name} ({m_sequencesProgress[seq]}/{seq.Blocs.Length})");

                // Séquence complétée ?
                if (m_sequencesProgress[seq] >= seq.Blocs.Length)
                {
                    Debug.Log($"<color=green> SÉQUENCE VALIDÉE : {seq.name} !</color>");
                    OnSequenceValidated?.Invoke(seq);
                    m_sequencesProgress[seq] = 0; // Reset pour le lendemain
                }
            }
            else
            {
                // Mauvais vinyle : on reset le compteur pour cette séquence
                if (m_sequencesProgress[seq] > 0)
                {
                    Debug.Log($"<color=orange> Séquence {seq.name} brisée. Retour à zéro.</color>");
                    m_sequencesProgress[seq] = 0;

                    // Si c'est le 1er vinyle de la séquence, on le compte quand même
                    if (m_vinyl == seq.Blocs[0]) m_sequencesProgress[seq] = 1;
                }
            }
        }
    }*/
}