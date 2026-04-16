using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class UIVinylItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public VinylObject VinylData; 

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

        // 2. AUTO-RÉPARATION DE L'IMAGE (Fini la ligne 29 !)
        if (BackgroundImage == null)
        {
            BackgroundImage = GetComponentInChildren<Image>(); // Cherche s'il en reste une cachée

            // Si vraiment il n'y a plus AUCUNE image sur ton Prefab, le script en crée une !
            if (BackgroundImage == null)
            {
                BackgroundImage = gameObject.AddComponent<Image>();
            }
        }
    }

    public void Setup(VinylObject data)
    {
        if (data == null) return; // Sécurité

        VinylData = data;

        if (TitleText != null)
        {
            TitleText.text = data.Title;
        }

        // LA FAMEUSE LIGNE 29 (Qui ne plantera plus jamais)
        // Maintenant on est sûr à 1000% que BackgroundImage existe grâce au Awake
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

        // On se met en toute dernière position pour être dessiné au-dessus de tout le reste
        transform.SetAsLastSibling();

        // On désactive le raycast pour que la souris puisse cliquer sur la zone de drop en dessous
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

        // Si on le lâche dans le vide il retourne à sa place d'origine
        if (transform.parent == transform.root)
        {
            transform.SetParent(m_originalParent);
        }
    }
}