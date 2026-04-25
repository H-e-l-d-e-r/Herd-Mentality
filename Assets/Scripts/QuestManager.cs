using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UnityEngine;

[DefaultExecutionOrder(2400)]
public class QuestManager : MonoBehaviour 
{
    public UILibraryManager Library;
    public RadioManager RadioManager;

    public QuestObject Quest { get; private set; }
    public bool IsComplete
    {
        get => RadioManager.FindSequences(Quest.ConstraintVinyles).ContainsSubSequence(RadioManager.FindSequences());
    }

    private DialoguePtr m_questDialoguePtr = DialoguePtr.k_INVALID; 
    private DialoguePtr m_questEndDialoguePtr = DialoguePtr.k_INVALID; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NullComponents.ThrowIfNull(Library);
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    /// <summary>
    /// Get the next quest
    /// </summary>
    /// <param name="modifier">black listed underground groups</param>
    /// <returns></returns>
    public QuestObject GetNextQuest(CollectibleObject.UndergroundGroups modifier, bool reset = false)
    {        
        // si on doit piocher une nouvelle trame narrative
        if(Quest == null || Quest.Next == null || reset)
        {
            // determine les groupes valides
            List<QuestObject> undergroundQuest = new List<QuestObject>();
            if(!modifier.YoungLetterists) undergroundQuest.Add(GlobalGameSettings.Instance.Quests[0]);
            if(!modifier.SquatRoskoff) undergroundQuest.Add(GlobalGameSettings.Instance.Quests[1]);
            if(!modifier.Scilas) undergroundQuest.Add(GlobalGameSettings.Instance.Quests[2]);

            // add a default quest
            if(undergroundQuest.Count == 0)
            {
                undergroundQuest.Add(GlobalGameSettings.Instance.Quests[3]);
            }

            // retourne une quete random dans la liste
            return undergroundQuest[Random.Range(0, undergroundQuest.Count)];
        }

        // prend la quete suivante
        return Quest.Next;
    }

    public void StartCurrentQuest()
    {
        if(Quest == null)
        {
            Quest = GetNextQuest(new CollectibleObject.UndergroundGroups());
            CreateDialogueContext();
        }
        
        // if the quest dialogue exists
        if(m_questDialoguePtr != DialoguePtr.k_INVALID)
        {
            Dialogue.PlayDialogue(m_questDialoguePtr);
        }
    }

    public void FinishCurrentQuest()
    {
        // if the quest dialogue exists
        if(m_questEndDialoguePtr != DialoguePtr.k_INVALID)
        {
            Dialogue.PlayDialogue(m_questEndDialoguePtr);
        }

        if(Quest.Next == null && IsComplete)
        {
            GameManager.Instance.Statistics.NarrativeDone.YoungLetterists |= Quest.Like.YoungLetterists; 
            GameManager.Instance.Statistics.NarrativeDone.SquatRoskoff |= Quest.Like.SquatRoskoff; 
            GameManager.Instance.Statistics.NarrativeDone.Scilas |= Quest.Like.Scilas; 
        }

        // if the player has validated the music sequence
        // it will go to the next one
        // otherwise, it will not
        Quest = GetNextQuest(GameManager.Instance.Statistics.NarrativeDone, !IsComplete);
        CreateDialogueContext();
    }

    void CreateDialogueContext()
    {
        m_questDialoguePtr = DialoguePtr.k_INVALID; 
        m_questEndDialoguePtr = DialoguePtr.k_INVALID; 

        // create intro dialogue
        if (Quest.IntroductionTable)
        {
            m_questDialoguePtr = Dialogue.RegisterDialogue(Quest.IntroductionTable);
        }
        else
        {
            Debug.Log("failed to create the introduction table");
        }

        // create end dialogue
        if (Quest.EndTable)
        {
            m_questEndDialoguePtr = Dialogue.RegisterDialogue(Quest.EndTable);
        }

        // load constraints
        for (int i = 0; i < Quest.ConstraintVinyles.Length; i++)
        {
            // loop until the last one
            if(Quest.ConstraintVinyles[i] == null)
            {
                break;
            }

            // set the slot to be constrained
            Library.VinylsDropZones[i].IsInteractible = false;
            Library.VinylsDropZones[i].AttachGameObject(Library.FindVinyle(Quest.ConstraintVinyles[i]));
        }
    }
}
