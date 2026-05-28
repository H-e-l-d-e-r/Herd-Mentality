using DialogueSystem;
using UnityEngine;

public class UIRadio : MonoBehaviour
{
    public UIGroupBehaviour Groups;

    public UILibraryManager LibraryManager;
    public UINotify Notify;

    public UiNoteManager Carnet;

    void Start()
    {
        Carnet.Hide();   
    }

    public void ToggleCarnet()
    { 
        if(Carnet.IsActive) Carnet.Hide();
        else Carnet.Show();
    }
}
