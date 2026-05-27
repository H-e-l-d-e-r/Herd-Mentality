using DialogueSystem;
using UnityEngine;

public class UIRadio : MonoBehaviour
{
    public UIGroupBehaviour Groups;

    public UILibraryManager LibraryManager;
    public UINotify Notify;

    public Transform CarnetTransform;

    void Start()
    {
        
    }

    public void ToggleCarnet()
    { 
        CarnetTransform.gameObject.SetActive(!CarnetTransform.gameObject.activeInHierarchy);
    }
}
