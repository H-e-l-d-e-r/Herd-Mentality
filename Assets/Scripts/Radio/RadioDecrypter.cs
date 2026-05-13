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
    [Header("Components")]
    public Button RecordButton;
    public Button TranslateButton;
    public Slider ProgressBar;

    [Header("Behaviours")]
    public UISelectDecryptionMode DecryptionModeSelector;
    public DialogueTypewritter Typewritter;

    private SequenceObject m_recordObject;
    private bool m_isRecordRunning;
    private bool m_isTranslateRunning;

    private float m_recordDuration;
    private Coroutine m_recordRoutine;

    void Start()
    {
        NullComponents.ThrowIfNull(RecordButton);
        NullComponents.ThrowIfNull(TranslateButton);
        NullComponents.ThrowIfNull(DecryptionModeSelector);
        NullComponents.ThrowIfNull(Typewritter);

        RecordButton.onClick.AddListener(StartRecording);
        TranslateButton.onClick.AddListener(StartTranslating);
        DecryptionModeSelector.OnChange.AddListener((_) => StartTranslating());

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

    IEnumerator Translate()
    {
        if (!RadioManager.Instance.RadioBehaviour.IsOn)
        {
            yield break;
        }

        if (m_isRecordRunning)
        {
            yield break;
        }

        if (m_recordObject == null)
        {
            Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Recording not found"));
            yield break;
        }

        Typewritter.Show();

        // when start recording
        DecryptionModes mode = DecryptionModeSelector.Current;
        IEnumerable<DecryptionsResults> results = m_recordObject.Translations.Where(t => t.Mode == mode);

        // if there is a translation
        if (results.Count() > 0)
        {
            DecryptionsResults result = results.First();
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", null, result.Content));
        }
        else
        {
            Typewritter.Clear();
            Typewritter.TryEnqueueCommand(new DialogueCommand("", null, "Translation not found"));
            yield break;
        }
    }
}
