using DialogueSystem;
using UnityEngine;

public class UIRadio : MonoBehaviour
{
    public UIGroupBehaviour Groups;

    public UILibraryManager LibraryManager;

    [Header("Quest")]
    public Transform QuestCompletedTransform;
    public GameObject QuestCompletedText;

    [Header("End")]
    public GameObject AudimatTexte;
    public GameObject YoungLetteristsTexte;

    public GameObject EndGameCanvas;
    public GameObject EndScreenAudimat;
    public GameObject EndScreenYoungLetterists;
    public GameObject EndScreenSquatRoskoff;
    public GameObject EndScreenScilas;
    public GameObject EndScreenSequencesCount;

    void Start()
    {
        Dialogue.Instance.OnDialogueCloseEvent += DialogueCallback;
    }

    public void ShowQuestDialogue()
    {
        Groups.CurrentGroup = 0;
    }

    public void ShowPreparationScreen()
    {
        Groups.CurrentGroup = 1;
    }

    public void ShowEndScreen()
    {
        Groups.CurrentGroup = 3;

        UIToolkit.SetFormattedText(EndScreenYoungLetterists, GameManager.Instance.Statistics.AprYoungLetterists, "0");
        UIToolkit.SetFormattedText(EndScreenSquatRoskoff, GameManager.Instance.Statistics.AprSquatRoskoff, "0");
        UIToolkit.SetFormattedText(EndScreenScilas, GameManager.Instance.Statistics.AprScilas, "0");

        UIToolkit.SetFormattedText(EndScreenAudimat, GameManager.Instance.Statistics.GlobalAudimat, "0");

        UIToolkit.CloseCanvas(QuestCompletedTransform);
        //UIToolkit.SetFormattedText(EndScreenSequencesCount, m_validatedSequences.Count, "");
    }

    public void ShowQuestComplete()
    {
        UIToolkit.OpenCanvas(QuestCompletedTransform);
    }

    void DialogueCallback()
    {
        if(Groups.CurrentGroup == 0)
        {
            Groups.CurrentGroup = 1;
        }
    }
}
