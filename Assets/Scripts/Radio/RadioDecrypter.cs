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
    public UISelectNumber[] NumberSelectors;

    [Header("Behaviours")]
    public UISelectDecryptionMode DecryptionModeSelector;
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
        DecryptionModeSelector.OnChange.AddListener((_) => StartTranslating());

        Groups.CurrentGroup = 0; 
    }

    void Update()
    {
        // update state
        ProgressBar.gameObject.SetActive(m_isRecordRunning);
    }

    public void StartRecording() => m_recordRoutine = StartCoroutine(Record());
    public void StartTranslating() => StartCoroutine(Translate());

    public void InteruptRecoding()
    {
        if (!m_isRecordRunning)
        {
            return;
        }

        StopCoroutine(m_recordRoutine);

        Typewritter.Clear();
        Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Recording faild!"));


        m_isRecordRunning = false;
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

        // when start recording
        RadioBroadcastBehaviour behaviour = RadioManager.Instance.RadioBehaviour.GetCurrentBroadcast();

        // if something worked
        if (behaviour == null || behaviour.Current == null)
        {
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Signal not found!"));

            m_recordObject = null;
            m_isRecordRunning = false;
            yield break;
        }

        double remainingTime = behaviour.GetTimeToNext();

        m_isRecordRunning = true;
        m_recordDuration = (float)remainingTime;

        Typewritter.Clear();
        Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Recording..."));

        m_recordObject = behaviour.Current;

        while (behaviour.GetTimeToNext() > Mathf.Epsilon)
        {
            ProgressBar.value = (float)behaviour.GetTimeToNext() / m_recordDuration;
            yield return new WaitForSeconds(Time.deltaTime);
        }


        Typewritter.Clear();
        Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Recording done!"));

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
            Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Recording not found"));
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

            if (!result.DoesAskModifier)
            {
                // when there is nothing to ask
                Typewritter.TryEnqueueCommand(new DialogueCommand("", null, result.GetContent()));            
            }
            else
            {
                Typewritter.Clear();
                Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Waiting for modifier"));

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
                Typewritter.TryEnqueueCommand(new DialogueCommand("", null, result.GetContent(modifier)));            

                Groups.CurrentGroup = 0;
            }

            m_isTranslateRunning = false;
            m_isTranslateRunningTwice = false;
            yield break;
        }
        else
        {
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Translation not found"));
            
            m_isTranslateRunning = false;
            m_isTranslateRunningTwice = false;
            yield break;
        }
    }
}
