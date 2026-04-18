using TMPro;
using UnityEngine;

// [RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(MeshRenderer))]
public class VinylRecord : MonoBehaviour
{
    // ON STOCK LA MARCHANDISE ICI (BEENDO Z EST PAS BETE HEIN)
    public VinylObject Vinyl
    {
        get => m_vinyl;
        set
        {
            m_vinyl = value;
            UpdateVinylProperties();
        }
    }

    public MeshRenderer Renderer;
    public TMP_Text TitleText;

    [HideInInspector]
    public bool IsDragged;

    private VinylObject m_vinyl;

    void OnEnable()
    {
        UpdateVinylProperties();
    }

    public void SetObjectPosition(Vector3 vector)
    {
        transform.position = vector;
        IsDragged = true;
    }

    public void UpdateVinylProperties()
    {
        if(Vinyl != null)
        {
            gameObject.name = Vinyl.ToString();

            if (Renderer)
            {
                Renderer.material.color = Vinyl.Color;

            }

            if (TitleText)
            {
                TitleText.text = Vinyl.Title;            
            }
        }
    }

    public void DestroyObject()
    {
        // EXPLOOSSSIIOOONNN!!! Megumin la goat, Kazuma le goat Best Ship Best Waifu Best Husbando (Yes i'm cronically online mtfcka)
        // helder t'es un sale pedo elle est surement mineure :(((
        // Ohla ohla je ne fais pas de ca moi je parle du ship 
        Destroy(gameObject);  
    }
}