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
        m_canvasGroup = GetComponent<CanvasGroup>();
        if (m_canvasGroup == null) m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(VinylObject data)
    {
        VinylData = data;
        TitleText.text = data.Title;

        // On change la couleur de l'objet UI
        BackgroundImage.color = data.Color;


        if (data.BackgroundImage != null)
        {
            BackgroundImage.sprite = data.BackgroundImage; // On applique le dessin
            BackgroundImage.color = Color.white; // On remet la couleur à blanc pour voir l'image normalement
        }
        else
        {
            // Sinon on garde juste la couleur unie par défaut
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
        transform.position = Input.mousePosition; // Suit la souris
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