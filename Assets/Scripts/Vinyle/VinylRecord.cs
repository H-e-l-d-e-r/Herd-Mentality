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
    private Material m_material;

    private void Start()
    {
        if (Renderer)
        {
            m_material = new Material(Renderer.material);
            Renderer.material = m_material;
        }
    }

    void OnEnable()
    {
        UpdateVinylProperties();
    }

    void Update()
    {
        
    }

    public void SetObjectPosition(Vector3 vector)
    {
        transform.position = Vector3.Lerp(
            transform.position, vector, GlobalGameSettings.Instance.LerpStrengh
        );
        
        IsDragged = true;
    }

    public void UpdateVinylProperties()
    {
        if (Vinyl != null)
        {
            gameObject.name = Vinyl.ToString();

            if (Renderer)
            {
                Renderer.material.SetColor("_MainColor", Vinyl.Color);

            }

            if (TitleText)
            {
                TitleText.text = Vinyl.Title;
            }
        }
    }

    public void Destroy()
    {
        // EXPLOOSSSIIOOONNN!!! Megumin la goat, Kazuma le goat Best Ship Best Waifu Best Husbando (Yes i'm cronically online mtfcka)
        // helder t'es un sale pedo elle est surement mineure :(((
        // Ohla ohla je ne fais pas de ca moi je parle du ship 
        Destroy(gameObject);
    }
}