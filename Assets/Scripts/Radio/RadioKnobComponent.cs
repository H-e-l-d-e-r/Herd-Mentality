using UnityEngine;
using UnityEngine.EventSystems;

public class RadioKnob : RadioComponentBehaviour<float>, IDragHandler
{
    [Header("Parameters")]
    public float MinValue;
    public float MaxValue;

    [Header("Components")]
    public RectTransform Borehole;
    public RectTransform Knob;

    private float m_length;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.pressPosition - eventData.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        m_value = Mathf.Clamp(angle, MinValue, MaxValue);
    }

    private void Start()
    {
        NullComponents.ThrowIfNull(Borehole);
        NullComponents.ThrowIfNull(Knob);
        print(m_value);

        m_length = (MaxValue - MinValue) / 2;

        UpdateComponents();
    }

    private void Update()
    {
        UpdateComponents();
    }

    void UpdateComponents()
    {
        Knob.eulerAngles = new Vector3(0, 0, m_value);
    }
}
