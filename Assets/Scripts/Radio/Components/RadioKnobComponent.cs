using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class RadioKnobComponent : RadioComponentBehaviour<float>, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Parameters")]
    public bool IsInfinite;
    public float MinValue;
    public float MaxValue;

    [Space]
    public bool UseAngle = true;
    public bool UseDistance = true;

    public float Sensitivity = 1.0f;
    public float Exponential;

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

        SetValue(Default);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float delta = 0f;
        float angle = 0f;
        float deltaAngle = 0f;
        float deltaDist = 0f;

        // get mouvements directions
        Vector2 curDir = eventData.position - new Vector2(transform.position.x, transform.position.y);
        Vector2 lastDir = m_lastPointPosition - new Vector2(transform.position.x, transform.position.y);
        
        // invert order or not?
        float order = -1.0f;

        // calculate de difference between the current and the last angle
        float currentAngle = Mathf.Atan2(curDir.y, curDir.x) * Mathf.Rad2Deg;
        float lastAngle = Mathf.Atan2(lastDir.y, lastDir.x) * Mathf.Rad2Deg;

        // delta angle
        angle = currentAngle - lastAngle;

        if (UseAngle)
        {
            deltaAngle = angle;
        }

        if (UseDistance)
        {
            deltaDist = Vector2.Distance(m_lastPointPosition, eventData.position) / Sensitivity;        
        }

        // calc an increment
        float exp = Mathf.Min(Mathf.Exp(delta), Exponential) / Sensitivity;

        delta = (deltaAngle + deltaDist * Mathf.Sign(angle)) * order;
        delta *= Increment;

        //m_lastPointPosition = eventData.position;

        // updates
        SetValue(Value + delta);
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

    /// <summary>
    /// Setting value overwrite.
    /// There, we add value claming and angle calculations
    /// </summary>
    /// <param name="_value"></param>
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
        Knob.localRotation = Quaternion.Euler(0, 0, -(m_totalAngles / Sensitivity));
    }
}