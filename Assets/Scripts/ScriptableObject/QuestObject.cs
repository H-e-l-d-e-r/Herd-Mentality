using DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Object", menuName = "Herd Mentality/Quest Object")]
public class QuestObject : CollectibleObject
{
    [Header("Quest")]
    public VinylObject[] ConstraintVinyles = new VinylObject[4];

    public DialogueTable IntroductionTable;
    public DialogueTable EndTable;
    public QuestObject Next;

    private DialoguePtr m_dialoguePtr;
    private DialoguePtr m_dialogueEndPtr;

    public void StartDialogue()
    {
        m_dialoguePtr = Dialogue.RegisterDialogue(IntroductionTable);
        m_dialogueEndPtr = DialoguePtr.k_INVALID;

        if (EndTable)
        {
            m_dialogueEndPtr = Dialogue.RegisterDialogue(EndTable);
        }

        Dialogue.PlayDialogue(m_dialoguePtr);
    }

    public void StartEndDialogue()
    {
        if(m_dialogueEndPtr != DialoguePtr.k_INVALID)
        {
            Dialogue.PlayDialogue(m_dialogueEndPtr);
        }
    }
}
