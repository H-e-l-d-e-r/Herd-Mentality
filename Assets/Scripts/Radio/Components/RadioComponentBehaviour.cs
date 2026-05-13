using System;
using System.Numerics;

using UnityEngine;
using UnityEngine.Events;

public abstract class RadioComponentBehaviour<T> : MonoBehaviour
{
    [SerializeField]
    [Header("Radio Component Parent")]
    public T Default;
    public T Increment;

    public T Value => m_value;

    public UnityEvent<T> OnValueChange;
    public UnityEvent<string> OnValueChangeAsString;
 
    private T m_value;

    protected void Reset()
    {
        SetValue(Default);
    }

    public virtual void SetValue(T value)
    {
        m_value = value;
        OnValueChange.Invoke(m_value);
        OnValueChangeAsString.Invoke(m_value.ToString());
    }
}
