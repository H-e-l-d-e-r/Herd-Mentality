using DialogueSystem;

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class manage :
/// - link radio behaviour to the game systems
/// 
/// </summary>
[DefaultExecutionOrder(2500)]
public class RadioManager : MonoBehaviour
{
    public static RadioManager Instance { get; private set; }

    [Header("Components")]
    public VinylRecordPlayer VinylPlayer;
    public RadioBehaviour RadioBehaviour;
    public UIRadio UI;

    [Header("Audio")]
    public float BackgroundMusicVolume = 0.5f;
    public AudioSource BackgroundMusicSource;

    [Header("Events")]
    public UnityEvent OnPlayTimeEnd;
    public UnityEvent<SequenceObject> OnSequenceValidated;

    [ReadOnly]
    public Clock GameClock;

    private float m_currentAudimat = 0f;
    private float m_audimatLogTimer = 10.0f;

    [Header("Debug")]

    [SerializeField]
    private int[] m_selectedSequences;

    [SerializeField]
    private bool m_overwriteRadioTicks = false;

    // les vinyles qui ont deja ete joues
    private Queue<QuestObject> m_playedVinyls;

    // les sequences que le joueur doit jouer
    private Queue<SequenceObject> m_discoverd;

    // les sequences qui ont ete valides
    private List<QuestObject> m_validatedQuests;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        NullComponents.ThrowIfNull(VinylPlayer);
        
        GameClock = new Clock();

        m_playedVinyls = new Queue<QuestObject>();

        m_discoverd = new Queue<SequenceObject>();
        m_validatedQuests = new List<QuestObject>();

        // register targets
        if (m_selectedSequences.Length > 0)
        {
            foreach (int seq in m_selectedSequences)
            {
                EnqueueSequence(GlobalGameSettings.Instance.Sequences[seq]);
            }
        }
        else
        {
            foreach (SequenceObject seq in GlobalGameSettings.Instance.Sequences)
            {
                EnqueueSequence(seq);
            }
        }


        // On-Off callback
        RadioBehaviour.OnRadioEnable.AddListener(() => { BackgroundMusicSource.volume = 0.0f; });
        RadioBehaviour.OnRadioDisable.AddListener(() => { BackgroundMusicSource.volume = BackgroundMusicVolume; });

        Debug.Log($"<color=magenta>[DEBUG] {m_discoverd.Count()} sequences chargees depuis le GameManager !</color>");
    }

    void Update()
    {
        bool isDialoguePlaying = (Dialogue.Instance != null) && Dialogue.Instance.IsPlaying;

        if (m_overwriteRadioTicks && !isDialoguePlaying)
        {
            RadioUpdate();
        }

    }

    public void RadioUpdate()
    {
        // when the play time is over
        // we stop the game
        //if (m_timer < Mathf.Epsilon)
        //{
        //    // reset the timer
        //    m_timer = GlobalGameSettings.Instance.RadioPlayTime * 60.0f;
        //
        //    UI.ShowEndScreen();
        //    if (QuestManager.IsComplete)
        //    {
        //        UI.ShowQuestComplete();
        //    }
        //
        //    OnPlayTimeEnd.Invoke();
        //    
        //    return;
        //}

        GameClock.Now += Time.deltaTime;

        UpdateStats();

        // update timer
        //m_timer -= Time.deltaTime;
    }

    void UpdateStats()
    {
        m_audimatLogTimer -= Time.deltaTime;

        bool hasRadio = RadioBehaviour != null;
        float signal = hasRadio ? RadioBehaviour.FrequencyQuality / 2f : 0f;

        if (m_audimatLogTimer <= 0)
        {
            // feedbacks
            if (!VinylPlayer.IsPlaying) Debug.Log("<color=red>[INFO 10S] Audimat en pause. La platine ne joue rien.</color>");
            else if (!hasRadio) Debug.Log("<color=red>[INFO 10S] Audimat en pause. RadioBehaviour non assigne.</color>");
            else if (signal <= 0.1f) Debug.Log($"<color=orange>[INFO 10S] Audimat en pause : Signal trop faible ({signal:P0}). Reglez la molette !</color>");
            else Debug.Log($"<color=yellow>[INFO 10S] L'Audimat grimpe ! Gain: +{GlobalGameSettings.Instance.AppreciationIncrease * signal:F2}/sec | Audimat Total : {m_currentAudimat:F1}</color>");

            m_audimatLogTimer = 10.0f;
        }

        if (VinylPlayer.IsPlaying && hasRadio && signal > 0.1f)
        {
            // audimat
            float gain = GlobalGameSettings.Instance.AppreciationIncrease * signal * Time.deltaTime;
            m_currentAudimat += gain;
            GameManager.Instance.Statistics.GlobalAudimat = (uint)m_currentAudimat;
        
            // group appreciations
            float increase = GlobalGameSettings.Instance.AppreciationIncreaseMusic * Time.deltaTime;
            float decrease = GlobalGameSettings.Instance.AppreciationDecreaseMusic * Time.deltaTime;

            // les elements apprecies
            GameManager.Instance.Statistics.AprYoungLetterists += (Convert.ToSingle(VinylPlayer.CurrentVinyl.Like.YoungLetterists) * increase);
            GameManager.Instance.Statistics.AprSquatRoskoff += (Convert.ToSingle(VinylPlayer.CurrentVinyl.Like.SquatRoskoff) * increase);
            GameManager.Instance.Statistics.AprScilas += (Convert.ToSingle(VinylPlayer.CurrentVinyl.Like.Scilas) * increase);

            // les elements non-apprecies
            GameManager.Instance.Statistics.AprYoungLetterists -= (Convert.ToSingle(VinylPlayer.CurrentVinyl.Dislike.YoungLetterists) * decrease);
            GameManager.Instance.Statistics.AprSquatRoskoff -= (Convert.ToSingle(VinylPlayer.CurrentVinyl.Dislike.SquatRoskoff) * decrease);
            GameManager.Instance.Statistics.AprScilas -= (Convert.ToSingle(VinylPlayer.CurrentVinyl.Dislike.Scilas) * decrease);
        }
    }

    public void AddAudimatBonus(float bonus)
    {
        m_currentAudimat += bonus;
        GameManager.Instance.Statistics.GlobalAppreciation = m_currentAudimat;
        Debug.Log($"<color=green>[HIJACK] Bonus recu : +{bonus} Audimat ! Total : {m_currentAudimat:F1}</color>");
    }

    /// <summary>
    /// Vinyl enqueue wrapper.
    /// </summary>
    /// <param name="vinyl"></param>
    public void EnqueueVinyl(VinylObject vinyl, float frequence, float orientation)
    {
        if (m_playedVinyls == null)
        {
            return;
        }

        m_playedVinyls.Enqueue(new QuestObject(vinyl, frequence, orientation));

        int maxSize = GlobalGameSettings.Instance.QuestObjects.Length;
        while (m_playedVinyls.Count > maxSize)
            m_playedVinyls.Dequeue();

        Debug.Log($"{vinyl.Name} {frequence} {orientation}");

        if (FindSequences()) 
        {
            Debug.Log("fin?");
            GameManager.Instance.End();
        }
    }

    public void EnqueueSequence(SequenceObject seq)
    {
        if (m_discoverd.Contains(seq))
        {
            return;
        }

        // register
        m_discoverd.Enqueue(seq);
        GameManager.Instance.AddCollectible(seq);
        
        // callback
        UI.Notify.Notify($"Nouvelle entrée!", 2f);
    }

    /// <summary>
    /// Vinyl clearing wrapper.
    /// </summary>
    public void ClearVinylQueue()
    {
        if (m_playedVinyls == null)
        {
            return;
        }

        m_playedVinyls.Clear();
    }

    /// <summary>
    /// Return current appreciation
    /// </summary>
    /// <returns></returns>
    public float GetAppreciation()
    {
        float eq = 0;
        float[] groups = new float[] {
            GameManager.Instance.Statistics.AprYoungLetterists,
            GameManager.Instance.Statistics.AprSquatRoskoff,
            GameManager.Instance.Statistics.AprScilas
        };

        for (int i = 0; i < groups.Length; i++)
        {
            float diff = Mathf.Abs(groups[i] - groups[(i + 1) % (groups.Length - 1)]);
            eq = Mathf.Max(eq, diff);
        }

        return eq / 200.0f * 100.0f;
    }


    public bool FindSequences() => FindSequences(m_playedVinyls.ToArray());

    public bool FindSequences(QuestObject[] vinyls)
    {
        QuestObject[] targets = GlobalGameSettings.Instance.QuestObjects;
        if (vinyls.Length != targets.Length) return false;

        for (int i = 0; i < targets.Length; i++)
        {
            if (!vinyls[i].Equals(targets[i])) return false;
        }

        return true;
    }

    [Serializable]
    public struct Clock
    {
        /// <summary>
        /// Seconds from the start of the radio
        /// </summary>
        public double Now;
    }
}