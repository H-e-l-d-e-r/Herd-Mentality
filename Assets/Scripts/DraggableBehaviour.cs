using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DraggableBehaviour : MonoBehaviour
{
    public bool IsDragged;
    public RadioVinyl Vinyl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        IsDragged = false;
    }

    public void SetObjectPosition(Vector3 vector)
    {
        transform.position = vector;
        IsDragged = true;
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
