using DialogueSystem;

using UnityEngine;
using UnityEngine.Events;

public class TutorielScript : MonoBehaviour
{
    [Header("Tables")]
    public DialogueTable Scenes;
    public DialogueTable Scenes1;
    public DialogueTable Tutori;
    public DialogueTable Tu;
    public DialogueTable Tutoriele;
    public DialogueTable TutlEnd;
    public DialogueTable Tutorlay;

    [Header("Event Callbacks")]
    public UnityEvent StartEvent;

    private int ScenesProgression;
    private DialoguePtr StartScene;
    //private DialoguePtr m_tutorielPreGameplay;
    //private DialoguePtr m_tutorielAntenna;
    //private DialoguePtr m_tutorielFrequencyModulator;
    //private DialoguePtr m_tutorielFrequencyForce;
    //private DialoguePtr m_tutorielEnd;
    //private DialoguePtr m_tutorielPostGameplay;

    void Start()
    {
        StartScene = Dialogue.RegisterDialogue(Scenes);
        //m_tutorielPreGameplay = Dialogue.RegisterDialogue(TutorielPreGameplay);
        //m_tutorielAntenna = Dialogue.RegisterDialogue(TutorielAntenna);
        //m_tutorielFrequencyModulator = Dialogue.RegisterDialogue(TutorielFrequencyModulator);
        //m_tutorielFrequencyForce = Dialogue.RegisterDialogue(TutorielFrequencyForce);
        //m_tutorielEnd = Dialogue.RegisterDialogue(TutorielEnd);
        //m_tutorielPostGameplay = Dialogue.RegisterDialogue(TutorielPostGameplay);    
    
        StartEvent.Invoke();    
        Dialogue.PlayDialogue(StartScene);
    }

    public void NextTutorielStep()
    {
        switch (++ScenesProgression)
        {
            case 1:
                //Dialogue.PlayDialogue(m_tutorielPreGameplay);
                break;

            case 2:
                //Dialogue.PlayDialogue(m_tutorielFrequencyModulator);
                break;
            
            case 3:
                //Dialogue.PlayDialogue(m_tutorielAntenna);
                break;
            
            case 4:
                //Dialogue.PlayDialogue(m_tutorielFrequencyForce);
                break;

            case 5:
                //Dialogue.PlayDialogue(m_tutorielEnd);
                break;

            default:
                break;
        }
    }
}
