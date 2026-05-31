using System.Collections;
using DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class FrequencyBehaviour : RadioComponentBehaviour<float>, IScrollHandler
{
    [Header("Components")]
    public Slider Slider;

    [Header("Parameters")]
    public float MinFrequency;
    public float MaxFrequency;

    [Range(0f, 1f)]
    public float Smoothing = 0.5f;

    void Start()
    {
        Slider.onValueChanged.AddListener((value) => SetValue(value));

        Dialogue.Instance.OnDialogueStartEvent += () =>
        {
            Slider.enabled = false;
        };

        Dialogue.Instance.OnDialogueCloseEvent += () =>
        {
            Slider.enabled = true;
        };

        SetValue(Default);
    }

    public void OnScroll(PointerEventData eventData)
    {
        Slider.value += eventData.scrollDelta.y * (Increment / MaxFrequency);
    }

    public override void SetValue(float value)
    {
        value = Mathf.Lerp(MinFrequency, MaxFrequency, value);

        base.SetValue(value);

        OnValueChangeAsString.Invoke(((int)Value).ToString());
    }

    
}