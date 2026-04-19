using DialogueSystem;
using UnityEngine;

public class EntityBot : EntityBehaviour
{
    [Header("Entity Bot")]
    public DialogueTable[] Tables;
    public bool PlayOnce;

    protected DialoguePtr m_DialoguePtr;

    void Start()
    {
        base.StartBehaviour();

        // choisit un dialogue random dans la liste.
        if(Tables.Length > 0)
        {
            int random = Random.Range(0, Tables.Length);
            m_DialoguePtr = Dialogue.RegisterDialogue(Tables[random]);
        }
    }

    void Update()
    {
        base.UpdateBehaviour();  
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

    protected override void OnHover()
    {
        
    }
}
