using DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(2500)]
public class RadioManager : MonoBehaviour
{
    public static RadioManager Instance { get; private set; }

    public float Timer => m_timer;

    [Header("Camera")]
    public Camera Camera;
    public CameraAnchor[] Anchors;

    // public GameObject PlanningCanvas; // Le Canvas a afficher
    // public Transform PlanningNote;
    // public GameObject PlanningNotePrefab;
    // public int PlanningAnchorIndex;   // L'index de l'anchor "Planification"
    // public bool EnqueueAllSequences;

    [Header("Components")]
    public TextGroupBehaviour CanvasManager;
    public UILibraryManager PreparationManager;
    public VinylRecordPlayer VinylPlayer;
    public RadioBehaviour RadioBehaviour;

    [Header("Inputs")]
    public InputActionReference SwitchCameraInput;

    [Header("Events")]
    public UnityEvent OnPlayTimeEnd;

    public UnityEvent<RadioSequenceObject> OnSequenceValidated;

    [Header("Parametres Audimat")]
    public float audimatIncreaseRate = 0.5f;
    private float m_currentAudimat = 0f;
    private float m_audimatLogTimer = 10.0f;

    [Header("Textes UI")]
    public TMP_Text AudimatTexte;
    public TMP_Text YoungLetteristsTexte;

    [Header("UI Fin de Niveau")]
    public GameObject EndGameCanvas;
    public TMP_Text EndScreenYoungLetterists;
    public TMP_Text EndScreenSquatRoskoff;
    public TMP_Text EndScreenScilas;
    public TMP_Text EndScreenSequencesCount;

    // constants
    private float k_switchCooldown;

    // inputs
    private InputAction m_switchCameraInput;
    private float m_switchCooldown = 0.0f;

    // camera anchors
    private int m_currentAnchorIndex = 0;

    private float m_timer;
   

    [Header("Debug")]
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

    // le quete que le joueur doit realiser
    [SerializeField]
    [ReadOnly]
    private QuestObject m_questObject;

    // private Dictionary<RadioSequenceObject, int> m_sequencesProgress = new Dictionary<RadioSequenceObject, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        NullComponents.ThrowIfNull(VinylPlayer);
        NullComponents.ThrowIfNull(PreparationManager);
        NullComponents.ThrowIfNull(Camera);

        m_switchCameraInput = InputActionReference.Create(SwitchCameraInput);

        k_switchCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
        m_switchCooldown = k_switchCooldown;

        m_timer = GlobalGameSettings.Instance.RadioPlayTime * 60.0f;

        m_playedVinyls = new Queue<VinylObject>();
        m_targetSequences = new Queue<RadioSequenceObject>();
        m_validatedSequences = new List<RadioSequenceObject>();
        m_questObject = GetRandomQuest(new CollectibleObject.UndergroundGroups());

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

        if (Anchors.Length > 0)
        {
            // On se focus sur la first cam
            Anchors[0].Focus(Camera);
            // UpdatePlanningUI(); // On check si la premiere cam est la planning
        }

        // focus sur l'introduction
        CanvasManager.ActivateGroupsOfText(0);

        // dialogue callback
        Dialogue.Instance.OnDialogueCloseEvent += () =>
        {
            // permet d'aller a la scene de preparation des que le dialogue est fini
            if (CanvasManager.CurrentGroup == 0)
            {
                CanvasManager.ActivateGroupsOfText(1);

                // define quest constraints
                for (int i = 0; i < m_questObject.ConstraintVinyles.Length; i++)
                {
                    if (m_questObject.ConstraintVinyles[i] == null)
                    {
                        break;
                    }

                    PreparationManager.VinylsDropZones[i].IsDroppable = false;
                    PreparationManager.VinylsDropZones[i].SetContent(PreparationManager.FindVinyle(m_questObject.ConstraintVinyles[i]));
                }
            }
        };

        m_questObject.StartDialogue();


        Debug.Log($"<color=magenta>[DEBUG] {m_targetSequences.Count()} sequences chargees depuis le GameManager !</color>");
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
            // On ouvre le endGame
            if (EndGameCanvas != null)
            {
                UI_Manager.Instance.UpdateText(EndScreenYoungLetterists, GameManager.Instance.Statistics.AprYoungLetterists.ToString("0") + " Pts");
                UI_Manager.Instance.UpdateText(EndScreenSquatRoskoff, GameManager.Instance.Statistics.AprSquatRoskoff.ToString("0") + " Pts");
                UI_Manager.Instance.UpdateText(EndScreenScilas, GameManager.Instance.Statistics.AprScilas.ToString("0") + " Pts");

                
                UI_Manager.Instance.UpdateText(EndScreenSequencesCount, m_validatedSequences.Count.ToString());

                UI_Manager.Instance.OpenCanvas(EndGameCanvas);
            }

            OnPlayTimeEnd.Invoke();
            return;
        }

        // camera movements
        UpdateCameraSwitch();

        UpdateAudimatLogic();

        // music increase
        if (VinylPlayer.IsPlaying)
        {
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

        // update timer
        m_timer -= Time.deltaTime;
    }

    void UpdateAudimatLogic()
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
            else Debug.Log($"<color=yellow>[INFO 10S] L'Audimat grimpe ! Gain: +{audimatIncreaseRate * signal:F2}/sec | Audimat Total : {m_currentAudimat:F1}</color>");

            m_audimatLogTimer = 10.0f;
        }

        if (VinylPlayer.IsPlaying && hasRadio && signal > 0.1f)
        {
            float gain = audimatIncreaseRate * signal * Time.deltaTime;
            m_currentAudimat += gain;
            GameManager.Instance.Statistics.GlobalAppreciation = m_currentAudimat;
            UI_Manager.Instance.UpdateText(AudimatTexte, m_currentAudimat.ToString("0") + " Auditeurs");
        }
    }

    void UpdateCameraSwitch()
    {
        // ANCHORS?! Ca fait beaucoup la non? ;)
        // ahaha trop drole ta blague. non
        if (m_switchCooldown > Mathf.Epsilon)
        {
            m_switchCooldown -= Time.deltaTime;
            return;
        }

        if (m_switchCameraInput.ReadValue<float>() > 0.1f)
        {
            // On va vers le prochain anchor 
            m_currentAnchorIndex = (m_currentAnchorIndex + 1) % Anchors.Length;
            Anchors[m_currentAnchorIndex].Focus(Camera);

            // UpdatePlanningUI();

            // reset du cooldown
            m_switchCooldown = k_switchCooldown;
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

        m_playedVinyls.Enqueue(vinyl);

        Debug.Log(FindSequences().Length);

        //ProcessVinylForSequences(vinyl);
    }

    /*public void ProcessVinylForSequences(VinylObject playedVinyl)
    {
        if (playedVinyl == null) return;

        var sequences = m_sequencesProgress.Keys.ToList();
        foreach (var seq in sequences)
        {
            int currentIndex = m_sequencesProgress[seq];
            if (playedVinyl == seq.Blocs[currentIndex])
            {
                m_sequencesProgress[seq]++;
                Debug.Log($"<color=cyan>[PROGRESSION] {seq.name} : {m_sequencesProgress[seq]}/{seq.Blocs.Length}</color>");

                if (m_sequencesProgress[seq] >= seq.Blocs.Length)
                {
                    ProcessSequenceValidation(seq);
                    OnSequenceValidated?.Invoke(seq);
                    m_sequencesProgress[seq] = 0;
                }
            }
            else if (m_sequencesProgress[seq] > 0)
            {
                m_sequencesProgress[seq] = (playedVinyl == seq.Blocs[0]) ? 1 : 0;
            }
        }
    }

    public void ProcessSequenceValidation(RadioSequenceObject seq)
    {
        int diffYL = 0; int diffSR = 0; int diffSC = 0;

        foreach (var vinyl in seq.Blocs)
        {
            diffYL += (vinyl.Like.YoungLetterists ? 2 : 0) - (vinyl.Dislike.YoungLetterists ? 2 : 0);
            diffSR += (vinyl.Like.SquatRoskoff ? 2 : 0) - (vinyl.Dislike.SquatRoskoff ? 2 : 0);
            diffSC += (vinyl.Like.Scilas ? 2 : 0) - (vinyl.Dislike.Scilas ? 2 : 0);
        }

        diffYL += (seq.Like.YoungLetterists ? 10 : 0) - (seq.Dislike.YoungLetterists ? 10 : 0);
        diffSR += (seq.Like.SquatRoskoff ? 10 : 0) - (seq.Dislike.SquatRoskoff ? 10 : 0);
        diffSC += (seq.Like.Scilas ? 10 : 0) - (seq.Dislike.Scilas ? 10 : 0);

        GameManager.Instance.Statistics.AprYoungLetterists += diffYL;
        GameManager.Instance.Statistics.AprSquatRoskoff += diffSR;
        GameManager.Instance.Statistics.AprScilas += diffSC;

        Debug.Log($"<color=green>[VALIDATION] {seq.name} terminee ! Bilan : YL:{diffYL} SR:{diffSR} SC:{diffSC}</color>");
    }*/

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
        List<RadioSequenceObject> list = new List<RadioSequenceObject>();

        foreach (RadioSequenceObject seq in m_targetSequences)
        {
            if (vinyls.ContainsSubSequence(seq.Blocs))
            {
                list.Add(seq);
            }
        }

        m_validatedSequences.Clear();
        m_validatedSequences.AddRange(list);

        return list.ToArray();
    }

    public QuestObject GetRandomQuest(CollectibleObject.UndergroundGroups blacklist)
    {
        // lorsqu'il n'y a pas de quete precedente
        if (m_questObject == null || m_questObject.Next == null)
        {
            // choisit un groupe
            int random = UnityEngine.Random.Range(0, 3);
            return GlobalGameSettings.Instance.Quests[random];
        }

        // si on a deja eu une quete avant et qu'elle a une quete qui suit
        // on prend la quete suivante
        return m_questObject.Next;
    }

    // void UpdatePlanningUI()
    // {
    //     if (PlanningCanvas == null) return;
    // 
    //     // Si l'index actuel est celui de la planification, on affiche le Canvas
    //     bool isOnPlanning = (m_currentAnchorIndex == PlanningAnchorIndex);
    // 
    //     if (PlanningCanvas.activeSelf != isOnPlanning)
    //     {
    //         PlanningCanvas.SetActive(isOnPlanning);
    //     }
    // }

}