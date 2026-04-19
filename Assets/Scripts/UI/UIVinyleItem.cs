using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class UIVinylItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] 
    public VinylObject VinylData; 

    public TMP_Text TitleText;

    public Image BackgroundImage;

    private Transform m_originalParent;
    private CanvasGroup m_canvasGroup;

    void Awake()
    {
        // 1. Gestion du CanvasGroup
        m_canvasGroup = GetComponent<CanvasGroup>();
        if (m_canvasGroup == null)
        {
            m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

       
        if (BackgroundImage == null)
        {
            BackgroundImage = GetComponentInChildren<Image>(); // Cherche s'il en reste une cach�e

            // Si vraiment il n'y a plus dímage sur ton Prefab le script en cree une
            if (BackgroundImage == null)
            {
                BackgroundImage = gameObject.AddComponent<Image>();
            }
        }
    }

    public void Setup(VinylObject data)
    {
        if (data == null) return; // au cas ou

        VinylData = data;

        if (TitleText != null)
        {
            TitleText.text = data.Title;
        }

        
        // on est sur que BackgroundImage existe grace au Awake
        if (data.BackgroundImage != null)
        {
            BackgroundImage.sprite = data.BackgroundImage;
            BackgroundImage.color = Color.white;
        }
        else
        {
            BackgroundImage.sprite = null;
            BackgroundImage.color = data.Color;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_originalParent = transform.parent;

        // NOUVEAU : On trouve le Canvas le plus proche et on s'y attache !
        Canvas mainCanvas = GetComponentInParent<Canvas>();
        transform.SetParent(mainCanvas.transform);

        // On se met en toute derni�re position pour �tre dessin� au-dessus de tout le reste
        transform.SetAsLastSibling();

        // On d�sactive le raycast pour que la souris puisse cliquer sur la zone de drop en dessous
        m_canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // On utilise eventData.position au lieu de Input.mousePosition
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_canvasGroup.blocksRaycasts = true;

        // Si on le lache dans le vide il retourne a sa place d'origine
        if (transform.parent == transform.root)
        {
            transform.SetParent(m_originalParent);
        }
    }
}