using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DraggableBehaviour : MonoBehaviour
{
    public bool IsDragged;
    public RadioVinyl Vinyl; // ON STOCK LA MARCHANDISE ICI (BEENDO Z EST PAS BETE HEIN)
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       // deplacer le IsDragged dans le vinyle storage ma couille
    }

    public void SetObjectPosition(Vector3 vector)
    {
        transform.position = vector;
        IsDragged = true;
    }

    public void DestroyObject()
    {
        Destroy(gameObject); // EXPLOOSSSIIOOONNN!!! Megumin la goat, Kazuma le goat Best Ship Best Waifu Best Husbando (Yes i'm cronically online mtfcka)
    }
}