using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class RadioKnobComponent : RadioComponentBehaviour<float>, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Parameters")]
    public bool IsInfinite;
    public float MinValue;
    public float MaxValue;

    [Tooltip("Multiplicateur de vitesse pour tourner le bouton. 1 = Normal, 2 = Rapide, 0.5 = Lent/Precis")]
    public float Sensitivity = 1.0f;

    [Header("Components")]
    public RectTransform Borehole;
    public RectTransform Knob;

    private const float k_ARC_H = 150f;

    private float m_totalAngles;
    private float m_previousValue;
    private Vector2 m_lastPointPosition;

    private void Start()
    {
        NullComponents.ThrowIfNull(Borehole);
        NullComponents.ThrowIfNull(Knob);

        SetValue(InitialValue);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - m_lastPointPosition.x;
        m_lastPointPosition = eventData.position;

        // create an exponential acceleration curve
        float increase = Mathf.Min((Mathf.Abs(Value - m_previousValue) + StepIncrement) * (Sensitivity / 100.0f), 15.0f);

        // C'est ici que la magie opere : on multiplie par la Sensitivity !
        float deltaDeg = deltaX * StepIncrement * increase;

        if (IsInfinite)
        {
            m_totalAngles += deltaDeg * Time.deltaTime;
            base.SetValue(Mathf.Clamp(Value + deltaDeg, MinValue, MaxValue));
        }
        else
        {
            float newAngle = Mathf.Clamp(m_totalAngles + deltaDeg, -k_ARC_H, k_ARC_H);
            deltaDeg = newAngle - m_totalAngles;
            m_totalAngles = newAngle * Time.deltaTime;

            float t = (m_totalAngles + k_ARC_H) / (k_ARC_H * 2.0f);
            base.SetValue(Mathf.Lerp(MinValue, MaxValue, t));
        }

        ApplyRotation();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // no-op
        m_previousValue = Value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_lastPointPosition = eventData.position;
        m_previousValue = Value; 
    }

    public override void SetValue(float _value)
    {
        float value;

        if (IsInfinite)
        {
            value = _value;
            m_totalAngles += (_value - Value);
        }
        else
        {
            value = Mathf.Clamp(_value, MinValue, MaxValue);

            float range = Mathf.Abs(MaxValue - MinValue) < Mathf.Epsilon ? 1f : MaxValue - MinValue;
            float t = (value - MinValue) / range;

            m_totalAngles = Mathf.Lerp(-k_ARC_H, k_ARC_H, t);
        }

        base.SetValue(value);

        ApplyRotation();
    }

    void ApplyRotation()
    {
        Knob.localRotation = Quaternion.Euler(0, 0, -m_totalAngles);
    }
}