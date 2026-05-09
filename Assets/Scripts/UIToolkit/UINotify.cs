using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINotify : MonoBehaviour
{
    public CanvasGroup GroupComponent;
    public TMP_Text TextComponent;

    public string Text
    {
        get => m_content;
        set
        {
            if(TextComponent)
            {
                TextComponent.text = value;
            }

            m_content = value;
        }
    }

    public float EaseInDuration;
    public float EaseOutDuration;

    private string m_content;
    private float m_remaingTime;
    private float m_duration;

    void OnEnable()
    {
        GroupComponent.alpha = 0f;
        Text = string.Empty;
        m_remaingTime = 0f;
        m_duration = 0f;
    }

    void Update()
    {
        // reset 
        GroupComponent.alpha = 0f;

        UpdateNotify();
    }

    void UpdateNotify()
    {
        // when finished
        if(m_remaingTime <= Mathf.Epsilon)
        {
            Text = string.Empty;
            m_remaingTime = 0.0f;
            m_duration = 0.0f;

            return;
        }

        // calculate easing
        float elapsed = m_duration - m_remaingTime;

        // when appears
        float easeIn = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0, EaseInDuration, elapsed));
        
        // when desappears
        float easeOut = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(m_duration - EaseOutDuration, m_duration, elapsed));
 
        // setting the alpha based on easing and timers
        GroupComponent.alpha = 1.0f * easeIn * easeOut;

        // cooldown
        m_remaingTime -= Time.deltaTime;
    }

    public void Notify(string message, float duration)
    {
        if (!TextComponent)
        {
            Debug.Log("missing component! failed to update text");
            return;
        }

        // update content and duration
        Text = message;
        m_remaingTime = duration;
        m_duration = duration;
    }
}
