using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDropBehaviour<T> : MonoBehaviour, IDropHandler where T : Component
{
    public T Content
    {
        get => m_attachedContent; 
        private set => m_attachedContent = value;
    }

    public bool IsInteractible;

    public UnityEvent<T> OnDropEvent;

    [SerializeField]
    [ReadOnly]
    private T m_attachedContent;

    // drop callback
    public void OnDrop(PointerEventData eventData)
    {
        if (IsInteractible)
        {
            AttachGameObject(eventData.pointerDrag);
        }
    }

    public void AttachTransform(Transform transform) => AttachGameObject(transform.gameObject);

    /// <summary>
    /// link a game object to the drop area
    /// </summary>
    /// <param name="object"></param>
    public void AttachGameObject(GameObject @object)
    {
        // check for a correct object
        if(@object.TryGetComponent<UIDragBehaviour>(out UIDragBehaviour drag))
        {
            // if something is already attached
            if(m_attachedContent != null)
            {
                // clear previous object attachment
                m_attachedContent.transform.SetParent(drag.OriginParent, false);
                m_attachedContent.transform.localScale = Vector3.one;
                m_attachedContent.transform.localPosition = Vector3.zero;
                m_attachedContent.transform.localRotation = Quaternion.identity;
            }            

            drag.transform.SetParent(transform, false);
            drag.transform.localScale = Vector3.one;
            drag.transform.localPosition = Vector3.zero;
            drag.transform.localRotation = Quaternion.identity;
            m_attachedContent = drag as T;

            OnDropEvent.Invoke(m_attachedContent);
            OnContentChange(m_attachedContent);
        }
    }

    public virtual void OnContentChange(T newValue) {}
}