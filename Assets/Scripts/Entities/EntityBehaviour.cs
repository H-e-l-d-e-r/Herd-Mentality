using DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class EntityBehaviour : MonoBehaviour
{
    [Header("Entity Behaviour")]
    public bool IsInteractible = true;
    public Color InteractibleColor = Color.white;
    public Color DisableColor = Color.white;
    public Color HoverColor = Color.white;

    [Header("Events")]
    public UnityEvent OnInteractEvent;
    public UnityEvent OnHoverEvent;

    protected MeshRenderer p_renderer;
    protected bool p_IsHover;

    private const string m_materialColorId = "_MainColor";

    protected void StartBehaviour()
    {
        p_renderer = GetComponent<MeshRenderer>();
    }

    protected void UpdateBehaviour()
    {
        if (IsInteractible)
        {
            p_renderer.material.SetColor(m_materialColorId, p_IsHover ? HoverColor : InteractibleColor);
        }
        else
        {
            p_renderer.material.SetColor(m_materialColorId, p_IsHover ? HoverColor : DisableColor);
        }
    }   

    public void Interact()
    {
        if (IsInteractible)
        {
            OnInteract();
            OnInteractEvent.Invoke();        
        }
    }

    public void Focus()
    {
        p_IsHover = true;

        OnHover();
        OnHoverEvent.Invoke();
    }

    internal void LostFocus()
    {
        p_IsHover = false;
    }

    protected abstract void OnInteract();
    protected abstract void OnHover();
}
