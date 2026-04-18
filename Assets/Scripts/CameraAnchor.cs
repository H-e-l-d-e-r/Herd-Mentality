using UnityEngine;
using UnityEngine.Events;

public class CameraAnchor : MonoBehaviour
{
    public bool IsCameraAttached => transform.GetComponentsInChildren<Camera>().Length > 0;
    public Camera Camera => transform.GetComponentInChildren<Camera>();

    public UnityEvent OnFocus;
    public UnityEvent OnFocusOnce;

    private bool m_once = false;

    public void Focus(Camera camera)
    {
        camera.transform.SetParent(transform);

        camera.transform.position = transform.position;
        camera.transform.rotation = transform.rotation;

        OnFocus.Invoke();

        if (!m_once)
        {
            m_once = true;
            OnFocusOnce.Invoke();
        }
    }
}
