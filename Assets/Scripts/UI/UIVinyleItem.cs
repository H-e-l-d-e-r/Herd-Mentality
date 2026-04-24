using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIVinylItem : UIDragBehaviour
{
    public VinylObject Vinyl
    {
        get => m_vinyl;
        set
        {
            m_vinyl = value;
            UpdateInformations();
        }
    }

    public Image Image;
    public GameObject Title;

    [SerializeField]
    [ReadOnly]
    private VinylObject m_vinyl;
    
    private Material m_material;

    void Awake()
    {
        // create a material copy
        if (Image)
        {
            m_material = new Material(Image.material);
            Image.material = m_material;  
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (IsInteractible)
        {
            transform.position = Vector3.Lerp(
                transform.position, eventData.position, GlobalGameSettings.Instance.LerpStrengh
            );
        }
    }

    void UpdateInformations()
    {
        if (Vinyl)
        {
            UIToolkit.SetText(Title, Vinyl.Title);

            if (m_material)
            {
                Image.sprite = Vinyl.BackgroundImage;
                Image.material.SetColor("_MainColor", Vinyl.Color);                
            }
        }
    }
}