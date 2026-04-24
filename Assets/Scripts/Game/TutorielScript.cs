using DialogueSystem;

using UnityEngine;
using UnityEngine.Events;

public class StartSceneScript : MonoBehaviour
{
    public RadioBehaviour RadioBehaviour;

    [Header("Tables")]
    public DialogueTable Introduction;
    public DialogueTable TutorielPreGameplay;
    public DialogueTable TutorielAntenna;
    public DialogueTable TutorielFrequencyModulator;
    public DialogueTable TutorielFrequencyForce;
    public DialogueTable TutorielEnd;
    public DialogueTable TutorielPostGameplay;

    [Header("Event Callbacks")]
    public UnityEvent StartEvent;
    public UnityEvent OnDialogueEnd;

    private int m_tutorielProgression;
    private DialoguePtr m_introduction;
    private DialoguePtr m_tutorielPreGameplay;
    private DialoguePtr m_tutorielAntenna;
    private DialoguePtr m_tutorielFrequencyModulator;
    private DialoguePtr m_tutorielFrequencyForce;
    private DialoguePtr m_tutorielEnd;
    private DialoguePtr m_tutorielPostGameplay;

    void Start()
    {
        m_introduction = Dialogue.RegisterDialogue(Introduction);
        m_tutorielPreGameplay = Dialogue.RegisterDialogue(TutorielPreGameplay);
        m_tutorielAntenna = Dialogue.RegisterDialogue(TutorielAntenna);
        m_tutorielFrequencyModulator = Dialogue.RegisterDialogue(TutorielFrequencyModulator);
        m_tutorielFrequencyForce = Dialogue.RegisterDialogue(TutorielFrequencyForce);
        m_tutorielEnd = Dialogue.RegisterDialogue(TutorielEnd);
        m_tutorielPostGameplay = Dialogue.RegisterDialogue(TutorielPostGameplay);    
    
        StartEvent.Invoke();    
        Dialogue.PlayDialogue(m_introduction);
        Dialogue.Instance.OnDialogueCloseEvent += () => OnDialogueEnd.Invoke();
    }

    public void NextTutorielStep()
    {
        switch (++m_tutorielProgression)
        {
            case 1:
                Dialogue.PlayDialogue(m_tutorielPreGameplay);
                break;

            case 2:
                Dialogue.PlayDialogue(m_tutorielFrequencyModulator);
                break;
            
            case 3:
                Dialogue.PlayDialogue(m_tutorielAntenna);
                break;
            
            case 4:
                Dialogue.PlayDialogue(m_tutorielFrequencyForce);
                break;

            case 5:
                Dialogue.PlayDialogue(m_tutorielEnd);
                break;

            default:
                break;
        }
    }
}
