using UnityEngine;

public abstract class RadioComponentBehaviour<T> : MonoBehaviour
{
    [SerializeField]
    public T DefaultValue;

    public T Value => m_value;

    protected T m_value;

    protected void CreateBehaviouu()
    {
        m_value = DefaultValue;
    }
}
