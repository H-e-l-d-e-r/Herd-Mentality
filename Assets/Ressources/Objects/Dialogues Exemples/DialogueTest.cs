
using System.Linq;
using DialogueSystem;

using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public DialogueTable DialogueTable;

    private DialoguePtr m_ptr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_ptr = Dialogue.RegisterDialogue(DialogueTable);
        Dialogue.PlayDialogue(m_ptr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
