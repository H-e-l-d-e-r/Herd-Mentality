using DialogueSystem;
using UnityEngine;

public class DialogueTutorial : MonoBehaviour
{
    public DialogueTable DialogueTableTut;
    public DialogueTable Diag_Lei_2;
    public RadioBehaviour fILSDEPUTE;
    public DialogueTypewritter TypeWritter;
    public float FreqTuto;
    [Range(0f, 1f)]
    public float SignalThreshold = 0.7f; // 0.7 = 70% de précision suffisante

    private DialoguePtr Lei_Diag;
    private DialoguePtr Lei_Diag2;
    private bool m_diag2Played = false;

    void Start()
    {
        Lei_Diag = Dialogue.RegisterDialogue(DialogueTableTut);
        Lei_Diag2 = Dialogue.RegisterDialogue(Diag_Lei_2);
        Dialogue.PlayDialogue(Lei_Diag);

        fILSDEPUTE.FreqKnob.OnValueChange.AddListener(SuiteTuto);
    }

    void SuiteTuto(float frequence)
    {
        if (m_diag2Played) return;
        if (TypeWritter.HasCommand) return;

        // Gaussian identique à RadioBehaviour.UpdateSwitchFreq
        RadioBroadcastBehaviour behaviour = fILSDEPUTE.GetCurrentBroadcast();
        if (behaviour == null) return;

        float delta = Mathf.Abs(frequence - FreqTuto);
        float signal = Mathf.Exp(-Mathf.Pow(delta / (behaviour.Mask.Bandwidth / 2f), 2));

        if (signal >= SignalThreshold)
        {
            Dialogue.PlayDialogue(Lei_Diag2);
            m_diag2Played = true;
        }
    }
}