using DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Object", menuName = "Herd Mentality/Quest Object")]
public class QuestObject : CollectibleObject
{
    [Header("Quest")]
    public VinylObject[] ConstraintVinyles = new VinylObject[4];

    public DialogueTable IntroductionTable;
    public QuestObject Next;

    private DialoguePtr m_dialoguePtr;

    public void StartDialogue()
    {
        m_dialoguePtr = Dialogue.RegisterDialogue(IntroductionTable);

        Dialogue.PlayDialogue(m_dialoguePtr);
    }
}
