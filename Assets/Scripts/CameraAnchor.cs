using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    public bool IsCameraAttached => transform.GetComponentsInChildren<Camera>().Length > 0;
    public Camera Camera => transform.GetComponentInChildren<Camera>();

    public void Focus(Camera camera)
    {
        camera.transform.SetParent(transform);

        camera.transform.position = transform.position;
        camera.transform.rotation = transform.rotation;
    }
}
