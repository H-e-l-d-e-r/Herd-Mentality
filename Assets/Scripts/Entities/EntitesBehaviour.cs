using DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class EntitesBehaviour : MonoBehaviour
{
    public bool IsInteractible = true;
    public UnityEvent OnInteractEvent;

    public void Interact()
    {
        if (IsInteractible)
        {
            OnInteract();
            OnInteractEvent.Invoke();        
        }
    }

    protected abstract void OnInteract();
}
