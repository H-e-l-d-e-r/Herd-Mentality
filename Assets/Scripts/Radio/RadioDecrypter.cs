using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadioDecrypter : MonoBehaviour
{
    public UIGroupBehaviour Groups;

    [Header("Components")]
    public Button RecordButton;
    public Button TranslateButton;
    public Slider ProgressBar;
    public event Action<DecryptionModes> OnDecodeSuccess;
    public ScrollRect ScrollView;
    public UISelectNumber[] NumberSelectors;
    public AudioSource RecordingAudio;

    [Header("Behaviours")]
    public UISelectDecryptionMode DecryptionModeSelector;
    public DialogueActor DialogueActor;
    public DialogueTypewritter Typewritter;

    [Header("Buttons")]
    public Color DefaultColor;
    public Color ActionnableColor;

    private SequenceObject m_recordObject;
    private bool m_isRecordRunning;
    private bool m_isTranslateRunning;
    private bool m_isTranslateRunningTwice;

    private float m_recordDuration;
    private Coroutine m_recordRoutine;

    private static string k_RecordingFailed = "Enregistrement echoue";
    private static string k_SignalNotFound = "Signal perdu";
    private static string k_RecordingNotFound = "Aucun enregistrement...";
    private static string k_TraductionNotFound = "Aucune traduction, essayez un autre mode.";
    private static string k_Recording = "Enregistrement...";
    private static string k_RecordingDone = "Enregistrement fini";
    private static string k_WaitingForInput = "Attente d'une action...";

    void Start()
    {
        NullComponents.ThrowIfNull(RecordButton);
        NullComponents.ThrowIfNull(TranslateButton);
        NullComponents.ThrowIfNull(DecryptionModeSelector);
        NullComponents.ThrowIfNull(Typewritter);

        if(NumberSelectors.Length == 0)
        {
            Debug.LogError("Number Selector is empty.");
        }

        RecordButton.onClick.AddListener(StartRecording);
        TranslateButton.onClick.AddListener(StartTranslating);
        DecryptionModeSelector.OnChange.AddListener((_) => { InteruptTranslating(); StartTranslating(); });

        Groups.CurrentGroup = 0; 

        InteruptRecoding();
    }

    void Update()
    {
        // update state
        ProgressBar.gameObject.SetActive(m_isRecordRunning);
        ScrollView.normalizedPosition = new Vector2(0, 0);
    }

    public void StartRecording() => m_recordRoutine = StartCoroutine(Record());
    public void StartTranslating() => StartCoroutine(Translate());

    public void InteruptRecoding()
    {
        RecordingAudio.volume = 0.0f;

        if (!m_isRecordRunning)
        {
            return;
        }

        StopCoroutine(m_recordRoutine);

        Typewritter.Clear();
        Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_RecordingFailed));

        m_isRecordRunning = false;
        Groups.CurrentGroup = 0;
    }

    public void InteruptTranslating()
    {
        if (!m_isTranslateRunning)
        {
            return;
        }

        m_isTranslateRunningTwice = false;
        Groups.CurrentGroup = 0;
    }

    IEnumerator Record()
    {
        if (!RadioManager.Instance.RadioBehaviour.IsOn)
        {
            yield break;
        }

        if (m_isRecordRunning)
        {
            yield break;
        }

        Typewritter.Show();

        RadioBroadcastBehaviour behaviour = RadioManager.Instance.RadioBehaviour.GetCurrentBroadcast();

        if (behaviour == null || behaviour.Current == null)
        {
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_SignalNotFound));

            m_recordObject = null;
            m_isRecordRunning = false;
            yield break;
        }

        // FIX : on capture le BroadcastMessageObject au moment du record
        // pour ne pas dépendre de m_current qui avance dans RadioUpdate()
        RadioBroadcastBehaviour.BroadcastMessageObject capturedMessage = behaviour.CurrentMessage;

        if (capturedMessage == null)
        {
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_SignalNotFound));

            m_recordObject = null;
            m_isRecordRunning = false;
            yield break;
        }

        m_isRecordRunning = true;

        // FIX : on calcule la durée depuis le EndTime capturé, pas depuis GetTimeToNext()
        double capturedEndTime = behaviour.LoopOffset + capturedMessage.EndTime; // temps absolu !
        double startRemaining = capturedEndTime - RadioManager.Instance.GameClock.Now;
        m_recordDuration = (float)startRemaining;

        Typewritter.Clear();
        Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_Recording));

        m_recordObject = capturedMessage.Object;
        RecordingAudio.volume = 1.0f;

        // FIX : on attend le EndTime du message capturé, indépendamment de m_current
        while (capturedEndTime - RadioManager.Instance.GameClock.Now > Mathf.Epsilon)
        {
            float remaining = (float)(capturedEndTime - RadioManager.Instance.GameClock.Now);
            ProgressBar.value = remaining / m_recordDuration;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Typewritter.Clear();
        Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_RecordingDone));

        RecordingAudio.volume = 0.0f;

        m_isRecordRunning = false;
    }

    /// <summary>
    /// Translation coroutine task
    /// This is the main tranlation behaviour
    /// </summary>
    /// <returns></returns>
    IEnumerator Translate()
    {
        // if it's off
        if (!RadioManager.Instance.RadioBehaviour.IsOn)
        {
            yield break;
        }

        // if nothing is recorded
        if (m_recordObject == null)
        {
            Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_RecordingNotFound));
            m_isTranslateRunning = false;
            m_isTranslateRunningTwice = false;
            yield break;
        }

        // if it's while a something is being recorded
        if (m_isRecordRunning)
        {
            m_isTranslateRunning = false;
            m_isTranslateRunningTwice = false;
            yield break;
        }

        Typewritter.Show();

        m_isTranslateRunningTwice = m_isTranslateRunning;
        m_isTranslateRunning = true;

        // when start recording
        DecryptionModes mode = DecryptionModeSelector.Current;
        IEnumerable<DecryptionsResults> results = m_recordObject.Translations.Where(t => t.Mode == mode);

        // if there is a translation
        if (results.Count() > 0)
        {
            DecryptionsResults result = results.First();
            Typewritter.Clear();

            bool modeValid = m_recordObject.Valid == DecryptionModes.FromAll;

            if (!result.DoesAskModifier)
            {
                // when there is nothing to ask
                Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, result.GetContent()));
                modeValid |= mode == m_recordObject.Valid;
            }
            else
            {
                Typewritter.Clear();
                Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_WaitingForInput));

                // select the second group
                Groups.CurrentGroup = 1;

                // wait for a second input
                while (!m_isTranslateRunningTwice)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                int modifier = (int)NumberSelectors[0].Current;
                for (int i = 1; i < NumberSelectors.Length; i++)
                {
                    modifier += (int)((float)NumberSelectors[i].Current * Mathf.Pow(10, i));
                }

                Typewritter.Clear();
                Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, result.GetContent(modifier)));            

                Groups.CurrentGroup = 0;

                modeValid |= mode == m_recordObject.Valid && m_recordObject.ValidModifier == modifier;
            }

            m_isTranslateRunning = false;
            m_isTranslateRunningTwice = false;

            if (modeValid)
            {
                while (!Typewritter.IsFinish)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                OnDecodeSuccess?.Invoke(mode);
                RadioManager.Instance.EnqueueSequence(m_recordObject);
            }

            yield break;
        }
        else
        {
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", DialogueActor, k_TraductionNotFound));
            
            m_isTranslateRunning = false;
            m_isTranslateRunningTwice = false;
            yield break;
        }
    }
}