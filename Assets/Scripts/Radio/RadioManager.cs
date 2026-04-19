using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadioManager : MonoBehaviour
{
    public float Timer => m_timer;

    [Header("Camera")]
    public Camera Camera;
    public CameraAnchor[] Anchors;

    [Header("Planification UI")]
    public GameObject PlanningCanvas; // Le Canvas � afficher
    public Transform PlanningNote;

    public GameObject PlanningNotePrefab;
    
    public int PlanningAnchorIndex;   // L'index de l'anchor "Planification"
    public bool EnqueueAllSequences; 

    [Header("Components")]
    public VinylRecordPlayer VinylPlayer;

    [Header("Inputs")]
    public InputActionReference SwitchCameraInput;

    [Header("Events")]
    public UnityEvent OnPlayTimeEnd;

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

    private void Start()
    {
        NullComponents.ThrowIfNull(VinylPlayer);
        NullComponents.ThrowIfNull(PlanningCanvas);
        NullComponents.ThrowIfNull(PlanningNote);
        NullComponents.ThrowIfNull(Camera);

        m_switchCameraInput = InputActionReference.Create(SwitchCameraInput);

        k_switchCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
        m_switchCooldown = k_switchCooldown;

        m_timer = GlobalGameSettings.Instance.RadioPlayTime * 60.0f;

        m_playedVinyls = new Queue<VinylObject>();
        m_targetSequences = new Queue<RadioSequenceObject>();
        m_validatedSequences = new List<RadioSequenceObject>();

        // register targets
        if (EnqueueAllSequences)
        {
            foreach (int seq in m_selectedSequences)
            {
                EnqueueSequence(GlobalGameSettings.Instance.Sequences[seq]);
            }   
        }

        if (Anchors.Length > 0)
        {
            // On se focus sur la first cam
            Anchors[0].Focus(Camera);
            UpdatePlanningUI(); // On check si la premi�re cam est la planning
        }
    }

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
        if (m_overwriteRadioTicks && !Dialogue.Instance.IsPlaying)
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
            OnPlayTimeEnd.Invoke();
            return;
        }

        // camera movements
        UpdateCameraSwitch();

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

            GameManager.Instance.Statistics.GlobalAppreciation = GetAppreciation(); 
        }

        // update timer
        m_timer -= Time.deltaTime;
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

        // apres, des qu'un vinyl est joue, on pourra check s'il 
        // permet de completer un objectif. 
        Debug.Log(FindSequences().Length);
    }

    public void EnqueueSequence(RadioSequenceObject seq)
    {
        if (m_targetSequences.Contains(seq))
        {
            return;
        }

        m_targetSequences.Enqueue(seq);
        TMP_Text textInstance = Instantiate(PlanningNotePrefab, PlanningNote).GetComponent<TMP_Text>();
        textInstance.text = seq.ToString();
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

        for(int i = 0; i < groups.Length; i++) 
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
        foreach(RadioSequenceObject seq in GlobalGameSettings.Instance.Sequences)
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

    void UpdatePlanningUI()
    {
        if (PlanningCanvas == null) return;

        // Si l'index actuel est celui de la planification, on affiche le Canvas
        bool isOnPlanning = (m_currentAnchorIndex == PlanningAnchorIndex);

        if (PlanningCanvas.activeSelf != isOnPlanning)
        {
            PlanningCanvas.SetActive(isOnPlanning);
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

            // 
            UpdatePlanningUI();

            // reset du cooldown
            m_switchCooldown = k_switchCooldown;
        }
    }
}