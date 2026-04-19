using DialogueSystem;
using UnityEngine;

public class EntitiesBot : EntitesBehaviour
{
    public DialogueTable[] Tables;
    public bool PlayOnce;

    protected DialoguePtr m_DialoguePtr;

    void Start()
    {
        // choisit un dialogue random dans la liste.
        if(Tables.Length > 0)
        {
            int random = Random.Range(0, Tables.Length);
            m_DialoguePtr = Dialogue.RegisterDialogue(Tables[random]);
        }
    }

    protected override void OnInteract()
    {
        // si le pointeur est valide
        if (m_DialoguePtr != DialoguePtr.k_INVALID)
        {
            IsInteractible = IsInteractible && !PlayOnce;

            Dialogue.PlayDialogue(m_DialoguePtr);
        }
    }
}
