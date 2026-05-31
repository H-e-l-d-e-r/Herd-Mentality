using DialogueSystem;
using UnityEngine;

public class EndDialogue : MonoBehaviour
{
    public DialogueTable EndTable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Dialogue.PlayDialogue(Dialogue.RegisterDialogue(EndTable));        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
