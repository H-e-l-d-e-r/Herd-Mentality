using DialogueSystem;
using UnityEngine;

public class EntitiesBot : EntitesBehaviour
{
    public DialogueTable RefDialogues;

    protected DialoguePtr m_DialoguePtr;

    void Start()
    {
        m_DialoguePtr = Dialogue.RegisterDialogue(RefDialogues);

    }

    public override void OnInteract()
    {
        Dialogue.PlayDialogue(m_DialoguePtr);
    }
}
