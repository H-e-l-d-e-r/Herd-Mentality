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

    public float Timer => m_timer;

    [Header("Components")]
    public VinylRecordPlayer VinylPlayer;
    public RadioBehaviour RadioBehaviour;
    public QuestManager QuestManager;
    public UIRadio UI;

    [Header("Audio")]
    public float BackgroundMusicVolume = 0.5f;
    public AudioSource BackgroundMusicSource;

    [Header("Events")]
    public UnityEvent OnPlayTimeEnd;
    public UnityEvent<RadioSequenceObject> OnSequenceValidated;

    private float m_timer;

    private float m_currentAudimat = 0f;
    private float m_audimatLogTimer = 10.0f;

    [Header("Debug")]
    public bool EnableQuests = true;

    [SerializeField]
    private int[] m_selectedSequences;

    [SerializeField]
    private bool m_overwriteRadioTicks = false;

    // les vinyles qui ont deja ete joues
    private Queue<VinylObject> m_playedVinyls;

    // les sequences que le joueur doit jouer
    private Queue<RadioSequenceObject> m_targetSequences;

    // les sequences qui ont ete valides
    private List<RadioSequenceObject> m_validatedSequences;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        NullComponents.ThrowIfNull(VinylPlayer);

        m_timer = GlobalGameSettings.Instance.RadioPlayTime * 60.0f;

        m_playedVinyls = new Queue<VinylObject>();
        m_targetSequences = new Queue<RadioSequenceObject>();
        m_validatedSequences = new List<RadioSequenceObject>();

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
            foreach (RadioSequenceObject seq in GlobalGameSettings.Instance.Sequences)
            {
                EnqueueSequence(seq);
            }
        }

        // focus sur l'introduction
        //CanvasManager.ActivateGroupsOfText(0);

        // dialogue callback
        //Dialogue.Instance.OnDialogueCloseEvent += () =>
        //{
        //    // permet d'aller a la scene de preparation des que le dialogue est fini
        //    if (!m_disableQuests && CanvasManager.CurrentGroup == 0)
        //    {
        //        if (m_timer <= Mathf.Epsilon)
        //        {
        //            NextDay();
        //            return;
        //        }
//
        //        CanvasManager.ActivateGroupsOfText(1);
//
        //        // define quest constraints
        //        for (int i = 0; i < m_questObject.ConstraintVinyles.Length; i++)
        //        {
        //            if (m_questObject.ConstraintVinyles[i] == null)
        //            {
        //                break;
        //            }
//
        //            PreparationManager.VinylsDropZones[i].IsInteractible = false;
        //            PreparationManager.VinylsDropZones[i].AttachGameObject(PreparationManager.FindVinyle(m_questObject.ConstraintVinyles[i]));
        //        }
        //    }
        //};

        // start dialogue quest
        // TODO move this to a separate component
        // if (!m_disableQuests)
        // {
        //     m_questObject.StartDialogue();
        // }

        // On-Off callback
        RadioBehaviour.OnRadioEnable.AddListener(() => { BackgroundMusicSource.volume = 0.0f; });
        RadioBehaviour.OnRadioDisable.AddListener(() => { BackgroundMusicSource.volume = BackgroundMusicVolume; });

        Debug.Log($"<color=magenta>[DEBUG] {m_targetSequences.Count()} sequences chargees depuis le GameManager !</color>");

        if (EnableQuests)
        {
            UI.ShowQuestDialogue();
            QuestManager.StartCurrentQuest();
        }
        else
        {
            UI.ShowPreparationScreen();
        }
    }

    /*public void RefreshAvailableSequences()
    {
        m_targetSequences.Clear();

        RadioSequenceObject[] allSequences = GameManager.Instance.UnlockedSequences;

        if (allSequences != null)
        {
            foreach (RadioSequenceObject seq in allSequences)
            {
                EnqueueSequence(seq);
            }
        }

        Debug.Log($"<color=magenta>[DEBUG] {m_targetSequences.Count()} sequences chargees depuis le GameManager !</color>");
    }*/

    void OnEnable()
    {
        // reset
        ClearVinylQueue();
    }

    void OnDisable()
    {
        // reset
        ClearVinylQueue();
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
        if (m_timer < Mathf.Epsilon)
        {
            // reset the timer
            m_timer = GlobalGameSettings.Instance.RadioPlayTime * 60.0f;
            UI.ShowEndScreen();
            
            OnPlayTimeEnd.Invoke();
            
            return;
        }

        UpdateStats();

        // update timer
        m_timer -= Time.deltaTime;
    }

    void UpdateStats()
    {
        m_audimatLogTimer -= Time.deltaTime;

        bool hasRadio = RadioBehaviour != null;
        float signal = hasRadio ? (RadioBehaviour.AntennaSignalQuality + RadioBehaviour.FrequencyQuality) / 2f : 0f;

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
    public void EnqueueVinyl(VinylObject vinyl)
    {
        if (m_playedVinyls == null)
        {
            return;
        }

        int countBefore = m_validatedSequences.Count;


        m_playedVinyls.Enqueue(vinyl);


        FindSequences();


        if (m_validatedSequences.Count > countBefore)
        {
            RadioSequenceObject lastSeq = m_validatedSequences[m_validatedSequences.Count - 1];

            //if (QuestCompletedCanvas != null)
            //{
            //    UIToolkit.SetText(QuestCompletedText, lastSeq.name + " Complete !");
//
//
            //    StartCoroutine(ShowAndFadeQuestPopup());
            //}
        }
    }

    /// <summary>
    /// Reset everything to start a new day
    /// </summary>
    //public void NextDay()
    //{
//
    //    if (m_questObject.EndTable)
    //    {
    //        CanvasManager.ActivateGroupsOfText(0);
    //        m_questObject.StartEndDialogue();
    //        m_questObject = null; 
//
    //        return;
    //    }
//
    //    m_timer = GlobalGameSettings.Instance.RadioPlayTime * 60.0f;
//
    //    m_playedVinyls = new Queue<VinylObject>();
    //    m_targetSequences = new Queue<RadioSequenceObject>();
    //    m_validatedSequences = new List<RadioSequenceObject>();
    //    m_questObject = GetRandomQuest(new CollectibleObject.UndergroundGroups());
//
    //    CanvasManager.ActivateGroupsOfText(0);
    //    m_questObject.StartDialogue();
    //}

    public void EnqueueSequence(RadioSequenceObject seq)
    {
        m_targetSequences.Enqueue(seq);
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


    public RadioSequenceObject[] FindSequences() => FindSequences(m_playedVinyls.ToArray());

    public RadioSequenceObject[] FindSequences(VinylObject[] vinyls)
    {
        // avoid concurrency race 
        if(m_targetSequences == null || m_validatedSequences == null)
        {
            return new RadioSequenceObject[0];
        }

        List<RadioSequenceObject> list = new List<RadioSequenceObject>();

        foreach (RadioSequenceObject seq in m_targetSequences)
        {
            if (vinyls.ContainsSubSequence(seq.Blocs))
            {
                // for sequences that contains for than one element
                // theses are always added
                if(seq.Blocs.Length > 1)
                {
                    list.Add(seq);                
                }
                // when the sequence only contains one vinyl
                // we only add it if it's the current quest 
                else if (seq.Blocs.Length == 1 && EnableQuests)
                {
                    if (QuestManager.Quest.ConstraintVinyles.ContainsSubSequence(seq.Blocs))
                    {
                        list.Add(seq);                                    
                    }
                }
            }
        }

        m_validatedSequences.Clear();
        m_validatedSequences.AddRange(list);

        return list.ToArray();
    }

    public void NextQuest()
    {
        if (EnableQuests)
        {
            QuestManager.FinishCurrentQuest();
            UI.ShowQuestDialogue();
            QuestManager.StartCurrentQuest();
        }
        else
        {
            UI.ShowPreparationScreen();
        }
    }

    //-----TEMPORAIRE LE TEMPS DE FAIRE UNE ANIMATION------
   
    /*private IEnumerator ShowAndFadeQuestPopup()
    {
        
        UIToolkit.OpenCanvas(QuestCompletedCanvas);

        
        CanvasGroup canvasGroup = QuestCompletedCanvas.GetComponent<CanvasGroup>();

       
        if (canvasGroup == null)
        {
            canvasGroup = QuestCompletedCanvas.gameObject.AddComponent<CanvasGroup>();
        }

       
        canvasGroup.alpha = 1f;

       
        yield return new WaitForSeconds(4f);

        
        float fadeDuration = 2f;
        float timeElapsed = 0f;

       
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime; 

            
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);

            
            yield return null;
        }

        
        UIToolkit.CloseCanvas(QuestCompletedCanvas);
    }*/

}