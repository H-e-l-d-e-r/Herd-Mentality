using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIDragBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool IsInteractible;
    public Transform OriginParent;

    private Canvas m_attachedCanvas;
    private CanvasGroup m_group;

    void Start()
    {
        m_attachedCanvas = GetComponentInParent<Canvas>();
        m_group = GetComponent<CanvasGroup>();

        Debug.Assert(m_attachedCanvas);
        Debug.Assert(m_group);
    }

    // drop callback
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (IsInteractible)
        {
            transform.position = eventData.position;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (IsInteractible)
        {
            OriginParent = transform.parent;
            transform.SetParent(m_attachedCanvas.transform, false);
            transform.SetAsLastSibling();
            
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            m_group.blocksRaycasts = false;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (IsInteractible)
        {
            m_group.blocksRaycasts = true;

            // if is not in a slot
            if(transform.parent == m_attachedCanvas.transform)
            {
                transform.SetParent(OriginParent, false);

                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}